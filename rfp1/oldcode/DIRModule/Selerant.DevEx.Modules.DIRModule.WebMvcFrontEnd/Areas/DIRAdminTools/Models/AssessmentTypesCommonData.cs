using Selerant.DevEx.WebMvcModules.Areas.AdminTools.Models;

namespace Selerant.DevEx.Modules.DIRModule.WebMvcFrontEnd.Areas.DIRAdminTools.Models
{
    public class AssessmentTypesCommonData : AdminToolCommonData
    {
        public AssessmentTypesCommonData()
        {
            TabText = Locale.GetString("DIR_AdminTools", "ATDIRToolManagement");
            HeaderText = Locale.GetString("DIR_AdminTools", "ATAssessmentTypesTitle");
            Title = Locale.GetString("DIR_AdminTools", "ATAssessmentTypesTitle");
            ToolTip = Locale.GetString("DIR_AdminTools", "ATAssessmentsTypeTT");

            SmallIcon = Links_Images.Icons._16x16.DxCollaboration_png;
            MediumIcon = Links_Images.Icons._24x24.DxCollaboration_png;
            LargeIcon = Links_Images.Icons._24x24.DxCollaboration_png;
        }
    }
}