using Selerant.DevEx.Modules.DIRModule.ModuleBlocks._ThisProject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Selerant.DevEx.Modules.DIRModule.Configuration.Navigation.Assessment
{
    public static class NamesRepository
    {
        #region Assessment Names

        public static class AssessmentPanelNames
        {
            /// <summary>
            /// General Panel
            /// </summary>
            public const string TAB_GENERAL = DIRModuleInfo.MODULE_NAME + "_TabGeneral";

            /// <summary>
            /// Panel Resource Management
            /// </summary>
            public const string TAB_RESOURCE_MANAGEMENT = DIRModuleInfo.MODULE_NAME + "_TabResourceManagement";

            /// <summary>
            /// Panel AssessmentResults
            /// </summary>
            public const string TAB_ASSESSMENT_RESULTS = DIRModuleInfo.MODULE_NAME + "_TabAssessmentResults";
        }
 
        public static class AssessmentTabStripNames
        {
            /// <summary>
            /// Assessment Tabstrip
            /// </summary>
            public const string TABSTRIP_ASSESSMENT = "AssessmentTabStrip";
            /// <summary>
            /// Assessment Tabstrip Dialog
            /// </summary>
            public const string TABSTRIP_ASSESSMENT_DIALOG = "AssessmentTabStripDialog";
        }

        public static class AssessmentToolBarNames
        {
            /// <summary>
            /// Assessment ToolBar
            /// </summary>
            public const string TOOLBAR_ASSESSMENT = "AssessmentToolBar";
        }

        public static class AssessmentMenuCommandNames
        {
            /// <summary>
            /// Command Publish
            /// </summary>
            public const string COPY_ASSESSMENT = "Copy";
            public const string DELETE_ASSESSMENT = "Delete";

        }

        public static class AssessmentSettings
        { 
            public const string TABSTRIP_FILE_NAME = "AssessmentTabStrip";
            public const string TOOLBAR_FILE_NAME = "AssessmentToolBar";
            public const string TABSTRIP_DLG_FILE_NAME = "AssessmentTabStripDialog";
        }
        #endregion
    }
}
