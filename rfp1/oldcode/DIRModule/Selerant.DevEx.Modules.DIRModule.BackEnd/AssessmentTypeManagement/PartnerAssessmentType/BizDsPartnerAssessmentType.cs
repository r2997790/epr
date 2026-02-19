using Selerant.ApplicationBlocks.Data;
using Selerant.DevEx.Dal.BizObj;
using Selerant.DevEx.Modules.DIRModule.BackEnd.Helpers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Selerant.DevEx.Modules.DIRModule.BackEnd.AssessmentTypeManagement.PartnerAssessmentType
{
    internal class BizDsPartnerAssessmentType : BizDsObject
    {
        protected override string PackageName => "DXDIR_PK_PARTNER_ASMT_TYPE";

        protected override IDbDataParameter[] CreateParameters(RecordCollection recordCollection, GenericDbCollectionType collectionType, GenericDbOperationType operationType)
        {
            return BizDsCreateParametersHelper
            .CreatePrimaryKeysParameters(
                ProviderFactory.CreateInputStringParameter("sASSESSMENT_TYPE_CODE", collectionType, recordCollection.GetValues("ASSESSMENT_TYPE_CODE")),
                ProviderFactory.CreateInputStringParameter("sPARTNER_ORG_CODE", collectionType, recordCollection.GetValues("PARTNER_ORG_CODE")))
            .AddParametersIfNotReadOrDelete(operationType,
               ProviderFactory.CreateInputDecimalParameter("dcIS_SHARED", collectionType, recordCollection.GetValues("IS_SHARED")))
            .ToArray();
        }
    }
}
