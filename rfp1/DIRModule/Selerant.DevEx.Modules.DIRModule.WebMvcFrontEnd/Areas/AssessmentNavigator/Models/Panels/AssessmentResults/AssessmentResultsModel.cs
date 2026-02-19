using Selerant.DevEx.Modules.DIRModule.BackEnd.AssessmentManagement.BusinessObjects;
using Selerant.DevEx.WebMvcModules.Infrastructure.Navigators.Models;
using System;
using Selerant.DevEx.BusinessLayer;
using System.Collections.Generic;
using System.Linq;
using Selerant.DevEx.WebControls;
using Selerant.DevEx.WebMvcModules.Infrastructure.Controls;
using Selerant.DevEx.WebModules.LCIAScenarioManager.LCIAScenarioNavigator.Base;
using Selerant.DevEx.Modules.DIRModule.BackEnd.AssessmentResults;
using Selerant.DevEx.Modules.DIRModule.BackEnd.DestinationManagement;
using Selerant.DevEx.Modules.DIRModule.WebMvcFrontEnd.Areas.AssessmentNavigator.Security;
using Selerant.DevEx.Modules.DIRModule.Configuration.Resources;
using Selerant.DevEx.Modules.DIRModule.BackEnd.BusinessCostManagement;
using Selerant.DevEx.Modules.DIRModule.Configuration.Grids;
using Selerant.DevEx.WebMvcModules.Helpers;
using System.Globalization;
using Selerant.DevEx.Modules.DIRModule.WebMvcFrontEnd.Areas.AssessmentNavigator.Models.Panels.AssessmentResults.RowItem;

namespace Selerant.DevEx.Modules.DIRModule.WebMvcFrontEnd.Areas.AssessmentNavigator.Models.Panels.AssessmentResults
{
	public class AssessmentResultsModel : NavigatorPanelModel<DxAssessment, AssessmentResultsSecurity>
	{
		private static readonly Dictionary<string, string> Destinations = new Dictionary<string, string>
		{
			{ "COPRODUCT", "#228B22" },
			{ "COPRODUCT_2", "#228B22" }, // TODO: maybe set diffrent color
			{ "FOOD_RESCUE", "#556B2F" },
			{ "ANIMAL_FEED", "#70b117" },
			{ "BIOMASS_MATERIAL", "#579207" },
			{ "CODIGESTION_ANAEROBIC", "#ffba6b" },
			{ "COMPOSTING", "#ffaa4b" },
			{ "LAND_APP", "#f6901e" },
			{ "NOT_HARVESTED", "#ef8e21" },
			{ "COMBUSTION", "#c36c0a" },
			{ "LANDFILL", "#ef4821" },
			{ "SEWER", "#c32d0a" },
			{ "REFUSE_DISCARD", "#9a1e00" }
		};

		private static NumberFormatInfo currentUserNumberInfo;
		private readonly List<object> EmptyNameValueChartData;

		#region Properties

		public decimal LcStageId { get; set; }

		public string ProductionRatiosGridId { get => GridNames.PRODUCTION_RATIOS; }
		public string KeyFinantialIndicatorsGridId { get => GridNames.KEY_FINANTIAL_INDICATORS; }
		public string FoodWastePerDestinationGridId { get => GridNames.FOOD_WASTE_PER_DESTINATION; }
		public string CostOfWasteGridId { get => GridNames.COST_OF_WASTE; }
		public Dictionary<string, string> GridResultType { get; } = new Dictionary<string, string>
		{
			{ AssessmentResultType.MASS, GridNames.PRODUCTION_RATIOS },
			{ AssessmentResultType.PERCENTAGE, GridNames.KEY_FINANTIAL_INDICATORS }
		};

		private DxAssessment Assessment { get; }
		private DxAssessmentResultGridItemCollection Results { get; set; }
		private DxBusinessCostGridItemCollection CostOfWasteResults { get; set; }
		private DxDestinationPyramidChartItemCollection DestinationPyramidData { get; set; }

