using Selerant.ApplicationBlocks.Data;
using Selerant.DevEx.BusinessLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Selerant.DevEx.Modules.DIRModule.BackEnd.OutputManagement.GridItem
{
    [IsDxObject]
    [BoundTable(typeof(BizDsOutputGridItem), TableName = "DXDIR_OUTPUT", SourceType = BoundTableAttribute.DataSourceType.StoredProcedure)]
    public partial class DxOutputGridItem : DxObject
    {
        #region Constants

        private const string configElementName = "dxOutputGridItem";

        #endregion

        #region Fields

        [BoundColumn("ID", true, 0, DbType = GenericDbType.Decimal)]
        protected decimal id;

        [BoundColumn("OUTPUT_CATEGORY_ID", DbType = GenericDbType.Decimal)]
        protected decimal outputCategoryId;

        [BoundColumn("OUTPUT_CATEGORY_TYPE", DbType = GenericDbType.Varchar)]
        protected string outputCategoryType;

        [BoundColumn("OUTPUT_CATEGORY_TITLE", DbType = GenericDbType.Varchar)]
        protected string outputCategoryTitle;

        [BoundColumn("DESTINATION_CODE", DbType = GenericDbType.Varchar)]
        protected string destinationCode;

        [BoundColumn("DESTINATION_TITLE", DbType = GenericDbType.Varchar)]
        protected string destinationTitle;

        [BoundColumn("OUTPUT_COST", DbType = GenericDbType.Decimal)]
        protected Nullable<decimal> outputCost;

        [BoundColumn("INCOME", DbType = GenericDbType.Decimal)]
        protected Nullable<decimal> income;

        #endregion

        #region Properties

        [BoundProperty(new string[] { nameof(id) })]
        public decimal Id
        {
            get { return id; }
        }

        [BoundProperty(new string[] { nameof(outputCategoryId) })]
        public decimal OutputCategoryId
        {
            get { return outputCategoryId; }
            set { Change(x => outputCategoryId = x, value); }
        }

        [BoundProperty(new string[] { nameof(outputCategoryTitle) })]
        public string OutputCategoryTitle
        {
            get { return outputCategoryTitle; }
        }

        [BoundProperty(new string[] { nameof(outputCategoryType) })]
        public string OutputCategoryType
        {
            get { return outputCategoryType; }
        }

        [BoundProperty(new string[] { nameof(destinationCode) })]
        public string DestinationCode
        {
            get { return destinationCode; }
        }

        [BoundProperty(new string[] { nameof(destinationTitle) })]
        public string DestinationTitle
        {
            get { return destinationTitle; }
        }

        public string GridItemTitle
        {
            get
            {
                return !string.IsNullOrEmpty(DestinationTitle) ? DestinationTitle : !string.IsNullOrEmpty(OutputCategoryTitle) ? OutputCategoryTitle : "";
            }
        }

        [BoundProperty(new string[] { nameof(outputCost) })]
        public Nullable<decimal> OutputCost
        {
            get { return outputCost; }
            set { Change(x => outputCost = x, value); }
        }

        [BoundProperty(new string[] { nameof(income) })]
        public Nullable<decimal> Income
        {
            get { return income; }
            set { Change(x => income = x, value); }
        }

        #endregion

        #region Constructors

        public DxOutputGridItem() : base()
        {
            
        }

        public DxOutputGridItem(decimal id) : this()
        {
            this.id = id;
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
