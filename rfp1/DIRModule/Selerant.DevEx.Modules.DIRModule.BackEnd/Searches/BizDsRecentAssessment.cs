using Selerant.ApplicationBlocks.Data;
using Selerant.DevEx.Dal.BizObj;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Selerant.DevEx.Modules.DIRModule.BackEnd.Searches
{
    internal class BizDsRecentAssessment : BizDsRecentObject
    {
        public BizDsRecentAssessment() : base()
        {

        }

        protected override string PackageName => "DXDIR_PK_RECENT_ASSESSMENT";

        protected override IDbDataParameter[] CreateParameters(RecordCollection recordCollection, GenericDbCollectionType collectionType, GenericDbOperationType operationType)
        {
            ArrayList parameters = new ArrayList();

            parameters.Add(ProviderFactory.CreateInputDecimalParameter("dcUSER_ID", collectionType, recordCollection.GetValues("USER_ID")));
            parameters.Add(ProviderFactory.CreateInputStringParameter("sCODE", collectionType, recordCollection.GetValues("CODE")));

            if ((operationType != GenericDbOperationType.ReadRecord) && (operationType != GenericDbOperationType.DeleteRecord))
            {
                parameters.Add(ProviderFactory.CreateInputDateTimeParameter("daTIMESTAMP", collectionType, recordCollection.GetValues("TIMESTAMP")));
            }

            return (IDbDataParameter[])parameters.ToArray(typeof(IDbDataParameter));
        }
    }
}
