namespace Selerant.DevEx.Modules.DIRModule.BackEnd.Common
{
	public class Constants
	{
		public class MaterialType
		{
			public const string DIR_RESOURCE = "DIR_RESOURCE";
		}

		public class ResourceMaterial
		{
			/// <summary>
			/// Direct FOOD type WATER material code
			/// </summary>
			public const string DIR_FOOD_WATER = "DIR_FOOD_WATER";

			/// <summary>
			/// Direct NONFOOD type WATER material code
			/// </summary>
			public const string DIR_NONFOOD_WATER = "DIR_NONFOOD_WATER";
		}

		/// <summary>
		/// Used on: 
		/// Attribute[DXDIR_RESOURCE_TYPE] = 99510
		/// DXDIR_INPUT_CATEGORY.TYPE
		/// </summary>
		public class InputType
		{
			public const string FOOD = "FOOD";
			public const string NONFOOD = "NONFOOD";
		}

		/// <summary>
		/// Used on: 
		/// DXDIR_OUTPUT_CATEGORY.TYPE - (WHERE ID in (4, 5, 6))
		/// </summary>
		public class OutputType
		{
			public const decimal FOOD_ID = 4;
			public const string FOOD = "FOOD";

			public const decimal INEDIBLE_ID = 5;
			public const string INEDIBLE = "INEDIBLE";

			public const decimal NON_FOOD_ID = 5;
			public const string NON_FOOD = "NON_FOOD";

			public const decimal WASTEWATER_ID = 7;
		}

		/// <summary>
		/// Special Bussiness Cost rows codes/phrases
		/// </summary>
		public class BussinessCostPhrase
		{
			public const string WASTE = "WASTE";
			public const string MATLOSS = "MATLOSS";
		}

		public class AssessmentFB
		{
			/// <summary>
			/// Assessment Search
			/// </summary>
			public const string ASSESSMENT_SEARCH = "DIR-01";

			/// <summary>
			/// Assessment Search: Custom Query
			/// </summary>
			public const string ASMT_SEARCH_CUSTOM_QUERY = "DIR-01A";

			/// <summary>
			/// Assessment Search Result: Export
			/// </summary>
			public const string ASMT_SEARCH_RESULT_EXPORT = "DIR-02";

			/// <summary>
			/// New Assessment: Entity Creation
			/// </summary>
			public const string NEW_ASSESSMENT_CREATION_DIR_03 = "DIR-03";
		}

		public class AssessmentNavigatorFB
		{
			/// <summary>
			/// Assessment Navigator - Main Functional Block
			/// </summary>
			public const string ASSESSMENT_NAVIGATOR = "DIR-04";

			public const string ASSESSMENT_GENERAL_TAB = "DIR-04A";

			public const string ASSESSMENT_RESOURCE_MANAGEMENT_TAB = "DIR-04B";

			public const string ASSESSMENT_RESULT_TAB = "DIR-04C";

			public const string ASSESSMENT_NAVIGATOR_EXTRA_TABS = "DIR-04X";

			public const string ASSESSMENT_NAVIGATOR_TOOLBAR_BTNS = "DIR-04TX";

			public const string ASSESSMENT_NAVIGATOR_MATERIAL_CREATION = "M1-05";

			public const string ASSESSMENT_NAVIGATOR_SHARE = "DIR-04S";
		}

		public class AssessmentAdminTools
		{
			/// <summary>
			/// Administarive Tools: Assessment Types
			/// </summary>
			public const string ADMIN_TOOLS_ASSESSMENT_TYPES = "DIR-05";
		}
	}
}
