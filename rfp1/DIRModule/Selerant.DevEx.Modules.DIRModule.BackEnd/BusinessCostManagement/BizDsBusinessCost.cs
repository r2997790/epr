using Selerant.ApplicationBlocks.Data;
using Selerant.DevEx.Dal.BizObj;
using Selerant.DevEx.Modules.DIRModule.BackEnd.Helpers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Selerant.DevEx.Modules.DIRModule.BackEnd.BusinessCostManagement
{
	internal class BizDsBusinessCost : BizDsObject
	{
		protected override string PackageName => "DXDIR_PK_BUSINESS_COST";

		protected override IDbDataParameter[] CreateParameters(RecordCollection recordCollection, GenericDbCollectionType collectionType, GenericDbOperationType operationType)
		{
			return BizDsCreateParametersHelper
				.CreatePrimaryKeysParameters(ProviderFactory.CreateInputDecimalParameter("dcID", collectionType, recordCollection.GetValues("ID")))
				.AddParametersIfNotReadOrDelete(operationType,
					ProviderFactory.CreateInputStringParameter("sASSESSMENT_CODE", collectionType, recordCollection.GetValues("ASSESSMENT_CODE")),
					ProviderFactory.CreateInputDecimalParameter("dcASSESSMENT_LC_STAGE_ID", collectionType, recordCollection.GetValues("ASSESSMENT_LC_STAGE_ID")),
					ProviderFactory.CreateInputStringParameter("sTYPE", collectionType, recordCollection.GetValues("TYPE")),
					ProviderFactory.CreateInputStringParameter("sTITLE", collectionType, recordCollection.GetValues("TITLE")),
					ProviderFactory.CreateInputDecimalParameter("dcSORT_ORDER", collectionType, recordCollection.GetValues("SORT_ORDER")), 
					ProviderFactory.CreateInputDecimalParameter("dcCOST", collectionType, recordCollection.GetValues("COST"), 24, 4))
				.ToArray();
		}

		public decimal GetNextId() => InternalGetNextId();

		public void GetCarriedOverBusinessCosts(string assessmentCode, decimal lcStageId)
		{
			string commandText = GetStoredProcedureFullName(nameof(GetCarriedOverBusinessCosts));

			IDbDataParameter[] parameters = new IDbDataParameter[]
			{
				ProviderFactory.CreateInputStringParameter("sASSESSMENT_CODE", GenericDbCollectionType.None, assessmentCode),
				ProviderFactory.CreateInputDecimalParameter("dcASSESSMENT_LC_STAGE_ID", GenericDbCollectionType.None, lcStageId)
			};

			ExecuteReadOperation(commandText, parameters);
		}

		public decimal WasteCollectionTreatment(string assessmentCode, decimal lcStage)
		{
			string spName = ProviderFactory.GetStoredProcedureFullName("DXDIR_PK_RESULTCALCULATIONS", "WasteCollectionTreatmentProc");

			IDbDataParameter resultParameter = ProviderFactory.CreateParameter("dcRESULT", GenericDbType.Decimal, 0, ParameterDirection.Output, false, 26, 12, "", DataRowVersion.Proposed, DbDecimal.Null);
			resultParameter.Precision = 26;
			resultParameter.Scale = 12;

			IDbDataParameter[] parameters = new IDbDataParameter[]
			{
				ProviderFactory.CreateInputStringParameter("sASSESSMENT_CODE", GenericDbCollectionType.None, assessmentCode),
				ProviderFactory.CreateInputDecimalParameter("dcASSESSMENT_LC_STAGE_ID", GenericDbCollectionType.None, lcStage),
				resultParameter
			};

			ExecuteNonQuery(spName, parameters);

			return DbDecimal.ConvertFromParameter(resultParameter.Value).Value;
		}
	}
}
