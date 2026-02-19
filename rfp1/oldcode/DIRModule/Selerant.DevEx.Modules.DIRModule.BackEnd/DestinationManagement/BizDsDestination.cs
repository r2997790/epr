using System.Collections.Generic;
using System.Data;
using Selerant.ApplicationBlocks.Data;
using Selerant.DevEx.Dal.BizObj;
using Selerant.DevEx.Modules.DIRModule.BackEnd.Helpers;

namespace Selerant.DevEx.Modules.DIRModule.BackEnd.DestinationManagement
{
	internal class BizDsDestination : BizDsObject
	{
		protected override string PackageName => "DXDIR_PK_DESTINATION";

		protected override IDbDataParameter[] CreateParameters(RecordCollection recordCollection, GenericDbCollectionType collectionType, GenericDbOperationType operationType)
		{
			return BizDsCreateParametersHelper
				.CreatePrimaryKeysParameters(
					ProviderFactory.CreateInputStringParameter("sCODE", collectionType, recordCollection.GetValues("CODE")))
				.AddParametersIfNotReadOrDelete(operationType,
					ProviderFactory.CreateInputDecimalParameter("dcWASTE", collectionType, recordCollection.GetValues("WASTE"), 1, 0),
					ProviderFactory.CreateInputDecimalParameter("dcUSED_ON", collectionType, recordCollection.GetValues("USED_ON"), 1, 0),
					ProviderFactory.CreateInputDecimalParameter("dcSORT_ORDER", collectionType, recordCollection.GetValues("SORT_ORDER")),
					ProviderFactory.CreateInputStringParameter("sTITLE", collectionType, recordCollection.GetValues("TITLE")))
				.ToArray();
		}

        public void GetFoodLossesNotIncludedInedibleParts(string assessmentCode, decimal lcStage) => GetFoodLosses(assessmentCode, lcStage, "GetFoodLossesNotIncIndblParts");
        public void GetFoodLossesInediblePartsOnly(string assessmentCode, decimal lcStage) => GetFoodLosses(assessmentCode, lcStage, "GetFoodLossesInediblePartsOnly");

        private void GetFoodLosses(string assessmentCode, decimal lcStage, string procedureName)
        {
            string commandText = ProviderFactory.GetStoredProcedureFullName("DXDIR_PK_RESULTCALCULATIONS", procedureName);

            List<IDbDataParameter> parameters = new List<IDbDataParameter>();

            parameters.Add(ProviderFactory.CreateInputStringParameter("sASSESSMENT_CODE", GenericDbCollectionType.None, assessmentCode));
            parameters.Add(ProviderFactory.CreateInputStringParameter("dcASSESSMENT_LC_STAGE_ID", GenericDbCollectionType.None, lcStage));

            ExecuteReadOperation(commandText, parameters.ToArray());
        }
	}
}
