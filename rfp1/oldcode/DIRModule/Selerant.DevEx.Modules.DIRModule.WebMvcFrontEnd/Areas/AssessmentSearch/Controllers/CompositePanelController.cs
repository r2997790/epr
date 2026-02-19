using System;
using Selerant.DevEx.Modules.DIRModule.WebMvcFrontEnd._ThisProject;
using Selerant.DevEx.Modules.DIRModule.WebMvcFrontEnd.Areas.AssessmentSearch.Models.Panels.Composite;
using Selerant.DevEx.Modules.DIRModule.WebMvcFrontEnd.Areas.AssessmentSearch.Security;
using Selerant.DevEx.Web;
using Selerant.DevEx.WebMvcModules.Infrastructure.Searches.Models;

namespace Selerant.DevEx.Modules.DIRModule.WebMvcFrontEnd.Areas.AssessmentSearch.Controllers
{
    [ComponentDescriptor(DIRModuleComponentIdentifier.SEARCH_ASSESSMENT, SecurityObjectType = typeof(AssessmentSearchSecurity), VerifyControllerData = true, VerifyRequestSecurity = true)]
    public partial class CompositePanelController : AssessmentSearchPanelController
    {
        protected override DevExSearchPanelIndexModel BuildIndexModel(SearchControllerData data, ISearchController searchController, string nodeId)
        {
            return new IndexModel(GetControllerUrl(), data, searchController, nodeId);
        }

        protected override bool GetHasRights(SearchControllerData data, string nodeId = null)
        {
            if (nodeId == null)
                throw new ArgumentNullException("nodeId");

            return GetSecurityObject(data).CanUseNode(nodeId);
        }
    }
}