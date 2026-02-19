using Selerant.ApplicationBlocks.Data;
using Selerant.DevEx.BusinessLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Selerant.DevEx.Modules.DIRModule.BackEnd.AssessmentManagement.BusinessObjects
{
    [Serializable]
    [BoundTable(typeof(BizDsPartnerAssessment), TableName = "DXDIR_PARTNER_ASSESSMENT")]
    [IsDxObject]
    public partial class DxPartnerAssessment : DxObject
    {
        #region Constants

        private const string configElementName = "dxPartnerAssessment";

        #endregion

        #region Fields	

        [BoundColumn("ASSESSMENT_CODE", true, 0, DbType = GenericDbType.Varchar)]
        protected string assessmentCode;

        [BoundColumn("PARTNER_ORG_CODE", true, 2, DbType = GenericDbType.Varchar)]
        protected string partnerOrgCode;

        [BoundColumn("IS_SHARED")]
        protected decimal isShared;

        #endregion

        #region Properties

        [BoundProperty(new string[] { "assessmentCode" })]
        public string AssessmentCode
        {
            get { return assessmentCode; }
        }

        [BoundProperty(new string[] { "partnerOrgCode" })]
        public string PartnerOrgCode
        {
            get { return partnerOrgCode; }
        }

        [BoundProperty(new string[] { "isShared" })]
        public bool IsShared
        {
            get { return (isShared != 0); }
            set { Change(x => isShared = x, value ? 1m : 0m); }
        }

        #endregion

        #region Constructors

        public DxPartnerAssessment()
        {
        }

        public DxPartnerAssessment(string assessmentCode, string partnerOrgCode) : this(assessmentCode, partnerOrgCode, false)
        {
        }

        public DxPartnerAssessment(string assessmentCode, string partnerOrgCode, bool load) : this()
        {
            this.assessmentCode = assessmentCode;
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
            //Add any specific configuration
        }

        #endregion
    }
}
