using System;
using System.Collections.Generic;
using System.Data;
using Selerant.ApplicationBlocks.Data;
using Selerant.DevEx.Dal.BizObj;
using Selerant.DevEx.Modules.DIRModule.BackEnd.Helpers;
using Selerant.DevEx.Modules.DIRModule.BackEnd.OutputManagement;

namespace Selerant.DevEx.Modules.DIRModule.BackEnd.InputManagement
{
	internal class BizDsInput : BizDsObject
	{
		protected override string PackageName => "DXDIR_PK_INPUT";

		protected override IDbDataParameter[] CreateParameters(RecordCollection recordCollection, GenericDbCollectionType collectionType, GenericDbOperationType operationType)
		{
			return BizDsCreateParametersHelper
				.CreatePrimaryKeysParameters(ProviderFactory.CreateInputDecimalParameter("dcID", collectionType, recordCollection.GetValues("ID")))
				.AddParametersIfNotReadOrDelete(operationType,
					ProviderFactory.CreateInputStringParameter("sASSESSMENT_CODE", collectionType, recordCollection.GetValues("ASSESSMENT_CODE")),
					ProviderFactory.CreateInputDecimalParameter("dcINPUT_CATEGORY_ID", collectionType, recordCollection.GetValues("INPUT_CATEGORY_ID")),
					ProviderFactory.CreateInputDecimalParameter("dcASSESSMENT_LC_STAGE_ID", collectionType, recordCollection.GetValues("ASSESSMENT_LC_STAGE_ID")),
					ProviderFactory.CreateInputStringParameter("sMAT_PLANT", collectionType, recordCollection.GetValues("MAT_PLANT")),
					ProviderFactory.CreateInputStringParameter("sMAT_CODE", collectionType, recordCollection.GetValues("MAT_CODE")),
					ProviderFactory.CreateInputDecimalParameter("dcPART_OF_PRODUCT_COPRODUCT", collectionType, recordCollection.GetValues("PART_OF_PRODUCT_COPRODUCT"), 1, 0),
					ProviderFactory.CreateInputDecimalParameter("dcPACKAGING", collectionType, recordCollection.GetValues("PACKAGING"), 1, 0),
					ProviderFactory.CreateInputDecimalParameter("dcMASS", collectionType, recordCollection.GetValues("MASS"), 19, 3),
					ProviderFactory.CreateInputDecimalParameter("dcCOST", collectionType, recordCollection.GetValues("COST"), 24, 4),
					ProviderFactory.CreateInputDecimalParameter("dcINEDIBLE_PARTS", collectionType, recordCollection.GetValues("INEDIBLE_PARTS"), 3, 2),
					ProviderFactory.CreateInputDecimalParameter("dcMEASUREMENT", collectionType, recordCollection.GetValues("MEASUREMENT"), 1, 0),
					ProviderFactory.CreateInputStringParameter("sPRODUCT_SOURCE", collectionType, recordCollection.GetValues("PRODUCT_SOURCE")),
					ProviderFactory.CreateInputDecimalParameter("dcCATEGORY_SORT_ORDER", collectionType, recordCollection.GetValues("CATEGORY_SORT_ORDER"), 9, 0),
					ProviderFactory.CreateInputDecimalParameter("dcINPUT_SORT_ORDER", collectionType, recordCollection.GetValues("INPUT_SORT_ORDER"), 9, 0)
				).ToArray();
		}

		public decimal GetNextId()
		{
			return InternalGetNextId();
		}

		public void ManageWastewaterRow(string assessmentCode, decimal assessmentLcStageId)
		{
			string packageAndSpName = ProviderFactory.GetStoredProcedureFullName(BizDsOutput.DXDIR_PK_OUTPUT, nameof(ManageWastewaterRow));

			ExecuteNonQuery(packageAndSpName, new IDbDataParameter[]
			{
				ProviderFactory.CreateInputStringParameter("sASSESSMENT_CODE", GenericDbCollectionType.None, assessmentCode),
				ProviderFactory.CreateInputDecimalParameter("dcASSESSMENT_LC_STAGE_ID", GenericDbCollectionType.None, assessmentLcStageId)
			});
		}

