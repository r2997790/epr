using Selerant.DevEx.BusinessLayer;
using Selerant.DevEx.Modules.DIRModule.BackEnd.OutputManagement;
using Selerant.DevEx.Modules.DIRModule.BackEnd.OutputManagement.GridItem;
using Selerant.DevEx.WebPages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Selerant.DevEx.Modules.DIRModule.WebMvcFrontEnd.Areas.AssessmentNavigator.Models.Panels.ResourceManagement.Json
{
    public class OutputModel : IJsonSerialized, IToDxObject<DxOutput>
    {
        public string IdentifiableString { get; set; }
        public decimal? OutputCost { get; set; }
        public decimal? Income { get; set; }

        public DxOutput ToDxObject()
        {
            DxOutputGridItem outputGridItem = DxObject.ParseIdentifiableString<DxOutputGridItem>(IdentifiableString);

            DxOutput output = new DxOutput(outputGridItem.Id);

            output.LoadEntity();

            output.OutputCost = OutputCost;
            output.Income = Income;

            return output;
        }
    }
}