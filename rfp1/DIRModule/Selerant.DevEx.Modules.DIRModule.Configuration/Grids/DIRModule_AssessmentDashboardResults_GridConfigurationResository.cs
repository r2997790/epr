using Selerant.DevEx.Configuration.Grids.HtmlGrid.DTOs;
using Selerant.DevEx.Configuration.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Selerant.DevEx.Modules.DIRModule.Configuration.Grids
{
    internal class DIRModule_AssessmentDashboardResults_GridConfigurationResository : BaseConfigurationRepository<HtmlGridConfiguration>
    {
        protected HtmlGridConfiguration Version_3_9_0(HtmlGridConfiguration config)
        {
            //Grid Columns
            config.HtmlGridData.WithColumn("IdentifiableString").SetPrimary(true).SetRequired(true).SetExportHidden(true).SetFrozen(true).SetHidden(true)
                .SetLabel("IdentifiableString")
                .SetTargetProperties(new string[] { "IdentifiableString" })
                .SetType("String");

            config.HtmlGridData.WithColumn("Title")
                .SetLabel("@DIR_AssessmentManager.ResultsGridColumn_Category")
                .SetTargetProperties(new string[] { "Title" })
                .SetType("String")
                .SetWidth(400);

            config.HtmlGridData.WithColumn("Assessment")
                .SetLabel("@DIR_AssessmentManager.ResultsGridColumn_Assessment")
                .SetTargetProperties(new string[] { "AssessmentDescription" })
                .SetType("String")
                .SetWidth(400);

            config.HtmlGridData.WithColumn("Result")
                .SetLabel("@DIR_AssessmentManager.ResultsGridColumn_Result")
                .SetTargetProperties(new string[] { "ResultFormatted" })
                .SetType("Decimal")
                .SetWidth(175);

            config.HtmlGridData.WithColumn("UoM")
                .SetLabel("@DIR_AssessmentManager.ResultsGridColumn_UoM")
                .SetTargetProperties(new string[] { "UoM" })
                .SetType("String")
                .SetWidth(175);

            //GridUI
            config.HtmlGridUIs.GetOrCreateHtmlGridUI().WithColumn("IdentifiableString");
            config.HtmlGridUIs.GetOrCreateHtmlGridUI().WithColumn("Title");
            config.HtmlGridUIs.GetOrCreateHtmlGridUI().WithColumn("Assessment");
            config.HtmlGridUIs.GetOrCreateHtmlGridUI().WithColumn("Result");
            config.HtmlGridUIs.GetOrCreateHtmlGridUI().WithColumn("UoM");

            return config;
        }

        protected override System.Collections.Generic.IEnumerable<(Version Version, Func<HtmlGridConfiguration, HtmlGridConfiguration>)> ProvideConfigurationVersionHandlers()
        {
            return new List<(Version Version, Func<HtmlGridConfiguration, HtmlGridConfiguration>)>
            {
                (new Version(3, 9, 0), Version_3_9_0)
            };
        }
    }
}
