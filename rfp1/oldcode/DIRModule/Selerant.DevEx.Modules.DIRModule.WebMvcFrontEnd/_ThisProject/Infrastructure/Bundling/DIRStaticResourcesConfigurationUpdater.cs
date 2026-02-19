using Selerant.DevEx.Configuration.StaticResources;
using Selerant.DevEx.Modules.DIRModule.ModuleBlocks._ThisProject;

namespace Selerant.DevEx.Modules.DIRModule.WebMvcFrontEnd._ThisProject.Infrastructure.Bundling
{
	internal class DIRStaticResourcesConfigurationUpdater : IStaticResourcesConfigurationUpdater
	{
		public const string ASSESSMENT_NAVIGATOR_BUNDLE_NAME = DIRModuleInfo.MODULE_NAME + "_AssessmentNavigator_Bundle";
        public const string ASSESSMENT_SEARCH_BUNDLE_NAME = DIRModuleInfo.MODULE_NAME + "_AssessmentSearch_Bundle";


        public string ProvidedBy
		{
			get { return DIRModuleInfo.Instance.ModuleName; }
		}

		public void Update(IStaticResourcesConfiguration configuration)
		{
			configuration
				.WithBundle(ASSESSMENT_NAVIGATOR_BUNDLE_NAME)
				.AddJSFile(DIRLinks.Areas.AssessmentNavigator.Views.Home.Index_ts)
				.AddCSSFile(DIRLinks.Areas.AssessmentNavigator.Views.Home.Index_css)
				;

            configuration
                .WithBundle(ASSESSMENT_SEARCH_BUNDLE_NAME)
                .AddJSFile(DIRLinks.Areas.AssessmentSearch.Views.Home.AssessmentIndexModel_ts);
		}
	}
}