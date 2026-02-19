using Selerant.DevEx.Configuration.Infrastructure;
using Selerant.DevEx.Configuration.Navigator;
using Selerant.DevEx.Configuration.Navigator.DTOs;
using Selerant.DevEx.Configuration.Navigator.TabStrip.DTOs;
using Selerant.DevEx.Modules.DIRModule.Configuration.Navigation;
using Selerant.DevEx.Modules.DIRModule.Configuration.Resources;
using Selerant.DevEx.Modules.DIRModule.ModuleBlocks._ThisProject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Selerant.DevEx.Modules.DIRModule.Configuration.Searches.Assessment
{
    internal class AssessmentSearchTabStripOverlayModuleRepository : BaseConfigurationRepository<TabStripConfigurationOverlay>, ITabStripOverlayModuleRepository
    {
        public AssessmentSearchTabStripOverlayModuleRepository()
        {
        }

        protected override IEnumerable<(Version Version, Func<TabStripConfigurationOverlay, TabStripConfigurationOverlay>)> ProvideConfigurationVersionHandlers()
        {
            return new List<(Version, Func<TabStripConfigurationOverlay, TabStripConfigurationOverlay>)>
            {
                (new Version(3, 9, 0), Version_3_9_0)
            };
        }

        private TabStripConfigurationOverlay Version_3_9_0(TabStripConfigurationOverlay overlay)
        {
            overlay.WithControl(TabStripPanelContainer.AsCoreContainer(AssessmentSearchPanelNames.PANEL_AssessmentBasicPanel, Locale.GetString(ResourceFiles.AssessmentManager, "AssessmentBasicTab"))
                .AddCorePanel(TabStripPanel.AsCoreMvcPanel(AssessmentSearchPanelNames.PANEL_AssessmentBasicHolder, Locale.GetString(ResourceFiles.AssessmentManager, "AssessmentBasicTab"), "~//WebMvcModules//Infrastructure//Searches//Views//BaseCompositeSearch.cshtml")
                    .SetUseMvc(true)
                    .SetLoadOnDemand(EnumLoadOnDemandMode.Always)
                    .AddChildCorePanel(TabStripPanel.AsCoreMvcChildPanel(AssessmentSearchPanelNames.PANEL_PrimaryCriteria, "~//Modules//DIRModule//Areas//AssessmentSearch//Views//BasicSearch//Index.cshtml")
                        .SetUseMvc(true)
                        .SetLoadOnDemand(EnumLoadOnDemandMode.Always)))
                .AddCorePanel(TabStripPanel.AsCoreMvcPanel(AssessmentSearchPanelNames.PANEL_AssessmentRecent, Locale.GetString(ResourceFiles.AssessmentManager, "AssessmentRecentTab"), "~//WebMvcModules//Infrastructure//Searches//Views//RecentSearchPanel//Index.cshtml")
                    .SetUseMvc(true)
                    .SetLoadOnDemand(EnumLoadOnDemandMode.Always))
                    );

            overlay.LayoutChanges.Add(LayoutChange.Add(AffectedScopeNames.ASSESSMENT, OverlayControlReference.Of(AssessmentSearchPanelNames.PANEL_AssessmentBasicPanel)));
            //overlay.LayoutChanges.Add(LayoutChange.Add(bussinesIds, OverlayControlReference.Of("CustomQueriesPanels")));

            return overlay;
        }
    }
}
