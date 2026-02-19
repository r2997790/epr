namespace EPR.Domain.Entities;

/// <summary>
/// ASN Line Item - Individual product line on a pallet
/// </summary>
public class AsnLineItem
{
    public int Id { get; set; }
    
    /// <summary>
    /// Foreign key to ASN Pallet
    /// </summary>
    public int AsnPalletId { get; set; }
    
    /// <summary>
    /// Line item number
    /// </summary>
    public int LineNumber { get; set; }
    
    /// <summary>
    /// GTIN - Global Trade Item Number (14 digits)
    /// </summary>
    public string Gtin { get; set; } = string.Empty;
    
    /// <summary>
    /// Product description
    /// </summary>
    public string Description { get; set; } = string.Empty;
    
    /// <summary>
    /// Quantity despatched (in units)
    /// </summary>
    public decimal Quantity { get; set; }
    
    /// <summary>
    /// Unit of measure (PCE = Pieces, CA = Cases, etc.)
    /// </summary>
    public string UnitOfMeasure { get; set; } = "PCE";
    
    /// <summary>
    /// Batch/Lot number
    /// </summary>
    public string? BatchNumber { get; set; }
    
    /// <summary>
    /// Best before / expiry date
    /// </summary>
    public DateTime? BestBeforeDate { get; set; }
    
    /// <summary>
    /// Purchase order line reference
    /// </summary>
    public string? PoLineReference { get; set; }
    
    /// <summary>
    /// Supplier article number
    /// </summary>
    public string? SupplierArticleNumber { get; set; }
    
    /// <summary>
    /// Net weight per item in kilograms
    /// </summary>
    public decimal? NetWeight { get; set; }
    
    /// <summary>
    /// Indicates if this line item is simulated (added/edited manually)
    /// Used for modeling onward journeys where full visibility may not be known
    /// </summary>
    public bool IsSimulated { get; set; } = false;
    
    // Navigation properties
    public virtual AsnPallet AsnPallet { get; set; } = null!;
}
