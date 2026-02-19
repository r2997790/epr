using Selerant.ApplicationBlocks.Data;
using Selerant.DevEx.BusinessLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Selerant.DevEx.Modules.DIRModule.BackEnd.DestinationManagement
{
    [IsDxObject]
    [BoundTable(typeof(BizDsDestination), TableName = "DXDIR_DESTINATION")]
    public partial class DxDestinationPyramidChartItem : DxDestination
	{
        [BoundColumn("WEIGHT", DbType = GenericDbType.Decimal)]
        protected NullableDecimal weight;

        [BoundProperty(new string[] { nameof(weight) })]
        public NullableDecimal Weight
        {
            get { return weight; }
            set { Change(x => weight = x, value); }
        }

        #region Constructors

        public DxDestinationPyramidChartItem() : base()
        {
        }

        public DxDestinationPyramidChartItem(string code) : this()
        {
            this.code = code;
        }

        #endregion
    }
}
