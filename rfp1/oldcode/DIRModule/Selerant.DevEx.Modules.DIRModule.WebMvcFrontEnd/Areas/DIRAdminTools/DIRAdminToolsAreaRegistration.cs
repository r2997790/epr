using Selerant.DevEx.Modules.DIRModule.ModuleBlocks._ThisProject;
using Selerant.DevEx.Infrastructure.Modules;

namespace Selerant.DevEx.Modules.DIRModule.WebMvcFrontEnd.Areas.DIRAdminTools
{
    public class DIRAdminToolsAreaRegistration : ModuleAreaRegistration
    {
        public override string ModuleName { get { return DIRModuleInfo.Instance.ModuleName; } }

        public override string AreaName { get { return "DIRAdminTools"; } }
    }
}