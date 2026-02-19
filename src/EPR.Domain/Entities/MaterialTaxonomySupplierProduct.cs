namespace EPR.Domain.Entities;

/// <summary>
/// Links a raw material (MaterialTaxonomy) to supplier products that supply it.
/// A raw material (e.g. cardboard) can be supplied by one or more supplier products.
/// </summary>
public class MaterialTaxonomySupplierProduct
{
    public int Id { get; set; }

    public int MaterialTaxonomyId { get; set; }
    public int PackagingSupplierProductId { get; set; }

    /// <summary>
    /// Optional: is this the primary/preferred supplier for this raw material?
    /// </summary>
    public bool IsPrimary { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    public virtual MaterialTaxonomy MaterialTaxonomy { get; set; } = null!;
    public virtual PackagingSupplierProduct PackagingSupplierProduct { get; set; } = null!;
}
