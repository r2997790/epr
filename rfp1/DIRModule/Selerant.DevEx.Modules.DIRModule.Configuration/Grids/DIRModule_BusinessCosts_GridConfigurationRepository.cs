using System;
using System.Collections.Generic;
using Selerant.DevEx.Configuration.Grids.HtmlGrid;
using Selerant.DevEx.Configuration.Grids.HtmlGrid.DTOs;
using Selerant.DevEx.Modules.DIRModule.ModuleBlocks._ThisProject;
using Selerant.DevEx.Configuration.Infrastructure;

namespace Selerant.DevEx.Modules.DIRModule.Configuration.Grids
{
	public class DIRModule_BusinessCosts_GridConfigurationRepository : BaseConfigurationRepository<HtmlGridConfiguration>, ICoreHtmlGridConfigurationRepository
	{
		public string GridName => GridNames.BUSINESSCOSTS;

		private HtmlGridConfiguration Version_39(HtmlGridConfiguration config)
		{
			//GridData
			config.HtmlGridData.WithColumn("IdentifiableString").SetPrimary(true).SetRequired(true).SetExportHidden(true).SetFrozen(true).SetHidden(true).SetRequired(true)
							   .SetLabel("IdentifiableString")
							   .SetTargetProperties(new string[] { "IdentifiableString" })
							   .SetType("String");

			config.HtmlGridData.WithColumn("Title")
                               .SetConverter($"{DIRModuleInfo.Instance.ModuleName}.ClientEditablePhraseValueConverter")
                               .SetLabel("@DIR_AssessmentManager.BusinessCostsGridColumn_BusinessCost")
							   .SetTargetProperties(new string[] { "Title", "TitleDescription" })
							   .SetType("String");


			config.HtmlGridData.WithColumn("Cost")
							   .SetConverter($"{DIRModuleInfo.Instance.ModuleName}.ClientEditableValueConverter")
							   .SetLabel("@DIR_AssessmentManager.BusinessCostsGridColumn_AverageCostPerAnnum")
							   .SetTargetProperties(new string[] { "CostFormatted", "Cost" })
							   .SetType("Decimal");

			config.HtmlGridData.WithColumn("WasteCost")
							   .SetConverter($"{DIRModuleInfo.Instance.ModuleName}.ClientEditableValueConverter")
							   .SetLabel("@DIR_AssessmentManager.BusinessCostsGridColumn_CostWasteManagement")
							   .SetTargetProperties(new string[] { "WasteCostFormatted", "WasteCost" })
							   .SetEditable(false)
							   .SetType("Decimal");

			config.HtmlGridData.WithColumn("Actions").SetWidth(50).SetRequired(true).SetExportHidden(true).SetFrozen(true)
							   .SetConverter($"{DIRModuleInfo.Instance.ModuleName}.ResourceEntityActionConverter")
							   .SetLabel("@DIR_Controls.GridColumn_Actions")
							   .SetTargetProperties(new string[] { "IdentifiableString" })
							   .SetType("String");

			//GridUI
			config.HtmlGridUIs.GetOrCreateHtmlGridUI().SetOrderBy("IdentifiableString asc");

			config.HtmlGridUIs.GetOrCreateHtmlGridUI().WithColumn("IdentifiableString");
			config.HtmlGridUIs.GetOrCreateHtmlGridUI().WithColumn("Title");
			config.HtmlGridUIs.GetOrCreateHtmlGridUI().WithColumn("Cost");
			config.HtmlGridUIs.GetOrCreateHtmlGridUI().WithColumn("WasteCost");
			config.HtmlGridUIs.GetOrCreateHtmlGridUI().WithColumn("Actions");

			return config;
		}

		private HtmlGridConfiguration Version_3_10_1(HtmlGridConfiguration config)
		{
            config.HtmlGridData.WithColumn("TotalCost")
                               .SetConverter($"{DIRModuleInfo.Instance.ModuleName}.ClientEditableDecimalConverter")
                               .SetLabel("@DIR_AssessmentManager.BusinessCostsGridColumn_TotalCost")
                               .SetTargetProperties(new string[] { "TotalCostFormatted", "TotalCost" })
                               .SetEditable(false)
                               .SetType("Decimal");

            config.HtmlGridData.WithColumn("CarriedCost")
                               .SetConverter($"{DIRModuleInfo.Instance.ModuleName}.ClientEditableDecimalConverter")
                               .SetLabel("@DIR_AssessmentManager.BusinessCostsGridColumn_CarriedCost")
                               .SetTargetProperties(new string[] { "CarriedCostFormatted", "CarriedCost" })
                               .SetEditable(false)
                               .SetType("Decimal");

            config.HtmlGridData.GetColumn("Cost").SetConverter($"{DIRModuleInfo.Instance.ModuleName}.ClientEditableDecimalConverter");
			config.HtmlGridData.GetColumn("WasteCost").SetConverter($"{DIRModuleInfo.Instance.ModuleName}.ClientEditableDecimalConverter");
			config.HtmlGridData.GetColumn("Actions").SetTargetProperties(new string[] { "IdentifiableString", "IsCarriedOverCost" });

            config.HtmlGridUIs.GetOrCreateHtmlGridUI().WithColumnAfter("CarriedCost", "Cost");
            config.HtmlGridUIs.GetOrCreateHtmlGridUI().WithColumnAfter("TotalCost", "CarriedCost");


            return config;
		}

		protected override IEnumerable<(Version Version, Func<HtmlGridConfiguration, HtmlGridConfiguration>)> ProvideConfigurationVersionHandlers()
		{
			return new List<(Version Version, Func<HtmlGridConfiguration, HtmlGridConfiguration>)>
			{
				(new Version(3, 9), Version_39),
				(new Version(3, 10, 1), Version_3_10_1)
			};
		}
	}
}
