using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Selerant.DevEx.Modules.DIRModule.WebMvcFrontEnd._ThisProject;
using Selerant.DevEx.Web;
using ActionResult = System.Web.Mvc.ActionResult;
using Selerant.DevEx.Modules.DIRModule.BackEnd.AssessmentManagement.BusinessObjects;
using Selerant.DevEx.BusinessLayer.Authorization;
using Selerant.DevEx.Modules.DIRModule.WebMvcFrontEnd.Areas.AssessmentNavigator.Models.ObjectCreation;
using Selerant.DevEx.WebMvcModules.Infrastructure.Controllers;
using Selerant.DevEx.Modules.DIRModule.ModuleBlocks._ThisProject;
using Selerant.DevEx.Web.Security;
using static Selerant.DevEx.Modules.DIRModule.WebMvcFrontEnd.Areas.AssessmentNavigator.Models.ObjectCreation.IndexModel;
using Selerant.DevEx.BusinessLayer;
using Selerant.DevEx.WebMvcModules.Infrastructure.Base;
using Selerant.DevEx.CommonComponents;
using Selerant.DevEx.Modules.DIRModule.Configuration.Resources;
using Selerant.DevEx.Modules.DIRModule.BackEnd.InputManagement;
using Selerant.DevEx.Modules.DIRModule.WebMvcFrontEnd.Areas.AssessmentNavigator.Models.Panels.ResourceManagement;
using Selerant.DevEx.Modules.DIRModule.WebMvcFrontEnd.Areas.AssessmentNavigator.Models.ObjectCreation.MergeInputs;
using Selerant.DevEx.WebMvcModules.Infrastructure.Controls.SpreadsheetControl;
using Selerant.DevEx.Modules.DIRModule.WebMvcFrontEnd.Helpers;
using Selerant.DevEx.Modules.DIRModule.WebMvcFrontEnd.Areas.AssessmentNavigator.Security.Interfaces;
using Selerant.DevEx.Modules.DIRModule.WebMvcFrontEnd.Areas.AssessmentNavigator.Models.Dialogs.ManageAssessment;

namespace Selerant.DevEx.Modules.DIRModule.WebMvcFrontEnd.Areas.AssessmentNavigator.Controllers
{
    [ComponentDescriptor(
		new string[] {
			DIRModuleComponentIdentifier.CREATE_ASSESSMENT,
			DIRModuleComponentIdentifier.ASSESSMENT_NAVIGATOR_RESOURCE_MANAGEMENT_PANEL,
			DIRModuleComponentIdentifier.SEARCH_ASSESSMENT
		},
		SecurityObjectType = typeof(ICreateAssessmentSecurity),
		VerifyControllerData = true,
		VerifyRequestSecurity = true)]
	public partial class CreateAssessmentDialogController : DevExBaseController
	{
        #region Public Actions
        
        [HttpGet]
        [AntiCSRFToken(AntiCSRFTokenVerifyMode.Ignore)]
        public virtual ActionResult NewAssessmentDialogIndex(ViewControlControllerData controllerData)
        {
            SecurityVerifier.AssertHasRights(() => GetSecurity(controllerData).CanCreateAssessment);
            SetHasVerifiedRequestSecurity();

            return NewAssessmentDlgOpen(controllerData);
        }

		[HttpGet]
		[AntiCSRFToken(AntiCSRFTokenVerifyMode.Ignore)]
		public virtual ActionResult BusinessDataDialogIndex(ViewControlControllerData controllerData, string identifiableString, bool cancelOrCloseDeletes)
		{
			SecurityVerifier.AssertHasRights(() => GetSecurity(controllerData).CanCreateAssessment);
			SetHasVerifiedRequestSecurity();

			DxAssessment assessment = (DxAssessment)DxObject.ParseIdentifiableString(identifiableString);
			var model = new BusinessDataDialogModel(GetControllerUrl(), controllerData, assessment, cancelOrCloseDeletes);
			return View(DIRModuleInfo.Instance, MVC_DIR.AssessmentNavigator.CreateAssessmentDialog.Views.BusinessDataDialog, model);
		}

