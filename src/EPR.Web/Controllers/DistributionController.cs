using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EPR.Web.Attributes;
using EPR.Data;
using EPR.Domain.Entities;
using EPR.Web.Services;
using System.Text.Json;
using Microsoft.Data.Sqlite;

namespace EPR.Web.Controllers;

[Authorize]
public class DistributionController : Controller
{
    private readonly EPRDbContext _context;
    private readonly AsnParserService _asnParser;
    private readonly IDatasetService _datasetService;
    // private readonly AsnChainService _chainService; // Disabled until migration is run
    private readonly ILogger<DistributionController> _logger;

    public DistributionController(
        EPRDbContext context,
        AsnParserService asnParser,
        IDatasetService datasetService,
        // AsnChainService chainService, // Disabled until migration is run
        ILogger<DistributionController> logger)
    {
        _context = context;
        _asnParser = asnParser;
        _datasetService = datasetService;
        // _chainService = chainService; // Disabled until migration is run
        _logger = logger;
    }

    public IActionResult Index(int? id)
    {
        ViewBag.AsnId = id;
        return View();
    }

    /// <summary>
    /// Get all ASN shipments
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetAsnShipments()
    {
        Response.Headers["Cache-Control"] = "no-store, no-cache, must-revalidate, proxy-revalidate";
        Response.Headers["Pragma"] = "no-cache";
        Response.Headers["Expires"] = "0";
        try
        {
            _logger.LogInformation("GetAsnShipments called");

            var datasetKey = _datasetService.GetCurrentDataset();
            // Default to Electronics for Distribution when no dataset selected (one of three integrated datasets)
            if (string.IsNullOrEmpty(datasetKey))
                datasetKey = "Electronics";
            var requiresDataset = false;

            var shipmentsQuery = _context.AsnShipments
                .Include(s => s.Pallets)
                .ThenInclude(p => p.LineItems)
                .AsQueryable();
            shipmentsQuery = shipmentsQuery.Where(s => s.DatasetKey == datasetKey);
            
            // Use cancellation token with timeout to prevent hanging
            using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(30));
            var shipments = await shipmentsQuery
                .OrderByDescending(s => s.ShipDate)
                .ToListAsync(cts.Token);

            // Lazy-seed integrated dataset when none exist
            var integratedDatasets = new[] { "Electronics", "Alcoholic Beverages", "Confectionary", "Fresh Produce", "Garden", "Homewares", "Personal Care", "Pet Care", "Pharmaceuticals" };
            if (shipments.Count == 0 && integratedDatasets.Contains(datasetKey))
            {
                try
                {
                    if (datasetKey == "Electronics")
                    {
                        var electronicsSeeder = new EPR.Web.Seeders.ElectronicsDatasetSeeder(_context);
                        await electronicsSeeder.SeedAsync();
                    }
                    else if (datasetKey == "Alcoholic Beverages")
                    {
                        var alcSeeder = new EPR.Web.Seeders.AlcoholicBeveragesDatasetSeeder(_context);
                        await alcSeeder.SeedAsync();
                    }
                    else if (datasetKey == "Confectionary")
                    {
                        var confSeeder = new EPR.Web.Seeders.ConfectionaryDatasetSeeder(_context);
                        await confSeeder.SeedAsync();
                    }
                    else if (datasetKey == "Fresh Produce")
                    {
                        var fpSeeder = new EPR.Web.Seeders.FreshProduceDatasetSeeder(_context);
                        await fpSeeder.SeedAsync();
                    }
                    else if (datasetKey == "Garden")
                    {
                        var gdnSeeder = new EPR.Web.Seeders.GardenDatasetSeeder(_context);
                        await gdnSeeder.SeedAsync();
                    }
                    else if (datasetKey == "Homewares")
                    {
                        var hwSeeder = new EPR.Web.Seeders.HomewaresDatasetSeeder(_context);
                        await hwSeeder.SeedAsync();
                    }
                    else if (datasetKey == "Personal Care")
                    {
                        var pcSeeder = new EPR.Web.Seeders.PersonalCareDatasetSeeder(_context);
                        await pcSeeder.SeedAsync();
                    }
                    else if (datasetKey == "Pet Care")
                    {
                        var petSeeder = new EPR.Web.Seeders.PetCareDatasetSeeder(_context);
                        await petSeeder.SeedAsync();
                    }
                    else if (datasetKey == "Pharmaceuticals")
                    {
                        var pharmSeeder = new EPR.Web.Seeders.PharmaceuticalsDatasetSeeder(_context);
                        await pharmSeeder.SeedAsync();
                    }
                    shipmentsQuery = _context.AsnShipments
                        .Include(s => s.Pallets)
                        .ThenInclude(p => p.LineItems)
                        .Where(s => s.DatasetKey == datasetKey);
                    shipments = await shipmentsQuery
                        .OrderByDescending(s => s.ShipDate)
                        .ToListAsync(cts.Token);
                    _logger.LogInformation("Lazy-seeded {Dataset} ASNs, retrieved {Count} shipments", datasetKey, shipments.Count);
                }
                catch (Exception seedEx)
                {
                    _logger.LogWarning(seedEx, "Lazy {Dataset} ASN seed failed", datasetKey);
                }
            }

            _logger.LogInformation("Retrieved {Count} shipments from database", shipments.Count);

            // Process the data in memory to avoid SQL APPLY operation
            var result = shipments.Select(s => new
            {
                s.Id,
                s.AsnNumber,
                s.ShipperName,
                s.ReceiverName,
                s.ShipDate,
                s.DeliveryDate,
                s.CarrierName,
                s.VehicleRegistration,
                s.TotalPackages,
                s.TotalWeight,
                s.Status,
                s.SourceFormat,
                s.ImportedAt,
                IsSimulated = s.IsSimulated, // Handle gracefully if column doesn't exist
                PalletCount = s.Pallets?.Count ?? 0,
                TotalItems = s.Pallets?.Sum(p => p.LineItems?.Count ?? 0) ?? 0,
                Destinations = s.Pallets != null && s.Pallets.Any(p => !string.IsNullOrEmpty(p.DestinationName))
                    ? s.Pallets
                        .Where(p => !string.IsNullOrEmpty(p.DestinationName))
                        .Select(p => new { 
                            DestinationName = p.DestinationName ?? "Unknown", 
                            DestinationCity = p.DestinationCity ?? "", 
                            DestinationCountryCode = p.DestinationCountryCode ?? "" 
                        })
                        .GroupBy(d => new { d.DestinationName, d.DestinationCity, d.DestinationCountryCode })
                        .Select(g => (object)g.Key)
                        .ToList()
                    : new List<object>(),
                // Chain information (disabled for now until migration is run)
                ParentAsnId = (int?)null,
                ParentAsnNumber = (string?)null,
                ChildAsnCount = 0,
                IsChained = false
            }).ToList();

            _logger.LogInformation($"Returning {result.Count} shipments to client");

            Response.ContentType = "application/json";
            return Json(new { success = true, data = result, requiresDataset = requiresDataset });
        }
        catch (OperationCanceledException)
        {
            _logger.LogWarning("GetAsnShipments request timed out");
            return Json(new { success = false, message = "Request timed out. Please try again." });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting ASN shipments");
            return Json(new { success = false, message = ex.Message });
        }
    }

    /// <summary>
    /// Get single ASN shipment with full details
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetAsnShipment(int id)
    {
        Response.Headers["Cache-Control"] = "no-store, no-cache, must-revalidate, proxy-revalidate";
        Response.Headers["Pragma"] = "no-cache";
        Response.Headers["Expires"] = "0";
        try
        {
            var datasetKey = _datasetService.GetCurrentDataset();
            var query = _context.AsnShipments
                .Include(s => s.Pallets)
                .ThenInclude(p => p.LineItems)
                .AsQueryable();
            if (!string.IsNullOrEmpty(datasetKey))
                query = query.Where(s => s.DatasetKey == datasetKey);
            else
                query = query.Where(s => false); // Require dataset selection
            var shipment = await query.FirstOrDefaultAsync(s => s.Id == id);

            if (shipment == null)
            {
                return Json(new { success = false, message = "ASN shipment not found" });
            }

            // Use stored RawData or generate a summary for manual/dummy data (so Raw Data tab always has content)
            var rawData = shipment.RawData;
            if (string.IsNullOrWhiteSpace(rawData))
            {
                rawData = GenerateRawDataSummary(shipment);
            }

            // Resolve ProductId for each line item by GTIN (normalize to 14 digits for matching)
            static string NormalizeGtin(string? g) => string.IsNullOrWhiteSpace(g) ? "" : g.Trim().PadLeft(14, '0').Length > 14 ? g.Trim() : g.Trim().PadLeft(14, '0');
            var lineItemGtins = shipment.Pallets.SelectMany(p => p.LineItems.Select(li => new { li.Gtin, Normalized = NormalizeGtin(li.Gtin) })).Where(x => !string.IsNullOrWhiteSpace(x.Gtin)).DistinctBy(x => x.Normalized).ToList();
            var gtinToProductId = new Dictionary<string, int>(StringComparer.Ordinal);
            if (lineItemGtins.Count > 0)
            {
                var productQuery = _context.Products.Where(x => x.Gtin != null);
            if (!string.IsNullOrEmpty(datasetKey))
                productQuery = productQuery.Where(x => x.DatasetKey == datasetKey);
            var products = await productQuery.Select(x => new { x.Gtin, x.Id }).ToListAsync();
                foreach (var li in lineItemGtins)
                {
                    var product = products.FirstOrDefault(p => NormalizeGtin(p.Gtin) == li.Normalized);
                    if (product != null && li.Gtin != null)
                        gtinToProductId[li.Gtin] = product.Id;
                }
            }

            // Map to DTO to avoid circular reference issues
            var result = new
            {
                shipment.Id,
                shipment.AsnNumber,
                shipment.ShipperGln,
                shipment.ShipperName,
                shipment.ShipperAddress,
                shipment.ShipperCity,
                shipment.ShipperPostalCode,
                shipment.ShipperCountryCode,
                shipment.ReceiverGln,
                shipment.ReceiverName,
                shipment.ShipDate,
                shipment.DeliveryDate,
                shipment.PoReference,
                shipment.CarrierName,
                shipment.TransportMode,
                shipment.VehicleRegistration,
                shipment.TotalWeight,
                shipment.TotalPackages,
                shipment.SourceFormat,
                RawData = rawData,
                shipment.ImportedAt,
                shipment.Status,
                shipment.IsSimulated,
                Pallets = shipment.Pallets.Select(p => new
                {
                    p.Id,
                    p.SequenceNumber,
                    p.Sscc,
                    p.PackageTypeCode,
                    p.GrossWeight,
                    p.DestinationGln,
                    p.DestinationName,
                    p.DestinationAddress,
                    p.DestinationCity,
                    p.DestinationPostalCode,
                    p.DestinationCountryCode,
                    p.IsSimulated,
                    LineItems = p.LineItems.Select(li => new
                    {
                        li.Id,
                        li.LineNumber,
                        li.Gtin,
                        li.Description,
                        li.Quantity,
                        li.UnitOfMeasure,
                        li.BatchNumber,
                        li.BestBeforeDate,
                        li.PoLineReference,
                        li.SupplierArticleNumber,
                        li.NetWeight,
                        li.IsSimulated,
                        ProductId = gtinToProductId.TryGetValue(li.Gtin ?? "", out var pid) ? pid : (int?)null
                    }).ToList()
                }).ToList()
            };

            Response.ContentType = "application/json";
            return Json(new { success = true, data = result });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting ASN shipment {Id}", id);
            return Json(new { success = false, message = ex.Message });
        }
    }

    /// <summary>
    /// Generate a readable raw data summary for shipments that have no stored RawData (e.g. dummy/manual ASNs).
    /// </summary>
    private static string GenerateRawDataSummary(AsnShipment s)
    {
        var summary = new
        {
            _note = "Generated summary (no original ASN file). Source: " + (s.SourceFormat ?? "MANUAL"),
            AsnNumber = s.AsnNumber,
            Shipper = new { s.ShipperGln, s.ShipperName, s.ShipperAddress, s.ShipperCity, s.ShipperPostalCode, s.ShipperCountryCode },
            Receiver = new { s.ReceiverGln, s.ReceiverName },
            ShipDate = s.ShipDate,
            DeliveryDate = s.DeliveryDate,
            Carrier = s.CarrierName,
            Vehicle = s.VehicleRegistration,
            TotalWeight = s.TotalWeight,
            TotalPackages = s.TotalPackages,
            Status = s.Status,
            IsSimulated = s.IsSimulated,
            Pallets = (s.Pallets ?? new List<AsnPallet>()).Select(p => new
            {
                p.Sscc,
                p.DestinationName,
                p.DestinationCity,
                p.DestinationCountryCode,
                p.GrossWeight,
                LineItems = (p.LineItems ?? new List<AsnLineItem>()).Select(li => new { li.Gtin, li.Description, li.Quantity, li.UnitOfMeasure }).ToList()
            }).ToList()
        };
        return JsonSerializer.Serialize(summary, new JsonSerializerOptions { WriteIndented = true });
    }

    /// <summary>
    /// Upload and parse ASN file
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> UploadAsn(IFormFile file)
    {
        try
        {
            if (file == null || file.Length == 0)
            {
                return Json(new { success = false, message = "No file uploaded" });
            }

            string content;
            using (var reader = new StreamReader(file.OpenReadStream()))
            {
                content = await reader.ReadToEndAsync();
            }

            // Parse ASN
            var shipment = _asnParser.ParseAsn(content);

            // Check if ASN already exists
            var existing = await _context.AsnShipments
                .FirstOrDefaultAsync(s => s.AsnNumber == shipment.AsnNumber);

            if (existing != null)
            {
                return Json(new { 
                    success = false, 
                    message = $"ASN {shipment.AsnNumber} already exists in the system" 
                });
            }

            // Save to database
            _context.AsnShipments.Add(shipment);
            await _context.SaveChangesAsync();

            // Auto-detect and link chains (disabled until migration is run)
            // var linksCreated = await _chainService.DetectAndLinkChains(shipment.Id);
            // if (linksCreated > 0)
            // {
            //     _logger.LogInformation(
            //         "ASN {AsnNumber} linked to {LinkCount} other ASNs in the chain",
            //         shipment.AsnNumber, linksCreated);
            // }

            _logger.LogInformation(
                "ASN {AsnNumber} imported successfully with {PalletCount} pallets and {ItemCount} items",
                shipment.AsnNumber, shipment.Pallets.Count, 
                shipment.Pallets.Sum(p => p.LineItems.Count));

            return Json(new { 
                success = true, 
                message = $"ASN {shipment.AsnNumber} imported successfully",
                data = new { id = shipment.Id, asnNumber = shipment.AsnNumber }
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error uploading ASN file");
            return Json(new { success = false, message = $"Error: {ex.Message}" });
        }
    }

    /// <summary>
    /// Update ASN shipment
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> UpdateAsnShipment([FromBody] AsnShipment updatedShipment)
    {
        try
        {
            var shipment = await _context.AsnShipments
                .Include(s => s.Pallets)
                .ThenInclude(p => p.LineItems)
                .FirstOrDefaultAsync(s => s.Id == updatedShipment.Id);

            if (shipment == null)
            {
                return Json(new { success = false, message = "ASN shipment not found" });
            }

            // Update basic properties
            shipment.Status = updatedShipment.Status;
            shipment.VehicleRegistration = updatedShipment.VehicleRegistration;
            shipment.CarrierName = updatedShipment.CarrierName;
            shipment.DeliveryDate = updatedShipment.DeliveryDate;

            await _context.SaveChangesAsync();

            return Json(new { success = true, message = "ASN updated successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating ASN shipment {Id}", updatedShipment.Id);
            return Json(new { success = false, message = ex.Message });
        }
    }

    /// <summary>
    /// Delete ASN shipment
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> DeleteAsnShipment(int id)
    {
        try
        {
            var shipment = await _context.AsnShipments.FindAsync(id);
            
            if (shipment == null)
            {
                return Json(new { success = false, message = "ASN shipment not found" });
            }

            _context.AsnShipments.Remove(shipment);
            await _context.SaveChangesAsync();

            _logger.LogInformation("ASN {AsnNumber} deleted", shipment.AsnNumber);

            return Json(new { success = true, message = "ASN deleted successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting ASN shipment {Id}", id);
            return Json(new { success = false, message = ex.Message });
        }
    }

    /// <summary>
    /// Get product packaging data by GTIN for Visual View
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetProductPackagingByGtin(string gtin)
    {
        try
        {
            if (string.IsNullOrEmpty(gtin))
            {
                return Json(new { success = false, message = "GTIN is required" });
            }

            // Try to find product by GTIN (we'll add GTIN to Product later, for now use dummy data)
            // For now, return dummy packaging structure
            var packagingData = new
            {
                gtin = gtin,
                productName = $"Product for GTIN {gtin}",
                found = false, // Will be true when we have real data
                packagingFlow = new
                {
                    rawMaterials = new[]
                    {
                        new { id = 1, name = "PET Plastic", type = "Raw Material" },
                        new { id = 2, name = "Paper Label", type = "Raw Material" }
                    },
                    secondaryPackaging = new[]
                    {
                        new { id = 1, name = "Bottle", unitLevel = "Secondary", materials = new[] { 1 } },
                        new { id = 2, name = "Label", unitLevel = "Secondary", materials = new[] { 2 } }
                    },
                    tertiaryPackaging = new[]
                    {
                        new { id = 1, name = "Case", unitLevel = "Tertiary" }
                    }
                }
            };

            Response.Headers["Cache-Control"] = "no-store, no-cache, must-revalidate, proxy-revalidate";
            Response.Headers["Pragma"] = "no-cache";
            Response.Headers["Expires"] = "0";
            Response.ContentType = "application/json";
            return Json(new { success = true, data = packagingData });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting product packaging for GTIN {Gtin}", gtin);
            return Json(new { success = false, message = ex.Message });
        }
    }

    /// <summary>
    /// Get all product packaging data for an ASN (all GTINs)
    /// Returns the full packaging hierarchy: Products → PackagingUnits → PackagingTypes → RawMaterials
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetAsnProductPackaging(int asnId)
    {
        Response.Headers["Cache-Control"] = "no-store, no-cache, must-revalidate, proxy-revalidate";
        Response.Headers["Pragma"] = "no-cache";
        Response.Headers["Expires"] = "0";
        try
        {
            var datasetKey = _datasetService.GetCurrentDataset();
            var query = _context.AsnShipments
                .Include(s => s.Pallets)
                .ThenInclude(p => p.LineItems)
                .AsQueryable();
            if (!string.IsNullOrEmpty(datasetKey))
                query = query.Where(s => s.DatasetKey == datasetKey);
            else
                query = query.Where(s => false);
            var shipment = await query.FirstOrDefaultAsync(s => s.Id == asnId);

            if (shipment == null)
            {
                return Json(new { success = false, message = "ASN shipment not found" });
            }

            // Get all unique GTINs and line items from the ASN
            var lineItems = shipment.Pallets
                .SelectMany(p => p.LineItems)
                .Where(li => !string.IsNullOrEmpty(li.Gtin))
                .ToList();

            var gtins = lineItems
                .Select(li => li.Gtin)
                .Distinct()
                .ToList();

            // Get all products and their packaging data
            var allProducts = await _context.Products
                .Include(p => p.ProductPackagings)
                    .ThenInclude(pp => pp.PackagingUnit)
                        .ThenInclude(pu => pu.Items)
                            .ThenInclude(pui => pui.PackagingType)
                                .ThenInclude(pt => pt.Materials)
                                    .ThenInclude(ptm => ptm.Material)
                .ToListAsync();

            // Build packaging data for each GTIN
            var productPackagingData = new List<object>();
            string NormGtin(string? g) => string.IsNullOrWhiteSpace(g) ? "" : (g.Trim().Length > 14 ? g.Trim() : g.Trim().PadLeft(14, '0'));

            foreach (var gtin in gtins)
            {
                var normalizedGtin = NormGtin(gtin);
                var product = allProducts.FirstOrDefault(p => 
                    (p.Gtin != null && NormGtin(p.Gtin) == normalizedGtin) ||
                    p.Sku == gtin || 
                    p.Name.Contains(gtin, StringComparison.OrdinalIgnoreCase)) 
                    ?? allProducts.FirstOrDefault();

                var lineItem = lineItems.FirstOrDefault(li => li.Gtin == gtin);
                
                if (product != null)
                {
                    // Collect all raw materials used across all packaging units
                    var allRawMaterials = new Dictionary<int, object>();
                    var packagingUnits = new List<object>();

                    foreach (var productPackaging in product.ProductPackagings)
                    {
                        var packagingUnit = productPackaging.PackagingUnit;
                        
                        // Get packaging types in this unit
                        var packagingTypes = packagingUnit.Items.Select(pui => new
                        {
                            id = pui.PackagingTypeId,
                            name = pui.PackagingType.Name,
                            description = pui.PackagingType.Description,
                            quantity = pui.Quantity,
                            unitLevel = packagingUnit.UnitLevel,
                            rawMaterials = pui.PackagingType.Materials.Select(ptm => new
                            {
                                id = ptm.MaterialId,
                                name = ptm.Material.Name,
                                description = ptm.Material.Description
                            }).ToList()
                        }).ToList();

                        // Collect raw materials
                        foreach (var pt in packagingTypes)
                        {
                            foreach (var rm in pt.rawMaterials)
                            {
                                if (!allRawMaterials.ContainsKey(rm.id))
                                {
                                    allRawMaterials[rm.id] = new
                                    {
                                        id = rm.id,
                                        name = rm.name,
                                        description = rm.description
                                    };
                                }
                            }
                        }

                        packagingUnits.Add(new
                        {
                            id = packagingUnit.Id,
                            name = packagingUnit.Name,
                            unitLevel = packagingUnit.UnitLevel,
                            packagingTypes = packagingTypes
                        });
                    }

                    productPackagingData.Add(new
                    {
                        gtin = gtin,
                        productId = product.Id,
                        productName = product.Name,
                        productDescription = product.Description,
                        found = true,
                        quantity = lineItem?.Quantity ?? 0,
                        packagingFlow = new
                        {
                            rawMaterials = allRawMaterials.Values.ToList(),
                            packagingUnits = packagingUnits.OrderBy(pu => 
                                ((dynamic)pu).unitLevel == "Primary" ? 1 :
                                ((dynamic)pu).unitLevel == "Secondary" ? 2 :
                                ((dynamic)pu).unitLevel == "Tertiary" ? 3 : 4
                            ).ToList()
                        }
                    });
                }
                else
                {
                    // No product found - return placeholder
                    productPackagingData.Add(new
                    {
                        gtin = gtin,
                        productName = lineItem?.Description ?? $"Product {gtin}",
                        found = false,
                        quantity = lineItem?.Quantity ?? 0,
                        packagingFlow = new
                        {
                            rawMaterials = new List<object>(),
                            packagingUnits = new List<object>()
                        }
                    });
                }
            }

            Response.ContentType = "application/json";
            return Json(new { success = true, data = productPackagingData });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting ASN product packaging for {AsnId}", asnId);
            return Json(new { success = false, message = ex.Message });
        }
    }

    /// <summary>
    /// Create dummy ASN, Product, and Packaging data for testing
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> CreateDummyData()
    {
        try
        {
            var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
            var scriptLogger = loggerFactory.CreateLogger<Scripts.CreateDummyAsnData>();
            var script = new Scripts.CreateDummyAsnData(_context, scriptLogger);
            await script.ExecuteAsync();

            // Ensure integrated datasets exist
            foreach (var key in new[] { "Electronics", "Alcoholic Beverages", "Confectionary", "Fresh Produce", "Garden", "Homewares", "Personal Care", "Pet Care", "Pharmaceuticals" })
            {
                var asnCount = await _context.AsnShipments.CountAsync(s => s.DatasetKey == key);
                if (asnCount == 0)
                {
                    if (key == "Electronics")
                    {
                        var electronicsSeeder = new EPR.Web.Seeders.ElectronicsDatasetSeeder(_context);
                        await electronicsSeeder.SeedAsync();
                    }
                    else if (key == "Alcoholic Beverages")
                    {
                        var alcSeeder = new EPR.Web.Seeders.AlcoholicBeveragesDatasetSeeder(_context);
                        await alcSeeder.SeedAsync();
                    }
                    else if (key == "Confectionary")
                    {
                        var confSeeder = new EPR.Web.Seeders.ConfectionaryDatasetSeeder(_context);
                        await confSeeder.SeedAsync();
                    }
                    else if (key == "Fresh Produce")
                    {
                        var fpSeeder = new EPR.Web.Seeders.FreshProduceDatasetSeeder(_context);
                        await fpSeeder.SeedAsync();
                    }
                    else if (key == "Garden")
                    {
                        var gdnSeeder = new EPR.Web.Seeders.GardenDatasetSeeder(_context);
                        await gdnSeeder.SeedAsync();
                    }
                    else if (key == "Homewares")
                    {
                        var hwSeeder = new EPR.Web.Seeders.HomewaresDatasetSeeder(_context);
                        await hwSeeder.SeedAsync();
                    }
                    else if (key == "Personal Care")
                    {
                        var pcSeeder = new EPR.Web.Seeders.PersonalCareDatasetSeeder(_context);
                        await pcSeeder.SeedAsync();
                    }
                    else if (key == "Pet Care")
                    {
                        var petSeeder = new EPR.Web.Seeders.PetCareDatasetSeeder(_context);
                        await petSeeder.SeedAsync();
                    }
                    else if (key == "Pharmaceuticals")
                    {
                        var pharmSeeder = new EPR.Web.Seeders.PharmaceuticalsDatasetSeeder(_context);
                        await pharmSeeder.SeedAsync();
                    }
                }
            }
            
            return Json(new { 
                success = true, 
                message = "Sample data created successfully. Refresh the page to see new ASNs." 
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating dummy data");
            return Json(new { success = false, message = ex.Message });
        }
    }

    /// <summary>
    /// Create a new ASN shipment (can be Real or Simulated)
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> CreateAsnShipment([FromBody] CreateAsnShipmentRequest request)
    {
        try
        {
            if (string.IsNullOrEmpty(request.AsnNumber))
            {
                return Json(new { success = false, message = "ASN Number is required" });
            }

            // Check if ASN number already exists
            var existing = await _context.AsnShipments
                .FirstOrDefaultAsync(s => s.AsnNumber == request.AsnNumber);
            if (existing != null)
            {
                return Json(new { success = false, message = $"ASN Number {request.AsnNumber} already exists" });
            }

            var shipment = new AsnShipment
            {
                AsnNumber = request.AsnNumber.Trim(),
                ShipperGln = !string.IsNullOrWhiteSpace(request.ShipperGln) ? request.ShipperGln.Trim() : "0000000000000",
                ShipperName = !string.IsNullOrWhiteSpace(request.ShipperName) ? request.ShipperName.Trim() : "Unknown Shipper",
                ShipperAddress = request.ShipperAddress?.Trim(),
                ShipperCity = request.ShipperCity?.Trim(),
                ShipperPostalCode = request.ShipperPostalCode?.Trim(),
                ShipperCountryCode = request.ShipperCountryCode?.Trim(),
                ReceiverGln = !string.IsNullOrWhiteSpace(request.ReceiverGln) ? request.ReceiverGln.Trim() : "0000000000000",
                ReceiverName = !string.IsNullOrWhiteSpace(request.ReceiverName) ? request.ReceiverName.Trim() : "Unknown Receiver",
                ShipDate = request.ShipDate,
                DeliveryDate = request.DeliveryDate,
                PoReference = request.PoReference?.Trim(),
                CarrierName = request.CarrierName?.Trim(),
                TransportMode = request.TransportMode?.Trim(),
                VehicleRegistration = request.VehicleRegistration?.Trim(),
                TotalWeight = request.TotalWeight,
                TotalPackages = request.TotalPackages,
                SourceFormat = request.IsSimulated ? "MANUAL" : "GS1_XML",
                Status = request.Status ?? "PENDING",
                IsSimulated = request.IsSimulated,
                ImportedAt = DateTime.UtcNow
            };

            _context.AsnShipments.Add(shipment);
            await _context.SaveChangesAsync();

            // Add pallets if provided
            if (request.Pallets != null && request.Pallets.Count > 0)
            {
                int palletSequence = 1;
                foreach (var palletReq in request.Pallets)
                {
                    var pallet = new AsnPallet
                    {
                        AsnShipmentId = shipment.Id,
                        Sscc = !string.IsNullOrWhiteSpace(palletReq.Sscc) ? palletReq.Sscc.Trim() : GenerateSscc(),
                        PackageTypeCode = palletReq.PackageTypeCode?.Trim(),
                        GrossWeight = palletReq.GrossWeight,
                        DestinationGln = !string.IsNullOrWhiteSpace(palletReq.DestinationGln) ? palletReq.DestinationGln.Trim() : "0000000000000",
                        DestinationName = !string.IsNullOrWhiteSpace(palletReq.DestinationName) ? palletReq.DestinationName.Trim() : "Unknown Destination",
                        DestinationAddress = palletReq.DestinationAddress?.Trim(),
                        DestinationCity = palletReq.DestinationCity?.Trim(),
                        DestinationPostalCode = palletReq.DestinationPostalCode?.Trim(),
                        DestinationCountryCode = palletReq.DestinationCountryCode?.Trim(),
                        SequenceNumber = palletReq.SequenceNumber > 0 ? palletReq.SequenceNumber : palletSequence++,
                        IsSimulated = palletReq.IsSimulated
                    };

                    _context.AsnPallets.Add(pallet);
                    await _context.SaveChangesAsync();

                    // Add line items if provided
                    if (palletReq.LineItems != null && palletReq.LineItems.Count > 0)
                    {
                        int lineNumber = 1;
                        foreach (var itemReq in palletReq.LineItems)
                        {
                            var lineItem = new AsnLineItem
                            {
                                AsnPalletId = pallet.Id,
                                LineNumber = itemReq.LineNumber > 0 ? itemReq.LineNumber : lineNumber++,
                                Gtin = !string.IsNullOrWhiteSpace(itemReq.Gtin) ? itemReq.Gtin.Trim() : "00000000000000",
                                Description = !string.IsNullOrWhiteSpace(itemReq.Description) ? itemReq.Description.Trim() : "Unknown Item",
                                Quantity = itemReq.Quantity > 0 ? itemReq.Quantity : 1,
                                UnitOfMeasure = !string.IsNullOrWhiteSpace(itemReq.UnitOfMeasure) ? itemReq.UnitOfMeasure.Trim() : "PCE",
                                BatchNumber = itemReq.BatchNumber?.Trim(),
                                BestBeforeDate = itemReq.BestBeforeDate,
                                PoLineReference = itemReq.PoLineReference?.Trim(),
                                SupplierArticleNumber = itemReq.SupplierArticleNumber?.Trim(),
                                NetWeight = itemReq.NetWeight,
                                IsSimulated = itemReq.IsSimulated
                            };

                            _context.AsnLineItems.Add(lineItem);
                        }
                    }
                }
            }

            await _context.SaveChangesAsync();

            _logger.LogInformation("Created ASN shipment {AsnNumber} (Simulated: {IsSimulated})", shipment.AsnNumber, shipment.IsSimulated);

            Response.ContentType = "application/json";
            return Json(new { success = true, data = new { Id = shipment.Id, AsnNumber = shipment.AsnNumber } });
        }
        catch (DbUpdateException dbEx)
        {
            _logger.LogError(dbEx, "Database error creating ASN shipment");
            var innerMessage = dbEx.InnerException?.Message ?? dbEx.Message;
            return Json(new { success = false, message = $"Database error: {innerMessage}" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating ASN shipment");
            return Json(new { success = false, message = $"Error: {ex.Message}" });
        }
    }

    /// <summary>
    /// Update an existing ASN shipment (can mark parts as simulated)
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> UpdateAsnShipment([FromBody] UpdateAsnShipmentRequest request)
    {
        try
        {
            var shipment = await _context.AsnShipments
                .Include(s => s.Pallets)
                .ThenInclude(p => p.LineItems)
                .FirstOrDefaultAsync(s => s.Id == request.Id);

            if (shipment == null)
            {
                return Json(new { success = false, message = "ASN shipment not found" });
            }

            // Update shipment fields
            if (request.ShipperName != null) shipment.ShipperName = request.ShipperName;
            if (request.ShipperGln != null) shipment.ShipperGln = request.ShipperGln;
            if (request.ShipperAddress != null) shipment.ShipperAddress = request.ShipperAddress;
            if (request.ShipperCity != null) shipment.ShipperCity = request.ShipperCity;
            if (request.ReceiverName != null) shipment.ReceiverName = request.ReceiverName;
            if (request.ReceiverGln != null) shipment.ReceiverGln = request.ReceiverGln;
            if (request.ShipDate.HasValue) shipment.ShipDate = request.ShipDate.Value;
            if (request.DeliveryDate.HasValue) shipment.DeliveryDate = request.DeliveryDate;
            if (request.Status != null) shipment.Status = request.Status;
            if (request.IsSimulated.HasValue) shipment.IsSimulated = request.IsSimulated.Value;

            // Update pallets
            if (request.Pallets != null)
            {
                foreach (var palletReq in request.Pallets)
                {
                    AsnPallet pallet;
                    if (palletReq.Id > 0)
                    {
                        // Update existing pallet
                        pallet = shipment.Pallets.FirstOrDefault(p => p.Id == palletReq.Id);
                        if (pallet != null)
                        {
                            if (palletReq.Sscc != null) pallet.Sscc = palletReq.Sscc;
                            if (palletReq.PackageTypeCode != null) pallet.PackageTypeCode = palletReq.PackageTypeCode;
                            if (palletReq.DestinationName != null) pallet.DestinationName = palletReq.DestinationName;
                            if (palletReq.DestinationGln != null) pallet.DestinationGln = palletReq.DestinationGln;
                            if (palletReq.DestinationAddress != null) pallet.DestinationAddress = palletReq.DestinationAddress;
                            if (palletReq.DestinationCity != null) pallet.DestinationCity = palletReq.DestinationCity;
                            if (palletReq.DestinationPostalCode != null) pallet.DestinationPostalCode = palletReq.DestinationPostalCode;
                            if (palletReq.DestinationCountryCode != null) pallet.DestinationCountryCode = palletReq.DestinationCountryCode;
                            if (palletReq.GrossWeight.HasValue) pallet.GrossWeight = palletReq.GrossWeight;
                            if (palletReq.IsSimulated.HasValue) pallet.IsSimulated = palletReq.IsSimulated.Value;
                        }
                    }
                    else
                    {
                        // Add new pallet
                        pallet = new AsnPallet
                        {
                            AsnShipmentId = shipment.Id,
                            Sscc = palletReq.Sscc ?? GenerateSscc(),
                            PackageTypeCode = palletReq.PackageTypeCode,
                            GrossWeight = palletReq.GrossWeight,
                            DestinationGln = palletReq.DestinationGln ?? string.Empty,
                            DestinationName = palletReq.DestinationName ?? string.Empty,
                            DestinationAddress = palletReq.DestinationAddress,
                            DestinationCity = palletReq.DestinationCity,
                            DestinationPostalCode = palletReq.DestinationPostalCode,
                            DestinationCountryCode = palletReq.DestinationCountryCode,
                            SequenceNumber = palletReq.SequenceNumber,
                            IsSimulated = palletReq.IsSimulated ?? false
                        };
                        _context.AsnPallets.Add(pallet);
                        await _context.SaveChangesAsync();
                    }

                    // Update line items
                    if (palletReq.LineItems != null && pallet != null)
                    {
                        foreach (var itemReq in palletReq.LineItems)
                        {
                            AsnLineItem lineItem;
                            if (itemReq.Id > 0)
                            {
                                // Update existing line item
                                lineItem = pallet.LineItems.FirstOrDefault(li => li.Id == itemReq.Id);
                                if (lineItem != null)
                                {
                                    if (itemReq.Description != null) lineItem.Description = itemReq.Description;
                                    if (itemReq.Gtin != null) lineItem.Gtin = itemReq.Gtin;
                                    if (itemReq.Quantity.HasValue) lineItem.Quantity = itemReq.Quantity.Value;
                                    if (itemReq.UnitOfMeasure != null) lineItem.UnitOfMeasure = itemReq.UnitOfMeasure;
                                    if (itemReq.BatchNumber != null) lineItem.BatchNumber = itemReq.BatchNumber;
                                    if (itemReq.BestBeforeDate.HasValue) lineItem.BestBeforeDate = itemReq.BestBeforeDate;
                                    if (itemReq.PoLineReference != null) lineItem.PoLineReference = itemReq.PoLineReference;
                                    if (itemReq.SupplierArticleNumber != null) lineItem.SupplierArticleNumber = itemReq.SupplierArticleNumber;
                                    if (itemReq.NetWeight.HasValue) lineItem.NetWeight = itemReq.NetWeight;
                                    if (itemReq.IsSimulated.HasValue) lineItem.IsSimulated = itemReq.IsSimulated.Value;
                                }
                            }
                            else
                            {
                                // Add new line item
                                lineItem = new AsnLineItem
                                {
                                    AsnPalletId = pallet.Id,
                                    LineNumber = itemReq.LineNumber,
                                    Gtin = itemReq.Gtin ?? string.Empty,
                                    Description = itemReq.Description ?? string.Empty,
                                    Quantity = itemReq.Quantity ?? 0,
                                    UnitOfMeasure = itemReq.UnitOfMeasure ?? "PCE",
                                    BatchNumber = itemReq.BatchNumber,
                                    BestBeforeDate = itemReq.BestBeforeDate,
                                    PoLineReference = itemReq.PoLineReference,
                                    SupplierArticleNumber = itemReq.SupplierArticleNumber,
                                    NetWeight = itemReq.NetWeight,
                                    IsSimulated = itemReq.IsSimulated ?? false
                                };
                                _context.AsnLineItems.Add(lineItem);
                            }
                        }
                    }
                }
            }

            await _context.SaveChangesAsync();

            _logger.LogInformation("Updated ASN shipment {Id}", shipment.Id);

            Response.ContentType = "application/json";
            return Json(new { success = true, data = new { Id = shipment.Id } });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating ASN shipment");
            return Json(new { success = false, message = ex.Message });
        }
    }

    private string GenerateSscc()
    {
        // Generate a random SSCC (18 digits)
        var random = new Random();
        var sscc = "3" + random.Next(100000000, 999999999).ToString() + random.Next(10000000, 99999999).ToString();
        return sscc;
    }

    // DTOs for requests
    public class CreateAsnShipmentRequest
    {
        public string AsnNumber { get; set; } = string.Empty;
        public string? ShipperGln { get; set; }
        public string? ShipperName { get; set; }
        public string? ShipperAddress { get; set; }
        public string? ShipperCity { get; set; }
        public string? ShipperPostalCode { get; set; }
        public string? ShipperCountryCode { get; set; }
        public string? ReceiverGln { get; set; }
        public string? ReceiverName { get; set; }
        public DateTime ShipDate { get; set; }
        public DateTime? DeliveryDate { get; set; }
        public string? PoReference { get; set; }
        public string? CarrierName { get; set; }
        public string? TransportMode { get; set; }
        public string? VehicleRegistration { get; set; }
        public decimal? TotalWeight { get; set; }
        public int? TotalPackages { get; set; }
        public string? Status { get; set; }
        public bool IsSimulated { get; set; }
        public List<CreatePalletRequest>? Pallets { get; set; }
    }

    public class UpdateAsnShipmentRequest
    {
        public int Id { get; set; }
        public string? ShipperName { get; set; }
        public string? ShipperGln { get; set; }
        public string? ShipperAddress { get; set; }
        public string? ShipperCity { get; set; }
        public string? ReceiverName { get; set; }
        public string? ReceiverGln { get; set; }
        public DateTime? ShipDate { get; set; }
        public DateTime? DeliveryDate { get; set; }
        public string? Status { get; set; }
        public bool? IsSimulated { get; set; }
        public List<UpdatePalletRequest>? Pallets { get; set; }
    }

    public class CreatePalletRequest
    {
        public string? Sscc { get; set; }
        public string? PackageTypeCode { get; set; }
        public decimal? GrossWeight { get; set; }
        public string? DestinationGln { get; set; }
        public string? DestinationName { get; set; }
        public string? DestinationAddress { get; set; }
        public string? DestinationCity { get; set; }
        public string? DestinationPostalCode { get; set; }
        public string? DestinationCountryCode { get; set; }
        public int SequenceNumber { get; set; }
        public bool IsSimulated { get; set; }
        public List<CreateLineItemRequest>? LineItems { get; set; }
    }

    public class UpdatePalletRequest
    {
        public int Id { get; set; }
        public string? Sscc { get; set; }
        public string? PackageTypeCode { get; set; }
        public decimal? GrossWeight { get; set; }
        public string? DestinationGln { get; set; }
        public string? DestinationName { get; set; }
        public string? DestinationAddress { get; set; }
        public string? DestinationCity { get; set; }
        public string? DestinationPostalCode { get; set; }
        public string? DestinationCountryCode { get; set; }
        public int SequenceNumber { get; set; }
        public bool? IsSimulated { get; set; }
        public List<UpdateLineItemRequest>? LineItems { get; set; }
    }

    public class CreateLineItemRequest
    {
        public int LineNumber { get; set; }
        public string? Gtin { get; set; }
        public string? Description { get; set; }
        public decimal Quantity { get; set; }
        public string? UnitOfMeasure { get; set; }
        public string? BatchNumber { get; set; }
        public DateTime? BestBeforeDate { get; set; }
        public string? PoLineReference { get; set; }
        public string? SupplierArticleNumber { get; set; }
        public decimal? NetWeight { get; set; }
        public bool IsSimulated { get; set; }
    }

    public class UpdateLineItemRequest
    {
        public int Id { get; set; }
        public int LineNumber { get; set; }
        public string? Gtin { get; set; }
        public string? Description { get; set; }
        public decimal? Quantity { get; set; }
        public string? UnitOfMeasure { get; set; }
        public string? BatchNumber { get; set; }
        public DateTime? BestBeforeDate { get; set; }
        public string? PoLineReference { get; set; }
        public string? SupplierArticleNumber { get; set; }
        public decimal? NetWeight { get; set; }
        public bool? IsSimulated { get; set; }
    }
}









