using System.Linq;
using Selerant.DevEx.BusinessLayer;
using Selerant.DevEx.BusinessLayer.IconManagement;
using Selerant.DevEx.Modules.DIRModule.BackEnd.InputManagement;
using Selerant.DevEx.Modules.DIRModule.Configuration.Resources;
using Selerant.DevEx.WebPages;
using System.Web.Mvc;
using InputCategoryType = Selerant.DevEx.Modules.DIRModule.BackEnd.Common.Constants.InputType;
using System.Collections.Generic;
using Selerant.DevEx.Modules.DIRModule.WebMvcFrontEnd.Helpers;

namespace Selerant.DevEx.Modules.DIRModule.WebMvcFrontEnd.Areas.AssessmentNavigator.Models.Panels.ResourceManagement.Json
{
	public class InputResponseModel : IJsonSerialized
	{
		#region Fields

		private AmountFormatter formatter;
		private DxMaterial material;
		private readonly decimal? inediblePartsFraction;

		#endregion

		#region Properties

		public string IdentifiableString { get; private set; }

		public string MaterialIconUrl { get; private set; }
		public string MaterialIdentifiable { get; private set; }
		public string MaterialOpenTabHrefScript { get; private set; }
		public string MaterialDescription { get; private set; }

		public List<string> PartOfProductCoproductCodes { get; private set; }
		public string PartOfProductCoproductFormatted { get; private set; }

		public bool? Packaging { get; private set; }
		public string PackagingFormatted => FormatBooleanYesNo(Packaging);

		public decimal? Mass { get; private set; }
		public string MassFormatted => formatter.ToThreeDecimals(Mass);

		public decimal? Cost { get; private set; }
		public string CostFormatted => formatter.ToCurrency(Cost);

		public string FoodFormatted { get; private set; }

		public decimal? InedibleParts { get; private set; }
		public string InediblePartsFormatted => formatter.FractionToPercentage(inediblePartsFraction);

		public decimal Measurement { get; private set; }
		public string MeasurementDisplayValue { get; private set; }

        public string ProductSource { get; private set; }

        #endregion

        #region Constructors

        public InputResponseModel(DxInput input, string inputCategoryType, AmountFormatter amountFormatter)
		{
			formatter = amountFormatter;

			IdentifiableString = input.IdentifiableString;
			material = input.Material;

			PartOfProductCoproductCodes = new DxInputProductCoProductSpreadCollection(input.Id, true).Select(r => r.DestinationCode).ToList<string>();
			if (PartOfProductCoproductCodes.Count > 0)
				PartOfProductCoproductFormatted = GridHelpers.Instance.BuildPartOfProductCoProductText(PartOfProductCoproductCodes);
			else
				PartOfProductCoproductFormatted = Locale.GetString(ResourceFiles.AssessmentManager, "ATAddActiveStatusNo");

			Packaging = input.Packaging;
			Mass = input.Mass;
			Cost = input.Cost;

			if (inputCategoryType == InputCategoryType.FOOD)
			{
				InedibleParts = input.InedibleParts * 100;
				inediblePartsFraction = input.InedibleParts;

				FoodFormatted = formatter.FractionToPercentage(1 - (inediblePartsFraction ?? 0.0m));
			}
			else
				FoodFormatted = null;

			Measurement = (decimal)input.Measurement;
			MeasurementDisplayValue = input.MeasurementDesc;
            ProductSource = input.ProductSource;
		}

		#endregion

		private string FormatBooleanYesNo(bool? value)
		{
			if (value.HasValue)
			{
				return value.Value ? Locale.GetString(ResourceFiles.AssessmentManager, "ATAddActiveStatusYes") :
								     Locale.GetString(ResourceFiles.AssessmentManager, "ATAddActiveStatusNo");
			}
			else
				return string.Empty;
		}

		public InputResponseModel SetMaterialProperties(UrlHelper urlHelper)
		{
			MaterialIconUrl = urlHelper.Content(IconManager.Default.GetImageUrlByEntity(material, IconLogicalSize.Small));

			MaterialIdentifiable = material.IdentifiableString;

			string materialNavUrl = urlHelper.Action(MVC.MaterialNavigator.Home.Index(MaterialIdentifiable));
			MaterialOpenTabHrefScript = WebMvcModules.Controllers.HostController.GetOpenTabScriptReference(materialNavUrl);

			material.Load();
			MaterialDescription = material.Description;

			return this;
		}
	}
}