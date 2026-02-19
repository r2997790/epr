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
    [BoundBusinessObject(typeof(DxAssessmentResultRow))]
    public partial class DxAssessmentResultRowCollection : DxObjectCollection
    {
        #region Constants

        private const string configElementName = "dxAssessmentResultRowCollection";

        #endregion

        #region Queries

        protected static readonly Func<string, IQueryable<DxAssessmentResultRow>> queryByResultType = QueryCompiler.Compile((string type) =>
        (from o in new DxQueryable<DxAssessmentResultRow>()
         where o.Type == type
         select o));

        #endregion

        public enum Filter
        {
            Type
        }

        #region Constructors

        public DxAssessmentResultRowCollection() : base()
        {

        }

        public DxAssessmentResultRowCollection(string resultType, bool load = false) : this()
        {
            loadFilter = new LoadFilter(Filter.Type, new object[] { resultType });

            if (load)
                Load();
        }

        #endregion

        protected override IQueryable GetLoadQuery()
        {
            if (loadFilter != null)
            {
                switch ((Filter)loadFilter.Filter)
                {
                    case Filter.Type:
                        return queryByResultType((string)loadFilter.Parameters[0]);
                    default:
                        throw new NotImplementedException($"Filter {loadFilter.Filter} is not implemented!");
                }
            }

            return null;
        }

        #region Indexers Override

        public DxAssessmentResultRow this[decimal id] => (DxAssessmentResultRow)base[DxObject.GetIdentityKey(id)];

        public new DxAssessmentResultRow this[int index] => (DxAssessmentResultRow)base[index];

        #endregion

        #region Overrides

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