		[HttpGet]
        [AntiCSRFToken(AntiCSRFTokenVerifyMode.Ignore)]
        public virtual ActionResult MergeInputsDialogIndex(ViewControlControllerData controllerData, string identifiableString, decimal lcStageId, decimal nextLcStageId)
        {
            SecurityVerifier.AssertHasRights(() => GetSecurity(controllerData).CanCreateAssessment);
            SetHasVerifiedRequestSecurity();

            var model = new MergeInputsDialogModel(GetControllerUrl(), controllerData, identifiableString, lcStageId, nextLcStageId);

            return View(DIRModuleInfo.Instance, MVC_DIR.AssessmentNavigator.CreateAssessmentDialog.Views.MergeInputs.MergeInputsDialog, model);
        }

        [HttpPost]
        [AntiCSRFToken(AntiCSRFTokenVerifyMode.Verify)]
        public virtual DataAjaxResult CarryMergedProductOverToTheNextStage(ViewControlControllerData controllerData, string identifiableString, decimal lcStageId, decimal nextLcStageId, Spreadsheet.ChangesAjaxParameter gridParam)
        {
            SecurityVerifier.AssertHasRights(() => GetSecurity(controllerData).CanCreateAssessment);
            SetHasVerifiedRequestSecurity();

            var model = new MergeInputsDialogModel(GetControllerUrl(), controllerData, identifiableString, lcStageId, nextLcStageId);

            var success = model.SaveMergedInputRows(gridParam);

            return new DataAjaxResult(new { Success = success });
        }

        [HttpPost]
        [AntiCSRFToken(AntiCSRFTokenVerifyMode.Verify)]
        public virtual DataAjaxResult HasProductsToCarryOver(ViewControlControllerData controllerData, string identifiableString, decimal lcStageId, decimal nextLcStageId)
        {
            SecurityVerifier.AssertHasRights(() => GetSecurity(controllerData).CanCreateAssessment);
            SetHasVerifiedRequestSecurity();

            DxAssessment assessment = DxObject.ParseIdentifiableString<DxAssessment>(identifiableString);
			(decimal currentInputsCount, decimal nextInputsCount) = DxCarriedInputCollection.HasInputsToCarryOverCount(assessment.Code, lcStageId, nextLcStageId);

            return new DataAjaxResult(new { HasInputs = currentInputsCount > 0 && nextInputsCount == 0 });
        }

        [HttpPost]
        [AntiCSRFToken(AntiCSRFTokenVerifyMode.Verify)]
        public virtual DataAjaxResult CarryProductOverToTheNextStage(ViewControlControllerData controllerData, string identifiableString, decimal lcStageId, decimal nextLcStageId, bool mergeInputs)
        {
            SecurityVerifier.AssertHasRights(() => GetSecurity(controllerData).CanCreateAssessment);
            SetHasVerifiedRequestSecurity();

			DxAssessment assessment = (DxAssessment)DxObject.ParseIdentifiableString(identifiableString);

			// update next LcStage with carried over source
			var nextLcStage = new DxAssessmentLcStage(nextLcStageId, true);
			nextLcStage.SourceAssessmentLcStageId = lcStageId;
			nextLcStage.Update();

			if (mergeInputs)
            {
                var activity = new JSOpenDialogActivity();
                activity.Url = Utilities.MapUrlPath(Url.Action(MVC_DIR.AssessmentNavigator.CreateAssessmentDialog.MergeInputsDialogIndex(controllerData, identifiableString, lcStageId, nextLcStageId)));
                activity.CaptionDescriptor.Text = Locale.GetString(ResourceFiles.AssessmentManager, "MergeInputsDialogTitle");
                activity.Width = 800;
                activity.Height = 700;
                activity.IsResizable = true;

                return new DataAjaxResult(new { Success = true, Activity = activity });
            }
            else
            {
                var inputs = DxInputCollection.GetCarriedInputs(assessment.Code, lcStageId, true);

                inputs.ForEach((input, _) => 
                {
                    input.AssessmentLcStageId = nextLcStageId;
                    input.InedibleParts = 0;
                });

                var carriedInputs = new DxInputCollection();
                carriedInputs.AddRange(inputs);
                carriedInputs.Create();

                return new DataAjaxResult(new { Success = true });
            }
        }

