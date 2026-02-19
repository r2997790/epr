namespace EPR.Domain.Entities;

/// <summary>
/// Material Taxonomy Country Requirement - defines which taxonomy levels are required for specific countries
/// </summary>
public class MaterialTaxonomyCountryRequirement
{
    public int Id { get; set; }
    
    /// <summary>
    /// Reference to the taxonomy item (typically Level 1)
    /// </summary>
    public int MaterialTaxonomyId { get; set; }
    
    /// <summary>
    /// Country code (ISO 3166-1 alpha-2) or country name
    /// </summary>
    public string CountryCode { get; set; } = string.Empty;
    
    /// <summary>
    /// Country name
    /// </summary>
    public string CountryName { get; set; } = string.Empty;
    
    /// <summary>
    /// Minimum level required for this country (e.g., if Level 3 is required, user must classify down to Level 3)
    /// </summary>
    public int RequiredLevel { get; set; }
    
    /// <summary>
    /// Whether this requirement is active
    /// </summary>
    public bool IsActive { get; set; } = true;
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    
    // Navigation properties
    public virtual MaterialTaxonomy MaterialTaxonomy { get; set; } = null!;
}






