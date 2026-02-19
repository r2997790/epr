using EPR.Domain.Entities;

namespace EPR.Application.Services;

public interface IFlowBlueprintService
{
    Task<List<FlowBlueprint>> GetAllBlueprintsAsync();
    Task<FlowBlueprint?> GetBlueprintByKeyAsync(string key);
    Task<FlowBlueprint?> GetBlueprintByIdAsync(int id);
    Task<FlowBlueprint> CreateBlueprintAsync(FlowBlueprint blueprint);
    Task<bool> SeedDefaultBlueprintsAsync();
}









