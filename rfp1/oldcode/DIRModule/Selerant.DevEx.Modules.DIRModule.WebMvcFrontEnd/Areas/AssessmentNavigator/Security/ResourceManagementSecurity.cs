using Selerant.DevEx.BusinessLayer.Authorization;
using Selerant.DevEx.Modules.DIRModule.BackEnd.AssessmentManagement.BusinessObjects;
using Selerant.DevEx.Modules.DIRModule.BackEnd.Common;
using Selerant.DevEx.Modules.DIRModule.WebMvcFrontEnd.Areas.AssessmentNavigator.Security.Interfaces;
using Selerant.DevEx.Web.Security;

namespace Selerant.DevEx.Modules.DIRModule.WebMvcFrontEnd.Areas.AssessmentNavigator.Security
{
	/// <summary>
	/// Security implementation for Resource Management panel
	/// </summary>
	public class ResourceManagementSecurity : NavigatorPanelSecurityBase<DxAssessment>, IManageAssessmentSecurity, ICreateAssessmentSecurity
	{
		#region Fields

		private UserRightsOnObject materialCreationRights;

		private UserRightsOnObject resourceManagementRights;

		private UserRightsOnObject assessmentCreationRights;

		#endregion Fields

		#region Properties

		public UserRightsOnObject MaterialCreationRights
		{
			get
			{
				if (materialCreationRights == null)
					materialCreationRights = GetFunctionalBlockUserRights(Constants.AssessmentNavigatorFB.ASSESSMENT_NAVIGATOR_MATERIAL_CREATION);

				return materialCreationRights;
			}
		}

		private UserRightsOnObject ResourceManagementRights
		{
			get
			{
				if (resourceManagementRights == null)
					resourceManagementRights = GetFunctionalBlockUserRights(Constants.AssessmentNavigatorFB.ASSESSMENT_RESOURCE_MANAGEMENT_TAB);

				return resourceManagementRights;
			}
		}

		private UserRightsOnObject AssessmentCreationRights
		{
			get
			{
				if (assessmentCreationRights == null)
					assessmentCreationRights = GetFunctionalBlockUserRights(Constants.AssessmentFB.NEW_ASSESSMENT_CREATION_DIR_03);

				return assessmentCreationRights;
			}
		}

		public bool CanView => PanelRights.CanRead && ResourceManagementRights.CanRead;

		public bool CanDelete => CanView && PanelRights.CanDelete && ResourceManagementRights.CanDelete;

		public bool CanUpdate => CanView && PanelRights.CanUpdate && ResourceManagementRights.CanUpdate;

		public bool CanEditOrDelete => CanUpdate && CanDelete;

		public bool CanCreateMaterial => CanEditOrDelete && MaterialCreationRights.CanCreate;

		#endregion Properties

		#region IManageAssessmentSecurity

		// If for security management LcStages and Destinations needs to be different
		public bool CanUpdateLcStages => CanUpdate && CanDelete;
		public bool CanUpdateDestinations => CanUpdate && CanDelete;

		#endregion

		#region ICreateAssessmentSecurity

		public bool CanCreateAssessment => AssessmentCreationRights.CanRead && AssessmentCreationRights.CanCreate;

		public bool CanViewResourceManagement => ResourceManagementRights.CanRead;

		#endregion
	}
}