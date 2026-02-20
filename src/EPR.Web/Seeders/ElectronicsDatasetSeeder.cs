using EPR.Domain.Entities;
using EPR.Data;
using Microsoft.EntityFrameworkCore;

namespace EPR.Web.Seeders;

/// <summary>
/// Seeds a comprehensive Electronics dataset: 20 products with packaging, raw materials,
/// packaging groups, suppliers, distribution, and ASN data.
/// </summary>
public class ElectronicsDatasetSeeder
{
    private const string DatasetKey = "Electronics";
    private readonly EPRDbContext _context;

    public ElectronicsDatasetSeeder(EPRDbContext context)
    {
        _context = context;
    }

    public async Task SeedAsync()
    {
        // Skip if already seeded
        if (await _context.Products.AnyAsync(p => p.DatasetKey == DatasetKey))
        {
            Console.WriteLine("Electronics dataset already seeded");
            return;
        }

        // Ensure MaterialTaxonomy exists (create basic plastics/cardboard if none)
        var plasticTax = await EnsureMaterialTaxonomy("PLASTIC", "Plastics", 1);
        var paperTax = await EnsureMaterialTaxonomy("PAPER", "Paper & Cardboard", 1);
        var foamTax = await EnsureMaterialTaxonomy("FOAM", "Foam", 1);

        // Suppliers
        var supplierBox = await EnsureSupplier("TechPack Solutions Ltd", "12 Industrial Way", "Manchester", "UK", "techpack@example.com");
        var supplierPlastic = await EnsureSupplier("PlastiForm Industries", "45 Manufacturing Rd", "Birmingham", "UK", "sales@plastiform.co.uk");
        var supplierLabels = await EnsureSupplier("LabelTech UK", "8 Print Street", "Leeds", "UK", "info@labeltech.co.uk");

        // Packaging Library items (raw materials / packaging items)
        var boxCardboard = await EnsurePackagingLibrary("Cardboard shipping box 30x20x15cm", "ELEC-BOX-001", 450m, paperTax.Id, DatasetKey);
        var innerFoam = await EnsurePackagingLibrary("Foam insert for electronics", "ELEC-FOAM-001", 80m, foamTax.Id, DatasetKey);
        var plasticWrap = await EnsurePackagingLibrary("Anti-static plastic wrap", "ELEC-WRAP-001", 25m, plasticTax.Id, DatasetKey);
        var productBox = await EnsurePackagingLibrary("Product display box", "ELEC-PBOX-001", 120m, paperTax.Id, DatasetKey);
        var label = await EnsurePackagingLibrary("Product label", "ELEC-LBL-001", 5m, paperTax.Id, DatasetKey);

        await LinkPackagingMaterial(boxCardboard.Id, paperTax.Id);
        await LinkPackagingMaterial(innerFoam.Id, foamTax.Id);
        await LinkPackagingMaterial(plasticWrap.Id, plasticTax.Id);
        await LinkPackagingMaterial(productBox.Id, paperTax.Id);
        await LinkPackagingMaterial(label.Id, paperTax.Id);

        // Supplier products
        var spBox = await EnsureSupplierProduct(supplierBox.Id, "TechBox 30x20x15", "ELEC-BOX-001");
        var spFoam = await EnsureSupplierProduct(supplierPlastic.Id, "E-Foam Insert S", "ELEC-FOAM-001");
        var spWrap = await EnsureSupplierProduct(supplierPlastic.Id, "AntiStatic Wrap 500mm", "ELEC-WRAP-001");
        var spPBox = await EnsureSupplierProduct(supplierBox.Id, "Display Box A4", "ELEC-PBOX-001");
        var spLabel = await EnsureSupplierProduct(supplierLabels.Id, "Electronics Label 50x30", "ELEC-LBL-001");

        await LinkPackagingSupplier(boxCardboard.Id, spBox.Id, true);
        await LinkPackagingSupplier(innerFoam.Id, spFoam.Id, true);
        await LinkPackagingSupplier(plasticWrap.Id, spWrap.Id, true);
        await LinkPackagingSupplier(productBox.Id, spPBox.Id, true);
        await LinkPackagingSupplier(label.Id, spLabel.Id, true);

        // Packaging Groups
        var groupShipping = await EnsurePackagingGroup("ELEC-SHIP-001", "Electronics Shipping Pack", "Secondary", 655m, DatasetKey);
        await AddGroupItem(groupShipping.Id, boxCardboard.Id, 0);
        await AddGroupItem(groupShipping.Id, innerFoam.Id, 1);
        await AddGroupItem(groupShipping.Id, plasticWrap.Id, 2);

        var groupProduct = await EnsurePackagingGroup("ELEC-PROD-001", "Electronics Product Pack", "Primary", 125m, DatasetKey);
        await AddGroupItem(groupProduct.Id, productBox.Id, 0);
        await AddGroupItem(groupProduct.Id, label.Id, 1);

        // Packaging Units
        var puShipping = await EnsurePackagingUnit("Electronics Shipping Unit", "Secondary");
        var puProduct = await EnsurePackagingUnit("Electronics Product Unit", "Primary");

        // Geographies and Jurisdiction
        var ukJurisdiction = await EnsureJurisdiction("UK", "United Kingdom", "GB");
        var londonGeo = await EnsureGeography("LON", "London", ukJurisdiction.Id);
        var manchesterGeo = await EnsureGeography("MAN", "Manchester", ukJurisdiction.Id);
        var birminghamGeo = await EnsureGeography("BIR", "Birmingham", ukJurisdiction.Id);

        // 20 Electronics products
        var products = new[]
        {
            ("ELEC-001", "Wireless Bluetooth Earbuds Pro", "TechSound", "50601234567890"),
            ("ELEC-002", "USB-C Fast Charger 65W", "PowerFlow", "50601234567891"),
            ("ELEC-003", "Portable Power Bank 20000mAh", "PowerFlow", "50601234567892"),
            ("ELEC-004", "HD Webcam 1080p", "VisionTech", "50601234567893"),
            ("ELEC-005", "Mechanical Gaming Keyboard", "KeyMaster", "50601234567894"),
            ("ELEC-006", "Wireless Mouse Ergonomic", "KeyMaster", "50601234567895"),
            ("ELEC-007", "Bluetooth Speaker Waterproof", "TechSound", "50601234567896"),
            ("ELEC-008", "Tablet Stand Adjustable", "DeskPro", "50601234567897"),
            ("ELEC-009", "Phone Holder Car Mount", "DeskPro", "50601234567898"),
            ("ELEC-010", "HDMI Cable 2m", "CableTech", "50601234567899"),
            ("ELEC-011", "USB Hub 4-Port", "CableTech", "50601234567900"),
            ("ELEC-012", "Screen Protector Glass", "VisionTech", "50601234567901"),
            ("ELEC-013", "Laptop Sleeve 15\"", "DeskPro", "50601234567902"),
            ("ELEC-014", "Smart Watch Band Silicone", "WearTech", "50601234567903"),
            ("ELEC-015", "Earbud Tips Set S/M/L", "TechSound", "50601234567904"),
            ("ELEC-016", "Cable Organiser Box", "DeskPro", "50601234567905"),
            ("ELEC-017", "LED Desk Lamp USB", "LightPro", "50601234567906"),
            ("ELEC-018", "Webcam Cover Slider", "VisionTech", "50601234567907"),
            ("ELEC-019", "Laptop Cooling Pad", "DeskPro", "50601234567908"),
            ("ELEC-020", "Multi-Port Adapter USB-C", "CableTech", "50601234567909"),
        };

        var productEntities = new List<Product>();
        foreach (var (sku, name, brand, gtin) in products)
        {
            var p = await EnsureProduct(sku, name, brand, gtin, DatasetKey);
            productEntities.Add(p);

            // ProductForm
            await EnsureProductForm(p.Id, gtin, name, brand, "Electronics", "Consumer Electronics");

            // ProductPackaging - each product has shipping and product pack
            await EnsureProductPackaging(p.Id, puShipping.Id);
            await EnsureProductPackaging(p.Id, puProduct.Id);
        }

        // Distribution - distribute products to locations
        var rnd = new Random(42);
        var geos = new[] { (londonGeo, "London"), (manchesterGeo, "Manchester"), (birminghamGeo, "Birmingham") };
        foreach (var p in productEntities)
        {
            var (geo, cityName) = geos[rnd.Next(geos.Length)];
            await EnsureDistribution(p.Id, puShipping.Id, rnd.Next(50, 500), "Retail Store", cityName, "UK", geo.Id, ukJurisdiction.Id);
        }

        // ASN Shipments
        var baseDate = DateTime.UtcNow.AddDays(-7);
        for (int i = 0; i < 5; i++)
        {
            var ship = await EnsureAsnShipment(
                $"ASN-ELEC-{1000 + i}",
                "50601234567890",
                "TechPack Solutions Ltd",
                "50609876543210",
                "TechRetail DC London",
                baseDate.AddDays(i * 2),
                DatasetKey);
            var pallet = await EnsureAsnPallet(ship.Id, $"35791234567890123{i}", "TechRetail DC London", "London", "GB", 1);
            var prod = productEntities[i % productEntities.Count];
            await EnsureAsnLineItem(pallet.Id, prod.Gtin!, prod.Name, 24 + i * 5, 1);
        }

        await _context.SaveChangesAsync();
        Console.WriteLine($"✓ Electronics dataset seeded: 20 products, packaging, distribution, ASNs");
    }

