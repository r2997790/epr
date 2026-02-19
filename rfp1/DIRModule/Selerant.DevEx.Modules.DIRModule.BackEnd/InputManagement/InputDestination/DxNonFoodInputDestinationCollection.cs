using System;
using Selerant.DevEx.BusinessLayer;

namespace Selerant.DevEx.Modules.DIRModule.BackEnd.InputManagement
{
	[IsDxObjectCollection]
	[BoundBusinessObject(typeof(DxNonFoodInputDestination))]
	public partial class DxNonFoodInputDestinationCollection : DxObjectCollection
	{
		protected DxNonFoodInputDestinationCollection() : base()
		{
		}

		public DxNonFoodInputDestinationCollection(string assessmentCode, decimal assessmentLcStage, bool load = false) : this()
		{
			filteredLoadMethod = new DxMethodInvoker(BizDsObjectType.GetMethod(nameof(BizDsInputDestination.SelectNonFoodDestinations)));
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

			Func<DxNonFoodInputDestination, DxMaterial> propertyGetter = (x) => x.Material;
			Action<DxNonFoodInputDestination, DxMaterial> propertySetter = (x, y) => x.Material = y;

			FillItemsObjectProperty(propertyGetter, propertySetter);
		}
	}
}
