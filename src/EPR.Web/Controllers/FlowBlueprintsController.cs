using EPR.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace EPR.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
public class FlowBlueprintsController : ControllerBase
{
    private readonly IFlowBlueprintService _flowBlueprintService;

    public FlowBlueprintsController(IFlowBlueprintService flowBlueprintService)
    {
        _flowBlueprintService = flowBlueprintService;
    }

    /// <summary>
    /// Get all flow blueprints
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<List<object>>> GetAll()
    {
        var blueprints = await _flowBlueprintService.GetAllBlueprintsAsync();
        return Ok(blueprints.Select(b => new
        {
            b.Id,
            b.Key,
            b.Name,
            b.Purpose,
            b.DefaultMetric,
            Nodes = b.Nodes.OrderBy(n => n.DisplayOrder).Select(n => new
            {
                n.NodeKey,
                n.EntityType
            }),
            Edges = b.Edges.Select(e => new
            {
                e.FromNodeKey,
                e.ToNodeKey
            })
        }));
    }

    /// <summary>
    /// Get a flow blueprint by key
    /// </summary>
    [HttpGet("{key}")]
    public async Task<ActionResult<object>> GetByKey(string key)
    {
        var blueprint = await _flowBlueprintService.GetBlueprintByKeyAsync(key);
        if (blueprint == null)
        {
            return NotFound();
        }

        return Ok(new
        {
            blueprint.Id,
            blueprint.Key,
            blueprint.Name,
            blueprint.Purpose,
            blueprint.DefaultMetric,
            Nodes = blueprint.Nodes.OrderBy(n => n.DisplayOrder).Select(n => new
            {
                n.NodeKey,
                n.EntityType
            }),
            Edges = blueprint.Edges.Select(e => new
            {
                e.FromNodeKey,
                e.ToNodeKey
            })
        });
    }

    /// <summary>
    /// Seed default Sankey blueprints
    /// </summary>
    [HttpPost("seed-defaults")]
    public async Task<ActionResult> SeedDefaults()
    {
        try
        {
            var seeded = await _flowBlueprintService.SeedDefaultBlueprintsAsync();
            if (seeded)
            {
                return Ok(new { message = "Default blueprints seeded successfully" });
            }
            return Ok(new { message = "Blueprints already exist" });
        }
        catch (FileNotFoundException ex)
        {
            return NotFound(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = ex.Message });
        }
    }
}









