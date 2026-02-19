namespace EPR.Domain.Entities;

/// <summary>
/// Associates a material with a jurisdiction and its EPR fee structure
/// </summary>
public class MaterialJurisdiction
{
    public int Id { get; set; }
    public int MaterialId { get; set; }
    public int JurisdictionId { get; set; }
    public decimal FeePerTonne { get; set; }
    public string Currency { get; set; } = "USD";
    public DateTime EffectiveFrom { get; set; }
    public DateTime? EffectiveTo { get; set; }
    public string? ReferenceDocument { get; set; }
    public string? PresidingBody { get; set; }
    public string? ReportingFrequency { get; set; }
    public string? ReportingFormat { get; set; }
    public string? SubmissionType { get; set; } // Self Audit, Third-Party, Exception
    public int Version { get; set; } = 1;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    public virtual PackagingRawMaterial Material { get; set; } = null!;
    public virtual Jurisdiction Jurisdiction { get; set; } = null!;
    public virtual ICollection<MaterialJurisdictionHistory> History { get; set; } = new List<MaterialJurisdictionHistory>();
}









