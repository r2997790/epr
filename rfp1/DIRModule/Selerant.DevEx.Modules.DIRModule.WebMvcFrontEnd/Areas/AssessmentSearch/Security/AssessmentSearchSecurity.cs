using Selerant.DevEx.BusinessLayer;
using Selerant.DevEx.BusinessLayer.Authorization;
using Selerant.DevEx.Infrastructure.Modules.Searches.Security;
using Selerant.DevEx.Modules.DIRModule.BackEnd.AssessmentManagement.Attributes;
using Selerant.DevEx.Modules.DIRModule.BackEnd.Common;
using Selerant.DevEx.Modules.DIRModule.WebMvcFrontEnd.Areas.AssessmentNavigator.Security.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Selerant.DevEx.Modules.DIRModule.WebMvcFrontEnd.Areas.AssessmentSearch.Security
{
	public class AssessmentSearchSecurity : SearchSecurity, ICustomQuerySearchSecurity, ICreateAssessmentSecurity, IManageAssessmentSecurity
	{
		protected override DxAttributeScope Scope => DxAssessmentAttributeScope.NAME;

		protected override string StandardFblock => Constants.AssessmentFB.ASSESSMENT_SEARCH;

		protected override string ResultsExportFblock => Constants.AssessmentFB.ASMT_SEARCH_RESULT_EXPORT;

		#region Custom Query

		private UserRightsOnObject _customQueryFB;
		private UserRightsOnObject CustomQueryFB
		{
			get
			{
				if (_customQueryFB == null)
					_customQueryFB = GetFunctionalBlockUserRights(Constants.AssessmentFB.ASMT_SEARCH_CUSTOM_QUERY);
				return _customQueryFB;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		public bool CanUseCustomQuery
		{
			get { return CanUseStandard && CustomQueryFB.CanRead; }
		}

		/// <summary>
		/// 
		/// </summary>
		public bool CanNewCustomQuery
		{
			get { return CanUseCustomQuery && CustomQueryFB.CanCreate; }
		}

		/// <summary>
		/// 
		/// </summary>
		public bool CanEditCustomQuery
		{
			get { return CanUseCustomQuery && CustomQueryFB.CanUpdate; }
		}

		/// <summary>
		/// 
		/// </summary>
		public bool CanDeleteCustomQuery
		{
			get { return CanUseCustomQuery && CustomQueryFB.CanDelete; }
		}

		/// <summary>
		/// 
		/// </summary>
		public bool CanCopyCustomQuery
		{
			get { return CanUseCustomQuery && CustomQueryFB.CanCreate; }
		}

		/// <summary>
		/// 
		/// </summary>
		public bool CanCopyODataLinkCustomQuery
		{
			get { return CanUseCustomQuery; }
		}

		/// <summary>
		/// 
		/// </summary>
		public bool CanEditSpecificCustomQuery(DxQuery query)
		{
			return CanEditCustomQuery
				&& GetUserRightsOnQuery(query).CanRead
				&& GetUserRightsOnQuery(query).CanUpdate
				;
		}

		/// <summary>
		/// 
		/// </summary>
		public bool CanDeleteSpecificCustomQuery(DxQuery query)
		{
			return CanDeleteCustomQuery
				&& GetUserRightsOnQuery(query).CanRead
				&& GetUserRightsOnQuery(query).CanDelete
				;
		}

		/// <summary>
		/// 
		/// </summary>
		public bool CanSecureSpecificCustomQuery(DxQuery query)
		{
			return CanUseStandard && CustomQueryFB.CanAccess
				&& GetUserRightsOnQuery(query).CanRead
				&& GetUserRightsOnQuery(query).CanAccess
				;
		}

		#endregion

		#region ICreateAssessmentSecurity - Continue Draft Assessment

		public bool CanCreateAssessment
		{
			get
			{
				var assessmentCreationRights = GetFunctionalBlockUserRights(Constants.AssessmentFB.NEW_ASSESSMENT_CREATION_DIR_03);

				return assessmentCreationRights.CanRead && assessmentCreationRights.CanCreate;
			}
		}

		public bool CanViewResourceManagement
		{
			get
			{
				var assessmentResourcePartialViews = GetFunctionalBlockUserRights(Constants.AssessmentNavigatorFB.ASSESSMENT_RESOURCE_MANAGEMENT_TAB);

				return assessmentResourcePartialViews.CanRead;
			}
		}

		#endregion

		#region IManageAssessmentSecurity

		private bool CanManageAssessment
		{
			get
			{
				var assessmentCreationRights = GetFunctionalBlockUserRights(Constants.AssessmentFB.NEW_ASSESSMENT_CREATION_DIR_03);
				return assessmentCreationRights.CanUpdate && assessmentCreationRights.CanDelete;
			}
		}

		public bool CanUpdateLcStages => CanManageAssessment;
		public bool CanUpdateDestinations => CanManageAssessment;

		#endregion

		public override bool CanUseNode(string nodeId)
        {
            switch (nodeId)
            {
                case "AssessmentBasicHolder":
                case "AssessmentRecent":
                case "ASSESSMENTSearchResult":
                    return CanUseStandard;
            }

            return base.CanUseNode(nodeId);
        }
    }
}