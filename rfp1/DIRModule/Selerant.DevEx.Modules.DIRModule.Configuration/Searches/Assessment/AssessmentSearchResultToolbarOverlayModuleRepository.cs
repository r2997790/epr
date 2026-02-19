using Selerant.DevEx.Configuration.Infrastructure;
using Selerant.DevEx.Configuration.Navigator.DTOs;
using Selerant.DevEx.Configuration.Navigator.ToolBar;
using Selerant.DevEx.Configuration.Navigator.ToolBar.DTOs;
using Selerant.DevEx.Modules.DIRModule.Configuration.Navigation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Selerant.DevEx.Modules.DIRModule.Configuration.Searches.Assessment
{
    internal class AssessmentSearchResultToolbarOverlayModuleRepository : BaseConfigurationRepository<ToolBarConfigurationOverlay>, IToolBarOverlayModuleRepository
    {
        public AssessmentSearchResultToolbarOverlayModuleRepository()
        {
        }

        protected override IEnumerable<(Version Version, Func<ToolBarConfigurationOverlay, ToolBarConfigurationOverlay>)> ProvideConfigurationVersionHandlers()
        {
            return new List<(Version, Func<ToolBarConfigurationOverlay, ToolBarConfigurationOverlay>)>
            {
                (new Version(3, 9, 0), Version_3_9_0)
            };
        }

        private ToolBarConfigurationOverlay Version_3_9_0(ToolBarConfigurationOverlay overlay)
        {
            var resultOverlayConfig = new ToolBarConfigurationOverlay();

            resultOverlayConfig.LayoutChanges.Add(LayoutChange.Add(AffectedScopeNames.ASSESSMENT, OverlayControlReference.Of("SqlButton")));
            resultOverlayConfig.LayoutChanges.Add(LayoutChange.Add(AffectedScopeNames.ASSESSMENT, OverlayControlReference.Of("Filter")));
            resultOverlayConfig.LayoutChanges.Add(LayoutChange.Add(AffectedScopeNames.ASSESSMENT, OverlayControlReference.Of("NumberOfRowsLabel")));

            return resultOverlayConfig;
        }
    }
}
