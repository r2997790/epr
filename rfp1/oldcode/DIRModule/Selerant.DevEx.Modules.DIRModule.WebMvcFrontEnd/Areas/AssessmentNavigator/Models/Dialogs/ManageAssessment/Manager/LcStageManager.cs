using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Selerant.DevEx.Modules.DIRModule.BackEnd.AssessmentManagement.BusinessObjects;
using Selerant.DevEx.Modules.DIRModule.BackEnd.AssessmentTypeManagement.LcStageTemplate;
using Selerant.DevEx.Modules.DIRModule.Configuration.Resources;

namespace Selerant.DevEx.Modules.DIRModule.WebMvcFrontEnd.Areas.AssessmentNavigator.Models.Dialogs.ManageAssessment
{
	public class LcStageManager : EntityManager
	{
		public LcStageManager(DxAssessment assessment, EntityType entityType)
			 :base(assessment, entityType)
		{
		}

		public override EntityManager SetupViewData()
		{
			UsedListboxTitle = Locale.GetString(ResourceFiles.AssessmentManager, "UsedLCStages");
			AvailableListboxTitle = Locale.GetString(ResourceFiles.AssessmentManager, "AvailableLCStages");
			SetUsedListBoxItems();
			SetAvailableListBoxItems(UsedListBoxItems);
			
			return this;
		}

		public override bool Manage(List<string> removedEntities, List<string> addedEntities)
		{
			bool result = false;

			var assessmentLcStages = new DxAssessmentLcStageCollection(DxAssessmentLcStageCollection.Filter.AssessmentCode, Assessment.Code);
			assessmentLcStages.Load();

			result = RemoveLcStages(assessmentLcStages, removedEntities);
			result = AddLcStages(assessmentLcStages, addedEntities);

			return result;
		}

		#region Methods

		public void SetUsedListBoxItems()
		{
			var items = new List<SelectListItem>();
			var assessmentLcStages = new DxAssessmentLcStageCollection(DxAssessmentLcStageCollection.Filter.AssessmentCode, Assessment.Code);
			assessmentLcStages.Load();

			foreach (var item in assessmentLcStages.OrderBy(x => x.SortOrder))
			{
				items.Add(new SelectListItem { Text = item.Title, Value = item.IdentityKey });
			}
			UsedListBoxItems = items;
		}

		public void SetAvailableListBoxItems(List<SelectListItem> usedListBoxItems)
		{
			var items = new List<SelectListItem>();

			var lcStages = new DxLcStageTemplateCollection();
			lcStages.Load();

			var availableStages = lcStages.Select(x => x.Title).Except(usedListBoxItems.Select(y => y.Text)).ToList();

			foreach (var item in lcStages.Where(x => availableStages.Any(y => y == x.Title)).OrderBy(x => x.SortOrder))
			{
				items.Add(new SelectListItem { Text = item.Title, Value = item.IdentityKey });
			}

			AvailableListboxItems = items;
		}

		public bool RemoveLcStages(DxAssessmentLcStageCollection assessmentLcStages, List<string> removedEntities)
		{
			var itemsToDelete = new DxAssessmentLcStageCollection();

			var sectionOfNewAndCurrent = removedEntities.Where(x => assessmentLcStages.Select(y => y.IdentityKey).Any(y => y == x)).ToList();
			var lcStagedToRemove = assessmentLcStages.Where(x => sectionOfNewAndCurrent.Select(y => y).Any(y => y == x.IdentityKey)).ToList();

			foreach (var item in lcStagedToRemove)
			{
				var assessmentLcStage = new DxAssessmentLcStage(item.Id);
				itemsToDelete.Add(assessmentLcStage);
			}

			return itemsToDelete.Delete();
		}

		public bool AddLcStages(DxAssessmentLcStageCollection assessmentLcStages, List<string> addedEntities)
		{
			var itemsToSave = new DxAssessmentLcStageCollection();
			DxLcStageTemplateCollection lcStageTemplates = new DxLcStageTemplateCollection();
			lcStageTemplates.Load();

			var lcStagesToAdd = addedEntities.Select(x => x).Except(assessmentLcStages.Select(y => y.IdentityKey)).ToList();

			var realOnesToAdd = lcStageTemplates.Where(x => lcStagesToAdd.Select(y => y).Any(y => y == x.IdentityKey)).ToList();

			foreach (var item in realOnesToAdd)
			{
				var assessmentLcStage = new DxAssessmentLcStage();
				assessmentLcStage.SetNextIdentityKey();
				assessmentLcStage.AssessmentCode = Assessment.Code;

				assessmentLcStage.Title = item.Title;
				assessmentLcStage.SortOrder = item.SortOrder;
				assessmentLcStage.Visible = true;
				itemsToSave.Add(assessmentLcStage);
			}
			return itemsToSave.Create();
		}

		#endregion
	}
}