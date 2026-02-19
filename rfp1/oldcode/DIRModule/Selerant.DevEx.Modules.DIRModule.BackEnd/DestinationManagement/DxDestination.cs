using System;
using Selerant.ApplicationBlocks.Data;
using Selerant.DevEx.BusinessLayer;

namespace Selerant.DevEx.Modules.DIRModule.BackEnd.DestinationManagement
{
	[IsDxObject]
	[BoundTable(typeof(BizDsDestination), TableName = "DXDIR_DESTINATION")]
	public partial class DxDestination : DxObject
	{
		#region Constants

		private const string configElementName = "dxDestination";

		#endregion

		#region Enum

		[Flags]
		public enum DestinationUsedOn
		{
			Food = 1 << 0,
			NonFood = 1 << 1,
			Both = Food | NonFood
		}

		#endregion

		#region Fields

		[BoundColumn("CODE", true, 0, DbType = GenericDbType.Varchar)]
		protected string code;

		[BoundColumn("WASTE", DbType = GenericDbType.Int)]
		protected decimal waste;

		[BoundColumn("USED_ON", DbType = GenericDbType.Int)]
		protected decimal usedOn;

		[BoundColumn("TITLE", DbType = GenericDbType.Varchar)]
		protected string title;

		[BoundColumn("SORT_ORDER", DbType = GenericDbType.Int)]
		protected decimal sortOrder;

		#endregion

		#region Properties

		[BoundProperty(new string[] { nameof(code) })]
		public string Code
		{
			get { return code; }
			set { Change(x => code = x, value); }
		}

		[BoundProperty(new string[] { nameof(waste) })]
		public decimal Waste
		{
			get { return waste; }
			set { Change(x => waste = x, value); }
		}

		[BoundProperty(new string[] { nameof(usedOn) })]
		public DestinationUsedOn UsedOn
		{
			get { return (DestinationUsedOn)usedOn; }
			set { Change(x => usedOn = (decimal)x, value); }
		}

		[BoundProperty(new string[] { nameof(title) })]
		public string Title
		{
			get { return title; }
			set { Change(x => title = x, value); }
		}

		[BoundProperty(new string[] { nameof(sortOrder) })]
		public decimal SortOrder
		{
			get { return sortOrder; }
			set { Change(x => sortOrder = x, value); }
		}

		#endregion

		#region Constructor

		public DxDestination() : base()
		{
		}

		public DxDestination(string code) : this(code, false)
		{
		}

		public DxDestination(string code, bool load) : this()
		{
			this.code = code;

			if (load)
				Load();
		}

		#endregion

		#region Overrides

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
	}
}
