using Selerant.ApplicationBlocks.Data;
using Selerant.DevEx.Dal.BizObj;
using Selerant.DevEx.Modules.DIRModule.BackEnd.Helpers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Selerant.DevEx.Modules.DIRModule.BackEnd.ResourceNote
{
	internal class BizDsResourceNote : BizDsObject
	{
		protected override string PackageName => "DXDIR_PK_RESOURCE_NOTE";

		protected override IDbDataParameter[] CreateParameters(RecordCollection recordCollection, GenericDbCollectionType collectionType, GenericDbOperationType operationType)
		{
			return BizDsCreateParametersHelper
				.CreatePrimaryKeysParameters(
					ProviderFactory.CreateInputStringParameter("sASSESSMENT_CODE", collectionType, recordCollection.GetValues("ASSESSMENT_CODE")),
					ProviderFactory.CreateInputDecimalParameter("dcLC_STAGE_ID", collectionType, recordCollection.GetValues("LC_STAGE_ID")),
					ProviderFactory.CreateInputStringParameter("sTYPE", collectionType, recordCollection.GetValues("TYPE")))
				.AddParametersIfNotReadOrDelete(operationType,
					ProviderFactory.CreateInputStringParameter("sNOTE", collectionType, recordCollection.GetValues("NOTE")))
				.ToArray();
		}
	}
}
