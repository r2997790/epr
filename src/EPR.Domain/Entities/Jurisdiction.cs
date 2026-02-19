namespace EPR.Domain.Entities;

/// <summary>
/// Represents a jurisdiction in the EPR system
/// </summary>
public class Jurisdiction
{
    public int Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? CountryCode { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    // Navigation properties
    public virtual ICollection<Geography> Geographies { get; set; } = new List<Geography>();
    public virtual ICollection<FeeScheme> FeeSchemes { get; set; } = new List<FeeScheme>();
}









