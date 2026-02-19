using System;
using Selerant.ApplicationBlocks.Data;
using Selerant.DevEx.BusinessLayer;
using Selerant.DevEx.Modules.DIRModule.BackEnd.AssessmentManagement.BusinessObjects;

namespace Selerant.DevEx.Modules.DIRModule.BackEnd.OutputManagement
{
    [IsDxObject]
    [BoundTable(typeof(BizDsOutput), TableName = "DXDIR_OUTPUT")]
    public partial class DxOutput : DxObject
    {
        #region Constants

        private const string configElementName = "dxOutput";

        #endregion

        #region Fields

        [BoundColumn("ID", true, 0, DbType = GenericDbType.Decimal)]
        protected decimal id;

        [BoundColumn("ASSESSMENT_CODE", DbType = GenericDbType.Varchar)]
        protected string assessmentCode;

        [BoundColumn("OUTPUT_CATEGORY_ID", DbType = GenericDbType.Decimal)]
        protected decimal outputCategoryId;

		[BoundColumn("DESTINATION_CODE", DbType = GenericDbType.Varchar)]
		protected string destinationCode;

		[BoundColumn("ASSESSMENT_LC_STAGE_ID", DbType = GenericDbType.Decimal)]
        protected decimal assessmentLcStageId;

		[BoundColumn("INPUT_ID", DbType = GenericDbType.Decimal)]
		protected Nullable<decimal> inputId;

		[BoundColumn("MAT_PLANT", DbType = GenericDbType.Varchar)]
        protected string materialPlant;

        [BoundColumn("MAT_CODE", DbType = GenericDbType.Varchar)]
        protected string materialCode;

        [BoundColumn("OUTPUT_COST", DbType = GenericDbType.Decimal)]
        protected Nullable<decimal> outputCost;

        [BoundColumn("COST", DbType = GenericDbType.Decimal)]
        protected Nullable<decimal> cost;

        [BoundColumn("INCOME", DbType = GenericDbType.Decimal)]
        protected Nullable<decimal> income;

        [BoundColumn("WEIGHT", DbType = GenericDbType.Decimal)]
        protected Nullable<decimal> weight;

		[BoundColumn("SORT_ORDER", DbType = GenericDbType.Int)]
		protected Nullable<decimal> sortOrder;

		protected DxMaterial material;
        protected DxAssessmentLcStage lcStage;

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

        [BoundProperty(new string[] { nameof(outputCategoryId) })]
        public decimal OutputCategoryId
        {
            get { return outputCategoryId; }
            set { Change(x => outputCategoryId = x, value); }
        }

		[BoundProperty(new string[] { nameof(destinationCode) })]
		public string DestinationCode
		{
			get { return destinationCode; }
			set { Change(x => destinationCode = x, value); }
		}

		[BoundProperty(new string[] { nameof(assessmentLcStageId) })]
        public decimal AssessmentLcStageId
        {
            get { return assessmentLcStageId; }
            set { Change(x => assessmentLcStageId = x, value); }
        }

		[BoundProperty(new string[] { nameof(inputId) })]
		public decimal? InputId
		{
			get { return inputId; }
			set { Change(x => inputId = x, value); }
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
				if (value != null)
				{
					materialPlant = value.Plant;
					materialCode = value.Code;
					Change(x => material = x, value);
				}
            }
        }

        [BoundProperty(new string[] { nameof(outputCost) })]
        public decimal? OutputCost
        {
            get { return outputCost; }
            set { Change(x => outputCost = x, value); }
        }

        [BoundProperty(new string[] { nameof(cost) })]
        public decimal? Cost
        {
            get { return cost; }
            set { Change(x => cost = x, value); }
        }

		[BoundProperty(new string[] { nameof(income) })]
		public decimal? Income
		{
			get { return income; }
			set { Change(x => income = x, value); }
		}

		[BoundProperty(new string[] { nameof(weight) })]
		public decimal? Weight
		{
			get { return weight; }
			set { Change(x => weight = x, value); }
		}

		[BoundProperty(new string[] { nameof(sortOrder) })]
		public decimal? SortOrder
		{
			get { return sortOrder; }
			set { Change(x => sortOrder = x, value); }
		}

		[BoundRelation(new string[] { nameof(assessmentLcStageId) }, new string[] { "id" })]
        [BoundProperty(new string[] { nameof(assessmentLcStageId) })]
        public DxAssessmentLcStage LcStage
        {
            get
            {
                if (lcStage == null)
                    lcStage = new DxAssessmentLcStage(assessmentLcStageId);

                return lcStage;
            }
            set
            {
                Change(x => lcStage = x, value);
                assessmentLcStageId = value.Id;
            }
        }

        #endregion

        #region Constructor

        public DxOutput() : base()
        {
            AutogeneratedIdentityKey = true;
        }

        public DxOutput(decimal id) : this(id, false)
        {
        }

        public DxOutput(decimal id, bool load) : this()
        {
            this.id = id;
            if (load)
                Load();
        }

        #endregion

        #region Overrides

        protected override bool SetNextIdentityKeyDirect()
        {
            var bizDsObj = new BizDsOutput();
            SetIdentityKeyValues(new object[] { bizDsObj.GetNextId() });
            return true;
        }

        #endregion

        #region Configuration

        protected override void OnConfigure()
        {
            base.OnConfigure();
            Configure();
        }

        private void Configure()
        {
            ReadConfigurationElement(configElementName);
            //Add any specific configuration
        }

		#endregion

		#region Static Methods

		public static void InsertOrUpdateNonWasteRecord(string assessmentCode, string destinationCode, decimal assessmentLcStageId)
		{
			var bizDsObj = new BizDsOutput();
			bizDsObj.InsertOrUpdateNonWasteRecord(assessmentCode, destinationCode, assessmentLcStageId);
		}

		#endregion
	}
}
