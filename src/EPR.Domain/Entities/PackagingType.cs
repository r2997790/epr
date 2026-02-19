namespace EPR.Domain.Entities;

/// <summary>
/// Packaging Type - packaging created from raw materials
/// </summary>
public class PackagingType
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal? Height { get; set; }
    public decimal? Weight { get; set; }
    public decimal? Depth { get; set; }
    public decimal? Volume { get; set; }
    public string? ImageUrl { get; set; }
    public bool IsFromLibrary { get; set; }
    public string? LibrarySource { get; set; } // Visy, Amcor, TetraPak, etc.
    public bool IsFavorite { get; set; }
    public bool IsUserCreated { get; set; }
    public int? CopiedFromId { get; set; }
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    // Navigation properties
    public virtual ICollection<PackagingTypeMaterial> Materials { get; set; } = new List<PackagingTypeMaterial>();
    public virtual ICollection<PackagingUnitItem> PackagingUnitItems { get; set; } = new List<PackagingUnitItem>();
}
