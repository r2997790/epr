namespace EPR.Domain.Entities;

/// <summary>
/// Represents a node in a flow blueprint
/// </summary>
public class FlowBlueprintNode
{
    public int Id { get; set; }
    public int FlowBlueprintId { get; set; }
    public string NodeKey { get; set; } = string.Empty;
    public string EntityType { get; set; } = string.Empty;
    public int DisplayOrder { get; set; }

    // Navigation properties
    public virtual FlowBlueprint FlowBlueprint { get; set; } = null!;
}









