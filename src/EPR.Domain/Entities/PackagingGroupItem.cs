namespace EPR.Domain.Entities;

/// <summary>
/// Junction entity linking Packaging Groups to Packaging Library items
/// </summary>
public class PackagingGroupItem
{
    public int Id { get; set; }
    
    /// <summary>
    /// Foreign key to PackagingGroup
    /// </summary>
    public int PackagingGroupId { get; set; }
    
    /// <summary>
    /// Foreign key to PackagingLibrary
    /// </summary>
    public int PackagingLibraryId { get; set; }
    
    /// <summary>
    /// Sort order within the group
    /// </summary>
    public int SortOrder { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    // Navigation properties
    public virtual PackagingGroup PackagingGroup { get; set; } = null!;
    public virtual PackagingLibrary PackagingLibrary { get; set; } = null!;
}






