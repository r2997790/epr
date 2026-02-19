using Selerant.ApplicationBlocks.Data.Migrations;
using System;

namespace Selerant.DevEx.Modules.DIRModule.DataBase.UpgradeSchema.Migration_00073
{
    public class Migration : MigrationBase
    {
        public override void Up()
        {
            Database.Execute.StoredProcedure("Input.sql");
        }

        public override void Down()
        {
            throw new NotImplementedException();
        }
    }
}
