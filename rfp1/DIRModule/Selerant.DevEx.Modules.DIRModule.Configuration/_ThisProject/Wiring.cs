using Autofac;
using Selerant.ApplicationBlocks.PathManagement.Interfaces;
using Selerant.ApplicationBlocks.PathManagement.Wiring;
using Selerant.DevEx.Modules.DIRModule.Configuration.PathManagement;
using Selerant.DevEx.Configuration.Grids.HtmlGrid;
using Selerant.DevEx.Modules.DIRModule.ModuleBlocks._ThisProject;
using Selerant.Infrastructure.DependencyContainer;
using Selerant.DevEx.Modules.DIRModule.Configuration.Searches.Assessment;
using Selerant.DevEx.Configuration.Search;
using Selerant.DevEx.Configuration.Search.ToolBar;
using Selerant.DevEx.Modules.DIRModule.Configuration.HostMenu;
using Selerant.DevEx.BusinessLayer;

namespace Selerant.DevEx.Modules.DIRModule.Configuration._ThisProject
{
    /// <summary>
    /// Wiring component for entire assembly, to fill properly the container
    /// </summary>
    public class Wiring : DIRWiring,
		ICanProvideResourcePaths
	{
		/// <summary>
		/// Loads dependencies and fills the container
		/// </summary>
		/// <param name="builder"></param>
		protected override void Load(SelerantContainerBuilder selerantBuilder)
		{
			var builder = selerantBuilder.AutofacContainerBuilder;

			//((INeedToMapEntities)this).LoadEntityFinders(builder);

            // Register grids configuration
            builder.RegisterType<Grids.ModuleHtmlGridProvider>().As<INeedToAddHtmlGrid>();

			((ICanProvideResourcePaths)this).LoadPathProviders(builder);

			LoadHeaderMenu(builder);

			// Register searches
			builder.RegisterType<AssessmentSearchChangesOverlayProvider>().As<INeedToChangeSearch>();
            builder.RegisterType<AssessmentSearchChangesOverlayProvider>().As<INeedToChangeSearchResultToolBar>();
        }

        /// <summary>
        /// Loads dependencies and fills the container
        /// </summary>
        /// <param name="builder"></param>
        public void LoadPathProviders(ContainerBuilder containerBuilder)
		{
			containerBuilder
				.RegisterType<DIRResourcePathProvider>()
				.As<IResourcePathProvider>()
                .SingleInstance();
		}

		/// <summary>
		/// Registers Main Header Menu (Links group and dropdown links)
		/// </summary>
		/// <param name="builder"></param>
		public void LoadHeaderMenu(ContainerBuilder builder)
		{
			builder.Register<IHostMenuRegistry>(x => new DIRHostMenuRegistry());
		}
	}
}
