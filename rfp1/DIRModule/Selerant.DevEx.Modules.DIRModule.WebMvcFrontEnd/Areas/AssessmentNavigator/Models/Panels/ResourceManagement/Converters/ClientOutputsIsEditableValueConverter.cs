using Selerant.ApplicationBlocks.Html;
using Selerant.DevEx.Web;
using Selerant.DevEx.WebMvcModules.Infrastructure.Controls.HtmlGrid;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;

namespace Selerant.DevEx.Modules.DIRModule.WebMvcFrontEnd.Areas.AssessmentNavigator.Models.Panels.ResourceManagement.Converters
{
    [ComponentDescriptor(ComponentIdentifier.IGNORE, Notes = "Provides no securable functionality and expsoses no securable information.")]
    public sealed class ClientOutputsIsEditableValueConverter : HtmlGridValueConverter
    {
        protected override object ConvertValues(object[] values, object collectionItem)
        {
            if (values.Length != 3)
                throw new ArgumentException("Converter requires three arguments");

            string formattedValue = values[0] as string;
            decimal? value = (decimal?)values[1];
            bool isEditable = (bool)values[2];

            return new HtmlPlaceHolderWriter()
                        .AppendWriter(new TagWriter("span")
                        .Attribute("data-is-editable", isEditable.ToString(CultureInfo.InvariantCulture).ToLower())
                        .Attribute("data-val", value.HasValue ? value.Value.ToString(CultureInfo.InvariantCulture) : "")
                        .AppendText(formattedValue))
                        .Write();            
        }
    }
}