		public string AssessmentTimeFrameFrom => Assessment.TimeframeFrom?.ToString("D", DxUser.CurrentUser.GetCulture());

		public string AssessementTimeFrameTo => Assessment.TimeframeTo?.ToString("D", DxUser.CurrentUser.GetCulture());

		public static NumberFormatInfo CurrentUserNumberInfo
		{
			get
			{
				if (currentUserNumberInfo == null)
				{
					currentUserNumberInfo = DxUser.CurrentUser.GetCulture().NumberFormat;
					currentUserNumberInfo.PercentPositivePattern = 1;
				}
				return currentUserNumberInfo;
			}
		}

		#endregion

		#region Constructors

		public AssessmentResultsModel(string controllerUrl, NavigatorPanelControllerData<DxAssessment> data)
		: base(controllerUrl, data)
		{
			Assessment = data.TargetObject;

			EmptyNameValueChartData = BuildEmptyNameValueChartData();
		}

		public AssessmentResultsModel(string controllerUrl, NavigatorPanelControllerData<DxAssessment> data, decimal lcStageId)
		: base(controllerUrl, data)
		{
			Assessment = data.TargetObject;
			LcStageId = lcStageId;

			SetupResults();
			SetupDataPyramidChart();
			SetupCostOfWasteResults();
		}

		#endregion

		#region Public Methods

		public ICollection<ResultRowItem> GetResults(string resultType = null)
		{
			var results = Results.AsEnumerable();

			if (!string.IsNullOrEmpty(resultType))
				results = results.Where(x => x.Type == resultType).AsEnumerable();

			List<ResultRowItem> rows = results.OrderBy(o => o.SortOrder).Select(r => new ResultRowItem
            {
				IdentifiableString = r.IdentifiableString,
				Title = r.TitleDescription,
				Result = NullableDecimal.Round(r.Result?.Value ?? 0.0m, 2),
				UoM = r.ResultUoM,
				SortOrder = r.SortOrder
			}).ToList();

			return rows;
		}

		public ICollection<CostOfWasteRowItem> GetCostOfWasteData()
		{
			List<CostOfWasteRowItem> results;
			decimal costOfWasteSum;
			decimal materialLoss;

			decimal wasteCollectionTreat = Math.Abs(DxBusinessCost.WasteCollectionTreatment(Assessment.Code, LcStageId));
			(costOfWasteSum, materialLoss) = CostOfWasteResults.GetMaterialLoss(wasteCollectionTreat);

			materialLoss = Math.Abs(materialLoss);

			costOfWasteSum += wasteCollectionTreat;
			costOfWasteSum += materialLoss;

			if (costOfWasteSum == 0)
				return new List<CostOfWasteRowItem>();

			results = new List<CostOfWasteRowItem>(CostOfWasteResults.Count + 2);

			if (CostOfWasteResults.Count > 0)
			{
				results = CostOfWasteResults.OrderBy(o => o.SortOrder).Select(r =>
				{
					decimal percentageValue = Math.Abs((r.WasteCost?.Value ?? 0.0m) / costOfWasteSum);

					return new CostOfWasteRowItem(Assessment.CurrencySymbol)
					{
						Id = r.Id,
						Title = r.TitleDescription,
						WasteCost = NullableDecimal.Round(r.WasteCost?.Value ?? 0.0m, 2),
						Percentage = percentageValue,
					};
				})
				.ToList();
			}

			if (wasteCollectionTreat > 0)
				results.Add(CreateBusinessCostItemForResults(("WASTE", -1, wasteCollectionTreat, costOfWasteSum)));

			if (materialLoss > 0)
				results.Add(CreateBusinessCostItemForResults(("MATLOSS", -2, materialLoss, costOfWasteSum)));

			return results;
		}

