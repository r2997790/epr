using System;
using IQToolkit;
using Selerant.DevEx.BusinessLayer;
using Selerant.DevEx.BusinessLayer.Linq;
using System.Collections.Generic;
using System.Linq;

namespace Selerant.DevEx.Modules.DIRModule.BackEnd.OutputManagement
{
    [IsDxObjectCollection]
    [BoundBusinessObject(typeof(DxOutputCategory))]
    public partial  class DxOutputCategoryCollection : DxObjectCollection
    {
        #region Constants

        private const string configElementName = "dxOutputCategoryCollection";

        #endregion

        #region Constructors

        public DxOutputCategoryCollection() : base()
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
    }
}
