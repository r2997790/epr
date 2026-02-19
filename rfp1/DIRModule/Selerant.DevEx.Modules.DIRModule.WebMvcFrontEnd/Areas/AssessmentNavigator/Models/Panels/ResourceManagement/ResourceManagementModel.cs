using Selerant.DevEx.WebMvcModules.Infrastructure.Navigators.Models;
using System.Collections.Generic;
using System.Linq;
using Selerant.DevEx.WebControls;
using Selerant.DevEx.WebMvcModules.Infrastructure.Controls;
using Selerant.DevEx.Modules.DIRModule.BackEnd.AssessmentManagement.BusinessObjects;
using Selerant.DevEx.Modules.DIRModule.ModuleBlocks._ThisProject;
using Selerant.DevEx.Modules.DIRModule.BackEnd.InputManagement;
using Selerant.DevEx.BusinessLayer;
using Selerant.DevEx.Modules.DIRModule.BackEnd.BusinessCostManagement;
using Selerant.DevEx.Modules.DIRModule.BackEnd.OutputManagement.GridItem;
using Selerant.DevEx.Modules.DIRModule.BackEnd.OutputManagement;
using Selerant.DevEx.Modules.DIRModule.WebMvcFrontEnd.Areas.AssessmentNavigator.Security;
using Selerant.DevEx.Modules.DIRModule.BackEnd.Common;
using Selerant.DevEx.Modules.DIRModule.BackEnd.AssessmentResults;
using Selerant.DevEx.WebMvcModules.Helpers;
using Selerant.DevEx.Modules.DIRModule.Configuration.Resources;
using Selerant.DevEx.Modules.DIRModule.WebMvcFrontEnd.Helpers;

namespace Selerant.DevEx.Modules.DIRModule.WebMvcFrontEnd.Areas.AssessmentNavigator.Models.Panels.ResourceManagement
{
	public class ResourceManagementModel : NavigatorPanelModel<DxAssessment, ResourceManagementSecurity>
	{
		#region Constant

		private const string PANEL_VIEW_CONTAINER_CLASS = "dir-module-bus-nav-resource-management";

        private const string WASTE_WATER_OUTPUT_CATEGORY_TYPE = "WASTE_WATER";
		public const string SEWER = "SEWER";

		#endregion

		public decimal LcStageId { get; set; }
		private DxAssessment Assessment { get; set; }
		private AmountFormatter AmountFormatter { get; set; }

		public Shared.InputsGridTabModel InputsGridTabModel
		{
			get => new Shared.InputsGridTabModel(ControllerUrl, (NavigatorPanelControllerData<DxAssessment>)ControllerData, LcStageId);
		}

		public Shared.DestinationsGridTabModel DestinationsGridTabModel
		{
			get => new Shared.DestinationsGridTabModel(ControllerUrl, (NavigatorPanelControllerData<DxAssessment>)ControllerData, LcStageId, "Food");
		}

		public Shared.DestinationsGridTabModel NonFoodDestinationsGridTabModel
		{
			get => new Shared.DestinationsGridTabModel(ControllerUrl, (NavigatorPanelControllerData<DxAssessment>)ControllerData, LcStageId, "NonFood");
		}

		public Shared.OutputsGridTabModel OutputsGridTabModel
		{
			get => new Shared.OutputsGridTabModel(ControllerUrl, (NavigatorPanelControllerData<DxAssessment>)ControllerData, LcStageId);
		}

		public Shared.BusinessCostGridTabModel BusinessCostGridTabModel
		{
			get => new Shared.BusinessCostGridTabModel(ControllerUrl, (NavigatorPanelControllerData<DxAssessment>)ControllerData, LcStageId);
		}

		public ResourceManagementModel(string controllerUrl, NavigatorPanelControllerData<DxAssessment> data)
		: base(controllerUrl, data)
		{
			Assessment = data.TargetObject;
			Assessment.LoadEntity();
			AmountFormatter = new AmountFormatter(Assessment.CurrencySymbol);

			LcStageId = DxAssessmentLcStageCollection.GetIdOfFirstBySortOrder(Assessment.Code);
		}

        #region Method overrides

        protected override void FillMenuModel()
        {
            HtmlMenu.AddMenuItem(new MenuItem("Export", Locale.GetString(ResourceFiles.AssessmentManager, "ExportToExcel"))
                               .SetEnabled(SecurityObject.CanView));
        }

        protected override void InitializeFromExtraParameters()
		{
			base.InitializeFromExtraParameters();

			if (ExtraParameters.ContainsKey("lcStageId") && decimal.TryParse(ExtraParameters["lcStageId"], out decimal lcStageId))
			{
				LcStageId = lcStageId;
			}
		}

		/// <summary>
		/// Sets the view
		/// </summary>
		protected override void InnerInitializeForView()
		{
			base.InnerInitializeForView();

			AddCssClass(PANEL_VIEW_CONTAINER_CLASS);
            StandardPanelButtons.EditSettings = false;
        }

