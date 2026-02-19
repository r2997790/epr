using Selerant.DevEx.Modules.DIRModule.BackEnd.AssessmentManagement.BusinessObjects;
using System.Collections.Generic;

namespace Selerant.DevEx.Modules.DIRModule.WebMvcFrontEnd.Areas.AssessmentNavigator.Models.Panels.AssessmentDashboardDialog
{
    public enum HotSpotValueType
    {
        MASS,
        COST
    }

    public interface IDataStore<T> where T : class
    {
        IList<T> GetHotSpot(HotSpotValueType type);
        IList<T> GetCostOfWaste();
        IList<T> GetResults(string resultType);
        (IList<T> Data, IList<string> DestinationColors) GetFoodLossesNotIncludedInedibleParts(DxAssessment assessment);
        (IList<T> Data, IList<string> DestinationColors) GetFoodLossesInediblePartsOnly(DxAssessment assessment);
    }
}
