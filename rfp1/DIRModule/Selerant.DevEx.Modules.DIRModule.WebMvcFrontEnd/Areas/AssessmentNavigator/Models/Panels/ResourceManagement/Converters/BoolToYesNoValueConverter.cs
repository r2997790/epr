using Selerant.ApplicationBlocks.Html;
using Selerant.DevEx.Modules.DIRModule.Configuration.Resources;
using Selerant.DevEx.WebMvcModules.Infrastructure.Controls.HtmlGrid;

namespace Selerant.DevEx.Modules.DIRModule.WebMvcFrontEnd.Areas.AssessmentNavigator.Models.Panels.ResourceManagement.Converters
{
	[Web.ComponentDescriptor(Web.ComponentIdentifier.IGNORE, Notes = "Provides no securable functionality and expsoses no securable information.")]
	public sealed class BoolToYesNoValueConverter : HtmlGridValueConverter
	{
		protected override object ConvertValues(object[] values, object collectionItem)
		{
			bool? value = (bool?)values[0];

			if (!value.HasValue)
				return string.Empty;

			string textValue = value.Value ? Locale.GetString(ResourceFiles.AssessmentManager, "ATAddActiveStatusYes") : Locale.GetString(ResourceFiles.AssessmentManager, "ATAddActiveStatusNo");

			return new HtmlPlaceHolderWriter()
				.AppendHtml($@"<span data-val=""{(value.Value ? "true" : "false")}"">")
                .AppendText(textValue)
				.Write();
		}
	}
}