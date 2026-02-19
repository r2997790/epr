using Selerant.DevEx.BusinessLayer;
using Selerant.DevEx.BusinessLayer.Authorization;
using Selerant.DevEx.Modules.DIRModule.BackEnd.Common;
using Selerant.DevEx.Web.Security;
using System;

namespace Selerant.DevEx.Modules.DIRModule.WebMvcFrontEnd.Areas.DIRAdminTools.Security
{
    public class AdminToolsAssessmentTypesSecurity : AdminToolSecurity
    {
		#region Constructors

		public AdminToolsAssessmentTypesSecurity(DxUser user)
			: base(user)
		{

		}

		#endregion Constructors

		#region Overrides

		public override string MainFunctionalBlockCode
        {
            get { return Constants.AssessmentAdminTools.ADMIN_TOOLS_ASSESSMENT_TYPES; }
        }

		#endregion Overrides

		#region Fields

		private UserRightsOnObject adminToolsAssessmentTypesRights;

		#endregion Fields

		#region Properties

		private UserRightsOnObject AdminToolsAssessmentTypesRights
		{
			get
			{
				if (adminToolsAssessmentTypesRights == null)
				{
					adminToolsAssessmentTypesRights = GetFunctionalBlockUserRights(MainFunctionalBlockCode);
				}
				return adminToolsAssessmentTypesRights;
			}
		}

		public Boolean CanView
        {
            get { return CanShowContent && AdminToolsAssessmentTypesRights.CanRead; }
        }

        public bool CanEdit
        {
            get
            {
                return CanShowContent && Rights.CanUpdate & AdminToolsAssessmentTypesRights.CanUpdate;
            }
        }

        public bool CanDelete
        {
            get
            {
                return CanShowContent && Rights.CanDelete && AdminToolsAssessmentTypesRights.CanDelete;
            }
        }

        public bool CanAdd
        {
            get
            {
				return CanShowContent && Rights.CanCreate && AdminToolsAssessmentTypesRights.CanCreate;

			}
        }

        public bool CanCreate
        {
            get
            {
				return CanAdd;

			}
        }

        public bool CanCreateOrUpdate
        {
            get
            {
                return CanCreate || CanEdit;
            }
        }

        #endregion
    }
}