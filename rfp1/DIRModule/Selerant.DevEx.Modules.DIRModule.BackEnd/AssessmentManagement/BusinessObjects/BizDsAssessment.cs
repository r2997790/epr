using System.Data;
using Selerant.ApplicationBlocks.Data;
using Selerant.DevEx.Dal.BizObj;
using Selerant.DevEx.Modules.DIRModule.BackEnd.Helpers;

namespace Selerant.DevEx.Modules.DIRModule.BackEnd.AssessmentManagement.BusinessObjects
{
	internal class BizDsAssessment : BizDsObject
	{
		protected override string PackageName => "DXDIR_PK_ASSESSMENT";

		protected override IDbDataParameter[] CreateParameters(RecordCollection recordCollection, GenericDbCollectionType collectionType, GenericDbOperationType operationType)
		{
			return BizDsCreateParametersHelper
				.CreatePrimaryKeysParameters(ProviderFactory.CreateInputStringParameter("sCODE", collectionType, recordCollection.GetValues("CODE")))
				.AddParametersIfNotReadOrDelete(operationType,
					ProviderFactory.CreateInputStringParameter("sTYPE_CODE", collectionType, recordCollection.GetValues("TYPE_CODE")),
					ProviderFactory.CreateInputNationalStringParameter("sDESCRIPTION", collectionType, recordCollection.GetValues("DESCRIPTION")),
					ProviderFactory.CreateInputStringParameter("sSTATUS", collectionType, recordCollection.GetValues("STATUS")),
					ProviderFactory.CreateInputNationalStringParameter("sCOMPANY_NAME", collectionType, recordCollection.GetValues("COMPANY_NAME")),
					ProviderFactory.CreateInputStringParameter("sORG_STRUCTURE", collectionType, recordCollection.GetValues("ORG_STRUCTURE")),
					ProviderFactory.CreateInputStringParameter("sLOCATION", collectionType, recordCollection.GetValues("LOCATION")),
					ProviderFactory.CreateInputStringParameter("sPROD_CLASSIF", collectionType, recordCollection.GetValues("PROD_CLASSIF")),
					ProviderFactory.CreateInputDecimalParameter("dcCOMPLETING_BY", collectionType, recordCollection.GetValues("COMPLETING_BY")),
					ProviderFactory.CreateInputStringParameter("sPHONE", collectionType, recordCollection.GetValues("PHONE")),
					ProviderFactory.CreateInputStringParameter("sEMAIL", collectionType, recordCollection.GetValues("EMAIL")),
					ProviderFactory.CreateInputDateTimeParameter("dtTIMEFRAME_FROM", collectionType, recordCollection.GetValues("TIMEFRAME_FROM")),
					ProviderFactory.CreateInputDateTimeParameter("dtTIMEFRAME_TO", collectionType, recordCollection.GetValues("TIMEFRAME_TO")),
					ProviderFactory.CreateInputDecimalParameter("dcWASTE_WATER_DISCHARGE_RATIO", collectionType, recordCollection.GetValues("WASTE_WATER_DISCHARGE_RATIO"), 3, 2),
					ProviderFactory.CreateInputNationalStringParameter("sCURRENCY", collectionType, recordCollection.GetValues("CURRENCY")),
					ProviderFactory.CreateInputDateTimeParameter("dtCREATE_DATE", collectionType, recordCollection.GetValues("CREATE_DATE")),
					ProviderFactory.CreateInputDecimalParameter("dcCREATED_BY", collectionType, recordCollection.GetValues("CREATED_BY")),
					ProviderFactory.CreateInputDateTimeParameter("dtMOD_DATE", collectionType, recordCollection.GetValues("MOD_DATE")),
					ProviderFactory.CreateInputDecimalParameter("dcMODIFIED_BY", collectionType, recordCollection.GetValues("MODIFIED_BY")),
					ProviderFactory.CreateInputDecimalParameter("dcAUTHORIZATION_ROLE", collectionType, recordCollection.GetValues("AUTHORIZATION_ROLE")))
				.ToArray();
		}
	}
}
