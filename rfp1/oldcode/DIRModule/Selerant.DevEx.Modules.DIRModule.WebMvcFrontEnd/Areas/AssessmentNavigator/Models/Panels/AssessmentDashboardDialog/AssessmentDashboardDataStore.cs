using Selerant.DevEx.Modules.DIRModule.BackEnd.AssessmentManagement.BusinessObjects;
using Selerant.DevEx.Modules.DIRModule.BackEnd.AssessmentResults;
using System;
using System.Collections.Generic;
using System.Linq;
using Selerant.DevEx.BusinessLayer.Linq;
using Selerant.DevEx.Modules.DIRModule.BackEnd.BusinessCostManagement;
using Selerant.DevEx.BusinessLayer;
using Selerant.DevEx.WebControls;
using Selerant.DevEx.WebModules.LCIAScenarioManager.LCIAScenarioNavigator.Base;
using Selerant.DevEx.Modules.DIRModule.WebMvcFrontEnd.Areas.AssessmentNavigator.Models.Panels.AssessmentResults.RowItem;
using Selerant.DevEx.Modules.DIRModule.BackEnd.OutputManagement;
using Selerant.DevEx.WebMvcModules.Infrastructure.Resources.Text;
using Selerant.DevEx.Modules.DIRModule.Configuration.Resources;
using Selerant.DevEx.Modules.DIRModule.BackEnd.DestinationManagement;

namespace Selerant.DevEx.Modules.DIRModule.WebMvcFrontEnd.Areas.AssessmentNavigator.Models.Panels.AssessmentDashboardDialog
{
    public sealed class AssessmentDashboardDataStore
    {
		// TODO: refactor this, there is same dict for result panel
        public static readonly Dictionary<string, string> Destinations = new Dictionary<string, string>
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

        private const decimal PRODUCT_OUTPUT_CATEGORY_ID = 1;
        private const string ESTIMATED_COST = "ESTIM_COST";

        private List<(DxAssessment Assessment, List<Result> Results)> _results;
        private List<(DxAssessment Assessment, List<DxBusinessCostGridItem> BusinessCosts)> _costsOfWaste;
        private List<(DxAssessment Assessment, List<DxOutput> Outputs)> _outputs;
        private List<(DxAssessment Assessment, List<DxDestinationPyramidChartItem> Losses)> _foodLossesNotIncludedInedibleParts;
        private List<(DxAssessment Assessment, List<DxDestinationPyramidChartItem> Losses)> _foodLossesInediblePartsOnly;

        public DxAssessmentCollection Assessments { get; }
        public ChartsStore Charts { get; }
        public GridStore Grids { get; }

        private IReadOnlyList<(DxAssessment Assessment, List<Result> Results)> Results
        {
            get
            {
                if (_results == null)
                    _results = InitializeResults();

                return _results;
            }
        }

        private IReadOnlyList<(DxAssessment Assessment, List<DxBusinessCostGridItem> BusinessCosts)> CostsOfWaste
        {
            get
            {
                if (_costsOfWaste == null)
                    _costsOfWaste = InitializeCostsOfWaste();

                return _costsOfWaste;
            }
        }

        private IReadOnlyList<(DxAssessment Assessment, List<DxOutput> Outputs)> Outputs
        {
            get
            {
                if (_outputs == null)
                    _outputs = InitializeOutputs();

                return _outputs;
            }
        }

        private IReadOnlyList<(DxAssessment Assessment, List<DxDestinationPyramidChartItem> Losses)> FoodLossesNotIncludedInedibleParts
        {
            get
            {
                if (_foodLossesNotIncludedInedibleParts == null)
                    _foodLossesNotIncludedInedibleParts = InitializeFoodLossesNotIncludedInedibleParts();

                return _foodLossesNotIncludedInedibleParts;
            }
        }

        private IReadOnlyList<(DxAssessment Assessment, List<DxDestinationPyramidChartItem> Losses)> FoodLossesInediblePartsOnly
        {
            get
            {
                if (_foodLossesInediblePartsOnly == null)
                    _foodLossesInediblePartsOnly = InitializeFoodLossesInediblePartsOnly();

                return _foodLossesInediblePartsOnly;
            }
        }

