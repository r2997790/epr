using Selerant.DevEx.Modules.DIRModule.BackEnd.AssessmentManagement.BusinessObjects;
using Selerant.DevEx.Modules.DIRModule.WebMvcFrontEnd.Areas.AssessmentNavigator.Security;
using Selerant.DevEx.WebControls;
using Selerant.DevEx.WebMvcModules.Infrastructure.Navigators.Models;
using Selerant.DevEx.BusinessLayer;

namespace Selerant.DevEx.Modules.DIRModule.WebMvcFrontEnd.Areas.AssessmentNavigator.Models.Home
{
	/// <summary>
	/// Header model for Assessment navigator
	/// </summary>
	public class AssessmentNavigatorHeaderModel : NavigatorHeaderModel<DxAssessment, AssessmentNavigatorSecurity>
	{
		/// <summary>
		/// Default Constructor
		/// </summary>
		/// <param name="controllerUrl"></param>
		/// <param name="navigatorControllerData"></param>
		public AssessmentNavigatorHeaderModel(string controllerUrl, NavigatorControllerData<DxAssessment> navigatorControllerData) 
			: base(controllerUrl, navigatorControllerData)
		{
			Caption = TargetObject.ToLongDescriptionString(DxUser.CurrentUser.GetDxCulture(), new DxFormattingContext());
			Subcaption = TargetObject.ToShortDescriptionString(DxUser.CurrentUser.GetDxCulture(), new DxFormattingContext());
		}

		protected override void FillScriptControlDescriptor(GxScriptControlDescriptor scriptControlDescriptor)
		{
			base.FillScriptControlDescriptor(scriptControlDescriptor);
			scriptControlDescriptor.TypeName = "DX.DIRModule.AssessmentNavigator.Home.Header";
		}

        protected override bool InnerIsMenuItemEnabled(string itemKey)
        {
            if (!TargetObject.Exists())
                return false;

            switch (itemKey)
            {
                case "Copy":
                    return NavigatorSecurity.CanExecuteCopy;
                case MENU_BUTTON_KEY_DELETE:
                    return NavigatorSecurity.CanExecuteDelete;
                case "AssessmentDashboard":
                    return NavigatorSecurity.CanExecuteAssessmentDashboard;
            }

            return base.InnerIsMenuItemEnabled(itemKey);
        }
    }
}