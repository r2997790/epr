using Selerant.DevEx.Dal.BizObj;
using Selerant.ApplicationBlocks.Data;
using Selerant.DevEx.Modules.DIRModule.BackEnd.Helpers;
using System.Data;

namespace Selerant.DevEx.Modules.DIRModule.BackEnd.OutputManagement
{
    internal class BizDsOutput : BizDsObject
    {
		public const string DXDIR_PK_OUTPUT = "DXDIR_PK_OUTPUT";

		protected override string PackageName => DXDIR_PK_OUTPUT;

        protected override IDbDataParameter[] CreateParameters(RecordCollection recordCollection, GenericDbCollectionType collectionType, GenericDbOperationType operationType)
        {
            return BizDsCreateParametersHelper
                .CreatePrimaryKeysParameters(ProviderFactory.CreateInputDecimalParameter("dcID", collectionType, recordCollection.GetValues("ID")))
                .AddParametersIfNotReadOrDelete(operationType,
                    ProviderFactory.CreateInputStringParameter("sASSESSMENT_CODE", collectionType, recordCollection.GetValues("ASSESSMENT_CODE")),
                    ProviderFactory.CreateInputDecimalParameter("dcOUTPUT_CATEGORY_ID", collectionType, recordCollection.GetValues("OUTPUT_CATEGORY_ID")),
					ProviderFactory.CreateInputStringParameter("sDESTINATION_CODE", collectionType, recordCollection.GetValues("DESTINATION_CODE")),
					ProviderFactory.CreateInputDecimalParameter("dcASSESSMENT_LC_STAGE_ID", collectionType, recordCollection.GetValues("ASSESSMENT_LC_STAGE_ID")),
					ProviderFactory.CreateInputDecimalParameter("dcINPUT_ID", collectionType, recordCollection.GetValues("INPUT_ID")),
					ProviderFactory.CreateInputStringParameter("sMAT_PLANT", collectionType, recordCollection.GetValues("MAT_PLANT")),
                    ProviderFactory.CreateInputStringParameter("sMAT_CODE", collectionType, recordCollection.GetValues("MAT_CODE")),
                    ProviderFactory.CreateInputDecimalParameter("dcOUTPUT_COST", collectionType, recordCollection.GetValues("OUTPUT_COST"), 24, 4),
                    ProviderFactory.CreateInputDecimalParameter("dcCOST", collectionType, recordCollection.GetValues("COST"), 24, 4),
                    ProviderFactory.CreateInputDecimalParameter("dcINCOME", collectionType, recordCollection.GetValues("INCOME"), 24, 4),
                    ProviderFactory.CreateInputDecimalParameter("dcWEIGHT", collectionType, recordCollection.GetValues("WEIGHT"), 19, 3),
					ProviderFactory.CreateInputDecimalParameter("dcSORT_ORDER", collectionType, recordCollection.GetValues("SORT_ORDER"))
				)
				.ToArray();
        }

		public decimal GetNextId()
        {
            return InternalGetNextId();
        }

		public void InsertOrUpdateNonWasteRecord(string assessmentCode, string destinationCode, decimal assessmentLcStageId)
		{
			string spName = GetStoredProcedureFullName("InsertOrUpdateNonWaste");

			IDbDataParameter[] parameters = new IDbDataParameter[]
			{
				ProviderFactory.CreateInputStringParameter("sASSESSMENT_CODE", GenericDbCollectionType.None, assessmentCode),
				ProviderFactory.CreateInputStringParameter("sDESTINATION_CODE", GenericDbCollectionType.None, destinationCode),
				ProviderFactory.CreateInputDecimalParameter("dcASSESSMENT_LC_STAGE_ID", GenericDbCollectionType.None, assessmentLcStageId)
			};

			ExecuteNonQuery(spName, parameters);
		}
	}
}
