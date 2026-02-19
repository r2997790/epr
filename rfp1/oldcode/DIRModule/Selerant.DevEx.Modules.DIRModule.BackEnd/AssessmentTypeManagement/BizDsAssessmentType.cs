using System.Data;
using Selerant.ApplicationBlocks.Data;
using Selerant.DevEx.Dal.BizObj;
using Selerant.DevEx.Modules.DIRModule.BackEnd.Helpers;

namespace Selerant.DevEx.Modules.DIRModule.BackEnd.AssessmentTypeManagement
{
	internal class BizDsAssessmentType : BizDsObject
	{
		protected override string PackageName => "DXDIR_PK_ASSESSMENT_TYPE";

		protected override IDbDataParameter[] CreateParameters(RecordCollection recordCollection, GenericDbCollectionType collectionType, GenericDbOperationType operationType)
		{
			return BizDsCreateParametersHelper
				.CreatePrimaryKeysParameters(ProviderFactory.CreateInputStringParameter("sCODE", collectionType, recordCollection.GetValues("CODE")))
				.AddParametersIfNotReadOrDelete(operationType,
					ProviderFactory.CreateInputStringParameter("sDESCRIPTION", collectionType, recordCollection.GetValues("DESCRIPTION")),
					ProviderFactory.CreateInputDecimalParameter("dcACTIVE", collectionType, recordCollection.GetValues("ACTIVE"), 1, 0))
				.ToArray();
		}
	}
}
