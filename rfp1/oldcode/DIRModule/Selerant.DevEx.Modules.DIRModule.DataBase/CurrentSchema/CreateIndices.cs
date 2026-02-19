using System;
using Selerant.ApplicationBlocks.Data.Migrations;
using Selerant.ApplicationBlocks.Data.Migrations.Operations.Schema;

namespace Selerant.DevEx.Modules.DIRModule.DataBase.CurrentSchema
{
	public class CreateIndices : MigrationBase
	{
		public override void Up()
		{
			// Predefine data tables
			Database.Create.Index("DXDIR_LC_STAGE_TMPL_IDX").OnTable("DXDIR_LC_STAGE_TMPL").OnColumns("ASSESSMENT_TYPE_CODE").IndexType(IndexType.Ordinary);

			Database.Create.Index("DXDIR_INPUT_CATEGORY_TMPL_IDX1").OnTable("DXDIR_INPUT_CATEGORY_TMPL").OnColumns("ASSESSMENT_TYPE_CODE").IndexType(IndexType.Ordinary);

			Database.Create.Index("DXDIR_BUSINESS_COST_TMPL_IDX1").OnTable("DXDIR_BUSINESS_COST_TMPL").OnColumns("ASSESSMENT_TYPE_CODE").IndexType(IndexType.Ordinary);
			// end Predefine data tables

			Database.Create.Index("DXDIR_ASSESSMENT_IDX1").OnTable("DXDIR_ASSESSMENT").OnColumns("TYPE_CODE").IndexType(IndexType.Ordinary);
			Database.Create.Index("DXDIR_ASSESSMENT_IDX2").OnTable("DXDIR_ASSESSMENT").OnColumns("COMPLETING_BY").IndexType(IndexType.Ordinary);
			Database.Create.Index("DXDIR_ASSESSMENT_IDX3").OnTable("DXDIR_ASSESSMENT").OnColumns("CURRENCY").IndexType(IndexType.Ordinary);
			Database.Create.Index("DXDIR_ASSESSMENT_IDX4").OnTable("DXDIR_ASSESSMENT").OnColumns("CREATED_BY").IndexType(IndexType.Ordinary);
			Database.Create.Index("DXDIR_ASSESSMENT_IDX5").OnTable("DXDIR_ASSESSMENT").OnColumns("MODIFIED_BY").IndexType(IndexType.Ordinary);
			Database.Create.Index("DXDIR_ASSESSMENT_IDX6").OnTable("DXDIR_ASSESSMENT").OnColumns("AUTHORIZATION_ROLE").IndexType(IndexType.Ordinary);
			Database.Create.Index("DXDIR_ASSESSMENT_IDX7").OnTable("DXDIR_ASSESSMENT").OnColumns("LOCATION").IndexType(IndexType.Ordinary);

			Database.Create.Index("DXDIR_ATTVALUE_ASSESSMENT_IDX1").OnTable("DXDIR_ATTVALUE_ASSESSMENT").OnColumns("CODE").IndexType(IndexType.Ordinary);
			Database.Create.Index("DXDIR_ATTVALUE_ASSESSMENT_IDX2").OnTable("DXDIR_ATTVALUE_ASSESSMENT").OnColumns("ID").IndexType(IndexType.Ordinary);
			Database.Create.Index("DXDIR_ATTVALUE_ASSESSMENT_IDX3").OnTable("DXDIR_ATTVALUE_ASSESSMENT").OnColumns("COUNTRY").IndexType(IndexType.Ordinary);
			Database.Create.Index("DXDIR_ATTVALUE_ASSESSMENT_IDX4").OnTable("DXDIR_ATTVALUE_ASSESSMENT").OnColumns("LANGUAGE").IndexType(IndexType.Ordinary);

			Database.Create.Index("DXDIR_ASSESSMENT_LC_STAGE_IDX1").OnTable("DXDIR_ASSESSMENT_LC_STAGE").OnColumns("ASSESSMENT_CODE").IndexType(IndexType.Ordinary);
			Database.Create.Index("DXDIR_ASSESSMENT_LC_STAGE_IDX2").OnTable("DXDIR_ASSESSMENT_LC_STAGE").OnColumns("SOURCE_ASMT_LC_STAGE_ID").IndexType(IndexType.Ordinary);

			Database.Create.Index("DXDIR_INPUT_CATEGORY_IDX1").OnTable("DXDIR_INPUT_CATEGORY").OnColumns("ASSESSMENT_CODE").IndexType(IndexType.Ordinary);
			Database.Create.Index("DXDIR_INPUT_CATEGORY_IDX2").OnTable("DXDIR_INPUT_CATEGORY").OnColumns("TYPE").IndexType(IndexType.Ordinary);

			Database.Create.Index("DXDIR_OUTPUT_CATEGORY_IDX1").OnTable("DXDIR_OUTPUT_CATEGORY").OnColumns("TYPE").IndexType(IndexType.Ordinary);

			Database.Create.Index("DXDIR_DESTINATION_IDX1").OnTable("DXDIR_DESTINATION").OnColumns("USED_ON").IndexType(IndexType.Ordinary);

			Database.Create.Index("DXDIR_INPUT_IDX1").OnTable("DXDIR_INPUT").OnColumns("ASSESSMENT_CODE").IndexType(IndexType.Ordinary);
			Database.Create.Index("DXDIR_INPUT_IDX2").OnTable("DXDIR_INPUT").OnColumns("INPUT_CATEGORY_ID").IndexType(IndexType.Ordinary);
			Database.Create.Index("DXDIR_INPUT_IDX3").OnTable("DXDIR_INPUT").OnColumns("ASSESSMENT_LC_STAGE_ID").IndexType(IndexType.Ordinary);
			Database.Create.Index("DXDIR_INPUT_IDX4").OnTable("DXDIR_INPUT").OnColumns("MAT_PLANT", "MAT_CODE").IndexType(IndexType.Ordinary);

			Database.Create.Index("DXDIR_OUTPUT_IDX1").OnTable("DXDIR_OUTPUT").OnColumns("ASSESSMENT_CODE").IndexType(IndexType.Ordinary);
			Database.Create.Index("DXDIR_OUTPUT_IDX2").OnTable("DXDIR_OUTPUT").OnColumns("OUTPUT_CATEGORY_ID").IndexType(IndexType.Ordinary);
			Database.Create.Index("DXDIR_OUTPUT_IDX3").OnTable("DXDIR_OUTPUT").OnColumns("DESTINATION_CODE").IndexType(IndexType.Ordinary);
			Database.Create.Index("DXDIR_OUTPUT_IDX4").OnTable("DXDIR_OUTPUT").OnColumns("ASSESSMENT_LC_STAGE_ID").IndexType(IndexType.Ordinary);
			Database.Create.Index("DXDIR_OUTPUT_IDX5").OnTable("DXDIR_OUTPUT").OnColumns("INPUT_ID").IndexType(IndexType.Ordinary);
			Database.Create.Index("DXDIR_OUTPUT_IDX6").OnTable("DXDIR_OUTPUT").OnColumns("MAT_PLANT", "MAT_CODE").IndexType(IndexType.Ordinary);

			Database.Create.Index("DXDIR_BUSINESS_COST_IDX1").OnTable("DXDIR_BUSINESS_COST").OnColumns("ASSESSMENT_CODE").IndexType(IndexType.Ordinary);
			Database.Create.Index("DXDIR_BUSINESS_COST_IDX2").OnTable("DXDIR_BUSINESS_COST").OnColumns("ASSESSMENT_LC_STAGE_ID").IndexType(IndexType.Ordinary);

			Database.Create.Index("DXDIR_RESULT_IDX1").OnTable("DXDIR_RESULT").OnColumns("ASSESSMENT_CODE").IndexType(IndexType.Ordinary);
			Database.Create.Index("DXDIR_RESULT_IDX2").OnTable("DXDIR_RESULT").OnColumns("ASSESSMENT_LC_STAGE_ID").IndexType(IndexType.Ordinary);
			Database.Create.Index("DXDIR_RESULT_IDX3").OnTable("DXDIR_RESULT").OnColumns("RESULT_ROW_ID").IndexType(IndexType.Ordinary);

			Database.Create.Index("DXDIR_PARTNER_ASSESSMENT_IDX").OnTable("DXDIR_PARTNER_ASSESSMENT").OnColumns("PARTNER_ORG_CODE").IndexType(IndexType.Ordinary);

			Database.Create.Index("DXDIR_RECENT_ASSESSMENT_IDX").OnTable("DXDIR_RECENT_ASSESSMENT").OnColumns("CODE").IndexType(IndexType.Ordinary);
		
            Database.Create.Index("DXDIR_PARTNER_ASMT_TYPE_IDX").OnTable("DXDIR_PARTNER_ASSESSMENT_TYPE").OnColumns("PARTNER_ORG_CODE").IndexType(IndexType.Ordinary);

			Database.Create.Index("DXDIR_ASSESSMENT_DEST_IDX1").OnTable("DXDIR_ASSESSMENT_DESTINATION").OnColumns("ASSESSMENT_CODE").IndexType(IndexType.Ordinary);
            Database.Create.Index("DXDIR_ASSESSMENT_DEST_IDX2").OnTable("DXDIR_ASSESSMENT_DESTINATION").OnColumns("DESTINATION_CODE").IndexType(IndexType.Ordinary);

            Database.Create.Index("DXDIR_INPUT_DES_IDX1").OnTable("DXDIR_INPUT_DESTINATION").OnColumns("INPUT_ID").IndexType(IndexType.Ordinary);
            Database.Create.Index("DXDIR_INPUT_DES_IDX2").OnTable("DXDIR_INPUT_DESTINATION").OnColumns("DESTINATION_CODE").IndexType(IndexType.Ordinary);
            Database.Create.Index("DXDIR_INPUT_DES_IDX3").OnTable("DXDIR_INPUT_DESTINATION").OnColumns("OUTPUT_CATEGORY_ID").IndexType(IndexType.Ordinary);

			Database.Create.Index("DXDIR_RESOURCE_NOTE_IDX1").OnTable("DXDIR_RESOURCE_NOTE").OnColumns("ASSESSMENT_CODE").IndexType(IndexType.Ordinary);
			Database.Create.Index("DXDIR_RESOURCE_NOTE_IDX2").OnTable("DXDIR_RESOURCE_NOTE").OnColumns("LC_STAGE_ID").IndexType(IndexType.Ordinary);
			Database.Create.Index("DXDIR_RESOURCE_NOTE_IDX3").OnTable("DXDIR_RESOURCE_NOTE").OnColumns("TYPE").IndexType(IndexType.Ordinary);
		}

		public override void Down()
		{
			throw new NotImplementedException();
		}
	}
}
