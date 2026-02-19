using IQToolkit;
using Selerant.DevEx.BusinessLayer;
using Selerant.DevEx.BusinessLayer.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Selerant.DevEx.Modules.DIRModule.BackEnd.AssessmentTypeManagement.PartnerAssessmentType
{
    [Serializable]
    [BoundBusinessObject(typeof(DxPartnerAssessmentType))]
    [IsDxObjectCollection]
    public partial class DxPartnerAssessmentTypeCollection : DxObjectCollection
    {
        #region Queries

        protected static readonly Func<DxAssessmentType, IQueryable<DxPartnerAssessmentType>> queryByAssessmentType = QueryCompiler.Compile((DxAssessmentType assessmentType) =>
            (from o in new DxQueryable<DxPartnerAssessmentType>()
             where o.AssessmentTypeCode == assessmentType.Code
             select o));

        protected static readonly Func<string, IQueryable<DxPartnerAssessmentType>> queryByOrganization = QueryCompiler.Compile((string code) =>
            (from o in new DxQueryable<DxPartnerAssessmentType>()
             where o.PartnerOrgCode == code
             select o));

        #endregion

        #region Constants

        private const string configElementName = "dxPartnerAssessmentTypeCollection";

        #endregion

        #region Enums

        public enum Filter { AssessmentType, PartnerOrganization };

        #endregion

        public DxPartnerAssessmentTypeCollection() : base()
        {

        }

        public DxPartnerAssessmentTypeCollection(DxAssessmentType assessmentType) : this()
        {
            loadFilter = new LoadFilter(Filter.AssessmentType, new object[] { assessmentType });
        }

        public DxPartnerAssessmentTypeCollection(string partnerOrganization) : this()
        {
            loadFilter = new LoadFilter(Filter.PartnerOrganization, new object[] { partnerOrganization });
        }

        #region Overrides

        protected override IQueryable GetLoadQuery()
        {
            if (loadFilter != null)
            {
                switch ((Filter)loadFilter.Filter)
                {
                    case Filter.AssessmentType:
                        return (loadFilter.Parameters[0] is DxAssessmentType) ? queryByAssessmentType(loadFilter.Parameters[0] as DxAssessmentType) : queryByAssessmentType(new DxAssessmentType(loadFilter.Parameters[0] as string));

                    case Filter.PartnerOrganization:
                        return queryByOrganization(loadFilter.Parameters[0] as string);

                    default:
                        throw new NotImplementedException(String.Format("Filter {0} is not implemented", loadFilter.Filter));
                }
            }

            return null;
        }

        #endregion

        #region Static Methods

        public static DxPartnerAssessmentTypeCollection New()
        {
            DxPartnerAssessmentTypeCollection obj = new DxPartnerAssessmentTypeCollection();
            return (DxPartnerAssessmentTypeCollection)obj.OnCreatingObject();
        }

        public static DxPartnerAssessmentTypeCollection New(DxAssessmentType assessmentType)
        {
            DxPartnerAssessmentTypeCollection obj = new DxPartnerAssessmentTypeCollection(assessmentType);
            return (DxPartnerAssessmentTypeCollection)obj.OnCreatingObject();
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
        }

        #endregion

        #region Indexers Override

        public DxPartnerAssessmentType this[string code, string partnerOrgCode]
        {
            get
            {
                return (DxPartnerAssessmentType)base[DxObject.GetIdentityKey(code, partnerOrgCode)];
            }
        }

        public new DxPartnerAssessmentType this[int index]
        {
            get
            {
                return (DxPartnerAssessmentType)base[index];
            }
        }

        #endregion
    }
}
