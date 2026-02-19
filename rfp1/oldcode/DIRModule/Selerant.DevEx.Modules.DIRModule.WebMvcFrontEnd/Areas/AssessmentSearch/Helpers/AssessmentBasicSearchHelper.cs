using Selerant.DevEx.BusinessLayer;
using Selerant.DevEx.Modules.DIRModule.BackEnd.Searches;
using Selerant.DevEx.WebMvcModules.Infrastructure.Searches.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Selerant.DevEx.Modules.DIRModule.WebMvcFrontEnd.Areas.AssessmentSearch.Helpers
{
    public class AssessmentBasicSearchHelper : SearchHelper
    {
        public AssessmentBasicSearchHelper(DxQuery query, DxQuery.SetOperator? refinementOperator, DxAttributeScope scope, string searchOptionId)
            : base(query, scope, refinementOperator, searchOptionId)
        {
        }

        protected override string GridCustomizationTemplateId => CustomizedGridNames.SearchAssessmentBasic;

        protected override void SetPrimaryCriterias(ref DxQuery query)
        {
            query.RootPredicate.AddCriteria(DxQueryCriteria.CreateCriteria("code"));
            query.RootPredicate.AddCriteria(DxQueryCriteria.CreateCriteria("description"));
            query.RootPredicate.AddCriteria(DxQueryCriteria.CreateCriteria("status"));
            query.RootPredicate.AddCriteria(DxQueryCriteria.CreateCriteria("typeCode"));
            query.RootPredicate.AddCriteria(DxQueryCriteria.CreateCriteria("modDate"));

            if (DxUser.CurrentUser.IsExternal)
            {
                DxQueryCriteria partnerOrganizationCriteria = DxQueryCriteria.CreateCriteria("PartnerAssessments.partnerOrgCode", DxQueryCriteria.SqlOperator.Equal, DxUser.CurrentUser.PartnerOrganizationCode);
                partnerOrganizationCriteria.CaseSensitive = true;

                DxQueryCriteria partnerIsSharedCriteria = DxQueryCriteria.CreateCriteria("PartnerAssessments.isShared", DxQueryCriteria.SqlOperator.Equal, 1);
                partnerIsSharedCriteria.CaseSensitive = true;

                DxQueryCriteria partnersOrgCriteria = DxQueryCriteria.CreateCriteria("PartnerAssessments.partnerOrgCode", DxQueryCriteria.SqlOperator.Equal, DxSecureObject.publicPartnerCode);
                partnersOrgCriteria.CaseSensitive = true;

                query.RootPredicate.AddANDPredicate();

                var mainAndPredicate = query.RootPredicate.AddANDPredicate();
                var orPredicate = mainAndPredicate.AddORPredicate();
                var andPredicate = orPredicate.AddANDPredicate();

                orPredicate.AddCriteria(partnerOrganizationCriteria);
                andPredicate.AddCriteria(partnersOrgCriteria);
                andPredicate.AddCriteria(partnerIsSharedCriteria);
            }
        }

        protected override void SetWhereCriterias(ref DxQuery query)
        {
            if (SearchData == null)
                return;

            ApplyCommonSearchCriteria(query, "AssessmentCode", "code");
            ApplyCommonSearchCriteria(query, "AssessmentDescription", "description");
            ApplyCommonSearchCriteria(query, "AssessmentStatusList", "status", DxQueryCriteria.SqlOperator.In);
            ApplyCommonSearchCriteria(query, "AssessmentTypeList", "typeCode", DxQueryCriteria.SqlOperator.In);
        }

        protected override void SetLevelInfos(DxQueryGridManager queryGridManager)
        {
            if (queryGridManager.GridCustomization.LevelInfos.Count == 0)
                queryGridManager.GridCustomization.LevelInfos.Add(new DxGridLevelInfo());

            DxGridColumn column = GetGridColumnFromTemplate("Icon", null);
            queryGridManager.AddGridColumnCheckExistance(column);

            AddGridColumnInfoCheckVisibility(queryGridManager, "description", "Description", "DxAssessment.Description");
            AddGridColumnInfoCheckVisibility(queryGridManager, "code", "Code", "DxAssessment.Code");
            //AddGridColumnInfoCheckVisibility(queryGridManager, "typeCode", "Type", "DxAssessment.AssessmentTypeDescription");
            AddGridColumnInfoCheckVisibility(queryGridManager, "status", "Status", "DxAssessment.StatusDescription");
            AddGridColumnInfoCheckVisibility(queryGridManager, "modDate", "ModDate", "DxAssessment.ModDate");
        }
    }
}