        private AssessmentDashboardDataStore(DxAssessmentCollection assessments)
        {
            Assessments = assessments;
            Charts = new ChartsStore(this);
            Grids = new GridStore(this);
        }

        public static AssessmentDashboardDataStore New(DxAssessmentCollection assessments)
        {
            var store = new AssessmentDashboardDataStore(assessments);
            store.Initialize();

            return store;
        }

        public static string GetAssessmentTitle(DxAssessment assessment) => $"({assessment.Code}) {assessment.Description}";

        public IEnumerable<IGrouping<string, Result>> GetResultsFor(DxAssessment assessment, string resultType)
        {
            var results = Results.Where(x => x.Assessment == assessment)
                .Select(r => r.Results.Where(x => x.ResultItem.Type == resultType))
                .FirstOrDefault()
                .GroupBy(x => x.ResultItem.TitleDescription);

            return results;
        }

        private void Initialize()
        {
            Assessments.ForEach(assessment =>
            {
                if (assessment.PersistenceStatus == DxPersistenceStatus.Unknown)
                    assessment.Load();

                if (assessment.LcStages.PersistenceStatus == DxPersistenceStatus.Unknown)
                    assessment.LcStages.Load();
            });
        }

        private List<(DxAssessment Assessment, List<Result> Results)> InitializeResults()
        {
            var results = new List<(DxAssessment, List<Result>)>();

            Assessments.ForEach(assessment =>
            {
                List<Result> allResults = new List<Result>();

                assessment.LcStages.OrderBy(o => o.SortOrder).ForEach(stage =>
                {
                    var resultItems = new DxAssessmentResultGridItemCollection(assessment.Code, stage.Id, true);

                    var assessmentResults = resultItems.OrderBy(o => o.SortOrder).Select(r => new Result
                    {
                        LcStage = stage,
                        ResultItem = r
                    }).ToList();

                    allResults.AddRange(assessmentResults);
                });

                results.Add((assessment, allResults));
            });

            return results;
        }

        private List<(DxAssessment Assessment, List<DxBusinessCostGridItem> BusinessCosts)> InitializeCostsOfWaste()
        {
            List<(DxAssessment Assessment, List<DxBusinessCostGridItem> BusinessCosts)> costsOfWaste = new List<(DxAssessment assessment, List<DxBusinessCostGridItem>)>();

            Assessments.ForEach(assessment =>
            {
                List<DxBusinessCostGridItem> allCosts = new List<DxBusinessCostGridItem>();

                assessment.LcStages.OrderBy(o => o.SortOrder).ForEach(stage =>
                {
                    var businessCosts = new DxBusinessCostGridItemCollection(assessment.Code, stage.Id, true);
                    allCosts.AddRange(businessCosts);

                    decimal wasteCollectionTreat = Math.Abs(DxBusinessCost.WasteCollectionTreatment(assessment.Code, stage.Id));
                    var (_, materialLoss) = businessCosts.GetMaterialLoss(wasteCollectionTreat);

                    materialLoss = Math.Abs(materialLoss);

                    if (wasteCollectionTreat > 0)
                        allCosts.Add(CreateBusinessCostItemForResult("WASTE", -1, wasteCollectionTreat));

                    if (materialLoss > 0)
                        allCosts.Add(CreateBusinessCostItemForResult("MATLOSS", -2, materialLoss));
                });

                var assessmentCosts = allCosts.OrderBy(o => o.SortOrder).GroupBy(x => x.TitleDescription)
                                        .Select((r, i) => new DxBusinessCostGridItem
                                        {
                                            Title = r.Key,
                                            WasteCost = r.Sum(s => s.WasteCost?.Value ?? 0),
                                            SortOrder = i,
                                        }).ToList();

                costsOfWaste.Add((assessment, assessmentCosts));
            });

            HashSet<string> categoriesList = new HashSet<string>(costsOfWaste.SelectMany(r => r.BusinessCosts.Select(rr => rr.Title)));

            categoriesList.ForEach((category, index) =>
            {
                costsOfWaste.ForEach(item =>
                {
                    if (!item.BusinessCosts.Any(x => x.Title == category))
                        item.BusinessCosts.Add(new DxBusinessCostGridItem { Title = category, WasteCost = 0 });
                });
            });

            costsOfWaste.ForEach(item => item.BusinessCosts.Sort(Comparer<DxBusinessCostGridItem>.Create((d1, d2) => d1.Title.CompareTo(d2.Title))));
            
            return costsOfWaste;
        }

