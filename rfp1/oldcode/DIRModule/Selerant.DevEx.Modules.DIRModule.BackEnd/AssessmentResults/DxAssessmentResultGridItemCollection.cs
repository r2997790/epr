using Selerant.DevEx.BusinessLayer;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Selerant.DevEx.Modules.DIRModule.BackEnd.AssessmentResults
{ 
	[IsDxObjectCollection]
    [BoundBusinessObject(typeof(DxAssessmentResultGridItem))]
    public partial class DxAssessmentResultGridItemCollection : DxObjectCollection
    {
		#region Constructors

		protected DxAssessmentResultGridItemCollection() : base()
        {
        }

		public DxAssessmentResultGridItemCollection(string assessmentCode, decimal lcStage, bool load = false) : this()
		{
			filteredLoadMethod = new DxMethodInvoker(BizDsObjectType.GetMethod(nameof(BizDsAssessmentResultRow.GetResults)));
			FilteredLoadMethod.MethodActualParameters = new object[] { assessmentCode, lcStage };

			if (load)
				Load();
		}

		#endregion

		#region Indexers Override

		public new DxAssessmentResultGridItem this[int index] => (DxAssessmentResultGridItem)base[index];

		#endregion

		#region LoadItems Methods

		public List<DxAssessmentResultGridItem> LoadItemsResultRow()
		{
			if (Count == 0)
				return new List<DxAssessmentResultGridItem>();

			return this.ToList();
		}

		#endregion

		public static decimal GetEstimatedTrueCostOfWaste(string assessmentCode, decimal lcStageId)
        {
            BizDsAssessmentResultRow bizDs = new BizDsAssessmentResultRow();
            return bizDs.GetEstimatedTrueCostOfWaste(assessmentCode, lcStageId);
        }
	}
}
