using Selerant.DevEx.CommonComponents;
using Selerant.DevEx.Modules.DIRModule.BackEnd.AssessmentManagement.BusinessObjects;
using Selerant.DevEx.Modules.DIRModule.Configuration.Resources;
using Selerant.DevEx.Modules.DIRModule.WebMvcFrontEnd.Areas.AssessmentNavigator.Security;
using Selerant.DevEx.Web;
using Selerant.DevEx.Web.Security;
using Selerant.DevEx.WebControls;
using Selerant.DevEx.WebMvcModules.Infrastructure.Base;
using Selerant.DevEx.WebMvcModules.Infrastructure.Navigators.MenuCommands;
using Selerant.DevEx.WebMvcModules.Infrastructure.Navigators.Models;
using Selerant.DevEx.WebPages;
using System.Collections.Generic;
using System.Web.Mvc;

namespace Selerant.DevEx.Modules.DIRModule.WebMvcFrontEnd.Areas.AssessmentNavigator.MenuCommands
{
	public class CopyCommand : IMenuCommand<DxAssessment>
	{
		public CopyCommand()
		{
		}

		public ActionResult Execute(NavigatorControllerData<DxAssessment> controllerData, Dictionary<string, object> customData)
		{
			ActionResult result = MVC_DIR.AssessmentNavigator.CopyAssessment.CopyAssessment(controllerData, controllerData.TargetObject.IdentifiableString);

			JSOpenDialogActivity activity = new JSOpenDialogActivity("A1");
			activity.Url = ComponentDataHelper.AddDataToUrl(Utilities.MapUrlPath(CurrentRequestData.Instance.MvcUrlHelper.Action(result)), controllerData);
			activity.SetCaption(Locale.GetString(ResourceFiles.AssessmentManager, "Copy_DialogTitle"));
			activity.UrlParameters.Add(new StringKeyValueListItem("Id", controllerData.TargetObject.IdentifiableString));
			activity.Width = 1000;
			activity.Height = 600;

			return new JSActivityAjaxResult(activity);
		}

		public bool CheckSecurity(NavigatorSecurity<DxAssessment> securityObject)
		{
			return ((AssessmentNavigatorSecurity)securityObject).CanCopy;
		}
	}
}