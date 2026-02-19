using System;
using Selerant.ApplicationBlocks.Data.Migrations;

namespace Selerant.DevEx.Modules.DIRModule.DataBase.CurrentSchema
{
	public class CreateCurrentSchema : MigrationBase
	{
		public override void Up()
		{
			Database.Data.FromScript(@"CreateTypes.cs");
			Database.Data.FromScript(@"CreateTables.cs", "&idx_tspace &lob_tspace &choice");
			Database.Data.FromScript(@"CreateSequences.cs");
			Database.Data.FromScript(@"CreateIndices.cs");
			Database.Data.FromScript(@"CreateProcedures.cs");

			Database.Data.FromScript(@"Populate_Data.cs");
		}

		public override void Down()
		{
			throw new NotImplementedException();
		}
	}
}
