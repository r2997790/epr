using Selerant.ApplicationBlocks.Data.Migrations;
using System;

namespace Selerant.DevEx.Modules.DIRModule.DataBase.UpgradeSchema.Migration_00083
{
	public class Migration : MigrationBase
	{
		public override void Up()
		{
			Database.Alter.Table("DXDIR_INPUT")
				.Add.NewColumn("PRODUCT_SOURCE").Type.Varchar().Length(64).Nullable();

			Database.Execute.StoredProcedure("Input.sql");
		}

		public override void Down()
		{
			throw new NotImplementedException();
		}
	}
}
