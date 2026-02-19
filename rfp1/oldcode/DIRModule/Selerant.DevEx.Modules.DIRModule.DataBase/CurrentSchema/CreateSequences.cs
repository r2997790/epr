using System;
using Selerant.ApplicationBlocks.Data.Migrations;

namespace Selerant.DevEx.Modules.DIRModule.DataBase.CurrentSchema
{
	public class CreateSequences : MigrationBase
	{
		public override void Up()
		{
			Database.Create.Sequence("DXDIR_LC_STAGE_TMPL_SEQ")
			   .IncrementBy(1)
			   .StartWith(9)
			   .MinValue(9)
			   .MaxValue(99999999999)
			   .Cycle(false)
			   .Cache(20)
			   .Order(false);

			Database.Create.Sequence("DXDIR_INPUT_CATEGORY_TMPL_SEQ")
			   .IncrementBy(1)
			   .StartWith(1)
			   .MinValue(1)
			   .MaxValue(99999999999)
			   .Cycle(false)
			   .Cache(20)
			   .Order(false);

			Database.Create.Sequence("DXDIR_OUTPUT_CATEGORY_TMPL_SEQ")
			   .IncrementBy(1)
			   .StartWith(1)
			   .MinValue(1)
			   .MaxValue(99999999999)
			   .Cycle(false)
			   .Cache(20)
			   .Order(false);

			Database.Create.Sequence("DXDIR_BUSINESS_COST_TMPL_SEQ")
			   .IncrementBy(1)
			   .StartWith(1)
			   .MinValue(1)
			   .MaxValue(99999999999)
			   .Cycle(false)
			   .Cache(20)
			   .Order(false);

			Database.Create.Sequence("DXDIR_ASSESSMENT_LC_STAGE_SEQ")
			   .IncrementBy(1)
			   .StartWith(1)
			   .MinValue(1)
			   .MaxValue(99999999999)
			   .Cycle(false)
			   .Cache(20)
			   .Order(false);

			Database.Create.Sequence("DXDIR_INPUT_CATEGORY_SEQ")
			   .IncrementBy(1)
			   .StartWith(1)
			   .MinValue(1)
			   .MaxValue(99999999999)
			   .Cycle(false)
			   .Cache(20)
			   .Order(false);

			Database.Create.Sequence("DXDIR_OUTPUT_CATEGORY_SEQ")
			   .IncrementBy(1)
			   .StartWith(8)
			   .MinValue(8)
			   .MaxValue(99999999999)
			   .Cycle(false)
			   .Cache(20)
			   .Order(false);

			Database.Create.Sequence("DXDIR_INPUT_SEQ")
			   .IncrementBy(1)
			   .StartWith(1)
			   .MinValue(1)
			   .MaxValue(99999999999)
			   .Cycle(false)
			   .Cache(20)
			   .Order(false);

			Database.Create.Sequence("DXDIR_OUTPUT_SEQ")
			   .IncrementBy(1)
			   .StartWith(1)
			   .MinValue(1)
			   .MaxValue(99999999999)
			   .Cycle(false)
			   .Cache(20)
			   .Order(false);

			Database.Create.Sequence("DXDIR_BUSINESS_COST_SEQ")
			   .IncrementBy(1)
			   .StartWith(1)
			   .MinValue(1)
			   .MaxValue(99999999999)
			   .Cycle(false)
			   .Cache(20)
			   .Order(false);

			Database.Create.Sequence("DXDIR_RESULT_SEQ")
			   .IncrementBy(1)
			   .StartWith(1)
			   .MinValue(1)
			   .MaxValue(99999999999)
			   .Cycle(false)
			   .Cache(20)
			   .Order(false);
        }

		public override void Down()
		{
			throw new NotImplementedException();
		}
	}
}
