using System;
using Selerant.ApplicationBlocks.Data;
using Selerant.DevEx.BusinessLayer;
using Selerant.DevEx.Modules.DIRModule.Configuration.Resources;

namespace Selerant.DevEx.Modules.DIRModule.BackEnd.InputManagement
{
	[IsDxObject]
	[BoundTable(typeof(BizDsCarriedInput), TableName = "DXDIR_INPUT")]
	public partial class DxCarriedInput : DxObject
	{
		#region Enum

		public enum Measure
		{
			Measured = 1,
			Projected = 2,
			Estimated = 3
		}

		#endregion

		#region Fields

		[BoundColumn("ID", true, 0, DbType = GenericDbType.Decimal)]
		protected decimal id;

		[BoundColumn("ASSESSMENT_CODE", DbType = GenericDbType.Varchar)]
		protected string assessmentCode;

		[BoundColumn("INPUT_CATEGORY_ID", DbType = GenericDbType.Decimal)]
		protected decimal inputCategoryId;

		[BoundColumn("ASSESSMENT_LC_STAGE_ID", DbType = GenericDbType.Decimal)]
		protected decimal assessmentLcStageId;

		[BoundColumn("MAT_PLANT", DbType = GenericDbType.Varchar)]
		protected string materialPlant;

		[BoundColumn("MAT_CODE", DbType = GenericDbType.Varchar)]
		protected string materialCode;

		[BoundColumn("PACKAGING", DbType = GenericDbType.Decimal)]
		protected Nullable<decimal> packaging;

		[BoundColumn("MASS", DbType = GenericDbType.Decimal)]
		protected Nullable<decimal> mass;

		[BoundColumn("COST", DbType = GenericDbType.Decimal)]
		protected Nullable<decimal> cost;

		[BoundColumn("MEASUREMENT", DbType = GenericDbType.Decimal)]
		protected decimal measurement;

        [BoundColumn("CATEGORY_SORT_ORDER", DbType = GenericDbType.Int)]
        protected decimal categorySortOrder;

        [BoundColumn("INPUT_SORT_ORDER", DbType = GenericDbType.Int)]
        protected decimal inputSortOrder;

        [BoundColumn("PRODUCT_SOURCE", DbType = GenericDbType.Varchar)]
		protected string productSource;

		protected DxInputCategory inputCategory;
		protected DxMaterial material;

		#endregion

		#region Properties

		[BoundProperty(new string[] { nameof(id) })]
		public decimal Id
		{
			get { return id; }
		}

		[BoundProperty(new string[] { nameof(assessmentCode) })]
		public string AssessmentCode
		{
			get { return assessmentCode; }
			set { Change(x => assessmentCode = x, value); }
		}

		[BoundProperty(new string[] { nameof(inputCategoryId) })]
		public decimal InputCategoryId
		{
			get { return inputCategoryId; }
			set { Change(x => inputCategoryId = x, value); }
		}

		[BoundRelation(new string[] { nameof(inputCategoryId) }, new string[] { "id" })]
		[BoundProperty(new string[] { nameof(inputCategoryId) })]
		public DxInputCategory InputCategory
		{
			get
			{
				if (inputCategory == null)
					inputCategory = new DxInputCategory(inputCategoryId);
				return inputCategory;
			}
			set
			{
				Change(x => inputCategory = x, value);
				inputCategoryId = value.Id;
			}
		}

		[BoundProperty(new string[] { nameof(assessmentLcStageId) })]
		public decimal AssessmentLcStageId
		{
			get { return assessmentLcStageId; }
			set { Change(x => assessmentLcStageId = x, value); }
		}

		[BoundProperty(new string[] { nameof(materialPlant) })]
		public string MaterialPlant
		{
			get { return materialPlant; }
		}

		[BoundProperty(new string[] { nameof(materialCode) })]
		public string MaterialCode
		{
			get { return materialCode; }
		}

		[BoundRelation(new string[] { nameof(materialPlant), nameof(materialCode) }, new string[] { "plant", "code" }, IsMandatory = true)]
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
				Change(x => material = x, value);
			}
		}

		[BoundProperty(new string[] { nameof(mass) })]
		public decimal? Mass
		{
			get { return mass; }
			set { Change(x => mass = x, value); }
		}

		[BoundProperty(new string[] { nameof(cost) })]
		public decimal? Cost
		{
			get { return cost; }
			set { Change(x => cost = x, value); }
		}

		[BoundProperty(new string[] { nameof(measurement) })]
		public Measure Measurement
		{
			get { return (Measure)measurement; }
			set { Change(x => measurement = (decimal)x, value); }
		}

		[BoundProperty(IsValid = false)]
		public string MeasurementDesc => Locale.GetString(ResourceFiles.AssessmentManager, "DXDIR_INPUT_MEASURE_" + measurement.ToString());

        [BoundProperty(new string[] { nameof(categorySortOrder) })]
        public decimal CategorySortOrder
        {
            get { return categorySortOrder; }
            set { Change(x => categorySortOrder = x, value); }
        }

        [BoundProperty(new string[] { nameof(inputSortOrder) })]
        public decimal InputSortOrder
        {
            get { return inputSortOrder; }
            set { Change(x => inputSortOrder = x, value); }
        }

        [BoundProperty(new string[] { nameof(productSource) })]
		public string ProductSource
		{
			get { return productSource; }
			set { Change(x => productSource = x, value); }
		}

		#endregion

		#region Constructor

		public DxCarriedInput()
			: base()
		{
		}

		public DxCarriedInput(decimal id):this()
		{
			this.id = id;
		}

		#endregion
	}
}
