using Selerant.ApplicationBlocks.Data;
using Selerant.ApplicationBlocks.Data.Migrations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Selerant.DevEx.Modules.DIRModule.DataBase.UpgradeSchema.Migration_00092
{
    public class Migration : MigrationBase
    {
        public override void Up()
        {
            if (Context.ProviderType == DataProviderType.SqlServer)
            {
                Database.Execute.StoredProcedure("ResultCalculations.sql");
            }
        }

        public override void Down()
        {
            throw new NotImplementedException();
        }
    }
}
