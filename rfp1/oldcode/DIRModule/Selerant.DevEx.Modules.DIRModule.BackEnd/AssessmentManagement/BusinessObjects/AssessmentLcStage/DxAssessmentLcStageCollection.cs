using System;
using System.Linq;
using IQToolkit;
using Selerant.DevEx.BusinessLayer;
using Selerant.DevEx.BusinessLayer.Linq;

namespace Selerant.DevEx.Modules.DIRModule.BackEnd.AssessmentManagement.BusinessObjects
{
	[IsDxObjectCollection]
	[BoundBusinessObject(typeof(DxAssessmentLcStage))]
	public partial class DxAssessmentLcStageCollection : DxObjectCollection
	{
		#region Constants

		private const string configElementName = "dxAssessmentLcStageCollection";

		#endregion

		#region Queries

		protected static readonly Func<string, IQueryable<DxAssessmentLcStage>> queryByAssessmentCode
			= QueryCompiler.Compile((string assessmentCode) => 
				(from o in new DxQueryable<DxAssessmentLcStage>()
				 where o.AssessmentCode == assessmentCode
				 select o));

        protected static readonly Func<string, IQueryable<decimal>> querySortOrderByAssessmentCode
            = QueryCompiler.Compile((string assessmentCode) =>
                (from o in new DxQueryable<DxAssessmentLcStage>()
                 where o.AssessmentCode == assessmentCode
                 select o.SortOrder));

		protected static readonly Func<string, decimal> queryIdOfFirstLcStageByAssessmentCode
			= QueryCompiler.Compile((string assessmentCode) =>
				(from o in new DxQueryable<DxAssessmentLcStage>()
				 where o.AssessmentCode == assessmentCode
				 orderby o.SortOrder ascending
				 select o.Id).First());

		#endregion

		#region Filter Enum
		public enum Filter
		{
			AssessmentCode
		}
		#endregion

		#region Constructors

		public DxAssessmentLcStageCollection() : base()
		{
		}

        public DxAssessmentLcStageCollection(DxAssessment assessment) : this()
        {
            loadFilter = new LoadFilter(Filter.AssessmentCode, new[] { assessment.Code });
        }

		public DxAssessmentLcStageCollection(Filter filter, params object[] parameters)
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
					default:
						throw new NotImplementedException($"Filter {loadFilter.Filter} is not implemented!");
				}
			}
			return null;
		}

        #endregion

        #region Indexers Override

        public DxAssessmentLcStage this[decimal id] => (DxAssessmentLcStage)base[DxObject.GetIdentityKey(id)];
        public new DxAssessmentLcStage this[int index] => (DxAssessmentLcStage)base[index];

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

        public static DxAssessmentLcStageCollection New(DxAssessment assessment)
        {
            DxAssessmentLcStageCollection obj = new DxAssessmentLcStageCollection(assessment);
            return (DxAssessmentLcStageCollection)obj.OnCreatingObject();
        }

        public static decimal GetNextSortOrder(string assessmentCode)
        {
            return querySortOrderByAssessmentCode(assessmentCode).DefaultIfEmpty(0).Max() + 1m;
        }

		public static decimal GetIdOfFirstBySortOrder(string assessmentCode)
		{
			return queryIdOfFirstLcStageByAssessmentCode(assessmentCode);
		}

		#endregion
	}
}
