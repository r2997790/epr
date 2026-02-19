using Selerant.ApplicationBlocks.Data.Migrations;
using System;

namespace Selerant.DevEx.Modules.DIRModule.DataBase.CurrentSchema.PopulateData
{
	class DXDIR_PARTNER_ASSESSMENT_TYPE: MigrationBase
	{
		public override void Up()
		{
			Database.Data.InsertInto("DXDIR_PARTNER_ASSESSMENT_TYPE")
				.Columns("ASSESSMENT_TYPE_CODE", "PARTNER_ORG_CODE", "IS_SHARED")
				.Values("DEFAULT", "PARTNERS", 1m);
		}

		public override void Down()
		{
			throw new NotImplementedException();
		}
	}
}
