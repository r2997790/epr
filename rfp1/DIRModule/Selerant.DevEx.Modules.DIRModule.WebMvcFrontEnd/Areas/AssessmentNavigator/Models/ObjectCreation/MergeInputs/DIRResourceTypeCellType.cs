using Selerant.DevEx.WebMvcModules.Infrastructure.Controls.SpreadsheetControl;

namespace Selerant.DevEx.Modules.DIRModule.WebMvcFrontEnd.Areas.AssessmentNavigator.Models.ObjectCreation.MergeInputs
{
    public class DIRResourceTypeCellType : SpreadsheetRefObjectCellType
    {
        public override string Type => "DX.DIRModule.AssessmentNavigator.DIRResourceTypeCellType";
        public override CellTypeName TypeName => CellTypeName.Custom;
    }
}