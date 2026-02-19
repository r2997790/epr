using Selerant.DevEx.Modules.DIRModule.BackEnd.InputManagement;
using Selerant.DevEx.Modules.DIRModule.BackEnd.OutputManagement;

namespace Selerant.DevEx.Modules.DIRModule.WebMvcFrontEnd.Areas.AssessmentNavigator.Models.Panels.ResourceManagement
{
	public class NonFoodDestinationRowItem : BaseDestinationRowItem, IResourceRowItem
	{
		#region Constructors

		public NonFoodDestinationRowItem(DxOutputCategory category, SimplePercentageFormatter simpleFormatter)
			: base(TreeRowType.Category, simpleFormatter)
		{
			IdentifiableString = category.IdentifiableString;
			Category = category.Title;
			CategoryMaterialIdentifiableString = category.IdentifiableString;
		}

        public NonFoodDestinationRowItem(DxNonFoodInputDestination destination, SimplePercentageFormatter simpleformatter)
            : base(destination, simpleformatter)
        {
        }

        #endregion
    }
}