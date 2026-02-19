using System;
using System.Collections.Generic;
using Selerant.DevEx.Configuration.Grids.HtmlGrid;
using Selerant.DevEx.Configuration.Grids.HtmlGrid.DTOs;
using Selerant.DevEx.Modules.DIRModule.ModuleBlocks._ThisProject;
using Selerant.DevEx.Configuration.Infrastructure;

namespace Selerant.DevEx.Modules.DIRModule.Configuration.Grids
{
	public class DIRModule_Inputs_GridConfigurationRepository : BaseConfigurationRepository<HtmlGridConfiguration>, ICoreHtmlGridConfigurationRepository
	{
		public string GridName => GridNames.INPUTS;

		private HtmlGridConfiguration Version_3_9_0(HtmlGridConfiguration config)
		{
			//GridData
			config.HtmlGridData.WithColumn("IdentifiableString").SetPrimary(true).SetRequired(true).SetExportHidden(true).SetFrozen(true).SetHidden(true)
							   .SetLabel("IdentifiableString")
							   .SetTargetProperties(new string[] { "IdentifiableString" })
							   .SetType("String");

			config.HtmlGridData.WithColumn("Category").SetRequired(true)
							   .SetConverter($"{DIRModuleInfo.Instance.ModuleName}.CategoryMaterialReferenceValueConverter")
							   .SetLabel("@DIR_AssessmentManager.InputsGridColumn_Category")
							   .SetTargetProperties(new string[] { "MaterialIdentifiableString", "CategoryTitle" })
							   .SetType("String");

			config.HtmlGridData.WithColumn("CategoryType").SetExportHidden(true).SetFrozen(true).SetHidden(true)
							   .SetTargetProperties(new string[] { "CategoryType" })
							   .SetType("String");

			config.HtmlGridData.WithColumn("CategoryAction").SetWidth(24).SetExportHidden(true).SetFrozen(true)
							   .SetCellAttr("DX.DIRModule.AssessmentNavigator.ResourceManagement.Shared.InputsGridTab.getInstance().CategoryActionCellFunc")
							   .SetLabel(string.Empty)
							   .SetConverter($"{DIRModuleInfo.Instance.ModuleName}.InputCategoryActionConverter")
							   .SetTargetProperties(new string[] { "CategoryIdentifiableString", "ProductSource" })
							   .SetType("String");

			config.HtmlGridData.WithColumn("PartOfProductCoproduct").SetWidth(120).SetAlign(DevEx.Configuration.Grids.HtmlGrid.GridData.DTOs.Align.center)
							   .SetConverter($"{DIRModuleInfo.Instance.ModuleName}.BoolToYesNoValueConverter")
							   .SetLabel("@DIR_AssessmentManager.InputsGridColumn_PartOfProductCoproduct")
							   .SetTargetProperties(new string[] { "PartOfProductCoproduct" })
							   .SetType("String");

			config.HtmlGridData.WithColumn("Packaging").SetWidth(120).SetAlign(DevEx.Configuration.Grids.HtmlGrid.GridData.DTOs.Align.center)
							   .SetConverter($"{DIRModuleInfo.Instance.ModuleName}.BoolToYesNoValueConverter")
							   .SetLabel("@DIR_AssessmentManager.InputsGridColumn_Packaging")
							   .SetTargetProperties(new string[] { "Packaging" })
							   .SetType("String");

			config.HtmlGridData.WithColumn("Mass")
							   .SetConverter($"{DIRModuleInfo.Instance.ModuleName}.ClientEditableValueConverter")
							   .SetLabel("@DIR_AssessmentManager.InputsGridColumn_Mass")
							   .SetTargetProperties(new string[] { "MassFormatted", "Mass" })
							   .SetType("Decimal");

			config.HtmlGridData.WithColumn("Cost")
							   .SetConverter($"{DIRModuleInfo.Instance.ModuleName}.ClientEditableValueConverter")
							   .SetLabel("@DIR_AssessmentManager.InputsGridColumn_Cost")
							   .SetTargetProperties(new string[] { "CostFormatted", "Cost" })
							   .SetType("String");

			config.HtmlGridData.WithColumn("Food")
							   .SetLabel("@DIR_AssessmentManager.InputsGridColumn_Food")
							   .SetTargetProperties(new string[] { "FoodFormatted" })
							   .SetType("String");

			config.HtmlGridData.WithColumn("InedibleParts")
							   .SetConverter($"{DIRModuleInfo.Instance.ModuleName}.ClientEditableValueConverter")
							   .SetLabel("@DIR_AssessmentManager.InputsGridColumn_InedibleParts")
							   .SetTargetProperties(new string[] { "InediblePartsFormatted", "InedibleParts" })
							   .SetType("String");

			config.HtmlGridData.WithColumn("Measurement")
							   .SetConverter($"{DIRModuleInfo.Instance.ModuleName}.ClientEditableValueConverter")
							   .SetLabel("@DIR_AssessmentManager.InputsGridColumn_Measurement")
							   .SetTargetProperties(new string[] { "MeasurementFormatted", "Measurement" })
							   .SetType("String");



			config.HtmlGridData.WithColumn("Actions").SetWidth(50).SetRequired(true).SetExportHidden(true).SetFrozen(true)
							   .SetConverter($"{DIRModuleInfo.Instance.ModuleName}.ResourceEntityActionConverter")
							   .SetLabel("@DIR_Controls.GridColumn_Actions")
							   .SetTargetProperties(new string[] { "IdentifiableString" })
							   .SetType("String");

			// Default columns
			config.HtmlGridUIs.GetOrCreateHtmlGridUI().WithColumn("IdentifiableString");
			config.HtmlGridUIs.GetOrCreateHtmlGridUI().WithColumn("Category");
			config.HtmlGridUIs.GetOrCreateHtmlGridUI().WithColumn("CategoryType");
			config.HtmlGridUIs.GetOrCreateHtmlGridUI().WithColumn("CategoryAction");
			config.HtmlGridUIs.GetOrCreateHtmlGridUI().WithColumn("PartOfProductCoproduct");
			config.HtmlGridUIs.GetOrCreateHtmlGridUI().WithColumn("Packaging");
			config.HtmlGridUIs.GetOrCreateHtmlGridUI().WithColumn("Mass");
			config.HtmlGridUIs.GetOrCreateHtmlGridUI().WithColumn("Cost");
			config.HtmlGridUIs.GetOrCreateHtmlGridUI().WithColumn("Food");
			config.HtmlGridUIs.GetOrCreateHtmlGridUI().WithColumn("InedibleParts");
			config.HtmlGridUIs.GetOrCreateHtmlGridUI().WithColumn("Measurement");

			config.HtmlGridUIs.GetOrCreateHtmlGridUI().WithColumn("Actions");

			return config;
		}

