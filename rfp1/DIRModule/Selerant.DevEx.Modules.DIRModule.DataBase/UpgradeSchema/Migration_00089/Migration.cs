using Selerant.ApplicationBlocks.Data.Migrations;
using Selerant.ApplicationBlocks.Data.Migrations.Model.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Selerant.DevEx.Modules.DIRModule.DataBase.UpgradeSchema.Migration_00089
{
	class Migration : MigrationBase
	{
		public override void Up()
		{
			// Updating phrase
			Database.Data.ExecuteProcedure("DXTMP_UPD_PHRASE_TEXT").Values(11, "DXDIR_RESULT.TITLE", "LOSS_EX_WATER", "en", DbValue.Nvarchar("Total material loss to material input ratio"), 0);
		}

		public override void Down()
		{
			throw new NotImplementedException();
		}
	}
}
