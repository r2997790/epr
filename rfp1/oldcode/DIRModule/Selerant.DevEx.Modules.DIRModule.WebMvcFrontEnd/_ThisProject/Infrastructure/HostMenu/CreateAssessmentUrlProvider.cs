using System.Collections.Generic;
using Selerant.DevEx.Modules.DIRModule.WebMvcFrontEnd.Areas.AssessmentNavigator.Security;
using Selerant.DevEx.Web;
using Selerant.DevEx.Web.Infrastructure;
using Selerant.DevEx.Web.ObjectCreation;

namespace Selerant.DevEx.Modules.DIRModule.WebMvcFrontEnd._ThisProject.Infrastructure.HostMenu
{
	public class CreateAssessmentUrlProvider : ModuleComponentUrlProvider
	{
		#region Constructors
		
		public CreateAssessmentUrlProvider()
		: base($"Create|{DIRModuleComponentIdentifier.CREATE_ASSESSMENT}", MVC_DIR.AssessmentNavigator.CreateAssessmentDialog.NewAssessmentDialogIndex())
		{
		}

		#endregion

		protected override bool CanAccessComponent(BaseControllerData controllerData)
		{
			return ((CreateAssessmentObjectCreationSecurity)controllerData.SecurityObject).CanCreateAssessment;
		}

		protected override BaseControllerData CreateControllerData(Dictionary<string, string> parameters)
		{
			return new ObjectCreationControllerData(DIRModuleComponentIdentifier.CREATE_ASSESSMENT, parameters);
		}
	}
}