using System;
using Selerant.DevEx.BusinessLayer;
using Selerant.DevEx.Modules.DIRModule.BackEnd.AssessmentManagement.BusinessObjects;

namespace Selerant.DevEx.Modules.DIRModule.BackEnd.AssessmentManagement.Attributes
{
	[Serializable]
	[BoundBusinessObject(typeof(DxAssessmentAttributeElement))]
	[IsDxObjectCollection(IsQueryable = false)]
	public partial class DxAssessmentAttributeElementCollection : DxAttributeElementCollection
	{
		#region Constants

		private readonly string configElementName = "dxAssessmentAttributeElementCollection";

		#endregion

		#region Constructors

		public DxAssessmentAttributeElementCollection()
			: base()
		{
			this.scope = DxAssessmentAttributeScope.NAME;
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

		#region Overrides

		protected override void AddContainerPropertyGetParameters(IAttributesContainer container, DxMethodInvoker invoker)
		{
			invoker.AddPropertyGetParameters(container, "Code");
		}

		protected override object[] GetPKeyValuesFromContainerKey(string objectPKey)
		{
			return DxObject.GetKeyValues(objectPKey, typeof(DxAssessment));
		}

		#endregion
	}
}
