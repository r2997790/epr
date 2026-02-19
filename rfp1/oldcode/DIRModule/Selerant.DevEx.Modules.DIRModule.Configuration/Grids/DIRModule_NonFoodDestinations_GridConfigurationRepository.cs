using System;
using System.Collections.Generic;
using Selerant.DevEx.Configuration.Grids.HtmlGrid;
using Selerant.DevEx.Configuration.Grids.HtmlGrid.DTOs;
using Selerant.DevEx.Modules.DIRModule.ModuleBlocks._ThisProject;
using Selerant.DevEx.Configuration.Infrastructure;

namespace Selerant.DevEx.Modules.DIRModule.Configuration.Grids
{
	public class DIRModule_NonFoodDestinations_GridConfigurationRepository : BaseConfigurationRepository<HtmlGridConfiguration>, ICoreHtmlGridConfigurationRepository
	{
		public string GridName => GridNames.NON_FOOD_DESTINATIONS;

		private HtmlGridConfiguration Version_39(HtmlGridConfiguration config)
		{
			//GridData
			config.HtmlGridData.WithColumn("IdentifiableString").SetPrimary(true).SetRequired(true).SetExportHidden(true).SetFrozen(true).SetHidden(true)
							   .SetLabel("IdentifiableString")
							   .SetTargetProperties(new string[] { "IdentifiableString" })
							   .SetType("String");

			config.HtmlGridData.WithColumn("Category").SetRequired(true)
							   .SetConverter($"{DIRModuleInfo.Instance.ModuleName}.CategoryDestinationsReferenceValueConverter")
							   .SetLabel("@DIR_AssessmentManager.DestinationsGridColumn_Category")
							   .SetTargetProperties(new string[] { "CategoryMaterialIdentifiableString", "Category", "ProductSource" })
							   .SetType("String");

			config.HtmlGridData.WithColumn("PRODUCT")
							   .SetConverter($"{DIRModuleInfo.Instance.ModuleName}.ClientEditableValueConverter")
							   .SetLabel("@DIR_AssessmentManager.DestinationsGridColumn_Product")
							   .SetTargetProperties(new string[] { "ProductFormatted", "Product" })
							   .SetType("String");

			config.HtmlGridData.WithColumn("COPRODUCT")
							   .SetConverter($"{DIRModuleInfo.Instance.ModuleName}.ClientEditableValueConverter")
							   .SetLabel("@DIR_AssessmentManager.DestinationsGridColumn_CoProduct")
							   .SetTargetProperties(new string[] { "CoProductFormatted", "CoProduct" })
							   .SetType("String");

			config.HtmlGridData.WithColumn("FOOD_RESCUE")
						   .SetConverter($"{DIRModuleInfo.Instance.ModuleName}.ClientEditableValueConverter")
						   .SetLabel("@DIR_AssessmentManager.DestinationsGridColumn_FoodRescue")
						   .SetTargetProperties(new string[] { "FoodRescueFormatted", "FoodRescue" })
						   .SetType("String");

			config.HtmlGridData.WithColumn("RECYCLING")
						   .SetConverter($"{DIRModuleInfo.Instance.ModuleName}.ClientEditableValueConverter")
						   .SetLabel("@DIR_AssessmentManager.DestinationsGridColumn_Recycling")
						   .SetTargetProperties(new string[] { "RecyclingFormatted", "Recycling" })
						   .SetType("String");

			config.HtmlGridData.WithColumn("INCIN_WITH_EN_RECOVER")
						   .SetConverter($"{DIRModuleInfo.Instance.ModuleName}.ClientEditableValueConverter")
						   .SetLabel("@DIR_AssessmentManager.DestinationsGridColumn_IncinWithEnRecover")
						   .SetTargetProperties(new string[] { "IncinWithEnRecoverFormatted", "IncinWithEnRecover" })
						   .SetType("String");

			config.HtmlGridData.WithColumn("LANDFILL")
						   .SetConverter($"{DIRModuleInfo.Instance.ModuleName}.ClientEditableValueConverter")
						   .SetLabel("@DIR_AssessmentManager.DestinationsGridColumn_Landfill")
						   .SetTargetProperties(new string[] { "LandfillFormatted", "Landfill" })
						   .SetType("String");

			config.HtmlGridData.WithColumn("SEWER")
						   .SetConverter($"{DIRModuleInfo.Instance.ModuleName}.ClientEditableValueConverter")
						   .SetLabel("@DIR_AssessmentManager.DestinationsGridColumn_Sewer")
						   .SetTargetProperties(new string[] { "SewerFormatted", "Sewer" })
						   .SetType("String");

			config.HtmlGridData.WithColumn("OTHER")
						   .SetConverter($"{DIRModuleInfo.Instance.ModuleName}.ClientEditableValueConverter")
						   .SetLabel("@DIR_AssessmentManager.DestinationsGridColumn_Other")
						   .SetTargetProperties(new string[] { "OtherFormatted", "Other" })
						   .SetType("String");

			config.HtmlGridData.WithColumn("PartOfProductCoproduct")
						   .SetTargetProperties(new string[] { "PartOfProductCoproduct" })
						   .SetHidden(true)
						   .SetExportHidden(true)
						   .SetType("Decimal");

			config.HtmlGridData.WithColumn("Actions").SetWidth(50).SetRequired(true).SetExportHidden(true).SetFrozen(true)
							.SetConverter($"{DIRModuleInfo.Instance.ModuleName}.ResourceEntityDestinationsActionConverter")
							.SetLabel("@DIR_Controls.GridColumn_Actions")
							.SetTargetProperties(new string[] { "IdentifiableString" })
							.SetType("String");

			//GridUI
			config.HtmlGridUIs.GetOrCreateHtmlGridUI().SetOrderBy("IdentifiableString asc");

			config.HtmlGridUIs.GetOrCreateHtmlGridUI().WithColumn("IdentifiableString");
			config.HtmlGridUIs.GetOrCreateHtmlGridUI().WithColumn("Category");
			config.HtmlGridUIs.GetOrCreateHtmlGridUI().WithColumn("PRODUCT");
			config.HtmlGridUIs.GetOrCreateHtmlGridUI().WithColumn("COPRODUCT");
			config.HtmlGridUIs.GetOrCreateHtmlGridUI().WithColumn("FOOD_RESCUE");
			config.HtmlGridUIs.GetOrCreateHtmlGridUI().WithColumn("RECYCLING");
			config.HtmlGridUIs.GetOrCreateHtmlGridUI().WithColumn("INCIN_WITH_EN_RECOVER");
			config.HtmlGridUIs.GetOrCreateHtmlGridUI().WithColumn("LANDFILL");
			config.HtmlGridUIs.GetOrCreateHtmlGridUI().WithColumn("SEWER");
			config.HtmlGridUIs.GetOrCreateHtmlGridUI().WithColumn("OTHER");
			config.HtmlGridUIs.GetOrCreateHtmlGridUI().WithColumn("PartOfProductCoproduct");
			config.HtmlGridUIs.GetOrCreateHtmlGridUI().WithColumn("Actions");

			return config;
		}

