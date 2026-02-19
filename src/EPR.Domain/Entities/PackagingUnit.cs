namespace EPR.Domain.Entities;

/// <summary>
/// Packaging Unit - collection of packaging types (primary, secondary, tertiary, quaternary)
/// </summary>
public class PackagingUnit
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string UnitLevel { get; set; } = "Primary"; // Primary, Secondary, Tertiary, Quaternary
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    // Navigation properties
    public virtual ICollection<PackagingUnitItem> Items { get; set; } = new List<PackagingUnitItem>();
    public virtual ICollection<ProductPackaging> ProductPackagings { get; set; } = new List<ProductPackaging>();
}
