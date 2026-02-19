using Selerant.DevEx.Modules.DIRModule.BackEnd.AssessmentManagement.BusinessObjects;
using Selerant.DevEx.Modules.DIRModule.WebMvcFrontEnd.Areas.AssessmentSearch.Security;
using Selerant.DevEx.WebMvcModules.Infrastructure.Searches.Helpers;
using Selerant.DevEx.WebMvcModules.Infrastructure.Searches.Models;

namespace Selerant.DevEx.Modules.DIRModule.WebMvcFrontEnd.Areas.AssessmentSearch.SearchOptions
{
    [SearchOption("CustomQueriesPanels")]
    public class AssessmentCustomQuerySearchOption : CustomQuerySearchOption<DxAssessment, AssessmentSearchSecurity>
    {
    }
}