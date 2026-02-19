using System.Collections.Generic;
using Selerant.DevEx.BusinessLayer;
using System.Linq;

namespace Selerant.DevEx.Modules.DIRModule.BackEnd.DestinationManagement
{
	[IsDxObjectCollection]
	[BoundBusinessObject(typeof(DxDestinationSort))]
	public partial class DxDestinationSortCollection : DxObjectCollection
	{
		public DxDestinationSortCollection(string[] destinationCodes, bool load = false)
		{
			filteredLoadMethod = new DxMethodInvoker(BizDsObjectType.GetMethod(nameof(BizDsDestinationSort.SelectDestinationSort)));
			FilteredLoadMethod.MethodActualParameters = new object[] { destinationCodes };

			if (load)
				Load();
		}

		public Dictionary<string, decimal> ToDictionary()
		{
			return this.ToDictionary(key => key.Code, value => value.SortOrder);
		}
	}
}
