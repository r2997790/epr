using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EPR.Web.Attributes;
using EPR.Data;
using EPR.Domain.Entities;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace EPR.Web.Controllers;

[Authorize]
public class ProductFormController : Controller
{
    private readonly EPRDbContext _context;
    private readonly ILogger<ProductFormController> _logger;

    public ProductFormController(EPRDbContext context, ILogger<ProductFormController> logger)
    {
        _context = context;
        _logger = logger;
    }

    public IActionResult AddProduct()
    {
        return View();
    }

    /// <summary>
    /// Validate GS1 GTIN format (8, 12, 13, or 14 digits)
    /// </summary>
    private bool IsValidGtin(string? gtin)
    {
        if (string.IsNullOrWhiteSpace(gtin)) return false;
        return Regex.IsMatch(gtin, @"^\d{8}$|^\d{12}$|^\d{13}$|^\d{14}$");
    }

    /// <summary>
    /// Get existing products for associated SKUs dropdown
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetExistingProducts()
    {
        try
        {
            var products = await _context.Products
                .Select(p => new { p.Id, p.Name, p.Sku, p.Gtin })
                .OrderBy(p => p.Name)
                .ToListAsync();

            Response.ContentType = "application/json";
            return Json(new { success = true, data = products });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting existing products");
            return Json(new { success = false, message = ex.Message });
        }
    }

    /// <summary>
    /// Save product form data
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> SaveProductForm([FromBody] ProductFormRequest request)
    {
        try
        {
            // Validate GTIN
            if (!IsValidGtin(request.Gtin))
            {
                return Json(new { success = false, message = "Invalid GTIN format. Must be 8, 12, 13, or 14 digits." });
            }

            // Check if GTIN already exists
            var existingForm = await _context.ProductForms
                .FirstOrDefaultAsync(pf => pf.Gtin == request.Gtin);
            if (existingForm != null)
            {
                return Json(new { success = false, message = $"Product with GTIN {request.Gtin} already exists." });
            }

            // Check if Product with GTIN exists
            var existingProduct = await _context.Products
                .FirstOrDefaultAsync(p => p.Gtin == request.Gtin);
            
            Product product;
            if (existingProduct != null)
            {
                product = existingProduct;
                // Update existing product
                product.Name = request.ProductName;
                product.Brand = request.Brand;
                product.ProductCategory = request.ProductCategory;
                product.ProductSubCategory = request.ProductSubCategory;
                product.CountryOfOrigin = request.CountryOfOrigin;
                product.ProductWeight = request.ProductWeight;
                product.ProductWeightUnit = request.ProductWeightUnit;
                product.ProductVolume = request.ProductVolume;
                product.ProductVolumeUnit = request.ProductVolumeUnit;
                product.ParentUnitGtin = request.ParentUnitGtin;
                product.UnitsPerPackage = request.UnitsPerPackage;
                product.UpdatedAt = DateTime.UtcNow;
            }
            else
            {
                // Create new product
                product = new Product
                {
                    Sku = request.SkuCode ?? $"GTIN-{request.Gtin}",
                    Name = request.ProductName,
                    Description = request.GeneralNotes,
                    Gtin = request.Gtin,
                    Brand = request.Brand,
                    ProductCategory = request.ProductCategory,
                    ProductSubCategory = request.ProductSubCategory,
                    CountryOfOrigin = request.CountryOfOrigin,
                    ProductWeight = request.ProductWeight,
                    ProductWeightUnit = request.ProductWeightUnit,
                    ProductVolume = request.ProductVolume,
                    ProductVolumeUnit = request.ProductVolumeUnit,
                    ParentUnitGtin = request.ParentUnitGtin,
                    UnitsPerPackage = request.UnitsPerPackage,
                    CreatedAt = DateTime.UtcNow
                };
                _context.Products.Add(product);
            }

            await _context.SaveChangesAsync();

            // Create or update ProductForm
            var productForm = existingForm ?? new ProductForm
            {
                ProductId = product.Id,
                Gtin = request.Gtin,
                CreatedAt = DateTime.UtcNow
            };

            // Update all fields
            productForm.ProductName = request.ProductName;
            productForm.Brand = request.Brand;
            productForm.ProductWeight = request.ProductWeight;
            productForm.ProductWeightUnit = request.ProductWeightUnit;
            productForm.ProductVolume = request.ProductVolume;
            productForm.ProductVolumeUnit = request.ProductVolumeUnit;
            productForm.SkuCode = request.SkuCode;
            productForm.ProductCategory = request.ProductCategory;
            productForm.ProductSubCategory = request.ProductSubCategory;
            productForm.ParentUnitGtin = request.ParentUnitGtin;
            productForm.UnitsPerPackage = request.UnitsPerPackage;
            productForm.CountryOfOrigin = request.CountryOfOrigin;
            productForm.ProductPhotosJson = request.ProductPhotos != null ? JsonSerializer.Serialize(request.ProductPhotos) : null;

            // Packaging
            productForm.PackagingLevel = request.PackagingLevel;
            productForm.PackagingType = request.PackagingType;
            productForm.PackagingConfiguration = request.PackagingConfiguration;
            productForm.TotalPackagingWeight = request.TotalPackagingWeight;
            productForm.PackagingComponentsJson = request.PackagingComponents != null ? JsonSerializer.Serialize(request.PackagingComponents) : null;
            productForm.ClosureTypesJson = request.ClosureTypes != null ? JsonSerializer.Serialize(request.ClosureTypes) : null;
            productForm.AdhesiveType = request.AdhesiveType;
            productForm.TamperEvidence = request.TamperEvidence;
            productForm.Resealable = request.Resealable;
            productForm.LabelTypesJson = request.LabelTypes != null ? JsonSerializer.Serialize(request.LabelTypes) : null;
            productForm.LabelMaterial = request.LabelMaterial;
            productForm.LegalMarksJson = request.LegalMarks != null ? JsonSerializer.Serialize(request.LegalMarks) : null;

            // Distribution
            productForm.PrimaryRetailersJson = request.PrimaryRetailers != null ? JsonSerializer.Serialize(request.PrimaryRetailers) : null;
            productForm.RetailFormatJson = request.RetailFormat != null ? JsonSerializer.Serialize(request.RetailFormat) : null;
            productForm.MostCommonPackSize = request.MostCommonPackSize;
            productForm.IsPrivateLabel = request.IsPrivateLabel;
            productForm.GeographicDistributionJson = request.GeographicDistribution != null ? JsonSerializer.Serialize(request.GeographicDistribution) : null;

            // Environmental
            productForm.ShelfLifeExtension = request.ShelfLifeExtension;
            productForm.EstimatedShelfLifeDays = request.EstimatedShelfLifeDays;
            productForm.FoodWasteReductionImpact = request.FoodWasteReductionImpact;
            productForm.EprSchemeApplicable = request.EprSchemeApplicable;
            productForm.EprCategory = request.EprCategory;
            productForm.ApcoSignatory = request.ApcoSignatory;
            productForm.SustainabilityCertificationsJson = request.SustainabilityCertifications != null ? JsonSerializer.Serialize(request.SustainabilityCertifications) : null;

            // Related Products
            productForm.ProductFamily = request.ProductFamily;
            productForm.AssociatedSkusJson = request.AssociatedSkus != null ? JsonSerializer.Serialize(request.AssociatedSkus) : null;
            productForm.VariantReason = request.VariantReason;

            // Documentation
            productForm.GeneralNotes = request.GeneralNotes;
            productForm.PackagingRationale = request.PackagingRationale;
            productForm.KnownIssues = request.KnownIssues;
            productForm.ImprovementPlans = request.ImprovementPlans;

            productForm.Status = request.Status ?? "submitted";
            productForm.UpdatedAt = DateTime.UtcNow;
            productForm.CreatedBy = User.Identity?.Name;

            if (existingForm == null)
            {
                _context.ProductForms.Add(productForm);
            }

            await _context.SaveChangesAsync();

            _logger.LogInformation("Product form saved: GTIN {Gtin}, ProductId {ProductId}", request.Gtin, product.Id);

            Response.ContentType = "application/json";
            return Json(new { success = true, data = new { ProductId = product.Id, ProductFormId = productForm.Id } });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error saving product form");
            return Json(new { success = false, message = ex.Message });
        }
    }

