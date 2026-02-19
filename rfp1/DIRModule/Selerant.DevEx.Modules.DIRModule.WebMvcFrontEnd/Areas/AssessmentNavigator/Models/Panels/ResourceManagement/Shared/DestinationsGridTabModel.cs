using System.Web.Mvc;
using System.Linq;
using System.Collections.Generic;
using Selerant.DevEx.Modules.DIRModule.BackEnd.AssessmentManagement.BusinessObjects;
using Selerant.DevEx.WebControls;
using Selerant.DevEx.WebMvcModules.Helpers.jqGrid;
using Selerant.DevEx.WebMvcModules.Infrastructure.Controls;
using Selerant.DevEx.WebMvcModules.Infrastructure.Navigators.Models;
using Selerant.DevEx.Modules.DIRModule.Configuration.Resources;
using Selerant.DevEx.WebMvcModules.Infrastructure.Controls.HtmlGrid;
using Selerant.DevEx.Modules.DIRModule.Configuration.Grids;
using Selerant.DevEx.Modules.DIRModule.WebMvcFrontEnd.Helpers;

namespace Selerant.DevEx.Modules.DIRModule.WebMvcFrontEnd.Areas.AssessmentNavigator.Models.Panels.ResourceManagement.Shared
{
	public class DestinationsGridTabModel : BaseGridTabModel
	{
		#region Enums

		public enum DestinationTypes
		{
			Food,
			NonFood
		}

		#endregion

		#region Properties

		public override string GridID
		{
			get
			{
				if(DestinationType == DestinationTypes.Food)
					return GridNames.DESTINATIONS;

				return GridNames.NON_FOOD_DESTINATIONS;

			}
		}

		public override string ResourceType 
		{
			get
			{
				if(DestinationType == DestinationTypes.Food)
					return ResourceTabIds.FOOD_DEST;
				return ResourceTabIds.NONFOOD_DEST;
			}
		}

		public override string EmptyGridMessage => Locale.GetString(ResourceFiles.AssessmentManager, "DestinationsEmptyGrid");

		public DestinationTypes DestinationType { get; set; }

        public override string GridUrl
        {
            get
            {
                if (DestinationType == DestinationTypes.Food)
                {
                    return Url.Action(MVC_DIR.AssessmentNavigator.ResourceManagement.GetDestinationsTree((NavigatorPanelControllerData<DxAssessment>)ControllerData, null, true));                   
                }
                else
                {
                    return Url.Action(MVC_DIR.AssessmentNavigator.ResourceManagement.GetNonFoodDestinationsTree((NavigatorPanelControllerData<DxAssessment>)ControllerData, null, true));
                }
            }
        }

		public Dictionary<string, int> VisibleColumnsSortIndex { get; private set; }

		#endregion

		#region Constructors

		public DestinationsGridTabModel(string controllerUrl, NavigatorPanelControllerData<DxAssessment> navigatorPanelControllerData, decimal lcStageId, string isFood, ViewMode viewMode = ViewMode.Navigator) 
			: base(controllerUrl, navigatorPanelControllerData, lcStageId)
		{
			if (isFood == DestinationTypes.Food.ToString())
				DestinationType = DestinationTypes.Food;
			else
				DestinationType = DestinationTypes.NonFood;

			VisibleColumnsSortIndex = new Dictionary<string, int>();
			Notes = new NotesModel(controllerUrl, navigatorPanelControllerData, ResourceType, lcStageId, navigatorPanelControllerData.TargetObject.Code, viewMode);
		}

		#endregion

		#region Overrides

		protected override void InitializeResources(List<ControlResource> resources)
		{
			base.InitializeResources(resources);
			resources.Add(new ControlCssFile(DIRLinks.Areas.AssessmentNavigator.Views.ResourceManagement.Shared.DestinationsGridTab_css));
			resources.Add(new ControlJsFile(DIRLinks.Areas.AssessmentNavigator.Views.ResourceManagement.Shared.DestinationsGridTab_ts));
		}

		protected override void FillScriptControlDescriptor(GxScriptControlDescriptor scriptControlDescriptor)
		{
			base.FillScriptControlDescriptor(scriptControlDescriptor);
			scriptControlDescriptor.TypeName = "DX.DIRModule.AssessmentNavigator.ResourceManagement.Shared.DestinationsGridTab";

			scriptControlDescriptor.Data.Add("assessmentCode", Assessment.IdentifiableString);
			scriptControlDescriptor.Data.Add("isFood", DestinationType == DestinationTypes.Food);
		}

		protected override void InitializeForView()
		{
			base.InitializeForView();

			AddCssClass("dir-module-partial-dest-tab");
		}

		#endregion Overrides

		public jqGrid CreateJqGrid()
		{
			HtmlGridHelper helper = new HtmlGridHelper(GridID);
			// NOTE pull this list, but maybe add Products and CoProducts columns dynamically
			List<Column> columns = helper.jqGridColumns();
			Dictionary<string, int> columnsOrderIndex = columns.Select((r, idx) => new { r.ColumnName, idx }).ToDictionary(x => x.ColumnName, x => x.idx);

			DxAssessmentDestinationCollection assessmentDestinations = new DxAssessmentDestinationCollection(DxAssessmentDestinationCollection.Filter.AssessmentCode, Assessment.Code);
			assessmentDestinations.Load();

			var destinationCodes = new HashSet<string>(assessmentDestinations.Select(x => x.DestinationCode));

			foreach (var column in columns)
			{
				if (!destinationCodes.Contains(column.ColumnName) && !GridHelpers.Instance.FixedDestinationsColumns.Contains(column.ColumnName))
					column.setHidden(true);
				else
				{
					if (column.ColumnName == GridHelpers.CATEGORY || column.ColumnName == GridHelpers.ACTIONS)
						VisibleColumnsSortIndex.Add(column.ColumnName, 0);
					else
						VisibleColumnsSortIndex.Add(column.ColumnName, columnsOrderIndex[column.ColumnName]);
				}
			}

            string gridUrlAction;

            if (DestinationType == DestinationTypes.Food)
            {
                gridUrlAction = Url.Action(MVC_DIR.AssessmentNavigator.ResourceManagement.GetDestinationsTree((NavigatorPanelControllerData<DxAssessment>)ControllerData, null));
            }
            else
            {
                gridUrlAction = Url.Action(MVC_DIR.AssessmentNavigator.ResourceManagement.GetNonFoodDestinationsTree((NavigatorPanelControllerData<DxAssessment>)ControllerData, null));
            }

            var grid = jqGrid.New(GridID)
				.setLogicId(GridID)
				.initJqGrid(gridUrlAction)
				.setTreeGridModel(TreeGridModel.adjacency)
				.setTreeGrid(true)
				.setColumns(columns) // NOTE maybe grid cloud take columns build in code not just from config (for extending Product and CoProduct)
				.setTreeGridExpandColClick(false)
				.setAutoWidth(true)
				.setShrinkToFit(true)
				.setTreeGridExpandColumn(GridHelpers.CATEGORY)
				.setEmptyBlock(GridEmptyBlockID)
				.setPager(null);

			return grid;
		}
	}
}