        private List<(DxAssessment Assessment, List<DxOutput> Outputs)> InitializeOutputs()
        {
            List<(DxAssessment Assessment, List<DxOutput> Outputs)> outputs = new List<(DxAssessment Assessment, List<DxOutput> Outputs)>();

            Assessments.ForEach(assessment =>
            {
                DxOutputCollection assessmentOutputs = new DxOutputCollection(DxOutputCollection.Filter.AssessmentCode, assessment.Code);
                assessmentOutputs.Load();
                assessmentOutputs.LoadItemsLcStage();

                outputs.Add((assessment, assessmentOutputs.ToList()));
            });

            return outputs;
        }

        private List<(DxAssessment Assessment, List<DxDestinationPyramidChartItem>)> InitializeFoodLossesNotIncludedInedibleParts() => InitializeFoodLosses((string assessmentCode, decimal lcStage) => 
        DxDestinationPyramidChartItemCollection.OfFoodLossesNotIncludedInedibleParts(assessmentCode, lcStage, true));

        private List<(DxAssessment Assessment, List<DxDestinationPyramidChartItem>)> InitializeFoodLossesInediblePartsOnly() => 
            InitializeFoodLosses((string assessmentCode, decimal lcStage) => DxDestinationPyramidChartItemCollection.OfFoodLossesInediblePartsOnly(assessmentCode, lcStage, true));

        private List<(DxAssessment Assessment, List<DxDestinationPyramidChartItem>)> InitializeFoodLosses(Func<string, decimal, DxDestinationPyramidChartItemCollection> getLossesFunc)
        {
            List<(DxAssessment Assessment, List<DxDestinationPyramidChartItem>)> losses = new List<(DxAssessment Assessment, List<DxDestinationPyramidChartItem>)>();

            Assessments.ForEach(assessment =>
            {
                List<DxDestinationPyramidChartItem> allLosses = new List<DxDestinationPyramidChartItem>();

                assessment.LcStages.OrderBy(o => o.SortOrder).ForEach(stage =>
                {
                    var stageLosses = getLossesFunc(assessment.Code, stage.Id);
                    allLosses.AddRange(stageLosses);
                });

                var assessmentLosses = Destinations.Join(allLosses.GroupBy(x => new { x.Code, x.Title }),
                    dest => dest.Key,
                    loss => loss.Key.Code,
                    (dest, loss) => new DxDestinationPyramidChartItem
                    {
                        Title = loss.Key.Title,
                        Code = loss.Key.Code,
                        Weight = loss.Sum(s => s.Weight?.Value ?? 0)
                    })
                    .Where(x => x.Weight > 0)
                    .ToList();

                losses.Add((assessment, assessmentLosses));
            });

            return losses;
        }

        private DxBusinessCostGridItem CreateBusinessCostItemForResult(string phraseCode, decimal id, decimal wasteCost)
        {
            return new DxBusinessCostGridItem(id)
            {
                Title = phraseCode,
                WasteCost = wasteCost
            };
        }

        public class Result
        {
            public DxAssessmentLcStage LcStage { get; set; }
            public DxAssessmentResultGridItem ResultItem { get; set; }
        }

        public class ChartsStore : IDataStore<IChartData>
        {
            private AssessmentDashboardDataStore DashboardStore { get; }

            public ChartsStore(AssessmentDashboardDataStore store)
            {
                DashboardStore = store;
            }

            public IList<IChartData> GetHotSpot(HotSpotValueType type)
            {
                List<IChartData> groupedBarChartData = new List<IChartData>();
                List<List<NameValueChartData>> barData = new List<List<NameValueChartData>>();

                switch (type)
                {
                    case HotSpotValueType.MASS:
                        barData = GetBarData(GetMassForAssessment);
                        break;
                    case HotSpotValueType.COST:
                        barData = GetBarData(GetCostForAssessment);
                        break;
                }

                groupedBarChartData = new List<IChartData>
                {
                    new ConnectedBarChart
                    {
                        BarData = barData
                    }
                };

                return groupedBarChartData;
            }

