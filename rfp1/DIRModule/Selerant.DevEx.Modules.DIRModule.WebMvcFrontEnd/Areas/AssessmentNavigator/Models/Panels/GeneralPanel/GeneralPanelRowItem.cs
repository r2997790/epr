using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Selerant.DevEx.Modules.DIRModule.WebMvcFrontEnd.Areas.AssessmentNavigator.Models.Panels.GeneralPanel
{
    public class GeneralPanelRowItem
    {
        public string Title { get; }
        public string Value { get; }

        public GeneralPanelRowItem(string title, string value)
        {
            Title = title;
            Value = value;
        }

        public static GeneralPanelRowItem Of(string title, string value) => new GeneralPanelRowItem(title, value);
    }
}