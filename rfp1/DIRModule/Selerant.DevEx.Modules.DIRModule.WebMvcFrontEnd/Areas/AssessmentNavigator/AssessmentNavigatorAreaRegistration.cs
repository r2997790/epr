using Selerant.DevEx.Infrastructure.Modules;
using Selerant.DevEx.Modules.DIRModule.ModuleBlocks._ThisProject;

namespace Selerant.DevEx.Modules.DIRModule.WebMvcFrontEnd.Areas.AssessmentNavigator
{
	public class AssessmentNavigatorAreaRegistration : ModuleAreaRegistration
	{
		public override string ModuleName => DIRModuleInfo.Instance.ModuleName;

		public override string AreaName => "AssessmentNavigator";
	}
}