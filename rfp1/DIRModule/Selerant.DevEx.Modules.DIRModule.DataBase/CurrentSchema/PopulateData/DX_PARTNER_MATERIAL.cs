using System;
using Selerant.ApplicationBlocks.Data.Migrations;

namespace Selerant.DevEx.Modules.DIRModule.DataBase.CurrentSchema.PopulateData
{
	public class DX_PARTNER_MATERIAL : MigrationBase
	{
		public override void Up()
		{
			Database.Data.InsertInto("DX_PARTNER_MATERIAL")
				.Columns("PLANT", "MATCODE", "PARTNER_ORG_CODE", "IS_SHARED")
				.Values("NONE", "DIR_FOOD_WATER", "PARTNERS", 1m);

			Database.Data.InsertInto("DX_PARTNER_MATERIAL")
				.Columns("PLANT", "MATCODE", "PARTNER_ORG_CODE", "IS_SHARED")
				.Values("NONE", "DIR_NONFOOD_WATER", "PARTNERS", 1m);
		}

		public override void Down()
		{
			throw new NotImplementedException();
		}
	}
}