using System.Collections.Generic;
using System.Data;
using Selerant.ApplicationBlocks.Data;
using Selerant.DevEx.Dal.BizObj;
using Selerant.DevEx.Modules.DIRModule.BackEnd.Helpers;

namespace Selerant.DevEx.Modules.DIRModule.BackEnd.InputManagement
{
	internal class BizDsInputDestination : BizDsObject
	{
		protected override string PackageName => "DXDIR_PK_INPUT_DESTINATION";

		protected override IDbDataParameter[] CreateParameters(RecordCollection recordCollection, GenericDbCollectionType collectionType, GenericDbOperationType operationType)
		{
			return BizDsCreateParametersHelper
				.CreatePrimaryKeysParameters(
					ProviderFactory.CreateInputDecimalParameter("dcINPUT_ID", collectionType, recordCollection.GetValues("INPUT_ID")),
					ProviderFactory.CreateInputStringParameter("sDESTINATION_CODE", collectionType, recordCollection.GetValues("DESTINATION_CODE")),
					ProviderFactory.CreateInputDecimalParameter("dcOUTPUT_CATEGORY_ID", collectionType, recordCollection.GetValues("OUTPUT_CATEGORY_ID")))
				.AddParametersIfNotReadOrDelete(operationType,
					ProviderFactory.CreateInputDecimalParameter("dcPERCENTAGE", collectionType, recordCollection.GetValues("PERCENTAGE"), 4, 1))
				.ToArray();
		}

		public void SelectFoodDestinations(string assessmentCode, decimal assessmentLcStage)
		{
			string commandText = GetStoredProcedureFullName(nameof(SelectFoodDestinations));

			List<IDbDataParameter> parameters = new List<IDbDataParameter>();

			parameters.Add(ProviderFactory.CreateInputStringParameter("sASSESSMENT_CODE", GenericDbCollectionType.None, assessmentCode));
			parameters.Add(ProviderFactory.CreateInputDecimalParameter("dcASSESSMENT_LC_STAGE_ID", GenericDbCollectionType.None, assessmentLcStage));

			ExecuteReadOperation(commandText, parameters.ToArray());
		}

		public void SelectNonFoodDestinations(string assessmentCode, decimal assessmentLcStage)
		{
			string commandText = GetStoredProcedureFullName(nameof(SelectNonFoodDestinations));

			List<IDbDataParameter> parameters = new List<IDbDataParameter>();

			parameters.Add(ProviderFactory.CreateInputStringParameter("sASSESSMENT_CODE", GenericDbCollectionType.None, assessmentCode));
			parameters.Add(ProviderFactory.CreateInputDecimalParameter("dcASSESSMENT_LC_STAGE_ID", GenericDbCollectionType.None, assessmentLcStage));

			ExecuteReadOperation(commandText, parameters.ToArray());
		}

		public void DeleteByDestination(string assessmentCode, string destinationCode)
		{
			string spName = GetStoredProcedureFullName("DeleteByDestination");

			IDbDataParameter[] parametars = new IDbDataParameter[]
			{
				ProviderFactory.CreateInputStringParameter("sASSESSMENT_CODE", GenericDbCollectionType.None, assessmentCode),
				ProviderFactory.CreateInputStringParameter("sDESTINATION_CODE", GenericDbCollectionType.None, destinationCode),
			};

			ExecuteNonQuery(spName, parametars);
		}

		/// <summary>
		/// Deletes any product, co-product or food rescue destinations
		/// </summary>
		/// <param name="inputId"></param>
		public void DeleteDestiantionByCodes(decimal inputId, string[] destinationCodes)
		{
			ExecuteNonQuery(
				GetStoredProcedureFullName(nameof(DeleteDestiantionByCodes)),
				new IDbDataParameter[] 
				{
					ProviderFactory.CreateInputDecimalParameter("dcINPUT_ID", GenericDbCollectionType.None, inputId),
					ProviderFactory.CreateInputStringParameter("sDESTINATION_CODES", GenericDbCollectionType.AssociativeArray, destinationCodes)
				}
			);
		}
	}
}
