using Selerant.DevEx.Modules.DIRModule.BackEnd.AssessmentManagement.BusinessObjects;
using Selerant.DevEx.Modules.DIRModule.Configuration.Grids;
using Selerant.DevEx.Modules.DIRModule.Configuration.Resources;
using Selerant.DevEx.WebControls;
using Selerant.DevEx.WebMvcModules.Helpers.jqGrid;
using Selerant.DevEx.WebMvcModules.Infrastructure.Controls;
using Selerant.DevEx.WebMvcModules.Infrastructure.Navigators.Models;
using System.Collections.Generic;
using System.Web.Mvc;

namespace Selerant.DevEx.Modules.DIRModule.WebMvcFrontEnd.Areas.AssessmentNavigator.Models.Panels.ResourceManagement.Shared
{
    public class OutputsGridTabModel : BaseGridTabModel
    {
		public override string GridID
		{
			get { return GridNames.OUTPUTS; }
		}

		public override string ResourceType => ResourceTabIds.OUTPUT;

		public override string EmptyGridMessage => Locale.GetString(ResourceFiles.AssessmentManager, "OutputsEmptyGrid");

        public override string GridUrl => Url.Action(MVC_DIR.AssessmentNavigator.ResourceManagement.GetOutputsTree((NavigatorPanelControllerData<DxAssessment>)ControllerData, null, true));

		public OutputsGridTabModel(string controllerUrl, NavigatorPanelControllerData<DxAssessment> navigatorPanelControllerData, decimal lcStageId, ViewMode viewMode = ViewMode.Navigator) : base(controllerUrl, navigatorPanelControllerData, lcStageId)
        {
			Notes = new NotesModel(controllerUrl, navigatorPanelControllerData, ResourceType, lcStageId, navigatorPanelControllerData.TargetObject.Code, viewMode);
        }

        protected override void InitializeResources(List<ControlResource> resources)
        {
            base.InitializeResources(resources);
            resources.Add(new ControlCssFile(DIRLinks.Areas.AssessmentNavigator.Views.ResourceManagement.Shared.OutputsGridTab_css));
            resources.Add(new ControlJsFile(DIRLinks.Areas.AssessmentNavigator.Views.ResourceManagement.Shared.OutputsGridTab_ts));
        }

        protected override void FillScriptControlDescriptor(GxScriptControlDescriptor scriptControlDescriptor)
        {
            base.FillScriptControlDescriptor(scriptControlDescriptor);
            scriptControlDescriptor.TypeName = "DX.DIRModule.AssessmentNavigator.ResourceManagement.Shared.OutputsGridTab";
        }

        protected override void InitializeForView()
        {
            base.InitializeForView();
            AddCssClass("dir-module-partial-outputs-tab");
        }

        public jqGrid CreateJqGrid()
        {
            string gridUrlAction = Url.Action(MVC_DIR.AssessmentNavigator.ResourceManagement.GetOutputsTree((NavigatorPanelControllerData<DxAssessment>)ControllerData, null));

            var grid = jqGrid.New(GridID)
                .setLogicId(GridID)
                .initJqGrid(gridUrlAction)
                .setTreeGridModel(TreeGridModel.adjacency)
                .setTreeGrid(true)
                .setTreeGridExpandColClick(false)
                .setAutoWidth(true)                
                .setShrinkToFit(true)
                .setTreeGridExpandColumn("Category")
                .setEmptyBlock(GridEmptyBlockID)
                .setPager(null);

            return grid;
        }
    }
}