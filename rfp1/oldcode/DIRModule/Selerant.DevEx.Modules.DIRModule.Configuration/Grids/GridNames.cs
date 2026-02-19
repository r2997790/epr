using Selerant.DevEx.Modules.DIRModule.ModuleBlocks._ThisProject;

namespace Selerant.DevEx.Modules.DIRModule.Configuration.Grids
{
    public static class GridNames
    {
        public const string GENERAL = DIRModuleInfo.MODULE_NAME + "_General";

		#region Resource Managmenet Grid IDs

        public const string INPUTS = DIRModuleInfo.MODULE_NAME + "_InputsTabGrid";
		public const string DESTINATIONS = DIRModuleInfo.MODULE_NAME + "_DestinationsTabGrid";
		public const string NON_FOOD_DESTINATIONS = DIRModuleInfo.MODULE_NAME + "_NonFoodDestinationsTabGrid";
        public const string OUTPUTS = DIRModuleInfo.MODULE_NAME + "_OutputsTabGrid";
		public const string BUSINESSCOSTS = DIRModuleInfo.MODULE_NAME + "_BusinessCostsTabGrid";

		#endregion

		public const string RESULTS = DIRModuleInfo.MODULE_NAME + "_Results";
        public const string PRODUCTION_RATIOS = DIRModuleInfo.MODULE_NAME + "_ProductionRatios";
        public const string KEY_FINANTIAL_INDICATORS = DIRModuleInfo.MODULE_NAME + "_KeyFinantialIndicators";
        public const string FOOD_WASTE_PER_DESTINATION = DIRModuleInfo.MODULE_NAME + "_FoodWastePerDestination";
        public const string COST_OF_WASTE = DIRModuleInfo.MODULE_NAME + "_CostOfWaste";

        private const string DASHBOARD_SUFFIX = "_Dashboard";

        public const string HOT_SPOT_DASHBOARD = DIRModuleInfo.MODULE_NAME + "_HotSpot" + DASHBOARD_SUFFIX;
        public const string PRODUCTION_RATIOS_DASHBOARD = PRODUCTION_RATIOS + DASHBOARD_SUFFIX;
        public const string KEY_FINANTIAL_INDICATORS_DASHBOARD = KEY_FINANTIAL_INDICATORS + DASHBOARD_SUFFIX;
        public const string COST_OF_WASTE_DASHBOARD = COST_OF_WASTE + DASHBOARD_SUFFIX;
        public const string FOOD_WASTE_PER_DESTINATION_DASHBOARD = FOOD_WASTE_PER_DESTINATION + DASHBOARD_SUFFIX;

        public const string ADMIN_TOOLS_ASSESSMENT_TYPES = DIRModuleInfo.MODULE_NAME + "_AdminToolsAssessmentTypes";
        public const string ASSESSMENT_SEARCH_RECENT = DIRModuleInfo.MODULE_NAME + "_AssessmentSearch_Recent";
        public const string ASSESSMENT_SEARCH_RESULT = DIRModuleInfo.MODULE_NAME + "_AssessmentSearch_Result";
    }
}
