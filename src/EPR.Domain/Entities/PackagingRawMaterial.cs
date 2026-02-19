namespace EPR.Domain.Entities;

/// <summary>
/// Packaging Raw Material - base material used to create packaging
/// </summary>
public class PackagingRawMaterial
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Specification { get; set; }
    public int? ParentMaterialId { get; set; }
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    // Navigation properties
    public virtual PackagingRawMaterial? ParentMaterial { get; set; }
    public virtual ICollection<PackagingRawMaterial> SubMaterials { get; set; } = new List<PackagingRawMaterial>();
    public virtual ICollection<MaterialJurisdiction> Jurisdictions { get; set; } = new List<MaterialJurisdiction>();
    public virtual ICollection<PackagingTypeMaterial> PackagingTypes { get; set; } = new List<PackagingTypeMaterial>();
}









