using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Selerant.DevEx.CommonComponents;
using Selerant.DevEx.Modules.DIRModule.BackEnd.DestinationManagement;
using Selerant.DevEx.Modules.DIRModule.Configuration.Resources;
using Selerant.DevEx.Web;
using Selerant.DevEx.WebMvcModules.Infrastructure.Base;

namespace Selerant.DevEx.Modules.DIRModule.WebMvcFrontEnd.Areas.AssessmentNavigator.Models.Dialogs.ManageAssessment
{
	public enum EntityType
	{
		LcStage = 0,
		Destination = 1,
	}

	public static class ManageAssessmentUtility
	{
		public static DataAjaxResult CreateManageDlgActivityActionResult(UrlHelper url, ViewControlControllerData controllerData, EntityType entityType, string assessmentIdent, decimal? lcStageId)
		{
			string dialogUrl = url.Action(MVC_DIR.AssessmentNavigator.ManageAssessmentDialog.Index(controllerData, entityType, assessmentIdent, lcStageId));

			var activity = new JSOpenDialogActivity()
			{
				Url = ComponentDataHelper.AddDataToUrl(dialogUrl, controllerData),
				Width = 800,
				Height = 500,
				IsResizable = true
			};

			switch (entityType)
			{
				case EntityType.LcStage:
					activity.SetCaption(Locale.GetString(ResourceFiles.AssessmentManager, "ManageAssessmentLCStages"));
					break;
				case EntityType.Destination:
					activity.SetCaption(Locale.GetString(ResourceFiles.AssessmentManager, "ManageMaterialDestination"));
					break;
				default:
					throw new NotImplementedException("Not implemente dialog mode for caption resources.");
			}

			return new DataAjaxResult().SetDataValue("activity", activity);
		}

		public static void AddDestinationsGridColumnsChange(DataAjaxResult response, List<string> removedDestinations, List<string> addedDestinations)
		{
			var wasteDestinations = new DxDestinationCollection(DxDestinationCollection.Filter.Waste, 1.0m);
			wasteDestinations.Load();

			var removedFood = new List<string>();
			var addedFood = new Dictionary<string, int>();

			var removedNonFood = new List<string>();
			var addedNonFood = new Dictionary<string, int>();

			foreach (DxDestination dest in wasteDestinations)
			{
				if (dest.UsedOn.HasFlag(DxDestination.DestinationUsedOn.Food))
				{
					if (removedDestinations.Contains(dest.Code))
						removedFood.Add(dest.Code);

					else if (addedDestinations.Contains(dest.Code))
						addedFood.Add(dest.Code, (int)dest.SortOrder);
				}
				// not using else to cover also Both
				if (dest.UsedOn.HasFlag(DxDestination.DestinationUsedOn.NonFood))
				{
					if (removedDestinations.Contains(dest.Code))
						removedNonFood.Add(dest.Code);

					else if (addedDestinations.Contains(dest.Code))
						addedNonFood.Add(dest.Code, (int)dest.SortOrder);
				}
			}

			response.SetDataValue("removedFood", removedFood)
					.SetDataValue("addedFood", addedFood)
					.SetDataValue("removedNonFood", removedNonFood)
					.SetDataValue("addedNonFood", addedNonFood);
		}
	}
}