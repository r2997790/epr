using System.Data;
using Selerant.ApplicationBlocks.Data;
using Selerant.DevEx.Dal.BizObj;

namespace Selerant.DevEx.Modules.DIRModule.BackEnd.DestinationManagement
{
	public class BizDsDestinationSort : BizDsObject
	{
		protected override string PackageName => "DXDIR_PK_DESTINATION";

		protected override IDbDataParameter[] CreateParameters(RecordCollection recordCollection, GenericDbCollectionType collectionType, GenericDbOperationType operationType)
		{
			return null;
		}

		public void SelectDestinationSort(string[] destinationCodes)
		{
			ExecuteReadOperation(GetStoredProcedureFullName(nameof(SelectDestinationSort)),
								 new IDbDataParameter[] { ProviderFactory.CreateInputStringParameter("sDESTINATION_CODES", GenericDbCollectionType.AssociativeArray, destinationCodes) });
		}
	}
}
