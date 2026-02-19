namespace EPR.Domain.Entities;

/// <summary>
/// Links a Product to a PackagingSupplierProduct - indicates which supplier packaging is used for a product
/// </summary>
public class ProductPackagingSupplierProduct
{
    public int Id { get; set; }
    public int ProductId { get; set; }
    public int PackagingSupplierProductId { get; set; }

    // Navigation properties
    public virtual Product Product { get; set; } = null!;
    public virtual PackagingSupplierProduct PackagingSupplierProduct { get; set; } = null!;
}
