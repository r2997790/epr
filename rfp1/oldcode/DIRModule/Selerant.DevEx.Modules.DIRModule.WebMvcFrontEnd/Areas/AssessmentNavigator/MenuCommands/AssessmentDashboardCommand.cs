using Selerant.DevEx.CommonComponents;
using Selerant.DevEx.Modules.DIRModule.BackEnd.AssessmentManagement.BusinessObjects;
using Selerant.DevEx.Modules.DIRModule.Configuration.Resources;
using Selerant.DevEx.Modules.DIRModule.WebMvcFrontEnd.Areas.AssessmentNavigator.Security;
using Selerant.DevEx.Web;
using Selerant.DevEx.Web.Security;
using Selerant.DevEx.WebMvcModules.Infrastructure.Base;
using Selerant.DevEx.WebMvcModules.Infrastructure.Navigators.MenuCommands;
using Selerant.DevEx.WebMvcModules.Infrastructure.Navigators.Models;
using Selerant.DevEx.WebPages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Selerant.DevEx.Modules.DIRModule.WebMvcFrontEnd.Areas.AssessmentNavigator.MenuCommands
{
    public class AssessmentDashboardCommand : IMenuCommand<DxAssessment>
    {
        public AssessmentDashboardCommand()
        {
        }

        public bool CheckSecurity(NavigatorSecurity<DxAssessment> securityObject)
        {
            return ((AssessmentNavigatorSecurity)securityObject).CanView;
        }

        public ActionResult Execute(NavigatorControllerData<DxAssessment> controllerData, Dictionary<string, object> customData)
        {
            ActionResult result = MVC_DIR.AssessmentNavigator.AssessmentDashboardDialog.AssessmentDashboardDialogIndex(controllerData, controllerData.TargetObject.IdentifiableString);

            JSOpenDialogActivity activity = new JSOpenDialogActivity("A1");
            activity.Url = ComponentDataHelper.AddDataToUrl(Utilities.MapUrlPath(CurrentRequestData.Instance.MvcUrlHelper.Action(result)), controllerData);
            activity.SetCaption(Locale.GetString(ResourceFiles.AssessmentManager, "AssessmentDashboard"));
            activity.Width = short.MaxValue;
            activity.Height = short.MaxValue;
            activity.IsResizable = false;

            return new JSActivityAjaxResult(activity);
        }
    }
}