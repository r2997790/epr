namespace EPR.Domain.Entities;

/// <summary>
/// Comprehensive Product Form Data - GS1 Compliant Product and Packaging Information
/// Stores all data from the Fresh Produce Packaging Audit Form
/// </summary>
public class ProductForm
{
    public int Id { get; set; }
    
    // Link to Product
    public int ProductId { get; set; }
    public virtual Product Product { get; set; } = null!;
    
    // SECTION 1: PRODUCT IDENTIFICATION
    public string Gtin { get; set; } = string.Empty; // GS1 GTIN - Primary Key
    public string ProductName { get; set; } = string.Empty;
    public string Brand { get; set; } = string.Empty;
    public decimal? ProductWeight { get; set; }
    public string? ProductWeightUnit { get; set; } // g, kg
    public decimal? ProductVolume { get; set; }
    public string? ProductVolumeUnit { get; set; } // ml, L
    public string? SkuCode { get; set; }
    public string ProductCategory { get; set; } = string.Empty;
    public string ProductSubCategory { get; set; } = string.Empty;
    public string? ParentUnitGtin { get; set; }
    public int? UnitsPerPackage { get; set; }
    public string CountryOfOrigin { get; set; } = string.Empty; // ISO 3166 alpha-2
    public string? ProductPhotosJson { get; set; } // JSON array of photo URLs
    
    // SECTION 2: PACKAGING SPECIFICATION
    public string PackagingLevel { get; set; } = string.Empty; // Consumer Unit, Retail Unit, Consignment Unit
    public string PackagingType { get; set; } = string.Empty;
    public string PackagingConfiguration { get; set; } = string.Empty; // Single component, Multi-component, Assembly
    public decimal? TotalPackagingWeight { get; set; } // grams
    
    // Packaging Components (stored as JSON)
    public string? PackagingComponentsJson { get; set; }
    
    // Closures & Fixings
    public string? ClosureTypesJson { get; set; } // JSON array
    public string? AdhesiveType { get; set; }
    public bool? TamperEvidence { get; set; }
    public bool? Resealable { get; set; }
    
    // Labeling & Marking
    public string? LabelTypesJson { get; set; } // JSON array
    public string? LabelMaterial { get; set; }
    public string? LegalMarksJson { get; set; } // JSON array
    
    // SECTION 3: RETAILER & DISTRIBUTION
    public string? PrimaryRetailersJson { get; set; } // JSON array
    public string? RetailFormatJson { get; set; } // JSON array
    public string? MostCommonPackSize { get; set; }
    public bool? IsPrivateLabel { get; set; }
    public string? GeographicDistributionJson { get; set; } // JSON array
    
    // SECTION 4: ENVIRONMENTAL & COMPLIANCE
    public bool? ShelfLifeExtension { get; set; }
    public int? EstimatedShelfLifeDays { get; set; }
    public string? FoodWasteReductionImpact { get; set; }
    public bool? EprSchemeApplicable { get; set; }
    public string? EprCategory { get; set; }
    public bool? ApcoSignatory { get; set; }
    public string? SustainabilityCertificationsJson { get; set; } // JSON array
    
    // SECTION 5: RELATED PRODUCTS
    public string? ProductFamily { get; set; }
    public string? AssociatedSkusJson { get; set; } // JSON array
    public string? VariantReason { get; set; }
    
    // SECTION 6: NOTES & DOCUMENTATION
    public string? GeneralNotes { get; set; }
    public string? PackagingRationale { get; set; }
    public string? KnownIssues { get; set; }
    public string? ImprovementPlans { get; set; }
    
    // SYSTEM FIELDS
    public string Status { get; set; } = "draft"; // draft, submitted, reviewed
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    public string? CreatedBy { get; set; }
}

/// <summary>
/// Packaging Component - Individual material within packaging
/// </summary>
public class PackagingComponent
{
    public string MaterialType { get; set; } = string.Empty; // Plastics, Paper, Cardboard, etc.
    public decimal Weight { get; set; } // grams
    public decimal? Height { get; set; } // mm
    public decimal? Width { get; set; } // mm
    public decimal? Depth { get; set; } // mm
    public decimal? VolumeCapacity { get; set; } // ml or L
    public string? VolumeUnit { get; set; } // ml, L
}
