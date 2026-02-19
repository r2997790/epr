using Selerant.DevEx.BusinessLayer;
using Selerant.DevEx.Infrastructure.SearchManager.QuickBox.Extensions;
using Selerant.DevEx.Modules.DIRModule.BackEnd.AssessmentManagement.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Selerant.DevEx.Modules.DIRModule.WebMvcFrontEnd.Areas.AssessmentSearch.Controllers
{
    public class AssessmentQuickBoxAjaxControllerExtension : QuickBoxAjaxControllerExtension
    {
        public override DxAttributeScope Scope => new DxAssessmentAttributeScope();

        public override void SetBasicCriteria(SetBasicCriteriaParameter param)
        {
            var query = param.Query;
            var SearchText = param.SearchText;
            var SearchByField = param.SearchByField;

            DxQueryCriteria subjectCriteria = DxQueryCriteria.CreateCriteria("description", DxQueryCriteria.SqlOperator.Like, SearchText);
            query.Sorting.Items.Add(new DxQuerySortingItem("description", DxQuerySortingDirection.Ascending));

            DxQueryPredicate orPredicate = query.RootPredicate.AddORPredicate();
            orPredicate.AddCriteria(subjectCriteria);
        }
    }
}