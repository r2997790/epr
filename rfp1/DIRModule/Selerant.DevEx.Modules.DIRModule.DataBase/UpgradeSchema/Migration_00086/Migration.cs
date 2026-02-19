using System;
using Selerant.ApplicationBlocks.Data;
using Selerant.ApplicationBlocks.Data.Migrations;
using Selerant.ApplicationBlocks.Data.Migrations.Operations.Schema;

namespace Selerant.DevEx.Modules.DIRModule.DataBase.UpgradeSchema.Migration_00086
{
	public class Migration : MigrationBase
	{
		public override void Up()
		{			
			// DXDIR_ASSESSMENT_LC_STAGE - New column

			Database.Alter.Table("DXDIR_ASSESSMENT_LC_STAGE")
				.Add.NewColumn("SOURCE_ASMT_LC_STAGE_ID").Type.Decimal().Length(11).Nullable();

			Database.Alter.Table("DXDIR_ASSESSMENT_LC_STAGE")
				.Add.Constraint.ForeignKey("DXDIR_ASSESSMENT_LC_STAGE_FK2").Keys("SOURCE_ASMT_LC_STAGE_ID").References("DXDIR_ASSESSMENT_LC_STAGE").Columns("ID").OnDelete.SetNull();

			Database.Create.Index("DXDIR_ASSESSMENT_LC_STAGE_IDX2").OnTable("DXDIR_ASSESSMENT_LC_STAGE").OnColumns("SOURCE_ASMT_LC_STAGE_ID").IndexType(IndexType.Ordinary);

			Database.Execute.StoredProcedure("AssessmentLcStage.sql");


			// NOTE: this table type for SQLServer is defined at the top of BusinessCost.sql (because DbManager is outputing wrong type for PBC_COST_PREV)
			if (Context.ProviderType == DataProviderType.Oracle)
			{
				Database.Create.Type("DXDIR_BizCostCarriedKey").As.Object()
					.WithField("SORT_ORDER").Type.Decimal().Length(9)
					.WithField("TITLE_PREV").Type.Nvarchar().Length(256)
					.WithField("PBC_COST_PREV").Type.Decimal().Precision(26, 4);

				Database.Create.Type("DXDIR_BizCostCarriedKeyTable").As.Table().Of.UserDefined("DXDIR_BizCostCarriedKey");
			}

			
			Database.Execute.StoredProcedure("BusinessCost.sql");

			Database.Execute.StoredProcedure("ResultCalculations.sql");
		}

		public override void Down()
		{
			throw new NotImplementedException();
		}
	}
}
