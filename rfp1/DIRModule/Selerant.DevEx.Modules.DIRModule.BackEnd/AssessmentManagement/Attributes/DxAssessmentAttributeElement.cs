using System;
using Selerant.ApplicationBlocks.Data;
using Selerant.DevEx.BusinessLayer;

namespace Selerant.DevEx.Modules.DIRModule.BackEnd.AssessmentManagement.Attributes
{
	[Serializable]
	[BoundTable(typeof(BizDsAssessmentAttValue), TableName = "DXDIR_ATTVALUE_ASSESSMENT")]
	[IsDxObject(IsQueryable = false)]
	public partial class DxAssessmentAttributeElement : DxAttributeElement
	{
		#region Constants

		private readonly string configElementName = "dxAssessmentAttributeElement";

		#endregion

		#region Fields

		[BoundColumn("CODE", DbType = GenericDbType.Varchar)]
		private string code;

		#endregion

		#region Properties

		[BoundProperty(new string[] { nameof(code) })]
		public string Code
		{
			get { return code; }
		}

		#endregion

		#region Constructors

		public DxAssessmentAttributeElement()
			: base()
		{ }

		public DxAssessmentAttributeElement(string objectPKey, string name, decimal id, decimal index)
			: this(objectPKey, name, id, index, false)
		{ }

		public DxAssessmentAttributeElement(string objectPKey, string name, decimal id, decimal index, bool load)
			: this()
		{
			this.objectPKey = objectPKey;
			this.name = name;
			this.id = id;
			this.index = index;
			SetContainerKeyValues(objectPKey);

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
			// Add any specific configuration
		}

		#endregion

		#region Overrides

		protected override void SetContainerKeyValues(string objectPkey)
		{
			string[] keyParams = objectPkey.Split(AttributeKeyFieldSeparator);
			code = keyParams[0];

			filteredLoadMethod = new DxMethodInvoker(BizDsObjectType.GetMethod("SelectOne"));
			filteredLoadMethod.AddParameters(code, id, index);
		}

		protected override void OnAfterWriteToRecord(Dal.BizObj.Record record, bool identityKeyOnly)
		{
			// Always write database primary key fields, even if they do not belong to the identity key 
			// of the business object, because they are used in the proper package.
			record["CODE"] = DataTypeConverter.ToDbString(this.code);

			base.OnAfterWriteToRecord(record, identityKeyOnly);
		}

		#endregion
	}
}
