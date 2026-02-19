using Selerant.DevEx.Configuration.Grids.HtmlGrid;
using Selerant.DevEx.Configuration.Grids.HtmlGrid.DTOs;
using Selerant.DevEx.Configuration.Grids.HtmlGrid.GridUI.DTOs.TileView;
using Selerant.DevEx.Configuration.Infrastructure;
using System;
using System.Collections.Generic;

namespace Selerant.DevEx.Modules.DIRModule.Configuration.Grids
{
    internal class DIRModule_AssessmentSearch_Results_GridConfigurationRepository : BaseConfigurationRepository<HtmlGridConfiguration>, ICoreHtmlGridConfigurationRepository
    {
        public string GridName => GridNames.ASSESSMENT_SEARCH_RESULT;

        protected override IEnumerable<(Version Version, Func<HtmlGridConfiguration, HtmlGridConfiguration>)> ProvideConfigurationVersionHandlers()
        {
            return new List<(Version Version, Func<HtmlGridConfiguration, HtmlGridConfiguration>)>
            {
                (new Version(3, 9, 0), Version_3_9_0)
            };
        }

        private HtmlGridConfiguration Version_3_9_0(HtmlGridConfiguration config)
        {
			//GridData
			config.HtmlGridData.WithColumn("UserRights").SetConverter("UserRightsConverter").SetExportHidden(true).SetHidden(true).SetLabel("UserRights").SetRequired(true).SetSortProperty("TargetObject.Assessment.IdentifiableString").SetTargetProperties(new string[] { "TargetObject.Assessment.IdentifiableString" }).SetType("string");

			//GridUI
			config.HtmlGridUIs.GetOrCreateHtmlGridUI().WithColumn("UserRights");

			HtmlTileViewUI tileViewUI = new HtmlTileViewUI();
			config.HtmlTileViewUIs["DEFAULT"] = tileViewUI;

			tileViewUI.Title.Add("Description");
			tileViewUI.Title.Add("Icon");
			tileViewUI.Body.Add("Code");
			tileViewUI.Body.Add("Type");
            tileViewUI.Body.Add("Status");
            tileViewUI.Body.Add("ModDate");
			tileViewUI.Actions.Add("Actions");

			return config;
        }
    }
}