            public IList<IChartData> GetCostOfWaste()
            {
                List<IChartData> costOfWasteSpiderChart = new List<IChartData>();

                DashboardStore.CostsOfWaste.ForEach((costOfWasteData, index) =>
                {
                    costOfWasteSpiderChart.Add(new ChartData
                    {
                        Name = GetAssessmentTitle(costOfWasteData.Assessment),
                        Data = costOfWasteData.BusinessCosts.Select(r => new Chart
                        {
                            Category = r.TitleDescription,
                            Value = r.WasteCost?.Value ?? 0
                        }).ToList()
                    });
                });

                return costOfWasteSpiderChart;
            }

            
            public IList<IChartData> GetResults(string resultType)
            {
                List<IChartData> chartData = new List<IChartData>();

                DashboardStore.Assessments.ForEach(assessment => chartData.Add(GetResultsChartData(assessment, resultType)));

                return chartData;
            }

            public (IList<IChartData> Data, IList<string> DestinationColors) GetFoodLossesNotIncludedInedibleParts(DxAssessment assessment) => 
                GetFoodLosses(assessment, DashboardStore.FoodLossesNotIncludedInedibleParts);

            public (IList<IChartData> Data, IList<string> DestinationColors) GetFoodLossesInediblePartsOnly(DxAssessment assessment) => 
                GetFoodLosses(assessment, DashboardStore.FoodLossesInediblePartsOnly);

            #region Private Methods - Results

            private DictionaryNameValueChartData GetResultsChartData(DxAssessment assessment, string resultType)
            {
                var chartData = DashboardStore.GetResultsFor(assessment, resultType)
                                .Select(r => new NameValueChartData
                                {
                                    Name = r.Key,
                                    Value = r.Average(x => x.ResultItem.Result?.Value ?? 0.0m)
                                }).ToList();

                DictionaryNameValueChartData data = new DictionaryNameValueChartData
                {
                    Key = GetAssessmentTitle(assessment),
                    Value = chartData
                };

                return data;
            }

            #endregion

            #region Private Methods - HotSpot

            private List<List<NameValueChartData>> GetBarData(Func<DxAssessment, List<DxAssessmentLcStage>, List<NameValueChartData>> func)
            {
                List<List<NameValueChartData>> barData = new List<List<NameValueChartData>>();
                List<DxAssessmentLcStage> allstages = new List<DxAssessmentLcStage>();

                foreach (DxAssessment assessment in DashboardStore.Assessments)
                {
                    allstages = allstages.Union(assessment.LcStages.ToList()).ToList();
                }

                allstages = allstages.OrderBy(o => o.SortOrder).Distinct(new LcStageSortOrderComparer()).ToList();

                foreach (DxAssessment assessment in DashboardStore.Assessments)
                    barData.Add(func(assessment, allstages));

                return barData;
            }

            private List<NameValueChartData> GetMassForAssessment(DxAssessment assessment, List<DxAssessmentLcStage> allLcStages)
            {
                List<NameValueChartData> chartData = new List<NameValueChartData>();

                var outputs = DashboardStore.Outputs.Where(x => x.Assessment == assessment).Select(r => r.Outputs).FirstOrDefault();

                DxAssessmentLcStageCollection stages = DxAssessmentLcStageCollection.New(assessment);
                stages.Load();

                var lcStages = outputs.Select(r => r.LcStage).OrderBy(o => o.SortOrder).Distinct().ToList();

                var missingLcStages = allLcStages.ToList().Except(lcStages, new LcStageSortOrderComparer()).ToList();

                var lcStageValues = outputs.OrderBy(o => o.LcStage.SortOrder)
                                           .GroupBy(x => x.LcStage)
                                           .Select(r => new
                                           {
                                               Name = r.Key.Title,
                                               Value = r.Where(x => x.OutputCategoryId != PRODUCT_OUTPUT_CATEGORY_ID)
                                                         .Sum(x => x.Weight.HasValue ? x.Weight.Value : 0.0m),
                                               r.Key.SortOrder
                                           })
                                           .ToList();

                if (missingLcStages.Count > 0)
                {
                    lcStageValues.AddRange(missingLcStages.Select(r => new
                    { Name = r.Title, Value = 0.0m, r.SortOrder }));
                }

                return lcStageValues.OrderBy(o => o.SortOrder).Select(r => new NameValueChartData
                {
                    Name = r.Name,
                    Value = r.Value
                }).ToList();
            }

