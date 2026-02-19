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
    public class DIRModule_Outputs_GridConfigurationRepository : BaseConfigurationRepository<HtmlGridConfiguration>, ICoreHtmlGridConfigurationRepository
    {
        public string GridName => GridNames.OUTPUTS;

        private HtmlGridConfiguration Version_39(HtmlGridConfiguration config)
        {
            config.HtmlGridData.WithColumn("IdentifiableString").SetPrimary(true).SetRequired(true).SetExportHidden(true).SetFrozen(true).SetHidden(true)
                .SetWidth(40)
                .SetLabel("IdentifiableString")
                .SetTargetProperties(new string[] { "IdentifiableString" })
                .SetType(nameof(String));

            config.HtmlGridData.WithColumn("Category").SetRequired(true)
                  .SetWidth(20)
                  .SetLabel("@DIR_AssessmentManager.OutputsGridColumn_Destination")
                  .SetTargetProperties(new string[] { "GridItemTitle" })
                  .SetType(nameof(String));

            config.HtmlGridData.WithColumn("OutputCost")
                  .SetWidth(20)
                  .SetConverter($"{DIRModuleInfo.Instance.ModuleName}.ClientOutputsIsEditableValueConverter")
                  .SetLabel("@DIR_AssessmentManager.OutputsGridColumn_OutputCost")
                  .SetTargetProperties(new string[] { "OutputCostFormatted", "OutputCost", "IsOutputCostEditable" })
                  .SetType(nameof(Decimal));

            config.HtmlGridData.WithColumn("Income")
                  .SetWidth(20)
                  .SetConverter($"{DIRModuleInfo.Instance.ModuleName}.ClientOutputsIsEditableValueConverter")
                  .SetLabel("@DIR_AssessmentManager.OutputsGridColumn_Income")
                  .SetTargetProperties(new string[] { "IncomeFormatted", "Income", "IsIncomeEditable" })
                  .SetType(nameof(Decimal));

            config.HtmlGridData.WithColumn("Actions").SetRequired(true).SetExportHidden(true).SetFrozen(true)
                  .SetWidth(20)
                  .SetConverter($"{DIRModuleInfo.Instance.ModuleName}.ResourceEntityOutputsActionConverter")
                  .SetLabel("@DIR_Controls.GridColumn_Actions")
                  .SetTargetProperties(new string[] { "IdentifiableString", "DestinationCode" })
                  .SetType(nameof(String));

            config.HtmlGridUIs.GetOrCreateHtmlGridUI().SetOrderBy("IdentifiableString asc");

            config.HtmlGridUIs.GetOrCreateHtmlGridUI().WithColumn("IdentifiableString");
            config.HtmlGridUIs.GetOrCreateHtmlGridUI().WithColumn("Category");
            config.HtmlGridUIs.GetOrCreateHtmlGridUI().WithColumn("OutputCost");
            config.HtmlGridUIs.GetOrCreateHtmlGridUI().WithColumn("Income");
            config.HtmlGridUIs.GetOrCreateHtmlGridUI().WithColumn("Actions");

            return config;
        }
        
        protected override IEnumerable<(Version Version, Func<HtmlGridConfiguration, HtmlGridConfiguration>)> ProvideConfigurationVersionHandlers()
        {
            yield return (new Version(3, 9), Version_39);
        }
    }
}
