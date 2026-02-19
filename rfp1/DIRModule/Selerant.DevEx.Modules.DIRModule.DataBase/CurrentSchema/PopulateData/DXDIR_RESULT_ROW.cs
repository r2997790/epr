using Selerant.ApplicationBlocks.Data.Migrations;
using System;

namespace Selerant.DevEx.Modules.DIRModule.DataBase.CurrentSchema.PopulateData
{
    public class DXDIR_RESULT_ROW : MigrationBase
    {
        public override void Up()
        {
			Database.Data.InsertInto("DXDIR_RESULT_ROW")
				.Columns("ID", "TITLE", "RESULT_UOM", "RESULT_TYPE", "SORT_ORDER")
				.Values(1, "UTILISED_INPUT", "kg/kg", "M", 1);

			Database.Data.InsertInto("DXDIR_RESULT_ROW")
				.Columns("ID", "TITLE", "RESULT_UOM", "RESULT_TYPE", "SORT_ORDER")
				.Values(2, "STOCK_HOLDING", "kg/kg", "M", 2);

			Database.Data.InsertInto("DXDIR_RESULT_ROW")
				.Columns("ID", "TITLE", "RESULT_UOM", "RESULT_TYPE", "SORT_ORDER")
				.Values(3, "FOOD_LOSS", "kg/kg", "M", 3);

			Database.Data.InsertInto("DXDIR_RESULT_ROW")
				.Columns("ID", "TITLE", "RESULT_UOM", "RESULT_TYPE", "SORT_ORDER")
				.Values(4, "FOOD_EDIBLE_LOSS_INPUTS", "kg/kg", "M", 4);

			Database.Data.InsertInto("DXDIR_RESULT_ROW")
				.Columns("ID", "TITLE", "RESULT_UOM", "RESULT_TYPE", "SORT_ORDER")
				.Values(5, "FOOD_EDIBLE_LOSS", "kg/kg", "M", 5);

			Database.Data.InsertInto("DXDIR_RESULT_ROW")
				.Columns("ID", "TITLE", "RESULT_UOM", "RESULT_TYPE", "SORT_ORDER")
				.Values(6, "LOSS_EX_WATER", "kg/kg", "M", 6);

			Database.Data.InsertInto("DXDIR_RESULT_ROW")
				.Columns("ID", "TITLE", "RESULT_UOM", "RESULT_TYPE", "SORT_ORDER")
				.Values(7, "PACK_LOSS", "kg/kg", "M", 7);

			Database.Data.InsertInto("DXDIR_RESULT_ROW")
				.Columns("ID", "TITLE", "RESULT_UOM", "RESULT_TYPE", "SORT_ORDER")
				.Values(8, "OUTPUT_PACK", "kg/kg", "M", 8);

			Database.Data.InsertInto("DXDIR_RESULT_ROW")
				.Columns("ID", "TITLE", "RESULT_UOM", "RESULT_TYPE", "SORT_ORDER")
				.Values(9, "ESTIM_COST", "$", "V", 9);

			Database.Data.InsertInto("DXDIR_RESULT_ROW")
				.Columns("ID", "TITLE", "RESULT_UOM", "RESULT_TYPE", "SORT_ORDER")
				.Values(10, "WASTE_REMOVAL", "$", "V", 10);

			Database.Data.InsertInto("DXDIR_RESULT_ROW")
				.Columns("ID", "TITLE", "RESULT_UOM", "RESULT_TYPE", "SORT_ORDER")
				.Values(11, "ENERGY_SPEND", "$/$", "P", 11);

			Database.Data.InsertInto("DXDIR_RESULT_ROW")
				.Columns("ID", "TITLE", "RESULT_UOM", "RESULT_TYPE", "SORT_ORDER")
				.Values(12, "MAT_SPEND", "$/$", "P", 12);

			Database.Data.InsertInto("DXDIR_RESULT_ROW")
				.Columns("ID", "TITLE", "RESULT_UOM", "RESULT_TYPE", "SORT_ORDER")
				.Values(13, "DISPOSAL", "$/$", "P", 13);

			Database.Data.InsertInto("DXDIR_RESULT_ROW")
				.Columns("ID", "TITLE", "RESULT_UOM", "RESULT_TYPE", "SORT_ORDER")
				.Values(14, "ESTIM_COST_WASTE", "$/$", "P", 14);
		}

        public override void Down()
        {
            throw new NotImplementedException();
        }
    }
}
