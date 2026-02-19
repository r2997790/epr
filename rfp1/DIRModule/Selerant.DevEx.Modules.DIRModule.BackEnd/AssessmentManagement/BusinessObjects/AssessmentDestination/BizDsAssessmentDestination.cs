using System.Data;
using Selerant.ApplicationBlocks.Data;
using Selerant.DevEx.Dal.BizObj;
using Selerant.DevEx.Modules.DIRModule.BackEnd.Helpers;

namespace Selerant.DevEx.Modules.DIRModule.BackEnd.AssessmentManagement.BusinessObjects
{
	internal class BizDsAssessmentDestination : BizDsObject
	{
		protected override string PackageName => "DXDIR_PK_ASSESSMENT_DEST";

		protected override IDbDataParameter[] CreateParameters(RecordCollection recordCollection, GenericDbCollectionType collectionType, GenericDbOperationType operationType)
		{
			return BizDsCreateParametersHelper
				.CreatePrimaryKeysParameters(
					ProviderFactory.CreateInputStringParameter("sASSESSMENT_CODE", collectionType, recordCollection.GetValues("ASSESSMENT_CODE")),
					ProviderFactory.CreateInputStringParameter("sDESTINATION_CODE", collectionType, recordCollection.GetValues("DESTINATION_CODE")))
				.ToArray();
		}
	}
}