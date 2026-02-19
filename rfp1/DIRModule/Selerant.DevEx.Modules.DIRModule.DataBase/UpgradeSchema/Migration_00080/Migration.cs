using Selerant.ApplicationBlocks.Data;
using Selerant.ApplicationBlocks.Data.Migrations;
using System;

namespace Selerant.DevEx.Modules.DIRModule.DataBase.UpgradeSchema.Migration_00080
{
    public class Migration : MigrationBase
    {
        public override void Up()
        {
            if (Context.ProviderType == DataProviderType.Oracle)
                Database.Execute.StoredProcedure("Input.sql");

            Database.Execute.StoredProcedure("Output.sql");
        }

        public override void Down()
        {
            throw new NotImplementedException();
        }
    }
}
