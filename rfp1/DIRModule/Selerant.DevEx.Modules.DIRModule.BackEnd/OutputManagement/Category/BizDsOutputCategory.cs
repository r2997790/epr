using Selerant.DevEx.Dal.BizObj;
using Selerant.ApplicationBlocks.Data;
using Selerant.DevEx.Modules.DIRModule.BackEnd.Helpers;
using System.Data;

namespace Selerant.DevEx.Modules.DIRModule.BackEnd.OutputManagement
{
    internal class BizDsOutputCategory : BizDsObject
    {
        protected override string PackageName => "DXDIR_PK_OUTPUT_CATEGORY";

        protected override IDbDataParameter[] CreateParameters(RecordCollection recordCollection, GenericDbCollectionType collectionType, GenericDbOperationType operationType)
        {
            return BizDsCreateParametersHelper
                .CreatePrimaryKeysParameters(ProviderFactory.CreateInputDecimalParameter("dcID", collectionType, recordCollection.GetValues("ID")))
                .AddParametersIfNotReadOrDelete(
                operationType,
                    ProviderFactory.CreateInputNationalStringParameter("sTITLE", collectionType, recordCollection.GetValues("TITLE")),
                    ProviderFactory.CreateInputDecimalParameter("dcSORT_ORDER", collectionType, recordCollection.GetValues("SORT_ORDER"), 9, 0),
                    ProviderFactory.CreateInputStringParameter("sTYPE", collectionType, recordCollection.GetValues("TYPE")))
                .ToArray();
        }

        public decimal GetNextId()
        {
            return InternalGetNextId();
        }       
    }
}
