using Selerant.ApplicationBlocks.Data.Migrations;

namespace Selerant.DevEx.Modules.DIRModule.DataBase.CurrentSchema
{
	public class Populate_Data : MigrationBase
	{
		public override void Up()
		{
            Database.Data.FromScript(@"PopulateData\DX_ATTRIBUTE_DEF.cs");
            Database.Data.FromScript(@"PopulateData\DX_COMPONENT_CONTROLLER.cs");
			Database.Data.FromScript(@"PopulateData\DX_MATERIAL_TYPE.cs");
			Database.Data.FromScript(@"PopulateData\DX_MATERIAL.cs");
			Database.Data.FromScript(@"PopulateData\DX_PARTNER_MATERIAL.cs");
			Database.Data.FromScript(@"PopulateData\DX_PHRASE_TYPE.cs");
            Database.Data.FromScript(@"PopulateData\DX_PHRASE_DEF.cs");
			Database.Data.FromScript(@"PopulateData\DX_PHRASE_TEXT.cs");
			Database.Data.FromScript(@"PopulateData\DX_ATTVALUE_MATERIAL.cs");

			Database.Data.FromScript(@"PopulateData\DXDIR_RESULT_ROW.cs");
			Database.Data.FromScript(@"PopulateData\DXDIR_OUTPUT_CATEGORY.cs");
			Database.Data.FromScript(@"PopulateData\DXDIR_DESTINATION.cs");
			Database.Data.FromScript(@"PopulateData\DXDIR_ASSESSMENT_TYPE.cs");
			Database.Data.FromScript(@"PopulateData\DXDIR_LC_STAGE_TMPL.cs");
			Database.Data.FromScript(@"PopulateData\DXDIR_PARTNER_ASSESSMENT_TYPE.cs");
		}

		public override void Down()
		{
			throw new System.NotImplementedException();
		}
	}
}
