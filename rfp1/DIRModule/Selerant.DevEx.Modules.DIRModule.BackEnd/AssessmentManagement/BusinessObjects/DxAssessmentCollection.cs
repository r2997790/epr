using System;
using System.Collections.Generic;
using System.Linq;
using Selerant.DevEx.BusinessLayer;

namespace Selerant.DevEx.Modules.DIRModule.BackEnd.AssessmentManagement.BusinessObjects
{
	[Serializable]
	[BoundBusinessObject(typeof(DxAssessment))]
	[IsDxObjectCollection]
	public partial class DxAssessmentCollection : DxSecureObjectCollection, IAttributesContainerCollection
	{
		#region Constants

		private const string configElementName = "dxAssessmentCollection";

		#endregion

		#region Queries

		#endregion

		#region Constructors

		public DxAssessmentCollection() : base()
		{
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

        public new DxAssessment this[string code]
        {
            get
            {
                return (DxAssessment)base[DxObject.GetIdentityKey(code)];
            }
        }

		#region IAttributesContainerCollection

		public DxAttributeScope AttributesScope
		{
			get { return ChildrenDataMapper.GetBoundAttributesScopeAttribute().AttributesScope; }
		}

		public void LoadAndMergeItemsAttributes(params string[] names)
		{
			BaseLoadAndMergeItemsAttributes(names);
		}

		public void LoadItemsAttributes(params string[] names)
		{
			BaseLoadItemsAttributes(names);
		}

		#endregion
	}
}
