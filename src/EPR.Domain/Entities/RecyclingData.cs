namespace EPR.Domain.Entities;

/// <summary>
/// Recycling data - percentage of materials that can be recycled by location/jurisdiction
/// </summary>
public class RecyclingData
{
    public int Id { get; set; }
    public int MaterialId { get; set; }
    public int? GeographyId { get; set; }
    public int? JurisdictionId { get; set; }
    public decimal RecyclablePercentage { get; set; } // 0-100
    public decimal NonRecyclablePercentage { get; set; } // 0-100
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    // Navigation properties
    public virtual PackagingRawMaterial Material { get; set; } = null!;
    public virtual Geography? Geography { get; set; }
    public virtual Jurisdiction? Jurisdiction { get; set; }
}