            private List<NameValueChartData> GetCostForAssessment(DxAssessment assessment, List<DxAssessmentLcStage> allLcStages)
            {
                var assessmentResults = DashboardStore.Results.Where(x => x.Assessment == assessment).FirstOrDefault();

                var lcStages = assessmentResults.Results.Where(x => x.ResultItem.Title == ESTIMATED_COST).Select(c => c.LcStage);

                var missingLcStages = allLcStages.ToList().Except(lcStages, new LcStageSortOrderComparer()).ToList();

                var chartData = assessmentResults.Results.Where(x => x.ResultItem.Title == ESTIMATED_COST)
                                                         .Select(r => new
                                                         {
                                                             Name = r.LcStage.Title,
                                                             Value = r.ResultItem.Result?.Value ?? 0.0m,
                                                             r.LcStage.SortOrder
                                                         }).ToList();

                if (missingLcStages.Count > 0)
                {
                    chartData.AddRange(missingLcStages.Select(r => new
                    { Name = r.Title, Value = 0.0m, r.SortOrder }));
                }

                return chartData.OrderBy(o => o.SortOrder).Select(r => new NameValueChartData
                {
                    Name = r.Name,
                    Value = r.Value
                }).ToList();
            }

            #endregion

            #region Private Methods - Food Losses

            public (IList<IChartData> Data, IList<string> DestinationColors) GetFoodLosses(DxAssessment assessment, IReadOnlyList<(DxAssessment Assessment, List<DxDestinationPyramidChartItem> Losses)> losses)
            {
                List<IChartData> chartData = new List<IChartData>();
                List<string> destinationColors = new List<string>();

                var foodLosses = losses.Where(x => x.Assessment == assessment)
                    .FirstOrDefault();

                chartData.AddRange(foodLosses.Losses.Select(r =>
                {
                    destinationColors.Add(Destinations[r.Code]);
                    return new NameValueChartData
                    {
                        Name = r.Code,
                        Label = r.Title,
                        Value = NullableDecimal.Round(r.Weight ?? 0.0m, 2)
                    };
                }).ToList());

                return (chartData, destinationColors);
            }

            #endregion
        }

        public class GridStore : IDataStore<IDashboardRowItem>
        {
            private AssessmentDashboardDataStore DashboardStore { get; }

            public GridStore(AssessmentDashboardDataStore store)
            {
                DashboardStore = store;
            }

            public IList<IDashboardRowItem> GetHotSpot(HotSpotValueType type)
            {
                IList<IDashboardRowItem> data;

                switch (type)
                {
                    case HotSpotValueType.MASS:
                        data = GetHotSpotRows(GetMassForAssessment);
                        break;
                    case HotSpotValueType.COST:
                        data = GetHotSpotRows(GetCostForAssessment);
                        break;
                    default:
                        throw new NotSupportedException($"{nameof(type)} is not supported");
                }

                return data;
            }

            public IList<IDashboardRowItem> GetCostOfWaste()
            {
                List<IDashboardRowItem> gridData = new List<IDashboardRowItem>();

                DashboardStore.CostsOfWaste.ForEach((costOfWasteData, assessmentOrder) =>
                {
                    var assessment = costOfWasteData.Assessment;
                    var costs = costOfWasteData.BusinessCosts.Select((r, i) => new DashboardCostOfWasteRowItem(assessment.CurrencySymbol)
                    {
                        IdentifiableString = $"{assessment.IdentifiableString}|{r.Title}",
                        AssessmentDescription = GetAssessmentTitle(assessment),
                        Title = r.TitleDescription,
                        WasteCost = NullableDecimal.Round(r.WasteCost?.Value ?? 0.0m, 2),
                        SortOrder = i,
                        AssessmentSortOrder = assessmentOrder
                    });

                    gridData.AddRange(costs);
                });

                gridData = gridData.OrderBy(o => o.SortOrder)
                    .ThenBy(o => o.AssessmentSortOrder)
                    .ToList();

                return gridData;
            }

