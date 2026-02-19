namespace EPR.Domain.Entities;

/// <summary>
/// Association between Products and Packaging Units
/// </summary>
public class ProductPackaging
{
    public int Id { get; set; }
    public int ProductId { get; set; }
    public int PackagingUnitId { get; set; }

    // Navigation properties
    public virtual Product Product { get; set; } = null!;
    public virtual PackagingUnit PackagingUnit { get; set; } = null!;
}









