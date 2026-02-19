using Selerant.DevEx.Configuration.Grids.HtmlGrid;
using Selerant.DevEx.Configuration.Grids.HtmlGrid.DTOs;
using Selerant.DevEx.Configuration.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Selerant.DevEx.Modules.DIRModule.Configuration.Grids
{
    public class DIRModule_General_GridConfigurationRepository : BaseConfigurationRepository<HtmlGridConfiguration>, ICoreHtmlGridConfigurationRepository
    {
        public string GridName => GridNames.GENERAL;

        private HtmlGridConfiguration Version_39(HtmlGridConfiguration config)
        {
            config.HtmlGridData.WithColumn("Title").SetPrimary(true).SetRequired(true)
                .SetLabel("@DIR_AssessmentManager.ANNNavTabGeneral")
                .SetTargetProperties("Title")
                .SetType("String");

            config.HtmlGridData.WithColumn("Value").SetRequired(true)
                .SetTargetProperties("Value")
                .SetType("String");

            config.HtmlGridUIs.GetOrCreateHtmlGridUI().WithColumn("Title");
            config.HtmlGridUIs.GetOrCreateHtmlGridUI().WithColumn("Value");

            return config;
        }

        protected override IEnumerable<(Version Version, Func<HtmlGridConfiguration, HtmlGridConfiguration>)> ProvideConfigurationVersionHandlers()
        {
            return new List<(Version Version, Func<HtmlGridConfiguration, HtmlGridConfiguration>)>
            {
                (new Version(3, 9), Version_39)
            };
        }
    }
}
