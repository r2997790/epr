using Selerant.ApplicationBlocks.Data;
using Selerant.DevEx.BusinessLayer;

namespace Selerant.DevEx.Modules.DIRModule.BackEnd.InputManagement
{
    [IsDxObject]
    [BoundTable(typeof(BisDsDestinationValidation), TableName = "DXDIR_PK_INPUT_DESTINATION", SourceType = BoundTableAttribute.DataSourceType.StoredProcedure)]
    public partial class DxDestinationValidation : DxObject
    {
		#region ENUM

		public enum InvalidCategory
		{
			NONE = 0,
			FOOD = 1,
			NONFOOD = 2,
			NONFOODANDFOOD = 3
		}

		#endregion

		#region Fields

		[BoundColumn("LCSTAGE", true, 0, DbType = GenericDbType.Decimal)]
		protected decimal lcStage;

		[BoundColumn("TITLE",  DbType = GenericDbType.Varchar)]
        protected string title;

		[BoundColumn("INVALID", DbType = GenericDbType.Decimal)]
		protected decimal invalidDestination;

		#endregion

		#region Properties

		[BoundProperty(new string[] { nameof(lcStage) })]
		public decimal LcStage
		{
			get { return lcStage; }
		}

		[BoundProperty(new string[] { nameof(title) })]
        public string Title
        {
            get { return title; }
        }

		[BoundProperty(new string[] { nameof(invalidDestination) })]
		public InvalidCategory InvalidDestination
		{
			get { return (InvalidCategory)((int)invalidDestination); }
		}

		#endregion

		#region Constructor

		public DxDestinationValidation()
        {
        }

        #endregion
    }
}
