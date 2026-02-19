using Selerant.ApplicationBlocks.Data.Migrations;
using System;

namespace Selerant.DevEx.Modules.DIRModule.DataBase.CurrentSchema.PopulateData
{
	class DXDIR_ASSESSMENT_TYPE: MigrationBase
	{
		public override void Up()
		{
			Database.Data.InsertInto("DXDIR_ASSESSMENT_TYPE")
				.Columns("CODE", "DESCRIPTION", "ACTIVE")
				.Values("DEFAULT", "Food Industry", 1);
		}

		public override void Down()
		{
			throw new NotImplementedException();
		}
	}
}
