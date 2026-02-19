using System;
using System.Collections.Generic;
using System.Linq;
using IQToolkit;
using Selerant.DevEx.BusinessLayer;
using Selerant.DevEx.BusinessLayer.Linq;
using Selerant.DevEx.Modules.DIRModule.BackEnd.DestinationManagement;

namespace Selerant.DevEx.Modules.DIRModule.BackEnd.AssessmentManagement.BusinessObjects
{
	[IsDxObjectCollection]
	[BoundBusinessObject(typeof(DxAssessmentDestination))]
	public partial class DxAssessmentDestinationCollection : DxObjectCollection
	{
		#region Constants

		private const string configElementName = "dxAssessmentDestinationCollection";

		#endregion

		#region Queries

		protected static readonly Func<string, IQueryable<DxAssessmentDestination>> queryByAssessmentCode
			= QueryCompiler.Compile((string assessmentCode) =>
				(from o in new DxQueryable<DxAssessmentDestination>()
				 where o.AssessmentCode == assessmentCode
				 select o));

		protected static readonly Func<string, string, IQueryable<DxAssessmentDestination>> queryByAssessmentCodeAndDestinationCode
			= QueryCompiler.Compile((string assessmentCode, string destinationCode) =>
				(from o in new DxQueryable<DxAssessmentDestination>()
				 where o.AssessmentCode == assessmentCode && o.DestinationCode == destinationCode
				 select o));


		#endregion

		#region Filter Enum

		public enum Filter
		{
			AssessmentCode,
			AssessmentCodeAndWasteDestCode
		}

		#endregion

		#region Constructors

		public DxAssessmentDestinationCollection() : base()
		{
		}

		public DxAssessmentDestinationCollection(Filter filter, params object[] parameters)
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
					case Filter.AssessmentCodeAndWasteDestCode:
						return queryByAssessmentCodeAndDestinationCode((string)loadFilter.Parameters[0], (string)loadFilter.Parameters[1]);
					case Filter.AssessmentCode:
						return queryByAssessmentCode((string)loadFilter.Parameters[0]);
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

		#region Static Methods

		#endregion

		#region LoadItems Method

		/// <summary>
		/// Loads Destinations of all items in this collection
		/// </summary>
		/// <returns></returns>
		public List<DxDestination> LoadItemsDestination()
		{
			if (Count == 0)
				return new List<DxDestination>();

			Func<DxAssessmentDestination, DxDestination> propertyGetter = (x) => x.Destination;
			Action<DxAssessmentDestination, DxDestination> propertySetter = (x, y) => x.Destination = y;

			return FillItemsObjectProperty(propertyGetter, propertySetter);
		}

		#endregion
	}
}