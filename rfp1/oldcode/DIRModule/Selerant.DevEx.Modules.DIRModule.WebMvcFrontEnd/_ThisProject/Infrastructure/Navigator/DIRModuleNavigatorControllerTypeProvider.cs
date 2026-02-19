using System.Collections.Generic;
using Selerant.DevEx.Web;
using Selerant.DevEx.Infrastructure.ComponentsData;
using Selerant.DevEx.Modules.DIRModule.BackEnd.AssessmentManagement.Attributes;
using Selerant.DevEx.Modules.DIRModule.BackEnd.AssessmentManagement.BusinessObjects;
using Selerant.DevEx.Modules.DIRModule.ModuleBlocks._ThisProject;

namespace Selerant.DevEx.Modules.DIRModule.WebMvcFrontEnd._ThisProject.Infrastructure.Navigator
{
	public class DIRModuleNavigatorControllerTypeProvider : NavigatorControllerTypeProvider
	{
		protected override List<INavigatorControllerTypeVO> ProvideList()
		{

			return new List<INavigatorControllerTypeVO>()
			{
				new NavigatorControllerTypeVO<Areas.AssessmentNavigator.Controllers.HomeController, DxAssessment>(
				new ComponentIdentifier(DIRModuleComponentIdentifier.ASSESSMENT_NAVIGATOR),
				new DxAssessmentAttributeScope(),
				DIRModuleInfo.Instance.ModuleName)
			};

		}
	}
}