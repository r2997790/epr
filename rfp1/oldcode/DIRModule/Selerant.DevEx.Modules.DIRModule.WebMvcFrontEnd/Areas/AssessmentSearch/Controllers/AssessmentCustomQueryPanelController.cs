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
    [SearchPanel("CustomQueriesPanel", DxAssessmentAttributeScope.NAME)]
    public partial class AssessmentCustomQueryPanelController : CustomQueryPanelController<DxAssessment, AssessmentSearchSecurity>
    {
        protected override DevExSearchPanelIndexModel BuildIndexModel(SearchControllerData data, ISearchController searchController, string nodeId)
        {
            return new CustomQueryBrowser(GetControllerUrl(), data, searchController, nodeId);
        }
    }

    [ComponentDescriptor(DIRModuleComponentIdentifier.SEARCH_ASSESSMENT, SecurityObjectType = typeof(AssessmentSearchSecurity), VerifyControllerData = true, VerifyRequestSecurity = true)]
    [SearchPanel("CustomQueriesPanelCreate", DxAssessmentAttributeScope.NAME)]
    public partial class AssessmentCustomQueryPanelCreateController : CustomQueryPanelCreateController<DxAssessment, AssessmentSearchSecurity>
    {
        protected override DevExSearchPanelIndexModel BuildIndexModel(SearchControllerData data, ISearchController searchController, string nodeId)
        {
            return new CustomQueryDesignerCreate(GetControllerUrl(), data, searchController, nodeId);
        }
    }

    [ComponentDescriptor(DIRModuleComponentIdentifier.SEARCH_ASSESSMENT, SecurityObjectType = typeof(AssessmentSearchSecurity), VerifyControllerData = true, VerifyRequestSecurity = true)]
    [SearchPanel("CustomQueriesPanelEdit", DxAssessmentAttributeScope.NAME)]
    public partial class CustomQueryPanelEditController : CustomQueryPanelEditController<DxAssessment, AssessmentSearchSecurity>
    {
        protected override DevExSearchPanelIndexModel BuildIndexModel(SearchControllerData data, ISearchController searchController, string nodeId)
        {
            return new CustomQueryDesignerEdit(GetControllerUrl(), data, searchController, nodeId);
        }
    }

    [ComponentDescriptor(DIRModuleComponentIdentifier.SEARCH_ASSESSMENT, SecurityObjectType = typeof(AssessmentSearchSecurity), VerifyControllerData = true, VerifyRequestSecurity = true)]
    [SearchPanel("CustomQueriesPanelRun", DxAssessmentAttributeScope.NAME)]
    public partial class AssessmentCustomQueryPanelRunController : CustomQueryRunController<DxAssessment, AssessmentSearchSecurity>
    {
        protected override DevExSearchPanelIndexModel BuildIndexModel(SearchControllerData data, ISearchController searchController, string nodeId)
        {
            return new CustomQueriesRunModel(GetControllerUrl(), data, searchController, nodeId);
        }
    }
}