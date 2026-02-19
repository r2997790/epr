using System;
using Selerant.ApplicationBlocks.Data.Migrations;

namespace Selerant.DevEx.Modules.DIRModule.DataBase.UpgradeSchema.Migration_00079
{
	public class Migration : MigrationBase
	{
		public override void Up()
		{
			Database.Execute.StoredProcedure("Calculations.sql");
			Database.Execute.StoredProcedure("BusinessCost.sql");
			Database.Execute.StoredProcedure("ResultCalculations.sql");
		}

		public override void Down()
		{
			throw new NotImplementedException();
		}
	}
}
