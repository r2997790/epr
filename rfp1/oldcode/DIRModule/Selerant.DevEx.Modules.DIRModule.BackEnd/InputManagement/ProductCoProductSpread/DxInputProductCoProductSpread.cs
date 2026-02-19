using System;
using System.Collections.Generic;
using Selerant.ApplicationBlocks.Data;
using Selerant.DevEx.BusinessLayer;

namespace Selerant.DevEx.Modules.DIRModule.BackEnd.InputManagement
{
	[IsDxObject]
	[BoundTable(typeof(BizDsInputProductCoProductSpread), TableName = "DXDIR_INPUT_PROD_COPROD_SPREAD")]
	public partial class DxInputProductCoProductSpread : DxObject
	{
		#region Fields

		[BoundColumn("INPUT_ID", true, 0, DbType = GenericDbType.Decimal)]
		protected decimal inputId;

		[BoundColumn("DESTINATION_CODE", true, 1, DbType = GenericDbType.Varchar)]
		protected string destinationCode;

		#endregion

		#region Properties

		[BoundProperty(new string[] { nameof(inputId) })]
		public decimal InputId
		{
			get { return inputId; }
			set { Change(x => inputId = x, value); }
		}

		[BoundProperty(new string[] { nameof(destinationCode) })]
		public string DestinationCode
		{
			get { return destinationCode; }
			set { Change(x => destinationCode = x, value); }
		}

		#endregion

		#region Constructor

		public DxInputProductCoProductSpread() : base()
		{
		}

		public DxInputProductCoProductSpread(decimal inputId, string destinationCode, bool load = false) : this()
		{
			this.inputId = inputId;
			this.destinationCode = destinationCode;

			if (load)
				Load();
		}

		#endregion
	}
}
