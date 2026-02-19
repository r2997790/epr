namespace EPR.Domain.Entities;

/// <summary>
/// Version history for material jurisdiction fees
/// </summary>
public class MaterialJurisdictionHistory
{
    public int Id { get; set; }
    public int MaterialJurisdictionId { get; set; }
    public decimal FeePerTonne { get; set; }
    public DateTime EffectiveFrom { get; set; }
    public DateTime? EffectiveTo { get; set; }
    public string? ReferenceDocument { get; set; }
    public string? PresidingBody { get; set; }
    public int Version { get; set; }
    public DateTime ChangedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    public virtual MaterialJurisdiction MaterialJurisdiction { get; set; } = null!;
}









