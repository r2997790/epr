using System.Collections.Generic;
using System.Text.RegularExpressions;
using Selerant.ApplicationBlocks.Html;
using Selerant.ApplicationBlocks.JavaScript;
using Selerant.DevEx.Modules.DIRModule.Configuration.Resources;
using Selerant.DevEx.Modules.DIRModule.WebMvcFrontEnd.Areas.AssessmentNavigator.Security;
using Selerant.DevEx.Modules.DIRModule.WebMvcFrontEnd.Helpers;
using Selerant.DevEx.WebMvcModules.Infrastructure.Controls.HtmlGrid;

namespace Selerant.DevEx.Modules.DIRModule.WebMvcFrontEnd.Areas.AssessmentNavigator.Models.Panels.ResourceManagement.Converters
{
	[Web.ComponentDescriptor(Web.ComponentIdentifier.IGNORE, Notes = "Provides no securable functionality and expsoses no securable information.")]
	public class InputCategoryActionConverter : HtmlGridValueConverter
	{
		private string iconTitle = null;
		public string IconTitle
		{
			get
			{
				if (iconTitle == null)
					iconTitle = Locale.GetString(ResourceFiles.AssessmentManager, "InputsGrid_CategoryAddIconTitle");
				return iconTitle;
			}
		}

		protected override object ConvertValues(object[] values, object collectionItem)
		{
			string categoryIdentifiableString = (string)values[0];
            string productSource = (string)values[1];

            if (string.IsNullOrEmpty(productSource))
            {
                if (string.IsNullOrEmpty(categoryIdentifiableString) || !((ResourceManagementSecurity)SecurityObject).CanEditOrDelete)
                    return string.Empty;

                return new HtmlPlaceHolderWriter().AppendWriter(new TagWriter("img").CssClass("category-icon-add")
                        .Attributes(new Dictionary<string, string>
                        {
                        { "src", UrlHelper.Content("~/WebMvcModules/Content/Images/toolbar_icons/DxAddUltraDarkGrey.svg") },
                        { "title", "Add material input to category" },
                        { "onclick", $"DX.Ctrl.findParentControlOfType(this, DX.DIRModule.AssessmentNavigator.ResourceManagement.Shared.InputsGridTab).AddRow({ExJavaScriptSerializer.Instance.Serialize(categoryIdentifiableString)});" }
                        }))
                        .Write();
            }
            else
            {
                var productSourceShortend = GridHelpers.GetProductSourceShortened(productSource);
                var tooltip = GridHelpers.GetProductSourceTooltipText(productSourceShortend);

                return new HtmlPlaceHolderWriter()
                    .AppendWriter(new TagWriter("span")
                        .Attribute("data-val", productSource)
                        .Attribute("title", tooltip)
                        .AppendText(productSourceShortend)
                    ).Write();
            }
			
		}
	}
}