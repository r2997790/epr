using EPR.Data;
using EPR.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace EPR.Application.Services;

public class FlowBlueprintService : IFlowBlueprintService
{
    private readonly EPRDbContext _context;

    public FlowBlueprintService(EPRDbContext context)
    {
        _context = context;
    }

    public async Task<List<FlowBlueprint>> GetAllBlueprintsAsync()
    {
        return await _context.FlowBlueprints
            .Include(b => b.Nodes)
            .Include(b => b.Edges)
            .OrderBy(b => b.Name)
            .ToListAsync();
    }

    public async Task<FlowBlueprint?> GetBlueprintByKeyAsync(string key)
    {
        return await _context.FlowBlueprints
            .Include(b => b.Nodes)
            .Include(b => b.Edges)
            .FirstOrDefaultAsync(b => b.Key == key);
    }

    public async Task<FlowBlueprint?> GetBlueprintByIdAsync(int id)
    {
        return await _context.FlowBlueprints
            .Include(b => b.Nodes)
            .Include(b => b.Edges)
            .FirstOrDefaultAsync(b => b.Id == id);
    }

    public async Task<FlowBlueprint> CreateBlueprintAsync(FlowBlueprint blueprint)
    {
        blueprint.CreatedAt = DateTime.UtcNow;
        _context.FlowBlueprints.Add(blueprint);
        await _context.SaveChangesAsync();
        return blueprint;
    }

    public async Task<bool> SeedDefaultBlueprintsAsync()
    {
        // Check if blueprints already exist
        if (await _context.FlowBlueprints.AnyAsync())
        {
            return false; // Already seeded
        }

        var jsonPath = Path.Combine(Directory.GetCurrentDirectory(), "default_sankey_blueprints.json");
        if (!File.Exists(jsonPath))
        {
            // Try parent directory
            jsonPath = Path.Combine(Directory.GetCurrentDirectory(), "..", "..", "default_sankey_blueprints.json");
            if (!File.Exists(jsonPath))
            {
                throw new FileNotFoundException("default_sankey_blueprints.json not found");
            }
        }

        var jsonContent = await File.ReadAllTextAsync(jsonPath);
        var blueprintsData = System.Text.Json.JsonSerializer.Deserialize<DefaultBlueprintsData>(jsonContent);

        if (blueprintsData?.Blueprints == null)
        {
            return false;
        }

        foreach (var blueprintData in blueprintsData.Blueprints)
        {
            var blueprint = new FlowBlueprint
            {
                Key = blueprintData.Key,
                Name = blueprintData.Name,
                Purpose = blueprintData.Purpose,
                DefaultMetric = blueprintData.DefaultMetric,
                CreatedAt = DateTime.UtcNow
            };

            // Add nodes
            int displayOrder = 0;
            foreach (var nodeData in blueprintData.Nodes)
            {
                blueprint.Nodes.Add(new FlowBlueprintNode
                {
                    NodeKey = nodeData.NodeKey,
                    EntityType = nodeData.EntityType,
                    DisplayOrder = displayOrder++
                });
            }

            // Add edges
            foreach (var edgeData in blueprintData.Edges)
            {
                blueprint.Edges.Add(new FlowBlueprintEdge
                {
                    FromNodeKey = edgeData.From,
                    ToNodeKey = edgeData.To
                });
            }

            _context.FlowBlueprints.Add(blueprint);
        }

        await _context.SaveChangesAsync();
        return true;
    }

    private class DefaultBlueprintsData
    {
        public string Version { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public List<BlueprintData> Blueprints { get; set; } = new();
    }

    private class BlueprintData
    {
        public string Key { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Purpose { get; set; } = string.Empty;
        public List<NodeData> Nodes { get; set; } = new();
        public List<EdgeData> Edges { get; set; } = new();
        public string DefaultMetric { get; set; } = string.Empty;
    }

    private class NodeData
    {
        public string NodeKey { get; set; } = string.Empty;
        public string EntityType { get; set; } = string.Empty;
    }

    private class EdgeData
    {
        public string From { get; set; } = string.Empty;
        public string To { get; set; } = string.Empty;
    }
}









