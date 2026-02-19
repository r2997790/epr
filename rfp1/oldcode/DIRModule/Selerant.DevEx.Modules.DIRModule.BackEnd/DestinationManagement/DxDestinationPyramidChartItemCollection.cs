using Selerant.DevEx.BusinessLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Selerant.DevEx.Modules.DIRModule.BackEnd.DestinationManagement
{ 
	[IsDxObjectCollection]
    [BoundBusinessObject(typeof(DxDestinationPyramidChartItem))]
    public partial class DxDestinationPyramidChartItemCollection : DxObjectCollection
    {

        protected DxDestinationPyramidChartItemCollection() : base()
        {

        }

        private DxDestinationPyramidChartItemCollection(string assessmentCode, decimal lcStage, string methodName, bool load = false) : this()
        {
            filteredLoadMethod = new DxMethodInvoker(BizDsObjectType.GetMethod(methodName));
            FilteredLoadMethod.MethodActualParameters = new object[] { assessmentCode, lcStage };

            if (load)
                Load();
        }

        public static DxDestinationPyramidChartItemCollection OfFoodLossesNotIncludedInedibleParts(string assessmentCode, decimal lcStage, bool load = false) 
            => new DxDestinationPyramidChartItemCollection(assessmentCode, lcStage, nameof(BizDsDestination.GetFoodLossesNotIncludedInedibleParts), load);

        public static DxDestinationPyramidChartItemCollection OfFoodLossesInediblePartsOnly(string assessmentCode, decimal lcStage, bool load = false) 
            => new DxDestinationPyramidChartItemCollection(assessmentCode, lcStage, nameof(BizDsDestination.GetFoodLossesInediblePartsOnly), load);
	}
}
