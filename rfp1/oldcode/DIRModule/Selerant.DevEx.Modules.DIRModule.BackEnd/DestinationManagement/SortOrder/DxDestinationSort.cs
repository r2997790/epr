using Selerant.ApplicationBlocks.Data;
using Selerant.DevEx.BusinessLayer;

namespace Selerant.DevEx.Modules.DIRModule.BackEnd.DestinationManagement
{
	[IsDxObject]
	[BoundTable(typeof(BizDsDestinationSort), TableName = "DXDIR_DESTINATION", SourceType = BoundTableAttribute.DataSourceType.StoredProcedure)]
	public partial class DxDestinationSort : DxObject
	{
		#region Fields

		[BoundColumn("SORT_ORDER", true, 0, DbType = GenericDbType.Int)]
		protected decimal sortOrder;

		[BoundColumn("CODE", DbType = GenericDbType.Varchar)]
		protected string code;

		#endregion

		#region Properties

		[BoundProperty(new string[] { nameof(sortOrder) })]
		public decimal SortOrder
		{
			get { return sortOrder; }
		}

		[BoundProperty(new string[] { nameof(code) })]
		public string Code
		{
			get { return code; }
		}

		#endregion

		#region Constructor

		public DxDestinationSort()
		{
		}

		#endregion
	}
}
