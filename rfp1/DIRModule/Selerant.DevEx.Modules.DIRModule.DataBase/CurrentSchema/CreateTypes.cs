using System;
using Selerant.ApplicationBlocks.Data;
using Selerant.ApplicationBlocks.Data.Migrations;

namespace Selerant.DevEx.Modules.DIRModule.DataBase.CurrentSchema
{
	public class CreateTypes : MigrationBase
	{
		public override void Up()
		{
			Database.Create.Type("DXDIR_ASSESSMENTKey").As.Object()
				.WithField("CODE").Type.Varchar().Length(128);

			Database.Create.Type("DXDIR_ASSESSMENTKeyTable").As.Table().Of.UserDefined("DXDIR_ASSESSMENTKey");

			// NOTE: this table type for SQLServer is defined at the top of BusinessCost.sql (because DbManager is outputing wrong type for PBC_COST_PREV)
			if (Context.ProviderType == DataProviderType.Oracle)
			{
				Database.Create.Type("DXDIR_BizCostCarriedKey").As.Object()
					.WithField("SORT_ORDER").Type.Decimal().Length(9)
					.WithField("TITLE_PREV").Type.Nvarchar().Length(256)
					.WithField("PBC_COST_PREV").Type.Decimal().Precision(26, 4);

				Database.Create.Type("DXDIR_BizCostCarriedKeyTable").As.Table().Of.UserDefined("DXDIR_BizCostCarriedKey");
			}
		}

		public override void Down()
		{
			throw new NotImplementedException();
		}
	}
}
