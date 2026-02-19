using System.Globalization;
using Selerant.ApplicationBlocks.Html;
using Selerant.DevEx.WebMvcModules.Infrastructure.Controls.HtmlGrid;

namespace Selerant.DevEx.Modules.DIRModule.WebMvcFrontEnd.Areas.AssessmentNavigator.Models.Panels.ResourceManagement.Converters
{
	[Web.ComponentDescriptor(Web.ComponentIdentifier.IGNORE, Notes = "Provides no securable functionality and expsoses no securable information.")]
	public sealed class ClientEditableBusinessCostValueConverter : HtmlGridValueConverter
	{
		#region IValueConverter Members

		protected override object ConvertValues(object[] values, object collectionItem)
		{
			string formattedAmount = values[0].ToString(); 

			if (string.IsNullOrEmpty(formattedAmount))
				return string.Empty;

			decimal amount = (decimal)values[0];

			return new HtmlPlaceHolderWriter()
					   .AppendWriter(new TagWriter("span").Attribute("data-val", amount.ToString(CultureInfo.InvariantCulture))
					   .AppendText(formattedAmount))
					   .Write();
		}

		#endregion
	}
}