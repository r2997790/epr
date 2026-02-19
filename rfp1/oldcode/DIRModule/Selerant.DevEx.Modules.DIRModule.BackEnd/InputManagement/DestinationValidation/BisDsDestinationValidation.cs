using System.Data;
using Selerant.ApplicationBlocks.Data;
using Selerant.DevEx.Dal.BizObj;

namespace Selerant.DevEx.Modules.DIRModule.BackEnd.InputManagement
{
    public class BisDsDestinationValidation: BizDsObject
    {
        protected override string PackageName => "DXDIR_PK_INPUT_DESTINATION";

        protected override IDbDataParameter[] CreateParameters(RecordCollection recordCollection, GenericDbCollectionType collectionType, GenericDbOperationType operationType)
        {
            return null;
        }
		
		/// <summary>
		/// Returns list of LC Stage which have some input destinations with total percentage less than 100% (in food, non-food or in both destinations types).
		/// </summary>
		/// <param name="assessmentCode"></param>
        public void CheckValidationFillDestination(string assessmentCode)
        {
            ExecuteReadOperation(GetStoredProcedureFullName("CheckValidationFillDestination"),
                                 new IDbDataParameter[] { ProviderFactory.CreateInputStringParameter("sASSESSMENT_CODE", GenericDbCollectionType.None, assessmentCode) });
        }
    }
}
