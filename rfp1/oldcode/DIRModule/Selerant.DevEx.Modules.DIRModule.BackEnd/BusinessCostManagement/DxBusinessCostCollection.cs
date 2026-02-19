using IQToolkit;
using Selerant.DevEx.BusinessLayer;
using Selerant.DevEx.BusinessLayer.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Selerant.DevEx.Modules.DIRModule.BackEnd.BusinessCostManagement
{
	[IsDxObjectCollection]
	[BoundBusinessObject(typeof(DxBusinessCost))]
	public partial class DxBusinessCostCollection : DxObjectCollection
	{
		#region Constants

		private const string configElementName = "dxBusinessCostCollection";

		#endregion

		#region Queries

		protected static readonly Func<string, decimal, IQueryable<DxBusinessCost>> queryByAssessmentCodeAndAssessmentLcStageId = QueryCompiler.Compile((string assessmentCode, decimal assessmentLcStageId) =>
		(from o in new DxQueryable<DxBusinessCost>()
		 where o.AssessmentCode == assessmentCode && o.AssessmentLcStageId == assessmentLcStageId
		 select o));

		protected static readonly Func<string, IQueryable<DxBusinessCost>> queryByAssessmentCode 
			= QueryCompiler.Compile((string assessmentCode) =>
				(from o in new DxQueryable<DxBusinessCost>()
				where o.AssessmentCode == assessmentCode
				select o));

        protected static readonly Func<string, decimal, IQueryable<decimal>> querySortOrderByAssessmentCodeAndAssessmentLcStageId = QueryCompiler.Compile((string assessmentCode, decimal assessmentLcStageId) =>
        (from o in new DxQueryable<DxBusinessCost>()
         where o.AssessmentCode == assessmentCode && o.AssessmentLcStageId == assessmentLcStageId
         select o.SortOrder));

        protected static readonly Func<string, decimal, IQueryable<string>> queryCodesByAssessmentCodeAndAssessmentLcStageId = QueryCompiler.Compile((string assessmentCode, decimal assessmentLcStageId) =>
        (from o in new DxQueryable<DxBusinessCost>()
         where o.AssessmentCode == assessmentCode && o.AssessmentLcStageId == assessmentLcStageId
         select o.Title));

		#endregion

		public enum Filter
		{
			AssessmentCode,
			AssessmentCodeAndAssessmentLcStageId
		}

		#region Constructors

		public DxBusinessCostCollection() : base()
		{

		}

		public DxBusinessCostCollection(string assessmentCode, decimal assessmentLcStageId, bool load = false) : this()
		{
			loadFilter = new LoadFilter(Filter.AssessmentCodeAndAssessmentLcStageId, new object[] { assessmentCode, assessmentLcStageId });

			if (load)
				Load();
		}

		public DxBusinessCostCollection(string assessmentCode, bool load = false) : this()
		{
			loadFilter = new LoadFilter(Filter.AssessmentCode, new object[] { assessmentCode });

            if (load)
                Load();
		}

		#endregion

		#region Overrides

		protected override IQueryable GetLoadQuery()
		{
			if (loadFilter != null)
			{
				switch ((Filter)loadFilter.Filter)
				{
					case Filter.AssessmentCodeAndAssessmentLcStageId:
						return queryByAssessmentCodeAndAssessmentLcStageId((string)loadFilter.Parameters[0], (decimal)loadFilter.Parameters[1]);
					case Filter.AssessmentCode:
						return queryByAssessmentCode((string)loadFilter.Parameters[0]);
					default:
						throw new NotImplementedException($"Filter {loadFilter.Filter} is not implemented!");
				}
			}

			return null;
		}

		protected override void OnConfigure()
		{
			base.OnConfigure();
			Configure();
		}

        #endregion

        #region Static Methods

        public static decimal GetNextSortOrder(string assessmentCode, decimal lcStageId)
        {
            return querySortOrderByAssessmentCodeAndAssessmentLcStageId(assessmentCode, lcStageId).DefaultIfEmpty(0).Max() + 1m;
        }

        public static IEnumerable<string> GetBusinessCostCodes(string assessmentCode, decimal lcStageId)
        {
            return queryCodesByAssessmentCodeAndAssessmentLcStageId(assessmentCode, lcStageId).ToList();
        }

        #endregion

        #region Private Methods
        private void Configure()
        {
            ReadConfigurationElement(configElementName);
        } 
        #endregion
    }
}
