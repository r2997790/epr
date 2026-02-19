using Selerant.DevEx.Modules.DIRModule.Configuration.HostMenu;
using Selerant.DevEx.Modules.DIRModule.ModuleBlocks._ThisProject;
using Selerant.DevEx.Web;

namespace Selerant.DevEx.Modules.DIRModule.WebMvcFrontEnd._ThisProject
{
	public class DIRModuleComponentIdentifier
	{
		[LogicComponentIdentifier]
		public const string ASSESSMENT_NAVIGATOR = DIRModuleInfo.MODULE_NAME + "_AssessmentNavigator";
		[LogicComponentIdentifier]
		public const string SEARCH_ASSESSMENT = DIRHostMenuRegistry.SEARCH_ASSESSMENT_ID;
		[LogicComponentIdentifier]
		public const string CREATE_ASSESSMENT = DIRHostMenuRegistry.CREATE_ASSESSMENT_ID;
        [LogicComponentIdentifier]
        public const string ASSESSMENT_DASHBOARD = DIRModuleInfo.MODULE_NAME + "_AssessmentDashboard";

        /// <summary>
        /// General Panel - Component identifier
        /// </summary>
        [LogicComponentIdentifier]
		public const string ASSESSMENT_NAVIGATOR_GENERAL_PANEL = DIRModuleInfo.MODULE_NAME + "_AssessmentNavigator_GeneralPanel";
		/// <summary>
		/// ResourceManagement Panel - Component identifier
		/// </summary>
		[LogicComponentIdentifier]
		public const string ASSESSMENT_NAVIGATOR_RESOURCE_MANAGEMENT_PANEL = DIRModuleInfo.MODULE_NAME + "_AssessmentNavigator_ResourceManagement";
		/// <summary>
		/// Assessment Results Panel - Component identifier
		/// </summary>
		[LogicComponentIdentifier]
		public const string ASSESSMENT_NAVIGATOR_ASSESSMENT_RESULTS_PANEL = DIRModuleInfo.MODULE_NAME + "_AssessmentNavigator_AssessmentResults";
	}
}