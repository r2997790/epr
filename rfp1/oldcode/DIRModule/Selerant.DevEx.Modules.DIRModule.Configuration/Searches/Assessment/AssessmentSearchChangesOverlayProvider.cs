using Selerant.DevEx.Configuration.Infrastructure.TreeLayerServices;
using Selerant.DevEx.Configuration.Navigator.TabStrip.DTOs;
using Selerant.DevEx.Configuration.Navigator.ToolBar.DTOs;
using Selerant.DevEx.Configuration.Search;
using Selerant.DevEx.Configuration.Search.ToolBar;
using Selerant.DevEx.Modules.DIRModule.ModuleBlocks._ThisProject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Selerant.DevEx.Modules.DIRModule.Configuration.Searches.Assessment
{
    public class AssessmentSearchChangesOverlayProvider : INeedToChangeSearch, INeedToChangeSearchResultToolBar
    {
        SearchModuleChangeVO INeedToChangeConfig<SearchModuleChangeVO, TabStripConfigurationOverlay>.ProvideDefinition()
        {
            return new SearchModuleChangeVO(DIRModuleInfo.MODULE_NAME, SearchNames.BASIC_SEARCH, new AssessmentSearchTabStripOverlayModuleRepository());
        }

        SearchResultToolBarModuleChangeVO INeedToChangeConfig<SearchResultToolBarModuleChangeVO, ToolBarConfigurationOverlay>.ProvideDefinition()
        {
            return new SearchResultToolBarModuleChangeVO(SearchResultToolBarNames.SEARCH_RESULT_TOOLBAR, DIRModuleInfo.MODULE_NAME, new AssessmentSearchResultToolbarOverlayModuleRepository());
        }
    }
}
