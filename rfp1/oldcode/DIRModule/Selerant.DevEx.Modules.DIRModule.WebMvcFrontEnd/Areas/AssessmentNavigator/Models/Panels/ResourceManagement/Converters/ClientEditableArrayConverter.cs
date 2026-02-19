using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Selerant.ApplicationBlocks.Html;
using Selerant.DevEx.WebMvcModules.Infrastructure.Controls.HtmlGrid;

namespace Selerant.DevEx.Modules.DIRModule.WebMvcFrontEnd.Areas.AssessmentNavigator.Models.Panels.ResourceManagement.Converters
{
	[Web.ComponentDescriptor(Web.ComponentIdentifier.IGNORE, Notes = "Provides no securable functionality and expsoses no securable information.")]
	public class ClientEditableArrayConverter : HtmlGridValueConverter
	{
		protected override object ConvertValues(object[] values, object collectionItem)
		{
			string formattedText = values[0] as string;

			if (!string.IsNullOrEmpty(formattedText))
			{
				List<string> array = (List<string>)values[1];
				// Editable cell
				string jsonValue = array != null ? JsonConvert.SerializeObject(array) : "[]";
				var tagWriter = new TagWriter("span").Attribute("data-val", jsonValue)
														.AppendText(formattedText);

				return new HtmlPlaceHolderWriter().AppendWriter(tagWriter).Write();
			}
			else
				return string.Empty;
		}
	}
}