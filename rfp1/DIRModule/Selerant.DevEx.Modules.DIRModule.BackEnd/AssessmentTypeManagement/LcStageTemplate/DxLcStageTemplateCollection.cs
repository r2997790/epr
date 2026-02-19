using IQToolkit;
using Selerant.DevEx.BusinessLayer;
using Selerant.DevEx.BusinessLayer.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Selerant.DevEx.Modules.DIRModule.BackEnd.AssessmentTypeManagement.LcStageTemplate
{
    [Serializable]
    [IsDxObjectCollection]
    [BoundBusinessObject(typeof(DxLcStageTemplate))]
    public partial class DxLcStageTemplateCollection : DxObjectCollection
    {
        #region Constants

        private const string configElementName = "dxLcStageTemplateCollection";

        #endregion

        #region Queries

        protected static readonly Func<string, IQueryable<DxLcStageTemplate>> queryByAssessmentTypeCode
            = QueryCompiler.Compile((string assessmentTypeCode) =>
                (from o in new DxQueryable<DxLcStageTemplate>()
                 where o.AssessmentTypeCode == assessmentTypeCode
                 select o));

        #endregion

        #region Filter Enum

        public enum Filter
        {
            AssessmentTypeCode
        }

        #endregion

        #region Constructors

        public DxLcStageTemplateCollection() : base()
        {

        }

        public DxLcStageTemplateCollection(DxAssessmentType assessmentType)
            : this(assessmentType.Code)
        {
            
        }

        public DxLcStageTemplateCollection(string assessmentTypeCode) : this()
        {
            loadFilter = new LoadFilter(Filter.AssessmentTypeCode, new[] { assessmentTypeCode });
        }

        #endregion

        #region Indexers Override

        public DxLcStageTemplate this[decimal id] => (DxLcStageTemplate)base[DxObject.GetIdentityKey(id)];

        public new DxLcStageTemplate this[int index] => (DxLcStageTemplate)base[index];

        #endregion

        #region Overrides

        protected override IQueryable GetLoadQuery()
        {
            if (loadFilter != null)
            {
                switch ((Filter)loadFilter.Filter)
                {
                    case Filter.AssessmentTypeCode:
                        return queryByAssessmentTypeCode((string)loadFilter.Parameters[0]);
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
