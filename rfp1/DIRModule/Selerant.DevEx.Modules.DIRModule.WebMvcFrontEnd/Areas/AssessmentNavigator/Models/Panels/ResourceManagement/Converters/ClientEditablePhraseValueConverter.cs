using Selerant.ApplicationBlocks.Html;
using Selerant.DevEx.WebMvcModules.Infrastructure.Controls.HtmlGrid;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Selerant.DevEx.Modules.DIRModule.WebMvcFrontEnd.Areas.AssessmentNavigator.Models.Panels.ResourceManagement.Converters
{
    [Web.ComponentDescriptor(Web.ComponentIdentifier.IGNORE, Notes = "Provides no securable functionality and expsoses no securable information.")]
    public class ClientEditablePhraseValueConverter : HtmlGridValueConverter
    {
        protected override object ConvertValues(object[] values, object collectionItem)
        {
            string phraseText = values[0] as string;
            string description = "";

            if (values.Length == 2)
                description = values[1] as string;

            return new HtmlPlaceHolderWriter()
                .AppendWriter(new TagWriter("span").Attribute("data-val", phraseText)
                .AppendText(!string.IsNullOrEmpty(description) ? description : phraseText))
                .Write();
        }
    }
}