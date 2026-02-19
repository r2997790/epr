using System;
using System.Collections;
using System.Collections.Generic;
using IQToolkit;
using System.Linq;
using Selerant.DevEx.BusinessLayer;
using Selerant.DevEx.BusinessLayer.Linq;

namespace Selerant.DevEx.Modules.DIRModule.BackEnd.InputManagement
{
	[IsDxObjectCollection]
	[BoundBusinessObject(typeof(DxInput))]
	public partial class DxInputCollection : DxObjectCollection
	{
		#region Constants

		private const string configElementName = "dxInputCollection";

		#endregion

		#region Queries

		protected static readonly Func<string, decimal, IQueryable<DxInput>> queryByAssessmentCodeAndAssessmentLcStageId
			= QueryCompiler.Compile((string assessmentCode, decimal assessmentLcStageId) =>
				(from o in new DxQueryable<DxInput>()
				 where o.AssessmentCode == assessmentCode && o.AssessmentLcStageId == assessmentLcStageId
				 select o));

		protected static readonly Func<string, IQueryable<DxInput>> queryByAssessmentCode
			= QueryCompiler.Compile((string assessmentCode) =>
				(from o in new DxQueryable<DxInput>()
				 where o.AssessmentCode == assessmentCode
				 select o));

		#endregion

		#region Filter Enum

		public enum Filter
		{
			AssessmentCode,
			AssessmentCodeAndAssessmentLcStageId
		}

		#endregion

		#region Constructors

		public DxInputCollection() : base()
		{
		}

		public DxInputCollection(string assessmentCode, decimal assessmentLcStageId, bool load = false)
			: this()
		{
			loadFilter = new LoadFilter(Filter.AssessmentCodeAndAssessmentLcStageId, new object[] { assessmentCode, assessmentLcStageId });

			if (load)
				Load();
		}

		public DxInputCollection(string assessmentCode, bool load = false)
			: this()
		{
			loadFilter = new LoadFilter(Filter.AssessmentCode, new object[] { assessmentCode });

			if (load)
				Load();
		}

		// Constructor methods

		public static DxInputCollection GetCarriedInputs(string assessmentCode, decimal assessmentLcStageId, bool load = false)
		{
			var carriedInputs = new DxCarriedInputCollection(assessmentCode, assessmentLcStageId, nameof(BizDsCarriedInput.GetCarriedInputsToNextStage), load);

			var result = new DxInputCollection();
			result.AddRange(carriedInputs.Select(carriedInput => DxInput.NewFromCarried(carriedInput)));
			result.PersistenceStatus = DxPersistenceStatus.UpToDate;
			return result;
		}

		// end Constructor methods

		#endregion

		#region Overrides

		protected override IQueryable GetLoadQuery()
		{
			if (loadFilter != null)
			{
				switch ((Filter)loadFilter.Filter)
				{
					case Filter.AssessmentCodeAndAssessmentLcStageId:
						return queryByAssessmentCodeAndAssessmentLcStageId((string)loadFilter.Parameters[0], (decimal)loadFilter.Parameters[1]);
					case Filter.AssessmentCode:
						return queryByAssessmentCode((string)loadFilter.Parameters[0]);
					default:
						throw new NotImplementedException($"Filter {loadFilter.Filter} is not implemented!");
				}
			}
			return null;
		}

		public override void OnBeforeDelete()
		{
			BizDsInput bizDs = new BizDsInput();
			DxInput first = this.FirstOrDefault();

			if (first != null)
			{
				decimal[] ids = this.Select(x => x.Id).ToArray();

				bizDs.DeleteCascadeOutputParent(ids, first.AssessmentCode, first.AssessmentLcStageId);
				base.OnBeforeDelete();
			}
		}

		public override void OnAfterDelete()
		{
			base.OnAfterDelete();

            BizDsInput bizDs = new BizDsInput();
            DxInput first = this.FirstOrDefault();

			if (first != null)
			{
				bizDs.UpdateCascadeOnOutput(first.AssessmentCode, first.AssessmentLcStageId);
				bizDs.ManageWastewaterRow(first.AssessmentCode, first.AssessmentLcStageId);
			}
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

		#region Indexers Override 

		public DxInput this[decimal id] => (DxInput)base[DxObject.GetIdentityKey(id)];

		public new DxInput this[int index] => (DxInput)base[index];

		#endregion

		#region LoadItems Methods

		/// <summary>
		/// Loads InputCategories of all items in this collection
		/// </summary>
		/// <returns></returns>
		public List<DxInputCategory> LoadItemsInputCategory()
		{
			if (Count == 0)
				return new List<DxInputCategory>();

			DxInputCategoryCollection inputCategories = new DxInputCategoryCollection();

			foreach (var input in this)
			{
				inputCategories.Add(new DxInputCategory(input.InputCategoryId));
			}

			inputCategories.LoadItems();

			foreach (var input in this)
			{
				input.InputCategory = inputCategories[input.InputCategoryId];
				input.InputCategory.SortOrder = input.CategorySortOrder;
			}

			return inputCategories.ToList();
		}

		/// <summary>
		/// Loads Material of all items in this collection
		/// </summary>
		/// <returns></returns>
		public List<DxMaterial> LoadItemsMaterial()
		{
			if (Count == 0)
				return new List<DxMaterial>();

			Func<DxInput, DxMaterial> propertyGetter = (x) => x.Material;
			Action<DxInput, DxMaterial> propertySetter = (x, y) => x.Material = y;

			return FillItemsObjectProperty(propertyGetter, propertySetter);
		}

		public Dictionary<decimal, List<string>> LoadInputIdsProductCoProductCodes()
		{
			decimal[] inputIds = this.Where(x => x.PartOfProductCoproduct).Select(r => r.Id).ToArray();
			return new DxInputProductCoProductSpreadCollection(inputIds, true).ToDictionary();
		}

		#endregion

	}
}