		private HtmlGridConfiguration Version_3_10_1(HtmlGridConfiguration config)
		{
			//GridData
			config.HtmlGridData.GetColumn("PRODUCT")
							.SetConverter($"{DIRModuleInfo.Instance.ModuleName}.ClientEditableDecimalConverter")
							.SetLabel("@DIR_AssessmentManager.DestinationsGridColumn_Product_1");

			config.HtmlGridData.WithColumn("PRODUCT_2")
							.SetConverter($"{DIRModuleInfo.Instance.ModuleName}.ClientEditableDecimalConverter")
							.SetLabel("@DIR_AssessmentManager.DestinationsGridColumn_Product_2")
							.SetTargetProperties(new string[] { "Product2Formatted", "Product2" })
							.SetType("String");

			config.HtmlGridData.WithColumn("PRODUCT_3")
							.SetConverter($"{DIRModuleInfo.Instance.ModuleName}.ClientEditableDecimalConverter")
							.SetLabel("@DIR_AssessmentManager.DestinationsGridColumn_Product_3")
							.SetTargetProperties(new string[] { "Product3Formatted", "Product3" })
							.SetType("String");

			config.HtmlGridData.GetColumn("COPRODUCT")
							.SetConverter($"{DIRModuleInfo.Instance.ModuleName}.ClientEditableDecimalConverter")
							.SetLabel("@DIR_AssessmentManager.DestinationsGridColumn_CoProduct_1");

			config.HtmlGridData.WithColumn("COPRODUCT_2")
							.SetConverter($"{DIRModuleInfo.Instance.ModuleName}.ClientEditableDecimalConverter")
							.SetLabel("@DIR_AssessmentManager.DestinationsGridColumn_CoProduct_2")
							.SetTargetProperties(new string[] { "CoProduct2Formatted", "CoProduct2" })
							.SetType("String");

            config.HtmlGridData.WithColumn("ANIMAL_FEED")
                           .SetConverter($"{DIRModuleInfo.Instance.ModuleName}.ClientEditableValueConverter")
                           .SetLabel("@DIR_AssessmentManager.DestinationsGridColumn_AnimalFeed")
                           .SetTargetProperties(new string[] { "AnimalFeedFormatted", "AnimalFeed" })
                           .SetType("String");

            config.HtmlGridData.WithColumn("BIOMASS_MATERIAL")
                           .SetConverter($"{DIRModuleInfo.Instance.ModuleName}.ClientEditableValueConverter")
                           .SetLabel("@DIR_AssessmentManager.DestinationsGridColumn_BiomassMaterial")
                           .SetTargetProperties(new string[] { "BiomassMaterialFormatted", "BiomassMaterial" })
                           .SetType("String");

            config.HtmlGridData.WithColumn("CODIGESTION_ANAEROBIC")
                           .SetConverter($"{DIRModuleInfo.Instance.ModuleName}.ClientEditableValueConverter")
                           .SetLabel("@DIR_AssessmentManager.DestinationsGridColumn_CodigestionAnaerobic")
                           .SetTargetProperties(new string[] { "CodigestionAnaerobicFormatted", "CodigestionAnaerobic" })
                           .SetType("String");

            config.HtmlGridData.WithColumn("COMPOSTING")
                           .SetConverter($"{DIRModuleInfo.Instance.ModuleName}.ClientEditableValueConverter")
                           .SetLabel("@DIR_AssessmentManager.DestinationsGridColumn_Composting")
                           .SetTargetProperties(new string[] { "CompostingFormatted", "Composting" })
                           .SetType("String");

            config.HtmlGridData.WithColumn("COMBUSTION")
                           .SetConverter($"{DIRModuleInfo.Instance.ModuleName}.ClientEditableValueConverter")
                           .SetLabel("@DIR_AssessmentManager.DestinationsGridColumn_Combustion")
                           .SetTargetProperties(new string[] { "CombustionFormatted", "Combustion" })
                           .SetType("String");

            config.HtmlGridData.WithColumn("LAND_APP")
                           .SetConverter($"{DIRModuleInfo.Instance.ModuleName}.ClientEditableValueConverter")
                           .SetLabel("@DIR_AssessmentManager.DestinationsGridColumn_LandApp")
                           .SetTargetProperties(new string[] { "LandAppFormatted", "LandApp" })
                           .SetType("String");

            config.HtmlGridData.WithColumn("NOT_HARVESTED")
                           .SetConverter($"{DIRModuleInfo.Instance.ModuleName}.ClientEditableValueConverter")
                           .SetLabel("@DIR_AssessmentManager.DestinationsGridColumn_NotHarvested")
                           .SetTargetProperties(new string[] { "NotHarvestedFormatted", "NotHarvested" })
                           .SetType("String");

            config.HtmlGridData.WithColumn("REFUSE_DISCARD")
                           .SetConverter($"{DIRModuleInfo.Instance.ModuleName}.ClientEditableValueConverter")
                           .SetLabel("@DIR_AssessmentManager.DestinationsGridColumn_RefuseDiscard")
                           .SetTargetProperties(new string[] { "RefuseDiscardFormatted", "RefuseDiscard" })
                           .SetType("String");

            config.HtmlGridData.WithColumn("ENVIRONMENT_LOSS")
                           .SetConverter($"{DIRModuleInfo.Instance.ModuleName}.ClientEditableValueConverter")
                           .SetLabel("@DIR_AssessmentManager.DestinationsGridColumn_EnvironmentLoss")
                           .SetTargetProperties(new string[] { "EnvironmentLossFormatted", "EnvironmentLoss" })
                           .SetType("String");

            config.HtmlGridData.GetColumn("FOOD_RESCUE").SetConverter($"{DIRModuleInfo.Instance.ModuleName}.ClientEditableDecimalConverter");
            config.HtmlGridData.GetColumn("ANIMAL_FEED").SetConverter($"{DIRModuleInfo.Instance.ModuleName}.ClientEditableDecimalConverter");
            config.HtmlGridData.GetColumn("BIOMASS_MATERIAL").SetConverter($"{DIRModuleInfo.Instance.ModuleName}.ClientEditableDecimalConverter");
            config.HtmlGridData.GetColumn("CODIGESTION_ANAEROBIC").SetConverter($"{DIRModuleInfo.Instance.ModuleName}.ClientEditableDecimalConverter");
            config.HtmlGridData.GetColumn("COMPOSTING").SetConverter($"{DIRModuleInfo.Instance.ModuleName}.ClientEditableDecimalConverter");
            config.HtmlGridData.GetColumn("COMBUSTION").SetConverter($"{DIRModuleInfo.Instance.ModuleName}.ClientEditableDecimalConverter");
            config.HtmlGridData.GetColumn("LAND_APP").SetConverter($"{DIRModuleInfo.Instance.ModuleName}.ClientEditableDecimalConverter");
            config.HtmlGridData.GetColumn("RECYCLING").SetConverter($"{DIRModuleInfo.Instance.ModuleName}.ClientEditableDecimalConverter");
			config.HtmlGridData.GetColumn("INCIN_WITH_EN_RECOVER").SetConverter($"{DIRModuleInfo.Instance.ModuleName}.ClientEditableDecimalConverter");
			config.HtmlGridData.GetColumn("LANDFILL").SetConverter($"{DIRModuleInfo.Instance.ModuleName}.ClientEditableDecimalConverter");
            config.HtmlGridData.GetColumn("NOT_HARVESTED").SetConverter($"{DIRModuleInfo.Instance.ModuleName}.ClientEditableDecimalConverter");
            config.HtmlGridData.GetColumn("REFUSE_DISCARD").SetConverter($"{DIRModuleInfo.Instance.ModuleName}.ClientEditableDecimalConverter");
            config.HtmlGridData.GetColumn("SEWER").SetConverter($"{DIRModuleInfo.Instance.ModuleName}.ClientEditableDecimalConverter");
            config.HtmlGridData.GetColumn("ENVIRONMENT_LOSS").SetConverter($"{DIRModuleInfo.Instance.ModuleName}.ClientEditableDecimalConverter");
            config.HtmlGridData.GetColumn("OTHER").SetConverter($"{DIRModuleInfo.Instance.ModuleName}.ClientEditableDecimalConverter");

			config.HtmlGridData.RemoveColumn("PartOfProductCoproduct");

			config.HtmlGridData.WithColumn("ProductCoproductArray")
			   .SetTargetProperties(new string[] { "ProductCoproductArray" })
			   .SetHidden(true)
			   .SetExportHidden(true)
			   .SetType("Decimal");

			//GridUI
			config.HtmlGridUIs.GetOrCreateHtmlGridUI().WithColumnAfter("PRODUCT_2", "PRODUCT");
			config.HtmlGridUIs.GetOrCreateHtmlGridUI().WithColumnAfter("PRODUCT_3", "PRODUCT_2");
            config.HtmlGridUIs.GetOrCreateHtmlGridUI().WithColumnAfter("COPRODUCT_2", "COPRODUCT");
            config.HtmlGridUIs.GetOrCreateHtmlGridUI().WithColumnAfter("ANIMAL_FEED", "FOOD_RESCUE");
            config.HtmlGridUIs.GetOrCreateHtmlGridUI().WithColumnAfter("BIOMASS_MATERIAL", "ANIMAL_FEED");
            config.HtmlGridUIs.GetOrCreateHtmlGridUI().WithColumnAfter("CODIGESTION_ANAEROBIC", "BIOMASS_MATERIAL");
            config.HtmlGridUIs.GetOrCreateHtmlGridUI().WithColumnAfter("COMPOSTING", "CODIGESTION_ANAEROBIC");
            config.HtmlGridUIs.GetOrCreateHtmlGridUI().WithColumnAfter("COMBUSTION", "COMPOSTING");
            config.HtmlGridUIs.GetOrCreateHtmlGridUI().WithColumnAfter("LAND_APP", "COMBUSTION");
            config.HtmlGridUIs.GetOrCreateHtmlGridUI().WithColumnAfter("NOT_HARVESTED", "LANDFILL");
            config.HtmlGridUIs.GetOrCreateHtmlGridUI().WithColumnAfter("REFUSE_DISCARD", "NOT_HARVESTED");
            config.HtmlGridUIs.GetOrCreateHtmlGridUI().WithColumnAfter("ENVIRONMENT_LOSS", "SEWER");

            config.HtmlGridUIs.GetHtmlGridUI().RemoveColumn("PartOfProductCoproduct");
			config.HtmlGridUIs.GetOrCreateHtmlGridUI().WithColumnAfter("ProductCoproductArray", "IdentifiableString");

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
