namespace EPR.Domain.Entities;

/// <summary>
/// Association between Packaging Types and Raw Materials
/// </summary>
public class PackagingTypeMaterial
{
    public int Id { get; set; }
    public int PackagingTypeId { get; set; }
    public int MaterialId { get; set; }

    // Navigation properties
    public virtual PackagingType PackagingType { get; set; } = null!;
    public virtual PackagingRawMaterial Material { get; set; } = null!;
}









