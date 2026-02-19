namespace EPR.Domain.Entities;

/// <summary>
/// Material Taxonomy - hierarchical classification system for materials
/// Based on the Taxonomy sheet from the Excel file
/// </summary>
public class MaterialTaxonomy
{
    public int Id { get; set; }
    
    /// <summary>
    /// Taxonomy level (1-5)
    /// </summary>
    public int Level { get; set; }
    
    /// <summary>
    /// Code from the taxonomy
    /// </summary>
    public string Code { get; set; } = string.Empty;
    
    /// <summary>
    /// Display name for the toolbar
    /// </summary>
    public string DisplayName { get; set; } = string.Empty;
    
    /// <summary>
    /// Full name/description
    /// </summary>
    public string? Name { get; set; }
    
    /// <summary>
    /// Description
    /// </summary>
    public string? Description { get; set; }
    
    /// <summary>
    /// Icon class (Bootstrap icon) for display
    /// </summary>
    public string? IconClass { get; set; }
    
    /// <summary>
    /// Parent taxonomy ID (for hierarchical structure)
    /// </summary>
    public int? ParentTaxonomyId { get; set; }
    
    /// <summary>
    /// Sort order within the same level
    /// </summary>
    public int SortOrder { get; set; }
    
    /// <summary>
    /// Whether this taxonomy item is active
    /// </summary>
    public bool IsActive { get; set; } = true;
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    
    // Navigation properties
    public virtual MaterialTaxonomy? ParentTaxonomy { get; set; }
    public virtual ICollection<MaterialTaxonomy> ChildTaxonomies { get; set; } = new List<MaterialTaxonomy>();
    public virtual ICollection<MaterialTaxonomyCountryRequirement> CountryRequirements { get; set; } = new List<MaterialTaxonomyCountryRequirement>();
    public virtual ICollection<PackagingLibraryMaterial> PackagingLibraryMaterials { get; set; } = new List<PackagingLibraryMaterial>();
    public virtual ICollection<MaterialTaxonomySupplierProduct> MaterialTaxonomySupplierProducts { get; set; } = new List<MaterialTaxonomySupplierProduct>();
}






