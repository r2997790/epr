using Selerant.DevEx.Modules.DIRModule.BackEnd.InputManagement;
using Selerant.DevEx.Modules.DIRModule.BackEnd.OutputManagement;

namespace Selerant.DevEx.Modules.DIRModule.WebMvcFrontEnd.Areas.AssessmentNavigator.Models.Panels.ResourceManagement
{
    public class DestinationRowItem : BaseDestinationRowItem, IResourceRowItem
    {
        #region Constructors

        public DestinationRowItem(DxOutputCategory category, SimplePercentageFormatter simpleformatter)
            : base(TreeRowType.Category, simpleformatter)
        {
            IdentifiableString = category.IdentifiableString;
            Category = category.Title;
            CategoryMaterialIdentifiableString = category.IdentifiableString;
        }

        public DestinationRowItem(DxFoodInputDestination destination, SimplePercentageFormatter simpleformatter)
            : base(destination, simpleformatter)
        {
        }

        #endregion
    }
}