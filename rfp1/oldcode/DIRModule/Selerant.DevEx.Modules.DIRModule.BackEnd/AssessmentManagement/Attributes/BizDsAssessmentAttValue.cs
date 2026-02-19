using System;
using System.Collections.Generic;
using System.Linq;
using Selerant.ApplicationBlocks.Data;
using Selerant.DevEx.Dal.BizObj;

namespace Selerant.DevEx.Modules.DIRModule.BackEnd.AssessmentManagement.Attributes
{
	public class BizDsAssessmentAttValue : BizDsBaseAttValue
	{
		#region Constructors

		/// <summary>
		/// Constructor for BizDsAttributeValue
		/// </summary>
		public BizDsAssessmentAttValue() : base() { }

		#endregion

		#region Properties

		protected override List<ParameterDefinition> ContainerParameterDefinitions => new List<ParameterDefinition>()
		{
			new ParameterDefinition("CODE", GenericDbType.Varchar)
		};

		protected override string PackageName => "DXDIR_PK_NEWATTVALUEASSESSMENT";

		#endregion

		#region Default Methods

		public void SelectOne(string entityCode, decimal id, decimal arrayIndex)
		{
			BaseSelectOne(id, arrayIndex, entityCode);
		}

		public void SelectAllValuesForAttribute(string entityCode, decimal id)
		{
			BaseSelectAllValuesForAttribute(id, entityCode);
		}

		public void SelectAllValuesForSet(string entityCode, decimal id)
		{
			BaseSelectAllValuesForSet(id, entityCode);
		}

		public void SelectAllValuesForContainer(string entityCode)
		{
			BaseSelectAllValuesForContainer(entityCode);
		}

		public void SelectNonMaterializedValuesForContainer(string entityCode)
		{
			BaseSelectNonMaterializedValuesForContainer(entityCode);
		}

		#endregion
	}
}
