using System;
using System.Linq;
using IQToolkit;
using Selerant.DevEx.BusinessLayer;
using Selerant.DevEx.BusinessLayer.Linq;

namespace Selerant.DevEx.Modules.DIRModule.BackEnd.InputManagement
{
	[IsDxObjectCollection]
	[BoundBusinessObject(typeof(DxInputDestination))]
	public partial class DxInputDestinationCollection : DxObjectCollection
	{
		#region Constants

		private const string configElementName = "dxInputDestinationCollection";

		// NOTE: ugly ass hell, there's no other way to query where in
		private const string DESTINATION_PRODUCT = "PRODUCT";
		private const string DESTINATION_PRODUCT_2 = "PRODUCT_2";
		private const string DESTINATION_PRODUCT_3 = "PRODUCT_3";
		private const string DESTINATION_COPRODUCT = "COPRODUCT";
		private const string DESTINATION_COPRODUCT_2 = "COPRODUCT_2";

		#endregion

		#region Queries

		protected static readonly Func<decimal, IQueryable<DxInputDestination>> queryByInputId
			= QueryCompiler.Compile((decimal inputId) =>
				(from o in new DxQueryable<DxInputDestination>()
				 where o.InputId == inputId
				 select o));

		protected static readonly Func<string, IQueryable<DxInputDestination>> queryByDestinationCode
			= QueryCompiler.Compile((string destinationCode) =>
				(from o in new DxQueryable<DxInputDestination>()
				 where o.DestinationCode == destinationCode
				 select o));

		protected static readonly Func<decimal, IQueryable<DxInputDestination>> queryByOutputCategoryId
			= QueryCompiler.Compile((decimal outputCategoryId) =>
				(from o in new DxQueryable<DxInputDestination>()
				 where o.OutputCategoryId == outputCategoryId
				 select o));

		protected static readonly Func<decimal, decimal, IQueryable<DxInputDestination>> queryByInputIdAndOutputCategoryId
			= QueryCompiler.Compile((decimal inputId, decimal outputCategoryId) =>
				(from o in new DxQueryable<DxInputDestination>()
				 where o.InputId == inputId && o.OutputCategoryId == outputCategoryId
				 select o));

		protected static readonly Func<decimal, IQueryable<DxInputDestination>> queryByInputIdAndNonWasteDestination
			= QueryCompiler.Compile((decimal inputId) =>
				(from o in new DxQueryable<DxInputDestination>()
				 where (o.InputId == inputId &&
						// NOTE: ugly ass hell, there's no other way to query where in
						(o.DestinationCode == DESTINATION_PRODUCT || o.DestinationCode == DESTINATION_PRODUCT_2 || o.DestinationCode == DESTINATION_PRODUCT_3 ||
				         o.DestinationCode == DESTINATION_COPRODUCT || o.DestinationCode == DESTINATION_COPRODUCT_2))
				 select o));

		#endregion

		#region Filter Enum

		public enum Filter
		{
			InputId,
			DestinationCode,
			OutputCategoryId,
			InputIdAndOutputCategoryId,
			InputIdAndNonWasteDestination
		}

		#endregion

		#region Constructors

		public DxInputDestinationCollection() : base()
		{
		}

		public DxInputDestinationCollection(Filter filter, params object[] parameters)
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
					case Filter.InputId:
						return queryByInputId((decimal)loadFilter.Parameters[0]);
					case Filter.DestinationCode:
						return queryByDestinationCode((string)loadFilter.Parameters[0]);
					case Filter.OutputCategoryId:
						return queryByOutputCategoryId((decimal)loadFilter.Parameters[0]);
					case Filter.InputIdAndOutputCategoryId:
						return queryByInputIdAndOutputCategoryId((decimal)loadFilter.Parameters[0], (decimal)loadFilter.Parameters[1]);
					case Filter.InputIdAndNonWasteDestination:
						return queryByInputIdAndNonWasteDestination((decimal)loadFilter.Parameters[0]);
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
	}
}
