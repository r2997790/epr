using System.Collections.Generic;
using Newtonsoft.Json;
using Selerant.DevEx.Modules.DIRModule.BackEnd.InputManagement;

namespace Selerant.DevEx.Modules.DIRModule.WebMvcFrontEnd.Areas.AssessmentNavigator.Models.Panels.ResourceManagement
{
	public interface IResourceRowItem
	{
		string IdentifiableString { get; set; }
	}

	public enum TreeRowType
	{
		Category = 0,
		ResourceMaterial = 1,
		TotalRow = 2
	}

	public abstract class BaseRowItem
	{
		// jqTree Properties
		public string id { get; set; }
		public string parent { get; set; }
		public int level { get; set; }
		public bool isLeaf { get; set; }
		public bool expanded { get; set; }
		public string icon { get; set; }
	}

	public abstract class BaseDestinationRowItem : BaseRowItem
	{
		protected readonly SimplePercentageFormatter formatter;

        public TreeRowType RowType { get; }
        public string ProductCoproductArray { get; protected set; }

        public string IdentifiableString { get; set; }

        public string Category { get; set; }

        public string CategoryMaterialIdentifiableString { get; set; }
        public string ProductSource { get; set; }
        public decimal? Product { get; set; }
        public string ProductFormatted => formatter.FormatToPercentage(Product);

        public decimal? Product2 { get; set; }
        public string Product2Formatted => formatter.FormatToPercentage(Product2);

        public decimal? Product3 { get; set; }
        public string Product3Formatted => formatter.FormatToPercentage(Product3);

        public decimal? CoProduct { get; set; }
        public string CoProductFormatted => formatter.FormatToPercentage(CoProduct);

        public decimal? CoProduct2 { get; set; }
        public string CoProduct2Formatted => formatter.FormatToPercentage(CoProduct2);

        public decimal? FoodRescue { get; set; }

        public string FoodRescueFormatted => formatter.FormatToPercentage(FoodRescue);

        public decimal? AnimalFeed { get; set; }

        public string AnimalFeedFormatted => formatter.FormatToPercentage(AnimalFeed);

        public decimal? BiomassMaterial { get; set; }

        public string BiomassMaterialFormatted => formatter.FormatToPercentage(BiomassMaterial);

        public decimal? CodigestionAnaerobic { get; set; }

        public string CodigestionAnaerobicFormatted => formatter.FormatToPercentage(CodigestionAnaerobic);

        public decimal? Composting { get; set; }

        public string CompostingFormatted => formatter.FormatToPercentage(Composting);

        public decimal? Combustion { get; set; }

        public string CombustionFormatted => formatter.FormatToPercentage(Combustion);

        public decimal? LandApp { get; set; }

        public string LandAppFormatted => formatter.FormatToPercentage(LandApp);

        public decimal? Recycling { get; set; }

        public string RecyclingFormatted => formatter.FormatToPercentage(Recycling);

        public decimal? IncinWithEnRecover { get; set; }

        public string IncinWithEnRecoverFormatted => formatter.FormatToPercentage(IncinWithEnRecover);

        public decimal? Landfill { get; set; }

        public string LandfillFormatted => formatter.FormatToPercentage(Landfill);

        public decimal? NotHarvested { get; set; }

        public string NotHarvestedFormatted => formatter.FormatToPercentage(NotHarvested);

        public decimal? RefuseDiscard { get; set; }

        public string RefuseDiscardFormatted => formatter.FormatToPercentage(RefuseDiscard);

        public decimal? Sewer { get; set; }

        public string SewerFormatted => formatter.FormatToPercentage(Sewer);

        public decimal? EnvironmentLoss { get; set; }

        public string EnvironmentLossFormatted => formatter.FormatToPercentage(EnvironmentLoss);

        public decimal? Other { get; set; }

        public string OtherFormatted => formatter.FormatToPercentage(Other);

        #region Constructor

        public BaseDestinationRowItem(SimplePercentageFormatter simpleFormatter)
		{
			formatter = simpleFormatter;
		}

        protected BaseDestinationRowItem(TreeRowType rowType, SimplePercentageFormatter simpleFormatter)
        {
            RowType = rowType;
            formatter = simpleFormatter;
        }

        public BaseDestinationRowItem(IInputDestination destination, SimplePercentageFormatter simpleFormatter)
            : this(TreeRowType.ResourceMaterial, simpleFormatter)
        {
            IdentifiableString = destination.IdentifiableString;
            Category = destination.Material.Description;
            CategoryMaterialIdentifiableString = destination.Material.IdentifiableString;

            ProductSource = destination.ProductSource;

            Product = destination.Product;
            Product2 = destination.Product2;
            Product3 = destination.Product3;
            CoProduct = destination.Coproduct;
            CoProduct2 = destination.Coproduct2;

            FoodRescue = destination.FoodRescue;
            AnimalFeed = destination.AnimalFeed;
            BiomassMaterial = destination.BiomassMaterial;
            CodigestionAnaerobic = destination.CodigestionAnaerobic;
            Composting = destination.Composting;
            Combustion = destination.Combustion;
            LandApp = destination.LandApplication;
            Landfill = destination.Landfill;
            Recycling = destination.Recycling;
            IncinWithEnRecover = destination.IncinWithEnRecover;
            NotHarvested = destination.NotHarvested;
            RefuseDiscard = destination.RefuseDiscard;
            Landfill = destination.Landfill;
            Sewer = destination.Sewer;
            EnvironmentLoss = destination.EnvironmentLoss;
            Other = destination.Other;
        }

        #endregion

        public BaseDestinationRowItem SetPartOfProductCoproduct(List<string> productCoProductsCodes)
		{
			if (productCoProductsCodes != null && productCoProductsCodes.Count > 0)
				ProductCoproductArray = JsonConvert.SerializeObject(productCoProductsCodes);
			else
				ProductCoproductArray = "[]";

			return this;
		}
	}

	public abstract class BaseCurrencyRowItem : BaseRowItem
	{
		public AmountFormatter Formatter { get; private set; }

		public BaseCurrencyRowItem(AmountFormatter amountFormatter)
		{
			Formatter = amountFormatter;
		}
	}
}