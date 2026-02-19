using System;
using Selerant.ApplicationBlocks.Data.Migrations;
using Selerant.ApplicationBlocks.Data.Migrations.Model.Data;

namespace Selerant.DevEx.Modules.DIRModule.DataBase.CurrentSchema.PopulateData
{
	public class DX_MATERIAL : MigrationBase
	{
		public override void Up()
		{
			Database.Data.InsertInto("DX_MATERIAL")
				.Columns("PLANT", "MATCODE", "MAT_TYPE", "DES", "CREAT_DATE", "CREATED_BY", "MOD_DATE", "FLAGS", "STATUS", "IS_SYSTEM")
				.Values("NONE", "DIR_FOOD_WATER", "DIR_RESOURCE", DbValue.Nvarchar("Water"), DbValue.DefaultTime, "DIRModule", DbValue.DefaultTime, 0, "00", 0);

			Database.Data.InsertInto("DX_MATERIAL")
				.Columns("PLANT", "MATCODE", "MAT_TYPE", "DES", "CREAT_DATE", "CREATED_BY", "MOD_DATE", "FLAGS", "STATUS", "IS_SYSTEM")
				.Values("NONE", "DIR_NONFOOD_WATER", "DIR_RESOURCE", DbValue.Nvarchar("Water"), DbValue.DefaultTime, "DIRModule", DbValue.DefaultTime, 0, "00", 0);
		}

		public override void Down()
		{
			throw new NotImplementedException();
		}
	}
}
