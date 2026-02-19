using Selerant.DevEx.BusinessLayer;

namespace Selerant.DevEx.Modules.DIRModule.BackEnd.InputManagement
{
	[IsDxObjectCollection]
	[BoundBusinessObject(typeof(DxCarriedInput))]
	public partial class DxCarriedInputCollection : DxObjectCollection
	{
		#region Constructors

		public DxCarriedInputCollection() : base()
		{
		}

		public DxCarriedInputCollection(string assessmentCode, decimal assessmentLcStageId, string bizMethodName, bool load = false) : this()
		{
			filteredLoadMethod = new DxMethodInvoker(BizDsObjectType.GetMethod(bizMethodName));
			FilteredLoadMethod.MethodActualParameters = new object[] { assessmentCode, assessmentLcStageId };

			if (load)
				Load();
		}

		#endregion

		public static (decimal currentInputsCount, decimal nextInputsCount) HasInputsToCarryOverCount(string assessmentCode, decimal lcStageCurrent, decimal lcStageNext)
		{
			var bizDs = new BizDsCarriedInput();
			return bizDs.HasInputsToCarryOverCount(assessmentCode, lcStageCurrent, lcStageNext);
		}
	}
}
