using System;
using Selerant.ApplicationBlocks.Data;
using Selerant.ApplicationBlocks.Data.Migrations;
using Selerant.ApplicationBlocks.Data.Migrations.Model.Data;

namespace Selerant.DevEx.Modules.DIRModule.DataBase.CurrentSchema
{
	public class Seed_Dev_Data : MigrationBase
	{
		public void CommitGo()
		{
			if (Context.ProviderType == DataProviderType.Oracle)
				Database.Execute.RawSql(DataProviderType.Oracle, "commit;");
			else if (Context.ProviderType == DataProviderType.MsOracle)
				Database.Execute.RawSql(DataProviderType.SqlServer, "GO");
		}

		public override void Up()
		{
			const string ASSESSMENT_CODE = "DEV_00009999";
			const decimal LCS = 10m; // ASSESSMENT_LC_STAGE_ID

			Database.Data.InsertInto("DXDIR_ASSESSMENT")
				.Columns("CODE"
						, "TYPE_CODE"
						, "DESCRIPTION"
						, "STATUS"
						, "COMPANY_NAME"
						, "COMPLETING_BY"
						, "PHONE"
						, "EMAIL"
						, "TIMEFRAME_FROM"
						, "TIMEFRAME_TO"
						, "CREATE_DATE"
						, "CREATED_BY"
						, "PROD_CLASSIF")
				.Values(ASSESSMENT_CODE
						, "DEFAULT"
						, "Development Assessment"
						, "DEVELOPMENT"
						, "ACME Food company"
						, 1
						, "03 9123 4567"
						, @"joe@acme.com.au"
						, "05-JUL-19"
						, "05-JUL-20"
						, "05-JUL-19"
						, 1
						, "2143");
			CommitGo();

			#region DXDIR_ASSESSMENT_DESTINATION - Waste destinations

			Database.Data.InsertInto("DXDIR_ASSESSMENT_DESTINATION").Columns("ASSESSMENT_CODE", "DESTINATION_CODE")
										.Values(ASSESSMENT_CODE, "PRODUCT");
			Database.Data.InsertInto("DXDIR_ASSESSMENT_DESTINATION").Columns("ASSESSMENT_CODE", "DESTINATION_CODE")
										.Values(ASSESSMENT_CODE, "COPRODUCT");
			Database.Data.InsertInto("DXDIR_ASSESSMENT_DESTINATION").Columns("ASSESSMENT_CODE", "DESTINATION_CODE")
										.Values(ASSESSMENT_CODE, "FOOD_RESCUE");
			Database.Data.InsertInto("DXDIR_ASSESSMENT_DESTINATION").Columns("ASSESSMENT_CODE", "DESTINATION_CODE")
										.Values(ASSESSMENT_CODE, "ANIMAL_FEED");
			Database.Data.InsertInto("DXDIR_ASSESSMENT_DESTINATION").Columns("ASSESSMENT_CODE", "DESTINATION_CODE")
										.Values(ASSESSMENT_CODE, "BIOMASS_MATERIAL");
			Database.Data.InsertInto("DXDIR_ASSESSMENT_DESTINATION").Columns("ASSESSMENT_CODE", "DESTINATION_CODE")
										.Values(ASSESSMENT_CODE, "CODIGESTION_ANAEROBIC");
			Database.Data.InsertInto("DXDIR_ASSESSMENT_DESTINATION").Columns("ASSESSMENT_CODE", "DESTINATION_CODE")
										.Values(ASSESSMENT_CODE, "COMPOSTING");
			Database.Data.InsertInto("DXDIR_ASSESSMENT_DESTINATION").Columns("ASSESSMENT_CODE", "DESTINATION_CODE")
										.Values(ASSESSMENT_CODE, "COMBUSTION");
			Database.Data.InsertInto("DXDIR_ASSESSMENT_DESTINATION").Columns("ASSESSMENT_CODE", "DESTINATION_CODE")
										.Values(ASSESSMENT_CODE, "LANDFILL");
			Database.Data.InsertInto("DXDIR_ASSESSMENT_DESTINATION").Columns("ASSESSMENT_CODE", "DESTINATION_CODE")
										.Values(ASSESSMENT_CODE, "ENVIRONMENT_LOSS");

			#endregion

			// Inpust - development seed data - DON'T CHECK IN THIS TO TFS (useless in production)
			// DbValue.NextVal("DXDIR_INPUT_SEQ")
			// LcStages
			Database.Data.InsertInto("DXDIR_ASSESSMENT_LC_STAGE").Columns("ID", "ASSESSMENT_CODE", "TITLE", "SORT_ORDER")
													.Values(LCS, ASSESSMENT_CODE, "Farm", 1);

			// Input Categories
			Database.Data.InsertInto("DXDIR_INPUT_CATEGORY").Columns("ID", "ASSESSMENT_CODE", "TITLE", "TYPE")
															.Values(10, ASSESSMENT_CODE, "Food ingredients", "FOOD");

			Database.Data.InsertInto("DXDIR_INPUT_CATEGORY").Columns("ID", "ASSESSMENT_CODE", "TITLE", "TYPE")
															.Values(11, ASSESSMENT_CODE, "Packaging inputs", "NONFOOD");

			Database.Data.InsertInto("DXDIR_INPUT_CATEGORY").Columns("ID", "ASSESSMENT_CODE", "TITLE", "TYPE")
															.Values(12, ASSESSMENT_CODE, "Other inputs", "NONFOOD");

			Database.Data.InsertInto("DXDIR_INPUT_CATEGORY").Columns("ID", "ASSESSMENT_CODE", "TITLE", "TYPE")
															.Values(13, ASSESSMENT_CODE, "Reticulated Water inputs", "NONFOOD");

			Database.Data.InsertInto("DXDIR_INPUT_CATEGORY").Columns("ID", "ASSESSMENT_CODE", "TITLE", "TYPE")
															.Values(14, ASSESSMENT_CODE, "Additional packaging", null);

			Database.Data.InsertInto("DXDIR_INPUT_CATEGORY").Columns("ID", "ASSESSMENT_CODE", "TITLE", "TYPE")
															.Values(15, ASSESSMENT_CODE, "Supplements", "FOOD");

			CommitGo();

			// Material Types must be inserted with current schema script

			// Input materials
			Database.Data.InsertInto("DX_MATERIAL")
					.Columns("PLANT", "MATCODE", "MAT_TYPE", "DES", "CREAT_DATE", "CREATED_BY", "MOD_DATE", "FLAGS", "STATUS", "IS_SYSTEM")
					.Values("NONE", "RES00000001", "DIR_RESOURCE", "Flours, gums spice, etc.", DbValue.DefaultTime, "DEVEX", DbValue.DefaultTime, 0, "00", 0);
			Database.Data.InsertInto("DX_MATERIAL")
					.Columns("PLANT", "MATCODE", "MAT_TYPE", "DES", "CREAT_DATE", "CREATED_BY", "MOD_DATE", "FLAGS", "STATUS", "IS_SYSTEM")
					.Values("NONE", "RES00000002", "DIR_RESOURCE", "Green grocery", DbValue.DefaultTime, "DEVEX", DbValue.DefaultTime, 0, "00", 0);
			Database.Data.InsertInto("DX_MATERIAL")
					.Columns("PLANT", "MATCODE", "MAT_TYPE", "DES", "CREAT_DATE", "CREATED_BY", "MOD_DATE", "FLAGS", "STATUS", "IS_SYSTEM")
					.Values("NONE", "RES00000003", "DIR_RESOURCE", "Grocery", DbValue.DefaultTime, "DEVEX", DbValue.DefaultTime, 0, "00", 0);
			Database.Data.InsertInto("DX_MATERIAL")
					.Columns("PLANT", "MATCODE", "MAT_TYPE", "DES", "CREAT_DATE", "CREATED_BY", "MOD_DATE", "FLAGS", "STATUS", "IS_SYSTEM")
					.Values("NONE", "RES00000004", "DIR_RESOURCE", "Meat", DbValue.DefaultTime, "DEVEX", DbValue.DefaultTime, 0, "00", 0);

			Database.Data.InsertInto("DX_MATERIAL")
					.Columns("PLANT", "MATCODE", "MAT_TYPE", "DES", "CREAT_DATE", "CREATED_BY", "MOD_DATE", "FLAGS", "STATUS", "IS_SYSTEM")
					.Values("NONE", "RES00000005", "DIR_RESOURCE", "Al trays", DbValue.DefaultTime, "DEVEX", DbValue.DefaultTime, 0, "00", 0);
			Database.Data.InsertInto("DX_MATERIAL")
					.Columns("PLANT", "MATCODE", "MAT_TYPE", "DES", "CREAT_DATE", "CREATED_BY", "MOD_DATE", "FLAGS", "STATUS", "IS_SYSTEM")
					.Values("NONE", "RES00000006", "DIR_RESOURCE", "Plastic trays and films", DbValue.DefaultTime, "DEVEX", DbValue.DefaultTime, 0, "00", 0);
			Database.Data.InsertInto("DX_MATERIAL")
					.Columns("PLANT", "MATCODE", "MAT_TYPE", "DES", "CREAT_DATE", "CREATED_BY", "MOD_DATE", "FLAGS", "STATUS", "IS_SYSTEM")
					.Values("NONE", "RES00000007", "DIR_RESOURCE", "Plastic films to cover contents", DbValue.DefaultTime, "DEVEX", DbValue.DefaultTime, 0, "00", 0);

			Database.Data.InsertInto("DX_MATERIAL")
					.Columns("PLANT", "MATCODE", "MAT_TYPE", "DES", "CREAT_DATE", "CREATED_BY", "MOD_DATE", "FLAGS", "STATUS", "IS_SYSTEM")
					.Values("NONE", "RES00000008", "DIR_RESOURCE", "Hand towels", DbValue.DefaultTime, "DEVEX", DbValue.DefaultTime, 0, "00", 0);
			Database.Data.InsertInto("DX_MATERIAL")
					.Columns("PLANT", "MATCODE", "MAT_TYPE", "DES", "CREAT_DATE", "CREATED_BY", "MOD_DATE", "FLAGS", "STATUS", "IS_SYSTEM")
					.Values("NONE", "RES00000009", "DIR_RESOURCE", "Labels", DbValue.DefaultTime, "DEVEX", DbValue.DefaultTime, 0, "00", 0);
			Database.Data.InsertInto("DX_MATERIAL")
					.Columns("PLANT", "MATCODE", "MAT_TYPE", "DES", "CREAT_DATE", "CREATED_BY", "MOD_DATE", "FLAGS", "STATUS", "IS_SYSTEM")
					.Values("NONE", "RES00000010", "DIR_RESOURCE", "Gloves", DbValue.DefaultTime, "DEVEX", DbValue.DefaultTime, 0, "00", 0);
			Database.Data.InsertInto("DX_MATERIAL")
					.Columns("PLANT", "MATCODE", "MAT_TYPE", "DES", "CREAT_DATE", "CREATED_BY", "MOD_DATE", "FLAGS", "STATUS", "IS_SYSTEM")
					.Values("NONE", "RES00000011", "DIR_RESOURCE", "Degreaser and detergent", DbValue.DefaultTime, "DEVEX", DbValue.DefaultTime, 0, "00", 0);

			Database.Data.InsertInto("DX_MATERIAL")
					.Columns("PLANT", "MATCODE", "MAT_TYPE", "DES", "CREAT_DATE", "CREATED_BY", "MOD_DATE", "FLAGS", "STATUS", "IS_SYSTEM")
					.Values("NONE", "RES00000012", "DIR_RESOURCE", "250000", DbValue.DefaultTime, "DEVEX", DbValue.DefaultTime, 0, "00", 0);

            Database.Data.InsertInto("DX_ATTVALUE_MATERIAL")
                    .Columns("PLANT", "MATCODE", "ID", "COUNTRY", "LANGUAGE", "ARRAY_INDEX", "VALUE")
                    .Values("NONE", "RES00000001", 99510, "NNN", "nn", 0, "FOOD");

            Database.Data.InsertInto("DX_ATTVALUE_MATERIAL")
                    .Columns("PLANT", "MATCODE", "ID", "COUNTRY", "LANGUAGE", "ARRAY_INDEX", "VALUE")
                    .Values("NONE", "RES00000002", 99510, "NNN", "nn", 0, "FOOD");

            Database.Data.InsertInto("DX_ATTVALUE_MATERIAL")
                    .Columns("PLANT", "MATCODE", "ID", "COUNTRY", "LANGUAGE", "ARRAY_INDEX", "VALUE")
                    .Values("NONE", "RES00000003", 99510, "NNN", "nn", 0, "FOOD");

            Database.Data.InsertInto("DX_ATTVALUE_MATERIAL")
                    .Columns("PLANT", "MATCODE", "ID", "COUNTRY", "LANGUAGE", "ARRAY_INDEX", "VALUE")
                    .Values("NONE", "RES00000004", 99510, "NNN", "nn", 0, "FOOD");

            Database.Data.InsertInto("DX_ATTVALUE_MATERIAL")
                    .Columns("PLANT", "MATCODE", "ID", "COUNTRY", "LANGUAGE", "ARRAY_INDEX", "VALUE")
                    .Values("NONE", "RES00000005", 99510, "NNN", "nn", 0, "FOOD");

            Database.Data.InsertInto("DX_ATTVALUE_MATERIAL")
                    .Columns("PLANT", "MATCODE", "ID", "COUNTRY", "LANGUAGE", "ARRAY_INDEX", "VALUE")
                    .Values("NONE", "RES00000006", 99510, "NNN", "nn", 0, "FOOD");

            Database.Data.InsertInto("DX_ATTVALUE_MATERIAL")
                    .Columns("PLANT", "MATCODE", "ID", "COUNTRY", "LANGUAGE", "ARRAY_INDEX", "VALUE")
                    .Values("NONE", "RES00000007", 99510, "NNN", "nn", 0, "FOOD");

            Database.Data.InsertInto("DX_ATTVALUE_MATERIAL")
                    .Columns("PLANT", "MATCODE", "ID", "COUNTRY", "LANGUAGE", "ARRAY_INDEX", "VALUE")
                    .Values("NONE", "RES00000008", 99510, "NNN", "nn", 0, "FOOD");

            Database.Data.InsertInto("DX_ATTVALUE_MATERIAL")
                    .Columns("PLANT", "MATCODE", "ID", "COUNTRY", "LANGUAGE", "ARRAY_INDEX", "VALUE")
                    .Values("NONE", "RES00000009", 99510, "NNN", "nn", 0, "FOOD");

            Database.Data.InsertInto("DX_ATTVALUE_MATERIAL")
                    .Columns("PLANT", "MATCODE", "ID", "COUNTRY", "LANGUAGE", "ARRAY_INDEX", "VALUE")
                    .Values("NONE", "RES00000010", 99510, "NNN", "nn", 0, "FOOD");

            Database.Data.InsertInto("DX_ATTVALUE_MATERIAL")
                    .Columns("PLANT", "MATCODE", "ID", "COUNTRY", "LANGUAGE", "ARRAY_INDEX", "VALUE")
                    .Values("NONE", "RES00000011", 99510, "NNN", "nn", 0, "FOOD");

            Database.Data.InsertInto("DX_ATTVALUE_MATERIAL")
                    .Columns("PLANT", "MATCODE", "ID", "COUNTRY", "LANGUAGE", "ARRAY_INDEX", "VALUE")
                    .Values("NONE", "RES00000012", 99510, "NNN", "nn", 0, "FOOD");

			Database.Data.InsertInto("DXDIR_ATTVALUE_ASSESSMENT")
					.Columns("CODE", "ID", "COUNTRY", "LANGUAGE", "ARRAY_INDEX", "VALUE")
					.Values("DEV_00009999", 99511, "NNN", "nn", 0, "Measured");

			CommitGo();

			Database.Data.InsertInto("DXDIR_INPUT").Columns("ID", "ASSESSMENT_CODE", "INPUT_CATEGORY_ID", "ASSESSMENT_LC_STAGE_ID", "MAT_PLANT", "MAT_CODE", "MASS", "COST", "INEDIBLE_PARTS", "MEASUREMENT", "PART_OF_PRODUCT_COPRODUCT", "CATEGORY_SORT_ORDER", "INPUT_SORT_ORDER")
							.Values(-101, ASSESSMENT_CODE, 10, LCS, "NONE", "RES00000001", DbValue.Decimal(40000), DbValue.Decimal(72000), DbValue.Decimal(0.15m), 1, 1, 1, 1);

			Database.Data.InsertInto("DXDIR_INPUT").Columns("ID", "ASSESSMENT_CODE", "INPUT_CATEGORY_ID", "ASSESSMENT_LC_STAGE_ID", "MAT_PLANT", "MAT_CODE", "MASS", "COST", "INEDIBLE_PARTS", "MEASUREMENT", "PART_OF_PRODUCT_COPRODUCT", "CATEGORY_SORT_ORDER", "INPUT_SORT_ORDER")
							.Values(-102, ASSESSMENT_CODE, 10, LCS, "NONE", "RES00000002", DbValue.Decimal(10000), DbValue.Decimal(12727.8910m), DbValue.Decimal(0.35m), 1, 1, 1, 2);

			Database.Data.InsertInto("DXDIR_INPUT").Columns("ID", "ASSESSMENT_CODE", "INPUT_CATEGORY_ID", "ASSESSMENT_LC_STAGE_ID", "MAT_PLANT", "MAT_CODE", "MASS", "COST", "INEDIBLE_PARTS", "MEASUREMENT", "PART_OF_PRODUCT_COPRODUCT", "CATEGORY_SORT_ORDER", "INPUT_SORT_ORDER")
							.Values(-103, ASSESSMENT_CODE, 10, LCS, "NONE", "RES00000003", DbValue.Decimal(15000), DbValue.Decimal(45000), DbValue.Decimal(0.20m), 1, 1, 1, 3);

			Database.Data.InsertInto("DXDIR_INPUT").Columns("ID", "ASSESSMENT_CODE", "INPUT_CATEGORY_ID", "ASSESSMENT_LC_STAGE_ID", "MAT_PLANT", "MAT_CODE", "MASS", "COST", "INEDIBLE_PARTS", "MEASUREMENT", "PART_OF_PRODUCT_COPRODUCT", "CATEGORY_SORT_ORDER", "INPUT_SORT_ORDER")
							.Values(-104, ASSESSMENT_CODE, 10, LCS, "NONE", "RES00000004", DbValue.Decimal(5000), DbValue.Decimal(38049.8247m), DbValue.Decimal(0.0m), 1, 1, 1, 4);

			Database.Data.InsertInto("DXDIR_INPUT").Columns("ID", "ASSESSMENT_CODE", "INPUT_CATEGORY_ID", "ASSESSMENT_LC_STAGE_ID", "MAT_PLANT", "MAT_CODE", "MASS", "COST", "INEDIBLE_PARTS", "MEASUREMENT", "PART_OF_PRODUCT_COPRODUCT", "CATEGORY_SORT_ORDER", "INPUT_SORT_ORDER")
							.Values(-105, ASSESSMENT_CODE, 11, LCS, "NONE", "RES00000005", DbValue.Decimal(200), DbValue.Decimal(3875.04m), null, 1, 0, 2, 1);

			Database.Data.InsertInto("DXDIR_INPUT").Columns("ID", "ASSESSMENT_CODE", "INPUT_CATEGORY_ID", "ASSESSMENT_LC_STAGE_ID", "MAT_PLANT", "MAT_CODE", "MASS", "COST", "INEDIBLE_PARTS", "MEASUREMENT", "PART_OF_PRODUCT_COPRODUCT", "CATEGORY_SORT_ORDER", "INPUT_SORT_ORDER")
							.Values(DbValue.NextVal("DXDIR_INPUT_SEQ"), ASSESSMENT_CODE, 11, LCS, "NONE", "RES00000006", DbValue.Decimal(200), DbValue.Decimal(8961.84m), DbValue.Decimal(0.05m), 1, 1, 2, 2);

			Database.Data.InsertInto("DXDIR_INPUT").Columns("ID", "ASSESSMENT_CODE", "INPUT_CATEGORY_ID", "ASSESSMENT_LC_STAGE_ID", "MAT_PLANT", "MAT_CODE", "MASS", "COST", "INEDIBLE_PARTS", "MEASUREMENT", "PART_OF_PRODUCT_COPRODUCT", "CATEGORY_SORT_ORDER", "INPUT_SORT_ORDER")
							.Values(DbValue.NextVal("DXDIR_INPUT_SEQ"), ASSESSMENT_CODE, 11, LCS, "NONE", "RES00000007", DbValue.Decimal(300), DbValue.Decimal(6984.24m), DbValue.Decimal(0.05m), 1, 1, 2, 3);

			Database.Data.InsertInto("DXDIR_INPUT").Columns("ID", "ASSESSMENT_CODE", "INPUT_CATEGORY_ID", "ASSESSMENT_LC_STAGE_ID", "MAT_PLANT", "MAT_CODE", "MASS", "COST", "INEDIBLE_PARTS", "MEASUREMENT", "PART_OF_PRODUCT_COPRODUCT", "CATEGORY_SORT_ORDER", "INPUT_SORT_ORDER")
							.Values(DbValue.NextVal("DXDIR_INPUT_SEQ"), ASSESSMENT_CODE, 12, LCS, "NONE", "RES00000008", DbValue.Decimal(300), DbValue.Decimal(1172.16m), DbValue.Decimal(1.0m), 1, 0, 3, 1);

			Database.Data.InsertInto("DXDIR_INPUT").Columns("ID", "ASSESSMENT_CODE", "INPUT_CATEGORY_ID", "ASSESSMENT_LC_STAGE_ID", "MAT_PLANT", "MAT_CODE", "MASS", "COST", "INEDIBLE_PARTS", "MEASUREMENT", "PART_OF_PRODUCT_COPRODUCT", "CATEGORY_SORT_ORDER", "INPUT_SORT_ORDER")
							.Values(DbValue.NextVal("DXDIR_INPUT_SEQ"), ASSESSMENT_CODE, 12, LCS, "NONE", "RES00000009", DbValue.Decimal(100), DbValue.Decimal(8935.82m), DbValue.Decimal(0.05m), 1, 0, 3, 2);

			Database.Data.InsertInto("DXDIR_INPUT").Columns("ID", "ASSESSMENT_CODE", "INPUT_CATEGORY_ID", "ASSESSMENT_LC_STAGE_ID", "MAT_PLANT", "MAT_CODE", "MASS", "COST", "INEDIBLE_PARTS", "MEASUREMENT", "PART_OF_PRODUCT_COPRODUCT", "CATEGORY_SORT_ORDER", "INPUT_SORT_ORDER")
							.Values(DbValue.NextVal("DXDIR_INPUT_SEQ"), ASSESSMENT_CODE, 12, LCS, "NONE", "RES00000010", DbValue.Decimal(200), DbValue.Decimal(382.56m), DbValue.Decimal(0.05m), 1, 0, 3, 3);

			Database.Data.InsertInto("DXDIR_INPUT").Columns("ID", "ASSESSMENT_CODE", "INPUT_CATEGORY_ID", "ASSESSMENT_LC_STAGE_ID", "MAT_PLANT", "MAT_CODE", "MASS", "COST", "INEDIBLE_PARTS", "MEASUREMENT", "PART_OF_PRODUCT_COPRODUCT", "CATEGORY_SORT_ORDER", "INPUT_SORT_ORDER")
							.Values(DbValue.NextVal("DXDIR_INPUT_SEQ"), ASSESSMENT_CODE, 12, LCS, "NONE", "RES00000011", DbValue.Decimal(1000), DbValue.Decimal(1764), DbValue.Decimal(1.0m), 2, 0, 3, 4);

			Database.Data.InsertInto("DXDIR_INPUT").Columns("ID", "ASSESSMENT_CODE", "INPUT_CATEGORY_ID", "ASSESSMENT_LC_STAGE_ID", "MAT_PLANT", "MAT_CODE", "MASS", "COST", "INEDIBLE_PARTS", "MEASUREMENT", "PART_OF_PRODUCT_COPRODUCT", "CATEGORY_SORT_ORDER", "INPUT_SORT_ORDER")
							.Values(DbValue.NextVal("DXDIR_INPUT_SEQ"), ASSESSMENT_CODE, 13, LCS, "NONE", "RES00000012", DbValue.Decimal(250000), DbValue.Decimal(1266), DbValue.Decimal(0.05m), 2, 0, 4, 1);

			CommitGo();

			#region DXDIR_INPUT_DESTINATION
			// Food
			Database.Data.InsertInto("DXDIR_INPUT_DESTINATION")
				.Columns("INPUT_ID", "DESTINATION_CODE", "OUTPUT_CATEGORY_ID", "PERCENTAGE")
				.Values(-101, "PRODUCT", 4, 70);

			Database.Data.InsertInto("DXDIR_INPUT_DESTINATION")
				.Columns("INPUT_ID", "DESTINATION_CODE", "OUTPUT_CATEGORY_ID", "PERCENTAGE")
				.Values(-101, "COPRODUCT", 4, 20);

			Database.Data.InsertInto("DXDIR_INPUT_DESTINATION")
				.Columns("INPUT_ID", "DESTINATION_CODE", "OUTPUT_CATEGORY_ID", "PERCENTAGE")
				.Values(-101, "FOOD_RESCUE", 4, 5);

			Database.Data.InsertInto("DXDIR_INPUT_DESTINATION")
				.Columns("INPUT_ID", "DESTINATION_CODE", "OUTPUT_CATEGORY_ID", "PERCENTAGE")
				.Values(-101, "ANIMAL_FEED", 4, 5);

			Database.Data.InsertInto("DXDIR_INPUT_DESTINATION")
				.Columns("INPUT_ID", "DESTINATION_CODE", "OUTPUT_CATEGORY_ID", "PERCENTAGE")
				.Values(-102, "PRODUCT", 4, 50);

			Database.Data.InsertInto("DXDIR_INPUT_DESTINATION")
				.Columns("INPUT_ID", "DESTINATION_CODE", "OUTPUT_CATEGORY_ID", "PERCENTAGE")
				.Values(-102, "LANDFILL", 4, 40);

			Database.Data.InsertInto("DXDIR_INPUT_DESTINATION")
				.Columns("INPUT_ID", "DESTINATION_CODE", "OUTPUT_CATEGORY_ID", "PERCENTAGE")
				.Values(-102, "ENVIRONMENT_LOSS", 4, 5);

			Database.Data.InsertInto("DXDIR_INPUT_DESTINATION")
				.Columns("INPUT_ID", "DESTINATION_CODE", "OUTPUT_CATEGORY_ID", "PERCENTAGE")
				.Values(-102, "SEWER", 4, 5);

			// Inedible parts
			// Can't have PRODUCT, COPRODUCT or FOOD_RESCUE
			Database.Data.InsertInto("DXDIR_INPUT_DESTINATION")
				.Columns("INPUT_ID", "DESTINATION_CODE", "OUTPUT_CATEGORY_ID", "PERCENTAGE")
				.Values(-101, "ANIMAL_FEED", 5, 50);

			Database.Data.InsertInto("DXDIR_INPUT_DESTINATION")
				.Columns("INPUT_ID", "DESTINATION_CODE", "OUTPUT_CATEGORY_ID", "PERCENTAGE")
				.Values(-101, "COPRODUCT", 5, 50);

			Database.Data.InsertInto("DXDIR_INPUT_DESTINATION")
				.Columns("INPUT_ID", "DESTINATION_CODE", "OUTPUT_CATEGORY_ID", "PERCENTAGE")
				.Values(-102, "LANDFILL", 5, 100);

			Database.Data.InsertInto("DXDIR_INPUT_DESTINATION")
				.Columns("INPUT_ID", "DESTINATION_CODE", "OUTPUT_CATEGORY_ID", "PERCENTAGE")
				.Values(-102, "ENVIRONMENT_LOSS", 5, 3);

			// Non Food
			Database.Data.InsertInto("DXDIR_INPUT_DESTINATION")
				.Columns("INPUT_ID", "DESTINATION_CODE", "OUTPUT_CATEGORY_ID", "PERCENTAGE")
				.Values(-105, "RECYCLING", 6, 80);

			Database.Data.InsertInto("DXDIR_INPUT_DESTINATION")
				.Columns("INPUT_ID", "DESTINATION_CODE", "OUTPUT_CATEGORY_ID", "PERCENTAGE")
				.Values(-105, "LANDFILL", 6, 15);

			Database.Data.InsertInto("DXDIR_INPUT_DESTINATION")
				.Columns("INPUT_ID", "DESTINATION_CODE", "OUTPUT_CATEGORY_ID", "PERCENTAGE")
				.Values(-105, "ENVIRONMENT_LOSS", 6, 5);

			CommitGo();

			#endregion

			#region DXDIR_BUSINESS_COST

			Database.Data.InsertInto("DXDIR_BUSINESS_COST")
                .Columns("ID", "ASSESSMENT_CODE", "ASSESSMENT_LC_STAGE_ID", "TITLE", "SORT_ORDER", "COST")
                .Values(DbValue.NextVal("DXDIR_BUSINESS_COST_SEQ"), ASSESSMENT_CODE, LCS, "RENT", 1, 1000000m);

            Database.Data.InsertInto("DXDIR_BUSINESS_COST")
                .Columns("ID", "ASSESSMENT_CODE", "ASSESSMENT_LC_STAGE_ID", "TITLE", "SORT_ORDER", "COST")
                .Values(DbValue.NextVal("DXDIR_BUSINESS_COST_SEQ"), ASSESSMENT_CODE, LCS, "WAGES", 2, 300000m);

            Database.Data.InsertInto("DXDIR_BUSINESS_COST")
                .Columns("ID", "ASSESSMENT_CODE", "ASSESSMENT_LC_STAGE_ID", "TITLE", "SORT_ORDER", "COST")
                .Values(DbValue.NextVal("DXDIR_BUSINESS_COST_SEQ"), ASSESSMENT_CODE, LCS, "MANAGEMENT", 3, 200000m);

            Database.Data.InsertInto("DXDIR_BUSINESS_COST")
                .Columns("ID", "ASSESSMENT_CODE", "ASSESSMENT_LC_STAGE_ID", "TITLE", "SORT_ORDER", "COST")
                .Values(DbValue.NextVal("DXDIR_BUSINESS_COST_SEQ"), ASSESSMENT_CODE, LCS, "FACILITIES", 4, 50000m);

            Database.Data.InsertInto("DXDIR_BUSINESS_COST")
                .Columns("ID", "ASSESSMENT_CODE", "ASSESSMENT_LC_STAGE_ID", "TITLE", "SORT_ORDER", "COST")
                .Values(DbValue.NextVal("DXDIR_BUSINESS_COST_SEQ"), ASSESSMENT_CODE, LCS, "ELECTRICITY", 5, 40000m);

            Database.Data.InsertInto("DXDIR_BUSINESS_COST")
                .Columns("ID", "ASSESSMENT_CODE", "ASSESSMENT_LC_STAGE_ID", "TITLE", "SORT_ORDER", "COST")
                .Values(DbValue.NextVal("DXDIR_BUSINESS_COST_SEQ"), ASSESSMENT_CODE, LCS, "GAS", 6, 25000m);

            Database.Data.InsertInto("DXDIR_BUSINESS_COST")
                .Columns("ID", "ASSESSMENT_CODE", "ASSESSMENT_LC_STAGE_ID", "TITLE", "SORT_ORDER", "COST")
                .Values(DbValue.NextVal("DXDIR_BUSINESS_COST_SEQ"), ASSESSMENT_CODE, LCS, "STORAGE", 7, 20000m);

            Database.Data.InsertInto("DXDIR_BUSINESS_COST")
                .Columns("ID", "ASSESSMENT_CODE", "ASSESSMENT_LC_STAGE_ID", "TITLE", "SORT_ORDER", "COST")
                .Values(DbValue.NextVal("DXDIR_BUSINESS_COST_SEQ"), ASSESSMENT_CODE, LCS, "FREIGHT", 8, 3000m);

            Database.Data.InsertInto("DXDIR_BUSINESS_COST")
                .Columns("ID", "ASSESSMENT_CODE", "ASSESSMENT_LC_STAGE_ID", "TITLE", "SORT_ORDER", "COST")
                .Values(DbValue.NextVal("DXDIR_BUSINESS_COST_SEQ"), ASSESSMENT_CODE, LCS, "WASTE", 9, 4700m);

            CommitGo();

			#endregion

            #region DXDIR_OUTPUT

            // Product
            Database.Data.InsertInto("DXDIR_OUTPUT")
                .Columns("ID", "ASSESSMENT_CODE", "OUTPUT_CATEGORY_ID", "DESTINATION_CODE", "ASSESSMENT_LC_STAGE_ID", "INPUT_ID", "OUTPUT_COST", "COST", "INCOME", "WEIGHT")
                .Values(DbValue.NextVal("DXDIR_OUTPUT_SEQ"), ASSESSMENT_CODE, 1, null, LCS, null, null, 189700m, 1000000m, 207.75m);

            // Co-product
            Database.Data.InsertInto("DXDIR_OUTPUT")
                .Columns("ID", "ASSESSMENT_CODE", "OUTPUT_CATEGORY_ID", "DESTINATION_CODE", "ASSESSMENT_LC_STAGE_ID", "INPUT_ID", "OUTPUT_COST", "COST", "INCOME", "WEIGHT")
                .Values(DbValue.NextVal("DXDIR_OUTPUT_SEQ"), ASSESSMENT_CODE, 2, null, LCS, null, null, 114325m, 10000m, 97m);

            // Food Rescue
            Database.Data.InsertInto("DXDIR_OUTPUT")
                .Columns("ID", "ASSESSMENT_CODE", "OUTPUT_CATEGORY_ID", "DESTINATION_CODE", "ASSESSMENT_LC_STAGE_ID", "INPUT_ID", "OUTPUT_COST", "COST", "INCOME", "WEIGHT")
                .Values(DbValue.NextVal("DXDIR_OUTPUT_SEQ"), ASSESSMENT_CODE, 3, null, LCS, null, null, 16250, 2000m, 16.5m);

			// (Waste) Food - Animal Feed
			Database.Data.InsertInto("DXDIR_OUTPUT")
                .Columns("ID", "ASSESSMENT_CODE", "OUTPUT_CATEGORY_ID", "DESTINATION_CODE", "ASSESSMENT_LC_STAGE_ID", "INPUT_ID", "OUTPUT_COST", "COST", "INCOME", "WEIGHT")
                .Values(DbValue.NextVal("DXDIR_OUTPUT_SEQ"), ASSESSMENT_CODE, 4, "ANIMAL_FEED", LCS, null, 1000m, null, null, null);

			Database.Data.InsertInto("DXDIR_OUTPUT")
				.Columns("ID", "ASSESSMENT_CODE", "OUTPUT_CATEGORY_ID", "DESTINATION_CODE", "ASSESSMENT_LC_STAGE_ID", "INPUT_ID", "OUTPUT_COST", "COST", "INCOME", "WEIGHT")
				.Values(DbValue.NextVal("DXDIR_OUTPUT_SEQ"), ASSESSMENT_CODE, 4, "ANIMAL_FEED", LCS, -101, null, 1700m, null, 3060);

			// (Waste) Food - Landfill
			Database.Data.InsertInto("DXDIR_OUTPUT")
                .Columns("ID", "ASSESSMENT_CODE", "OUTPUT_CATEGORY_ID", "DESTINATION_CODE", "ASSESSMENT_LC_STAGE_ID", "INPUT_ID", "OUTPUT_COST", "COST", "INCOME", "WEIGHT")
                .Values(DbValue.NextVal("DXDIR_OUTPUT_SEQ"), ASSESSMENT_CODE, 4, "LANDFILL", LCS, null, 2000m, null, null, null);

			Database.Data.InsertInto("DXDIR_OUTPUT")
				.Columns("ID", "ASSESSMENT_CODE", "OUTPUT_CATEGORY_ID", "DESTINATION_CODE", "ASSESSMENT_LC_STAGE_ID", "INPUT_ID", "OUTPUT_COST", "COST", "INCOME", "WEIGHT")
				.Values(DbValue.NextVal("DXDIR_OUTPUT_SEQ"), ASSESSMENT_CODE, 4, "LANDFILL", LCS, -102, null, 3309.2517m, null, 2600m);

			// (Waste) Food - Environmental loss
			Database.Data.InsertInto("DXDIR_OUTPUT")
                .Columns("ID", "ASSESSMENT_CODE", "OUTPUT_CATEGORY_ID", "DESTINATION_CODE", "ASSESSMENT_LC_STAGE_ID", "INPUT_ID", "OUTPUT_COST", "COST", "INCOME", "WEIGHT")
                .Values(DbValue.NextVal("DXDIR_OUTPUT_SEQ"), ASSESSMENT_CODE, 4, "ENVIRONMENT_LOSS", LCS, null, null, null, null, null);

			Database.Data.InsertInto("DXDIR_OUTPUT")
				.Columns("ID", "ASSESSMENT_CODE", "OUTPUT_CATEGORY_ID", "DESTINATION_CODE", "ASSESSMENT_LC_STAGE_ID", "INPUT_ID", "OUTPUT_COST", "COST", "INCOME", "WEIGHT")
				.Values(DbValue.NextVal("DXDIR_OUTPUT_SEQ"), ASSESSMENT_CODE, 4, "ENVIRONMENT_LOSS", LCS, -102, null, 413.6564m, null, 325m);

			// (Waste) Food - Sewer
			Database.Data.InsertInto("DXDIR_OUTPUT")
                .Columns("ID", "ASSESSMENT_CODE", "OUTPUT_CATEGORY_ID", "DESTINATION_CODE", "ASSESSMENT_LC_STAGE_ID", "INPUT_ID", "OUTPUT_COST", "COST", "INCOME", "WEIGHT")
                .Values(DbValue.NextVal("DXDIR_OUTPUT_SEQ"), ASSESSMENT_CODE, 4, "SEWER", LCS, null, null, null, null, null);

			Database.Data.InsertInto("DXDIR_OUTPUT")
				.Columns("ID", "ASSESSMENT_CODE", "OUTPUT_CATEGORY_ID", "DESTINATION_CODE", "ASSESSMENT_LC_STAGE_ID", "INPUT_ID", "OUTPUT_COST", "COST", "INCOME", "WEIGHT")
				.Values(DbValue.NextVal("DXDIR_OUTPUT_SEQ"), ASSESSMENT_CODE, 4, "SEWER", LCS, -102, null, 413.6564m, null, 325m);

			// (Waste) Inedible Parts - Animal feed
			Database.Data.InsertInto("DXDIR_OUTPUT")
                .Columns("ID", "ASSESSMENT_CODE", "OUTPUT_CATEGORY_ID", "DESTINATION_CODE", "ASSESSMENT_LC_STAGE_ID", "INPUT_ID", "OUTPUT_COST", "COST", "INCOME", "WEIGHT")
                .Values(DbValue.NextVal("DXDIR_OUTPUT_SEQ"), ASSESSMENT_CODE, 5, "ANIMAL_FEED", LCS, null, 1050, null, null, null);

			Database.Data.InsertInto("DXDIR_OUTPUT")
				.Columns("ID", "ASSESSMENT_CODE", "OUTPUT_CATEGORY_ID", "DESTINATION_CODE", "ASSESSMENT_LC_STAGE_ID", "INPUT_ID", "OUTPUT_COST", "COST", "INCOME", "WEIGHT")
				.Values(DbValue.NextVal("DXDIR_OUTPUT_SEQ"), ASSESSMENT_CODE, 5, "ANIMAL_FEED", LCS, -101, null, 540m, null, 3000m);

			// (Waste) Inedible Parts - Landfill
			Database.Data.InsertInto("DXDIR_OUTPUT")
                .Columns("ID", "ASSESSMENT_CODE", "OUTPUT_CATEGORY_ID", "DESTINATION_CODE", "ASSESSMENT_LC_STAGE_ID", "INPUT_ID", "OUTPUT_COST", "COST", "INCOME", "WEIGHT")
                .Values(DbValue.NextVal("DXDIR_OUTPUT_SEQ"), ASSESSMENT_CODE, 5, "LANDFILL", LCS, null, 2050, null, null, null);

			Database.Data.InsertInto("DXDIR_OUTPUT")
				.Columns("ID", "ASSESSMENT_CODE", "OUTPUT_CATEGORY_ID", "DESTINATION_CODE", "ASSESSMENT_LC_STAGE_ID", "INPUT_ID", "OUTPUT_COST", "COST", "INCOME", "WEIGHT")
				.Values(DbValue.NextVal("DXDIR_OUTPUT_SEQ"), ASSESSMENT_CODE, 5, "LANDFILL", LCS, -102, null, 4454.7618, null, 3500m);

			// (Waste) Inedible Parts - Environmental loss
			Database.Data.InsertInto("DXDIR_OUTPUT")
                .Columns("ID", "ASSESSMENT_CODE", "OUTPUT_CATEGORY_ID", "DESTINATION_CODE", "ASSESSMENT_LC_STAGE_ID", "INPUT_ID", "OUTPUT_COST", "COST", "INCOME", "WEIGHT")
                .Values(DbValue.NextVal("DXDIR_OUTPUT_SEQ"), ASSESSMENT_CODE, 5, "ENVIRONMENT_LOSS", LCS, null, null, null, null, null);

			Database.Data.InsertInto("DXDIR_OUTPUT")
				.Columns("ID", "ASSESSMENT_CODE", "OUTPUT_CATEGORY_ID", "DESTINATION_CODE", "ASSESSMENT_LC_STAGE_ID", "INPUT_ID", "OUTPUT_COST", "COST", "INCOME", "WEIGHT")
				.Values(DbValue.NextVal("DXDIR_OUTPUT_SEQ"), ASSESSMENT_CODE, 5, "ENVIRONMENT_LOSS", LCS, -102, null, 133.6428m, null, 105m);

			// (Waste) Non Food - Recycling
			Database.Data.InsertInto("DXDIR_OUTPUT")
                .Columns("ID", "ASSESSMENT_CODE", "OUTPUT_CATEGORY_ID", "DESTINATION_CODE", "ASSESSMENT_LC_STAGE_ID", "INPUT_ID", "OUTPUT_COST", "COST", "INCOME", "WEIGHT")
                .Values(DbValue.NextVal("DXDIR_OUTPUT_SEQ"), ASSESSMENT_CODE, 6, "RECYCLING", LCS, null, 3100, null, null, null);

			Database.Data.InsertInto("DXDIR_OUTPUT")
				.Columns("ID", "ASSESSMENT_CODE", "OUTPUT_CATEGORY_ID", "DESTINATION_CODE", "ASSESSMENT_LC_STAGE_ID", "INPUT_ID", "OUTPUT_COST", "COST", "INCOME", "WEIGHT")
				.Values(DbValue.NextVal("DXDIR_OUTPUT_SEQ"), ASSESSMENT_CODE, 6, "RECYCLING", LCS, -105, null, 3100.032m, null, 160m);

			// (Waste) Non Food - Landfill
			Database.Data.InsertInto("DXDIR_OUTPUT")
                .Columns("ID", "ASSESSMENT_CODE", "OUTPUT_CATEGORY_ID", "DESTINATION_CODE", "ASSESSMENT_LC_STAGE_ID", "INPUT_ID", "OUTPUT_COST", "COST", "INCOME", "WEIGHT")
                .Values(DbValue.NextVal("DXDIR_OUTPUT_SEQ"), ASSESSMENT_CODE, 6, "LANDFILL", LCS, null, 3200, null, null, null);

			Database.Data.InsertInto("DXDIR_OUTPUT")
				.Columns("ID", "ASSESSMENT_CODE", "OUTPUT_CATEGORY_ID", "DESTINATION_CODE", "ASSESSMENT_LC_STAGE_ID", "INPUT_ID", "OUTPUT_COST", "COST", "INCOME", "WEIGHT")
				.Values(DbValue.NextVal("DXDIR_OUTPUT_SEQ"), ASSESSMENT_CODE, 6, "LANDFILL", LCS, -105, null, 581.256m, null, 30m);

			// (Waste) Non Food - Enviromental Loss
			Database.Data.InsertInto("DXDIR_OUTPUT")
                .Columns("ID", "ASSESSMENT_CODE", "OUTPUT_CATEGORY_ID", "DESTINATION_CODE", "ASSESSMENT_LC_STAGE_ID", "INPUT_ID", "OUTPUT_COST", "COST", "INCOME", "WEIGHT")
                .Values(DbValue.NextVal("DXDIR_OUTPUT_SEQ"), ASSESSMENT_CODE, 6, "ENVIRONMENT_LOSS", LCS, null, null, null, null, null);

			Database.Data.InsertInto("DXDIR_OUTPUT")
				.Columns("ID", "ASSESSMENT_CODE", "OUTPUT_CATEGORY_ID", "DESTINATION_CODE", "ASSESSMENT_LC_STAGE_ID", "INPUT_ID", "OUTPUT_COST", "COST", "INCOME", "WEIGHT")
				.Values(DbValue.NextVal("DXDIR_OUTPUT_SEQ"), ASSESSMENT_CODE, 6, "ENVIRONMENT_LOSS", LCS, -105, null, 193.752m, null, 10m);

			#endregion
		}

		public override void Down()
		{
			throw new NotImplementedException();
		}
	}
}
