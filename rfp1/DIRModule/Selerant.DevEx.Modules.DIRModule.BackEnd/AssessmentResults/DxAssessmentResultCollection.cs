using IQToolkit;
using Selerant.DevEx.BusinessLayer;
using Selerant.DevEx.BusinessLayer.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Selerant.DevEx.Modules.DIRModule.BackEnd.AssessmentResults
{
    [Serializable]
    [IsDxObjectCollection]
    [BoundBusinessObject(typeof(DxAssessmentResult))]
    public partial class DxAssessmentResultCollection : DxObjectCollection
    {
        #region Constants

        private const string configElementName = "dxAssessmentResultCollection";

        #endregion

        #region Queries

        protected static readonly Func<string, decimal, IQueryable<DxAssessmentResult>> queryByAssessmentCodeAndAssessmentLcStageId = QueryCompiler.Compile((string assessmentCode, decimal assessmentLcStageId) =>
        (from o in new DxQueryable<DxAssessmentResult>()
         where o.AssessmentCode == assessmentCode && o.AssessmentLcStageId == assessmentLcStageId
         select o));

        protected static readonly Func<string, IQueryable<DxAssessmentResult>> queryByAssessmentCode = QueryCompiler.Compile((string assessmentCode) =>
        (from o in new DxQueryable<DxAssessmentResult>()
         where o.AssessmentCode == assessmentCode
         select o));

        #endregion

        public enum Filter
        {
            AssessmentCode,
            AssessmentCodeAndAssessmentLcStageId
        }

        #region Constructors

        public DxAssessmentResultCollection() : base()
        {

        }

        public DxAssessmentResultCollection(string assessmentCode, bool load = false) : this()
        {
            loadFilter = new LoadFilter(Filter.AssessmentCode, new object[] { assessmentCode });

            if (load)
                Load();
        }

        public DxAssessmentResultCollection(string assessmentCode, decimal assessmentLcStageId, bool load = false) : this()
        {
            loadFilter = new LoadFilter(Filter.AssessmentCodeAndAssessmentLcStageId, new object[] { assessmentCode, assessmentLcStageId });

            if (load)
                Load();
        }

        #endregion

        #region Indexers Override

        public DxAssessmentResult this[decimal id] => (DxAssessmentResult)base[DxObject.GetIdentityKey(id)];

        public new DxAssessmentResult this[int index] => (DxAssessmentResult)base[index];

        #endregion

        #region LoadItems Methods

        public List<DxAssessmentResultRow> LoadItemsResultRow()
        {
            if (Count == 0)
                return new List<DxAssessmentResultRow>();

            DxAssessmentResultRowCollection resultRows = new DxAssessmentResultRowCollection();

            foreach (var result in this)
            {
                resultRows.Add(new DxAssessmentResultRow(result.ResultRowId));
            }

            resultRows.LoadItems();

            foreach (var result in this)
            {
                result.ResultRow = resultRows[result.ResultRowId];
            }

            return resultRows.ToList();
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
                    case Filter.AssessmentCodeAndAssessmentLcStageId:
                        return queryByAssessmentCodeAndAssessmentLcStageId((string)loadFilter.Parameters[0], (decimal)loadFilter.Parameters[1]);
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

        private void Configure()
        {
            ReadConfigurationElement(configElementName);
        }

        #endregion
    }
}