		/// <summary>
		/// Set client definition of model
		/// </summary>
		/// <param name="scriptControlDescriptor"></param>
		protected override void FillScriptControlDescriptor(GxScriptControlDescriptor scriptControlDescriptor)
		{
			base.FillScriptControlDescriptor(scriptControlDescriptor);
			scriptControlDescriptor.TypeName = "DX.DIRModule.AssessmentNavigator.ResourceManagement.Index";

			scriptControlDescriptor.Data.Add("moduleName", DIRModuleInfo.MODULE_NAME);
			scriptControlDescriptor.Data.Add("viewContainerClass", PANEL_VIEW_CONTAINER_CLASS);
		}

		/// <summary>
		/// Initializes static resources
		/// </summary>
		/// <param name="resources"></param>
		protected override void InitializeResources(List<ControlResource> resources)
		{
			base.InitializeResources(resources);
			resources.Add(new ControlFilesGroup(Infrastructure.StaticResources.CoreBundleNames.BUNDLE_MVCSIMPLECONTROLS));
			resources.Add(new ControlJsFile(DIRLinks.Areas.AssessmentNavigator.Views.ResourceManagement.Index_ts));
			resources.Add(new ControlJsFile(DIRLinks.Areas.AssessmentNavigator.Views.ResourceManagement.Shared.GridTab_ts));
			resources.Add(new ControlCssFile(DIRLinks.Areas.AssessmentNavigator.Views.ResourceManagement.Index_css));
		}

		#endregion

		public List<InputRowItem> BuildInputsGridList()
		{
			var inputs = new DxInputCollection(Assessment.Code, LcStageId);
			inputs.Load();

			return GridHelpers.BuildInputsGridList(inputs, AmountFormatter, true);
		}

		public List<OutputRowItem> BuildOutputsGridList()
		{
			DxOutputGridItemCollection outputs = new DxOutputGridItemCollection(Assessment.Code, LcStageId, true);
			List<DxOutputCategory> outputCategories = outputs.LoadItemOutputCategories();

			List<OutputRowItem> result = new List<OutputRowItem>();

			IEnumerable<OutputRowItem> extractDestinationsForCategory(decimal outputCategoryId, string jqParentId)
			{
				return outputs.Where(x => x.OutputCategoryId == outputCategoryId &&
										 !string.IsNullOrEmpty(x.DestinationCode) &&
										 !(new string[] { GridHelpers.ENVIRONMENT_LOSS }).Contains(x.DestinationCode))
							  .Select(output => OutputRowItem.OfDestination(output, AmountFormatter, jqParentId))
							  .AsEnumerable();
			};

			HashSet<string> specialOutputCategories = new HashSet<string> { "PRODUCT", "COPRODUCT", "FOOD_RESCUE" };

			foreach (DxOutputCategory category in outputCategories.OrderBy(o => o.SortOrder))
			{
				if (specialOutputCategories.Contains(category.Type))
				{
					foreach (DxOutputGridItem outputGridItem in outputs.Where(x => x.OutputCategoryType == category.Type))
						result.Add(OutputRowItem.OfNonWasteOutputCategory(outputGridItem, AmountFormatter, category.Type));
				}
				else if (category.Type == WASTE_WATER_OUTPUT_CATEGORY_TYPE)
				{
					DxOutputGridItem outputGridItem = outputs.First(x => x.OutputCategoryType == category.Type);

					result.Add(OutputRowItem.OfWasteWaterOutputCategory(outputGridItem, AmountFormatter));
				}
				else
				{
					IEnumerable<OutputRowItem> categoryDestinations = extractDestinationsForCategory(category.Id, category.IdentifiableString);
					if (categoryDestinations.Count() > 0)
					{
						result.Add(OutputRowItem.OfOutputCategory(category, AmountFormatter));
						result.AddRange(categoryDestinations);
					}
				}
			}

			return result;
		}

		public List<BusinessCostRowItem> BuildBusinessCostsGridList()
		{
			DxBusinessCostGridItemCollection businessCosts = new DxBusinessCostGridItemCollection(Assessment.Code, LcStageId);
			businessCosts.Load();

			List<BusinessCostRowItem> data = new List<BusinessCostRowItem>(businessCosts.Count);

			foreach (DxBusinessCostGridItem busCost in businessCosts.OrderBy(o => o.SortOrder))
			{
				data.Add(new BusinessCostRowItem(busCost, AmountFormatter)
				{
					id = busCost.IdentifiableString,
				});
			}

			return data;
		}

