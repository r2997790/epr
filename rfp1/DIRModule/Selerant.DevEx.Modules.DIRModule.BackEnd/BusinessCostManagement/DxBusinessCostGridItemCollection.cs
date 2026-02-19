using Selerant.DevEx.BusinessLayer;
using Selerant.DevEx.Modules.DIRModule.BackEnd.AssessmentResults;
using System.Linq;

namespace Selerant.DevEx.Modules.DIRModule.BackEnd.BusinessCostManagement
{
	[IsDxObjectCollection]
    [BoundBusinessObject(typeof(DxBusinessCostGridItem))]
    public partial class DxBusinessCostGridItemCollection : DxObjectCollection
    {
		#region Fields

		private readonly string _assessmentCode;
		private readonly decimal _lcStageId;

		#endregion

		#region Constructor

		protected DxBusinessCostGridItemCollection(string assessmentCode, decimal lcStageId) : base()
        {
			this._assessmentCode = assessmentCode;
			this._lcStageId = lcStageId;
		}

		public DxBusinessCostGridItemCollection(string assessmentCode, decimal lcStageId, bool load = false)
			: this(assessmentCode, lcStageId)
		{
			filteredLoadMethod = new DxMethodInvoker(BizDsObjectType.GetMethod(nameof(BizDsBusinessCost.GetCarriedOverBusinessCosts)));
			FilteredLoadMethod.MethodActualParameters = new object[] { assessmentCode, lcStageId };

			if (load)
				Load();
		}

		#endregion

		//public static List<decimal> GetLcStageCarriedOverSequence(string assessmentCode, decimal lcStageId)
		//{
		//	var result = new List<decimal>();

		//	var lcStages = new DxAssessmentLcStageCollection(DxAssessmentLcStageCollection.Filter.AssessmentCode, assessmentCode);
		//	lcStages.Load();

		//	if (lcStages.Count == 1)
		//	{
		//		result.Add(lcStageId);
		//	}
		//	else
		//	{
		//		var startingLcStage = lcStages[lcStageId];
		//		var lcStagesReversedOrderArr = lcStages.Where(x => x.SortOrder <= startingLcStage.SortOrder).OrderByDescending(o => o.SortOrder).ToArray();

		//		DxAssessmentLcStage current;
		//		DxAssessmentLcStage next = null;
		//		for (int i = 0; i < lcStagesReversedOrderArr.Length; i++)
		//		{
		//			current = lcStagesReversedOrderArr[i];

		//			if (i + 1 < lcStagesReversedOrderArr.Length)
		//				next = lcStagesReversedOrderArr[i + 1];

		//			if (next != null && next.Id == current.SourceAssessmentLcStageId)
		//			{
		//				if (i < 1)
		//					result.Add(current.Id);
		//				result.Add(next.Id);
		//			}
		//			else if (i == 0)
		//			{
		//				result.Add(current.Id); // allways add starting stage id
		//				break;
		//			}
		//			else
		//				break;

		//			next = null;
		//		}
		//	}

		//	return result;
		//}

		public (decimal wasteCosts, decimal materialLoss) GetMaterialLoss(decimal wasteCollectionAndTreatment)
        {
			decimal estimatedTrueCostOfWaste = DxAssessmentResultGridItemCollection.GetEstimatedTrueCostOfWaste(_assessmentCode, _lcStageId);
			decimal wasteCosts = this.Sum(x => x.WasteCost?.Value ?? 0.0m);

			decimal result = estimatedTrueCostOfWaste - (wasteCosts + wasteCollectionAndTreatment);

			return (wasteCosts, result);
		}
    }
}
