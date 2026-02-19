using Autofac;
using Selerant.DevEx.BusinessLayer.Base.Components;
using Selerant.DevEx.Modules.DIRModule.BackEnd.AssessmentManagement.BusinessObjects;
using Selerant.DevEx.Modules.DIRModule.Configuration.Resources;
using Selerant.DevEx.Modules.DIRModule.WebMvcFrontEnd._ThisProject.Infrastructure.HostMenu;
using Selerant.DevEx.Modules.DIRModule.WebMvcFrontEnd.Areas.AssessmentNavigator.MenuCommands;
using Selerant.DevEx.WebMvcModules.Infrastructure.Navigators.MenuCommands;
using Selerant.DevEx.WebMvcModules.Infrastructure.Navigators.Models;

namespace Selerant.DevEx.Modules.DIRModule.WebMvcFrontEnd._ThisProject.Wirings
{
	public class WiringNavigatorCommands
	{
		public void Wiring(ContainerBuilder builder)
		{
			var commandRegistry = new MenuCommandWiring();

            MenuCommandFactory<DxAssessment>.RegisterCommand<SharingCommand>(builder, NavigatorHeaderModel.MENU_BUTTON_KEY_SHARING);
			MenuCommandFactory<DxAssessment>.RegisterCommand<CopyCommand>(builder, "Copy");
            MenuCommandFactory<DxAssessment>.RegisterCommand<AssessmentDashboardCommand>(builder, "AssessmentDashboard");

			commandRegistry.RegisterCommands(builder);

			builder.Register(c => new SearchAssessmentUrlProvider()).As<IComponentUrlProvider>();
			builder.Register(c => new CreateAssessmentUrlProvider()).As<IComponentUrlProvider>();
		}
	}
}