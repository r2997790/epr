namespace EPR.Domain.Entities;

/// <summary>
/// ASN Pallet/Despatch Unit - Represents a pallet or container in a shipment
/// </summary>
public class AsnPallet
{
    public int Id { get; set; }
    
    /// <summary>
    /// Foreign key to ASN Shipment
    /// </summary>
    public int AsnShipmentId { get; set; }
    
    /// <summary>
    /// SSCC - Serial Shipping Container Code (18 digits)
    /// </summary>
    public string Sscc { get; set; } = string.Empty;
    
    /// <summary>
    /// Package type code (PLT = Pallet, BOX = Box, etc.)
    /// </summary>
    public string? PackageTypeCode { get; set; }
    
    /// <summary>
    /// Gross weight in kilograms
    /// </summary>
    public decimal? GrossWeight { get; set; }
    
    /// <summary>
    /// Destination GLN (where this pallet is going)
    /// </summary>
    public string DestinationGln { get; set; } = string.Empty;
    
    /// <summary>
    /// Destination name
    /// </summary>
    public string DestinationName { get; set; } = string.Empty;
    
    /// <summary>
    /// Destination address
    /// </summary>
    public string? DestinationAddress { get; set; }
    
    /// <summary>
    /// Destination city
    /// </summary>
    public string? DestinationCity { get; set; }
    
    /// <summary>
    /// Destination postal code
    /// </summary>
    public string? DestinationPostalCode { get; set; }
    
    /// <summary>
    /// Destination country code (ISO 2-letter)
    /// </summary>
    public string? DestinationCountryCode { get; set; }
    
    /// <summary>
    /// Pallet sequence number in shipment
    /// </summary>
    public int SequenceNumber { get; set; }
    
    /// <summary>
    /// Indicates if this pallet is simulated (added/edited manually)
    /// Used for modeling onward journeys where full visibility may not be known
    /// </summary>
    public bool IsSimulated { get; set; } = false;
    
    // Navigation properties
    public virtual AsnShipment AsnShipment { get; set; } = null!;
    public virtual ICollection<AsnLineItem> LineItems { get; set; } = new List<AsnLineItem>();
}
