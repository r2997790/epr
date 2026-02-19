using System.Web.Mvc;
using System.Collections.Generic;
using Selerant.DevEx.Modules.DIRModule.BackEnd.AssessmentManagement.BusinessObjects;
using Selerant.DevEx.WebControls;
using Selerant.DevEx.WebMvcModules.Helpers.jqGrid;
using Selerant.DevEx.WebMvcModules.Infrastructure.Controls;
using Selerant.DevEx.WebMvcModules.Infrastructure.Navigators.Models;
using Selerant.DevEx.Modules.DIRModule.BackEnd.InputManagement;
using Selerant.DevEx.Modules.DIRModule.Configuration.Resources;
using Selerant.DevEx.BusinessLayer;
using Selerant.DevEx.Modules.DIRModule.Configuration.Grids;
using Selerant.DevEx.Modules.DIRModule.WebMvcFrontEnd.Areas.AssessmentNavigator.Security;
using System.Linq;

namespace Selerant.DevEx.Modules.DIRModule.WebMvcFrontEnd.Areas.AssessmentNavigator.Models.Panels.ResourceManagement.Shared
{
	public class InputsGridTabModel : BaseGridTabModel
	{
		private DxAssessment targetObject;

		#region Properties

		public override string GridID
		{
			get { return GridNames.INPUTS; }
		}

		public override string ResourceType => ResourceTabIds.INPUT;

		public override string EmptyGridMessage => Locale.GetString(ResourceFiles.AssessmentManager, "InputsEmptyGrid");

        public override string GridUrl => Url.Action(MVC_DIR.AssessmentNavigator.ResourceManagement.GetInputsTree((NavigatorPanelControllerData<DxAssessment>)ControllerData, null, true));

		protected new NavigatorPanelControllerData<DxAssessment> ControllerData
		{
			get { return (NavigatorPanelControllerData<DxAssessment>)base.ControllerData; }
		}

		#endregion

		public InputsGridTabModel(string controllerUrl, NavigatorPanelControllerData<DxAssessment> navigatorPanelControllerData, decimal lcStageId, ViewMode viewMode = ViewMode.Navigator)
			: base(controllerUrl, navigatorPanelControllerData, lcStageId)
		{
			targetObject = navigatorPanelControllerData.TargetObject;
			targetObject.LoadEntity();
			Notes = new NotesModel(controllerUrl, navigatorPanelControllerData, ResourceType, lcStageId, targetObject.Code, viewMode);
		}

		#region Overrides

		protected override void InitializeResources(List<ControlResource> resources)
		{
			base.InitializeResources(resources);
			resources.Add(new ControlCssFile(DIRLinks.Areas.AssessmentNavigator.Views.ResourceManagement.Shared.InputsGridTab_css));
			resources.Add(new ControlJsFile(DIRLinks.Areas.AssessmentNavigator.Views.ResourceManagement.Shared.InputsGridTab_ts));
		}

		protected override void FillScriptControlDescriptor(GxScriptControlDescriptor scriptControlDescriptor)
		{
			base.FillScriptControlDescriptor(scriptControlDescriptor);
			scriptControlDescriptor.TypeName = "DX.DIRModule.AssessmentNavigator.ResourceManagement.Shared.InputsGridTab";

			var measurmentWebItemsCol = new WebListItemCollection
			{
				{ ((int)DxInput.Measure.Measured).ToString(), Locale.GetString(ResourceFiles.AssessmentManager, $"DXDIR_INPUT_MEASURE_{(int)DxInput.Measure.Measured}") },
				{ ((int)DxInput.Measure.Projected).ToString(), Locale.GetString(ResourceFiles.AssessmentManager, $"DXDIR_INPUT_MEASURE_{(int)DxInput.Measure.Projected}") },
				{ ((int)DxInput.Measure.Estimated).ToString(), Locale.GetString(ResourceFiles.AssessmentManager, $"DXDIR_INPUT_MEASURE_{(int)DxInput.Measure.Estimated}") }
			};
            scriptControlDescriptor.Data.Add("measurementItems", measurmentWebItemsCol.ConvertToJSONExchangeableObject());

            targetObject.LoadAttributes("DXDIR_DATA_QUALITY");
            decimal.TryParse(targetObject.Attributes["DXDIR_DATA_QUALITY"].Value.Data as string, out decimal selectedMeasurement);
            scriptControlDescriptor.Data.Add("selectedMeasurement", selectedMeasurement);

			scriptControlDescriptor.Data.Add("canCreateMaterial", SecurityObject.CanCreateMaterial);

			scriptControlDescriptor.Data.Add("currencyDisplayFormat", AmountFormatter.CurrencyDisplayFormat(targetObject.CurrencySymbol));
		}

		protected override void InitializeForView()
		{
			base.InitializeForView();
			AddCssClass("dir-module-partial-input-tab");
		}

		#endregion Overrides

		public WebListItemCollection GetInputCategories()
		{
			DxInputCategoryCollection categories = new DxInputCategoryCollection(targetObject.Code);
			categories.LoadEntity();
			List<decimal> usedCategories = DxInputCategoryCollection.GetUsedInputCategories(targetObject.Code, LcStageId);

			var InputCategories = new WebListItemCollection();
			InputCategories.Add(string.Empty, string.Empty);

			foreach (var category in categories)
			{
				if (!usedCategories.Contains(category.Id))
				{
					InputCategories.Add(string.IsNullOrEmpty(category.Type) ?
						new WebListItem(category.IdentifiableString, category.Title) :
						new WebListItem(category.IdentifiableString, category.Title, new Dictionary<string, object>() { { "type", category.Type } })
						);
				}
			}

			return InputCategories;
		}

		public string IsNoValidLcStage()
		{ 
			DxDestinationValidationCollection dxDestinationValidations = new DxDestinationValidationCollection(targetObject.Code, true);
			var dxDestination = dxDestinationValidations.Where(destination => destination.LcStage == LcStageId);

			if (dxDestination.Count() == 1)
			{
				if (dxDestination.First().InvalidDestination == DxDestinationValidation.InvalidCategory.NONFOODANDFOOD)
					return string.Format(Locale.GetString("DIR_AssessmentManager", "AssessmentResultNotificationError"), Locale.GetString("DIR_AssessmentManager", "InputDestinationNON-FOODorFOOD"));

				if (dxDestination.First().InvalidDestination == DxDestinationValidation.InvalidCategory.NONFOOD)
					return string.Format(Locale.GetString("DIR_AssessmentManager", "AssessmentResultNotificationError"), Locale.GetString("DIR_AssessmentManager", "InputDestinationNON-FOOD"));

				if (dxDestination.First().InvalidDestination == DxDestinationValidation.InvalidCategory.FOOD)
					return string.Format(Locale.GetString("DIR_AssessmentManager", "AssessmentResultNotificationError"), Locale.GetString("DIR_AssessmentManager", "InputDestinationFOOD"));
			}

			return string.Empty;
		}

		public jqGrid CreateJqGrid()
		{
			string gridUrlAction = Url.Action(MVC_DIR.AssessmentNavigator.ResourceManagement.GetInputsTree(ControllerData, null));

			var grid = jqGrid.New(GridID)
				.setLogicId(GridID)
				.initJqGrid(gridUrlAction)
				.setTreeGridModel(TreeGridModel.adjacency)
				.setTreeGrid(true)
				.setTreeGridExpandColClick(false)
				//.setLoadOnce(true)
				.setAutoWidth(true)
				.setShrinkToFit(true)
				.setTreeGridExpandColumn("Category")
				.setEmptyBlock(GridEmptyBlockID)
				.setPager(null);

			return grid;
		}
	}
}