		public List<DestinationRowItem> BuildDestinationsGridList()
		{
			DxFoodInputDestinationCollection foodInputs = new DxFoodInputDestinationCollection(Assessment.Code, LcStageId, true);
			foodInputs.LoadItemsMaterial();

			var outputCategories = foodInputs.Select(x => x.OutputCategoryId)
											.Distinct()
											.ToList();

			DxOutputCategoryCollection outputCategoriesCollection = new DxOutputCategoryCollection();
			foreach (decimal outputCategroyId in outputCategories)
				outputCategoriesCollection.Add(new DxOutputCategory(outputCategroyId));
			outputCategoriesCollection.LoadItems();

			decimal[] inputIds = foodInputs.Where(x => x.PartOfProductCoproduct).Select(r => r.InputId).ToArray();
			Dictionary<decimal, List<string>> productCoProductsCodes = new DxInputProductCoProductSpreadCollection(inputIds, true).ToDictionary();

			List<DestinationRowItem> result = new List<DestinationRowItem>(foodInputs.Count + outputCategories.Count());
			var simplePercentageFormatter = new SimplePercentageFormatter();

			// 
			List<string> getProductCoProductsCodes(decimal outputCategoryId, decimal inputId)
			{
				if (outputCategoryId != Constants.OutputType.INEDIBLE_ID)
				{
					return productCoProductsCodes.TryGetValue(inputId, out List<string> codes) ? codes : null;
				}
				else
					return null;
			}

			IEnumerable<DestinationRowItem> extractChildrenInputs(decimal outputCategoryId, string jqParentId) // local function
			{
				return foodInputs.Where(x => x.OutputCategoryId == outputCategoryId)
						.Select(foodInput => 
							new DestinationRowItem(foodInput, simplePercentageFormatter)
							{
								id = "inp_" + foodInput.InputId,
								parent = jqParentId,
								level = 1,
								isLeaf = true,
								expanded = true
							}
							.SetPartOfProductCoproduct(getProductCoProductsCodes(outputCategoryId, foodInput.InputId))
						)
						.Cast<DestinationRowItem>()
						.AsEnumerable();
			};

			foreach(DxOutputCategory outputCategory in outputCategoriesCollection)
			{
				string jqId = outputCategory.IdentifiableString;

				result.Add(new DestinationRowItem(outputCategory, simplePercentageFormatter)
				{
					id = jqId,
					parent = null, // Root
					level = 0,
					isLeaf = false,
					expanded = true
				});
				// add children
				result.AddRange(extractChildrenInputs(outputCategory.Id, jqId));
			}

			return result;
		}

		public List<NonFoodDestinationRowItem> BuildNonFoodDestinationsGridList()
		{
			DxNonFoodInputDestinationCollection nonFoodInputs = new DxNonFoodInputDestinationCollection(Assessment.Code, LcStageId, true);
			nonFoodInputs.LoadItemsMaterial();

			var outputCategories = nonFoodInputs.Select(x => x.OutputCategoryId)
										.Distinct()
										.ToList();

			DxOutputCategoryCollection outputCategoriesCollection = new DxOutputCategoryCollection();
			foreach (decimal outputCategroyId in outputCategories)
				outputCategoriesCollection.Add(new DxOutputCategory(outputCategroyId));
			outputCategoriesCollection.LoadItems();

			decimal[] inputIds = nonFoodInputs.Where(x => x.PartOfProductCoproduct).Select(r => r.InputId).ToArray();
			Dictionary<decimal, List<string>> productCoProductsCodes = new DxInputProductCoProductSpreadCollection(inputIds, true).ToDictionary();

			List<NonFoodDestinationRowItem> result = new List<NonFoodDestinationRowItem>(nonFoodInputs.Count + outputCategories.Count());
			var simplePercentageFormatter = new SimplePercentageFormatter();

			IEnumerable<NonFoodDestinationRowItem> extractChildrenInputs(decimal outputCategoryId, string jqParentId) // local function
			{
				return nonFoodInputs.Where(x => x.OutputCategoryId == outputCategoryId)
						.Select(nonFoodInput => 
							new NonFoodDestinationRowItem(nonFoodInput, simplePercentageFormatter)
							{
								id = "inp_" + nonFoodInput.InputId,
								parent = jqParentId,
								level = 1,
								isLeaf = true,
								expanded = true
							}
							.SetPartOfProductCoproduct(productCoProductsCodes.TryGetValue(nonFoodInput.InputId, out List<string> codes) ? codes : null)
						)
						.Cast<NonFoodDestinationRowItem>()
						.AsEnumerable();
			};

			foreach (DxOutputCategory outputCategory in outputCategoriesCollection)
			{
				string jqId = outputCategory.IdentifiableString;

				result.Add(new NonFoodDestinationRowItem(outputCategory, simplePercentageFormatter)
				{
					id = jqId,
					parent = null, // Root
					level = 0,
					isLeaf = false,
					expanded = true
				});
				// add children
				result.AddRange(extractChildrenInputs(outputCategory.Id, jqId));
			}

			return result;
		}
	}
}