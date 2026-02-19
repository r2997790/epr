using System;
using Selerant.ApplicationBlocks.Data.Migrations;
using Selerant.ApplicationBlocks.Data.Migrations.Model.Data;
using Selerant.ApplicationBlocks.Data.Migrations.Model.Schema;

namespace Selerant.DevEx.Modules.DIRModule.DataBase.UpgradeSchema.Migration_00078
{
	public class Migration : MigrationBase
	{
		public override void Up()
		{
			// Dropping unused and 2 never used columns

			Database.Alter.Table("DXDIR_ASSESSMENT")
				.Drop.Column("ANNUAL_INCOME");

			Database.Alter.Table("DXDIR_ASSESSMENT")
				.Drop.Column("PART_IN_ANNUAL_INCOME");

			Database.Alter.Table("DXDIR_ASSESSMENT")
				.Drop.Column("GROSS_PRODUCTION");

			Database.Alter.Table("DXDIR_ASSESSMENT")
				.Drop.Column("PART_IN_ANNUAL_PRODUCTION");

			Database.Execute.StoredProcedure("Assessment.sql");
			Database.Execute.StoredProcedure("NewAttValueAsessment.sql");
		}

		public override void Down()
		{
			throw new NotImplementedException();
		}
	}
}
