namespace EPR.Domain.Entities;

/// <summary>
/// Links a Packaging Library item to its raw materials (many-to-many).
/// A packaging item (e.g. cardboard box) can be made from multiple raw materials
/// (e.g. cardboard, adhesive label, staples), each potentially from different suppliers.
/// </summary>
public class PackagingLibraryMaterial
{
    public int Id { get; set; }

    public int PackagingLibraryId { get; set; }
    public int MaterialTaxonomyId { get; set; }

    /// <summary>
    /// Optional sort order for display
    /// </summary>
    public int SortOrder { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    public virtual PackagingLibrary PackagingLibrary { get; set; } = null!;
    public virtual MaterialTaxonomy MaterialTaxonomy { get; set; } = null!;
}
