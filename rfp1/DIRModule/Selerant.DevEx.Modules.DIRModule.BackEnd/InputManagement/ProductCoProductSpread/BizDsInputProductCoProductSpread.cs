using System;
using System.Collections.Generic;
using System.Data;
using Selerant.ApplicationBlocks.Data;
using Selerant.DevEx.Dal.BizObj;
using Selerant.DevEx.Modules.DIRModule.BackEnd.Helpers;

namespace Selerant.DevEx.Modules.DIRModule.BackEnd.InputManagement
{
	public class BizDsInputProductCoProductSpread : BizDsObject
	{
		protected override string PackageName => "DXDIR_PK_INPUT_PCP_SPREAD";

		protected override IDbDataParameter[] CreateParameters(RecordCollection recordCollection, GenericDbCollectionType collectionType, GenericDbOperationType operationType)
		{
			return BizDsCreateParametersHelper
				.CreatePrimaryKeysParameters(
					ProviderFactory.CreateInputDecimalParameter("dcINPUT_ID", collectionType, recordCollection.GetValues("INPUT_ID")),
					ProviderFactory.CreateInputStringParameter("sDESTINATION_CODE", collectionType, recordCollection.GetValues("DESTINATION_CODE")))
				.ToArray();
		}

		public void SelectProductCoProductSpread(decimal[] inputIds)
		{
			ExecuteReadOperation(GetStoredProcedureFullName(nameof(SelectProductCoProductSpread)),
					 new IDbDataParameter[] { ProviderFactory.CreateInputDecimalParameter("dcINPUT_IDS", GenericDbCollectionType.AssociativeArray, inputIds) });
		}
	}
}