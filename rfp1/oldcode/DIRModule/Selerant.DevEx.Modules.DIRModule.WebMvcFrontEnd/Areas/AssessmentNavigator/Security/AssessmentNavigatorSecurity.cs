using Selerant.DevEx.BusinessLayer;
using Selerant.DevEx.BusinessLayer.Authorization;
using Selerant.DevEx.Modules.DIRModule.BackEnd.AssessmentManagement.BusinessObjects;
using Selerant.DevEx.Modules.DIRModule.BackEnd.Common;
using Selerant.DevEx.Modules.DIRModule.WebMvcFrontEnd.Areas.AssessmentNavigator.Security.Interfaces;
using Selerant.DevEx.Web.Security;
using Selerant.DevEx.WebMvcModules.Infrastructure.Navigators.Controllers;
using Selerant.DevEx.WebMvcModules.Infrastructure.Navigators.Models;
using static Selerant.DevEx.Modules.DIRModule.BackEnd.Common.Constants;

namespace Selerant.DevEx.Modules.DIRModule.WebMvcFrontEnd.Areas.AssessmentNavigator.Security
{
	public class AssessmentNavigatorSecurity : NavigatorSecurity<DxAssessment>, ISharingDialogSecurityObject, IManageAssessmentSecurity
	{
		#region Constructors

		/// <summary>
		/// Default Constructor
		/// </summary>
		/// <param name="controllerData"></param>
		public AssessmentNavigatorSecurity(NavigatorControllerData<DxAssessment> controllerData) 
			: base(controllerData, DxUser.CurrentUser)
		{
		}

		#endregion Constructors

		#region Fields

		private UserRightsOnObject objectCreationRights;

		#endregion Fields

		#region Properties

		private UserRightsOnObject ObjectCreationRights
		{
			get
			{
				if (objectCreationRights == null)
				{
					objectCreationRights = GetFunctionalBlockUserRights(Constants.AssessmentFB.NEW_ASSESSMENT_CREATION_DIR_03);
				}

				return objectCreationRights;
			}
		}

		public bool CanView => NavigatorRights.CanRead;

		public bool CanCreate => CanView && ObjectCreationRights.CanRead && ObjectCreationRights.CanCreate && MenuRights.CanCreate;

		public bool CanCopy => CanCreate;

		public bool CanDelete => CanView && NavigatorRights.CanDelete && ObjectCreationRights.CanDelete && MenuRights.CanDelete; 

		public bool CanExecuteSharing => CanUseMenuButton(NavigatorHeaderModel.MENU_BUTTON_KEY_SHARING, AssessmentNavigatorFB.ASSESSMENT_NAVIGATOR_SHARE, new UserRightsOnEntity(CommonRights.Read));

		public bool CanExecuteDelete => CanUseMenuButton(NavigatorHeaderModel.MENU_BUTTON_KEY_DELETE, AssessmentNavigatorFB.ASSESSMENT_NAVIGATOR_TOOLBAR_BTNS, new UserRightsOnEntity(CommonRights.Read))
									&& CanUseMenuButton(NavigatorHeaderModel.MENU_BUTTON_KEY_DELETE, AssessmentNavigatorFB.ASSESSMENT_NAVIGATOR_TOOLBAR_BTNS, new UserRightsOnEntity(CommonRights.Delete))
									&& CanDelete;

		public bool CanExecuteCopy => CanUseMenuButton("Copy", AssessmentNavigatorFB.ASSESSMENT_NAVIGATOR_TOOLBAR_BTNS, new UserRightsOnEntity(CommonRights.Read))
									&& CanUseMenuButton("Copy", AssessmentNavigatorFB.ASSESSMENT_NAVIGATOR_TOOLBAR_BTNS, new UserRightsOnEntity(CommonRights.Create))
									&& CanCopy;

        public bool CanExecuteAssessmentDashboard => CanUseMenuButton("AssessmentDashboard", AssessmentNavigatorFB.ASSESSMENT_NAVIGATOR_TOOLBAR_BTNS, new UserRightsOnEntity(CommonRights.Read)) && CanView;

		#endregion Properties

		#region Overrides

		public override bool CanViewContent => NavigatorRights.CanRead;

		#endregion

		#region IManageAssessmentSecurity

		public bool CanUpdateLcStages => NavigatorRights.CanUpdate && NavigatorRights.CanDelete;

		public bool CanUpdateDestinations => NavigatorRights.CanUpdate && NavigatorRights.CanDelete;

		#endregion
	}
}