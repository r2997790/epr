using System;
using Selerant.ApplicationBlocks.Data.Migrations;
using Selerant.ApplicationBlocks.Data.Migrations.Model.Schema;

namespace Selerant.DevEx.Modules.DIRModule.DataBase.CurrentSchema
{
	public class CreateTables : MigrationBase
	{
		public override void Up()
		{
			Database.Create.Table("DXDIR_ASSESSMENT_TYPE")
				.WithColumn("CODE").Type.Varchar().Length(32).NotNullable()
				.WithColumn("DESCRIPTION").Type.Nvarchar().Length(500)
				.WithColumn("ACTIVE").Type.Decimal().Length(1).NotNullable().Default.Decimal(1)
				.Constraint.PrimaryKey("DXDIR_ASSESSMENT_TYPE_PK").Columns("CODE")
				.Constraint.Check("DXDIR_ASSESSMENT_TYPE_CK1").Condition("ACTIVE IN (0, 1)")
				.ConfigureStorage(StorageSize.Size_16K);

			#region Predefine/Template data tables

			// Predefine data tables
			Database.Create.Table("DXDIR_LC_STAGE_TMPL")
				.WithColumn("ID").Type.Decimal().Length(11).NotNullable()
				.WithColumn("ASSESSMENT_TYPE_CODE").Type.Varchar().Length(32).NotNullable()
				.WithColumn("TITLE").Type.Nvarchar().Length(256)
				.WithColumn("SORT_ORDER").Type.Decimal().Length(9)
				.Constraint.PrimaryKey("DXDIR_LC_STAGE_TMPL_PK").Columns("ID")
				.Constraint.ForeignKey("DXDIR_LC_STAGE_TMPL_FK1").Keys("ASSESSMENT_TYPE_CODE").References("DXDIR_ASSESSMENT_TYPE").Columns("CODE").OnDelete.Cascade()
				.ConfigureStorage(StorageSize.Size_1M);

			Database.Create.Table("DXDIR_INPUT_CATEGORY_TMPL")
				.WithColumn("ID").Type.Decimal().Length(11).NotNullable()
				.WithColumn("ASSESSMENT_TYPE_CODE").Type.Varchar().Length(32).NotNullable()
				.WithColumn("TITLE").Type.Nvarchar().Length(256)
				.WithColumn("SORT_ORDER").Type.Decimal().Length(9)
				.WithColumn("TYPE").Type.Varchar().Length(24)
				.Constraint.PrimaryKey("DXDIR_INPUT_CATEGORY_TMPL_PK").Columns("ID")
				.Constraint.ForeignKey("DXDIR_INPUT_CATEGORY_TMPL_FK1").Keys("ASSESSMENT_TYPE_CODE").References("DXDIR_ASSESSMENT_TYPE").Columns("CODE").OnDelete.Cascade()
				.Constraint.Check("DXDIR_INPUT_CATEGORY_TMPL_CK1").Condition("TYPE in ('FOOD', 'PACK', 'WATER')")
				.ConfigureStorage(StorageSize.Size_1M);

			Database.Create.Table("DXDIR_BUSINESS_COST_TMPL")
				.WithColumn("ID").Type.Decimal().Length(11).NotNullable()
				.WithColumn("ASSESSMENT_TYPE_CODE").Type.Varchar().Length(32).NotNullable()
				.WithColumn("TYPE").Type.Varchar().Length(24)
				.WithColumn("TITLE").Type.Varchar().Length(256).NotNullable()
				.WithColumn("SORT_ORDER").Type.Decimal().Length(9)
				.Constraint.PrimaryKey("DXDIR_BUSINESS_COST_TMPL_PK").Columns("ID")
				.Constraint.ForeignKey("DXDIR_BUSINESS_COST_TMPL_FK1").Keys("ASSESSMENT_TYPE_CODE").References("DXDIR_ASSESSMENT_TYPE").Columns("CODE").OnDelete.Cascade()
				.Constraint.Check("DXDIR_BUSINESS_COST_TMPL_CK1").Condition("TYPE in ('ENERGY', 'WASTE_COLLECT_TREATMENT')")
				.ConfigureStorage(StorageSize.Size_2M);

			#endregion Predefine/Template data tables

			Database.Create.Table("DXDIR_ASSESSMENT")
				.WithColumn("CODE").Type.Varchar().Length(32).NotNullable()
				.WithColumn("TYPE_CODE").Type.Varchar().Length(32).NotNullable()
				.WithColumn("DESCRIPTION").Type.Nvarchar().Length(512)
				.WithColumn("STATUS").Type.Varchar().Length(16).NotNullable()
				.WithColumn("COMPANY_NAME").Type.Nvarchar().Length(512)
				.WithColumn("ORG_STRUCTURE").Type.Varchar().Length(32).Nullable()
				.WithColumn("LOCATION").Type.Varchar().Length(3).Nullable()
				.WithColumn("PROD_CLASSIF").Type.Varchar().Length(40).NotNullable().Default.String("2143")
				.WithColumn("COMPLETING_BY").Type.Decimal().Length(11)
				.WithColumn("PHONE").Type.Varchar().Length(64)
				.WithColumn("EMAIL").Type.Nvarchar().Length(128)
				.WithColumn("TIMEFRAME_FROM").Type.DateTime()
				.WithColumn("TIMEFRAME_TO").Type.DateTime()
				.WithColumn("WASTE_WATER_DISCHARGE_RATIO").Type.Decimal().Precision(3, 2).NotNullable().Default.String("0.9")
				.WithColumn("CURRENCY").Type.Nvarchar().Length(3).NotNullable().Default.String("USD")
				.WithColumn("CREATE_DATE").Type.DateTime().NotNullable()
				.WithColumn("CREATED_BY").Type.Decimal().Length(11).NotNullable()
				.WithColumn("MOD_DATE").Type.DateTime()
				.WithColumn("MODIFIED_BY").Type.Decimal().Length(11)
				.WithColumn("AUTHORIZATION_ROLE").Type.Decimal().Length(11)
				.Constraint.PrimaryKey("DXDIR_ASSESSMENT_PK").Columns("CODE")
				.Constraint.ForeignKey("DXDIR_ASSESSMENT_FK1").Keys("TYPE_CODE").References("DXDIR_ASSESSMENT_TYPE").Columns("CODE").OnDelete.Cascade()
				.Constraint.ForeignKey("DXDIR_ASSESSMENT_FK2").Keys("COMPLETING_BY").References("DX_USER").Columns("ID").OnDelete.Cascade()
				.Constraint.ForeignKey("DXDIR_ASSESSMENT_FK3").Keys("CURRENCY").References("DX_CURRENCY").Columns("CODE")
				.Constraint.ForeignKey("DXDIR_ASSESSMENT_FK4").Keys("CREATED_BY").References("DX_USER").Columns("ID").OnDelete.Cascade()
				.Constraint.ForeignKey("DXDIR_ASSESSMENT_FK5").Keys("MODIFIED_BY").References("DX_USER").Columns("ID").OnDelete.SetNull()
				.Constraint.ForeignKey("DXDIR_ASSESSMENT_FK6").Keys("AUTHORIZATION_ROLE").References("DX_AUTHORIZATION_ROLE").Columns("ID")
				.ConfigureStorage(StorageSize.Size_5M);

			Database.Create.Table("DXDIR_ATTVALUE_ASSESSMENT")
				.WithColumn("CODE").Type.Varchar().Length(32).NotNullable()
				.WithColumn("ID").Type.Decimal().Length(11).NotNullable()
				.WithColumn("COUNTRY").Type.Varchar().Length(3).NotNullable()
				.WithColumn("LANGUAGE").Type.Varchar().Length(2).NotNullable()
				.WithColumn("ARRAY_INDEX").Type.Decimal().Length(11).NotNullable()
				.WithColumn("VALUE").Type.Nvarchar().Length(2000)
				.Constraint.PrimaryKey("DXDIR_ATTVALUE_ASSESSMENT_PK").Columns("CODE", "ID", "COUNTRY", "LANGUAGE", "ARRAY_INDEX")
				.Constraint.ForeignKey("DXDIR_ATTVALUE_ASSESSMENT_FK1").Keys("CODE").References("DXDIR_ASSESSMENT").Columns("CODE").OnDelete.Cascade()
				.Constraint.ForeignKey("DXDIR_ATTVALUE_ASSESSMENT_FK2").Keys("ID").References("DX_ATTRIBUTE_DEF").Columns("ID")
				.Constraint.ForeignKey("DXDIR_ATTVALUE_ASSESSMENT_FK3").Keys("LANGUAGE").References("DX_LANGUAGE").Columns("CODE")
				.Constraint.ForeignKey("DXDIR_ATTVALUE_ASSESSMENT_FK4").Keys("COUNTRY").References("DX_COUNTRY").Columns("CODE")
				.ConfigureStorage(StorageSize.Size_1M);

			Database.Create.Table("DXDIR_ASSESSMENT_LC_STAGE")
				.WithColumn("ID").Type.Decimal().Length(11).NotNullable()
				.WithColumn("ASSESSMENT_CODE").Type.Varchar().Length(32).NotNullable()
				.WithColumn("TITLE").Type.Nvarchar().Length(256)
				.WithColumn("VISIBLE").Type.Decimal().Length(1).NotNullable().Default.Decimal(1)
				.WithColumn("SORT_ORDER").Type.Decimal().Length(9)
				.WithColumn("SOURCE_ASMT_LC_STAGE_ID").Type.Decimal().Length(11)
				.Constraint.PrimaryKey("DXDIR_ASSESSMENT_LC_STAGE_PK").Columns("ID")
				.Constraint.ForeignKey("DXDIR_ASSESSMENT_LC_STAGE_FK1").Keys("ASSESSMENT_CODE").References("DXDIR_ASSESSMENT").Columns("CODE").OnDelete.Cascade()
				.Constraint.ForeignKey("DXDIR_ASSESSMENT_LC_STAGE_FK2").Keys("SOURCE_ASMT_LC_STAGE_ID").References("DXDIR_ASSESSMENT_LC_STAGE").Columns("ID").OnDelete.SetNull()
				.ConfigureStorage(StorageSize.Size_1M);

			Database.Create.Table("DXDIR_INPUT_CATEGORY")
				.WithColumn("ID").Type.Decimal().Length(11).NotNullable()
				.WithColumn("ASSESSMENT_CODE").Type.Varchar().Length(32).NotNullable()
				.WithColumn("TITLE").Type.Nvarchar().Length(256).NotNullable()
				.WithColumn("TYPE").Type.Varchar().Length(24)
				.Constraint.PrimaryKey("DXDIR_INPUT_CATEGORY_PK").Columns("ID")
				.Constraint.ForeignKey("DXDIR_INPUT_CATEGORY_FK1").Keys("ASSESSMENT_CODE").References("DXDIR_ASSESSMENT").Columns("CODE").OnDelete.Cascade()
				.ConfigureStorage(StorageSize.Size_1M);

			Database.Create.Table("DXDIR_OUTPUT_CATEGORY")
				.WithColumn("ID").Type.Decimal().Length(11).NotNullable()
				.WithColumn("TITLE").Type.Nvarchar().Length(256).NotNullable()
				.WithColumn("SORT_ORDER").Type.Decimal().Length(9)
				.WithColumn("TYPE").Type.Varchar().Length(24).NotNullable()
				.Constraint.PrimaryKey("DXDIR_OUTPUT_CATEGORY_PK").Columns("ID")
				.ConfigureStorage(StorageSize.Size_1M);

			Database.Create.Table("DXDIR_DESTINATION")
				.WithColumn("CODE").Type.Varchar().Length(64).NotNullable()
				.WithColumn("WASTE").Type.Decimal().Length(1).NotNullable()
				.WithColumn("USED_ON").Type.Decimal().Length(1).NotNullable()
				.WithColumn("SORT_ORDER").Type.Decimal().Length(9).NotNullable()
				.WithColumn("TITLE").Type.Nvarchar().Length(256).NotNullable()
				.Constraint.PrimaryKey("DXDIR_DESTINATION_PK").Columns("CODE")
				.Constraint.Check("DXDIR_DESTINATION_CK1").Condition("WASTE IN (0, 1)")
				.ConfigureStorage(StorageSize.Size_16K);

			Database.Create.Table("DXDIR_INPUT")
				.WithColumn("ID").Type.Decimal().Length(11).NotNullable()
				.WithColumn("ASSESSMENT_CODE").Type.Varchar().Length(32).NotNullable()
				.WithColumn("INPUT_CATEGORY_ID").Type.Decimal().Length(11).NotNullable()
				.WithColumn("ASSESSMENT_LC_STAGE_ID").Type.Decimal().Length(11).NotNullable()
				.WithColumn("MAT_PLANT").Type.Varchar().Length(20).NotNullable()
				.WithColumn("MAT_CODE").Type.Varchar().Length(32).NotNullable()
				.WithColumn("PART_OF_PRODUCT_COPRODUCT").Type.Decimal().Length(1).NotNullable()
				.WithColumn("PACKAGING").Type.Decimal().Length(1).Nullable()
				.WithColumn("MASS").Type.Decimal().Precision(19, 3)
				.WithColumn("COST").Type.Decimal().Precision(24, 4)
				.WithColumn("INEDIBLE_PARTS").Type.Decimal().Precision(3, 2)
				.WithColumn("MEASUREMENT").Type.Decimal().Length(1).NotNullable().Default.Decimal(1)
				.WithColumn("PRODUCT_SOURCE").Type.Varchar().Length(64).Nullable()
				.WithColumn("CATEGORY_SORT_ORDER").Type.Decimal().Length(9).NotNullable()
				.WithColumn("INPUT_SORT_ORDER").Type.Decimal().Length(9).NotNullable()
				.Constraint.PrimaryKey("DXDIR_INPUT_PK").Columns("ID")
				.Constraint.ForeignKey("DXDIR_INPUT_FK1").Keys("ASSESSMENT_CODE").References("DXDIR_ASSESSMENT").Columns("CODE").OnDelete.Cascade()
				.Constraint.ForeignKey("DXDIR_INPUT_FK2").Keys("INPUT_CATEGORY_ID").References("DXDIR_INPUT_CATEGORY").Columns("ID").OnDelete.Cascade()
				.Constraint.ForeignKey("DXDIR_INPUT_FK3").Keys("ASSESSMENT_LC_STAGE_ID").References("DXDIR_ASSESSMENT_LC_STAGE").Columns("ID").OnDelete.Cascade()
				.Constraint.ForeignKey("DXDIR_INPUT_FK4").Keys("MAT_PLANT", "MAT_CODE").References("DX_MATERIAL").Columns("PLANT", "MATCODE")
				.Constraint.Check("DXDIR_INPUT_CK1").Condition("INEDIBLE_PARTS between 0.00 and 1.00")
				.Constraint.Check("DXDIR_INPUT_CK2").Condition("PART_OF_PRODUCT_COPRODUCT IN (0, 1)")
				.Constraint.Check("DXDIR_INPUT_CK3").Condition("PACKAGING IN (0, 1)")
				.ConfigureStorage(StorageSize.Size_2M);

			Database.Create.Table("DXDIR_INPUT_PROD_COPROD_SPREAD")
				.WithColumn("INPUT_ID").Type.Decimal().Length(11).NotNullable()
				.WithColumn("DESTINATION_CODE").Type.Varchar().Length(64).NotNullable()
				.Constraint.PrimaryKey("DXDIR_PK_INPUT_PCP_SPREAD_PK").Columns("INPUT_ID", "DESTINATION_CODE")
				.Constraint.ForeignKey("DXDIR_PK_INPUT_PCP_SPREAD_FK1").Keys("INPUT_ID").References("DXDIR_INPUT").Columns("ID").OnDelete.Cascade()
				.Constraint.ForeignKey("DXDIR_PK_INPUT_PCP_SPREAD_FK2").Keys("DESTINATION_CODE").References("DXDIR_DESTINATION").Columns("CODE").OnDelete.Cascade()
				.ConfigureStorage(StorageSize.Size_16K);

			Database.Create.Table("DXDIR_OUTPUT")
				.WithColumn("ID").Type.Decimal().Length(11).NotNullable()
				.WithColumn("ASSESSMENT_CODE").Type.Varchar().Length(32).NotNullable()
				.WithColumn("OUTPUT_CATEGORY_ID").Type.Decimal().Length(11).NotNullable()
				.WithColumn("DESTINATION_CODE").Type.Varchar().Length(64)
				.WithColumn("ASSESSMENT_LC_STAGE_ID").Type.Decimal().Length(11).NotNullable()
				.WithColumn("INPUT_ID").Type.Decimal().Length(11)
				.WithColumn("MAT_PLANT").Type.Varchar().Length(20)
				.WithColumn("MAT_CODE").Type.Varchar().Length(32)
                .WithColumn("OUTPUT_COST").Type.Decimal().Precision(24, 4)
                .WithColumn("COST").Type.Decimal().Precision(24, 4)
				.WithColumn("INCOME").Type.Decimal().Precision(24, 4)
				.WithColumn("WEIGHT").Type.Decimal().Precision(19, 3)
				.WithColumn("SORT_ORDER").Type.Decimal().Length(9)
				.Constraint.PrimaryKey("DXDIR_OUTPUT_PK").Columns("ID")
				.Constraint.ForeignKey("DXDIR_OUTPUT_FK1").Keys("ASSESSMENT_CODE").References("DXDIR_ASSESSMENT").Columns("CODE").OnDelete.Cascade()
				.Constraint.ForeignKey("DXDIR_OUTPUT_FK2").Keys("OUTPUT_CATEGORY_ID").References("DXDIR_OUTPUT_CATEGORY").Columns("ID").OnDelete.Cascade()
				.Constraint.ForeignKey("DXDIR_OUTPUT_FK3").Keys("DESTINATION_CODE").References("DXDIR_DESTINATION").Columns("CODE").OnDelete.Cascade()
				.Constraint.ForeignKey("DXDIR_OUTPUT_FK4").Keys("ASSESSMENT_LC_STAGE_ID").References("DXDIR_ASSESSMENT_LC_STAGE").Columns("ID").OnDelete.Cascade()
				.Constraint.ForeignKey("DXDIR_OUTPUT_FK5").Keys("INPUT_ID").References("DXDIR_INPUT").Columns("ID").OnDelete.Cascade()
				.Constraint.ForeignKey("DXDIR_OUTPUT_FK6").Keys("MAT_PLANT", "MAT_CODE").References("DX_MATERIAL").Columns("PLANT", "MATCODE")
				.ConfigureStorage(StorageSize.Size_2M);

            Database.Create.Table("DXDIR_BUSINESS_COST")
                .WithColumn("ID").Type.Decimal().Length(11).NotNullable()
                .WithColumn("ASSESSMENT_CODE").Type.Varchar().Length(32).NotNullable()
                .WithColumn("ASSESSMENT_LC_STAGE_ID").Type.Decimal().Length(11).NotNullable()
                .WithColumn("TYPE").Type.Varchar().Length(24)
                .WithColumn("TITLE").Type.Nvarchar().Length(256).NotNullable()
                .WithColumn("SORT_ORDER").Type.Decimal().Length(9)
                .WithColumn("COST").Type.Decimal().Precision(24, 4)
                .Constraint.PrimaryKey("DXDIR_BUSINESS_COST_PK").Columns("ID")
                .Constraint.ForeignKey("DXDIR_BUSINESS_COST_FK1").Keys("ASSESSMENT_CODE").References("DXDIR_ASSESSMENT").Columns("CODE").OnDelete.Cascade()
                .Constraint.ForeignKey("DXDIR_BUSINESS_COST_FK2").Keys("ASSESSMENT_LC_STAGE_ID").References("DXDIR_ASSESSMENT_LC_STAGE").Columns("ID").OnDelete.Cascade()
                .Constraint.Check("DXDIR_BUSINESS_COST_CK1").Condition("TYPE in ('ENERGY', 'WASTE_COLLECT_TREATMENT')")
                .ConfigureStorage(StorageSize.Size_2M);

            Database.Create.Table("DXDIR_RESULT_ROW")
				.WithColumn("ID").Type.Decimal().Precision(2, 0).NotNullable()
				.WithColumn("TITLE").Type.Varchar().Length(512).NotNullable()
				.WithColumn("RESULT_UOM").Type.Varchar().Length(16).NotNullable()
				.WithColumn("RESULT_TYPE").Type.Varchar().Length(1).NotNullable()
				.WithColumn("SORT_ORDER").Type.Decimal().Precision(3, 0).NotNullable()
				.Constraint.PrimaryKey("DXDIR_RESULT_ROW_PK").Columns("ID")
				.ConfigureStorage(StorageSize.Size_128K);

			Database.Create.Table("DXDIR_RESULT")
				.WithColumn("ID").Type.Decimal().Length(11).NotNullable()
				.WithColumn("ASSESSMENT_CODE").Type.Varchar().Length(32).NotNullable()
				.WithColumn("ASSESSMENT_LC_STAGE_ID").Type.Decimal().Length(11).NotNullable()
				.WithColumn("RESULT_ROW_ID").Type.Decimal().Precision(2, 0).NotNullable()
				.WithColumn("RESULT").Type.Decimal().Precision(24, 4).NotNullable()
                .Constraint.PrimaryKey("DXDIR_RESULT_PK").Columns("ID")
				.Constraint.ForeignKey("DXDIR_RESULT_FK1").Keys("ASSESSMENT_CODE").References("DXDIR_ASSESSMENT").Columns("CODE").OnDelete.Cascade()
				.Constraint.ForeignKey("DXDIR_RESULT_FK2").Keys("ASSESSMENT_LC_STAGE_ID").References("DXDIR_ASSESSMENT_LC_STAGE").Columns("ID").OnDelete.Cascade()
				.Constraint.ForeignKey("DXDIR_RESULT_FK3").Keys("RESULT_ROW_ID").References("DXDIR_RESULT_ROW").Columns("ID").OnDelete.Cascade()
				.ConfigureStorage(StorageSize.Size_2M);

			Database.Create.Table("DXDIR_PARTNER_ASSESSMENT")
				.WithColumn("ASSESSMENT_CODE").Type.Varchar().Length(32).NotNullable()
				.WithColumn("PARTNER_ORG_CODE").Type.Varchar().Length(20).NotNullable()
				.WithColumn("IS_SHARED").Type.Decimal().Length(1).NotNullable().Default.Decimal(0)
				.Constraint.PrimaryKey("DXDIR_PARTNER_ASSESSMENT_PK").Columns("ASSESSMENT_CODE", "PARTNER_ORG_CODE")
				.Constraint.ForeignKey("DXDIR_PARTNER_ASSESSMENT_FK1").Keys("ASSESSMENT_CODE").References("DXDIR_ASSESSMENT").Columns("CODE").OnDelete.Cascade()
				.Constraint.ForeignKey("DXDIR_PARTNER_ASSESSMENT_FK2").Keys("PARTNER_ORG_CODE").References("DX_ORGANIZATION").Columns("CODE").OnDelete.Cascade()
				.Constraint.Check("DXDIR_PARTNER_ASSESSMENT_CK1").Condition("IS_SHARED IN (0, 1)")
				.ConfigureStorage(StorageSize.Size_256K);

            Database.Create.Table("DXDIR_PARTNER_ASSESSMENT_TYPE")
                .WithColumn("ASSESSMENT_TYPE_CODE").Type.Varchar().Length(32).NotNullable()
                .WithColumn("PARTNER_ORG_CODE").Type.Varchar().Length(20).NotNullable()
                .WithColumn("IS_SHARED").Type.Decimal().Length(1).NotNullable().Default.Decimal(0)
                .Constraint.PrimaryKey("DXDIR_PARTNER_ASMT_TYPE_PK").Columns("ASSESSMENT_TYPE_CODE", "PARTNER_ORG_CODE")
                .Constraint.ForeignKey("DXDIR_PARTNER_ASMT_TYPE_FK1").Keys("ASSESSMENT_TYPE_CODE").References("DXDIR_ASSESSMENT_TYPE").Columns("CODE").OnDelete.Cascade()
                .Constraint.ForeignKey("DXDIR_PARTNER_ASMT_TYPE_FK2").Keys("PARTNER_ORG_CODE").References("DX_ORGANIZATION").Columns("CODE").OnDelete.Cascade()
                .Constraint.Check("DXDIR_PARTNER_ASMT_TYPE_CK1").Condition("IS_SHARED IN (0, 1)")
                .ConfigureStorage(StorageSize.Size_256K);


            Database.Create.Table("DXDIR_RECENT_ASSESSMENT")
				.WithColumn("USER_ID").Type.Decimal().Length(11).NotNullable()
				.WithColumn("CODE").Type.Varchar().Length(32).NotNullable()
				.WithColumn("TIMESTAMP").Type.DateTime().NotNullable()
				.Constraint.PrimaryKey("DX_RECENT_ASSESSMENT_PK").Columns("USER_ID", "CODE")
				.Constraint.ForeignKey("DX_RECENT_ASSESSMENT_FK1").Keys("USER_ID").References("DX_USER").Columns("ID").OnDelete.Cascade()
				.Constraint.ForeignKey("DX_RECENT_ASSESSMENT_FK2").Keys("CODE").References("DXDIR_ASSESSMENT").Columns("CODE").OnDelete.Cascade()
				.ConfigureStorage(StorageSize.Size_16K);

			Database.Create.Table("DXDIR_ASSESSMENT_DESTINATION")
				.WithColumn("ASSESSMENT_CODE").Type.Varchar().Length(32).NotNullable()
				.WithColumn("DESTINATION_CODE").Type.Varchar().Length(64).NotNullable()
				.Constraint.PrimaryKey("DXDIR_ASSESSMENT_DEST_PK").Columns("ASSESSMENT_CODE", "DESTINATION_CODE")
				.Constraint.ForeignKey("DXDIR_ASSESSMENT_DEST_FK1").Keys("ASSESSMENT_CODE").References("DXDIR_ASSESSMENT").Columns("CODE").OnDelete.Cascade()
                .Constraint.ForeignKey("DXDIR_ASSESSMENT_DEST_FK2").Keys("DESTINATION_CODE").References("DXDIR_DESTINATION").Columns("CODE").OnDelete.Cascade()
                .ConfigureStorage(StorageSize.Size_1M);

			Database.Create.Table("DXDIR_INPUT_DESTINATION")
				.WithColumn("INPUT_ID").Type.Decimal().Length(11).NotNullable()
				.WithColumn("DESTINATION_CODE").Type.Varchar().Length(64).NotNullable()
				.WithColumn("OUTPUT_CATEGORY_ID").Type.Decimal().Length(11).NotNullable()
				.WithColumn("PERCENTAGE").Type.Decimal().Precision(4, 1).NotNullable()
				.Constraint.PrimaryKey("DXDIR_INPUT_DESTINATION_PK").Columns("INPUT_ID", "DESTINATION_CODE", "OUTPUT_CATEGORY_ID")
				.Constraint.ForeignKey("DXDIR_INPUT_DESTINATION_FK1").Keys("INPUT_ID").References("DXDIR_INPUT").Columns("ID").OnDelete.Cascade()
				.Constraint.ForeignKey("DXDIR_INPUT_DESTINATION_FK2").Keys("DESTINATION_CODE").References("DXDIR_DESTINATION").Columns("CODE").OnDelete.Cascade()
				.Constraint.ForeignKey("DXDIR_INPUT_DESTINATION_FK3").Keys("OUTPUT_CATEGORY_ID").References("DXDIR_OUTPUT_CATEGORY").Columns("ID").OnDelete.Cascade()
				.Constraint.Check("DXDIR_INPUT_DESTINATION_CK1").Condition("OUTPUT_CATEGORY_ID IN (4, 5, 6)")
				.ConfigureStorage(StorageSize.Size_2M);

			Database.Create.Table("DXDIR_RESOURCE_NOTE")
				.WithColumn("ASSESSMENT_CODE").Type.Varchar().Length(32).NotNullable()
				.WithColumn("LC_STAGE_ID").Type.Decimal().Length(11).NotNullable()
				.WithColumn("TYPE").Type.Varchar().Length(32).NotNullable()
				.WithColumn("NOTE").Type.Varchar().Length(2000).Nullable()
				.Constraint.PrimaryKey("DXDIR_RESOURCE_NOTE_PK").Columns("ASSESSMENT_CODE", "LC_STAGE_ID", "TYPE")
				.Constraint.ForeignKey("DXDIR_RESOURCE_NOTE_FK1").Keys("ASSESSMENT_CODE").References("DXDIR_ASSESSMENT").Columns("CODE").OnDelete.Cascade()
				.Constraint.ForeignKey("DXDIR_RESOURCE_NOTE_FK2").Keys("LC_STAGE_ID").References("DXDIR_ASSESSMENT_LC_STAGE").Columns("ID").OnDelete.Cascade()
				.Constraint.Check("DXDIR_RESOURCE_NOTE_CK1").Condition("TYPE IN ('INPUT', 'FOOD_DEST', 'NONFOOD_DEST', 'OUTPUT', 'BUSINESS_COST', 'BUSINESS_COST_OTHER')")
				.ConfigureStorage(StorageSize.Size_1M);
		}

		public override void Down()
		{
			throw new NotImplementedException();
		}
	}
}