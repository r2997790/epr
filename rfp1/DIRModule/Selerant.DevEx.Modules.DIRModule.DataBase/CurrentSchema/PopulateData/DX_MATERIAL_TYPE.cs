using System;
using Selerant.ApplicationBlocks.Data.Migrations;

namespace Selerant.DevEx.Modules.DIRModule.DataBase.CurrentSchema.PopulateData
{
	public class DX_MATERIAL_TYPE : MigrationBase
	{
		public override void Up()
		{
			Database.Data.InsertInto("DX_MATERIAL_TYPE")
				.Columns("CODE", "CHILD_TYPE", "FORMULA_TYPES", "FORMULA_VER", "FORMULA_ALT", "ALWAYS_AUTO_GENERATION", "DEFAULT_UDF", "FLAGS", "DEFAULT_FORMULA_TYPE", "CATEGORY", "COPY_FORMULATIONS_TO_CHILD", "NEW_SPEC_LINKED_ONLY_TYPES", "NEW_SPEC_COUNT_DEVELOP", "NEW_SPEC_ALLOW_COPY")
				.Values("DIR_RESOURCE", null, null, 0, 0, 1, null, 0, null, "FORMULA", 0, 0, 0, 0);

			/*Database.Data.InsertInto("DX_MATERIAL_TYPE")
					.Columns("CODE", "CHILD_TYPE", "FORMULA_TYPES", "FORMULA_VER", "FORMULA_ALT", "ALWAYS_AUTO_GENERATION", "DEFAULT_UDF", "FLAGS", "DEFAULT_FORMULA_TYPE", "CATEGORY", "COPY_FORMULATIONS_TO_CHILD", "NEW_SPEC_LINKED_ONLY_TYPES", "NEW_SPEC_COUNT_DEVELOP", "NEW_SPEC_ALLOW_COPY", "FORMULA_MODE")
					.Values("DIR_INPUT", null, null, 0, 0, 1, null, 0, null, "FORMULA", 0, 0, 0, 0, "FORMULA");

			Database.Data.InsertInto("DX_MATERIAL_TYPE")
					.Columns("CODE", "CHILD_TYPE", "FORMULA_TYPES", "FORMULA_VER", "FORMULA_ALT", "ALWAYS_AUTO_GENERATION", "DEFAULT_UDF", "FLAGS", "DEFAULT_FORMULA_TYPE", "CATEGORY", "COPY_FORMULATIONS_TO_CHILD", "NEW_SPEC_LINKED_ONLY_TYPES", "NEW_SPEC_COUNT_DEVELOP", "NEW_SPEC_ALLOW_COPY", "FORMULA_MODE")
					.Values("DIR_OUTPUT", null, null, 0, 0, 1, null, 0, null, "FORMULA", 0, 0, 0, 0, "FORMULA");*/
		}

		public override void Down()
		{
			throw new NotImplementedException();
		}
	}
}