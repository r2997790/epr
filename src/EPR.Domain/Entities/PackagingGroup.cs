namespace EPR.Domain.Entities;

/// <summary>
/// Packaging Group - a collection of Packaging Library items
/// Example: Bottle+Cap+Label contains Bottle, Cap, and Label items
/// </summary>
public class PackagingGroup
{
    public int Id { get; set; }
    
    /// <summary>
    /// PackID from the spreadsheet
    /// </summary>
    public string PackId { get; set; } = string.Empty;
    
    /// <summary>
    /// Packaging Name - the group title
    /// </summary>
    public string Name { get; set; } = string.Empty;
    
    /// <summary>
    /// Packaging Layer (renamed from Packaging Group column)
    /// </summary>
    public string? PackagingLayer { get; set; }
    
    /// <summary>
    /// Style parameter
    /// </summary>
    public string? Style { get; set; }
    
    /// <summary>
    /// Shape parameter
    /// </summary>
    public string? Shape { get; set; }
    
    /// <summary>
    /// Size parameter
    /// </summary>
    public string? Size { get; set; }
    
    /// <summary>
    /// Volume Dimensions parameter
    /// </summary>
    public string? VolumeDimensions { get; set; }
    
    /// <summary>
    /// Colours Available parameter
    /// </summary>
    public string? ColoursAvailable { get; set; }
    
    /// <summary>
    /// Recycled Content parameter
    /// </summary>
    public string? RecycledContent { get; set; }
    
    /// <summary>
    /// Total Pack Weight (g)
    /// </summary>
    public decimal? TotalPackWeight { get; set; }
    
    /// <summary>
    /// Weight Basis parameter
    /// </summary>
    public string? WeightBasis { get; set; }
    
    /// <summary>
    /// Notes parameter
    /// </summary>
    public string? Notes { get; set; }
    
    /// <summary>
    /// Example reference parameter
    /// </summary>
    public string? ExampleReference { get; set; }
    
    /// <summary>
    /// Source parameter
    /// </summary>
    public string? Source { get; set; }
    
    /// <summary>
    /// URL parameter
    /// </summary>
    public string? Url { get; set; }
    
    /// <summary>
    /// Whether this group is active
    /// </summary>
    public bool IsActive { get; set; } = true;
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    /// <summary>
    /// Dataset key - filters data by selected dataset
    /// </summary>
    public string? DatasetKey { get; set; }

    /// <summary>
    /// FK to the next outer packaging level (Primary -> Secondary -> Tertiary)
    /// </summary>
    public int? ParentPackagingGroupId { get; set; }

    /// <summary>
    /// How many of this level fit in one parent (e.g., 20 bottles per box)
    /// </summary>
    public int? QuantityInParent { get; set; }
    
    // Navigation properties
    public virtual PackagingGroup? ParentPackagingGroup { get; set; }
    public virtual ICollection<PackagingGroup> ChildGroups { get; set; } = new List<PackagingGroup>();
    public virtual ICollection<PackagingGroupItem> Items { get; set; } = new List<PackagingGroupItem>();
    public virtual ICollection<PackagingGroupSupplierProduct> PackagingGroupSupplierProducts { get; set; } = new List<PackagingGroupSupplierProduct>();
}






