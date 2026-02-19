using System.Data;
using Selerant.ApplicationBlocks.Data;
using Selerant.DevEx.Dal.BizObj;
using Selerant.DevEx.Modules.DIRModule.BackEnd.Helpers;

namespace Selerant.DevEx.Modules.DIRModule.BackEnd.InputManagement
{
	internal class BizDsInputCategory : BizDsObject
	{
		protected override string PackageName => "DXDIR_PK_INPUT_CATEGORY";

		protected override IDbDataParameter[] CreateParameters(RecordCollection recordCollection, GenericDbCollectionType collectionType, GenericDbOperationType operationType)
		{
			return BizDsCreateParametersHelper
				.CreatePrimaryKeysParameters(ProviderFactory.CreateInputDecimalParameter("dcID", collectionType, recordCollection.GetValues("ID")))
				.AddParametersIfNotReadOrDelete(operationType,
					ProviderFactory.CreateInputStringParameter("sASSESSMENT_CODE", collectionType, recordCollection.GetValues("ASSESSMENT_CODE")),
					ProviderFactory.CreateInputNationalStringParameter("sTITLE", collectionType, recordCollection.GetValues("TITLE")),
					ProviderFactory.CreateInputStringParameter("sTYPE", collectionType, recordCollection.GetValues("TYPE")))
				.ToArray();
		}

		public decimal GetNextId()
		{
			return InternalGetNextId();
		}
	}
}