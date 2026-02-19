using System;
using Selerant.DevEx.Modules.DIRModule.BackEnd.OutputManagement;
using Selerant.DevEx.Modules.DIRModule.BackEnd.OutputManagement.GridItem;
using Selerant.DevEx.Modules.DIRModule.Configuration.Resources;

namespace Selerant.DevEx.Modules.DIRModule.WebMvcFrontEnd.Areas.AssessmentNavigator.Models.Panels.ResourceManagement
{
    public class OutputRowItem : BaseCurrencyRowItem, IResourceRowItem
    {
        #region Properties

        public string IdentifiableString { get; set; }
        public string DestinationCode { get; set; }
        public string GridItemTitle { get; set; }
        public decimal? OutputCost { get; set; }
        public bool IsOutputCostEditable { get; set; }
        public string OutputCostFormatted => Formatter.ToCurrency(OutputCost);
        public decimal? Income { get; set; }
        public bool IsIncomeEditable { get; set; }
        public string IncomeFormatted => Formatter.ToCurrency(Income);

        #endregion

        public OutputRowItem(AmountFormatter formatter)
			: base(formatter)
        {
        }

        public OutputRowItem(DxOutputGridItem outputGridItem, AmountFormatter formatter)
			: base(formatter)
        {
            IdentifiableString = outputGridItem.IdentifiableString;
            DestinationCode = outputGridItem.DestinationCode;
            GridItemTitle = outputGridItem.GridItemTitle;
            OutputCost = outputGridItem.OutputCost;
            Income = outputGridItem.Income;
        }

        #region Static Methods

        /// <summary>
        /// Use this constructor if category type is one of these : ("PRODUCT", "COPRODUCT", "FOOD_RESCUE")
        /// </summary>
        /// <returns></returns>
        public static OutputRowItem OfNonWasteOutputCategory(DxOutputGridItem outputGridItem, AmountFormatter formatter, string categoryType)
        {
			var rowItemObj = new OutputRowItem(outputGridItem, formatter)
			{
				id = outputGridItem.IdentifiableString,
				parent = null,
				level = 0,
				isLeaf = true,
				expanded = false,
				IsIncomeEditable = true
			};

			// as requested changes made column editable "Output Cost (as related to material loss)"
			if (categoryType == "COPRODUCT" || categoryType == "FOOD_RESCUE")
            {
                rowItemObj.GridItemTitle = string.Format(Locale.GetString(ResourceFiles.AssessmentManager, "OutputsGridColumn_Destination_FoodLoss"), rowItemObj.GridItemTitle);
                rowItemObj.IsOutputCostEditable = true;
            }

			return rowItemObj;

		}

        public static OutputRowItem OfWasteWaterOutputCategory(DxOutputGridItem outputGridItem, AmountFormatter formatter)
        {
            return new OutputRowItem(outputGridItem, formatter)
            {
                id = outputGridItem.IdentifiableString,
                parent = null,
                level = 0,
                isLeaf = true,
                expanded = false,
                IsOutputCostEditable = true
			};
        }

        public static OutputRowItem OfOutputCategory(DxOutputCategory category, AmountFormatter formatter)
        {
            return new OutputRowItem(formatter)
            {
                id = category.IdentifiableString,
                IdentifiableString = category.IdentifiableString,
                GridItemTitle = $"{Locale.GetString(ResourceFiles.AssessmentManager, "OutputsGridRow_LossPrefix")} {category.Title}",
				parent = null,
                level = 0,
                isLeaf = false,
                expanded = true
            };
        }

        public static OutputRowItem OfDestination(DxOutputGridItem outputGridItem, AmountFormatter formatter, string jqParentId)
        {
            return new OutputRowItem(outputGridItem, formatter)
            {
                id = $"out_{outputGridItem.Id}",
                parent = jqParentId,
                level = 1,
                isLeaf = true,
                expanded = true,
                IsOutputCostEditable = true,
				IsIncomeEditable = true
			};
        }

        #endregion
    }
}