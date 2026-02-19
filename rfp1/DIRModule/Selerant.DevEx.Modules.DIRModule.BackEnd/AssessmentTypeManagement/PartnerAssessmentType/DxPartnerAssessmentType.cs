using Selerant.ApplicationBlocks.Data;
using Selerant.DevEx.BusinessLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Selerant.DevEx.Modules.DIRModule.BackEnd.AssessmentTypeManagement.PartnerAssessmentType
{
    [Serializable]
    [BoundTable(typeof(BizDsPartnerAssessmentType), TableName = "DXDIR_PARTNER_ASSESSMENT_TYPE")]
    [IsDxObject]
    public partial class DxPartnerAssessmentType : DxObject
    {
        #region Constants

        private const string configElementName = "dxPartnerAssessmentType";

        #endregion

        #region Fields

        [BoundColumn("ASSESSMENT_TYPE_CODE", true, 0, DbType = GenericDbType.Varchar)]
        protected string assessmentTypeCode;

        [BoundColumn("PARTNER_ORG_CODE", true, 2, DbType = GenericDbType.Varchar)]
        protected string partnerOrgCode;

        [BoundColumn("IS_SHARED")]
        protected decimal isShared;

        #endregion

        #region Properties

        [BoundProperty(new string[] { nameof(assessmentTypeCode) })]
        public string AssessmentTypeCode
        {
            get { return assessmentTypeCode; }
        }

        [BoundProperty(new string[] { nameof(partnerOrgCode) })]
        public string PartnerOrgCode
        {
            get { return partnerOrgCode; }
        }

        [BoundProperty(new string[] { nameof(isShared) })]
        public bool IsShared
        {
            get { return (isShared != 0); }
            set { Change(x => isShared = x, value ? 1m : 0m); }
        }

        #endregion

        #region Constructors

        public DxPartnerAssessmentType()
        {

        }

        public DxPartnerAssessmentType(string assessmentTypeCode, string partnerOrgCode) : this(assessmentTypeCode, partnerOrgCode, false)
        {

        }

        public DxPartnerAssessmentType(string assessmentTypeCode, string partnerOrgCode, bool load) : this()
        {
            this.assessmentTypeCode = assessmentTypeCode;
            this.partnerOrgCode = partnerOrgCode;

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
        }

        #endregion
    }
}
