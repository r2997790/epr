using System.Collections.Generic;
using Selerant.DevEx.BusinessLayer;
using Selerant.DevEx.Helpers;
using Selerant.DevEx.Modules.DIRModule.BackEnd.Common;
using Selerant.DevEx.Modules.DIRModule.WebMvcFrontEnd.Areas.AssessmentNavigator.Security.Interfaces;
using Selerant.DevEx.Web.Security;
using Selerant.DevEx.Web.Security.ObjectCreation.EventHandlers;

namespace Selerant.DevEx.Modules.DIRModule.WebMvcFrontEnd.Areas.AssessmentNavigator.Security
{
	public class CreateAssessmentObjectCreationSecurity : ObjectCreationSecurityObject, ICreateAssessmentSecurity, IManageAssessmentSecurity
	{
		#region Constructors

		public CreateAssessmentObjectCreationSecurity(Dictionary<string, string> parameters) : base(parameters)
		{
		}

		#endregion Contructors

		#region Overrides

		protected override string MainFunctionalBlockCode
		{
			get { return Constants.AssessmentFB.NEW_ASSESSMENT_CREATION_DIR_03; }
		}

		protected override void OnCustomizingSecurityOnObjectCreationData(CustomizeSecurityOnObjectCreationContext eventHandlerData)
		{
			// implement if needed
		}

		#endregion Overrides

		#region ICreateAssessmentSecurity

		public bool CanCreateAssessment => Rights.CanRead && Rights.CanCreate;

		public bool CanViewResourceManagement => SecurityHelper.GetUserRightsOnFB(Constants.AssessmentNavigatorFB.ASSESSMENT_RESOURCE_MANAGEMENT_TAB, DxUser.CurrentUser).CanRead;

		#endregion

		#region IManageAssessmentSecurity

		public bool CanUpdateLcStages => Rights.CanUpdate;

		public bool CanUpdateDestinations => Rights.CanUpdate;

		#endregion
	}
}