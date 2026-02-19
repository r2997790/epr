using System;
using System.Collections.Generic;
using Selerant.DevEx.Modules.DIRModule.BackEnd.AssessmentManagement.BusinessObjects;
using Selerant.DevEx.Modules.DIRModule.Configuration.Grids;
using Selerant.DevEx.Modules.DIRModule.ModuleBlocks._ThisProject;
using Selerant.DevEx.WebMvcModules.Infrastructure.Controls;
using Selerant.DevEx.WebMvcModules.Infrastructure.Searches.Models;

namespace Selerant.DevEx.Modules.DIRModule.WebMvcFrontEnd.Areas.AssessmentSearch.Models.Panels.Result
{
	public class SearchResultIndexModel : SearchResultPanelBaseModel<DxAssessment, DxAssessment, DxAssessmentCollection>
    {
        #region Constants

        public const string GRID_ID = GridNames.ASSESSMENT_SEARCH_RESULT;
        private const string JS_TYPE_NAME = "DX.Mvc.AssessmentSearchResultPanel";

        private const string OPEN_ACTION = "Open@SearchManager.SMOpen";
        private const string ADDRECENT_ACTION = "AddRecent@SearchManager.SMAddRecent";
		private const string CONTINUE_ACTION = "Continue@DIR_Controls.SMContinueDraftAssessment";

		#endregion

		#region Properties - Overrides

		public override string FixedGridId => GRID_ID;
        public override string JsTypeName => JS_TYPE_NAME;

		public override bool SkipGridBaseActionItems => true;

		// Use DIRmodule Actions cell converter to show diffrent links for DRAFT or other like DEVELOPMENT assessment status
		public override string ResultPanelActionConverter => $"{DIRModuleInfo.Instance.ModuleName}.AssessmentResultPanelActionConverter";
		// Render unclickble cell link for DRAFT assessment
		public override string ResultPanelReferenceConverter => $"{DIRModuleInfo.Instance.ModuleName}.AssessmentSearchResultPanelReferenceConverter";

		#endregion

		#region Constructors

		public SearchResultIndexModel(string controllerUrl, SearchControllerData data, ISearchController searchController, string nodeId)
            : base(controllerUrl, data, searchController, nodeId)
        {
        }

        #endregion

        #region Methods - Overrides

        public override void AddCustomActionItems(List<string> actionItems)
        {
            actionItems.Add(OPEN_ACTION);
            actionItems.Add(ADDRECENT_ACTION);
			actionItems.Add(CONTINUE_ACTION);
		}

        protected override void InitializeResources(List<ControlResource> resources)
        {
            base.InitializeResources(resources);

            resources.Add(new ControlJsFile(DIRModuleInfo.Instance, "~/Areas/AssessmentSearch/Views/ResultPanel/AssessmentSearchResultPanel.js"));
        }

		#endregion
	}
}