namespace EPR.Domain.Entities;

/// <summary>
/// Packaging product provided by a supplier
/// </summary>
public class PackagingSupplierProduct
{
    public int Id { get; set; }

    public int PackagingSupplierId { get; set; }

    /// <summary>
    /// Product name
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Product description
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Product/SKU code
    /// </summary>
    public string? ProductCode { get; set; }

    /// <summary>
    /// Optional link to PackagingLibrary taxonomy
    /// </summary>
    public string? TaxonomyCode { get; set; }

    // Navigation properties
    public virtual PackagingSupplier PackagingSupplier { get; set; } = null!;
    public virtual ICollection<ProductPackagingSupplierProduct> ProductPackagingSupplierProducts { get; set; } = new List<ProductPackagingSupplierProduct>();
    public virtual ICollection<PackagingLibrarySupplierProduct> PackagingLibrarySupplierProducts { get; set; } = new List<PackagingLibrarySupplierProduct>();
    public virtual ICollection<MaterialTaxonomySupplierProduct> MaterialTaxonomySupplierProducts { get; set; } = new List<MaterialTaxonomySupplierProduct>();
    public virtual ICollection<PackagingGroupSupplierProduct> PackagingGroupSupplierProducts { get; set; } = new List<PackagingGroupSupplierProduct>();
}
