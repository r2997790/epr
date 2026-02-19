using System;
using Selerant.ApplicationBlocks.Data.Migrations;
using Selerant.ApplicationBlocks.Data.Migrations.Model.Data;

namespace Selerant.DevEx.Modules.DIRModule.DataBase.CurrentSchema.PopulateData
{
	public class DX_ATTVALUE_MATERIAL : MigrationBase
	{
		public override void Up()
		{
			Database.Data.InsertInto("DX_ATTVALUE_MATERIAL")
					.Columns("PLANT", "MATCODE", "ID", "COUNTRY", "LANGUAGE", "ARRAY_INDEX", "VALUE")
					.Values("NONE", "DIR_FOOD_WATER", 99510, "NNN", "nn", 0, DbValue.Nvarchar("FOOD"));

			Database.Data.InsertInto("DX_ATTVALUE_MATERIAL")
					.Columns("PLANT", "MATCODE", "ID", "COUNTRY", "LANGUAGE", "ARRAY_INDEX", "VALUE")
					.Values("NONE", "DIR_NONFOOD_WATER", 99510, "NNN", "nn", 0, DbValue.Nvarchar("NONFOOD"));
		}

		public override void Down()
		{
			throw new NotImplementedException();
		}
	}
}