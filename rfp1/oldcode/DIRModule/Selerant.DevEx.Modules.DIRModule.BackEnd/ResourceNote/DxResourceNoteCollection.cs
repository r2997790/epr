using IQToolkit;
using Selerant.DevEx.BusinessLayer;
using Selerant.DevEx.BusinessLayer.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Selerant.DevEx.Modules.DIRModule.BackEnd.ResourceNote
{
	[IsDxObjectCollection]
	[BoundBusinessObject(typeof(DxResourceNote))]
	public partial class DxResourceNoteCollection : DxObjectCollection
	{
		#region Constants

		private const string configElementName = "dxResourceNoteCollection";

		#endregion

		#region Queries

		protected static readonly Func<string, IQueryable<DxResourceNote>> queryByAssessmentCode
			= QueryCompiler.Compile((string assessmentCode) =>
				(from o in new DxQueryable<DxResourceNote>()
				 where o.AssessmentCode == assessmentCode
				 select o));

		protected static readonly Func<decimal, IQueryable<DxResourceNote>> queryByLCStageId
			= QueryCompiler.Compile((decimal lcStageId) =>
				(from o in new DxQueryable<DxResourceNote>()
				 where o.LCStageId == lcStageId
				 select o));

		protected static readonly Func<string, decimal, string, IQueryable<DxResourceNote>> queryByAssessmentCodeAndLCStageIdandType
			= QueryCompiler.Compile((string assessmentCode, decimal lcStageId, string type) =>
				(from o in new DxQueryable<DxResourceNote>()
				 where o.AssessmentCode == assessmentCode && o.LCStageId == lcStageId && o.Type == type
				 select o));

		protected static readonly Func<string, decimal, IQueryable<DxResourceNote>> queryByAssessmentCodeAndLCStageId
			= QueryCompiler.Compile((string assessmentCode, decimal lcStageId) =>
				(from o in new DxQueryable<DxResourceNote>()
				 where o.AssessmentCode == assessmentCode && o.LCStageId == lcStageId
				 select o));

		#endregion

		#region Filter Enum

		public enum Filter
		{
			AssessmentCode,
			LCStageId,
			AssessmentCodeAndLCStageId,
			AssessmentCodeandLcStageIdandType,
			Note
		}

		#endregion

		#region Constructors

		public DxResourceNoteCollection() : base()
		{
		}

		public DxResourceNoteCollection(Filter filter, params object[] parameters)
			: this()
		{
			loadFilter = new LoadFilter(filter, parameters);
		}

		#endregion

		#region Overrides

		protected override IQueryable GetLoadQuery()
		{
			if (loadFilter != null)
			{
				switch ((Filter)loadFilter.Filter)
				{
					case Filter.AssessmentCode:
						return queryByAssessmentCode((string)loadFilter.Parameters[0]);
					case Filter.LCStageId:
						return queryByLCStageId((decimal)loadFilter.Parameters[0]);
					case Filter.AssessmentCodeAndLCStageId:
						return queryByAssessmentCodeAndLCStageId((string)loadFilter.Parameters[0], (decimal)loadFilter.Parameters[1]);
					case Filter.AssessmentCodeandLcStageIdandType:
						return queryByAssessmentCodeAndLCStageIdandType((string)loadFilter.Parameters[0], (decimal)loadFilter.Parameters[1], (string)loadFilter.Parameters[2]);
					default:
						throw new NotImplementedException($"Filter {loadFilter.Filter} is not implemented!");
				}
			}
			return null;
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