		private HtmlGridConfiguration Version_3_10_1(HtmlGridConfiguration config)
		{
			//GridData
			config.HtmlGridData.GetColumn("PartOfProductCoproduct")
				.SetConverter($"{DIRModuleInfo.Instance.ModuleName}.ClientEditableArrayConverter")
				.SetTargetProperties(new string[] { "PartOfProductCoproductFormatted", "PartOfProductCoproductCodes" });

			config.HtmlGridData.GetColumn("Mass")
				.SetConverter($"{DIRModuleInfo.Instance.ModuleName}.ClientEditableDecimalConverter");

			config.HtmlGridData.GetColumn("Cost")
				.SetConverter($"{DIRModuleInfo.Instance.ModuleName}.ClientEditableDecimalConverter");

			config.HtmlGridData.GetColumn("InedibleParts")
				.SetConverter($"{DIRModuleInfo.Instance.ModuleName}.ClientEditableDecimalConverter");

			config.HtmlGridData.GetColumn("Measurement")
				.SetConverter($"{DIRModuleInfo.Instance.ModuleName}.ClientEditableDecimalConverter");

			return config;
		}

		protected override IEnumerable<(Version Version, Func<HtmlGridConfiguration, HtmlGridConfiguration>)> ProvideConfigurationVersionHandlers()
		{
			return new List<(Version Version, Func<HtmlGridConfiguration, HtmlGridConfiguration>)>
			{
				(new Version(3, 9, 0), Version_3_9_0),
				(new Version(3, 10, 1), Version_3_10_1)
			};
		}
	}
}
