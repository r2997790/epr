using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Selerant.ApplicationBlocks.Data.Migrations;

namespace Selerant.DevEx.Modules.DIRModule.DataBase.CurrentSchema.PopulateData
{
	public class DX_COMPONENT_CONTROLLER : MigrationBase
	{
		public override void Up()
		{
			Database.Data.InsertInto("DX_COMPONENT_CONTROLLER")
			.Columns("ID", "PARENT_ID", "TARGET_TYPE", "TYPE", "CONTROL_ID", "LOGICAL_PATH", "PHYSICAL_PATH", "FLAGS")
			.Values(319, null, "Selerant.DevEx.Modules.DIRModule.BackEnd.AssessmentManagement.BusinessObjects.DxAssessment, Selerant.DevEx.Modules.DIRModule.BackEnd", "Navigator", "AssessmentNavigator", "AssessmentNavigator", null, 0);

			Database.Data.InsertInto("DX_COMPONENT_CONTROLLER")
			.Columns("ID", "PARENT_ID", "TARGET_TYPE", "TYPE", "CONTROL_ID", "LOGICAL_PATH", "PHYSICAL_PATH", "FLAGS")
			.Values(320, 319, "Selerant.DevEx.Modules.DIRModule.BackEnd.AssessmentManagement.BusinessObjects.DxAssessment, Selerant.DevEx.Modules.DIRModule.BackEnd", "Tree", "NavigatorSection", "AssessmentNavigator/Tree", null, 0);

			Database.Data.InsertInto("DX_COMPONENT_CONTROLLER")
			.Columns("ID", "PARENT_ID", "TARGET_TYPE", "TYPE", "CONTROL_ID", "LOGICAL_PATH", "PHYSICAL_PATH", "FLAGS")
			.Values(321, 319, "Selerant.DevEx.Modules.DIRModule.BackEnd.AssessmentManagement.BusinessObjects.DxAssessment, Selerant.DevEx.Modules.DIRModule.BackEnd", "Toolbar", "MainToolbar", "AssessmentNavigator/Toolbar", null, 0);
		}

		public override void Down() 
		{
			throw new System.NotImplementedException();
		}
	}
}
