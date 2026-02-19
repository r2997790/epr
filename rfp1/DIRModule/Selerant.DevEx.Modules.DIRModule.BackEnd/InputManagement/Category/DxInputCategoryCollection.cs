using System;
using IQToolkit;
using System.Linq;
using Selerant.DevEx.BusinessLayer;
using Selerant.DevEx.BusinessLayer.Linq;
using System.Collections.Generic;
using ConstantsMatCodes = Selerant.DevEx.Modules.DIRModule.BackEnd.Common.Constants.ResourceMaterial;

namespace Selerant.DevEx.Modules.DIRModule.BackEnd.InputManagement
{
	[IsDxObjectCollection]
	[BoundBusinessObject(typeof(DxInputCategory))]
	public partial class DxInputCategoryCollection : DxObjectCollection
	{
		#region Constants

		private const string configElementName = "dxInputCategoryCollection";

		#endregion

		#region Queries

		protected static readonly Func<string, IQueryable<DxInputCategory>> queryByAssessmentCode
			= QueryCompiler.Compile((string assessmentCode) =>
				(from o in new DxQueryable<DxInputCategory>()
				 where o.AssessmentCode == assessmentCode
				 select o));

		protected static readonly Func<string, decimal, IQueryable<decimal>> queryUsedInputCategoriesByAssessmentCodeAndLcStageId
			= QueryCompiler.Compile((string assessmentCode, decimal assessmentLcStageId) =>
				(from o in new DxQueryable<DxInput>()
					where o.AssessmentCode == assessmentCode && o.AssessmentLcStageId == assessmentLcStageId
					select o.InputCategoryId));

		#endregion

		#region Filter Enum

		public enum Filter
		{
			AssessmentCode
		}

		#endregion

		#region Constructors

		public DxInputCategoryCollection() : base()
		{
		}

		public DxInputCategoryCollection(string assessmentCode)
			: this()
		{
			loadFilter = new LoadFilter(Filter.AssessmentCode, new object[] { assessmentCode });
		}

		public DxInputCategoryCollection(Filter filter, params object[] parameters)
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
					case Filter.AssessmentCode:
						return queryByAssessmentCode((string)loadFilter.Parameters[0]);
					default:
						throw new NotImplementedException($"Filter {loadFilter.Filter} is not implemented!");
				}
			}
			return null;
		}

		#endregion

		#region Indexers Override 

		public DxInputCategory this[decimal id] => (DxInputCategory)base[DxObject.GetIdentityKey(id)];

		public new DxInputCategory this[int index] => (DxInputCategory)base[index];

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

		public static List<decimal> GetUsedInputCategories(string assessmentCode, decimal lcStageId)
		{
			return queryUsedInputCategoriesByAssessmentCodeAndLcStageId(assessmentCode, lcStageId).Distinct().ToList();
		}

		#endregion
	}
}