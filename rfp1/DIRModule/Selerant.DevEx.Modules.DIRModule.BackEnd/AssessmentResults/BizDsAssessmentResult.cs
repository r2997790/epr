using Selerant.ApplicationBlocks.Data;
using Selerant.DevEx.Dal.BizObj;
using Selerant.DevEx.Modules.DIRModule.BackEnd.Helpers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Selerant.DevEx.Modules.DIRModule.BackEnd.AssessmentResults
{
    public class BizDsAssessmentResult : BizDsObject
    {
        protected override string PackageName => "DXDIR_PK_RESULT";

        protected override IDbDataParameter[] CreateParameters(RecordCollection recordCollection, GenericDbCollectionType collectionType, GenericDbOperationType operationType)
        {
            return BizDsCreateParametersHelper
                .CreatePrimaryKeysParameters(ProviderFactory.CreateInputDecimalParameter("dcID", collectionType, recordCollection.GetValues("ID")))
                .AddParametersIfNotReadOrDelete(operationType,
                    ProviderFactory.CreateInputStringParameter("sASSESSMENT_CODE", collectionType, recordCollection.GetValues("ASSESSMENT_CODE")),
                    ProviderFactory.CreateInputDecimalParameter("dcASSESSMENT_LC_STAGE_ID", collectionType, recordCollection.GetValues("ASSESSMENT_LC_STAGE_ID")),
                    ProviderFactory.CreateInputDecimalParameter("dcRESULT_ROW_ID", collectionType, recordCollection.GetValues("RESULT_ROW_ID"), 2, 0),
                    ProviderFactory.CreateInputDecimalParameter("dcRESULT", collectionType, recordCollection.GetValues("RESULT"), 24, 4)
                ).ToArray();
        }

        public decimal GetNextId()
        {
            return InternalGetNextId();
        }
    }
}
