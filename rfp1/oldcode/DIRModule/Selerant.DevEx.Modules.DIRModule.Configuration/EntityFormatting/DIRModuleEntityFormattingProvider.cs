using Selerant.DevEx.Configuration.Formatting;
using Selerant.DevEx.Modules.DIRModule.ModuleBlocks._ThisProject;

namespace Selerant.DevEx.Modules.DIRModule.Configuration.EntityFormatting
{
    /// <summary>
    /// Represents the module changes provider of entity formatting
    /// </summary>
    internal class DIRModuleEntityFormattingProvider : INeedToAddModuleEntityFormatting
    {
        public EntityFormattingModuleDefinitionVO ProvideDefinition() =>
            new EntityFormattingModuleDefinitionVO(DIRModuleInfo.Instance, new DIRModuleEntityFormatOverlayRepository());
    }
}
