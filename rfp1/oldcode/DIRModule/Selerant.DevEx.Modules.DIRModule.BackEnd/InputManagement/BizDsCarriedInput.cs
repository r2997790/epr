using System;
using System.Collections.Generic;
using System.Data;
using Selerant.ApplicationBlocks.Data;
using Selerant.DevEx.Dal.BizObj;

namespace Selerant.DevEx.Modules.DIRModule.BackEnd.InputManagement
{
	internal class BizDsCarriedInput : BizDsObject
	{
		protected override string PackageName => "DXDIR_PK_INPUT";

		protected override IDbDataParameter[] CreateParameters(RecordCollection recordCollection, GenericDbCollectionType collectionType, GenericDbOperationType operationType)
		{
			return default(IDbDataParameter[]);
		}

		public void GetCarriedInputsToNextStage(string assessmentCode, decimal lcStage)
		{
			string commandText = GetStoredProcedureFullName(nameof(GetCarriedInputsToNextStage));

			IDbDataParameter[] parameters = new IDbDataParameter[]
			{
				ProviderFactory.CreateInputStringParameter("sASSESSMENT_CODE", GenericDbCollectionType.None, assessmentCode),
				ProviderFactory.CreateInputDecimalParameter("dcASSESSMENT_LC_STAGE_ID", GenericDbCollectionType.None, lcStage)
			};

			ExecuteReadOperation(commandText, parameters);
		}

		public (decimal currentInputsCount, decimal nextInputsCount) HasInputsToCarryOverCount(string assessmentCode, decimal lcStageIdCurrent, decimal lcStageIdNext)
		{
			string spName = GetStoredProcedureFullName(nameof(HasInputsToCarryOverCount));

			IDbDataParameter createBigIntOutputParameter(string parameterName)
			{
				IDbDataParameter decimalOutputParameter = ProviderFactory.CreateParameter(parameterName, GenericDbType.Decimal, 0, ParameterDirection.Output, false, 38, 0, "", DataRowVersion.Proposed, DbDecimal.Null);
				decimalOutputParameter.Precision = 38;
				decimalOutputParameter.Scale = 0;
				return decimalOutputParameter;
			};

			IDbDataParameter resultCurrentParameter = createBigIntOutputParameter("dcRESULT_CURRENT");
			IDbDataParameter resultNextParameter = createBigIntOutputParameter("dcRESULT_NEXT");

			IDbDataParameter[] parameters = new IDbDataParameter[]
			{
				ProviderFactory.CreateInputStringParameter("sASSESSMENT_CODE", GenericDbCollectionType.None, assessmentCode),
				ProviderFactory.CreateInputDecimalParameter("dcASSESSMENT_LC_STAGE_ID_CUR", GenericDbCollectionType.None, lcStageIdCurrent),
				ProviderFactory.CreateInputDecimalParameter("dcASSESSMENT_LC_STAGE_ID_NEXT", GenericDbCollectionType.None, lcStageIdNext),
				resultCurrentParameter,
				resultNextParameter
			};

			ExecuteNonQuery(spName, parameters);

			var resultCurrentCount = DbDecimal.ConvertFromParameter(resultCurrentParameter.Value).Value;
			var resultNextCount = DbDecimal.ConvertFromParameter(resultNextParameter.Value).Value;

			return (resultCurrentCount, resultNextCount);
		}
	}
}