		[HttpPost]
		[AntiCSRFToken(AntiCSRFTokenVerifyMode.Verify)]
		public virtual DataAjaxResult ClearNextLcStageSource(ViewControlControllerData controllerData, decimal nextLcStageId)
		{
			SecurityVerifier.AssertHasRights(() => GetSecurity(controllerData).CanCreateAssessment);
			SetHasVerifiedRequestSecurity();

			DxAssessmentLcStage nextAsmtLcStage = new DxAssessmentLcStage(nextLcStageId, true);
			if (nextAsmtLcStage.PersistenceStatus == DxPersistenceStatus.UpToDate && nextAsmtLcStage.SourceAssessmentLcStageId != null)
			{
				nextAsmtLcStage.SourceAssessmentLcStageId = null;
				nextAsmtLcStage.Update();
			}

			return new DataAjaxResult().SetDataValue("Success", true);
		}

		[HttpPost]
        [AntiCSRFToken(AntiCSRFTokenVerifyMode.Verify)]
        public virtual DataAjaxResult CreateMaterial(ViewControlControllerData controllerData, string description, string categoryType)
        {
            SecurityVerifier.AssertHasRights(() => GetSecurity(controllerData).CanCreateAssessment);
            SetHasVerifiedRequestSecurity();

            bool actionResult = true;
            string identifiableString = string.Empty;

            using (DxUnitOfWork unitOfWork = DxUnitOfWork.New())
            {
                DxMaterial material = MaterialHelpers.CreateNewMaterial(description, categoryType, out bool result);
                actionResult &= result;

                if (actionResult)
                {
                    identifiableString = material.IdentifiableString;
                    unitOfWork.Commit();
                }
                else
                    unitOfWork.Abort();
            }

            return new DataAjaxResult(new { Success = actionResult, IdentifiableString = identifiableString });
        }

        [HttpPost]
        [AntiCSRFToken(AntiCSRFTokenVerifyMode.Verify)]
        public virtual DataAjaxResult CreateNewAssessment(ViewControlControllerData controllerData, CreateNewAssessmentDialogFormData formData)
        {
            SecurityVerifier.AssertHasRights(() => GetSecurity(controllerData).CanCreateAssessment);
            SetHasVerifiedRequestSecurity();

            string identifiableString = string.Empty;
            string errorMessage = string.Empty;

            var model = new IndexModel(GetControllerUrl(), controllerData);
            model.CreateNewAssessment(formData, out identifiableString, out errorMessage);

            DxAssessment newAssessment = (DxAssessment)DxObject.ParseIdentifiableString(identifiableString);

            var activity = new JSOpenDialogActivity()
			{
				Url = Url.Action(MVC_DIR.AssessmentNavigator.CreateAssessmentDialog.BusinessDataDialogIndex(controllerData, newAssessment.IdentifiableString, true)),
				Width = short.MaxValue,
				Height = short.MaxValue,
				IsResizable = false
			}
			.SetCaption(Locale.GetString(ResourceFiles.AssessmentManager, "ANNNavTabResourceManagement"));

            return new DataAjaxResult(new Dictionary<string, object>
            {
                { "Activity", activity },
				{ "ErrorMessage", errorMessage}
            });
        }

		[HttpPost]
		[AntiCSRFToken(AntiCSRFTokenVerifyMode.Verify)]
		public virtual DataAjaxResult CreateNewAssessmentAndCloseDialog(ViewControlControllerData controllerData, CreateNewAssessmentDialogFormData formData)
		{
			SecurityVerifier.AssertHasRights(() => GetSecurity(controllerData).CanCreateAssessment);
			SetHasVerifiedRequestSecurity();

			string identifiableString = string.Empty;
			string errorMessage = string.Empty;

			var model = new IndexModel(GetControllerUrl(), controllerData);
			bool result = model.CreateNewAssessment(formData, out identifiableString, out errorMessage);
			string url = string.Empty;

			if (result)
			{
				DxAssessment newAssessment = (DxAssessment)DxObject.ParseIdentifiableString(identifiableString);
				newAssessment.Load();
				newAssessment.Status = DxAssessment.AssessmentStatus.DEVELOPMENT;
				newAssessment.Update();

				url = BusinessLayer.Navigation.NavigationUtilities.GetNavigatorUrl(newAssessment);
			}

			return new DataAjaxResult(new Dictionary<string, Object>
			{
				{ "Url", url},
				{ "ErrorMessage", errorMessage }
			});
		}

