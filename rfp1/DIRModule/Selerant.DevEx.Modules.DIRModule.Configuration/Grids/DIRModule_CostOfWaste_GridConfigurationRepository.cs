using Selerant.DevEx.Configuration.Grids.HtmlGrid;
using Selerant.DevEx.Configuration.Grids.HtmlGrid.DTOs;
using Selerant.DevEx.Configuration.Infrastructure;
using Selerant.DevEx.Modules.DIRModule.ModuleBlocks._ThisProject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Selerant.DevEx.Modules.DIRModule.Configuration.Grids
{
    internal class DIRModule_CostOfWaste_GridConfigurationRepository : BaseConfigurationRepository<HtmlGridConfiguration>, ICoreHtmlGridConfigurationRepository
    {
        public string GridName => GridNames.COST_OF_WASTE;

        private HtmlGridConfiguration Version_3_9_0(HtmlGridConfiguration config)
        {
            config.HtmlGridData.WithColumn("Id").SetPrimary(true).SetRequired(true).SetExportHidden(true).SetFrozen(true).SetHidden(true)
                .SetLabel("Id")
                .SetTargetProperties(new string[] { "Id" })
                .SetType("Decimal");

            config.HtmlGridData.WithColumn("Title")
                .SetLabel("@DIR_AssessmentManager.ResultsGridColumn_Category")
                .SetTargetProperties(new string[] { "Title" })
                .SetType("String")
                .SetWidth(400);

            config.HtmlGridData.WithColumn("WasteCost")
                .SetLabel("@DIR_AssessmentManager.CostOfWasteGridColumn_Cost")
                .SetTargetProperties(new string[] { "WasteCostFormatted" })
                .SetType("Decimal")
                .SetWidth(175);

            config.HtmlGridData.WithColumn("Percentage")
                .SetLabel("@DIR_AssessmentManager.CostOfWasteGridColumn_Percentage")
                .SetTargetProperties(new string[] { "PercentageFormatted" })
                .SetType("Decimal")
                .SetWidth(175);

            config.HtmlGridUIs.GetOrCreateHtmlGridUI().WithColumn("IdentifiableString");
            config.HtmlGridUIs.GetOrCreateHtmlGridUI().WithColumn("Title");
            config.HtmlGridUIs.GetOrCreateHtmlGridUI().WithColumn("WasteCost");
            config.HtmlGridUIs.GetOrCreateHtmlGridUI().WithColumn("Percentage");

            return config;
        }

        protected override IEnumerable<(Version Version, Func<HtmlGridConfiguration, HtmlGridConfiguration>)> ProvideConfigurationVersionHandlers()
        {
            return new List<(Version Version, Func<HtmlGridConfiguration, HtmlGridConfiguration>)>
            {
                (new Version(3, 9, 0), Version_3_9_0)
            };
        }
    }
}
