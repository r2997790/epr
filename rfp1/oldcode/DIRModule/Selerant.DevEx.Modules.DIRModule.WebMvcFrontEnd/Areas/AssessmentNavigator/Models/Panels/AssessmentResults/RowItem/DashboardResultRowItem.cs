namespace Selerant.DevEx.Modules.DIRModule.WebMvcFrontEnd.Areas.AssessmentNavigator.Models.Panels.AssessmentResults.RowItem
{
    public class DashboardResultRowItem : ResultRowItem, IDashboardRowItem
    {
        public string AssessmentDescription { get; set; }
        public int AssessmentSortOrder { get; set; }
    }
}