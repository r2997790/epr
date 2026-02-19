using Selerant.ApplicationBlocks.Data.Migrations;
using System;

namespace Selerant.DevEx.Modules.DIRModule.DataBase.CurrentSchema.PopulateData
{
	class DXDIR_LC_STAGE_TMPL: MigrationBase
	{
		public override void Up()
		{
			Database.Data.InsertInto("DXDIR_LC_STAGE_TMPL")
				.Columns("ID", "ASSESSMENT_TYPE_CODE", "TITLE", "SORT_ORDER")
				.Values(1, "DEFAULT", "Primary Production", 1);

			Database.Data.InsertInto("DXDIR_LC_STAGE_TMPL")
				.Columns("ID", "ASSESSMENT_TYPE_CODE", "TITLE", "SORT_ORDER")
				.Values(2, "DEFAULT", "Post harvest", 2);

			/*
			 * 109756 - DIRECT - Assessment Wizard - Remove "Logistics" lc stage from options list
			 * By last requested change it's considered same as Distribution, leaved commented just if they change their mind
			 */
			//Database.Data.InsertInto("DXDIR_LC_STAGE_TMPL")
			//	.Columns("ID", "ASSESSMENT_TYPE_CODE", "TITLE", "SORT_ORDER")
			//	.Values(3, "DEFAULT", "Logistics", 3);

			Database.Data.InsertInto("DXDIR_LC_STAGE_TMPL")
				.Columns("ID", "ASSESSMENT_TYPE_CODE", "TITLE", "SORT_ORDER")
				.Values(4, "DEFAULT", "Processing", 4);

			Database.Data.InsertInto("DXDIR_LC_STAGE_TMPL")
				.Columns("ID", "ASSESSMENT_TYPE_CODE", "TITLE", "SORT_ORDER")
				.Values(5, "DEFAULT", "Distribution", 5);

			Database.Data.InsertInto("DXDIR_LC_STAGE_TMPL")
				.Columns("ID", "ASSESSMENT_TYPE_CODE", "TITLE", "SORT_ORDER")
				.Values(6, "DEFAULT", "Wholesale", 6);

			Database.Data.InsertInto("DXDIR_LC_STAGE_TMPL")
				.Columns("ID", "ASSESSMENT_TYPE_CODE", "TITLE", "SORT_ORDER")
				.Values(7, "DEFAULT", "Retail", 7);

			Database.Data.InsertInto("DXDIR_LC_STAGE_TMPL")
				.Columns("ID", "ASSESSMENT_TYPE_CODE", "TITLE", "SORT_ORDER")
				.Values(8, "DEFAULT", "Food Service", 8);
		}

		public override void Down()
		{
			throw new NotImplementedException();
		}
	}
}
