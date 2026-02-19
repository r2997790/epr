namespace EPR.Domain.Entities;

/// <summary>
/// Links a Packaging Library item to supplier products that supply it.
/// A packaging item (e.g. cardboard box) can be supplied by one or more supplier products.
/// </summary>
public class PackagingLibrarySupplierProduct
{
    public int Id { get; set; }

    public int PackagingLibraryId { get; set; }
    public int PackagingSupplierProductId { get; set; }

    /// <summary>
    /// Optional: is this the primary/preferred supplier for this item?
    /// </summary>
    public bool IsPrimary { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    public virtual PackagingLibrary PackagingLibrary { get; set; } = null!;
    public virtual PackagingSupplierProduct PackagingSupplierProduct { get; set; } = null!;
}
