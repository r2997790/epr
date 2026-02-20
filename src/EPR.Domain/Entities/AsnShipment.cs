namespace EPR.Domain.Entities;

/// <summary>
/// ASN Shipment - Advanced Shipping Notice header information
/// Supports EDI 856, DESADV, and GS1 XML ASN formats
/// </summary>
public class AsnShipment
{
    public int Id { get; set; }
    
    /// <summary>
    /// ASN Number (unique identifier)
    /// </summary>
    public string AsnNumber { get; set; } = string.Empty;
    
    /// <summary>
    /// Shipper GLN (GS1 Global Location Number - 13 digits)
    /// </summary>
    public string ShipperGln { get; set; } = string.Empty;
    
    /// <summary>
    /// Shipper organization name
    /// </summary>
    public string ShipperName { get; set; } = string.Empty;
    
    /// <summary>
    /// Shipper address
    /// </summary>
    public string? ShipperAddress { get; set; }
    
    /// <summary>
    /// Shipper city
    /// </summary>
    public string? ShipperCity { get; set; }
    
    /// <summary>
    /// Shipper postal code
    /// </summary>
    public string? ShipperPostalCode { get; set; }
    
    /// <summary>
    /// Shipper country code (ISO 2-letter)
    /// </summary>
    public string? ShipperCountryCode { get; set; }
    
    /// <summary>
    /// Receiver GLN
    /// </summary>
    public string ReceiverGln { get; set; } = string.Empty;
    
    /// <summary>
    /// Receiver organization name
    /// </summary>
    public string ReceiverName { get; set; } = string.Empty;
    
    /// <summary>
    /// Ship date (despatch date)
    /// </summary>
    public DateTime ShipDate { get; set; }
    
    /// <summary>
    /// Estimated delivery date
    /// </summary>
    public DateTime? DeliveryDate { get; set; }
    
    /// <summary>
    /// Purchase order reference
    /// </summary>
    public string? PoReference { get; set; }
    
    /// <summary>
    /// Carrier/transport company name
    /// </summary>
    public string? CarrierName { get; set; }
    
    /// <summary>
    /// Transport mode (ROAD, RAIL, AIR, SEA)
    /// </summary>
    public string? TransportMode { get; set; }
    
    /// <summary>
    /// Vehicle registration number
    /// </summary>
    public string? VehicleRegistration { get; set; }
    
    /// <summary>
    /// Total weight in kilograms
    /// </summary>
    public decimal? TotalWeight { get; set; }
    
    /// <summary>
    /// Total number of pallets/packages
    /// </summary>
    public int? TotalPackages { get; set; }
    
    /// <summary>
    /// Source format (EDI_856, DESADV, GS1_XML)
    /// </summary>
    public string SourceFormat { get; set; } = string.Empty;
    
    /// <summary>
    /// Raw source data (JSON or XML)
    /// </summary>
    public string? RawData { get; set; }
    
    /// <summary>
    /// Import date/time
    /// </summary>
    public DateTime ImportedAt { get; set; } = DateTime.UtcNow;
    
    /// <summary>
    /// Status (PENDING, IN_TRANSIT, DELIVERED, CANCELLED)
    /// </summary>
    public string Status { get; set; } = "PENDING";
    
    /// <summary>
    /// Indicates if this ASN is simulated (not from actual ASN file)
    /// Used for modeling onward journeys where full visibility may not be known
    /// </summary>
    public bool IsSimulated { get; set; } = false;

    /// <summary>
    /// Dataset key - filters data by selected dataset
    /// </summary>
    public string? DatasetKey { get; set; }
    
    // Navigation properties
    public virtual ICollection<AsnPallet> Pallets { get; set; } = new List<AsnPallet>();
}
