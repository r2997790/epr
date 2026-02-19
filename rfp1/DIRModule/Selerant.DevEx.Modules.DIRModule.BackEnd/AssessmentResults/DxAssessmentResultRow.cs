using Selerant.ApplicationBlocks.Data;
using Selerant.DevEx.BusinessLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Selerant.DevEx.Modules.DIRModule.BackEnd.AssessmentResults
{
    [Serializable]
    [IsDxObject]
    [BoundTable(typeof(BizDsAssessmentResultRow), TableName = "DXDIR_RESULT_ROW")]
    public partial class DxAssessmentResultRow : DxObject
    {
        #region Constants

        private const string configElementName = "dxAssessmentResultRow";

        #endregion

        #region Fields

        [BoundColumn("ID", true, 0, DbType = GenericDbType.Decimal)]
        protected decimal id;

        [BoundColumn("TITLE", DbType = GenericDbType.Varchar)]
        protected string title;

        [BoundColumn("RESULT_UOM", DbType = GenericDbType.Varchar)]
        protected string resultUoM;

        [BoundColumn("RESULT_TYPE", DbType = GenericDbType.Char)]
        protected string type;

        [BoundColumn("SORT_ORDER", DbType = GenericDbType.Int)]
        protected decimal sortOrder;

        #endregion

        #region Properties

        [BoundProperty(new string[] { nameof(id) })]
        public decimal Id
        {
            get { return id; }
        }

        [BoundProperty(new string[] { nameof(title) })]
        public string Title
        {
            get { return title; }
            set { Change(x => title = x, value); }
        }

        [BoundProperty(new string[] { nameof(resultUoM) })]
        public string ResultUoM
        {
            get { return resultUoM; }
            set { Change(x => resultUoM = x, value); }
        }

        [BoundProperty(new string[] { nameof(type) })]
        public string Type
        {
            get { return type; }
            set { Change(x => type = x, value); }
        }

        [BoundProperty(new string[] { nameof(sortOrder) })]
        public decimal SortOrder
        {
            get { return sortOrder; }
            set { Change(x => sortOrder = x, value); }
        }

		public string TitleDescription
		{
			get
			{
				DxPhraseText phraseText;

				if (DxUser.CurrentUser != null)
					phraseText = DxPhraseText.GetBest(DxPhraseTypeCategory.ListOfValuesStandard, "DXDIR_RESULT.TITLE", Title, DxUser.CurrentUser.ProgramCulture);
				else
					phraseText = DxPhraseText.GetBest(DxPhraseTypeCategory.ListOfValuesStandard, "DXDIR_RESULT.TITLE", Title, (DxCulture)null);

				return phraseText.Text;
			}
		}

        #endregion

        #region Constructors

        public DxAssessmentResultRow() : base()
        {
            
        }

        public DxAssessmentResultRow(decimal id) : this(id, false)
        {

        }

        public DxAssessmentResultRow(decimal id, bool load)
        {
            this.id = id;

            if (load)
                Load();
        }

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
