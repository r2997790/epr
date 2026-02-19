using Selerant.DevEx.Modules.DIRModule.BackEnd.AssessmentManagement.Attributes;
using Selerant.DevEx.Modules.DIRModule.BackEnd.AssessmentManagement.BusinessObjects;
using Selerant.DevEx.Modules.DIRModule.WebMvcFrontEnd._ThisProject;
using Selerant.DevEx.Modules.DIRModule.WebMvcFrontEnd.Areas.AssessmentSearch.Security;
using Selerant.DevEx.Web;
using Selerant.DevEx.WebMvcModules.Infrastructure.Searches.Controllers;
using Selerant.DevEx.WebMvcModules.Infrastructure.Searches.Helpers;
using Selerant.DevEx.WebMvcModules.Infrastructure.Searches.Models;

namespace Selerant.DevEx.Modules.DIRModule.WebMvcFrontEnd.Areas.AssessmentSearch.Controllers
{
    [ComponentDescriptor(DIRModuleComponentIdentifier.SEARCH_ASSESSMENT, SecurityObjectType = typeof(AssessmentSearchSecurity), VerifyControllerData = true, VerifyRequestSecurity = true)]
    [SearchPanel("AssessmentRecent", DxAssessmentAttributeScope.NAME)]
    public partial class AssessmentRecentSearchPanelController : RecentSearchPanelBaseController<DxAssessment, AssessmentSearchSecurity>
    {
        protected override DevExSearchPanelIndexModel BuildIndexModel(SearchControllerData data, ISearchController searchController, string nodeId)
        {
            return new Models.Panels.Recent.IndexModel(GetControllerUrl(), data, searchController, nodeId);
        }
    }
}