using System;
using System.Linq;
using IQToolkit;
using Selerant.DevEx.BusinessLayer;
using Selerant.DevEx.BusinessLayer.Linq;

namespace Selerant.DevEx.Modules.DIRModule.BackEnd.DestinationManagement
{
	[IsDxObjectCollection]
	[BoundBusinessObject(typeof(DxDestination))]
	public partial class DxDestinationCollection : DxObjectCollection
	{
		#region Constants

		private const string configElementName = "dxDestinationCollection";

		#endregion

		#region Queries

		protected static readonly Func<string, IQueryable<DxDestination>> queryByCode
			= QueryCompiler.Compile((string code) =>
				(from o in new DxQueryable<DxDestination>()
				 where o.Code == code
				 select o));

		protected static readonly Func<decimal, IQueryable<DxDestination>> queryByWaste
			= QueryCompiler.Compile((decimal waste) =>
				(from o in new DxQueryable<DxDestination>()
				 where o.Waste == waste
				 select o));

		protected static readonly Func<string, decimal, IQueryable<DxDestination>> queryByCodeAndWaste
			= QueryCompiler.Compile((string code, decimal waste) =>
				(from o in new DxQueryable<DxDestination>()
				 where o.Code == code && o.Waste == waste
				 select o));

		protected static readonly Func<decimal, IQueryable<DxDestination>> queryByAllAndByUsedOn
			= QueryCompiler.Compile((decimal usedOn) =>
				(from o in new DxQueryable<DxDestination>()
				 where o.UsedOn == DxDestination.DestinationUsedOn.Both || o.UsedOn == (DxDestination.DestinationUsedOn)usedOn
				 select o));

		#endregion

		#region Filter Enum

		public enum Filter
		{
			Code,
			Waste,
			CodeAndWaste,
			UsedOn
		}

		#endregion

		#region Constructors

		public DxDestinationCollection() : base()
		{
		}

		public DxDestinationCollection(Filter filter, params object[] parameters)
			: this()
		{
			loadFilter = new LoadFilter(filter, parameters);
		}

		#endregion

		#region Overrides

		protected override IQueryable GetLoadQuery()
		{
			if (loadFilter != null)
			{
				switch ((Filter)loadFilter.Filter)
				{
					case Filter.Code:
						return queryByCode((string)loadFilter.Parameters[0]);
					case Filter.Waste:
						return queryByWaste((decimal)loadFilter.Parameters[0]);
					case Filter.CodeAndWaste:
						return queryByCodeAndWaste((string)loadFilter.Parameters[0], (decimal)loadFilter.Parameters[1]);
					case Filter.UsedOn:
						return queryByAllAndByUsedOn((decimal)((DxDestination.DestinationUsedOn)loadFilter.Parameters[0]));
					default:
						throw new NotImplementedException($"Filter {loadFilter.Filter} is not implemented!");
				}
			}
			return null;
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

		public void AddItem(string code)
		{
			AddItem(new DxDestination(code));
		}
	}
}
