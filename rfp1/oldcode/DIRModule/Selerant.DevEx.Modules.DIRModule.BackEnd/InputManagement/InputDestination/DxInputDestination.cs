using Selerant.ApplicationBlocks.Data;
using Selerant.DevEx.BusinessLayer;

namespace Selerant.DevEx.Modules.DIRModule.BackEnd.InputManagement
{
	[IsDxObject]
	[BoundTable(typeof(BizDsInputDestination), TableName = "DXDIR_INPUT_DESTINATION")]
	public partial class DxInputDestination : DxObject
	{
		#region Constants

		private const string configElementName = "dxInputDestination";

		#endregion

		#region Fields

		[BoundColumn("INPUT_ID", true, 0, DbType = GenericDbType.Decimal)]
		protected decimal inputId;

		[BoundColumn("DESTINATION_CODE", true, 1, DbType = GenericDbType.Varchar)]
		protected string destinationCode;

		[BoundColumn("OUTPUT_CATEGORY_ID", true, 2, DbType = GenericDbType.Decimal)]
		protected decimal outputCategoryId;

		[BoundColumn("PERCENTAGE", DbType = GenericDbType.Decimal)]
		protected decimal percentage;

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

		[BoundProperty(new string[] { nameof(outputCategoryId) })]
		public decimal OutputCategoryId
		{
			get { return outputCategoryId; }
			set { Change(x => outputCategoryId = x, value); }
		}

		[BoundProperty(new string[] { nameof(percentage) })]
		public decimal Percentage
		{
			get { return percentage; }
			set { Change(x => percentage = x, value); }
		}

		#endregion

		#region Constructor

		public DxInputDestination() : base()
		{
		}

		public DxInputDestination(decimal inputId, string destinationCode, decimal outputCategoryId) : this(inputId, destinationCode, outputCategoryId, false)
		{
		}

		public DxInputDestination(decimal inputId, string destinationCode, decimal outputCategoryId, bool load) : this()
		{
			this.inputId = inputId;
			this.destinationCode = destinationCode;
			this.outputCategoryId = outputCategoryId;

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

		#region Static Methods

		public static void DeleteByDestination(string assessmentCode, string destinationCode)
		{
			var bizDsInputDestination = new BizDsInputDestination();

			bizDsInputDestination.DeleteByDestination(assessmentCode, destinationCode);
		}

		#endregion
	}
}
