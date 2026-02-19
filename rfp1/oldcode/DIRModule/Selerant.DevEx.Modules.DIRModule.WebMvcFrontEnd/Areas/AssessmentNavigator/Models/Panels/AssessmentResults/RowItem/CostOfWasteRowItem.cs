namespace Selerant.DevEx.Modules.DIRModule.WebMvcFrontEnd.Areas.AssessmentNavigator.Models.Panels.AssessmentResults.RowItem
{
    public class CostOfWasteRowItem : BaseRowItem
    {
        private string CurrencySymbol { get; }
        public decimal Id { get; set; }
        public string Title { get; set; }
        public decimal WasteCost { get; set; }
        public string WasteCostFormatted { get => $"{CurrencySymbol}{WasteCost.ToString("n", CurrentUserNumberInfo)}"; }
        public decimal Percentage { get; set; }
        public string PercentageFormatted { get => Percentage.ToString("P", CurrentUserNumberInfo); }

        public CostOfWasteRowItem(string currencySymbol)
        {
            CurrencySymbol = currencySymbol;
        }
    }
}