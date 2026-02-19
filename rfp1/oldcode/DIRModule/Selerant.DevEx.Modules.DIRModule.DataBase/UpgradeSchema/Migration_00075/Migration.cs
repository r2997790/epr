using Selerant.ApplicationBlocks.Data.Migrations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Selerant.DevEx.Modules.DIRModule.DataBase.UpgradeSchema.Migration_00075
{
	public class Migration : MigrationBase
	{
		public override void Up()
		{
			Database.Execute.StoredProcedure("ResultCalculations.sql");
		}

		public override void Down()
		{
			throw new NotImplementedException();
		}
	}
}
