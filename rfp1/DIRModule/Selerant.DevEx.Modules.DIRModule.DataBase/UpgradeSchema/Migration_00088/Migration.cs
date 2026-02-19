using Selerant.ApplicationBlocks.Data.Migrations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Selerant.DevEx.Modules.DIRModule.DataBase.UpgradeSchema.Migration_00088
{
    public class Migration : MigrationBase
    {
        public override void Up()
        {
            Database.Data.Update("DXDIR_DESTINATION")
                .Set("USED_ON = 3");

            Database.Execute.StoredProcedure("InputDestination.sql");
        }

        public override void Down()
        {
            throw new NotImplementedException();
        }
    }
}
