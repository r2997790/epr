using System;
using Selerant.ApplicationBlocks.Data;
using Selerant.ApplicationBlocks.Data.Migrations;

namespace Selerant.DevEx.Modules.DIRModule.DataBase.UpgradeSchema.Migration_00087
{
	public class Migration : MigrationBase
	{
		public override void Up()
		{
			// CREATE all Foreign Keys, required only for MSSQL.
			if (Context.ProviderType == DataProviderType.SqlServer)
			{
				// REMOVE Foreign Keys, required only for MSSQL.
				Database.Alter.Table("DXDIR_LC_STAGE_TMPL").Drop.Constraint("DXDIR_LC_STAGE_TMPL_FK1");
				Database.Alter.Table("DXDIR_INPUT_CATEGORY_TMPL").Drop.Constraint("DXDIR_INPUT_CATEGORY_TMPL_FK1");
				Database.Alter.Table("DXDIR_BUSINESS_COST_TMPL").Drop.Constraint("DXDIR_BUSINESS_COST_TMPL_FK1");
				Database.Alter.Table("DXDIR_ASSESSMENT").Drop.Constraint("DXDIR_ASSESSMENT_FK1");
				Database.Alter.Table("DXDIR_ASSESSMENT").Drop.Constraint("DXDIR_ASSESSMENT_FK2");
				Database.Alter.Table("DXDIR_ASSESSMENT").Drop.Constraint("DXDIR_ASSESSMENT_FK3");
				Database.Alter.Table("DXDIR_ASSESSMENT").Drop.Constraint("DXDIR_ASSESSMENT_FK4");
				Database.Alter.Table("DXDIR_ASSESSMENT").Drop.Constraint("DXDIR_ASSESSMENT_FK5");
				Database.Alter.Table("DXDIR_ASSESSMENT").Drop.Constraint("DXDIR_ASSESSMENT_FK6");
				Database.Alter.Table("DXDIR_ATTVALUE_ASSESSMENT").Drop.Constraint("DXDIR_ATTVALUE_ASSESSMENT_FK1");
				Database.Alter.Table("DXDIR_ATTVALUE_ASSESSMENT").Drop.Constraint("DXDIR_ATTVALUE_ASSESSMENT_FK2");
				Database.Alter.Table("DXDIR_ATTVALUE_ASSESSMENT").Drop.Constraint("DXDIR_ATTVALUE_ASSESSMENT_FK3");
				Database.Alter.Table("DXDIR_ATTVALUE_ASSESSMENT").Drop.Constraint("DXDIR_ATTVALUE_ASSESSMENT_FK4");
				Database.Alter.Table("DXDIR_ASSESSMENT_LC_STAGE").Drop.Constraint("DXDIR_ASSESSMENT_LC_STAGE_FK1");
				Database.Alter.Table("DXDIR_ASSESSMENT_LC_STAGE").Drop.Constraint("DXDIR_ASSESSMENT_LC_STAGE_FK2");
				Database.Alter.Table("DXDIR_INPUT_CATEGORY").Drop.Constraint("DXDIR_INPUT_CATEGORY_FK1");
				Database.Alter.Table("DXDIR_INPUT").Drop.Constraint("DXDIR_INPUT_FK1");
				Database.Alter.Table("DXDIR_INPUT").Drop.Constraint("DXDIR_INPUT_FK2");
				Database.Alter.Table("DXDIR_INPUT").Drop.Constraint("DXDIR_INPUT_FK3");
				Database.Alter.Table("DXDIR_INPUT").Drop.Constraint("DXDIR_INPUT_FK4");
				Database.Alter.Table("DXDIR_INPUT_PROD_COPROD_SPREAD").Drop.Constraint("DXDIR_PK_INPUT_PCP_SPREAD_FK1");
				Database.Alter.Table("DXDIR_INPUT_PROD_COPROD_SPREAD").Drop.Constraint("DXDIR_PK_INPUT_PCP_SPREAD_FK2");
				Database.Alter.Table("DXDIR_OUTPUT").Drop.Constraint("DXDIR_OUTPUT_FK1");
				Database.Alter.Table("DXDIR_OUTPUT").Drop.Constraint("DXDIR_OUTPUT_FK2");
				Database.Alter.Table("DXDIR_OUTPUT").Drop.Constraint("DXDIR_OUTPUT_FK3");
				Database.Alter.Table("DXDIR_OUTPUT").Drop.Constraint("DXDIR_OUTPUT_FK4");
				Database.Alter.Table("DXDIR_OUTPUT").Drop.Constraint("DXDIR_OUTPUT_FK5");
				Database.Alter.Table("DXDIR_OUTPUT").Drop.Constraint("DXDIR_OUTPUT_FK6");
				Database.Alter.Table("DXDIR_BUSINESS_COST").Drop.Constraint("DXDIR_BUSINESS_COST_FK1");
				Database.Alter.Table("DXDIR_BUSINESS_COST").Drop.Constraint("DXDIR_BUSINESS_COST_FK2");
				Database.Alter.Table("DXDIR_RESULT").Drop.Constraint("DXDIR_RESULT_FK1");
				Database.Alter.Table("DXDIR_RESULT").Drop.Constraint("DXDIR_RESULT_FK2");
				Database.Alter.Table("DXDIR_RESULT").Drop.Constraint("DXDIR_RESULT_FK3");
				Database.Alter.Table("DXDIR_PARTNER_ASSESSMENT").Drop.Constraint("DXDIR_PARTNER_ASSESSMENT_FK1");
				Database.Alter.Table("DXDIR_PARTNER_ASSESSMENT").Drop.Constraint("DXDIR_PARTNER_ASSESSMENT_FK2");
				Database.Alter.Table("DXDIR_PARTNER_ASSESSMENT_TYPE").Drop.Constraint("DXDIR_PARTNER_ASMT_TYPE_FK1");
				Database.Alter.Table("DXDIR_PARTNER_ASSESSMENT_TYPE").Drop.Constraint("DXDIR_PARTNER_ASMT_TYPE_FK2");
				Database.Alter.Table("DXDIR_RECENT_ASSESSMENT").Drop.Constraint("DX_RECENT_ASSESSMENT_FK1");
				Database.Alter.Table("DXDIR_RECENT_ASSESSMENT").Drop.Constraint("DX_RECENT_ASSESSMENT_FK2");
				Database.Alter.Table("DXDIR_ASSESSMENT_DESTINATION").Drop.Constraint("DXDIR_ASSESSMENT_DEST_FK1");
				Database.Alter.Table("DXDIR_ASSESSMENT_DESTINATION").Drop.Constraint("DXDIR_ASSESSMENT_DEST_FK2");
				Database.Alter.Table("DXDIR_INPUT_DESTINATION").Drop.Constraint("DXDIR_INPUT_DESTINATION_FK1");
				Database.Alter.Table("DXDIR_INPUT_DESTINATION").Drop.Constraint("DXDIR_INPUT_DESTINATION_FK2");
				Database.Alter.Table("DXDIR_INPUT_DESTINATION").Drop.Constraint("DXDIR_INPUT_DESTINATION_FK3");
				Database.Alter.Table("DXDIR_RESOURCE_NOTE").Drop.Constraint("DXDIR_RESOURCE_NOTE_FK1");
				Database.Alter.Table("DXDIR_RESOURCE_NOTE").Drop.Constraint("DXDIR_RESOURCE_NOTE_FK2");


				// CREATE Foreign Keys, required only for MSSQL.
				Database.Alter.Table("DXDIR_LC_STAGE_TMPL").Add
						.Constraint.ForeignKey("DXDIR_LC_STAGE_TMPL_FK1").Keys("ASSESSMENT_TYPE_CODE").References("DXDIR_ASSESSMENT_TYPE").Columns("CODE").OnDelete.Cascade();

				Database.Alter.Table("DXDIR_INPUT_CATEGORY_TMPL").Add
						.Constraint.ForeignKey("DXDIR_INPUT_CATEGORY_TMPL_FK1").Keys("ASSESSMENT_TYPE_CODE").References("DXDIR_ASSESSMENT_TYPE").Columns("CODE").OnDelete.Cascade();

				Database.Alter.Table("DXDIR_BUSINESS_COST_TMPL").Add
						.Constraint.ForeignKey("DXDIR_BUSINESS_COST_TMPL_FK1").Keys("ASSESSMENT_TYPE_CODE").References("DXDIR_ASSESSMENT_TYPE").Columns("CODE").OnDelete.Cascade();

				Database.Alter.Table("DXDIR_ASSESSMENT").Add
						.Constraint.ForeignKey("DXDIR_ASSESSMENT_FK1").Keys("TYPE_CODE").References("DXDIR_ASSESSMENT_TYPE").Columns("CODE").OnDelete.Cascade()
						.Constraint.ForeignKey("DXDIR_ASSESSMENT_FK2").Keys("COMPLETING_BY").References("DX_USER").Columns("ID").OnDelete.Cascade()
						.Constraint.ForeignKey("DXDIR_ASSESSMENT_FK3").Keys("CURRENCY").References("DX_CURRENCY").Columns("CODE")
						.Constraint.ForeignKey("DXDIR_ASSESSMENT_FK4").Keys("CREATED_BY").References("DX_USER").Columns("ID").OnDelete.Cascade()
						.Constraint.ForeignKey("DXDIR_ASSESSMENT_FK5").Keys("MODIFIED_BY").References("DX_USER").Columns("ID").OnDelete.SetNull()
						.Constraint.ForeignKey("DXDIR_ASSESSMENT_FK6").Keys("AUTHORIZATION_ROLE").References("DX_AUTHORIZATION_ROLE").Columns("ID");

				Database.Alter.Table("DXDIR_ATTVALUE_ASSESSMENT").Add
						.Constraint.ForeignKey("DXDIR_ATTVALUE_ASSESSMENT_FK1").Keys("CODE").References("DXDIR_ASSESSMENT").Columns("CODE").OnDelete.Cascade()
						.Constraint.ForeignKey("DXDIR_ATTVALUE_ASSESSMENT_FK2").Keys("ID").References("DX_ATTRIBUTE_DEF").Columns("ID")
						.Constraint.ForeignKey("DXDIR_ATTVALUE_ASSESSMENT_FK3").Keys("LANGUAGE").References("DX_LANGUAGE").Columns("CODE")
						.Constraint.ForeignKey("DXDIR_ATTVALUE_ASSESSMENT_FK4").Keys("COUNTRY").References("DX_COUNTRY").Columns("CODE");

				Database.Alter.Table("DXDIR_ASSESSMENT_LC_STAGE").Add
						.Constraint.ForeignKey("DXDIR_ASSESSMENT_LC_STAGE_FK1").Keys("ASSESSMENT_CODE").References("DXDIR_ASSESSMENT").Columns("CODE").OnDelete.Cascade()
						.Constraint.ForeignKey("DXDIR_ASSESSMENT_LC_STAGE_FK2").Keys("SOURCE_ASMT_LC_STAGE_ID").References("DXDIR_ASSESSMENT_LC_STAGE").Columns("ID").OnDelete.SetNull();

				Database.Alter.Table("DXDIR_INPUT_CATEGORY").Add
						.Constraint.ForeignKey("DXDIR_INPUT_CATEGORY_FK1").Keys("ASSESSMENT_CODE").References("DXDIR_ASSESSMENT").Columns("CODE").OnDelete.Cascade();

				Database.Alter.Table("DXDIR_INPUT").Add
						.Constraint.ForeignKey("DXDIR_INPUT_FK1").Keys("ASSESSMENT_CODE").References("DXDIR_ASSESSMENT").Columns("CODE").OnDelete.Cascade()
						.Constraint.ForeignKey("DXDIR_INPUT_FK2").Keys("INPUT_CATEGORY_ID").References("DXDIR_INPUT_CATEGORY").Columns("ID").OnDelete.Cascade()
						.Constraint.ForeignKey("DXDIR_INPUT_FK3").Keys("ASSESSMENT_LC_STAGE_ID").References("DXDIR_ASSESSMENT_LC_STAGE").Columns("ID").OnDelete.Cascade()
						.Constraint.ForeignKey("DXDIR_INPUT_FK4").Keys("MAT_PLANT", "MAT_CODE").References("DX_MATERIAL").Columns("PLANT", "MATCODE");

				Database.Alter.Table("DXDIR_INPUT_PROD_COPROD_SPREAD").Add
						.Constraint.ForeignKey("DXDIR_PK_INPUT_PCP_SPREAD_FK1").Keys("INPUT_ID").References("DXDIR_INPUT").Columns("ID").OnDelete.Cascade()
						.Constraint.ForeignKey("DXDIR_PK_INPUT_PCP_SPREAD_FK2").Keys("DESTINATION_CODE").References("DXDIR_DESTINATION").Columns("CODE").OnDelete.Cascade();

				Database.Alter.Table("DXDIR_OUTPUT").Add
						.Constraint.ForeignKey("DXDIR_OUTPUT_FK1").Keys("ASSESSMENT_CODE").References("DXDIR_ASSESSMENT").Columns("CODE").OnDelete.Cascade()
						.Constraint.ForeignKey("DXDIR_OUTPUT_FK2").Keys("OUTPUT_CATEGORY_ID").References("DXDIR_OUTPUT_CATEGORY").Columns("ID").OnDelete.Cascade()
						.Constraint.ForeignKey("DXDIR_OUTPUT_FK3").Keys("DESTINATION_CODE").References("DXDIR_DESTINATION").Columns("CODE").OnDelete.Cascade()
						.Constraint.ForeignKey("DXDIR_OUTPUT_FK4").Keys("ASSESSMENT_LC_STAGE_ID").References("DXDIR_ASSESSMENT_LC_STAGE").Columns("ID").OnDelete.Cascade()
						.Constraint.ForeignKey("DXDIR_OUTPUT_FK5").Keys("INPUT_ID").References("DXDIR_INPUT").Columns("ID").OnDelete.Cascade()
						.Constraint.ForeignKey("DXDIR_OUTPUT_FK6").Keys("MAT_PLANT", "MAT_CODE").References("DX_MATERIAL").Columns("PLANT", "MATCODE");

				Database.Alter.Table("DXDIR_BUSINESS_COST").Add
						.Constraint.ForeignKey("DXDIR_BUSINESS_COST_FK1").Keys("ASSESSMENT_CODE").References("DXDIR_ASSESSMENT").Columns("CODE").OnDelete.Cascade()
						.Constraint.ForeignKey("DXDIR_BUSINESS_COST_FK2").Keys("ASSESSMENT_LC_STAGE_ID").References("DXDIR_ASSESSMENT_LC_STAGE").Columns("ID").OnDelete.Cascade();

				Database.Alter.Table("DXDIR_RESULT").Add
						.Constraint.ForeignKey("DXDIR_RESULT_FK1").Keys("ASSESSMENT_CODE").References("DXDIR_ASSESSMENT").Columns("CODE").OnDelete.Cascade()
						.Constraint.ForeignKey("DXDIR_RESULT_FK2").Keys("ASSESSMENT_LC_STAGE_ID").References("DXDIR_ASSESSMENT_LC_STAGE").Columns("ID").OnDelete.Cascade()
						.Constraint.ForeignKey("DXDIR_RESULT_FK3").Keys("RESULT_ROW_ID").References("DXDIR_RESULT_ROW").Columns("ID").OnDelete.Cascade();

				Database.Alter.Table("DXDIR_PARTNER_ASSESSMENT").Add
						.Constraint.ForeignKey("DXDIR_PARTNER_ASSESSMENT_FK1").Keys("ASSESSMENT_CODE").References("DXDIR_ASSESSMENT").Columns("CODE").OnDelete.Cascade()
						.Constraint.ForeignKey("DXDIR_PARTNER_ASSESSMENT_FK2").Keys("PARTNER_ORG_CODE").References("DX_ORGANIZATION").Columns("CODE").OnDelete.Cascade();

				Database.Alter.Table("DXDIR_PARTNER_ASSESSMENT_TYPE").Add
						.Constraint.ForeignKey("DXDIR_PARTNER_ASMT_TYPE_FK1").Keys("ASSESSMENT_TYPE_CODE").References("DXDIR_ASSESSMENT_TYPE").Columns("CODE").OnDelete.Cascade()
						.Constraint.ForeignKey("DXDIR_PARTNER_ASMT_TYPE_FK2").Keys("PARTNER_ORG_CODE").References("DX_ORGANIZATION").Columns("CODE").OnDelete.Cascade();

				Database.Alter.Table("DXDIR_RECENT_ASSESSMENT").Add
						.Constraint.ForeignKey("DX_RECENT_ASSESSMENT_FK1").Keys("USER_ID").References("DX_USER").Columns("ID").OnDelete.Cascade()
						.Constraint.ForeignKey("DX_RECENT_ASSESSMENT_FK2").Keys("CODE").References("DXDIR_ASSESSMENT").Columns("CODE").OnDelete.Cascade();

				Database.Alter.Table("DXDIR_ASSESSMENT_DESTINATION").Add
						.Constraint.ForeignKey("DXDIR_ASSESSMENT_DEST_FK1").Keys("ASSESSMENT_CODE").References("DXDIR_ASSESSMENT").Columns("CODE").OnDelete.Cascade()
						.Constraint.ForeignKey("DXDIR_ASSESSMENT_DEST_FK2").Keys("DESTINATION_CODE").References("DXDIR_DESTINATION").Columns("CODE").OnDelete.Cascade();

				Database.Alter.Table("DXDIR_INPUT_DESTINATION").Add
						.Constraint.ForeignKey("DXDIR_INPUT_DESTINATION_FK1").Keys("INPUT_ID").References("DXDIR_INPUT").Columns("ID").OnDelete.Cascade()
						.Constraint.ForeignKey("DXDIR_INPUT_DESTINATION_FK2").Keys("DESTINATION_CODE").References("DXDIR_DESTINATION").Columns("CODE").OnDelete.Cascade()
						.Constraint.ForeignKey("DXDIR_INPUT_DESTINATION_FK3").Keys("OUTPUT_CATEGORY_ID").References("DXDIR_OUTPUT_CATEGORY").Columns("ID").OnDelete.Cascade();

				Database.Alter.Table("DXDIR_RESOURCE_NOTE").Add
						.Constraint.ForeignKey("DXDIR_RESOURCE_NOTE_FK1").Keys("ASSESSMENT_CODE").References("DXDIR_ASSESSMENT").Columns("CODE").OnDelete.Cascade()
						.Constraint.ForeignKey("DXDIR_RESOURCE_NOTE_FK2").Keys("LC_STAGE_ID").References("DXDIR_ASSESSMENT_LC_STAGE").Columns("ID").OnDelete.Cascade();
			}
		}

		public override void Down()
		{
			throw new NotImplementedException();
		}
	}
}
