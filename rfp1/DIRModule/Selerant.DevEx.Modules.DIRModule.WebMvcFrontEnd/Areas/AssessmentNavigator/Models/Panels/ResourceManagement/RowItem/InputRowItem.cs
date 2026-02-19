using System.Collections.Generic;
using System.Globalization;
using Selerant.DevEx.Modules.DIRModule.BackEnd.InputManagement;
using Selerant.DevEx.Modules.DIRModule.Configuration.Resources;
using Selerant.DevEx.Modules.DIRModule.WebMvcFrontEnd.Helpers;
using InputCategoryType = Selerant.DevEx.Modules.DIRModule.BackEnd.Common.Constants.InputType;

namespace Selerant.DevEx.Modules.DIRModule.WebMvcFrontEnd.Areas.AssessmentNavigator.Models.Panels.ResourceManagement
{
	public class InputRowItem : BaseCurrencyRowItem, IResourceRowItem
	{
		public const string TOTALROW_ID = "totalRow";

		#region Fields

		private readonly decimal? inediblePartsFraction;

		#endregion

		public TreeRowType RowType { get; }

		public string IdentifiableString { get; set; }

		public string CategoryTitle { get; set; }

		public string CategoryIdentifiableString { get; set; }

		public string CategoryType { get; set; }

		public string MaterialIdentifiableString { get; set; }

		public List<string> PartOfProductCoproductCodes { get; private set; }
		public string PartOfProductCoproductFormatted { get; private set; }

		public bool? Packaging { get; private set; }

		// Average mass per annum (kg/year)
		public decimal? Mass { get; set; }
		public string MassFormatted => Formatter.ToThreeDecimals(Mass);

		// Average raw material purchase costs per annum ($/year)
		public decimal? Cost { get; set; }
		public string CostFormatted => Formatter.ToCurrency(Cost);

		// Food (%)
		public string FoodFormatted { get; set; }

		// Inedible Parts (%)
		public decimal? InedibleParts { get; set; }
		public string InediblePartsFormatted => Formatter.FractionToPercentage(inediblePartsFraction);

		public decimal Measurement { get; private set; }
		public string MeasurementFormatted { get; private set; }
        public string ProductSource { get; private set; }

        #region Constructors

        private InputRowItem(TreeRowType rowType, AmountFormatter formatter)
			: base(formatter)
		{
			RowType = rowType;
		}

		public InputRowItem(AmountFormatter formatter)
			: this(TreeRowType.TotalRow, formatter)
		{
			IdentifiableString = id = TOTALROW_ID;
			CategoryTitle = Locale.GetString(ResourceFiles.AssessmentManager, "Inputs_TotalRow");

			Mass = Cost = 0.0m;

			isLeaf = true;
			expanded = false;
		}

		public InputRowItem(DxInputCategory category, AmountFormatter formatter)
			:base(formatter)
		{
			RowType = TreeRowType.Category;

			IdentifiableString = category.IdentifiableString;
			CategoryIdentifiableString = category.IdentifiableString;
			CategoryTitle = category.Title;
			CategoryType = category.Type;
		}

		public InputRowItem(DxInput input, AmountFormatter formatter)
			: this(TreeRowType.ResourceMaterial, formatter)
		{
			IdentifiableString = input.IdentifiableString;
			CategoryTitle = input.Material.Description;
			MaterialIdentifiableString = input.Material.IdentifiableString;

			if (input.Packaging.HasValue)
				Packaging = input.Packaging;
			else if (input.InputCategory.Type == InputCategoryType.NONFOOD)
				Packaging = false;

			Mass = input.Mass;
			Cost = input.Cost;

			if (input.InputCategory.Type == InputCategoryType.FOOD)
			{
				inediblePartsFraction = input.InedibleParts;
				InedibleParts = input.InedibleParts * 100;

				FoodFormatted = Formatter.FractionToPercentage(1 - (inediblePartsFraction ?? 0.0m));
			}
			else
				FoodFormatted = null;

			Measurement = (decimal)input.Measurement;
			MeasurementFormatted = input.MeasurementDesc;

            ProductSource = input.ProductSource;
		}

		#endregion

		public InputRowItem SetPartOfProductCoproductCellValues(List<string> productCoProductsCodes)
		{
			if (productCoProductsCodes != null && productCoProductsCodes.Count > 0)
			{
				PartOfProductCoproductCodes = productCoProductsCodes;
				PartOfProductCoproductFormatted = GridHelpers.Instance.BuildPartOfProductCoProductText(productCoProductsCodes);
			}
			else
				PartOfProductCoproductFormatted = Locale.GetString(ResourceFiles.AssessmentManager, "ATAddActiveStatusNo");

			return this;
		}

		public static InputRowItem OfInputType(DxInput input, AmountFormatter amountFormatter, string jqParentId)
        {
            return new InputRowItem(input, amountFormatter)
            {
                id = "inp_" + input.Id,
                parent = jqParentId,
                level = 1,
                isLeaf = true,
                expanded = true
            };
        }

        public static InputRowItem OfCategoryType(DxInputCategory category, AmountFormatter amountFormatter)
        {
            return new InputRowItem(category, amountFormatter)
            {
                id = category.IdentifiableString,
                parent = null, // Root
                level = 0,
                isLeaf = false,
                expanded = true
            };
        }
    }
}