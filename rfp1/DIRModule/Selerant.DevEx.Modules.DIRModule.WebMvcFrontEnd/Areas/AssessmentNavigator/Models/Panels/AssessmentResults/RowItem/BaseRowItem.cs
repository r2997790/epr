using Selerant.DevEx.BusinessLayer;
using System.Globalization;

namespace Selerant.DevEx.Modules.DIRModule.WebMvcFrontEnd.Areas.AssessmentNavigator.Models.Panels.AssessmentResults.RowItem
{
    public abstract class BaseRowItem : IRowItem
    {
        private NumberFormatInfo currentUserNumberInfo;

        protected NumberFormatInfo CurrentUserNumberInfo
        {
            get
            {
                if (currentUserNumberInfo == null)
                {
                    currentUserNumberInfo = DxUser.CurrentUser.GetCulture().NumberFormat;
                    currentUserNumberInfo.PercentPositivePattern = 1;
                }

                return currentUserNumberInfo;
            }
        }

        public decimal SortOrder { get; set; }
    }
}