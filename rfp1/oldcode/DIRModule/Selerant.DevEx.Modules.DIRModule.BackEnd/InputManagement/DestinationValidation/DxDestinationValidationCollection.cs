using System.Collections.Generic;
using Selerant.DevEx.BusinessLayer;
using System.Linq;

namespace Selerant.DevEx.Modules.DIRModule.BackEnd.InputManagement
{
    [IsDxObjectCollection]
    [BoundBusinessObject(typeof(DxDestinationValidation))]
    public partial class DxDestinationValidationCollection : DxObjectCollection
    {
        public DxDestinationValidationCollection(string assessment, bool load = false) 
        {
            filteredLoadMethod = new DxMethodInvoker(BizDsObjectType.GetMethod(nameof(BisDsDestinationValidation.CheckValidationFillDestination)));
            FilteredLoadMethod.MethodActualParameters = new object[] { assessment };

            if (load)
                Load();
        }
    }
}