		public ICollection<CostOfWasteRowItem> GetCostOfWasteGridData()
		{
			var data = GetCostOfWasteData();

			data.Add(new CostOfWasteRowItem(Assessment.CurrencySymbol)
			{
				Title = ResText.GetResourceModuleByName(ResourceFiles.AssessmentManager).GetLocalizedString("CostOfWasteGridTotalRowTitle"),
				WasteCost = data.Sum(x => x.WasteCost),
				Percentage = data.Sum(x => x.Percentage)
			});

			return data;
		}

		public ICollection<NameValueChartData> GetCostOfWasteChartData()
		{
			return GetCostOfWasteData().Select(r => new NameValueChartData
			{
				Name = r.Title,
				Value = r.Percentage,
				Label = r.PercentageFormatted
			}).ToList();

		}

		public List<DictionaryNameValueChartData> GetProductionRatiosChartData() => GetChartData(AssessmentResultType.MASS);

		public List<DictionaryNameValueChartData> GetKeyFinantialIndicators() => GetChartData(AssessmentResultType.PERCENTAGE);

		public IReadOnlyCollection<string> GetLegendNames(ICollection<NameValueChartData> businessCosts) => businessCosts.Select(cost => cost.Name).ToList();

		public ICollection<DxDestinationPyramidChartItem> GetFoodWastePerDestination()
		{
			List<DxDestinationPyramidChartItem> items = new List<DxDestinationPyramidChartItem>();

			Destinations.ForEach(destination =>
			{
				if (DestinationPyramidData.Contains(destination.Key))
				{
					var item = DestinationPyramidData.GetItem<DxDestinationPyramidChartItem>(destination.Key);

					if (item.Weight != 0)
						items.Add(item);
				}
			});

			return items;
		}

		public List<NameValueChartData> GetPyramidDestinationData()
		{
			List<NameValueChartData> pyramidData = GetFoodWastePerDestination()
				.Select(r => new NameValueChartData
				{
					Name = r.Title,
					Value = NullableDecimal.Round(r.Weight ?? 0.0m, 2)
				}).ToList();

			if (pyramidData.Count == 0)
				pyramidData.Add(new NameValueChartData(ResText.GetResourceModuleByName(ResourceFiles.Controls).GetLocalizedString("NoData_PyramidChart"), 100));

			return pyramidData;
		}

		public List<string> GetPyramidColorData()
		{
			List<string> colorData = new List<string>();

			foreach (var item in Destinations)
			{
				if (DestinationPyramidData.Contains(item.Key))
				{
					decimal? weight = DestinationPyramidData.GetItem<DxDestinationPyramidChartItem>(item.Key).Weight;
					if (weight.HasValue && weight != 0)
						colorData.Add(item.Value);
				}
			}

			if (colorData.Count == 0)
				colorData.Add("#808080");

			return colorData;
		}

		/*
		 * Methods below are used to populate charts with dummy data, because anyway they are reloaded client side with real data
		 */
		public List<object> GetEmptyCostOfWasteChartData() => new List<object>();

		public List<object> GetEmptyProductionRatiosChartData() => EmptyNameValueChartData;

		public List<object> GetEmptyKeyFinantialIndicators() => EmptyNameValueChartData;

		public List<object> GetEmptyPyramidDestinationData()
		{
			return new List<NameValueChartData>()
			{
				new NameValueChartData("N/A", 100)
			}
			.Cast<object>().ToList();
		}

		public string[] GetEmptyPyramidColorData() => new string[] { "#808080" };

		#endregion

		#region Private Methods

		private List<DictionaryNameValueChartData> GetChartData(string resultType)
		{
			List<DictionaryNameValueChartData> data = new List<DictionaryNameValueChartData>();

			List<NameValueChartData> assessmentResults = GetResults(resultType)
																.Select(r => new NameValueChartData
																{
																	Name = r.Title,
																	Value = r.Result
																}).ToList();

			data.Add(new DictionaryNameValueChartData
			{
				Key = Assessment.Code,
				Value = assessmentResults
			});

			return data;
		}

		private void SetupResults()
		{
			if (Results == null)
			{
				Results = new DxAssessmentResultGridItemCollection(Assessment.Code, LcStageId, true);

				Results.LoadItemsResultRow();
			}
		}