    // DTOs
    public class ProductFormRequest
    {
        // SECTION 1: PRODUCT IDENTIFICATION
        public string Gtin { get; set; } = string.Empty;
        public string ProductName { get; set; } = string.Empty;
        public string Brand { get; set; } = string.Empty;
        public decimal? ProductWeight { get; set; }
        public string? ProductWeightUnit { get; set; }
        public decimal? ProductVolume { get; set; }
        public string? ProductVolumeUnit { get; set; }
        public string? SkuCode { get; set; }
        public string ProductCategory { get; set; } = string.Empty;
        public string ProductSubCategory { get; set; } = string.Empty;
        public string? ParentUnitGtin { get; set; }
        public int? UnitsPerPackage { get; set; }
        public string CountryOfOrigin { get; set; } = string.Empty;
        public List<string>? ProductPhotos { get; set; }

        // SECTION 2: PACKAGING SPECIFICATION
        public string PackagingLevel { get; set; } = string.Empty;
        public string PackagingType { get; set; } = string.Empty;
        public string PackagingConfiguration { get; set; } = string.Empty;
        public decimal? TotalPackagingWeight { get; set; }
        public List<PackagingComponentDto>? PackagingComponents { get; set; }
        public List<string>? ClosureTypes { get; set; }
        public string? AdhesiveType { get; set; }
        public bool? TamperEvidence { get; set; }
        public bool? Resealable { get; set; }
        public List<string>? LabelTypes { get; set; }
        public string? LabelMaterial { get; set; }
        public List<string>? LegalMarks { get; set; }

        // SECTION 3: RETAILER & DISTRIBUTION
        public List<string>? PrimaryRetailers { get; set; }
        public List<string>? RetailFormat { get; set; }
        public string? MostCommonPackSize { get; set; }
        public bool? IsPrivateLabel { get; set; }
        public List<string>? GeographicDistribution { get; set; }

        // SECTION 4: ENVIRONMENTAL & COMPLIANCE
        public bool? ShelfLifeExtension { get; set; }
        public int? EstimatedShelfLifeDays { get; set; }
        public string? FoodWasteReductionImpact { get; set; }
        public bool? EprSchemeApplicable { get; set; }
        public string? EprCategory { get; set; }
        public bool? ApcoSignatory { get; set; }
        public List<string>? SustainabilityCertifications { get; set; }

        // SECTION 5: RELATED PRODUCTS
        public string? ProductFamily { get; set; }
        public List<string>? AssociatedSkus { get; set; }
        public string? VariantReason { get; set; }

        // SECTION 6: NOTES & DOCUMENTATION
        public string? GeneralNotes { get; set; }
        public string? PackagingRationale { get; set; }
        public string? KnownIssues { get; set; }
        public string? ImprovementPlans { get; set; }

        public string? Status { get; set; }
    }

    public class PackagingComponentDto
    {
        public string MaterialType { get; set; } = string.Empty;
        public decimal Weight { get; set; }
        public decimal? Height { get; set; }
        public decimal? Width { get; set; }
        public decimal? Depth { get; set; }
        public decimal? VolumeCapacity { get; set; }
        public string? VolumeUnit { get; set; }
    }
}
