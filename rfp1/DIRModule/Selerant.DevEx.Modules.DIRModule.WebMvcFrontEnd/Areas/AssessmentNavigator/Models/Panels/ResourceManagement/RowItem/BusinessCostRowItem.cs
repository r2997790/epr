using System;
using Selerant.DevEx.Modules.DIRModule.BackEnd.BusinessCostManagement;

namespace Selerant.DevEx.Modules.DIRModule.WebMvcFrontEnd.Areas.AssessmentNavigator.Models.Panels.ResourceManagement
{
	public class BusinessCostRowItem : BaseCurrencyRowItem
	{
        #region Properties

        public string IdentifiableString { get; set; }
        public string Title { get; set; }
        public string TitleDescription { get; set; }
        public decimal Cost { get; set; }
        public string CostFormatted => Formatter.ToCurrency(Cost);
        public decimal TotalCost { get; set; }
        public string TotalCostFormatted => Formatter.ToCurrency(TotalCost);
        public decimal CarriedCost { get; set; }
        public string CarriedCostFormatted => Formatter.ToCurrency(CarriedCost);
        public decimal WasteCost { get; set; }        
        public string WasteCostFormatted => Formatter.ToCurrency(WasteCost);
		public bool IsCarriedOverCost { get; set; }

		#endregion

		#region Constructors

		public BusinessCostRowItem(DxBusinessCostGridItem businessCosts, AmountFormatter formatter)
			: base(formatter)
		{
            IdentifiableString = businessCosts.IdentifiableString;
			Title = businessCosts.Title;
            TitleDescription = businessCosts.TitleDescription;
			Cost = businessCosts.Cost?.Value ?? 0.0m;
            TotalCost = businessCosts.TotalCost?.Value ?? 0.0m;
            CarriedCost = businessCosts.CarriedCost?.Value ?? 0.0m;
			WasteCost = businessCosts.WasteCost?.Value ?? 0.0m;
			IsCarriedOverCost = businessCosts.Id < 0;
		}

		#endregion
    }
}