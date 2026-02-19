using Selerant.ApplicationBlocks.Data.Migrations;
using Selerant.ApplicationBlocks.Data.Migrations.Model.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Selerant.DevEx.Modules.DIRModule.DataBase.UpgradeSchema.Migration_00081
{
	class Migration : MigrationBase
	{
		public override void Up()
		{
			// renaming
			Database.Data.ExecuteProcedure("DXTMP_UPD_PHRASE_TEXT").Values(11, "DXDIR_RESULT.TITLE", "UTILISED_INPUT", "en", DbValue.Nvarchar("Material inputs to gross product ratio"), 0);
			Database.Data.ExecuteProcedure("DXTMP_UPD_PHRASE_TEXT").Values(11, "DXDIR_RESULT.TITLE", "MAT_SPEND", "en", DbValue.Nvarchar("Material spend to operating cost ratio"), 0);
			Database.Data.ExecuteProcedure("DXTMP_UPD_PHRASE_TEXT").Values(11, "DXDIR_RESULT.TITLE", "ENERGY_SPEND", "en", DbValue.Nvarchar("Energy spend to operating cost ratio"), 0);
		}

		public override void Down()
		{
			throw new NotImplementedException();
		}
	}
}
