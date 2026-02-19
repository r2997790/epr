namespace EPR.Domain.Entities;

/// <summary>
/// Packaging Supplier - companies that supply packaging materials
/// </summary>
public class PackagingSupplier
{
    public int Id { get; set; }

    /// <summary>
    /// Supplier company name
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Street address
    /// </summary>
    public string? Address { get; set; }

    /// <summary>
    /// City
    /// </summary>
    public string? City { get; set; }

    /// <summary>
    /// State / Province
    /// </summary>
    public string? State { get; set; }

    /// <summary>
    /// Country
    /// </summary>
    public string? Country { get; set; }

    /// <summary>
    /// Primary phone number
    /// </summary>
    public string? Phone { get; set; }

    /// <summary>
    /// Primary email
    /// </summary>
    public string? Email { get; set; }

    /// <summary>
    /// Website URL
    /// </summary>
    public string? Website { get; set; }

    /// <summary>
    /// Whether this supplier is active
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Optional: ID of the supplier who supplies this supplier (supply chain hierarchy).
    /// E.g. cardboard box manufacturer gets cardboard from a cardboard supplier.
    /// </summary>
    public int? SuppliedBySupplierId { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    // Navigation properties
    public virtual PackagingSupplier? SuppliedBySupplier { get; set; }
    public virtual ICollection<PackagingSupplier> SuppliedSuppliers { get; set; } = new List<PackagingSupplier>();
    public virtual ICollection<PackagingSupplierContact> Contacts { get; set; } = new List<PackagingSupplierContact>();
    public virtual ICollection<PackagingSupplierProduct> Products { get; set; } = new List<PackagingSupplierProduct>();
}
