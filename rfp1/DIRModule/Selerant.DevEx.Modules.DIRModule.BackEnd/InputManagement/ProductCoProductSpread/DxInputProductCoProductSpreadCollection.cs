using System;
using System.Linq;
using System.Collections.Generic;
using Selerant.DevEx.BusinessLayer;
using IQToolkit;
using Selerant.DevEx.BusinessLayer.Linq;

namespace Selerant.DevEx.Modules.DIRModule.BackEnd.InputManagement
{
	[IsDxObjectCollection]
	[BoundBusinessObject(typeof(DxInputProductCoProductSpread))]
	public partial class DxInputProductCoProductSpreadCollection : DxObjectCollection
	{
		#region Queries

		protected static readonly Func<decimal, IQueryable<DxInputProductCoProductSpread>> queryByInputId
			= QueryCompiler.Compile((decimal inputId) =>
			(from o in new DxQueryable<DxInputProductCoProductSpread>()
			 where o.InputId == inputId
			 select o));

		#endregion

		#region Filter Enum

		public enum Filter
		{
			InputId
		}

		#endregion

		#region Constructors

		public DxInputProductCoProductSpreadCollection(): base()
		{ }

		public DxInputProductCoProductSpreadCollection(decimal inputId, bool load) : base()
		{
			loadFilter = new LoadFilter(Filter.InputId, new object[] { inputId });

			if (load)
				Load();
		}

		public DxInputProductCoProductSpreadCollection(decimal[] inputIds, bool load) : base()
		{
			filteredLoadMethod = new DxMethodInvoker(BizDsObjectType.GetMethod(nameof(BizDsInputProductCoProductSpread.SelectProductCoProductSpread)));
			FilteredLoadMethod.MethodActualParameters = new object[] { inputIds };

			if (load)
				Load();
		}

		#endregion

		protected override IQueryable GetLoadQuery()
		{
			if (loadFilter != null)
			{
				switch ((Filter)loadFilter.Filter)
				{
					case Filter.InputId:
						return queryByInputId((decimal)loadFilter.Parameters[0]);
					default:
						throw new NotImplementedException($"Filter {loadFilter.Filter} is not implemented!");
				}
			}
			return null;
		}

		public Dictionary<decimal, List<string>> ToDictionary()
		{
			return this.GroupBy(u => u.InputId)
				.Select(grp => new
				{
					inputId = grp.Key,
					codes = grp.Select(r => r.DestinationCode).ToList()
				})
				.ToDictionary(key => key.inputId, value => value.codes);
		}
	}
}