		public (decimal categorySortOrder, decimal inputSortOrder) GetNextSortOrder(string assessmentCode, decimal inputCategoryId, decimal assessmentLcStageId)
		{
			string spName = GetStoredProcedureFullName("NextSortOrder");

			var categorySortOrder = ProviderFactory.CreateOutputDecimalParameter("dcCATEGORY_SORT_ORDER", GenericDbCollectionType.None, 1m);
			var inputSortOrder = ProviderFactory.CreateOutputDecimalParameter("dcINPUT_SORT_ORDER", GenericDbCollectionType.None, 1m);
			IDbDataParameter[] parameters = new IDbDataParameter[]
			{
				ProviderFactory.CreateInputStringParameter("sASSESSMENT_CODE", GenericDbCollectionType.None, assessmentCode),
				ProviderFactory.CreateInputDecimalParameter("dcINPUT_CATEGORY_ID", GenericDbCollectionType.None, inputCategoryId),
				ProviderFactory.CreateInputDecimalParameter("dcASSESSMENT_LC_STAGE_ID", GenericDbCollectionType.None, assessmentLcStageId),
				categorySortOrder,
				inputSortOrder
			};

			ExecuteNonQuery(spName, parameters);

			return (DbDecimal.ConvertFromParameter(categorySortOrder.Value).Value, DbDecimal.ConvertFromParameter(inputSortOrder.Value).Value);
		}

		public void UpdateDestinInediblePart(string assessmentCode, decimal assessmentLcStageId, decimal inputId)
		{
			string spName = GetStoredProcedureFullName("UpdateDestinInediblePart");

			IDbDataParameter[] parametars = new IDbDataParameter[]
			{
				ProviderFactory.CreateInputStringParameter("sASSESSMENT_CODE", GenericDbCollectionType.None, assessmentCode),
				ProviderFactory.CreateInputDecimalParameter("dcASSESSMENT_LC_STAGE_ID", GenericDbCollectionType.None, assessmentLcStageId),
				ProviderFactory.CreateInputDecimalParameter("dcINPUT_ID", GenericDbCollectionType.None, inputId),
			};

			ExecuteNonQuery(spName, parametars);
		}

		public void UpdateOutputAnotherDestination(string assessmentCode, decimal assessmentLcStageId, decimal inputId)
		{
			string spName = GetStoredProcedureFullName("UpdateOutputAnotherDestination");

			IDbDataParameter[] parametars = new IDbDataParameter[]
			{
				ProviderFactory.CreateInputStringParameter("sASSESSMENT_CODE", GenericDbCollectionType.None, assessmentCode),
				ProviderFactory.CreateInputDecimalParameter("dcASSESSMENT_LC_STAGE_ID", GenericDbCollectionType.None, assessmentLcStageId),
				ProviderFactory.CreateInputDecimalParameter("dcINPUT_ID", GenericDbCollectionType.None, inputId),
			};

			ExecuteNonQuery(spName, parametars);
		}

		public void UpdateCascadeOnOutput(string assessmentCode, decimal assessmentLcStageId)
		{
			string spName = GetStoredProcedureFullName("UpdateCascadeOnOutput");

			IDbDataParameter[] parametars = new IDbDataParameter[]
			{
				ProviderFactory.CreateInputStringParameter("sASSESSMENT_CODE", GenericDbCollectionType.None, assessmentCode),
				ProviderFactory.CreateInputDecimalParameter("dcASSESSMENT_LC_STAGE_ID", GenericDbCollectionType.None, assessmentLcStageId)
			};

			ExecuteNonQuery(spName, parametars);
		}

		public void DeleteCascadeOutputParent(decimal[] inputIDs, string assessmentCode, decimal assessmentLcStageId)
		{
			string spName = GetStoredProcedureFullName("DeleteCascadeOutputParent");

			IDbDataParameter[] parametars = new IDbDataParameter[]
			{
				ProviderFactory.CreateInputDecimalParameter("dcINPUT_ID", GenericDbCollectionType.AssociativeArray, inputIDs),
				ProviderFactory.CreateInputStringParameter("sASSESSMENT_CODE", GenericDbCollectionType.None, assessmentCode),
				ProviderFactory.CreateInputDecimalParameter("dcASSESSMENT_LC_STAGE_ID", GenericDbCollectionType.None, assessmentLcStageId)
			};

			ExecuteNonQuery(spName, parametars);
		}
	}
}