            public IList<IDashboardRowItem> GetResults(string resultType)
            {
                List<IDashboardRowItem> gridData = new List<IDashboardRowItem>();

                DashboardStore.Assessments.ForEach((assessment, index) => gridData.AddRange(GetResultsGridData(assessment, resultType, index)));

                gridData = gridData.OrderByDescending(o => o.SortOrder)
                    .ThenBy(o => o.AssessmentSortOrder)
                    .ToList();

                return gridData;
            }

            public (IList<IDashboardRowItem> Data, IList<string> DestinationColors) GetFoodLossesNotIncludedInedibleParts(DxAssessment assessment) => 
                GetFoodLosses(assessment, DashboardStore.FoodLossesNotIncludedInedibleParts);

            public (IList<IDashboardRowItem> Data, IList<string> DestinationColors) GetFoodLossesInediblePartsOnly(DxAssessment assessment) => 
                GetFoodLosses(assessment, DashboardStore.FoodLossesInediblePartsOnly);

            #region Private Methods - Results

            private IEnumerable<DashboardResultRowItem> GetResultsGridData(DxAssessment assessment, string resultType, int assessmentOrder)
            {
                var gridData = DashboardStore.GetResultsFor(assessment, resultType)
                                .Select((r, i) => new DashboardResultRowItem
                                {
                                    IdentifiableString = $"{assessment.IdentifiableString}|{r.Key}",
                                    AssessmentDescription = GetAssessmentTitle(assessment),
                                    Title = r.Key,
                                    Result = r.Average(x => x.ResultItem.Result?.Value ?? 0.0m),
                                    UoM = r.Select(x => x.ResultItem.ResultUoM).First(),
                                    SortOrder = i,
                                    AssessmentSortOrder = assessmentOrder
                                });

                return gridData;
            }

            #endregion

            #region Private Methods - HotSpot

            private IList<IDashboardRowItem> GetHotSpotRows(Func<DxAssessment, IEnumerable<DxAssessmentLcStage>, int, IList<DashboardResultRowItem>> func)
            {
                List<IDashboardRowItem> allRows = new List<IDashboardRowItem>();
                List<DxAssessmentLcStage> allstages = new List<DxAssessmentLcStage>();

                foreach (DxAssessment assessment in DashboardStore.Assessments)
                    allstages = allstages.Union(assessment.LcStages.ToList()).ToList();

                allstages = allstages.OrderBy(o => o.SortOrder).Distinct(new LcStageSortOrderComparer()).ToList();

                DashboardStore.Assessments.ForEach((assessment, index) =>
                {
                    allRows.AddRange(func(assessment, allstages, index));
                });

                allRows = allRows.OrderBy(o => o.SortOrder)
                            .ThenBy(o => o.AssessmentSortOrder)
                            .ToList();

                return allRows;
            }

            private List<DashboardResultRowItem> GetMassForAssessment(DxAssessment assessment, IEnumerable<DxAssessmentLcStage> allLcStages, int assessmentOrder)
            {
                var outputs = DashboardStore.Outputs.Where(x => x.Assessment == assessment).Select(r => r.Outputs).FirstOrDefault();

                var lcStages = outputs.Select(r => r.LcStage).OrderBy(o => o.SortOrder).Distinct().ToList();
                var missingLcStages = allLcStages.ToList().Except(lcStages, new LcStageSortOrderComparer()).ToList();

                string uom = TextResource.Instance.GetResourceModuleByName(ResourceFiles.AssessmentManager).GetLocalizedString("AssessmentDashboard_YAxisTitleHotspotMass");

                var rows = outputs.OrderBy(o => o.LcStage.SortOrder)
                                  .GroupBy(x => x.LcStage)
                                  .Select(r => new DashboardResultRowItem
                                  {
                                      IdentifiableString = $"{assessment.IdentifiableString}|{r.Key.Title}",
                                      Title = r.Key.Title,
                                      Result = NullableDecimal.Round(
                                                    r.Where(x => x.OutputCategoryId != PRODUCT_OUTPUT_CATEGORY_ID)
                                                     .Sum(x => x.Weight ?? 0.0m), 2),
                                      AssessmentDescription = GetAssessmentTitle(assessment),
                                      SortOrder = r.Key.SortOrder,
                                      AssessmentSortOrder = assessmentOrder,
                                      UoM = uom
                                  }).ToList();

                if (missingLcStages.Count > 0)
                    rows.AddRange(missingLcStages.Select(stage => CreateMissingRow(stage, assessment, assessmentOrder, uom)));

                return rows;
            }

