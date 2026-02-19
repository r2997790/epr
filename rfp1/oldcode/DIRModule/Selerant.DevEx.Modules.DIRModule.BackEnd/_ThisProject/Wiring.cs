using Autofac;
using Selerant.ApplicationBlocks.PathManagement;
using Selerant.DevEx.BusinessLayer;
using Selerant.DevEx.BusinessLayer.AttributeManagement;
using Selerant.DevEx.BusinessLayer.IconManagement;
using Selerant.DevEx.Modules.DIRModule.BackEnd.Configuration.NavigatorFunctionalBlock.Assessment;
using Selerant.DevEx.Modules.DIRModule.ModuleBlocks._ThisProject;
using Selerant.DevEx.Modules.DIRModule.BackEnd._ThisProject.Infrastructure;
using Selerant.Infrastructure.DependencyContainer;
using Selerant.DevEx.Modules.DIRModule.BackEnd.Searches;
using Selerant.DevEx.BusinessLayer.BusinessObjects.Recent;
using Selerant.DevEx.Modules.DIRModule.BackEnd._ThisProject.Infrastructure.AttributesProvider;
using Selerant.DevEx.Modules.DIRModule.BackEnd.IconsManagement;

namespace Selerant.DevEx.Modules.DIRModule.BackEnd._ThisProject
{
    public class Wiring : DIRWiring,
		INeedToHandleAttributeScopes,
		INeedToLoadNavigatorsFunctionalBlocks,
		INeedToProvideIcons
	{
		public string ModuleName => ThisAssembly.GetName().Name;

		protected override void Load(SelerantContainerBuilder selerantBuilder)
		{
			var builder = selerantBuilder.AutofacContainerBuilder;
			((INeedToHandleAttributeScopes)this).LoadAttributeScopeProvider(builder);
			((INeedToHandleAttributeScopes)this).LoadAttributeElementProvider(builder);

            LoadNavigatorsFunctionalBlocks(builder);
            LoadSearchesFunctionalBlocks(builder);

            builder.RegisterType<CustomizedGridRepository>().As<ICustomizedGridRepository>();
            builder.RegisterType<RecentObjectProvider>().As<IRecentObjectProvider>().SingleInstance();

            // register AutoNumbering script file path provider (script will be located in module WF)
            builder.RegisterType<DIRAutoNumberingFileProvider>().As<IAutoNumberingFileProvider>();

			// Load specific icon provider
			LoadIconsProviders(builder);
		}

		public void LoadAttributeScopeProvider(ContainerBuilder builder)
		{
			builder.RegisterType<DIRAttributeScopesProvider>().As<IAttributeScopeProvider>();
		}

		public void LoadAttributeElementProvider(ContainerBuilder builder)
		{
			builder.RegisterType<DIRAttributeElementProvider>().As<IAttributeElementProvider>();
		}

		/// <summary>
		/// Registers navigators functional blocks, and theirs node(panels) functional blocks (also take a look in DX_COMPONENT_CONTROLLER)
		/// </summary>
		/// <param name="builder"></param>
		public void LoadNavigatorsFunctionalBlocks(ContainerBuilder builder)
		{
			builder.Register<INavigatorsFunctionalBlocksUpdater>(x => new AssessmentFunctionalBlocksUpdater());
		}

        void LoadSearchesFunctionalBlocks(ContainerBuilder builder)
        {
            builder.RegisterType<DxAssessmentSearchesFunctionalBlockUpdater>().As<ISearchesFunctionalBlocksUpdater>();
        }

		public void LoadIconsProviders(ContainerBuilder builder)
		{
			builder.Register(context => new DIRGenericIconsProvider(PathFinder.Instance)).As<IGenericIconsProvider>();
		}
	}
}