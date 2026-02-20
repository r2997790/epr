namespace EPR.Domain.Entities;

/// <summary>
/// Distribution - products distributed to locations
/// </summary>
public class Distribution
{
    public int Id { get; set; }
    public int ProductId { get; set; }
    public int PackagingUnitId { get; set; }
    public decimal Quantity { get; set; }
    public string? RetailerName { get; set; }
    public string? StreetAddress { get; set; }
    public string City { get; set; } = string.Empty;
    public string StateProvince { get; set; } = string.Empty;
    public string County { get; set; } = string.Empty;
    public string PostcodeZipcode { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
    public DateTime DispatchDate { get; set; }
    public int? GeographyId { get; set; }
    public int? JurisdictionId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Dataset key - filters data by selected dataset
    /// </summary>
    public string? DatasetKey { get; set; }

    // Navigation properties
    public virtual Product Product { get; set; } = null!;
    public virtual PackagingUnit PackagingUnit { get; set; } = null!;
    public virtual Geography? Geography { get; set; }
    public virtual Jurisdiction? Jurisdiction { get; set; }
}









