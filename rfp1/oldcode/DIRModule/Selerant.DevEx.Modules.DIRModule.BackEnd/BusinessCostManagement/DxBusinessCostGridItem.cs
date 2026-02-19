using Selerant.ApplicationBlocks.Data;
using Selerant.DevEx.BusinessLayer;

namespace Selerant.DevEx.Modules.DIRModule.BackEnd.BusinessCostManagement
{
    [IsDxObject]
    [BoundTable(typeof(BizDsBusinessCost), TableName = "DXDIR_BUSINESS_COST")]
    public partial class DxBusinessCostGridItem : DxBusinessCost
    {
		#region Fields

		[BoundColumn("WASTE_COST", DbType = GenericDbType.Decimal)]
        protected NullableDecimal wasteCost;

		[BoundColumn("TOTAL_COST", DbType = GenericDbType.Decimal)]
		protected NullableDecimal totalCost;

        [BoundColumn("CARRIED_COST", DbType = GenericDbType.Decimal)]
        protected NullableDecimal carriedCost;

        #endregion

        #region Properties

        [BoundProperty(new string[] { nameof(wasteCost) })]
        public NullableDecimal WasteCost
        {
            get { return wasteCost; }
			set { wasteCost = value; }
        }

		[BoundProperty(new string[] { nameof(totalCost) })]
		public NullableDecimal TotalCost
		{
			get { return totalCost; }
			set { totalCost = value; }
		}

        [BoundProperty(new string[] { nameof(carriedCost) })]
        public NullableDecimal CarriedCost
        {
            get { return carriedCost; }
            set { carriedCost = value; }
        }

        #endregion

        #region Constructors

        public DxBusinessCostGridItem() : base()
        {

        }

        public DxBusinessCostGridItem(decimal id) : this()
        {
            this.id = id;
        }

        #endregion
    }
}
