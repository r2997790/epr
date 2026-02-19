using System;
using Selerant.DevEx.BusinessLayer;

namespace Selerant.DevEx.Modules.DIRModule.BackEnd.InputManagement
{
	[IsDxObjectCollection]
	[BoundBusinessObject(typeof(DxFoodInputDestination))]
	public partial class DxFoodInputDestinationCollection : DxObjectCollection
	{
		protected DxFoodInputDestinationCollection() : base()
		{
		}

		public DxFoodInputDestinationCollection(string assessmentCode, decimal assessmentLcStage, bool load = false) : this()
		{
			filteredLoadMethod = new DxMethodInvoker(BizDsObjectType.GetMethod(nameof(BizDsInputDestination.SelectFoodDestinations)));
			filteredLoadMethod.MethodActualParameters = new object[] { assessmentCode, assessmentLcStage };

			if (load)
				Load();
		}

		/// <summary>
		/// Loads at once all Materials
		/// </summary>
		/// <returns></returns>
		public void LoadItemsMaterial()
		{
			if (Count == 0)
				return;

			Func<DxFoodInputDestination, DxMaterial> propertyGetter = (x) => x.Material;
			Action<DxFoodInputDestination, DxMaterial> propertySetter = (x, y) => x.Material = y;

			FillItemsObjectProperty(propertyGetter, propertySetter);
		}
	}
}
