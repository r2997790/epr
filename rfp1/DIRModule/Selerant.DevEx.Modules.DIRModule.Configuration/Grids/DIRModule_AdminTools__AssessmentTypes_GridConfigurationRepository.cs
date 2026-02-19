using System;
using System.Collections.Generic;
using Selerant.DevEx.Configuration.Grids.HtmlGrid;
using Selerant.DevEx.Configuration.Grids.HtmlGrid.DTOs;
using Selerant.DevEx.Configuration.Infrastructure;

namespace Selerant.DevEx.Modules.DIRModule.Configuration.Grids
{
    class DIRModule_AdminTools__AssessmentTypes_GridConfigurationRepository : BaseConfigurationRepository<HtmlGridConfiguration>, ICoreHtmlGridConfigurationRepository
	{
        public string GridName => GridNames.ADMIN_TOOLS_ASSESSMENT_TYPES;

		private HtmlGridConfiguration Version_3_9_0(HtmlGridConfiguration config)
		{
            //GridData
            config.HtmlGridData.WithColumn("IdentifiableString").SetPrimary(true).SetRequired(true).SetExportHidden(true).SetFrozen(true).SetHidden(true).SetRequired(true).SetWidth(1)
                                .SetLabel("IdentifiableString")
                                .SetTargetProperties(new string[] { "IdentifiableString" })
                                .SetType("String");
            config.HtmlGridData.WithColumn("Code").SetWidth(50).SetSortProperty("Code")
                                .SetLabel("@DIR_AdminTools.ATAssessmentTypes_Code")
                                .SetTargetProperties(new string[] { "Code" })
                                .SetType("String");
            config.HtmlGridData.WithColumn("Description").SetWidth(250)
                                .SetLabel("@DIR_AdminTools.ATAssessmentTypes_Description")
                                .SetTargetProperties(new string[] { "Description" })
                                .SetType("String");
            config.HtmlGridData.WithColumn("Active").SetWidth(50).SetFilterProperty("Active")
                                .SetLabel("@DIR_AdminTools.ATAssessmentTypes_Active")
                                .SetTargetProperties(new string[] { "Active" })
                                .SetType("String");
            config.HtmlGridData.WithColumn("Actions").SetWidth(200).SetRequired(true).SetExportHidden(true)
                                .SetConverter("DIRModule.AssessmentTypesActionsConverter")
                                .SetLabel("@DIR_Controls.GridColumn_Actions")
                                .SetTargetProperties(new string[] { "IdentifiableString" })
                                .SetType("String");

            //GridUI
            config.HtmlGridUIs.GetOrCreateHtmlGridUI().OrderBy = "Code desc";

            config.HtmlGridUIs.GetOrCreateHtmlGridUI().WithColumn("IdentifiableString");
            config.HtmlGridUIs.GetOrCreateHtmlGridUI().WithColumn("Code");
            config.HtmlGridUIs.GetOrCreateHtmlGridUI().WithColumn("Description");
            config.HtmlGridUIs.GetOrCreateHtmlGridUI().WithColumn("Active");
            config.HtmlGridUIs.GetOrCreateHtmlGridUI().WithColumn("Actions");

            return config;
        }

		protected override IEnumerable<(Version Version, Func<HtmlGridConfiguration, HtmlGridConfiguration>)> ProvideConfigurationVersionHandlers()
		{
			return new List<(Version Version, Func<HtmlGridConfiguration, HtmlGridConfiguration>)>
			{
				(new Version(3, 9, 0), Version_3_9_0)
			};
		}
	}
}
