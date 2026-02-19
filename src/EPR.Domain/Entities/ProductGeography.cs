namespace EPR.Domain.Entities;

/// <summary>
/// Represents the relationship between products and geographies
/// </summary>
public class ProductGeography
{
    public int Id { get; set; }
    public int ProductId { get; set; }
    public int GeographyId { get; set; }
    public decimal? Quantity { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    public virtual Product Product { get; set; } = null!;
    public virtual Geography Geography { get; set; } = null!;
}









