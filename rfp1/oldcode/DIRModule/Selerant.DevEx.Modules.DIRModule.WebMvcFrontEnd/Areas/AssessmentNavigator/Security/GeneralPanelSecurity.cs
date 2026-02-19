using Selerant.DevEx.BusinessLayer.Authorization;
using Selerant.DevEx.Modules.DIRModule.BackEnd.AssessmentManagement.BusinessObjects;
using Selerant.DevEx.Modules.DIRModule.BackEnd.Common;
using Selerant.DevEx.Web.Security;

namespace Selerant.DevEx.Modules.DIRModule.WebMvcFrontEnd.Areas.AssessmentNavigator.Security
{
    /// <summary>
    /// Security implementation for Vendors panel
    /// </summary>
    public class GeneralPanelSecurity : NavigatorPanelSecurityBase<DxAssessment>
    {
		#region Fields

		private UserRightsOnObject generalTabRights;

		#endregion Fields

		#region Properties

		private UserRightsOnObject GeneralTabRights
		{
			get
			{
				if (generalTabRights == null)
				{
					generalTabRights = GetFunctionalBlockUserRights(Constants.AssessmentNavigatorFB.ASSESSMENT_GENERAL_TAB);
				}

				return generalTabRights;
			}
		}

		public bool CanView => PanelRights.CanRead && GeneralTabRights.CanRead;

		public bool CanEdit => CanView && PanelRights.CanUpdate && GeneralTabRights.CanUpdate;

		#endregion Properties
	}
}