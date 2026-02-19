using Selerant.ApplicationBlocks.Data.Migrations;
using Selerant.ApplicationBlocks.Data.Migrations.Model.Data;
using System;

namespace Selerant.DevEx.Modules.DIRModule.DataBase.CurrentSchema.PopulateData
{
	public class DXDIR_OUTPUT_CATEGORY : MigrationBase
	{
		public override void Up()
		{
			Database.Data.InsertInto("DXDIR_OUTPUT_CATEGORY")
				.Columns("ID", "TITLE", "SORT_ORDER", "TYPE")
				.Values(1, "Product", 1, "PRODUCT");

			Database.Data.InsertInto("DXDIR_OUTPUT_CATEGORY")
				.Columns("ID", "TITLE", "SORT_ORDER", "TYPE")
				.Values(2, "Co-Product", 2, "COPRODUCT");

            Database.Data.InsertInto("DXDIR_OUTPUT_CATEGORY")
                .Columns("ID", "TITLE", "SORT_ORDER", "TYPE")
                .Values(3, "Food Rescue", 3, "FOOD_RESCUE");

            Database.Data.InsertInto("DXDIR_OUTPUT_CATEGORY")
				.Columns("ID", "TITLE", "SORT_ORDER", "TYPE")
				.Values(4, "Food", 4, "FOOD");

			Database.Data.InsertInto("DXDIR_OUTPUT_CATEGORY")
				.Columns("ID", "TITLE", "SORT_ORDER", "TYPE")
				.Values(5, "Inedible Parts", 5, "INEDIBLE");

			Database.Data.InsertInto("DXDIR_OUTPUT_CATEGORY")
				.Columns("ID", "TITLE", "SORT_ORDER", "TYPE")
				.Values(6, "Non-Food", 6, "NON_FOOD");

			Database.Data.InsertInto("DXDIR_OUTPUT_CATEGORY")
				.Columns("ID", "TITLE", "SORT_ORDER", "TYPE")
				.Values(7, "Wastewater",7, "WASTE_WATER");
		}

		public override void Down()
		{
			throw new NotImplementedException();
		}
	}
}
