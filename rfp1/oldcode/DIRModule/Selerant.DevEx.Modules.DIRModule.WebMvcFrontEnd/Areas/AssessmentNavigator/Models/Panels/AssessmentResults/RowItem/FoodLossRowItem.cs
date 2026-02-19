using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Selerant.DevEx.Modules.DIRModule.WebMvcFrontEnd.Areas.AssessmentNavigator.Models.Panels.AssessmentResults.RowItem
{
    public class FoodLossRowItem : IDashboardRowItem
    {
        public string IdentifiableString { get; set; }
        public string Title { get; set; }
        public decimal Weight { get; set; }
        public string LegendColor { get;  set; }
        public string AssessmentDescription { get; set; }
        public int AssessmentSortOrder { get; set; }
        public decimal SortOrder { get; set; }
    }
}