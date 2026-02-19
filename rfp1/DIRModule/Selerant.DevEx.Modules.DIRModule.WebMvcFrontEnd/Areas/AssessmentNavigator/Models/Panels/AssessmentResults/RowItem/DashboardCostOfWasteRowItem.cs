namespace Selerant.DevEx.Modules.DIRModule.WebMvcFrontEnd.Areas.AssessmentNavigator.Models.Panels.AssessmentResults.RowItem
{
    public class DashboardCostOfWasteRowItem : CostOfWasteRowItem, IDashboardRowItem
    {
        public string IdentifiableString { get; set; }
        public string AssessmentDescription { get; set; }
        public int AssessmentSortOrder { get; set; }

        public DashboardCostOfWasteRowItem(string currencySymbol) 
            : base(currencySymbol)
        {
        }
    }
}