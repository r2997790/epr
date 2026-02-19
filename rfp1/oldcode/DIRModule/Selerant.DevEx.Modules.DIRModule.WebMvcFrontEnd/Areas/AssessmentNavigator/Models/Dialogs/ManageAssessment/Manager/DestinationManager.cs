using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Selerant.DevEx.BusinessLayer;
using Selerant.DevEx.Modules.DIRModule.BackEnd.AssessmentManagement.BusinessObjects;
using Selerant.DevEx.Modules.DIRModule.BackEnd.DestinationManagement;
using Selerant.DevEx.Modules.DIRModule.BackEnd.InputManagement;
using Selerant.DevEx.Modules.DIRModule.BackEnd.OutputManagement;
using Selerant.DevEx.Modules.DIRModule.Configuration.Resources;
using Selerant.DevEx.Modules.DIRModule.WebMvcFrontEnd.Helpers;

namespace Selerant.DevEx.Modules.DIRModule.WebMvcFrontEnd.Areas.AssessmentNavigator.Models.Dialogs.ManageAssessment
{
	public class DestinationManager : EntityManager
	{
		#region Properties

		public override decimal LcStageId { get; }

		#endregion

		#region Constructors

		public DestinationManager(DxAssessment assessment, decimal lcStageId, EntityType entityType)
		: base(assessment, entityType)
		{
			LcStageId = lcStageId;
		}

		#endregion

		#region Override

		public override EntityManager SetupViewData()
		{
			AvailableListboxTitle = Locale.GetString(ResourceFiles.AssessmentManager, "ManageDialog_AvailableDestinationManagment");
			UsedListboxTitle = Locale.GetString(ResourceFiles.AssessmentManager, "ManageDialog_UsedDestination");
			SetUsedListBoxItems();
			SetAvailableListBoxItems(UsedListBoxItems);

			return this;
		}

		public override bool Manage(List<string> removedEntities, List<string> addedEntities)
		{
			bool result = true;

			using (DxUnitOfWork uow = DxUnitOfWork.New())
			{
				var assessmentDestinations = new DxAssessmentDestinationCollection(DxAssessmentDestinationCollection.Filter.AssessmentCode, Assessment.Code);
				assessmentDestinations.Load();

				List<DxDestination> destinations = assessmentDestinations.LoadItemsDestination();

				var inputDestinationsToDelete = new DxInputDestination();
				var destinationCodesToRemove = removedEntities.Where(x => destinations.Select(y => y.Code).Any(y => y == x)).ToList();
				var itemsToDelete = new DxAssessmentDestinationCollection();
                
				destinationCodesToRemove.ForEach(destinationCode => 
                {
                    itemsToDelete.AddItem(new DxAssessmentDestination(Assessment.Code, destinationCode));
                    DxOutputCollection outputsToDelete = new DxOutputCollection(Assessment.Code, LcStageId, destinationCode, true);
                    result &= outputsToDelete.Delete();
                });

				result &= itemsToDelete.Delete();

				destinationCodesToRemove.ForEach(destCode => DxInputDestination.DeleteByDestination(Assessment.Code, destCode));

				var destinationsToAdd = addedEntities.Select(x => x).Except(destinations.Select(y => y.Code)).ToList();
				var itemsToAdd = new DxAssessmentDestinationCollection();

				destinationsToAdd.ForEach(x => itemsToAdd.AddItem(new DxAssessmentDestination(Assessment.Code, x)));

				result &= itemsToAdd.Create();

				if (result)
					uow.Commit();
				else
					uow.Abort();
			}

			return result;
		}

		#endregion

		#region Methods

		public void SetUsedListBoxItems()
		{
			var items = new List<SelectListItem>();
			var assessmentDestinations = new DxAssessmentDestinationCollection(DxAssessmentDestinationCollection.Filter.AssessmentCode, Assessment.Code);
			assessmentDestinations.Load();		

			List<DxDestination> destinations = assessmentDestinations.LoadItemsDestination();

			var availableDestinations = destinations.Where(x => destinations.Select(y => y.Code).Any(y => y == x.Code)).ToList();

			availableDestinations.ForEach(x => items.Add( new SelectListItem { Text = x.Title, Value = x.Code }));
			
			UsedListBoxItems = items;
		}

		public void SetAvailableListBoxItems(List<SelectListItem> usedListBoxItems)
		{
			var items = new List<SelectListItem>();

			var destinations = new DxDestinationCollection();
			destinations.Load();

			var availableDestinations = destinations.Select(x => x.Code).Except(usedListBoxItems.Select(y => y.Value)).ToList();

			foreach (var item in destinations.Where(x => availableDestinations.Any(y => y == x.Code)))
			{
				if (!GridHelpers.Instance.DefaultDestinations.Contains(item.Code))
				{
					items.Add(new SelectListItem { Text = item.Title, Value = item.Code });
				}
			}

			AvailableListboxItems = items;
		}

		#endregion
	}
}