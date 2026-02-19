namespace EPR.Domain.Entities;

/// <summary>
/// Packaging Library Item - individual packaging items from the library
/// These are the building blocks that make up Packaging Groups
/// </summary>
public class PackagingLibrary
{
    public int Id { get; set; }
    
    /// <summary>
    /// Taxonomy code from the Materials field (TaxonID)
    /// </summary>
    public string TaxonomyCode { get; set; } = string.Empty;
    
    /// <summary>
    /// Name of the packaging item (from Materials field, in brackets)
    /// </summary>
    public string Name { get; set; } = string.Empty;
    
    /// <summary>
    /// Weight in grams
    /// </summary>
    public decimal? Weight { get; set; }
    
    /// <summary>
    /// Reference to MaterialTaxonomy if available
    /// </summary>
    public int? MaterialTaxonomyId { get; set; }
    
    /// <summary>
    /// Whether this item is active
    /// </summary>
    public bool IsActive { get; set; } = true;
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    
    // Navigation properties
    public virtual MaterialTaxonomy? MaterialTaxonomy { get; set; }
    public virtual ICollection<PackagingGroupItem> PackagingGroupItems { get; set; } = new List<PackagingGroupItem>();
    public virtual ICollection<PackagingLibraryMaterial> PackagingLibraryMaterials { get; set; } = new List<PackagingLibraryMaterial>();
    public virtual ICollection<PackagingLibrarySupplierProduct> PackagingLibrarySupplierProducts { get; set; } = new List<PackagingLibrarySupplierProduct>();
}






