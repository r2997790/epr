using System;
using Selerant.ApplicationBlocks.Data.Migrations;
using Selerant.ApplicationBlocks.Data.Migrations.Model.Data;
using Selerant.ApplicationBlocks.Data.Migrations.Model.Schema;

namespace Selerant.DevEx.Modules.DIRModule.DataBase.UpgradeSchema.Migration_00076
{
	public class Migration : MigrationBase
	{
		public override void Up()
		{
			#region Adding additional Non-Waste (Product/CoProduct) destinatins and keeping sortOrder

			// Rename existing  Product title
			Database.Data.Update("DXDIR_DESTINATION")
			.Set("TITLE = 'Product 1'")
			.Where("CODE = 'PRODUCT'");

			Database.Data.InsertInto("DXDIR_DESTINATION")
			.Columns("CODE", "WASTE", "USED_ON", "SORT_ORDER", "TITLE")
			.Values("PRODUCT_2", 0, 3, 2, DbValue.Nvarchar("Product 2"));

			Database.Data.InsertInto("DXDIR_DESTINATION")
			.Columns("CODE", "WASTE", "USED_ON", "SORT_ORDER", "TITLE")
			.Values("PRODUCT_3", 0, 3, 3, DbValue.Nvarchar("Product 3"));

			// Rename existing Co-Product title
			Database.Data.Update("DXDIR_DESTINATION")
			.Set("TITLE = 'Co-Product 1', SORT_ORDER = 4")
			.Where("CODE = 'COPRODUCT'");

			Database.Data.InsertInto("DXDIR_DESTINATION")
			.Columns("CODE", "WASTE", "USED_ON", "SORT_ORDER", "TITLE")
			.Values("COPRODUCT_2", 0, 3, 5, DbValue.Nvarchar("Co-Product 2"));

			// Maintain codebook sort order for other destinations
			Database.Data.Update("DXDIR_DESTINATION")
				.Set("SORT_ORDER = SORT_ORDER + 3")
				.Where("WASTE = 1");

			Database.Data.Update("DXDIR_DESTINATION")
				.Set("SORT_ORDER = 6")
				.Where("CODE = 'FOOD_RESCUE'");

			// Update Ouputs sort order, they are 1:1 with Destinations
			Database.Data.Update("DXDIR_OUTPUT")
				.Set("SORT_ORDER = SORT_ORDER + 3")
				.Where("SORT_ORDER IS NOT NULL");

			#endregion

			#region New Table for keeping per Input NonWaste destinations

			Database.Create.Table("DXDIR_INPUT_PROD_COPROD_SPREAD")
				.WithColumn("INPUT_ID").Type.Decimal().Length(11).NotNullable()
				.WithColumn("DESTINATION_CODE").Type.Varchar().Length(64).NotNullable()
				.Constraint.PrimaryKey("DXDIR_PK_INPUT_PCP_SPREAD_PK").Columns("INPUT_ID", "DESTINATION_CODE")
				.Constraint.ForeignKey("DXDIR_PK_INPUT_PCP_SPREAD_FK1").Keys("INPUT_ID").References("DXDIR_INPUT").Columns("ID").OnDelete.Cascade()
				.Constraint.ForeignKey("DXDIR_PK_INPUT_PCP_SPREAD_FK2").Keys("DESTINATION_CODE").References("DXDIR_DESTINATION").Columns("CODE").OnDelete.Cascade()
				.ConfigureStorage(StorageSize.Size_16K);

			Database.Execute.StoredProcedure("InputProductCoProductSpread.sql");

			// insert data for old input rows with PartOfProductCoProduct = TRUE
			Database.Execute.File("PopulateInputProductCoProductSpreadTable.sql");

			#endregion

			Database.Execute.StoredProcedure("InputDestination.sql");

			#region Output

			// Old Product
			Database.Data.Update("DXDIR_OUTPUT")
				.Set("SORT_ORDER = 1, DESTINATION_CODE = 'PRODUCT'")
				.Where("OUTPUT_CATEGORY_ID = 1 AND DESTINATION_CODE IS NULL AND INPUT_ID IS NULL AND SORT_ORDER IS NULL");
			// Old Co-Product
			Database.Data.Update("DXDIR_OUTPUT")
				.Set("SORT_ORDER = 4, DESTINATION_CODE = 'COPRODUCT'")
				.Where("OUTPUT_CATEGORY_ID = 2 AND DESTINATION_CODE IS NULL AND INPUT_ID IS NULL AND SORT_ORDER IS NULL");
			// Old Food Rescue
			Database.Data.Update("DXDIR_OUTPUT")
				.Set("SORT_ORDER = 6, DESTINATION_CODE = 'FOOD_RESCUE'")
				.Where("OUTPUT_CATEGORY_ID = 3 AND DESTINATION_CODE IS NULL AND INPUT_ID IS NULL AND SORT_ORDER IS NULL");

			Database.Execute.StoredProcedure("Output.sql");

			#endregion

			Database.Execute.StoredProcedure("BusinessCost.sql");
			Database.Execute.StoredProcedure("Input.sql");
			Database.Execute.StoredProcedure("Calculations.sql");
			Database.Execute.StoredProcedure("ResultCalculations.sql");
		}

		public override void Down()
		{
			throw new NotImplementedException();
		}
	}
}
