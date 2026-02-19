using Selerant.ApplicationBlocks.Data;
using Selerant.DevEx.BusinessLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Selerant.DevEx.Modules.DIRModule.BackEnd.ResourceNote
{
	[IsDxObject]
	[BoundTable(typeof(BizDsResourceNote), TableName = "DXDIR_RESOURCE_NOTE")]
	public partial class DxResourceNote : DxObject
	{
		#region Constants

		private const string configElementName = "dxResourceNote";

		#endregion

		#region Fields

		[BoundColumn("ASSESSMENT_CODE", true, 0, DbType = GenericDbType.Varchar)]
		protected string assessmentCode;

		[BoundColumn("LC_STAGE_ID", true, 1, DbType = GenericDbType.Decimal)]
		protected decimal lcStageId;

		[BoundColumn("TYPE", true, 2, DbType = GenericDbType.Varchar)]
		protected string type;

		[BoundColumn("NOTE", DbType = GenericDbType.Varchar)]
		protected string note;

		#endregion

		#region Properties

		[BoundProperty(new string[] { nameof(assessmentCode) })]
		public string AssessmentCode
		{
			get { return assessmentCode; }
			set { Change(x => assessmentCode = x, value); }
		}

		[BoundProperty(new string[] { nameof(lcStageId) })]
		public decimal LCStageId
		{
			get { return lcStageId; }
			set { Change(x => lcStageId = x, value); }
		}

		[BoundProperty(new string[] { nameof(type) })]
		public string Type
		{
			get { return type; }
			set { Change(x => type = x, value); }
		}

		[BoundProperty(new string[] { nameof(note) })]
		public string Note
		{
			get { return note; }
			set { Change(x => note = x, value); }
		}

		#endregion

		#region Constructor

		public DxResourceNote() 
			: base()
		{
		}

		public DxResourceNote(string assessmentCode, decimal lcStageId, string type) 
			: this(assessmentCode, lcStageId, type, false)
		{
		}

		public DxResourceNote(string assessmentCode, decimal lcStageId, string type, bool load) : this()
		{
			this.assessmentCode = assessmentCode;
			this.lcStageId = lcStageId;
			this.type = type;

			if (load)
				Load();
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