    private async Task<MaterialTaxonomy> EnsureMaterialTaxonomy(string code, string displayName, int level)
    {
        var t = await _context.MaterialTaxonomies.FirstOrDefaultAsync(m => m.Code == code);
        if (t == null)
        {
            t = new MaterialTaxonomy
            {
                Code = code,
                DisplayName = displayName,
                Level = level,
                IsActive = true,
                SortOrder = 0
            };
            _context.MaterialTaxonomies.Add(t);
            await _context.SaveChangesAsync();
        }
        return t;
    }

    private async Task<PackagingSupplier> EnsureSupplier(string name, string address, string city, string country, string email)
    {
        var s = await _context.PackagingSuppliers.FirstOrDefaultAsync(x => x.Name == name);
        if (s == null)
        {
            s = new PackagingSupplier { Name = name, Address = address, City = city, Country = country, Email = email, IsActive = true };
            _context.PackagingSuppliers.Add(s);
            await _context.SaveChangesAsync();
        }
        return s;
    }

    private async Task<PackagingLibrary> EnsurePackagingLibrary(string name, string taxonomyCode, decimal weight, int materialTaxonomyId, string datasetKey)
    {
        var lib = new PackagingLibrary
        {
            Name = name,
            TaxonomyCode = taxonomyCode,
            Weight = weight,
            MaterialTaxonomyId = materialTaxonomyId,
            DatasetKey = datasetKey,
            IsActive = true
        };
        _context.PackagingLibraries.Add(lib);
        await _context.SaveChangesAsync();
        return lib;
    }

