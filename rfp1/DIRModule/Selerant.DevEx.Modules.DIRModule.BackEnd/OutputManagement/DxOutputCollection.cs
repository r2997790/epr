using System;
using System.Collections.Generic;
using System.Linq;
using IQToolkit;
using Selerant.DevEx.BusinessLayer;
using Selerant.DevEx.BusinessLayer.Linq;
using Selerant.DevEx.Modules.DIRModule.BackEnd.AssessmentManagement.BusinessObjects;

namespace Selerant.DevEx.Modules.DIRModule.BackEnd.OutputManagement
{
	[IsDxObjectCollection]
	[BoundBusinessObject(typeof(DxOutput))]
	public partial class DxOutputCollection : DxObjectCollection
	{
		#region Constants

		private const string configElementName = "dxOutputCollection";

		#endregion

		#region Queries

		protected static readonly Func<string, decimal, IQueryable<DxOutput>> queryByAssessmentCodeAndAssessmentLcStageId
			= QueryCompiler.Compile((string assessmentCode, decimal assessmentLcStageId) =>
				(from o in new DxQueryable<DxOutput>()
				 where o.AssessmentCode == assessmentCode && o.AssessmentLcStageId == assessmentLcStageId
				 select o));

		protected static readonly Func<string, IQueryable<DxOutput>> queryByAssessmentCode
			= QueryCompiler.Compile((string assessmentCode) =>
				(from o in new DxQueryable<DxOutput>()
				 where o.AssessmentCode == assessmentCode
				 select o));

        protected static readonly Func<string, decimal, string, IQueryable<DxOutput>> queryByAssessmentCodeLcStageIdAndDestionationCode
           = QueryCompiler.Compile((string assessmentCode, decimal assessmentLcStageId, string destinationCode) =>
               (from o in new DxQueryable<DxOutput>()
                where o.AssessmentCode == assessmentCode && o.AssessmentLcStageId == assessmentLcStageId && o.DestinationCode == destinationCode
                select o));

        protected static readonly Func<string, decimal, decimal, string, IQueryable<DxOutput>> queryByAssessmentCodeAndLcStageIdAndOutputCategoryIdAndDestionationCode
		   = QueryCompiler.Compile((string assessmentCode, decimal assessmentLcStageId, decimal outputCategoryId, string destinationCode) =>
			   (from o in new DxQueryable<DxOutput>()
				where o.AssessmentCode == assessmentCode && o.AssessmentLcStageId == assessmentLcStageId && o.OutputCategoryId == outputCategoryId && o.DestinationCode == destinationCode
				select o));

		protected static readonly Func<string, decimal, decimal, IQueryable<DxOutput>> queryByAssessmentCodeAndLcStageIdAndOutputCategoryId
		   = QueryCompiler.Compile((string assessmentCode, decimal assessmentLcStageId, decimal outputCategoryId) =>
			   (from o in new DxQueryable<DxOutput>()
				where o.AssessmentCode == assessmentCode && o.AssessmentLcStageId == assessmentLcStageId && o.OutputCategoryId == outputCategoryId
				select o));

		protected static readonly Func<decimal, decimal, IQueryable<DxOutput>> queryByInputIdAndOutputCategoryId
			= QueryCompiler.Compile((decimal inputId, decimal outputCategoryId) =>
				(from o in new DxQueryable<DxOutput>()
				 where o.InputId == inputId && o.OutputCategoryId == outputCategoryId
				 select o));

		#endregion

		#region Filter Enum

		public enum Filter
        {
			AssessmentCode,
			AssessmentCodeAndAssessmentLcStageId,
			InputIdAndOutputCategoryId,
            AssessmentCode_LcStageId_DestinationCode,
			AssessmentCodeAndLcStageIdAndOutputCategoryIdAndDestionationCode,
			AssessmentCodeAndLcStageIdAndOutputCategoryId
		}

		#endregion

		#region Constructors

		public DxOutputCollection() : base()
		{
		}

		public DxOutputCollection(Filter filter, string assessmentCode, decimal assessmentLcStageId, bool load = false)
			: this()
		{
			loadFilter = new LoadFilter(filter, new object[] { assessmentCode, assessmentLcStageId });

			if (load)
				Load();
		}

		public DxOutputCollection(string assessmentCode, decimal assessmentLcStageId, decimal outputCategoryId, bool load = false)
			: this()
		{
			loadFilter = new LoadFilter(Filter.AssessmentCodeAndLcStageIdAndOutputCategoryId, new object[] { assessmentCode, assessmentLcStageId, outputCategoryId });

			if (load)
				Load();
		}

        public DxOutputCollection(string assessmentCode, decimal assessmentLcStageId, string destinationCode, bool load = false) 
            : this()
        {
            loadFilter = new LoadFilter(Filter.AssessmentCode_LcStageId_DestinationCode, new object[] { assessmentCode, assessmentLcStageId, destinationCode });

            if (load)
                Load();
        }

        public DxOutputCollection(Filter filter, params object[] parameters)
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
					case Filter.AssessmentCodeAndAssessmentLcStageId:
						return queryByAssessmentCodeAndAssessmentLcStageId((string)loadFilter.Parameters[0], (decimal)loadFilter.Parameters[1]);
					case Filter.InputIdAndOutputCategoryId:
						return queryByInputIdAndOutputCategoryId((decimal)loadFilter.Parameters[0], (decimal)loadFilter.Parameters[1]);
                    case Filter.AssessmentCode_LcStageId_DestinationCode:
                        return queryByAssessmentCodeLcStageIdAndDestionationCode((string)loadFilter.Parameters[0], (decimal)loadFilter.Parameters[1], (string)loadFilter.Parameters[2]);
                    case Filter.AssessmentCodeAndLcStageIdAndOutputCategoryIdAndDestionationCode:
						return queryByAssessmentCodeAndLcStageIdAndOutputCategoryIdAndDestionationCode((string)loadFilter.Parameters[0], (decimal)loadFilter.Parameters[1], (decimal)loadFilter.Parameters[2], (string)loadFilter.Parameters[3]);
					case Filter.AssessmentCodeAndLcStageIdAndOutputCategoryId:
						return queryByAssessmentCodeAndLcStageIdAndOutputCategoryId((string)loadFilter.Parameters[0], (decimal)loadFilter.Parameters[1], (decimal)loadFilter.Parameters[2]);
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

		#region Public Methods

        public List<DxAssessmentLcStage> LoadItemsLcStage()
        {
            if (Count == 0)
                return new List<DxAssessmentLcStage>();

            DxAssessmentLcStageCollection lcStages = new DxAssessmentLcStageCollection();

            foreach (var output in this)
            {
                lcStages.Add(new DxAssessmentLcStage(output.AssessmentLcStageId));
            }

            lcStages.LoadItems();

            foreach (var output in this)
            {
                output.LcStage = lcStages[output.AssessmentLcStageId];
            }

            return lcStages.ToList();
        }

		#endregion
	}
}
