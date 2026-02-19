using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Selerant.DevEx.WebMvcModules.Infrastructure.Controllers;
using Selerant.DevEx.Web;
using Selerant.DevEx.Modules.DIRModule.WebMvcFrontEnd._ThisProject;
using Selerant.DevEx.Web.Security;
using Selerant.DevEx.BusinessLayer.Authorization;
using Selerant.DevEx.Modules.DIRModule.WebMvcFrontEnd.Areas.AssessmentNavigator.Models.Dialogs.ManageAssessment;
using Selerant.DevEx.Modules.DIRModule.ModuleBlocks._ThisProject;
using Selerant.DevEx.WebMvcModules.Infrastructure.Base;
using Selerant.DevEx.Modules.DIRModule.BackEnd.AssessmentManagement.BusinessObjects;
using Selerant.DevEx.CommonComponents;
using Selerant.DevEx.Modules.DIRModule.Configuration.Resources;
using Selerant.DevEx.Modules.DIRModule.WebMvcFrontEnd.Areas.AssessmentNavigator.Security.Interfaces;
using DxObject = Selerant.DevEx.BusinessLayer.DxObject;

namespace Selerant.DevEx.Modules.DIRModule.WebMvcFrontEnd.Areas.AssessmentNavigator.Controllers
{
	[ComponentDescriptor(new string[] {
		DIRModuleComponentIdentifier.ASSESSMENT_NAVIGATOR_RESOURCE_MANAGEMENT_PANEL,
		DIRModuleComponentIdentifier.CREATE_ASSESSMENT,
		DIRModuleComponentIdentifier.ASSESSMENT_NAVIGATOR,
		DIRModuleComponentIdentifier.SEARCH_ASSESSMENT
	}, SecurityObjectType = typeof(IManageAssessmentSecurity), VerifyControllerData = true, VerifyRequestSecurity = true)]
	public partial class ManageAssessmentDialogController : DevExBaseController<IManageAssessmentSecurity>
	{
		#region Private methods

		private void AssertSecurity(EntityType entityType, ViewControlControllerData controllerData)
		{
			switch (entityType)
			{
				case EntityType.LcStage:
					SecurityVerifier.AssertHasRights(() => GetSecurityObject(controllerData).CanUpdateLcStages);
					break;
				case EntityType.Destination:
					SecurityVerifier.AssertHasRights(() => GetSecurityObject(controllerData).CanUpdateDestinations);
					break;
				default:
					throw new SecurityPropertyNotImplementedException($"Not implemented security assesetion for entity type: {entityType} in: {this.GetType().FullName}.");
			}
		}

		#endregion

		// entry point for Index dialog view
		[HttpGet]
		[ControllerEntryPoint]
		[AntiCSRFToken(AntiCSRFTokenVerifyMode.Ignore)]
		public virtual ViewResult Index(ViewControlControllerData controllerData, EntityType entityType, string asmtIdentifiableString, decimal? lcStageId)
		{
			AssertSecurity(entityType, controllerData);
			SetHasVerifiedRequestSecurity();

			DxAssessment assessment = DxObject.ParseIdentifiableString<DxAssessment>(asmtIdentifiableString);
			EntityManager specificViewData;

			switch (entityType)
			{
				case EntityType.LcStage:
					specificViewData = new LcStageManager(assessment, entityType)
						.SetupViewData();

					break;
				case EntityType.Destination:
					specificViewData = new DestinationManager(assessment, lcStageId.Value, entityType)
						.SetupViewData();

					break;
				default:
					throw new NotImplementedException($"Specific EntityManager is not implemented for entityType: {entityType}");
			}

			var model = new ManageAssessmentDialogModel(GetControllerUrl(), controllerData, specificViewData);
			return View(DIRModuleInfo.Instance, MVC_DIR.AssessmentNavigator.ManageAssessmentDialog.Views.Index, model);
		}

		[HttpPost]
		[AntiCSRFToken(AntiCSRFTokenVerifyMode.Verify)]
		public virtual DataAjaxResult Manage(ViewControlControllerData controllerData, ManageAssessmentDialogModel.ManageRequest requestData)
		{
			AssertSecurity(requestData.EntityType, controllerData);
			SetHasVerifiedRequestSecurity();

			DxAssessment assessment = DxObject.ParseIdentifiableString<DxAssessment>(requestData.AsmtIdentifiableString);
			EntityManager manager;

			if (requestData.EntityType == EntityType.LcStage)
				manager = new LcStageManager(assessment, requestData.EntityType);
			else
				manager = new DestinationManager(assessment, requestData.LcStageId.Value, requestData.EntityType);

			var success = manager.Manage(requestData.RemovedEntities, requestData.AddedEntities);

			var response = new DataAjaxResult()
				.SetDataValue("success", success)
				.SetDataValue("message", success ? Locale.GetString(ResourceFiles.AssessmentManager, "ManageDialog_ListboxUpdated") :
												   Locale.GetString(ResourceFiles.AssessmentManager, "ManageDialog_ListboxUpdateFailed"));

			if (requestData.EntityType == EntityType.Destination)
			{
				ManageAssessmentUtility.AddDestinationsGridColumnsChange(response, requestData.RemovedEntities, requestData.AddedEntities);
			}

			return response;
		}
	}
}