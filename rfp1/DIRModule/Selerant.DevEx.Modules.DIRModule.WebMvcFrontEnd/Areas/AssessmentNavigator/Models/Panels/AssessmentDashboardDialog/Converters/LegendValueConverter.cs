using Selerant.ApplicationBlocks.Html;
using Selerant.DevEx.WebMvcModules.Infrastructure.Controls.HtmlGrid;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Selerant.DevEx.Modules.DIRModule.WebMvcFrontEnd.Areas.AssessmentNavigator.Models.Panels.AssessmentDashboardDialog.Converters
{
    [Web.ComponentDescriptor(Web.ComponentIdentifier.IGNORE, Notes = "Provides no securable functionality and expsoses no securable information.")]
    public class LegendValueConverter : HtmlGridValueConverter
    {
        protected override object ConvertValues(object[] values, object collectionItem)
        {
            if (values.Length > 1)
            {
                string text = values[0] as string;
                string color = values[1] as string;

                var placeholder = new HtmlPlaceHolderWriter()
                    .AppendWriter(
                        new TagWriter("span")
                            .Attribute("class", "dir-module-grid-legend-item")
                            .AppendTag(new TagWriter("i")
                                .Attribute("style", $"background-color:{color}"))
                            .AppendTag(new TagWriter("p")
                                .AppendText(text))
                    ).Write();

                return placeholder;
            }
            else
            {
                return new HtmlPlaceHolderWriter()
                        .AppendText(values[0] as string)
                        .Write();
            }
        }
    }
}