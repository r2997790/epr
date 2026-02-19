namespace EPR.Domain.Entities;

/// <summary>
/// EcoModularity fee structure based on geography
/// </summary>
public class EcoModularityFee
{
    public int Id { get; set; }
    public int MaterialId { get; set; }
    public int JurisdictionId { get; set; }
    public int? GeographyId { get; set; }
    public decimal FeePerTonne { get; set; }
    public string Currency { get; set; } = "USD";
    public string DisposalMethod { get; set; } = string.Empty;
    public bool IsEnforced { get; set; }
    public DateTime EffectiveFrom { get; set; }
    public DateTime? EffectiveTo { get; set; }
    public string? ReferenceDocument { get; set; }
    public string? PresidingBody { get; set; }
    public string? ReportingFrequency { get; set; }
    public string? ReportingFormat { get; set; }
    public string? SubmissionType { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    public virtual PackagingRawMaterial Material { get; set; } = null!;
    public virtual Jurisdiction Jurisdiction { get; set; } = null!;
    public virtual Geography? Geography { get; set; }
}









