using System.Data;
using Selerant.ApplicationBlocks.Data;
using Selerant.DevEx.Dal.BizObj;
using Selerant.DevEx.Modules.DIRModule.BackEnd.Helpers;

namespace Selerant.DevEx.Modules.DIRModule.BackEnd.AssessmentManagement.BusinessObjects
{
	internal class BizDsAssessmentLcStage : BizDsObject
	{
		protected override string PackageName => "DXDIR_PK_ASSESSMENT_LC_STAGE";

		protected override IDbDataParameter[] CreateParameters(RecordCollection recordCollection, GenericDbCollectionType collectionType, GenericDbOperationType operationType)
		{
			return BizDsCreateParametersHelper
				.CreatePrimaryKeysParameters(ProviderFactory.CreateInputDecimalParameter("dcID", collectionType, recordCollection.GetValues("ID")))
				.AddParametersIfNotReadOrDelete(operationType,
					ProviderFactory.CreateInputStringParameter("sASSESSMENT_CODE", collectionType, recordCollection.GetValues("ASSESSMENT_CODE")),
					ProviderFactory.CreateInputNationalStringParameter("sTITLE", collectionType, recordCollection.GetValues("TITLE")),
					ProviderFactory.CreateInputDecimalParameter("dcSORT_ORDER", collectionType, recordCollection.GetValues("SORT_ORDER"), 9, 0),
					ProviderFactory.CreateInputDecimalParameter("dcVISIBLE", collectionType, recordCollection.GetValues("VISIBLE"), 1, 0),
					ProviderFactory.CreateInputDecimalParameter("dcSOURCE_ASMT_LC_STAGE_ID", collectionType, recordCollection.GetValues("SOURCE_ASMT_LC_STAGE_ID")))
				.ToArray();
		}

		public decimal GetNextId()
		{
			return InternalGetNextId();
		}
	}
}