    private async Task LinkPackagingMaterial(int packagingLibraryId, int materialTaxonomyId)
    {
        if (await _context.PackagingLibraryMaterials.AnyAsync(plm => plm.PackagingLibraryId == packagingLibraryId && plm.MaterialTaxonomyId == materialTaxonomyId))
            return;
        _context.PackagingLibraryMaterials.Add(new PackagingLibraryMaterial
        {
            PackagingLibraryId = packagingLibraryId,
            MaterialTaxonomyId = materialTaxonomyId,
            SortOrder = 0
        });
        await _context.SaveChangesAsync();
    }

    private async Task<PackagingSupplierProduct> EnsureSupplierProduct(int supplierId, string name, string productCode)
    {
        var sp = await _context.PackagingSupplierProducts.FirstOrDefaultAsync(x => x.PackagingSupplierId == supplierId && x.ProductCode == productCode);
        if (sp == null)
        {
            sp = new PackagingSupplierProduct { PackagingSupplierId = supplierId, Name = name, ProductCode = productCode, TaxonomyCode = productCode };
            _context.PackagingSupplierProducts.Add(sp);
            await _context.SaveChangesAsync();
        }
        return sp;
    }

    private async Task LinkPackagingSupplier(int packagingLibraryId, int supplierProductId, bool isPrimary)
    {
        if (await _context.PackagingLibrarySupplierProducts.AnyAsync(plsp => plsp.PackagingLibraryId == packagingLibraryId && plsp.PackagingSupplierProductId == supplierProductId))
            return;
        _context.PackagingLibrarySupplierProducts.Add(new PackagingLibrarySupplierProduct
        {
            PackagingLibraryId = packagingLibraryId,
            PackagingSupplierProductId = supplierProductId,
            IsPrimary = isPrimary
        });
        await _context.SaveChangesAsync();
    }

