namespace EPR.Domain.Entities;

/// <summary>
/// Items within a Packaging Unit (collection of packaging types)
/// </summary>
public class PackagingUnitItem
{
    public int Id { get; set; }
    public int PackagingUnitId { get; set; }
    public int PackagingTypeId { get; set; }
    public string CollectionName { get; set; } = string.Empty;
    public int Quantity { get; set; } = 1;

    // Navigation properties
    public virtual PackagingUnit PackagingUnit { get; set; } = null!;
    public virtual PackagingType PackagingType { get; set; } = null!;
}









