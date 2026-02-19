namespace Selerant.DevEx.Modules.DIRModule.WebMvcFrontEnd.Areas.AssessmentNavigator.Models.Panels.AssessmentResults.RowItem
{
    public class ResultRowItem : BaseRowItem
    {
        public string IdentifiableString { get; set; }
        public string Title { get; set; }
        public decimal Result { get; set; }
        public string ResultFormatted => Result.ToString("0.00", CurrentUserNumberInfo);
        public string UoM { get; set; }
    }
}