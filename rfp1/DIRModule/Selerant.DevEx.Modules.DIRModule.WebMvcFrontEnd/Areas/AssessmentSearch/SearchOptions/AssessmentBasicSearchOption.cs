using Selerant.DevEx.Modules.DIRModule.BackEnd.AssessmentManagement.BusinessObjects;
using Selerant.DevEx.Modules.DIRModule.WebMvcFrontEnd.Areas.AssessmentSearch.Security;
using Selerant.DevEx.WebMvcModules.Infrastructure.Searches.Helpers;
using Selerant.DevEx.WebMvcModules.Infrastructure.Searches.Models;

namespace Selerant.DevEx.Modules.DIRModule.WebMvcFrontEnd.Areas.AssessmentSearch.SearchOptions
{
    [SearchOption("AssessmentBasicPanel")]
    public class AssessmentBasicSearchOption : BaseSearchOption<DxAssessment, AssessmentSearchSecurity>
    {
        public override bool CanUseOption()
        {
            return SecurityObject.CanUseStandard;
        }
    }

    [SearchOption("ExtraPanels")]
    public class ExtraPanelsSearchOption : ExtraSearchOption<DxAssessment, AssessmentSearchSecurity>
    {

    }
}