    private async Task<PackagingGroup> EnsurePackagingGroup(string packId, string name, string layer, decimal totalWeight, string datasetKey)
    {
        var g = new PackagingGroup
        {
            PackId = packId,
            Name = name,
            PackagingLayer = layer,
            TotalPackWeight = totalWeight,
            DatasetKey = datasetKey,
            IsActive = true
        };
        _context.PackagingGroups.Add(g);
        await _context.SaveChangesAsync();
        return g;
    }

    private async Task AddGroupItem(int groupId, int libraryId, int sortOrder)
    {
        if (await _context.PackagingGroupItems.AnyAsync(gi => gi.PackagingGroupId == groupId && gi.PackagingLibraryId == libraryId))
            return;
        _context.PackagingGroupItems.Add(new PackagingGroupItem { PackagingGroupId = groupId, PackagingLibraryId = libraryId, SortOrder = sortOrder });
        await _context.SaveChangesAsync();
    }

    private async Task<PackagingUnit> EnsurePackagingUnit(string name, string level)
    {
        var pu = await _context.PackagingUnits.FirstOrDefaultAsync(x => x.Name == name);
        if (pu == null)
        {
            pu = new PackagingUnit { Name = name, UnitLevel = level };
            _context.PackagingUnits.Add(pu);
            await _context.SaveChangesAsync();
        }
        return pu;
    }

    private async Task<Jurisdiction> EnsureJurisdiction(string code, string name, string countryCode)
    {
        var j = await _context.Jurisdictions.FirstOrDefaultAsync(x => x.Code == code);
        if (j == null)
        {
            j = new Jurisdiction { Code = code, Name = name, CountryCode = countryCode };
            _context.Jurisdictions.Add(j);
            await _context.SaveChangesAsync();
        }
        return j;
    }

    private async Task<Geography> EnsureGeography(string code, string name, int jurisdictionId)
    {
        var g = await _context.Geographies.FirstOrDefaultAsync(x => x.Code == code);
        if (g == null)
        {
            g = new Geography { Code = code, Name = name, JurisdictionId = jurisdictionId };
            _context.Geographies.Add(g);
            await _context.SaveChangesAsync();
        }
        return g;
    }

    private async Task<Product> EnsureProduct(string sku, string name, string brand, string gtin, string datasetKey)
    {
        var p = await _context.Products.FirstOrDefaultAsync(x => x.Sku == sku);
        if (p == null)
        {
            p = new Product
            {
                Sku = sku,
                Name = name,
                Brand = brand,
                Gtin = gtin,
                ProductCategory = "Electronics",
                ProductSubCategory = "Consumer Electronics",
                CountryOfOrigin = "CN",
                DatasetKey = datasetKey
            };
            _context.Products.Add(p);
            await _context.SaveChangesAsync();
        }
        return p;
    }

