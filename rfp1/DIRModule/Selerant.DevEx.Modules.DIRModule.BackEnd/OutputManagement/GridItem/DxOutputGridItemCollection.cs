using Selerant.DevEx.BusinessLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Selerant.DevEx.Modules.DIRModule.BackEnd.OutputManagement.GridItem
{
    [BoundBusinessObject(typeof(DxOutputGridItem))]
    [IsDxObjectCollection]
    public partial class DxOutputGridItemCollection : DxObjectCollection
    {
        protected DxOutputGridItemCollection() : base()
        {

        }

        public DxOutputGridItemCollection(string assessmentCode, decimal lcStage, bool load = false) : this()
        {
            filteredLoadMethod = new DxMethodInvoker(BizDsObjectType.GetMethod(nameof(BizDsOutputGridItem.GetOutputGridItems)));
            FilteredLoadMethod.MethodActualParameters = new object[] { assessmentCode, lcStage };

            if (load)
                Load();
        }

        public List<DxOutputCategory> LoadItemOutputCategories()
        {
            if (Count == 0)
                return new List<DxOutputCategory>();

            DxOutputCategoryCollection outputCategories = new DxOutputCategoryCollection();

			foreach (var outputGridItem in this)
			{
				outputCategories.Add(new DxOutputCategory(outputGridItem.OutputCategoryId));
			}

            outputCategories.LoadItems();

            return outputCategories.ToList();
        }
    }
}
