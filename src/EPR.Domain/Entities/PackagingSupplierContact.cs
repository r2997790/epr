namespace EPR.Domain.Entities;

/// <summary>
/// Contact person for a packaging supplier
/// </summary>
public class PackagingSupplierContact
{
    public int Id { get; set; }

    public int PackagingSupplierId { get; set; }

    /// <summary>
    /// Contact person name
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Job title or role
    /// </summary>
    public string? Title { get; set; }

    /// <summary>
    /// Phone number
    /// </summary>
    public string? Phone { get; set; }

    /// <summary>
    /// Email address
    /// </summary>
    public string? Email { get; set; }

    // Navigation property
    public virtual PackagingSupplier PackagingSupplier { get; set; } = null!;
}
