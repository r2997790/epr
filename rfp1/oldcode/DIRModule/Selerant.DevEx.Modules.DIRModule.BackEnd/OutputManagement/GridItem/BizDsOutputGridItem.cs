using Selerant.ApplicationBlocks.Data;
using Selerant.DevEx.Dal.BizObj;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Selerant.DevEx.Modules.DIRModule.BackEnd.OutputManagement.GridItem
{
    internal class BizDsOutputGridItem : BizDsObject
    {
        protected override string PackageName => "DXDIR_PK_OUTPUT_GRIDITEM";

        protected override IDbDataParameter[] CreateParameters(RecordCollection recordCollection, GenericDbCollectionType collectionType, GenericDbOperationType operationType)
        {
            return null;
        }

        public void GetOutputGridItems(string assessmentCode, decimal lcStage)
        {
            string commandText = GetStoredProcedureFullName(nameof(GetOutputGridItems));

            List<IDbDataParameter> parameters = new List<IDbDataParameter>();
            parameters.Add(ProviderFactory.CreateInputStringParameter("sASSESSMENT_CODE", GenericDbCollectionType.None, assessmentCode));
            parameters.Add(ProviderFactory.CreateInputStringParameter("dcASSESSMENT_LC_STAGE_ID", GenericDbCollectionType.None, lcStage));

            ExecuteReadOperation(commandText, parameters.ToArray());
        }
    }
}
