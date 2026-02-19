using Selerant.ApplicationBlocks.Data.Migrations;
using Selerant.ApplicationBlocks.Data.Migrations.Model.Data;
using System;

namespace Selerant.DevEx.Modules.DIRModule.DataBase.CurrentSchema.PopulateData
{
	public class DXDIR_DESTINATION : MigrationBase
	{
		public override void Up()
		{
			// 1 - Food
			// 2 - Non-Food
			// 3 - Both
			Database.Data.InsertInto("DXDIR_DESTINATION")
				.Columns("CODE", "WASTE", "USED_ON", "SORT_ORDER", "TITLE")
				.Values("PRODUCT", 0, 3, 1, DbValue.Nvarchar("Product 1"));

			Database.Data.InsertInto("DXDIR_DESTINATION")
				.Columns("CODE", "WASTE", "USED_ON", "SORT_ORDER", "TITLE")
				.Values("PRODUCT_2", 0, 3, 2, DbValue.Nvarchar("Product 2"));

			Database.Data.InsertInto("DXDIR_DESTINATION")
				.Columns("CODE", "WASTE", "USED_ON", "SORT_ORDER", "TITLE")
				.Values("PRODUCT_3", 0, 3, 3, DbValue.Nvarchar("Product 3"));

			Database.Data.InsertInto("DXDIR_DESTINATION")
				.Columns("CODE", "WASTE", "USED_ON", "SORT_ORDER", "TITLE")
				.Values("COPRODUCT", 0, 3, 4, DbValue.Nvarchar("Co-Product 1"));

			Database.Data.InsertInto("DXDIR_DESTINATION")
				.Columns("CODE", "WASTE", "USED_ON", "SORT_ORDER", "TITLE")
				.Values("COPRODUCT_2", 0, 3, 5, DbValue.Nvarchar("Co-Product 2"));

			Database.Data.InsertInto("DXDIR_DESTINATION")
				.Columns("CODE", "WASTE", "USED_ON", "SORT_ORDER", "TITLE")
				.Values("FOOD_RESCUE", 0, 3, 6, DbValue.Nvarchar("Food Rescue"));

			Database.Data.InsertInto("DXDIR_DESTINATION")
				.Columns("CODE", "WASTE", "USED_ON", "SORT_ORDER", "TITLE")
				.Values("ANIMAL_FEED", 1, 3, 7, DbValue.Nvarchar("Animal feed"));

			Database.Data.InsertInto("DXDIR_DESTINATION")
				.Columns("CODE", "WASTE", "USED_ON", "SORT_ORDER", "TITLE")
				.Values("BIOMASS_MATERIAL", 1, 3, 8, DbValue.Nvarchar("Bio-material"));

			Database.Data.InsertInto("DXDIR_DESTINATION")
				.Columns("CODE", "WASTE", "USED_ON", "SORT_ORDER", "TITLE")
				.Values("CODIGESTION_ANAEROBIC", 1, 3, 9, DbValue.Nvarchar("Codigestion"));

			Database.Data.InsertInto("DXDIR_DESTINATION")
				.Columns("CODE", "WASTE", "USED_ON", "SORT_ORDER", "TITLE")
				.Values("COMPOSTING", 1, 3, 10, DbValue.Nvarchar("Composting"));

			Database.Data.InsertInto("DXDIR_DESTINATION")
				.Columns("CODE", "WASTE", "USED_ON", "SORT_ORDER", "TITLE")
				.Values("COMBUSTION", 1, 3, 11, DbValue.Nvarchar("Controlled combustion"));

			Database.Data.InsertInto("DXDIR_DESTINATION")
				.Columns("CODE", "WASTE", "USED_ON", "SORT_ORDER", "TITLE")
				.Values("LAND_APP", 1, 3, 12, DbValue.Nvarchar("Land Application"));

			Database.Data.InsertInto("DXDIR_DESTINATION")
				.Columns("CODE", "WASTE", "USED_ON", "SORT_ORDER", "TITLE")
				.Values("RECYCLING", 1, 3, 13, DbValue.Nvarchar("Recycling"));

			Database.Data.InsertInto("DXDIR_DESTINATION")
				.Columns("CODE", "WASTE", "USED_ON", "SORT_ORDER", "TITLE")
				.Values("INCIN_WITH_EN_RECOVER", 1, 3, 14, DbValue.Nvarchar("Incineration w en.recovery"));

			Database.Data.InsertInto("DXDIR_DESTINATION")
				.Columns("CODE", "WASTE", "USED_ON", "SORT_ORDER", "TITLE")
				.Values("LANDFILL", 1, 3, 15, DbValue.Nvarchar("Landfill"));

			Database.Data.InsertInto("DXDIR_DESTINATION")
				.Columns("CODE", "WASTE", "USED_ON", "SORT_ORDER", "TITLE")
				.Values("NOT_HARVESTED", 1, 3, 16, DbValue.Nvarchar("Not harvested"));

			Database.Data.InsertInto("DXDIR_DESTINATION")
				.Columns("CODE", "WASTE", "USED_ON", "SORT_ORDER", "TITLE")
				.Values("REFUSE_DISCARD", 1, 3, 17, DbValue.Nvarchar("Refuse"));

			Database.Data.InsertInto("DXDIR_DESTINATION")
				.Columns("CODE", "WASTE", "USED_ON", "SORT_ORDER", "TITLE")
				.Values("SEWER", 1, 3, 18, DbValue.Nvarchar("Sewer"));

			Database.Data.InsertInto("DXDIR_DESTINATION")
				.Columns("CODE", "WASTE", "USED_ON", "SORT_ORDER", "TITLE")
				.Values("ENVIRONMENT_LOSS", 1, 3, 19, DbValue.Nvarchar("Environmental loss"));

			Database.Data.InsertInto("DXDIR_DESTINATION")
				.Columns("CODE", "WASTE", "USED_ON", "SORT_ORDER", "TITLE")
				.Values("OTHER", 1, 3, 20, DbValue.Nvarchar("Other"));
		}

		public override void Down()
		{
			throw new NotImplementedException();
		}
	}
}