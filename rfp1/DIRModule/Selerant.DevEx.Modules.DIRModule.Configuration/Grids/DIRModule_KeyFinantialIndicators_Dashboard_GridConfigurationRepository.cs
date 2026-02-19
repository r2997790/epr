using Selerant.DevEx.Configuration.Grids.HtmlGrid;
using Selerant.DevEx.Configuration.Grids.HtmlGrid.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Selerant.DevEx.Modules.DIRModule.Configuration.Grids
{
    internal class DIRModule_KeyFinantialIndicators_Dashboard_GridConfigurationRepository : DIRModule_AssessmentDashboardResults_GridConfigurationResository, ICoreHtmlGridConfigurationRepository
    {
        public string GridName => GridNames.KEY_FINANTIAL_INDICATORS_DASHBOARD;

        protected override IEnumerable<(Version Version, Func<HtmlGridConfiguration, HtmlGridConfiguration>)> ProvideConfigurationVersionHandlers()
        {
            return base.ProvideConfigurationVersionHandlers();
        }
    }
}