    private async Task EnsureProductForm(int productId, string gtin, string productName, string brand, string category, string subCategory)
    {
        if (await _context.ProductForms.AnyAsync(pf => pf.ProductId == productId))
            return;
        _context.ProductForms.Add(new ProductForm
        {
            ProductId = productId,
            Gtin = gtin,
            ProductName = productName,
            Brand = brand,
            ProductCategory = category,
            ProductSubCategory = subCategory,
            CountryOfOrigin = "CN",
            PackagingLevel = "Consumer Unit",
            PackagingType = "Box",
            PackagingConfiguration = "Single component",
            Status = "submitted"
        });
        await _context.SaveChangesAsync();
    }

    private async Task EnsureProductPackaging(int productId, int packagingUnitId)
    {
        if (await _context.ProductPackagings.AnyAsync(pp => pp.ProductId == productId && pp.PackagingUnitId == packagingUnitId))
            return;
        _context.ProductPackagings.Add(new ProductPackaging { ProductId = productId, PackagingUnitId = packagingUnitId });
        await _context.SaveChangesAsync();
    }

    private async Task EnsureDistribution(int productId, int packagingUnitId, int quantity, string retailer, string city, string country, int geographyId, int jurisdictionId)
    {
        _context.Distributions.Add(new Distribution
        {
            ProductId = productId,
            PackagingUnitId = packagingUnitId,
            Quantity = quantity,
            RetailerName = retailer,
            City = city,
            StateProvince = city,
            County = "",
            PostcodeZipcode = "",
            Country = country,
            DispatchDate = DateTime.UtcNow.AddDays(-30),
            GeographyId = geographyId,
            JurisdictionId = jurisdictionId,
            DatasetKey = DatasetKey
        });
        await _context.SaveChangesAsync();
    }

    private async Task<AsnShipment> EnsureAsnShipment(string asnNumber, string shipperGln, string shipperName, string receiverGln, string receiverName, DateTime shipDate, string datasetKey)
    {
        var s = new AsnShipment
        {
            AsnNumber = asnNumber,
            ShipperGln = shipperGln,
            ShipperName = shipperName,
            ShipperCity = "Manchester",
            ShipperCountryCode = "GB",
            ReceiverGln = receiverGln,
            ReceiverName = receiverName,
            ShipDate = shipDate,
            DeliveryDate = shipDate.AddDays(1),
            CarrierName = "FastFreight UK",
            TransportMode = "ROAD",
            TotalPackages = 1,
            TotalWeight = 25.5m,
            SourceFormat = "GS1_XML",
            Status = "DELIVERED",
            DatasetKey = datasetKey
        };
        _context.AsnShipments.Add(s);
        await _context.SaveChangesAsync();
        return s;
    }

    private async Task<AsnPallet> EnsureAsnPallet(int shipmentId, string sscc, string destName, string destCity, string destCountry, int seq)
    {
        var p = new AsnPallet
        {
            AsnShipmentId = shipmentId,
            Sscc = sscc,
            PackageTypeCode = "PLT",
            GrossWeight = 25m,
            DestinationGln = "50609876543210",
            DestinationName = destName,
            DestinationCity = destCity,
            DestinationCountryCode = destCountry,
            SequenceNumber = seq
        };
        _context.AsnPallets.Add(p);
        await _context.SaveChangesAsync();
        return p;
    }

    private async Task EnsureAsnLineItem(int palletId, string gtin, string description, decimal quantity, int lineNum)
    {
        _context.AsnLineItems.Add(new AsnLineItem
        {
            AsnPalletId = palletId,
            LineNumber = lineNum,
            Gtin = gtin,
            Description = description,
            Quantity = quantity,
            UnitOfMeasure = "PCE"
        });
        await _context.SaveChangesAsync();
    }
}
