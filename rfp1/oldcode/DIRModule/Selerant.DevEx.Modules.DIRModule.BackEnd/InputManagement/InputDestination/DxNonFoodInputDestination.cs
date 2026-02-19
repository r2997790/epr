using System;
using Selerant.ApplicationBlocks.Data;
using Selerant.DevEx.BusinessLayer;
using Selerant.DevEx.Modules.DIRModule.Configuration.Resources;

namespace Selerant.DevEx.Modules.DIRModule.BackEnd.InputManagement
{
	[IsDxObject]
	[BoundTable(typeof(BizDsInputDestination), TableName = "DXDIR_PK_INPUT_DESTINATION", SourceType = BoundTableAttribute.DataSourceType.StoredProcedure)]
	public partial class DxNonFoodInputDestination : DxObject, IInputDestination
	{
		#region Fields

		[BoundColumn("INPUT_ID", true, 0, DbType = GenericDbType.Decimal)]
		protected decimal inputId;

		[BoundColumn("OUTPUT_CATEGORY_ID", true, 1, DbType = GenericDbType.Decimal)]
		protected decimal outputCategoryId;

		[BoundColumn("OUTPUT_CATEGORY_TITLE", DbType = GenericDbType.Varchar)]
		protected string outputCategoryTitle;

		[BoundColumn("MAT_PLANT", DbType = GenericDbType.Varchar)]
		protected string materialPlant;

		[BoundColumn("MAT_CODE", DbType = GenericDbType.Varchar)]
		protected string materialCode;

        [BoundColumn("PRODUCT_SOURCE", DbType = GenericDbType.Varchar)]
        protected string productSource;

        [BoundColumn("PRODUCT", DbType = GenericDbType.Decimal)]
		protected Nullable<decimal> product;

		[BoundColumn("PRODUCT_2", DbType = GenericDbType.Decimal)]
		protected Nullable<decimal> product2;

		[BoundColumn("PRODUCT_3", DbType = GenericDbType.Decimal)]
		protected Nullable<decimal> product3;

		[BoundColumn("COPRODUCT", DbType = GenericDbType.Decimal)]
		protected Nullable<decimal> coproduct;

		[BoundColumn("COPRODUCT_2", DbType = GenericDbType.Decimal)]
		protected Nullable<decimal> coproduct2;

		[BoundColumn("FOOD_RESCUE", DbType = GenericDbType.Decimal)]
		protected Nullable<decimal> foodRescue;

        [BoundColumn("ANIMAL_FEED", DbType = GenericDbType.Decimal)]
        protected Nullable<decimal> animalFeed;

        [BoundColumn("BIOMASS_MATERIAL", DbType = GenericDbType.Decimal)]
        protected Nullable<decimal> biomassMaterial;

        [BoundColumn("CODIGESTION_ANAEROBIC", DbType = GenericDbType.Decimal)]
        protected Nullable<decimal> codigestionAnaerobic;

        [BoundColumn("COMPOSTING", DbType = GenericDbType.Decimal)]
        protected Nullable<decimal> composting;

        [BoundColumn("COMBUSTION", DbType = GenericDbType.Decimal)]
        protected Nullable<decimal> combustion;

        [BoundColumn("LAND_APP", DbType = GenericDbType.Decimal)]
        protected Nullable<decimal> landApplication;

        [BoundColumn("RECYCLING", DbType = GenericDbType.Decimal)]
		protected Nullable<decimal> recycling;

		[BoundColumn("INCIN_WITH_EN_RECOVER", DbType = GenericDbType.Decimal)]
		protected Nullable<decimal> incinWithEnRecover;

		[BoundColumn("LANDFILL", DbType = GenericDbType.Decimal)]
		protected Nullable<decimal> landfill;

        [BoundColumn("NOT_HARVESTED", DbType = GenericDbType.Decimal)]
        protected Nullable<decimal> notHarvested;

        [BoundColumn("REFUSE_DISCARD", DbType = GenericDbType.Decimal)]
        protected Nullable<decimal> refuseDiscard;

        [BoundColumn("SEWER", DbType = GenericDbType.Decimal)]
		protected Nullable<decimal> sewer;

        [BoundColumn("ENVIRONMENT_LOSS", DbType = GenericDbType.Decimal)]
        protected Nullable<decimal> environmentLoss;

        [BoundColumn("OTHER", DbType = GenericDbType.Decimal)]
		protected Nullable<decimal> other;

		[BoundColumn("PART_OF_PRODUCT_COPRODUCT", DbType = GenericDbType.Decimal)]
		protected decimal partOfProductCoproduct;

