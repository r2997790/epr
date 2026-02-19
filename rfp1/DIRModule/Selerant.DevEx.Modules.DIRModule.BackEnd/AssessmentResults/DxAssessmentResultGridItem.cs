using Selerant.ApplicationBlocks.Data;
using Selerant.DevEx.BusinessLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Selerant.DevEx.Modules.DIRModule.BackEnd.AssessmentResults
{
    [IsDxObject]
    [BoundTable(typeof(BizDsAssessmentResultRow), TableName = "DXDIR_RESULT_ROW")]
    public partial class DxAssessmentResultGridItem : DxAssessmentResultRow
    {
        [BoundColumn("RESULT", DbType = GenericDbType.Decimal)]
        protected NullableDecimal result;

        [BoundProperty(new string[] { nameof(result) })]
        public NullableDecimal Result
        {
            get { return result; }
        }

        #region Constructors

        public DxAssessmentResultGridItem() : base()
        {

        }

        public DxAssessmentResultGridItem(decimal id) : this()
        {
            this.id = id;
        }

        #endregion
    }
}
