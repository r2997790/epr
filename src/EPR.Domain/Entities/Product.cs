namespace EPR.Domain.Entities;

/// <summary>
/// Product SKU
/// </summary>
public class Product
{
    public int Id { get; set; }
    public string Sku { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal? Size { get; set; }
    public decimal? Weight { get; set; }
    public decimal? Height { get; set; }
    public decimal? Quantity { get; set; }
    public string? ImageUrl { get; set; }
    
    /// <summary>
    /// GS1 GTIN (Global Trade Item Number) - 8, 12, 13, or 14 digits
    /// </summary>
    public string? Gtin { get; set; }
    
    /// <summary>
    /// Brand name (GS1 Brand Owner/Trademark)
    /// </summary>
    public string? Brand { get; set; }
    
    /// <summary>
    /// Product category (GDS Classification)
    /// </summary>
    public string? ProductCategory { get; set; }
    
    /// <summary>
    /// Product sub-category (GDS Classification - Hierarchical)
    /// </summary>
    public string? ProductSubCategory { get; set; }
    
    /// <summary>
    /// Country of Origin (ISO 3166 alpha-2 code)
    /// </summary>
    public string? CountryOfOrigin { get; set; }
    
    /// <summary>
    /// Product weight value
    /// </summary>
    public decimal? ProductWeight { get; set; }
    
    /// <summary>
    /// Product weight unit (g, kg, ml, L)
    /// </summary>
    public string? ProductWeightUnit { get; set; }
    
    /// <summary>
    /// Product volume value
    /// </summary>
    public decimal? ProductVolume { get; set; }
    
    /// <summary>
    /// Product volume unit (ml, L)
    /// </summary>
    public string? ProductVolumeUnit { get; set; }
    
    /// <summary>
    /// Parent Unit GTIN (for Level 2 & 3 packaging hierarchy)
    /// </summary>
    public string? ParentUnitGtin { get; set; }
    
    /// <summary>
    /// Units per package
    /// </summary>
    public int? UnitsPerPackage { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    // Navigation properties
    public virtual ICollection<ProductPackaging> ProductPackagings { get; set; } = new List<ProductPackaging>();
    public virtual ICollection<ProductPackagingSupplierProduct> ProductPackagingSupplierProducts { get; set; } = new List<ProductPackagingSupplierProduct>();
    public virtual ICollection<Distribution> Distributions { get; set; } = new List<Distribution>();
    public virtual ICollection<ProductGeography> ProductGeographies { get; set; } = new List<ProductGeography>();
}
