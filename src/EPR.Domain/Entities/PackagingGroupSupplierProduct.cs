namespace EPR.Domain.Entities;

/// <summary>
/// Links a Packaging Group to supplier products that supply it.
/// A packaging group (e.g. Electronics Product Pack) can be supplied by one or more supplier products.
/// </summary>
public class PackagingGroupSupplierProduct
{
    public int Id { get; set; }

    public int PackagingGroupId { get; set; }
    public int PackagingSupplierProductId { get; set; }

    /// <summary>
    /// Optional: is this the primary/preferred supplier for this group?
    /// </summary>
    public bool IsPrimary { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    public virtual PackagingGroup PackagingGroup { get; set; } = null!;
    public virtual PackagingSupplierProduct PackagingSupplierProduct { get; set; } = null!;
}
