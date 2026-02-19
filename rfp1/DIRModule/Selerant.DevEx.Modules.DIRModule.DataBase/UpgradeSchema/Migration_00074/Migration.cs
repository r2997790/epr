using Selerant.ApplicationBlocks.Data.Migrations;
using Selerant.ApplicationBlocks.Data.Migrations.Model.Schema;
using Selerant.ApplicationBlocks.Data.Migrations.Operations.Schema;
using System;

namespace Selerant.DevEx.Modules.DIRModule.DataBase.UpgradeSchema.Migration_00074
{
	class Migration : MigrationBase
	{
		public override void Up()
		{
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

			Database.Create.Index("DXDIR_RESOURCE_NOTE_IDX1").OnTable("DXDIR_RESOURCE_NOTE").OnColumns("ASSESSMENT_CODE").IndexType(IndexType.Ordinary);
			Database.Create.Index("DXDIR_RESOURCE_NOTE_IDX2").OnTable("DXDIR_RESOURCE_NOTE").OnColumns("LC_STAGE_ID").IndexType(IndexType.Ordinary);
			Database.Create.Index("DXDIR_RESOURCE_NOTE_IDX3").OnTable("DXDIR_RESOURCE_NOTE").OnColumns("TYPE").IndexType(IndexType.Ordinary);

			Database.Execute.StoredProcedure("ResourceNotes.sql");
		}

		public override void Down()
		{
			throw new NotImplementedException();
		}

	}
}
