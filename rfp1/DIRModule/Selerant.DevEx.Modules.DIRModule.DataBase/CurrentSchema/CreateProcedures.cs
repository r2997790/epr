using System;
using Selerant.ApplicationBlocks.Data.Migrations;

namespace Selerant.DevEx.Modules.DIRModule.DataBase.CurrentSchema
{
	public class CreateProcedures : MigrationBase
	{
		public override void Up()
		{
            Database.Execute.StoredProcedure(@"Procedures\AssessmentType.sql");
			Database.Execute.StoredProcedure(@"Procedures\Assessment.sql");
			Database.Execute.StoredProcedure(@"Procedures\AssessmentLcStage.sql");
			
			Database.Execute.StoredProcedure(@"Procedures\Destination.sql");
			Database.Execute.StoredProcedure(@"Procedures\InputCategory.sql");
			Database.Execute.StoredProcedure(@"Procedures\Input.sql");
			Database.Execute.StoredProcedure(@"Procedures\InputDestination.sql");
			Database.Execute.StoredProcedure(@"Procedures\OutputCategory.sql");
            Database.Execute.StoredProcedure(@"Procedures\Output.sql");
            Database.Execute.StoredProcedure(@"Procedures\Calculations.sql");
            Database.Execute.StoredProcedure(@"Procedures\BusinessCost.sql");
            Database.Execute.StoredProcedure(@"Procedures\AssessmentResult.sql");
            Database.Execute.StoredProcedure(@"Procedures\AssessmentResultRow.sql");
            Database.Execute.StoredProcedure(@"Procedures\RecentAssessment.sql");
            Database.Execute.StoredProcedure(@"Procedures\PartnerAssessment.sql");
            Database.Execute.StoredProcedure(@"Procedures\PartnerAssessmentType.sql");
            Database.Execute.StoredProcedure(@"Procedures\LcStageTmpl.sql");
			Database.Execute.StoredProcedure(@"Procedures\AssessmentDestination.sql");
            Database.Execute.StoredProcedure(@"Procedures\OutputGridItem.sql");
			Database.Execute.StoredProcedure(@"Procedures\ResultCalculations.sql");
			Database.Execute.StoredProcedure(@"Procedures\ResourceNotes.sql");
			Database.Execute.StoredProcedure(@"Procedures\InputProductCoProductSpread.sql");

			Database.Execute.StoredProcedure(@"Procedures\Attributes\AssessmentAttributeMaterialization.sql");
			Database.Execute.StoredProcedure(@"Procedures\Attributes\NewAttValueAsessment.sql");
		}

		public override void Down()
		{
			throw new NotImplementedException();
		}
	}
}