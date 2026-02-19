using Selerant.DevEx.Modules.DIRModule.BackEnd.AssessmentManagement.BusinessObjects;
using Selerant.DevEx.Modules.DIRModule.BackEnd.Searches;
using Selerant.DevEx.Modules.DIRModule.Configuration.Grids;
using Selerant.DevEx.Modules.DIRModule.ModuleBlocks._ThisProject;
using Selerant.DevEx.WebMvcModules.Infrastructure.Controls;
using Selerant.DevEx.WebMvcModules.Infrastructure.Searches.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Selerant.DevEx.Modules.DIRModule.WebMvcFrontEnd.Areas.AssessmentSearch.Models.Panels.Recent
{
    public class IndexModel : RecentSearchPanelBaseModel<DxAssessment, DxAssessmentCollection, DxRecentAssessment, DxRecentAssessmentCollection>
    {
        private const string JS_TYPE_NAME = "DX.DIRModule.AssessmentSearch.AssessmentSearchRecentPanel";

        public const string GRID_ID = GridNames.ASSESSMENT_SEARCH_RECENT;
        public override string JsTypeName => JS_TYPE_NAME;
        public override string FixedGridId => GRID_ID;

        public IndexModel(string controllerUrl, SearchControllerData data, ISearchController searchController, string nodeId)
            : base(controllerUrl, data, searchController, nodeId)
        {
        }

        protected override void InitializeResources(List<ControlResource> resources)
        {
            base.InitializeResources(resources);

            resources.Add(new ControlJsFile(DIRLinks.Areas.AssessmentSearch.Views.RecentPanel.AssessmentRecentPanel_ts));
        }
    }
}