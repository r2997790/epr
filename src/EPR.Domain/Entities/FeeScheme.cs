namespace EPR.Domain.Entities;

/// <summary>
/// Represents a fee/tax scheme in the EPR system
/// </summary>
public class FeeScheme
{
    public int Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int JurisdictionId { get; set; }
    public decimal? BaseRate { get; set; }
    public DateTime EffectiveFrom { get; set; }
    public DateTime? EffectiveTo { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    // Navigation properties
    public virtual Jurisdiction Jurisdiction { get; set; } = null!;
}









