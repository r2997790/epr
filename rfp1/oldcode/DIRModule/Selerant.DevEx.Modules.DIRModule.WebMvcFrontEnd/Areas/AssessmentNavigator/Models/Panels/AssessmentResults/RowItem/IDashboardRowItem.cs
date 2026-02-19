namespace Selerant.DevEx.Modules.DIRModule.WebMvcFrontEnd.Areas.AssessmentNavigator.Models.Panels.AssessmentResults.RowItem
{
    public interface IDashboardRowItem : IRowItem
    {
        string AssessmentDescription { get; set; }
        int AssessmentSortOrder { get; set; }
    }
}
