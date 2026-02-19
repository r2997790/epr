using System.Web.Mvc;
using System.Collections.Generic;
using Selerant.DevEx.Modules.DIRModule.BackEnd.AssessmentManagement.BusinessObjects;
using Selerant.DevEx.WebControls;
using Selerant.DevEx.WebMvcModules.Helpers.jqGrid;
using Selerant.DevEx.WebMvcModules.Infrastructure.Controls;
using Selerant.DevEx.WebMvcModules.Infrastructure.Navigators.Models;
using Selerant.DevEx.Modules.DIRModule.Configuration.Grids;
using Selerant.DevEx.Modules.DIRModule.Configuration.Resources;
using Selerant.DevEx.Modules.DIRModule.WebMvcFrontEnd.Helpers;
using System.Linq;
using Selerant.DevEx.Modules.DIRModule.BackEnd.BusinessCostManagement;

namespace Selerant.DevEx.Modules.DIRModule.WebMvcFrontEnd.Areas.AssessmentNavigator.Models.Panels.ResourceManagement.Shared
{
	public class BusinessCostGridTabModel : BaseGridTabModel
	{
		#region Constants

		public const string OTHER = "OTHER";

		#endregion

		#region Properties

		public override string GridID => GridNames.BUSINESSCOSTS;

		public override string ResourceType => ResourceTabIds.BUSINESS_COST;

		public override string EmptyGridMessage => Locale.GetString(ResourceFiles.AssessmentManager, "BusinessCostsEmptyGrid");

        public override string GridUrl => Url.Action(MVC_DIR.AssessmentNavigator.ResourceManagement.GetBusinessCostGrid((NavigatorPanelControllerData<DxAssessment>)ControllerData, null, true));

		public BusinessCostGridTabModel(string controllerUrl, NavigatorPanelControllerData<DxAssessment> navigatorPanelControllerData, decimal lcStageId, ViewMode viewMode = ViewMode.Navigator) 
			: base(controllerUrl, navigatorPanelControllerData, lcStageId)
		{
			Notes = new NotesModel(controllerUrl, navigatorPanelControllerData, ResourceType, lcStageId, navigatorPanelControllerData.TargetObject.Code, viewMode);
			Notes.HasBusinessCostOther = ContainsOtherNotes();
		}

		#endregion Properties

		#region Overrides

		protected override void InitializeResources(List<ControlResource> resources)
		{
			base.InitializeResources(resources);
			resources.Add(new ControlCssFile(DIRLinks.Areas.AssessmentNavigator.Views.ResourceManagement.Shared.BusinessCostGridTab_css));
			resources.Add(new ControlJsFile(DIRLinks.Areas.AssessmentNavigator.Views.ResourceManagement.Shared.BusinessCostGridTab_ts));
		}

		protected override void FillScriptControlDescriptor(GxScriptControlDescriptor scriptControlDescriptor)
		{
			base.FillScriptControlDescriptor(scriptControlDescriptor);
			scriptControlDescriptor.TypeName = "DX.DIRModule.AssessmentNavigator.ResourceManagement.Shared.BusinessCostGridTab";

			scriptControlDescriptor.Data.Add("businessCostItems", GetBusinessCostsList().ConvertToJSONExchangeableObject());
		}

		protected override void InitializeForView()
		{
			base.InitializeForView();
			AddCssClass("dir-module-partial-businesscost-tab");
		}

		#endregion Overrides

		#region Methods

		public WebListItemCollection GetBusinessCostsList()
		{
			var allBusinessCosts = LovListFactory.GetBusinessCosts();

            WebListItemCollection availableBusinessCosts = new WebListItemCollection();
            IEnumerable<string> businessCostCodes = DxBusinessCostCollection.GetBusinessCostCodes(Assessment.Code, LcStageId);

            availableBusinessCosts.AddRange(allBusinessCosts.Where(bc => !businessCostCodes.Contains(bc.Key)));

            return availableBusinessCosts;
		}

		public jqGrid CreateJqGrid()
		{
			string gridUrlAction = Url.Action(MVC_DIR.AssessmentNavigator.ResourceManagement.GetBusinessCostGrid((NavigatorPanelControllerData<DxAssessment>)ControllerData, null));

			var grid = jqGrid.New(GridID)
				.setLogicId(GridID)
				.initJqGrid(gridUrlAction)
				.setTreeGrid(false)
				.setAutoWidth(true)
				.setShrinkToFit(true)
				.setEmptyBlock(GridEmptyBlockID)
				.setPager(null);

			return grid;
		}

		public bool ContainsOtherNotes()
		{
			var businessCosts = DxBusinessCostCollection.GetBusinessCostCodes(Assessment.Code, LcStageId);

			return businessCosts.Any(x => x == OTHER);
		}

		#endregion Methods
	}
}