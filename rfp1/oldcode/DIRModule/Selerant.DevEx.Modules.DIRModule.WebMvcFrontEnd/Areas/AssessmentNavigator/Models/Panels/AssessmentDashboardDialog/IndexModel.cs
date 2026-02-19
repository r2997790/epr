using Selerant.DevEx.BusinessLayer;
using Selerant.DevEx.BusinessLayer.Linq;
using Selerant.DevEx.Modules.DIRModule.BackEnd.AssessmentManagement.BusinessObjects;
using Selerant.DevEx.Modules.DIRModule.Configuration.Grids;
using Selerant.DevEx.Modules.DIRModule.Configuration.Resources;
using Selerant.DevEx.Modules.DIRModule.WebMvcFrontEnd.Areas.AssessmentNavigator.Models.Panels.AssessmentResults;
using Selerant.DevEx.Modules.DIRModule.WebMvcFrontEnd.Areas.AssessmentNavigator.Models.Panels.AssessmentResults.RowItem;
using Selerant.DevEx.Web;
using Selerant.DevEx.WebControls;
using Selerant.DevEx.WebModules.LCIAScenarioManager.LCIAScenarioNavigator.Base;
using Selerant.DevEx.WebMvcModules.Infrastructure.Controls;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Selerant.DevEx.Modules.DIRModule.WebMvcFrontEnd.Areas.AssessmentNavigator.Models.Panels.AssessmentDashboardDialog
{
    public class IndexModel : DialogViewIndexModel
    {
        #region Constants

        public const char IDENTIFIABLE_STRING_SEPARATOR = '|';

        private const decimal PRODUCT_OUTPUT_CATEGORY_ID = 1;
        private const decimal COPRODUCT_OUTPUT_CATEGORY_ID = 2;
        private const decimal FOOD_RESCUE_OUTPUT_CATEGORY_ID = 3;

        private const string ESTIMATED_COST = "ESTIM_COST";

        #endregion

        #region Properties

        private DxAssessmentCollection AssessmentCollection { get; set; } = new DxAssessmentCollection();
        private AssessmentDashboardDataStore DataStore { get; }
        public bool ShowHotSpotInDialog { get; set; } = false;
        public List<string> AssessmentIdentifibleStrings { get; }
        public string HotSpotGridId => GridNames.HOT_SPOT_DASHBOARD;
        public string ProductionRatiosGridId => GridNames.PRODUCTION_RATIOS_DASHBOARD;
        public string KeyFinantialIndicatorsGridId => GridNames.KEY_FINANTIAL_INDICATORS_DASHBOARD;
        public string CostOfWasteGridId => GridNames.COST_OF_WASTE_DASHBOARD;
        public Dictionary<string, string> GridResultType { get; }
        public Dictionary<FoodLossType, FoodLossesContainerModel> FoodLosses { get; }
        public FoodLossesContainerModel FoodLossesNotIncludedInedibleParts { get; }
        public FoodLossesContainerModel FoodLossesInediblePartsOnly { get; }

        #endregion

        #region Constructors

        public IndexModel(string controllerUrl, ViewControlControllerData controllerData, List<string> objectIdentifiableStrings)
            : base(controllerUrl, controllerData)
        {
            AssessmentIdentifibleStrings = objectIdentifiableStrings;
            AssessmentIdentifibleStrings.ForEach(identifiableString => AssessmentCollection.Add((DxAssessment)DxObject.ParseIdentifiableString(identifiableString)));

            DataStore = AssessmentDashboardDataStore.New(AssessmentCollection);

            FoodLossesNotIncludedInedibleParts = InitializeFoodLosses(FoodLossType.NotIncInedibleParts);
            FoodLossesInediblePartsOnly = InitializeFoodLosses(FoodLossType.InediblePartsOnly);

            GridResultType = new Dictionary<string, string>
            {
                { AssessmentResultType.MASS, ProductionRatiosGridId },
                { AssessmentResultType.PERCENTAGE, KeyFinantialIndicatorsGridId }
            };

            FoodLosses = new Dictionary<FoodLossType, FoodLossesContainerModel>
            {
                { FoodLossType.NotIncInedibleParts, FoodLossesNotIncludedInedibleParts },
                { FoodLossType.InediblePartsOnly, FoodLossesInediblePartsOnly }
            };
        }

        #endregion

        #region Public Methods

        public IReadOnlyCollection<string> GetAssessmentNames()
        {
            List<string> assessmentNames = new List<string>();

            AssessmentCollection.ForEach(assessment =>
            {
                if (assessment.PersistenceStatus == DxPersistenceStatus.Unknown)
                    assessment.Load();

                assessmentNames.Add(AssessmentDashboardDataStore.GetAssessmentTitle(assessment));
            });

            return assessmentNames;
        }

        #region Data

        public IList<IChartData> GetHotSpotChartData(HotSpotValueType hotSpotValue) => DataStore.Charts.GetHotSpot(hotSpotValue);
        public IList<IDashboardRowItem> GetHotSpotGridData(HotSpotValueType hotSpotValue) => DataStore.Grids.GetHotSpot(hotSpotValue);

        public IList<IChartData> GetCostOfWasteChartData() => DataStore.Charts.GetCostOfWaste();
        public IList<IDashboardRowItem> GetCostOfWasteGridData() => DataStore.Grids.GetCostOfWaste();

        public IList<IChartData> GetResultsChartData(string resultType) => DataStore.Charts.GetResults(resultType);
        public IList<IDashboardRowItem> GetResultsGridData(string resultType) => DataStore.Grids.GetResults(resultType);

        #endregion

        public WebMvcModules.Helpers.MenuItem RemoveGroup
        {
            get
            {
                WebMvcModules.Helpers.MenuItem removeGroup = new Selerant.DevEx.WebMvcModules.Helpers.MenuItem("RemoveGroup", ResText.GetResourceModuleByName(ResourceFiles.AssessmentManager).GetLocalizedString("DashboardRemoveAssessment"));
                return AddAssessmentToSubMenuGroup("Remove_", "RemoveAssessment_Click(this)", removeGroup);
            }
        }

        public IList<IDashboardRowItem> GetFoodLossesNotIncludedInediblePartsFor(string assessmentIdentifiableString)
        {
            var assessment = AssessmentCollection.Where(x => x.IdentifiableString == assessmentIdentifiableString).First();
            return DataStore.Grids.GetFoodLossesNotIncludedInedibleParts(assessment).Data;
        }

        #endregion

        #region Private Methods

        private FoodLossesContainerModel InitializeFoodLosses(FoodLossType lossType)
        {
            List<FoodLossModel> foodLosses = new List<FoodLossModel>();

            int counter = 0;

            foreach (var assessment in AssessmentCollection)
            {
                (IList<IChartData> Data, IList<string> DestinationColors) chartData;
                (IList<IDashboardRowItem> Data, IList<string> DestinationColors) gridData;

                switch (lossType)
                {
                    case FoodLossType.NotIncInedibleParts:
                        chartData = DataStore.Charts.GetFoodLossesNotIncludedInedibleParts(assessment);
                        gridData = DataStore.Grids.GetFoodLossesNotIncludedInedibleParts(assessment);
                        break;
                    case FoodLossType.InediblePartsOnly:
                        chartData = DataStore.Charts.GetFoodLossesInediblePartsOnly(assessment);
                        gridData = DataStore.Grids.GetFoodLossesInediblePartsOnly(assessment);
                        break;
                    default:
                        throw new ArgumentException($"{lossType} not implemented");
                }

                foodLosses.Add(new FoodLossModel(ControllerUrl, ControllerData)
                {
                    Assessment = assessment,
                    FoodLossType = lossType,
                    ChartData = chartData.Data,
                    GridData = gridData.Data,
                    ChartItemColors = chartData.DestinationColors,
                    LogicId = $"FoodLoss_{assessment.Code}_{++counter}"
                });
            }

            var model = new FoodLossesContainerModel(ControllerUrl, ControllerData, lossType, foodLosses);

            return model;
        }

        private WebMvcModules.Helpers.MenuItem AddAssessmentToSubMenuGroup(string subItemsPrefix, string jsFunction, WebMvcModules.Helpers.MenuItem menuGroup)
        {
            short counter = 0;

            AssessmentCollection.ForEach(assessment =>
            {
                if (assessment.PersistenceStatus == DxPersistenceStatus.Unknown)
                    assessment.Load();

                menuGroup.AddMenuItem(new WebMvcModules.Helpers.MenuItem($"{subItemsPrefix}{(++counter)}", AssessmentDashboardDataStore.GetAssessmentTitle(assessment))
                {
                    OnClickJSScript = $"DX.Ctrl.findParentControlOfType(this, DX.DIRModule.AssessmentNavigator.AssessmentDashboardDialog).{jsFunction}"
                }
                .SetData("identifiable-string", assessment.IdentifiableString));
            });

            return menuGroup;
        }

        #endregion

        #region Overrides
        
        protected override void FillScriptControlDescriptor(GxScriptControlDescriptor scriptControlDescriptor)
        {
            base.FillScriptControlDescriptor(scriptControlDescriptor);
            scriptControlDescriptor.TypeName = "DX.DIRModule.AssessmentNavigator.AssessmentDashboardDialog";
            scriptControlDescriptor.Data["assmts"] = AssessmentCollection.Select(assessment => assessment.IdentifiableString).ToList();
            scriptControlDescriptor.Data["hotSpotGridID"] = HotSpotGridId;
            scriptControlDescriptor.Data["productionRatiosGridID"] = ProductionRatiosGridId;
            scriptControlDescriptor.Data["keyFinantialIndicatorsGridID"] = KeyFinantialIndicatorsGridId;
            scriptControlDescriptor.Data["costOfWasteGridID"] = CostOfWasteGridId;
        }

        /// <summary>
        /// Initializes static resources
        /// </summary>
        /// <param name="resources"></param>
        protected override void InitializeResources(List<ControlResource> resources)
        {
            base.InitializeResources(resources);

            resources.Add(new ControlJsFile(DIRLinks.Areas.AssessmentNavigator.Views.AssessmentDashboardDialog.AssessmentDashboardDialog_ts));
            resources.Add(new ControlCssFile(DIRLinks.Areas.AssessmentNavigator.Views.AssessmentDashboardDialog.AssessmentDashboardDialog_css));
        }

        #endregion
	}
}