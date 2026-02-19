using Selerant.ApplicationBlocks.Data;
using Selerant.DevEx.Dal.BizObj;
using Selerant.DevEx.Modules.DIRModule.BackEnd.Helpers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Selerant.DevEx.Modules.DIRModule.BackEnd.AssessmentTypeManagement.LcStageTemplate
{
    internal class BizDsLcStageTemplate : BizDsObject
    {
        protected override string PackageName => "DXDIR_PK_LCSTAGE_TMPL";

        protected override IDbDataParameter[] CreateParameters(RecordCollection recordCollection, GenericDbCollectionType collectionType, GenericDbOperationType operationType)
        {
            return BizDsCreateParametersHelper
                .CreatePrimaryKeysParameters(ProviderFactory.CreateInputDecimalParameter("dcID", collectionType, recordCollection.GetValues("ID")))
                .AddParametersIfNotReadOrDelete(operationType,
                    ProviderFactory.CreateInputStringParameter("sASSESSMENT_TYPE_CODE", collectionType, recordCollection.GetValues("ASSESSMENT_TYPE_CODE")),
                    ProviderFactory.CreateInputNationalStringParameter("sTITLE", collectionType, recordCollection.GetValues("TITLE")),
                    ProviderFactory.CreateInputDecimalParameter("dcSORT_ORDER", collectionType, recordCollection.GetValues("SORT_ORDER"), 9, 0))
                .ToArray();
        }

        public decimal GetNextId()
        {
            return InternalGetNextId();
        }
    }
}
