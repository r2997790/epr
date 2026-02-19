using IQToolkit;
using Selerant.DevEx.BusinessLayer;
using Selerant.DevEx.BusinessLayer.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Selerant.DevEx.Modules.DIRModule.BackEnd.AssessmentManagement.BusinessObjects
{
    /// <summary>
	/// Summary description for DxPartnerAssessmentCollection.
	/// </summary>
	[Serializable]
    [BoundBusinessObject(typeof(DxPartnerAssessment))]
    [IsDxObjectCollection]
    public partial class DxPartnerAssessmentCollection : DxObjectCollection
    {
        #region Queries

        protected static readonly Func<DxAssessment, IQueryable<DxPartnerAssessment>> queryByAssessment
            = QueryCompiler.Compile((DxAssessment assessment) =>
                (from o in new DxQueryable<DxPartnerAssessment>()
                 where o.AssessmentCode == assessment.Code
                 select o));

        protected static readonly Func<string, IQueryable<DxPartnerAssessment>> queryByOrganization
             = QueryCompiler.Compile((string code) =>
                 (from o in new DxQueryable<DxPartnerAssessment>()
                  where o.PartnerOrgCode == code
                  select o));

        #endregion

        #region Constants

        private const string configElementName = "dxPartnerAssessmentCollection";

        #endregion

        #region Enums

        public enum Filter { Assessment, PartnerOrganization };

        #endregion

        #region Constructors

        public DxPartnerAssessmentCollection() : base()
        {
        }

        public DxPartnerAssessmentCollection(DxAssessment dxAssessment)
            : this()
        {
            loadFilter = new LoadFilter(Filter.Assessment, new object[] { dxAssessment });
        }

        public DxPartnerAssessmentCollection(Filter filter, params object[] parameters)
            : base()
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
                    case Filter.Assessment:
                        return (loadFilter.Parameters[0] is DxAssessment) ? queryByAssessment(loadFilter.Parameters[0] as DxAssessment) : queryByAssessment(new DxAssessment(loadFilter.Parameters[0] as string));

                    case Filter.PartnerOrganization:
                        return queryByOrganization(loadFilter.Parameters[0] as string);

                    default:
                        throw new NotImplementedException(String.Format("Filter {0} is not implemented", loadFilter.Filter));
                }
            }

            return null;
        }

        #endregion

        #region Static methods

        public static DxPartnerAssessmentCollection New()
        {
            DxPartnerAssessmentCollection obj = new DxPartnerAssessmentCollection();
            return (DxPartnerAssessmentCollection)obj.OnCreatingObject();
        }

        public static DxPartnerAssessmentCollection New(DxAssessment dxAssessment)
        {
            DxPartnerAssessmentCollection obj = new DxPartnerAssessmentCollection(dxAssessment);
            return (DxPartnerAssessmentCollection)obj.OnCreatingObject();
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

        public DxPartnerAssessment this[string code, string partnerOrgCode]
        {
            get
            {
                return (DxPartnerAssessment)base[DxObject.GetIdentityKey(code, partnerOrgCode)];
            }
        }

        public new DxPartnerAssessment this[int index]
        {
            get
            {
                return (DxPartnerAssessment)base[index];
            }
        }

        #endregion
    }
}