            private List<DashboardResultRowItem> GetCostForAssessment(DxAssessment assessment, IEnumerable<DxAssessmentLcStage> allLcStages, int assessmentOrder)
            {
                var assessmentResults = DashboardStore.Results.Where(x => x.Assessment == assessment).FirstOrDefault();

                var lcStages = assessmentResults.Results.Where(x => x.ResultItem.Title == ESTIMATED_COST).Select(c => c.LcStage);

                var missingLcStages = allLcStages.ToList().Except(lcStages, new LcStageSortOrderComparer()).ToList();

                string uom = TextResource.Instance.GetResourceModuleByName(ResourceFiles.AssessmentManager).GetLocalizedString("AssessmentDashboard_YAxisTitleHotspotCost");

                var rows = assessmentResults.Results.Where(x => x.ResultItem.Title == ESTIMATED_COST)
                                                    .Select(r => new DashboardResultRowItem
                                                    {
                                                        IdentifiableString = $"{assessment.IdentifiableString}|{r.LcStage.Title}",
                                                        Title = r.LcStage.Title,
                                                        Result = NullableDecimal.Round(r.ResultItem.Result?.Value ?? 0.0m, 2),
                                                        AssessmentDescription = GetAssessmentTitle(assessment),
                                                        SortOrder = r.LcStage.SortOrder,
                                                        AssessmentSortOrder = assessmentOrder,
                                                        UoM = uom
                                                    }).ToList();

                if (missingLcStages.Count > 0)
                    rows.AddRange(missingLcStages.Select(stage => CreateMissingRow(stage, assessment, assessmentOrder, uom)));

                return rows;
            }

            private DashboardResultRowItem CreateMissingRow(DxAssessmentLcStage stage, DxAssessment assessment, int assessmentOrder, string uom)
            {
                return new DashboardResultRowItem
                {
                    IdentifiableString = $"{assessment.IdentifiableString}|{stage.Title}",
                    Title = stage.Title,
                    Result = NullableDecimal.Round(0.0m, 2),
                    AssessmentDescription = GetAssessmentTitle(assessment),
                    SortOrder = stage.SortOrder,
                    AssessmentSortOrder = assessmentOrder,
                    UoM = uom
                };
            }

            #endregion

            #region Private Methods - Food Losses

            private (IList<IDashboardRowItem> Data, IList<string> DestinationColors) GetFoodLosses(DxAssessment assessment, IReadOnlyList<(DxAssessment Assessment, List<DxDestinationPyramidChartItem> Losses)> losses)
            {
                List<IDashboardRowItem> gridData = new List<IDashboardRowItem>();

                var foodLosses = losses.Where(x => x.Assessment == assessment)
                    .FirstOrDefault();

                gridData.AddRange(foodLosses.Losses.Select(r => new FoodLossRowItem
                {
                    IdentifiableString = $"{foodLosses.Assessment.IdentifiableString}|{r.Code}",
                    Title = r.Title,
                    Weight = NullableDecimal.Round(r.Weight ?? 0.0m, 2),
                    LegendColor = Destinations[r.Code]
                }));

                return (gridData, null);
            }

            #endregion
        }

        private class LcStageSortOrderComparer : IEqualityComparer<DxAssessmentLcStage>
        {
            public bool Equals(DxAssessmentLcStage assessmentLcStage1, DxAssessmentLcStage assessmentLcStage2)
            {
                return assessmentLcStage1.SortOrder == assessmentLcStage2.SortOrder;
            }
            public int GetHashCode(DxAssessmentLcStage assessmentLcStage)
            {
                return assessmentLcStage.SortOrder.GetHashCode();
            }
        }
    }
}