		private void SetupDataPyramidChart()
		{
			if (DestinationPyramidData == null)
			{
				DestinationPyramidData = DxDestinationPyramidChartItemCollection.OfFoodLossesNotIncludedInedibleParts(Assessment.Code, LcStageId, true);
			}
		}

		private void SetupCostOfWasteResults()
		{
			if (CostOfWasteResults == null)
			{
				CostOfWasteResults = new DxBusinessCostGridItemCollection(Assessment.Code, LcStageId, true);
			}
		}

		private CostOfWasteRowItem CreateBusinessCostItemForResults((string PhraseCode, decimal Id, decimal WasteCost, decimal CostOfWasteSum) businessCost)
		{
			decimal percentageValue = businessCost.WasteCost / businessCost.CostOfWasteSum;
			return new CostOfWasteRowItem(Assessment.CurrencySymbol)
			{
				Id = businessCost.Id,
				Title = DxPhraseText.GetBest(DxPhraseTypeCategory.ListOfValuesStandard, "DXDIR_BUSINESS_COST.TITLE", businessCost.PhraseCode, DxUser.CurrentUser.ProgramCulture).Text,
				WasteCost = decimal.Round(businessCost.WasteCost, 2),
				Percentage = percentageValue,
			};
		}

		private List<object> BuildEmptyNameValueChartData()
		{
			return new List<DictionaryNameValueChartData>()
			{
				new DictionaryNameValueChartData
				{
					Key = Assessment.Code,
					Value = new List<NameValueChartData>()
					{
						new NameValueChartData("N/A", 0.0m)
					}
				}
			}.Cast<object>().ToList();
		}

		#endregion

		#region Overrides

		protected override void FillMenuModel()
        {
            HtmlMenu.AddMenuItem(new MenuItem("Export", Locale.GetString(ResourceFiles.AssessmentManager, "ExportToExcel"))
                                .SetEnabled(SecurityObject.CanView));
        }

        /// <summary>
        /// Name of the grid to be used also for settings
        /// </summary>
        //public static string GRID_NAME = "DIR_AssessmentNavigator_AssessmentResults";
        /// <summary>
        /// Sets the view
        /// </summary>
        protected override void InnerInitializeForView()
        {
            base.InnerInitializeForView();

            AddCssClass("dir-module-bus-nav-assessment-results");
            StandardPanelButtons.EditSettings = false;
        }

        /// <summary>
        /// Set client definition of model
        /// </summary>
        /// <param name="scriptControlDescriptor"></param>
        protected override void FillScriptControlDescriptor(GxScriptControlDescriptor scriptControlDescriptor)
        {
            base.FillScriptControlDescriptor(scriptControlDescriptor);
            scriptControlDescriptor.TypeName = "DX.DIRModule.AssessmentNavigator.AssessmentResults";

			scriptControlDescriptor.Data["productionRatiosGridID"] = ProductionRatiosGridId;
            scriptControlDescriptor.Data["keyFinantialIndicatorsGridID"] = KeyFinantialIndicatorsGridId;
            scriptControlDescriptor.Data["foodWastePerDestinationGridID"] = FoodWastePerDestinationGridId;
            scriptControlDescriptor.Data["costOfWasteGridID"] = CostOfWasteGridId;
            scriptControlDescriptor.Data["lcStageId"] = LcStageId;
		}

        /// <summary>
        /// Initializes static resources
        /// </summary>
        /// <param name="resources"></param>
        protected override void InitializeResources(List<ControlResource> resources)
        {
            base.InitializeResources(resources);

            resources.Add(new ControlJsFile(DIRLinks.Areas.AssessmentNavigator.Views.AssessmentResults.AssessmentResults_ts));
            resources.Add(new ControlCssFile(DIRLinks.Areas.AssessmentNavigator.Views.AssessmentResults.AssessmentResults_css));
        }

        #endregion
	}
}
