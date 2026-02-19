namespace EPR.Domain.Entities;

/// <summary>
/// Represents an edge (connection) between nodes in a flow blueprint
/// </summary>
public class FlowBlueprintEdge
{
    public int Id { get; set; }
    public int FlowBlueprintId { get; set; }
    public string FromNodeKey { get; set; } = string.Empty;
    public string ToNodeKey { get; set; } = string.Empty;

    // Navigation properties
    public virtual FlowBlueprint FlowBlueprint { get; set; } = null!;
}









