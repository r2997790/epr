using Autofac;
using Selerant.DevEx.BusinessLayer;
using Selerant.DevEx.Configuration.StaticResources;
using Selerant.DevEx.Infrastructure.ComponentsData;
using Selerant.DevEx.Infrastructure.Modules.AdminTools.Home;
using Selerant.DevEx.Modules.DIRModule.BackEnd.AssessmentManagement.BusinessObjects;
using Selerant.DevEx.Modules.DIRModule.ModuleBlocks._ThisProject;
using Selerant.DevEx.Modules.DIRModule.WebMvcFrontEnd._ThisProject.AdminTools;
using Selerant.DevEx.Modules.DIRModule.WebMvcFrontEnd._ThisProject.Infrastructure;
using Selerant.DevEx.Modules.DIRModule.WebMvcFrontEnd._ThisProject.Infrastructure.Bundling;
using Selerant.DevEx.Modules.DIRModule.WebMvcFrontEnd._ThisProject.Infrastructure.HostMenu;
using Selerant.DevEx.Modules.DIRModule.WebMvcFrontEnd._ThisProject.Infrastructure.Navigator;
using Selerant.DevEx.Modules.DIRModule.WebMvcFrontEnd._ThisProject.Wirings;
using Selerant.DevEx.Modules.DIRModule.WebMvcFrontEnd.Helpers.BreadCrumb;
using Selerant.DevEx.WebMvcModules.Helpers.Factory;
using Selerant.DevEx.WebMvcModules.Infrastructure.Resources.Text;
using Selerant.DevEx.WebMvcModules.Infrastructure.Searches.Helpers;
using Selerant.DevEx.WebPages;
using Selerant.Infrastructure.DependencyContainer;

namespace Selerant.DevEx.Modules.DIRModule.WebMvcFrontEnd._ThisProject
{
    public class Wiring : DIRWiring,
		INeedToLoadSecurity,
		INeedToLoadLogicalComponents,
		INeedToLoadAdminTools
	{
		protected override void Load(SelerantContainerBuilder selerantBuilder)
		{
			var builder = selerantBuilder.AutofacContainerBuilder;

			builder.RegisterType<ComponentDescriptorExtensionProvider>().As<IComponentDescriptorExtensionProvider>();

			LoadModuleNavigatorControllerTypeProviders(builder);
			LoadSecurity(builder);
			LoadLogicComponents(builder);
			LoadStaticResourcesService(builder);
			LoadMenuCommands(builder);
            LoadBreadCrumbs(builder);
			LoadAdministratorTools(builder);
			LoadSearches(builder);
            LoadCommandResolvers(builder);

            // resource files
            builder.RegisterType<Infrastructure.Resources.DIRResourceFileProvider>().As<IResourceFileProvider>();
        }

        private void LoadBreadCrumbs(ContainerBuilder builder)
        {
            BreadCrumbFactory.RegisterBreadCrumb<AssessmentBreadCrumb, DxAssessment>(builder);
        }

		public void LoadModuleNavigatorControllerTypeProviders(ContainerBuilder builder)
		{
			builder.RegisterType<DIRModuleNavigatorControllerTypeProvider>().As<INavigatorControllerTypeProvider>();
		}

		/// <summary>
		/// Registers inside container all the stuffs related to security
		/// </summary>
		public void LoadSecurity(ContainerBuilder builder)
		{
			new WiringSecurity().Wiring(builder);
		}

		/// <summary>
		/// Registers inside containre the needed component data
		/// </summary>
		public void LoadLogicComponents(ContainerBuilder builder)
		{
			// Registering logic components for this assembly
			builder.Register<ILogicComponentInfoFinder>(x => new LogicComponentInfoFinder(ThisAssembly, DIRModuleInfo.Instance.ModuleName, type => type.Name.StartsWith("T4MVC")));
		}

		/// <summary>
		/// Registers the proper service for static resources, to be injected in Bundle engine
		/// </summary>
		public void LoadStaticResourcesService(ContainerBuilder builder)
		{
			builder.RegisterType<DIRStaticResourcesConfigurationUpdater>().As<IStaticResourcesConfigurationUpdater>();
		}

		/// <summary>
		/// Registers url providers (for like main menu link)
		/// </summary>
		/// <param name="builder"></param>
		private void LoadMenuCommands(ContainerBuilder builder)
		{
			new WiringNavigatorCommands().Wiring(builder);
		}

        /// <summary>
        /// This interface is a marker to register admin tools inside the container
        /// </summary>
        public void LoadAdministratorTools(ContainerBuilder builder)
        {
            builder.RegisterType<DIRAdminToolsRegistry>().As<IRegisterAdminToolsButtons>();
        }

        private void LoadCommandResolvers(ContainerBuilder builder)
        {
            new WiringCommandResolvers().Wiring(builder);
        }

        public void LoadSearches(ContainerBuilder builder)
        {
            new SearchStaticUIDataWirer(ThisAssembly).Wire(builder);
        }
    }
}