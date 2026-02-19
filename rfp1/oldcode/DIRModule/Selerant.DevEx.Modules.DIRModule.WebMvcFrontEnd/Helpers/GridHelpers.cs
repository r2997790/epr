using Selerant.DevEx.BusinessLayer;
using Selerant.DevEx.Modules.DIRModule.BackEnd.DestinationManagement;
using Selerant.DevEx.Modules.DIRModule.BackEnd.InputManagement;
using Selerant.DevEx.Modules.DIRModule.Configuration.Resources;
using Selerant.DevEx.Modules.DIRModule.WebMvcFrontEnd.Areas.AssessmentNavigator.Models.Panels.ResourceManagement;
using Selerant.DevEx.WebMvcModules.Infrastructure.Resources.Text;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text.RegularExpressions;

namespace Selerant.DevEx.Modules.DIRModule.WebMvcFrontEnd.Helpers
{
    public static class GridHelpers
    {
		#region Constants

		public const string CATEGORY = "Category";
		public const string ACTIONS = "Actions";

		public const string ENVIRONMENT_LOSS = "ENVIRONMENT_LOSS";

		#endregion

		public static HelperWrapper Instance { get; } = new HelperWrapper();

		public sealed class HelperWrapper
		{
			//private readonly Dictionary<string, string> destinationsTitles;
			private readonly DxDestinationCollection destinations;

			public readonly IReadOnlyCollection<string> NonWasteDestinationCodes = new HashSet<string>() { "PRODUCT", "PRODUCT_2", "PRODUCT_3", "COPRODUCT", "COPRODUCT_2", "FOOD_RESCUE" };
			public readonly IReadOnlyCollection<string> DefaultDestinations;
			public readonly IReadOnlyCollection<string> ProductCoProductDestionCodes;
			public readonly IReadOnlyCollection<string> FixedDestinationsColumns;

			public HelperWrapper()
			{
				// Initilize
				HashSet<string> defaultDestinations = new HashSet<string>(8);
				foreach (string nonWasteDest in NonWasteDestinationCodes)
					defaultDestinations.Add(nonWasteDest);
				defaultDestinations.Add("ENVIRONMENT_LOSS");
				defaultDestinations.Add("OTHER");

				DefaultDestinations = defaultDestinations;

				ProductCoProductDestionCodes = NonWasteDestinationCodes.Where(x => x != "FOOD_RESCUE").ToList();

				var tempList = new List<string>(10);
				tempList.Add(CATEGORY);

				foreach (string destination in NonWasteDestinationCodes)
				{
					tempList.Add(destination);
				}

				tempList.Add("ENVIRONMENT_LOSS");
				tempList.Add("OTHER");
				tempList.Add(ACTIONS);
				FixedDestinationsColumns = tempList;

				destinations = new DxDestinationCollection();
				destinations.AddItem("PRODUCT");
				destinations.AddItem("PRODUCT_2");
				destinations.AddItem("PRODUCT_3");
				destinations.AddItem("COPRODUCT");
				destinations.AddItem("COPRODUCT_2");
				destinations.LoadItems();
			}

			public string BuildPartOfProductCoProductText(IList<string> codes)
			{
				var pickedAndOrdered = destinations.Where(x => codes.Contains(x.Code))
										 .OrderBy(o => o.SortOrder)
										 .Select(r => r.Title);

				return string.Join(",", pickedAndOrdered);
			}

			public Dictionary<string, string> GetProductsCodeTitle()
			{
				return destinations.Where(x => x.Code.StartsWith("PRODUCT")).ToDictionary(key => key.Code, value => value.Title);
			}
		}

		public static List<InputRowItem> BuildInputsGridList(DxInputCollection inputs, AmountFormatter amountFormatter, bool includeTotalRow)
        {
            if (inputs.PersistenceStatus == DxPersistenceStatus.Unknown)
                inputs.Load();
			inputs.LoadItemsMaterial();

			Dictionary<decimal, List<string>> productCoProductsCodes = inputs.LoadInputIdsProductCoProductCodes();
			List<DxInputCategory> inputCategories = inputs.LoadItemsInputCategory();

            List<InputRowItem> result = new List<InputRowItem>(inputs.Count + inputCategories.Count + 1);

            IEnumerable<InputRowItem> extractChildrenInputs(decimal categoryId, string jqParentId) // local function
            {
				return inputs.Where(x => x.InputCategoryId == categoryId)
						.OrderBy(o => o.InputSortOrder)
						.Select(input => 
							new InputRowItem(input, amountFormatter)
							{
								id = "inp_" + input.Id,
								parent = jqParentId,
								level = 1,
								isLeaf = true,
								expanded = true
							}
							.SetPartOfProductCoproductCellValues(productCoProductsCodes.TryGetValue(input.Id, out List<string> codes) ? codes : null)
						)
                        .AsEnumerable();
            };

            foreach (DxInputCategory category in inputCategories.OrderBy(o => o.SortOrder))
            {
                string jqId = category.IdentifiableString;

                result.Add(new InputRowItem(category, amountFormatter)
                {
                    id = jqId,
                    parent = null, // Root
                    level = 0,
                    isLeaf = false,
                    expanded = true
                });
                // add children
                result.AddRange(extractChildrenInputs(category.Id, jqId));
            }

            // Build last Total Row
            if (result.Count > 0 && includeTotalRow)
            {
                InputRowItem totalRow = result.Aggregate(new InputRowItem(amountFormatter), (accTotalRow, inputRow) =>
                {
                    if (inputRow.RowType == TreeRowType.ResourceMaterial)
                    {
                        accTotalRow.Mass += inputRow.Mass ?? 0.0m;
                        accTotalRow.Cost += inputRow.Cost ?? 0.0m;

                        return accTotalRow;
                    }
                    else
                        return accTotalRow;
                });

                result.Add(totalRow);
            }

            return result;
        }

        public static string GetProductSourceShortened(string productSource)
        {
            if (string.IsNullOrEmpty(productSource))
                throw new ArgumentNullException(nameof(productSource), "Product Source is not defined");

            var productSourceShortend = productSource.Substring(0, 1);
            var match = Regex.Match(productSource, @"\d+");
            productSourceShortend += !string.IsNullOrEmpty(match.Value) ? match.Value : "1";

            return productSourceShortend;
        }

        public static string GetProductSourceTooltipText(string productSourceShortened) => string.Format(Locale.GetString(ResourceFiles.AssessmentManager, "CarriedOverFromPreviousStageTT"), Locale.GetString(ResourceFiles.AssessmentManager, $"ProductSource_{productSourceShortened}"));
    }
}