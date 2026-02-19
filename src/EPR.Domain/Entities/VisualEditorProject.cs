namespace EPR.Domain.Entities;

/// <summary>
/// Represents a Visual Editor project that stores the complete project data as JSON
/// </summary>
public class VisualEditorProject
{
    public int Id { get; set; }
    public string Key { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string ProjectDataJson { get; set; } = string.Empty; // Stores the full ProjectData as JSON
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
}


