using Selerant.ApplicationBlocks.Data;
using Selerant.DevEx.BusinessLayer;
using Selerant.DevEx.Modules.DIRModule.BackEnd.AssessmentManagement.BusinessObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Selerant.DevEx.Modules.DIRModule.BackEnd.Searches
{
    [Serializable]
    [BoundTable(typeof(BizDsRecentAssessment), TableName = "DXDIR_RECENT_ASSESSMENT")]
    [IsDxObject]
    public partial class DxRecentAssessment : DxRecentObject
    {
        #region Constants

        private const string configElementName = "dxRecentAssessment";

        #endregion

        #region Fields

        [BoundColumn("CODE", true, 1, DbType = GenericDbType.Varchar)]
        protected string code;

        protected DxAssessment assessment;

        #endregion

        #region Properties

        [BoundProperty(new string[] { nameof(code) })]
        public string Code
        {
            get { return code; }
        }

        [BoundProperty(new string[] { nameof(code) })]
        [BoundRelation(new string[] { nameof(code) }, new string[] { nameof(code) })]
        public DxAssessment Assessment
        {
            get
            {
                if (assessment == null)
                    assessment = new DxAssessment(code);

                return assessment;
            }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("Value cannot be null.");

                if (value.Code != code)
                    throw new ArgumentException("Cannot set a Assessemnt that is not related to this recent object.");

                assessment = value;
            }
        }

        #endregion

        #region Constructors

        public DxRecentAssessment() : base()
        {

        }

        public DxRecentAssessment(decimal userId, string code) : this(userId, code, false)
        {

        }

        public DxRecentAssessment(decimal userId, string code, bool load) : this()
        {
            this.userId = userId;
            this.code = code;

            if (load)
                Load();
        }

        public DxRecentAssessment(DxUser user, DxAssessment assessment) : this(user, assessment, false)
        {

        }

        public DxRecentAssessment(DxUser user, DxAssessment assessment, bool load) : this(user.Id, assessment.Code, load)
        {

        }

        #endregion

        #region Overrides

        public override DxObject BoundObject => Assessment;

        protected override void UpdateRecentList(DxUser dxUser, int maxCount)
        {
            var bizDs = new BizDsRecentAssessment();

            bizDs.UpdateList(dxUser.Id, maxCount);
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
