namespace EPR.Domain.Entities;

/// <summary>
/// Represents a Sankey flow blueprint that defines how data flows through the EPR system
/// </summary>
public class FlowBlueprint
{
    public int Id { get; set; }
    public string Key { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Purpose { get; set; } = string.Empty;
    public string DefaultMetric { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    // Navigation properties
    public virtual ICollection<FlowBlueprintNode> Nodes { get; set; } = new List<FlowBlueprintNode>();
    public virtual ICollection<FlowBlueprintEdge> Edges { get; set; } = new List<FlowBlueprintEdge>();
}









