using Selerant.ApplicationBlocks;
using Selerant.ApplicationBlocks.PathManagement;
using Selerant.DevEx.Configuration.Navigator.DialogTabStrip;
using Selerant.DevEx.Configuration.Navigator.TabStrip;
using Selerant.DevEx.Configuration.Navigator.ToolBar;
using Selerant.DevEx.Modules.DIRModule.ModuleBlocks._ThisProject;
using Selerant.Infrastructure.DependencyContainer;

namespace Selerant.DevEx.Modules.DIRModule.Configuration.Navigation.Assessment
{
    /// <summary>
    /// Provider of module overlay for Assessment tabstrip and toolbar configuration
    /// </summary>
    internal class AssessmentSettingsProvider : INeedToAddModuleTabStrip, INeedToAddModuleToolbar, INeedToAddDialogTabStrip //INeedToAddTabStrip, INeedToAddToolbar
    {
        //private readonly ITabStripCustomerOverlayRepository _tabStripCustomerOverlayRepository;
        //private readonly ITabStripUserOverlayRepository _tabStripUserOverlayRepository;

        /// <summary>
        /// <see cref="INeedToAddModuleTabStrip.ProvideDefinition"/>
        /// </summary>
        TabStripModuleDefinitionVO INeedToAddModuleTabStrip.ProvideDefinition()
        {
            var tabStripModuleDefinition = new TabStripModuleDefinitionVO("AssessmentTabStrip", DIRModuleInfo.Instance.ModuleName, new AssessmentTabStripOverlayModuleRepository());
            return tabStripModuleDefinition;
        }

        /// <summary>
        /// <see cref="INeedToAddModuleToolbar.ProvideDefinition"/>
        /// </summary>
        ToolbarModuleDefinitionVO INeedToAddModuleToolbar.ProvideDefinition()
        {
            var toolbarModuleDefinition = new ToolbarModuleDefinitionVO("AssessmentToolBar", DIRModuleInfo.Instance.ModuleName, new AssessmentToolBarOverlayModuleRepository());
            return toolbarModuleDefinition;
        }

        //TO DO: Currently Assessment object is overlayed from model as if it exists in core - change that behavior

        //      TabStripDefinitionVO INeedToAddTabStrip.ProvideDefinition()
        //      {
        //          var repository = new AssessmentTabStripCore();
        //          var vo = new TabStripDefinitionVO(repository.TabStripName, "ASSESSMENT", repository,
        //              _tabStripCustomerOverlayRepository, _tabStripUserOverlayRepository);

        //          return vo;
        //      }

        //      /// <summary>
        ///// <see cref="INeedToAddToolbar.ProvideDefinition"/>
        ///// </summary>
        //ToolbarDefinitionVO INeedToAddToolbar.ProvideDefinition()
        //      {
        //          var navigationFolder = PathFinder.Instance.GetFolderPath(PathFinder.FolderKeys.NavigatorCustomization);
        //          var repo = new AssessmentToolBarCore();
        //          var vo = new ToolbarDefinitionVO(repo.ToolBarName, "ASSESSMENT", repo, new YaxToolBarOverlayRepository(navigationFolder));

        //          return vo;
        //      }

        DialogTabStripDefinitionVO INeedToAddDialogTabStrip.ProvideDefinition()
        {
            var navigationFolder = PathFinder.Instance.GetFolderRelativePath(PathFinder.FolderKeys.NavigatorCustomization);
            var vo = new DialogTabStripDefinitionVO("AssessmentDialogTabStrip", AffectedScopeNames.ASSESSMENT,
                new AssessmentDialogTabStripCore(),
                new YaxLibDialogTabStripOverlayRepository(CrossCutting.Instance.FileStore, navigationFolder));

            return vo;
        }
    }
}
