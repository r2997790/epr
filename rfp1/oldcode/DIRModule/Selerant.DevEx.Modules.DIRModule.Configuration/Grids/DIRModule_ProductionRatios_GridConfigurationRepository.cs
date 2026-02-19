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
    internal class DIRModule_ProductionRatios_GridConfigurationRepository : DIRModule_Results_GridConfigurationRepository, ICoreHtmlGridConfigurationRepository
    {
        public string GridName => GridNames.PRODUCTION_RATIOS;

        protected override IEnumerable<(Version Version, Func<HtmlGridConfiguration, HtmlGridConfiguration>)> ProvideConfigurationVersionHandlers()
        {
            return base.ProvideConfigurationVersionHandlers();
        }
    }
}
