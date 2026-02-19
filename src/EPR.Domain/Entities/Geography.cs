namespace EPR.Domain.Entities;

/// <summary>
/// Represents a geography/location in the EPR system
/// </summary>
public class Geography
{
    public int Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int? ParentGeographyId { get; set; }
    public int? JurisdictionId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    // Navigation properties
    public virtual Geography? ParentGeography { get; set; }
    public virtual Jurisdiction? Jurisdiction { get; set; }
    public virtual ICollection<Geography> ChildGeographies { get; set; } = new List<Geography>();
    public virtual ICollection<ProductGeography> ProductGeographies { get; set; } = new List<ProductGeography>();
}









