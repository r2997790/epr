using Selerant.DevEx.BusinessLayer.Authorization;
using Selerant.DevEx.Modules.DIRModule.BackEnd.AssessmentManagement.BusinessObjects;
using Selerant.DevEx.Modules.DIRModule.BackEnd.Common;
using Selerant.DevEx.Web.Security;

namespace Selerant.DevEx.Modules.DIRModule.WebMvcFrontEnd.Areas.AssessmentNavigator.Security
{
    /// <summary>
    /// Security implementation for Assessment Result panel
    /// </summary>
    public class AssessmentResultsSecurity : NavigatorPanelSecurityBase<DxAssessment>
    {
		#region Fields

		private UserRightsOnObject assessmentResultTabRights;

		#endregion Fields

		#region Properties

		private UserRightsOnObject AssessmentResultTabRights
		{
			get
			{
				if (assessmentResultTabRights == null)
				{
					assessmentResultTabRights = GetFunctionalBlockUserRights(Constants.AssessmentNavigatorFB.ASSESSMENT_RESULT_TAB);
				}
				return assessmentResultTabRights;
			}
		}

		public bool CanView => PanelRights.CanRead && AssessmentResultTabRights.CanRead;

		#endregion Properties
	}
}