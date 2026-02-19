using Selerant.DevEx.Configuration.Grids.HtmlGrid;
using Selerant.DevEx.Configuration.Grids.HtmlGrid.DTOs;
using Selerant.DevEx.Configuration.Grids.HtmlGrid.GridData.DTOs;
using Selerant.DevEx.Configuration.Grids.HtmlGrid.GridUI.DTOs.TileView;
using Selerant.DevEx.Configuration.Infrastructure;
using System;
using System.Collections.Generic;

namespace Selerant.DevEx.Modules.DIRModule.Configuration.Grids
{
    internal class DIRModule_AssessmentSearch_Recent_GridConfigurationRepository : BaseConfigurationRepository<HtmlGridConfiguration>, ICoreHtmlGridConfigurationRepository
    {
        public string GridName => GridNames.ASSESSMENT_SEARCH_RECENT;
        protected override IEnumerable<(Version Version, Func<HtmlGridConfiguration, HtmlGridConfiguration>)> ProvideConfigurationVersionHandlers()
        {
            return new List<(Version Version, Func<HtmlGridConfiguration, HtmlGridConfiguration>)>
            {
				(new Version(3, 8), Version_38),
				(new Version(3, 9), Version_39)
			};
        }

		private HtmlGridConfiguration Version_38(HtmlGridConfiguration config)
		{
			config.HtmlGridData.WithColumn("IdentifiableString")
				.SetExportHidden(true).SetFrozen(true).SetHidden(true)
				.SetLabel("IdentifiableString")
				.SetPrimary(true)
				.SetRequired(true)
				.SetTargetProperties(new string[] { "TargetObject.Assessment.IdentifiableString" })
				.SetType("String");

			config.HtmlGridData.WithColumn("Description")
				.SetLabel("@DIR_AssessmentManager.AssessmentDescriptionLbl")
				.SetConverter("RecentSearchPanelReferenceConverter")
				.SetSortProperty("TargetObject.Assessment.Description")
				.SetTargetProperties(new string[] { "TargetObject.Assessment.IdentifiableString", "TargetObject.Assessment.Description" })
				.SetType("String")
				.SetWidth(180);

			config.HtmlGridData.WithColumn("Type")
				.SetLabel("@DIR_AssessmentManager.AssessmentTypeLbl")
				.SetHidden(true)
				.SetTargetProperties(new string[] { "TargetObject.Assessment.AssessmentTypeDescription" })
				.SetSortProperty("TargetObject.Assessment.AssessmentTypeDescription")
				.SetType("String");

			config.HtmlGridData.WithColumn("Code")
				.SetLabel("@DIR_AssessmentManager.AssessmentCodeLbl")
				.SetTargetProperties(new string[] { "TargetObject.Assessment.Code" })
				.SetSortProperty("TargetObject.Assessment.Code")
				.SetType("String").SetWidth(50);

			config.HtmlGridData.WithColumn("Status")
				.SetLabel("@DIR_AssessmentManager.AssessmentStatusLbl")
				.SetSortProperty("TargetObject.Assessment.StatusDescription")
				.SetTargetProperties(new string[] { "TargetObject.Assessment.StatusDescription" })
				.SetType("String");

			config.HtmlGridData.WithColumn("ModDate")
				.SetLabel("@DIR_AssessmentManager.AssessmentModDateLbl")
				.SetConverter("DateTimeConverter")
				.SetSortProperty("TargetObject.Assessment.ModDate")
				.SetTargetProperties(new string[] { "TargetObject.Assessment.ModDate" })
				.SetType(nameof(DateTime));

			config.HtmlGridData.WithColumn("Icon")
				.SetConverter("IconConverter")
				.SetHidden(true)
				.SetExportHidden(true)
				.SetFrozen(true)
				.SetRequired(true)
				.SetLabel("")
				.SetTargetProperties(new string[] { "TargetObject.Assessment" })
				.SetType("String").SetWidth(28).SetAlign(Align.center);

			config.HtmlGridUIs.GetOrCreateHtmlGridUI().SetOrderBy("IdentifiableString desc");
			config.HtmlGridUIs.GetOrCreateHtmlGridUI().WithColumn("IdentifiableString");
			config.HtmlGridUIs.GetOrCreateHtmlGridUI().WithColumn("Description");
			config.HtmlGridUIs.GetOrCreateHtmlGridUI().WithColumn("Code");
			config.HtmlGridUIs.GetOrCreateHtmlGridUI().WithColumn("Type");
			config.HtmlGridUIs.GetOrCreateHtmlGridUI().WithColumn("Status");
			config.HtmlGridUIs.GetOrCreateHtmlGridUI().WithColumn("ModDate");

			return config;
		}

		private HtmlGridConfiguration Version_39(HtmlGridConfiguration config)
        {
			//GridData
			config.HtmlGridData.WithColumn("UserRights").SetConverter("UserRightsConverter").SetExportHidden(true).SetHidden(true).SetLabel("UserRights").SetRequired(true).SetSortProperty("TargetObject.Assessment.IdentifiableString").SetTargetProperties(new string[] { "TargetObject.Assessment.IdentifiableString" }).SetType("string");

			//GridUI
			config.HtmlGridUIs.GetOrCreateHtmlGridUI().WithColumn("UserRights");
			config.HtmlGridUIs.GetOrCreateHtmlGridUI().WithColumnAsFirst("Icon");

			HtmlTileViewUI tileViewUI = new HtmlTileViewUI();
			config.HtmlTileViewUIs["DEFAULT"] = tileViewUI;

			tileViewUI.Title.Add("Icon");
			tileViewUI.Title.Add("Description");
			tileViewUI.Body.Add("Code");
			//tileViewUI.Body.Add("Type");
			tileViewUI.Body.Add("Status");

			return config;
		}
    }
}
