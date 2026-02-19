using Selerant.DevEx.CommonComponents;
using Selerant.DevEx.Modules.DIRModule.BackEnd.AssessmentManagement.BusinessObjects;
using Selerant.DevEx.Modules.DIRModule.WebMvcFrontEnd.Areas.AssessmentNavigator.Security;
using Selerant.DevEx.Web;
using Selerant.DevEx.Web.Security;
using Selerant.DevEx.WebMvcModules.Infrastructure.Base;
using Selerant.DevEx.WebMvcModules.Infrastructure.Navigators.Controllers;
using Selerant.DevEx.WebMvcModules.Infrastructure.Navigators.MenuCommands;
using Selerant.DevEx.WebMvcModules.Infrastructure.Navigators.Models;
using System.Collections.Generic;
using System.Web.Mvc;

namespace Selerant.DevEx.Modules.DIRModule.WebMvcFrontEnd.Areas.AssessmentNavigator.MenuCommands
{
    public class SharingCommand : IMenuCommand<DxAssessment>
    {
        public SharingCommand()
        {
        }

        public ActionResult Execute(NavigatorControllerData<DxAssessment> controllerData, Dictionary<string, object> customData)
        {
            JSOpenDialogActivity activity = new JSOpenDialogActivity("A1");
            activity.Url = SharingController.GetIndexActionUrl((ViewControlControllerData)controllerData, controllerData.TargetObject.IdentifiableString);
            activity.SetCaption(Locale.GetString(Locale.ModGlobal, "Sharing_DialogTitle"));
            return new JSActivityAjaxResult(activity) { AutoExecute = true };
        }

        public bool CheckSecurity(NavigatorSecurity<DxAssessment> securityObject)
        {
            return ((AssessmentNavigatorSecurity)securityObject).CanExecuteSharing;
        }
    }
}