		[HttpPost]
		[AntiCSRFToken(AntiCSRFTokenVerifyMode.Verify)]
		public virtual DataAjaxResult ChangeStatusAndCloseDialog(ViewControlControllerData controllerData, string identifiableString)
		{
			SecurityVerifier.AssertHasRights(() => GetSecurity(controllerData).CanCreateAssessment);
			SetHasVerifiedRequestSecurity();

			string errorMessage = string.Empty;
			string url = string.Empty;

			DxAssessment assessment = DxAssessment.ParseIdentifiableString<DxAssessment>(identifiableString);
			assessment.Load();

			assessment.Status = DxAssessment.AssessmentStatus.DEVELOPMENT;
			if (!assessment.Update())
				errorMessage = Locale.GetString(ResourceFiles.AssessmentManager, "BusinessDataDialog_UpdateAssessmentError");
			
			url = Selerant.DevEx.BusinessLayer.Navigation.NavigationUtilities.GetNavigatorUrl(assessment);

			return new DataAjaxResult(new Dictionary<string, Object>
			{
				{ "Url", url},
				{ "ErrorMessage", errorMessage }
			});
		}

		[AcceptVerbs(HttpVerbs.Post)]
        [AntiCSRFToken(AntiCSRFTokenVerifyMode.Verify)]
        public virtual ActionResult DeleteAssessment(ViewControlControllerData controllerData, string identifiableString)
        {
            SecurityVerifier.AssertHasRights(() => GetSecurity(controllerData).CanCreateAssessment);
            SetHasVerifiedRequestSecurity();

            string errorMessage = string.Empty;
			DxAssessment assessment = DxAssessment.ParseIdentifiableString<DxAssessment>(identifiableString);
			assessment.Load();
			
			if(!assessment.Delete())
				errorMessage = Locale.GetString(ResourceFiles.AssessmentManager, "BusinessDataDialog_DeleteAssessmentError");

			return new DataAjaxResult(new Dictionary<string, Object>
                { { "ErrorMessage", errorMessage }
            });
        }

		/// <summary>
		/// Invokes LCStages managment dialog
		/// </summary>
		[HttpPost]
		[AntiCSRFToken(AntiCSRFTokenVerifyMode.Verify)]
		public virtual DataAjaxResult ManageAssessmentRaiseDialog(ViewControlControllerData controllerData, string identifiableString, EntityType entityType)
		{
			SecurityVerifier.AssertHasRights(() => GetSecurity(controllerData).CanCreateAssessment);
			SetHasVerifiedRequestSecurity();

			return ManageAssessmentUtility.CreateManageDlgActivityActionResult(Url, controllerData, entityType, identifiableString, null);
		}

		[HttpPost]
		[AntiCSRFToken(AntiCSRFTokenVerifyMode.Verify)]
		public virtual ActionResult LCStageStepsPartial(ViewControlControllerData controllerData, string assessmentIdent)
		{
			SecurityVerifier.AssertHasRights(() => GetSecurity(controllerData).CanCreateAssessment);
			SetHasVerifiedRequestSecurity();

			var model = new Models.ObjectCreation.Shared.LCStageStepsModel(DxObject.ParseIdentifiableString<DxAssessment>(assessmentIdent));
			return PartialView(DIRModuleInfo.Instance, MVC_DIR.AssessmentNavigator.CreateAssessmentDialog.Views.Shared.LCStageSteps, model);
		}

		#endregion

		#region Private Methods

		private ICreateAssessmentSecurity GetSecurity(ViewControlControllerData controllerData)
        {
            return controllerData.SecurityObject as ICreateAssessmentSecurity;
        }

		#endregion

		#region Private Actions

		private ViewResult NewAssessmentDlgOpen(ViewControlControllerData controllerData)
        {
            IndexModel model = new IndexModel(GetControllerUrl(), controllerData);

            return View(DIRModuleInfo.Instance, MVC_DIR.AssessmentNavigator.CreateAssessmentDialog.Views.Index, model);
        }

        #endregion
    }
}