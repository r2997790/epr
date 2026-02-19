using Selerant.ApplicationBlocks.Data;
using Selerant.DevEx.BusinessLayer;
using Selerant.DevEx.Modules.DIRModule.BackEnd.DestinationManagement;

namespace Selerant.DevEx.Modules.DIRModule.BackEnd.AssessmentManagement.BusinessObjects
{
	[IsDxObject]
	[BoundTable(typeof(BizDsAssessmentDestination), TableName = "DXDIR_ASSESSMENT_DESTINATION")]
	public partial class DxAssessmentDestination : DxObject
	{
		#region Constants

		private const string configElementName = "dxAssessmentDestination";

		#endregion

		#region Fields

		[BoundColumn("ASSESSMENT_CODE", true, 0, DbType = GenericDbType.Varchar)]
		protected string assessmentCode;

		[BoundColumn("DESTINATION_CODE", true, 1, DbType = GenericDbType.Varchar)]
		protected string destinationCode;

		protected DxDestination destination;

		#endregion

		#region Properties

		[BoundProperty(new string[] { nameof(assessmentCode) })]
		public string AssessmentCode
		{
			get { return assessmentCode; }
			set { Change(x => assessmentCode = x, value); }
		}

		[BoundProperty(new string[] { nameof(destinationCode) })]
		public string DestinationCode
		{
			get { return destinationCode; }
			set { Change(x => destinationCode = x, value); }
		}

		[BoundRelation(new string[] { nameof(destinationCode) }, new string[] { "destinationCode" })]
		[BoundProperty(new string[] { nameof(destinationCode) })]
		public DxDestination Destination
		{
			get
			{
				if (destination == null)
					destination = new DxDestination(destinationCode);
				return destination;
			}
			set
			{
				Change(x => destination = x, value);
				destinationCode = value.Code;
			}
		}

		#endregion

		#region Constructor

		public DxAssessmentDestination() : base()
		{
		}

		public DxAssessmentDestination(string assessmentCode, string wasteDestinationCode) : this(assessmentCode, wasteDestinationCode, false)
		{
		}

		public DxAssessmentDestination(string assessmentCode, string wasteDestinationCode, bool load) : this()
		{
			this.assessmentCode = assessmentCode;
			this.DestinationCode = wasteDestinationCode;

			if (load)
				Load();
		}

		#endregion

		#region Overrides

		public override bool Update()
		{
			throw new System.NotImplementedException();
		}

		protected override bool UpdateDirect()
		{
			throw new System.NotImplementedException();
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
	}
}