using System.Globalization;
using Selerant.ApplicationBlocks.Html;
using Selerant.DevEx.WebMvcModules.Infrastructure.Controls.HtmlGrid;

namespace Selerant.DevEx.Modules.DIRModule.WebMvcFrontEnd.Areas.AssessmentNavigator.Models.Panels.ResourceManagement.Converters
{
	[Web.ComponentDescriptor(Web.ComponentIdentifier.IGNORE, Notes = "Provides no securable functionality and expsoses no securable information.")]
	public sealed class ClientEditableDecimalConverter : HtmlGridValueConverter
	{
		protected override object ConvertValues(object[] values, object collectionItem)
		{
			string formattedAmount = values[0] as string;

			decimal? amount = null;
			if (values.Length == 2) // Amount can be not set
				amount = (decimal?)values[1];

			if (!string.IsNullOrEmpty(formattedAmount))
			{
                decimal amountValue = amount.HasValue ? amount.Value : 0.0m;

                // Editable cell
                var placeHolder = new HtmlPlaceHolderWriter();
                var tagWriter = new TagWriter("span").Attribute("data-val", amountValue.ToString("0.####", CultureInfo.InvariantCulture))
                                                        .AppendText(formattedAmount);


                if (amountValue == 0)
                    tagWriter.Style("opacity", ".5");

                return placeHolder.AppendWriter(tagWriter).Write();
			}
			else // Category row (no values)
				return string.Empty;
		}
	}
}