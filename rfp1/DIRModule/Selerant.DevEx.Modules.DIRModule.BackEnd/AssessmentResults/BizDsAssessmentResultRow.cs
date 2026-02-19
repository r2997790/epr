using Selerant.ApplicationBlocks.Data;
using Selerant.DevEx.Dal.BizObj;
using Selerant.DevEx.Modules.DIRModule.BackEnd.Helpers;
using System.Data;

namespace Selerant.DevEx.Modules.DIRModule.BackEnd.AssessmentResults
{
	internal class BizDsAssessmentResultRow : BizDsObject
    {
        protected override string PackageName => "DXDIR_PK_RESULT_ROW";

        protected override IDbDataParameter[] CreateParameters(RecordCollection recordCollection, GenericDbCollectionType collectionType, GenericDbOperationType operationType)
        {
            return BizDsCreateParametersHelper
                .CreatePrimaryKeysParameters(ProviderFactory.CreateInputDecimalParameter("dcID", collectionType, recordCollection.GetValues("ID")))
                .AddParametersIfNotReadOrDelete(operationType,
                    ProviderFactory.CreateInputStringParameter("sTITLE", collectionType, recordCollection.GetValues("TITLE")),
                    ProviderFactory.CreateInputStringParameter("sRESULT_UOM", collectionType, recordCollection.GetValues("RESULT_UOM")),
                    ProviderFactory.CreateInputStringParameter("sRESULT_TYPE", collectionType, recordCollection.GetValues("RESULT_TYPE")),
                    ProviderFactory.CreateInputDecimalParameter("dcSORT_ORDER", collectionType, recordCollection.GetValues("SORT_ORDER"), 3, 0))
                .ToArray();
        }

		public void GetResults(string assessmentCode, decimal lcStageId)
		{
			string commandText = ProviderFactory.GetStoredProcedureFullName("DXDIR_PK_RESULTCALCULATIONS", "GetResultsTable");

			IDbDataParameter[] parameters = new IDbDataParameter[]
			{
				ProviderFactory.CreateInputStringParameter("sASSESSMENT_CODE", GenericDbCollectionType.None, assessmentCode),
				ProviderFactory.CreateInputStringParameter("dcASSESSMENT_LC_STAGE_ID", GenericDbCollectionType.None, lcStageId)
			};

			ExecuteReadOperation(commandText, parameters);
		}

        public decimal GetEstimatedTrueCostOfWaste(string assessmentCode, decimal lcStageId)
        {
            string spName = ProviderFactory.GetStoredProcedureFullName("DXDIR_PK_RESULTCALCULATIONS", "EstimatedCostOfWaste");

			IDbDataParameter resultParameter = ProviderFactory.CreateParameter("dcRESULT", GenericDbType.Decimal, 0, ParameterDirection.Output, false, 26, 12, "", DataRowVersion.Proposed, DbDecimal.Null);
			resultParameter.Precision = 26;
			resultParameter.Scale = 12;

			IDbDataParameter[] parameters = new IDbDataParameter[]
			{
				ProviderFactory.CreateInputStringParameter("sASSESSMENT_CODE", GenericDbCollectionType.None, assessmentCode),
				ProviderFactory.CreateInputDecimalParameter("dcASSESSMENT_LC_STAGE_ID", GenericDbCollectionType.None, lcStageId),
				resultParameter
			};

			ExecuteNonQuery(spName, parameters);

			return DbDecimal.ConvertFromParameter(resultParameter.Value).Value;
		}
    }
}