		protected DxMaterial material;

		#endregion

		#region Properties

		[BoundProperty(new string[] { nameof(inputId) })]
		public decimal InputId => inputId;

		[BoundProperty(new string[] { nameof(outputCategoryId) })]
		public decimal OutputCategoryId => outputCategoryId;

		[BoundProperty(new string[] { nameof(outputCategoryTitle) })]
		public string OutputCategoryTitle => Locale.GetString(ResourceFiles.AssessmentManager, "DXDIR_OUTPUT_CATEGORY_" + outputCategoryTitle);

		[BoundProperty(new string[] { nameof(materialPlant) })]
		public string MaterialPlant => materialPlant;

		[BoundProperty(new string[] { nameof(materialCode) })]
		public string MaterialCode => materialCode;

        [BoundProperty(new string[] { nameof(productSource) })]
        public string ProductSource => productSource;

        [BoundProperty(new string[] { nameof(product) })]
		public decimal? Product => product;

		[BoundProperty(new string[] { nameof(product) })]
		public decimal? Product2 => product2;

		[BoundProperty(new string[] { nameof(product) })]
		public decimal? Product3 => product3;

		[BoundProperty(new string[] { nameof(coproduct) })]
		public decimal? Coproduct => coproduct;

		[BoundProperty(new string[] { nameof(coproduct) })]
		public decimal? Coproduct2 => coproduct2;

		[BoundProperty(new string[] { nameof(foodRescue) })]
		public decimal? FoodRescue => foodRescue;

        [BoundProperty(new string[] { nameof(animalFeed) })]
        public decimal? AnimalFeed => animalFeed;

        [BoundProperty(new string[] { nameof(biomassMaterial) })]
        public decimal? BiomassMaterial => biomassMaterial;

        [BoundProperty(new string[] { nameof(codigestionAnaerobic) })]
        public decimal? CodigestionAnaerobic => codigestionAnaerobic;

        [BoundProperty(new string[] { nameof(composting) })]
        public decimal? Composting => composting;

        [BoundProperty(new string[] { nameof(combustion) })]
        public decimal? Combustion => combustion;

        [BoundProperty(new string[] { nameof(landApplication) })]
        public decimal? LandApplication => landApplication;

        [BoundProperty(new string[] { nameof(recycling) })]
		public decimal? Recycling => recycling;

		[BoundProperty(new string[] { nameof(incinWithEnRecover) })]
		public decimal? IncinWithEnRecover => incinWithEnRecover;

		[BoundProperty(new string[] { nameof(landfill) })]
		public decimal? Landfill => landfill;

        [BoundProperty(new string[] { nameof(notHarvested) })]
        public decimal? NotHarvested => notHarvested;

        [BoundProperty(new string[] { nameof(refuseDiscard) })]
        public decimal? RefuseDiscard => refuseDiscard;

        [BoundProperty(new string[] { nameof(sewer) })]
		public decimal? Sewer => sewer;

        [BoundProperty(new string[] { nameof(environmentLoss) })]
        public decimal? EnvironmentLoss => environmentLoss;

        [BoundProperty(new string[] { nameof(other) })]
		public decimal? Other => other;

		[BoundProperty(new string[] { nameof(partOfProductCoproduct) })]
		public bool PartOfProductCoproduct => partOfProductCoproduct == 1.0m;

		[BoundRelation(new string[] { nameof(materialPlant), nameof(materialCode) }, new string[] { "plant", "code" })]
		[BoundProperty(new string[] { nameof(materialPlant), nameof(materialCode) })]
		public DxMaterial Material
		{
			get
			{
				if (material == null && !string.IsNullOrEmpty(materialPlant) && !string.IsNullOrEmpty(materialCode))
					material = new DxMaterial(materialPlant, materialCode);

				return material;
			}
			set
			{
				if (value == null)
					throw new ArgumentException("Attempt to set an invalid material");

				materialPlant = value.Plant;
				materialCode = value.Code;
				material = value;
			}
		}

		#endregion

		#region Constructors

		public DxNonFoodInputDestination() : base()
		{
		}

		public DxNonFoodInputDestination(decimal inputId, decimal outputCategoryId) : this(inputId, outputCategoryId, false)
		{
		}

		public DxNonFoodInputDestination(decimal inputId, decimal outputCategoryId, bool load) : this()
		{
			this.inputId = inputId;
			this.outputCategoryId = outputCategoryId;

			if (load)
				Load();
		}

		#endregion
	}
}
