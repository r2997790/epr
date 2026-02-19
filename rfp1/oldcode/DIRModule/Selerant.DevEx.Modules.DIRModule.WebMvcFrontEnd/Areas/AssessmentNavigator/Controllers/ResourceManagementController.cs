using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using ActionResult = System.Web.Mvc.ActionResult;
using Selerant.DevEx.Web;
using Selerant.DevEx.WebMvcModules.Infrastructure.Base;
using Selerant.DevEx.WebMvcModules.Infrastructure.Navigators.Controllers;
using Selerant.DevEx.WebMvcModules.Infrastructure.Navigators.Helpers;
using Selerant.DevEx.WebMvcModules.Infrastructure.Navigators.Models;
using Selerant.DevEx.Modules.DIRModule.BackEnd.AssessmentManagement.BusinessObjects;
using Selerant.DevEx.Modules.DIRModule.WebMvcFrontEnd._ThisProject;
using Selerant.DevEx.Modules.DIRModule.ModuleBlocks._ThisProject;
using Selerant.DevEx.Modules.DIRModule.WebMvcFrontEnd.Areas.AssessmentNavigator.Models.Panels.ResourceManagement;
using Shared = Selerant.DevEx.Modules.DIRModule.WebMvcFrontEnd.Areas.AssessmentNavigator.Models.Panels.ResourceManagement.Shared;
using Selerant.DevEx.Web.Security;
using Selerant.DevEx.BusinessLayer.Authorization;
using Selerant.DevEx.WebMvcModules.Helpers.jqGrid;
using Selerant.DevEx.WebMvcModules.Infrastructure.Controls.HtmlGrid;
using Selerant.DevEx.BusinessLayer;
using Selerant.DevEx.Modules.DIRModule.BackEnd.InputManagement;
using Selerant.DevEx.Modules.DIRModule.BackEnd.OutputManagement;
using Selerant.DevEx.Modules.DIRModule.BackEnd.BusinessCostManagement;
using Selerant.DevEx.Modules.DIRModule.WebMvcFrontEnd.Areas.AssessmentNavigator.Models.Panels.ResourceManagement.Json;
using Selerant.DevEx.Modules.DIRModule.Configuration.Resources;
using Selerant.DevEx.Modules.DIRModule.WebMvcFrontEnd.Areas.AssessmentNavigator.Security;
using System;
using Selerant.DevEx.WebPages;
using Selerant.DevEx.Modules.DIRModule.WebMvcFrontEnd.Areas.AssessmentNavigator.Models.Panels.ResourceManagement.Shared;
using Selerant.DevEx.Modules.DIRModule.WebMvcFrontEnd.Areas.AssessmentNavigator.Models.Dialogs.ManageAssessment;
using InputCategoryType = Selerant.DevEx.Modules.DIRModule.BackEnd.Common.Constants.InputType;
using Selerant.DevEx.Modules.DIRModule.BackEnd.Common;
using Selerant.DevEx.Modules.DIRModule.Configuration.Grids;
using Selerant.DevEx.CommonComponents;
using Selerant.DevEx.Modules.DIRModule.BackEnd.ResourceNote;
using Selerant.DevEx.Modules.DIRModule.WebMvcFrontEnd.Helpers;

namespace Selerant.DevEx.Modules.DIRModule.WebMvcFrontEnd.Areas.AssessmentNavigator.Controllers
{
	/// <summary>
	/// MVC controller for panel Resource Management
	/// </summary>
	[ComponentDescriptor(
		DIRModuleComponentIdentifier.ASSESSMENT_NAVIGATOR_RESOURCE_MANAGEMENT_PANEL,
		SecurityObjectType = typeof(ResourceManagementSecurity),
		VerifyControllerData = true,
		VerifyRequestSecurity = true
	)]
	[NavigatorPanel("Selerant.DevEx.Modules.DIRModule.AssessmentNavigator.ResourceManagement")]
	public partial class ResourceManagementController : DevExNavigatorPanelController<DxAssessment, ResourceManagementSecurity>
	{
		#region Constants

		private const string PRODUCT_1 = "PRODUCT";
		private const string PRODUCT_2 = "PRODUCT_2";
		private const string PRODUCT_3 = "PRODUCT_3";

		private const string COPRODUCT = "COPRODUCT";
		private const string FOOD_RESCUE = "FOOD_RESCUE";
		private const string FOOD = "Food";
		private const string NONFOOD = "NonFood";

		#endregion

		private static List<string> ProductCodes = new List<string>() { PRODUCT_1, PRODUCT_2, PRODUCT_3 };
		/// <summary>
		/// Generates a new Index Model
		/// </summary>
		/// <param name="controllerData"></param>
		/// <returns></returns>
		protected override NavigatorPanelModel<DxAssessment> BuildIndexModel(NavigatorPanelControllerData<DxAssessment> controllerData)
		{
			return new ResourceManagementModel(GetControllerUrl(), controllerData);
		}

        #region Private methods

        private decimal GetLcStageIdFromRequest()
        {
            decimal.TryParse(CurrentRequestData.Instance.Request["lcStageId"], out decimal lcStageId);           

            return lcStageId;
        }

        private bool DeleteGridRow<T>(string rowIdentifiableString) where T : DxObject
		{
			bool result = false;

			result = DxObject.TryParseIdentifiableString<T>(rowIdentifiableString, out T resultingRowObject);

			if (result && resultingRowObject.Exists())
			{
				result = resultingRowObject.Delete();
			}
			return result;
		}

		private ViewResult ModuleView(string viewName, object model)
		{
			return View(DIRModuleInfo.Instance, viewName, model);
		}

		private InputResponseModel CreateInputResponseModel(DxInput dxInput, string inputCategoryType, DxAssessment assessment)
		{
			assessment.LoadEntity();
			var amountFormatter = new AmountFormatter(assessment.CurrencySymbol);

			return new InputResponseModel(dxInput, inputCategoryType, amountFormatter).SetMaterialProperties(Url);
		}

		#endregion

		// Called from view CreateAssessmentDialog/BusinessDataDialog
		[HttpGet]
		[AntiCSRFToken(AntiCSRFTokenVerifyMode.Ignore)]
		[ControllerEntryPoint]
		public virtual ActionResult InputTabPartial(string assessmentId, decimal lcStageId)
		{
			DxAssessment dxAssessment = new DxAssessment(assessmentId);

			NavigatorPanelControllerData<DxAssessment> navigatorPanelControllerData =
				BuildNavigatorPanelControllerData(dxAssessment.IdentifiableString,
												  Configuration.Navigation.Assessment.NamesRepository.AssessmentPanelNames.TAB_RESOURCE_MANAGEMENT,
												  NavigatorModel.Modes.Standard);
			
			SecurityVerifier.AssertHasRights(() => GetSecurityObject(navigatorPanelControllerData).CanView);
			SetHasVerifiedRequestSecurity();

			var model = new Shared.InputsGridTabModel(GetControllerUrl(), navigatorPanelControllerData, lcStageId, BaseGridTabModel.ViewMode.CreationDialog);
			return PartialView(DIRModuleInfo.Instance, MVC_DIR.AssessmentNavigator.ResourceManagement.Views.Shared.InputsGridTab, model);
		}

		// Called from view CreateAssessmentDialog/BusinessDataDialog
		[HttpGet]
		[AntiCSRFToken(AntiCSRFTokenVerifyMode.Ignore)]
		[ControllerEntryPoint]
		public virtual ActionResult DestinationsTabPartial(string assessmentId, decimal lcStageId, string isFood)
		{
			DxAssessment dxAssessment = new DxAssessment(assessmentId);

			NavigatorPanelControllerData<DxAssessment> navigatorPanelControllerData =
				BuildNavigatorPanelControllerData(dxAssessment.IdentifiableString,
												  Configuration.Navigation.Assessment.NamesRepository.AssessmentPanelNames.TAB_RESOURCE_MANAGEMENT,
												  NavigatorModel.Modes.Standard);

			SecurityVerifier.AssertHasRights(() => GetSecurityObject(navigatorPanelControllerData).CanView);
			SetHasVerifiedRequestSecurity();

			var model = new DestinationsGridTabModel(GetControllerUrl(), navigatorPanelControllerData, lcStageId, isFood, BaseGridTabModel.ViewMode.CreationDialog);

			return PartialView(DIRModuleInfo.Instance, MVC_DIR.AssessmentNavigator.ResourceManagement.Views.Shared.DestinationsGridTab, model);
		}

		// Called from view CreateAssessmentDialog/BusinessDataDialog
		[HttpGet]
		[AntiCSRFToken(AntiCSRFTokenVerifyMode.Ignore)]
		[ControllerEntryPoint]
		public virtual ActionResult OutputTabPartial(string assessmentId, decimal lcStageId)
		{
			DxAssessment dxAssessment = new DxAssessment(assessmentId);

			NavigatorPanelControllerData<DxAssessment> navigatorPanelControllerData =
				BuildNavigatorPanelControllerData(dxAssessment.IdentifiableString,
												  Configuration.Navigation.Assessment.NamesRepository.AssessmentPanelNames.TAB_RESOURCE_MANAGEMENT,
												  NavigatorModel.Modes.Standard);

			SecurityVerifier.AssertHasRights(() => GetSecurityObject(navigatorPanelControllerData).CanView);
			SetHasVerifiedRequestSecurity();

			var model = new Models.Panels.ResourceManagement.Shared.OutputsGridTabModel(GetControllerUrl(), navigatorPanelControllerData, lcStageId, BaseGridTabModel.ViewMode.CreationDialog);
			return PartialView(DIRModuleInfo.Instance, MVC_DIR.AssessmentNavigator.ResourceManagement.Views.Shared.OutputsGridTab, model);
		}

		// Called from view CreateAssessmentDialog/BusinessDataDialog
		[HttpGet]
		[AntiCSRFToken(AntiCSRFTokenVerifyMode.Ignore)]
		[ControllerEntryPoint]
		public virtual ActionResult BusinessCostTabPartial(string assessmentId, decimal lcStageId)
		{
			DxAssessment dxAssessment = new DxAssessment(assessmentId);

			NavigatorPanelControllerData<DxAssessment> navigatorPanelControllerData =
				BuildNavigatorPanelControllerData(dxAssessment.IdentifiableString,
												  Configuration.Navigation.Assessment.NamesRepository.AssessmentPanelNames.TAB_RESOURCE_MANAGEMENT,
												  NavigatorModel.Modes.Standard);
			
			SecurityVerifier.AssertHasRights(() => GetSecurityObject(navigatorPanelControllerData).CanView);
			SetHasVerifiedRequestSecurity();

			var model = new Shared.BusinessCostGridTabModel(GetControllerUrl(), navigatorPanelControllerData, lcStageId, BaseGridTabModel.ViewMode.CreationDialog);
			return PartialView(DIRModuleInfo.Instance, MVC_DIR.AssessmentNavigator.ResourceManagement.Views.Shared.BusinessCostGridTab, model);
		}

		[ControllerEntryPoint]
		[AcceptVerbs(HttpVerbs.Get | HttpVerbs.Post)]
		[AntiCSRFToken(AntiCSRFTokenVerifyMode.Auto)]
		public virtual JsonResult GetInputsTree(NavigatorPanelControllerData<DxAssessment> controllerData, GridSettings clientGridRequest, bool loadData = false)
		{
			SecurityVerifier.AssertHasRights(() => GetSecurityObject(controllerData).CanView);
			SetHasVerifiedRequestSecurity();

            if (!loadData)
                return Json(new byte[0], JsonRequestBehavior.AllowGet);

            var model = (ResourceManagementModel)CreatePanelModel(controllerData);

            decimal lcStageId = GetLcStageIdFromRequest();

            if (lcStageId > 0)
                model.LcStageId = lcStageId;

            var data = model.BuildInputsGridList();

			var helper = new NavigatorPanelHtmlGridHelper(controllerData, GridNames.INPUTS, null, model.SecurityObject);

			int totalPages = data.Count > 0 ? 1 : 0;
			int totalRecords = data.Count;

			var jsonData = new JsonGridData
			{
				total = totalPages,
				page = clientGridRequest.pageIndex,
				records = totalRecords,
				rows = helper.ConvertToHtmlGridTreeArray(data)
			};

			return Json(jsonData, JsonRequestBehavior.AllowGet);
		}

		[ControllerEntryPoint]
		[AcceptVerbs(HttpVerbs.Get | HttpVerbs.Post)]
		[AntiCSRFToken(AntiCSRFTokenVerifyMode.Auto)]
		public virtual JsonResult GetOutputsTree(NavigatorPanelControllerData<DxAssessment> controllerData, GridSettings clientGridRequest, bool loadData = false)
		{
			SecurityVerifier.AssertHasRights(() => GetSecurityObject(controllerData).CanView);
			SetHasVerifiedRequestSecurity();

            if (!loadData)
                return Json(new byte[0], JsonRequestBehavior.AllowGet);

            var model = (ResourceManagementModel)CreatePanelModel(controllerData);

            decimal lcStageId = GetLcStageIdFromRequest();

            if (lcStageId > 0)
                model.LcStageId = lcStageId;

            var data = model.BuildOutputsGridList();

			var helper = new NavigatorPanelHtmlGridHelper(controllerData, GridNames.OUTPUTS, null, model.SecurityObject);

			int totalPages = data.Count > 0 ? 1 : 0;
			int totalRecords = data.Count;

			var jsonData = new JsonGridData
			{
				total = totalPages,
				page = clientGridRequest.pageIndex,
				records = totalRecords,
				rows = helper.ConvertToHtmlGridTreeArray(data)
			};

			return Json(jsonData, JsonRequestBehavior.AllowGet);
		}

		[HttpPost]
		[AntiCSRFToken(AntiCSRFTokenVerifyMode.Verify)]
		public virtual DataAjaxResult CheckValidateAssessmentLcStage(NavigatorPanelControllerData<DxAssessment> controllerData, decimal lcStageId)
		{
			SecurityVerifier.AssertHasRights(() => GetSecurityObject(controllerData).CanView);
			SetHasVerifiedRequestSecurity();

			InputsGridTabModel model = new InputsGridTabModel(GetControllerUrl(), controllerData, lcStageId);

			return new DataAjaxResult(new { WarningMassageForInvalidLcStage =  model.IsNoValidLcStage() });
		}

		[ControllerEntryPoint]
		[AcceptVerbs(HttpVerbs.Get | HttpVerbs.Post)]
		[AntiCSRFToken(AntiCSRFTokenVerifyMode.Auto)]
		public virtual JsonResult GetBusinessCostGrid(NavigatorPanelControllerData<DxAssessment> controllerData, GridSettings clientGridRequest, bool loadData = false)
		{
			SecurityVerifier.AssertHasRights(() => GetSecurityObject(controllerData).CanView);
			SetHasVerifiedRequestSecurity();

            if (!loadData)
                return Json(new byte[0], JsonRequestBehavior.AllowGet);

            var model = (ResourceManagementModel)CreatePanelModel(controllerData);

            decimal lcStageId = GetLcStageIdFromRequest();

            if (lcStageId > 0)
                model.LcStageId = lcStageId;

            var data = model.BuildBusinessCostsGridList();

			var helper = new NavigatorPanelHtmlGridHelper(controllerData, GridNames.BUSINESSCOSTS, null, model.SecurityObject);

			int totalPages = data.Count > 0 ? 1 : 0;
			int totalRecords = data.Count;

			var jsonData = new JsonGridData
			{
				total = totalPages,
				page = clientGridRequest.pageIndex,
				records = totalRecords,
				rows = helper.ConvertToHtmlGridArray(data)
			};

			return Json(jsonData, JsonRequestBehavior.AllowGet);
		}

		[ControllerEntryPoint]
		[AcceptVerbs(HttpVerbs.Get | HttpVerbs.Post)]
		[AntiCSRFToken(AntiCSRFTokenVerifyMode.Auto)]
		public virtual JsonResult GetDestinationsTree(NavigatorPanelControllerData<DxAssessment> controllerData, GridSettings clientGridRequest, bool loadData = false)
		{
			SecurityVerifier.AssertHasRights(() => GetSecurityObject(controllerData).CanView);
			SetHasVerifiedRequestSecurity();

            if (!loadData)
                return Json(new byte[0], JsonRequestBehavior.AllowGet);

            var model = (ResourceManagementModel)CreatePanelModel(controllerData);

            decimal lcStageId = GetLcStageIdFromRequest();

            if (lcStageId > 0)
                model.LcStageId = lcStageId;

            var data = model.BuildDestinationsGridList();

			var helper = new NavigatorPanelHtmlGridHelper(controllerData, GridNames.DESTINATIONS, null, model.SecurityObject);

			int totalPages = data.Count > 0 ? 1 : 0;
			int totalRecords = data.Count;

			var jsonData = new JsonGridData
			{
				total = totalPages,
				page = clientGridRequest.pageIndex,
				records = totalRecords,
				rows = helper.ConvertToHtmlGridTreeArray(data)
			};

			return Json(jsonData, JsonRequestBehavior.AllowGet);
		}

		[ControllerEntryPoint]
		[AcceptVerbs(HttpVerbs.Get | HttpVerbs.Post)]
		[AntiCSRFToken(AntiCSRFTokenVerifyMode.Auto)]
		public virtual JsonResult GetNonFoodDestinationsTree(NavigatorPanelControllerData<DxAssessment> controllerData, GridSettings clientGridRequest, bool loadData = false)
		{
			SecurityVerifier.AssertHasRights(() => GetSecurityObject(controllerData).CanView);
			SetHasVerifiedRequestSecurity();

            if (!loadData)
                return Json(new byte[0], JsonRequestBehavior.AllowGet);

            var model = (ResourceManagementModel)CreatePanelModel(controllerData);

            decimal lcStageId = GetLcStageIdFromRequest();

            if (lcStageId > 0)
                model.LcStageId = lcStageId;

            var data = model.BuildNonFoodDestinationsGridList();

			var helper = new NavigatorPanelHtmlGridHelper(controllerData, GridNames.NON_FOOD_DESTINATIONS, null, model.SecurityObject);

			int totalPages = data.Count > 0 ? 1 : 0;
			int totalRecords = data.Count;

			var jsonData = new JsonGridData
			{
				total = totalPages,
				page = clientGridRequest.pageIndex,
				records = totalRecords,
				rows = helper.ConvertToHtmlGridTreeArray(data)
			};

			return Json(jsonData, JsonRequestBehavior.AllowGet);
		}

		[HttpPost]
		[AntiCSRFToken(AntiCSRFTokenVerifyMode.Verify)]
		public virtual DataAjaxResult SaveBusinessCostRow(NavigatorPanelControllerData<DxAssessment> controllerData, BusinessCostModel businessCostRow)
		{
			SecurityVerifier.AssertHasRights(() => GetSecurityObject(controllerData).CanEditOrDelete);
			SetHasVerifiedRequestSecurity();

			bool result;

			DxBusinessCost dxBusinessCost = businessCostRow.ToDxObject();

			if (businessCostRow.CreateNew)
			{
				dxBusinessCost.AssessmentCode = controllerData.TargetObject.Code;
                dxBusinessCost.SortOrder = DxBusinessCostCollection.GetNextSortOrder(controllerData.TargetObject.Code, businessCostRow.LcStageId);
				result = dxBusinessCost.Create();
			}
			else
			{
				result = dxBusinessCost.Update();
			}

			return new DataAjaxResult(new { Success = result });
		}

		[HttpPost]
		[AntiCSRFToken(AntiCSRFTokenVerifyMode.Verify)]
		public virtual DataAjaxResult DeleteRow(NavigatorPanelControllerData<DxAssessment> controllerData, string rowIdentifiableString, ResourceRowType rowType)
		{
			SecurityVerifier.AssertHasRights(() => GetSecurityObject(controllerData).CanEditOrDelete);
			SetHasVerifiedRequestSecurity();

			bool actionResult = false;

			if (rowType == ResourceRowType.Input)
			{
				DxInput inputToDelete = DxObject.ParseIdentifiableString<DxInput>(rowIdentifiableString);
				inputToDelete.LoadEntity();
				
				decimal lcStageId = inputToDelete.AssessmentLcStageId;

				inputToDelete.InputCategory.LoadEntity();
				string inputCategoryType = inputToDelete.InputCategory.Type;

                actionResult = inputToDelete.Exists() && inputToDelete.Delete();

				inputToDelete.ManageOutputWastewater();
			}
			else if (rowType == ResourceRowType.Destination)
			{
				actionResult = DeleteGridRow<DxAssessmentDestination>(rowIdentifiableString);
			}
			else if (rowType == ResourceRowType.BusinessCost)
			{
                var gridItem = DxObject.ParseIdentifiableString<DxBusinessCostGridItem>(rowIdentifiableString);
                var businessCost = new DxBusinessCost(gridItem.Id, true);
				
				if (businessCost.Title == BusinessCostGridTabModel.OTHER)
				{
					var notes = new NotesModel(GetControllerUrl(), controllerData, ResourceTabIds.BUSINESS_COST_OTHER, businessCost.AssessmentLcStageId, businessCost.AssessmentCode);
					notes.DeleteBusinessCostOtherNotes();
				}
				actionResult = DeleteGridRow<DxBusinessCost>(businessCost.IdentifiableString);
			}

			return new DataAjaxResult(new { Success = actionResult });
		}

		[HttpPost]
		[AntiCSRFToken(AntiCSRFTokenVerifyMode.Verify)]
		public virtual DataAjaxResult SaveInputRow(NavigatorPanelControllerData<DxAssessment> controllerData, InputModel inputRow)
		{
			SecurityVerifier.AssertHasRights(() => GetSecurityObject(controllerData).CanEditOrDelete);
			SetHasVerifiedRequestSecurity();

			var actionResult = new DataAjaxResult();
			bool success;
			DxInput dxInput = inputRow.ToDxObject();

			using (var uow = DxUnitOfWork.New())
			{
				bool CreateInputProductCoProductSpread(decimal inputId, string[] destinationCodes)
				{
					var inputProductCoProductSpread = new DxInputProductCoProductSpreadCollection();

					foreach (string destinationCode in destinationCodes)
						inputProductCoProductSpread.AddItem(new DxInputProductCoProductSpread(inputId, destinationCode));

					return inputProductCoProductSpread.Create();
				};

				if (inputRow.CreateNew)
				{
					dxInput.AssessmentCode = controllerData.TargetObject.Code;
					success = dxInput.Create();
				}
				else
				{
					DxInputDestinationCollection nonWasteDestinations = new DxInputDestinationCollection(DxInputDestinationCollection.Filter.InputIdAndNonWasteDestination, dxInput.Id);
					nonWasteDestinations.Load();
					var existingNonWasteInputDestionaCodes = nonWasteDestinations.Select(inputDest => inputDest.DestinationCode);

					if (existingNonWasteInputDestionaCodes.Except(inputRow.PartOfProductCoproduct).Count() > 0)
					{
						var destinationTypeName = Locale.GetString(ResourceFiles.AssessmentManager, inputRow.CategoryType == InputCategoryType.FOOD ? "ResMngmt_TabDestinations" : "ResMngmt_TabNonFoodDestinations");
						var msg = string.Format(Locale.GetString(ResourceFiles.AssessmentManager, "Inputs_PartOfProductChange"), destinationTypeName.ToLower());
						actionResult.SetDataValue("DestinationDeletedMsg", msg);
					}

					success = dxInput
						.SetCurrentNonWasteDestinationCodes(inputRow.PartOfProductCoproduct, GridHelpers.Instance.NonWasteDestinationCodes, GridHelpers.Instance.ProductCoProductDestionCodes)
						.Update();

					var inputProductCoProductSpread = new DxInputProductCoProductSpreadCollection(new decimal[] { dxInput.Id }, true);
					inputProductCoProductSpread.Delete();

					if (inputRow.IsWaterMaterialInterchanged() || inputRow.AddedIneibleParts())
						dxInput.ManageOutputWastewater();
				}

				if (dxInput.PartOfProductCoproduct)
				{
					success &= CreateInputProductCoProductSpread(dxInput.Id, inputRow.PartOfProductCoproduct);
				}
				
				var errorMessage = string.Empty;
				if (success)
				{
					if (OutputValuesPerTonneAreValid(dxInput.AssessmentCode, dxInput.AssessmentLcStageId))
					{
						uow.Commit();
					}
					else
					{
						success = false;
						errorMessage = Locale.GetString(ResourceFiles.AssessmentManager, "Output_grid_NonProductsBiggerThanProductValuePerTonne");
						uow.Abort();
					}
				}
				else
				{
					errorMessage = Locale.GetString(ResourceFiles.AssessmentManager, "Inputs_SaveError");
					uow.Abort();
				}

				if (!string.IsNullOrEmpty(errorMessage))
					actionResult.SetDataValue("Message", errorMessage);
			}

			return actionResult
				.SetDataValue("Success", success)
				.SetDataValue("Entity", CreateInputResponseModel(dxInput, inputRow.CategoryType, controllerData.TargetObject));
		}
		
		[HttpPost]
		[AntiCSRFToken(AntiCSRFTokenVerifyMode.Verify)]
		public virtual DataAjaxResult CreateNewMaterial(NavigatorPanelControllerData<DxAssessment> controllerData, string materialDescription, string categoryType)
		{
			SecurityVerifier.AssertHasRights(() => GetSecurityObject(controllerData).CanCreateMaterial);
			SetHasVerifiedRequestSecurity();

			bool actionResult = true;
			string identifiableString = string.Empty;

			DxMaterialType materialType = new DxMaterialType(Constants.MaterialType.DIR_RESOURCE);
			string matCode = DxMaterial.GetAutoNumberingKey(materialType.Code, DxPlant.NONE, DxUser.CurrentUser);
			
			using (DxUnitOfWork unitOfWork = DxUnitOfWork.New())
			{
				DxMaterial material = new DxMaterial(DxPlant.NONE, matCode)
				{
					Description = materialDescription,
					MaterialType = materialType,
					CreatedBy = DxUser.CurrentUser.LoginName,
					CreateDate = DateTime.Now,
					Status = "00",
					Flags = decimal.Zero
				};

				actionResult &= material.Create();

				DxAttribute attribute = material.GetOrLoadAttribute("DXDIR_RESOURCE_TYPE");
				DxAttributeValue newValue = attribute.NewValue();
				newValue.Data = categoryType;

				attribute.AddValue(newValue);
				actionResult &= attribute.Create();

				if (DxUser.CurrentUser.IsExternal)
				{
					DxPartnerMaterial partnerMaterial = new DxPartnerMaterial(DxPlant.NONE, matCode, DxUser.CurrentUser.PartnerOrganizationCode)
					{
						IsShared = false
					};

					actionResult &= partnerMaterial.Create();
				}

				if (actionResult)
				{
					identifiableString = material.IdentifiableString;
					unitOfWork.Commit();
				}
				else
				{
					unitOfWork.Abort();
				}
			}

			return new DataAjaxResult(new { Success = actionResult, IdentifiableString = identifiableString});
		}

		[HttpPost]
		[AntiCSRFToken(AntiCSRFTokenVerifyMode.Verify)]
		public virtual DataAjaxResult SaveDestinationRow(NavigatorPanelControllerData<DxAssessment> controllerData, DestinationModel destRow, bool isFood)
		{
			SecurityVerifier.AssertHasRights(() => GetSecurityObject(controllerData).CanEditOrDelete);
			SetHasVerifiedRequestSecurity();

			DestinationRowItemModel model = new DestinationRowItemModel(controllerData.TargetObject, destRow, isFood);

			bool updated = true;
			string errorMessage = string.Empty;

			using (DxUnitOfWork unitOfWork = DxUnitOfWork.New())
			{
				updated &= model.DeleteInputDestinations();
				updated &= model.InsertInputDestinations();

				updated &= model.UpdateWasteDestinations();

				if (updated)
				{
					// Updates with stored procedures
					model.InsertOrUpdateNonWasteColumns();
					model.ManageOutputWasteWater();

					if (OutputValuesPerTonneAreValid(controllerData.TargetObject.Code, destRow.LcStageId))
					{
						unitOfWork.Commit();
					}
					else
					{
						updated = false;
						errorMessage = Locale.GetString(ResourceFiles.AssessmentManager, "Output_grid_NonProductsBiggerThanProductValuePerTonne");
						unitOfWork.Abort();
					}
				}
				else
				{
					errorMessage = Locale.GetString(ResourceFiles.AssessmentManager, "DestinationsGrid_ErrorMessage");
					unitOfWork.Abort();
				}
			}

			return new DataAjaxResult(new { Success = updated, ErrorMessage = errorMessage });
		}

        [HttpPost]
        [AntiCSRFToken(AntiCSRFTokenVerifyMode.Verify)]
        public virtual DataAjaxResult SaveOutputRow(NavigatorPanelControllerData<DxAssessment> controllerData, OutputModel outputRow)
        {
            SecurityVerifier.AssertHasRights(() => GetSecurityObject(controllerData).CanEditOrDelete);
            SetHasVerifiedRequestSecurity();

            var (result, message) = ValidateAndSaveOutputRow(outputRow.ToDxObject());
            return new DataAjaxResult(new { Success =  result, ErrorMessage = message});
		}

        [HttpPost]
        [AntiCSRFToken(AntiCSRFTokenVerifyMode.Verify)]
        public virtual DataAjaxResult GetAvailableBusinessCostDropdownItems(NavigatorPanelControllerData<DxAssessment> controllerData, decimal lcStage)
        {
            SecurityVerifier.AssertHasRights(() => GetSecurityObject(controllerData).CanView);
            SetHasVerifiedRequestSecurity();

            BusinessCostGridTabModel model = new BusinessCostGridTabModel(GetControllerUrl(), controllerData, lcStage);

            return new DataAjaxResult(new
            {
                BusinessCostsJSON = model.GetBusinessCostsList().ConvertToJSONExchangeableObject()
            });
        }

		/// <summary>
		/// Invokes Destination managment dialog
		/// </summary>
		[HttpPost]
		[AntiCSRFToken(AntiCSRFTokenVerifyMode.Verify)]
		public virtual DataAjaxResult ManageAssessmentRaiseDialog(NavigatorPanelControllerData<DxAssessment> controllerData, EntityType entityType, decimal lcStageId)
		{
			SecurityVerifier.AssertHasRights(() => GetSecurityObject(controllerData).CanEditOrDelete);
			SetHasVerifiedRequestSecurity();

			string assessmentIdent = controllerData.TargetObject.IdentifiableString;
			return ManageAssessmentUtility.CreateManageDlgActivityActionResult(Url, controllerData, entityType, assessmentIdent, lcStageId);
		}

		#region Inputs Grid Actions

		[HttpPost]
		[AntiCSRFToken(AntiCSRFTokenVerifyMode.Verify)]
		public virtual DataAjaxResult CreateNewInputCategory(NavigatorPanelControllerData<DxAssessment> controllerData, string title, string typeCode)
		{
			SecurityVerifier.AssertHasRights(() => GetSecurityObject(controllerData).CanEditOrDelete);
			SetHasVerifiedRequestSecurity();

			var newInputCategory = new DxInputCategory()
			{
				AssessmentCode = controllerData.TargetObject.Code,
				Title = title,
				Type = !string.IsNullOrEmpty(typeCode) ? typeCode : InputCategoryType.NONFOOD // default to NONFOOD (from Phrase (4, 99510))
			};

			if (newInputCategory.Create())
				return new DataAjaxResult(new { Success = true, InputCategoryIdentifiable = newInputCategory.IdentifiableString });
			else
				return new DataAjaxResult(new { Success = false });
		}

		[HttpPost]
		[AntiCSRFToken(AntiCSRFTokenVerifyMode.Verify)]
		public virtual DataAjaxResult DeleteInputs(NavigatorPanelControllerData<DxAssessment> controllerData, List<string> inputsIdentStrings)
		{
			SecurityVerifier.AssertHasRights(() => GetSecurityObject(controllerData).CanEditOrDelete);
			SetHasVerifiedRequestSecurity();

			IEnumerable<DxInput> prasedInputs = inputsIdentStrings.Select(identStrings => DxInput.ParseIdentifiableString<DxInput>(identStrings));

			DxInputCollection inputs = new DxInputCollection();
			inputs.AddRange(prasedInputs);
			inputs.LoadItems();

			var result = inputs.Delete();

			return new DataAjaxResult(new { Success = result });
		}

		[HttpPost]
		[AntiCSRFToken(AntiCSRFTokenVerifyMode.Verify)]
		public virtual JsonResult ReloadInputCategories(NavigatorPanelControllerData<DxAssessment> controllerData, decimal lcStage)
		{
			SecurityVerifier.AssertHasRights(() => GetSecurityObject(controllerData).CanView);
			SetHasVerifiedRequestSecurity();

			InputsGridTabModel model = new InputsGridTabModel(GetControllerUrl(), controllerData, lcStage);

			return Json(new
			{
				InputCategoryTypes = model.GetInputCategories().ConvertToJSONExchangeableObject()
			});
		}

		[HttpPost]
		[AntiCSRFToken(AntiCSRFTokenVerifyMode.Verify)]
		public virtual DataAjaxResult GetInputRow(NavigatorPanelControllerData<DxAssessment> controllerData, string inputIdentifiableString, string inputCategoryType)
		{
			SecurityVerifier.AssertHasRights(() => GetSecurityObject(controllerData).CanEditOrDelete);
			SetHasVerifiedRequestSecurity();

			var dxInput = DxInput.ParseIdentifiableString<DxInput>(inputIdentifiableString);
			bool result = dxInput.Load();

			return new DataAjaxResult(new
			{
				Success = result,
				Entity = CreateInputResponseModel(dxInput, inputCategoryType, controllerData.TargetObject)
			});
		}

		[HttpPost]
		[AntiCSRFToken(AntiCSRFTokenVerifyMode.Verify)]
		public virtual DataAjaxResult SaveResourceNote(NavigatorPanelControllerData<DxAssessment> controllerData, string resourceType, decimal lcStageId, string notes, string assessmentCode, string businessCostOther = "")
		{
			SecurityVerifier.AssertHasRights(() => GetSecurityObject(controllerData).CanEditOrDelete);
			SetHasVerifiedRequestSecurity();

			var model = new NotesModel(GetControllerUrl(), controllerData, resourceType, lcStageId, assessmentCode);
			var message = Locale.GetString(ResourceFiles.AssessmentManager, "ResMngmt_NotesSaveFailed");

			model.SaveNotes(notes, out message);

			if (resourceType == ResourceTabIds.BUSINESS_COST && model.DoesHaveOtherBusinessCost()) // If resource note is null or empty it shouldnt be saved.
			{
				model.ResourceType = ResourceTabIds.BUSINESS_COST_OTHER;
				model.SaveNotes(businessCostOther, out message);
			}

			return new DataAjaxResult().SetDataValue("message", message);
		}

		[HttpPost]
		[AntiCSRFToken(AntiCSRFTokenVerifyMode.Verify)]
		public virtual DataAjaxResult GetNotes(NavigatorPanelControllerData<DxAssessment> controllerData, string resourceType, decimal lcStageId, string assessmentCode)
		{
			SecurityVerifier.AssertHasRights(() => GetSecurityObject(controllerData).CanEditOrDelete);
			SetHasVerifiedRequestSecurity();

			var model = new NotesModel(GetControllerUrl(), controllerData, resourceType, lcStageId, assessmentCode);
			var businessCostOther = string.Empty;
			bool hasBusinessCostOther = false;

			var notes = model.GetNotesByResourceType(resourceType);

			if (resourceType == ResourceTabIds.BUSINESS_COST)
			{
				businessCostOther = model.GetNotesByResourceType(ResourceTabIds.BUSINESS_COST_OTHER);
				hasBusinessCostOther = model.DoesHaveOtherBusinessCost();
			}
			
			return new DataAjaxResult(new { Notes = notes, BusinessCostOther = businessCostOther, HasBusinessCostOther = hasBusinessCostOther });

		}

        [HttpPost]
        [AntiCSRFToken(AntiCSRFTokenVerifyMode.Verify)]
        public virtual DataAjaxResult Export(NavigatorPanelControllerData<DxAssessment> controllerData, decimal lcStage)
        {
            SecurityVerifier.AssertHasRights(() => GetSecurityObject(controllerData).CanView);
            SetHasVerifiedRequestSecurity();

            var model = (ResourceManagementModel)CreatePanelModel(controllerData);
            model.LcStageId = lcStage;

            var inputs = model.BuildInputsGridList();
            var foodDestinations = model.BuildDestinationsGridList();
            var nonFoodDestinations = model.BuildNonFoodDestinationsGridList();
            var outputs = model.BuildOutputsGridList();
            var businessCosts = model.BuildBusinessCostsGridList();

            DxAssessmentLcStage stage = new DxAssessmentLcStage(lcStage);

            if (stage.PersistenceStatus == DxPersistenceStatus.Unknown)
                stage.Load();

            string fileName = $"{controllerData.TargetObject.Description} ({controllerData.TargetObject.Code})_{stage.Title}_ResManagement_{Guid.NewGuid()}.xlsx";

            ExcelExportScriptGenerator exportScriptGenerator = new ExcelExportScriptGenerator(controllerData, fileName);

            exportScriptGenerator
                .AddData(GridNames.INPUTS, inputs, Locale.GetString(ResourceFiles.AssessmentManager, "ResMngmt_TabInputs"))
                .AddData(GridNames.DESTINATIONS, foodDestinations, Locale.GetString(ResourceFiles.AssessmentManager, "ResMngmt_TabDestinations"))
                .AddData(GridNames.NON_FOOD_DESTINATIONS, nonFoodDestinations, Locale.GetString(ResourceFiles.AssessmentManager, "ResMngmt_TabNonFoodDestinations"))
                .AddData(GridNames.OUTPUTS, outputs, Locale.GetString(ResourceFiles.AssessmentManager, "ResMngmt_TabOutputs"))
                .AddData(GridNames.BUSINESSCOSTS, businessCosts, Locale.GetString(ResourceFiles.AssessmentManager, "ResMngmt_TabBusinessCosts"));

            try
            {
                return new DataAjaxResult
                {
                    Data = new { Status = true, ScriptToExecute = exportScriptGenerator.GetExecutingScript() }
                };
            }
            catch (Exception ex)
            {
                return new DataAjaxResult
                {
                    Data = new { Status = false, ErrorMessage = ex.Message }
                };
            }
        }

		#endregion

		private bool OutputValuesPerTonneAreValid(string assessmentCode, decimal assessmentLcStageId)
		{
			var outputValidationData = GetOutputDataForValidation(assessmentCode, assessmentLcStageId);

			return outputValidationData.OutputCollection.Count == 0 ||
				   outputValidationData.OutputCollection.All(x => x.Income == null) ||
				   outputValidationData.Product1ValuePerTonne > outputValidationData.MaxNonProductValuePerTonne;
		}

		private OutputValidationData GetOutputDataForValidation(string assessmentCode, decimal assessmentLcStageId)
		{
			DxOutputCollection outputsCollection = new DxOutputCollection(DxOutputCollection.Filter.AssessmentCodeAndAssessmentLcStageId, assessmentCode, assessmentLcStageId);
			outputsCollection.Load();

			var groupedValuesPerTonne =
				outputsCollection
				.GroupBy(output => new { output.DestinationCode, output.OutputCategoryId })
				.Where(group => group.Sum(o => o.Weight) != 0)
				.Select(group => new OutputGroupedData
				{
					DestinationCode = group.Key.DestinationCode,
					OutputCategoryId = group.Key.OutputCategoryId,
					Income = group.Sum(o => o.Income),
					Weight = group.Sum(o => o.Weight),
					ValuePerTonne = group.Sum(o => o.Income) / group.Sum(o => o.Weight),
				}).ToList();

			decimal product1ValuePerTonne = groupedValuesPerTonne
				.FirstOrDefault(x => x.DestinationCode == PRODUCT_1)?.ValuePerTonne ?? 0;

			decimal maxNonProductValuePerTonne = groupedValuesPerTonne
				.Where(x => IsNonProduct(x.DestinationCode))
				.Max(x => x.ValuePerTonne) ?? 0;

			return new OutputValidationData()
			{
				OutputCollection = outputsCollection,
				Product1ValuePerTonne = product1ValuePerTonne,
				MaxNonProductValuePerTonne = maxNonProductValuePerTonne,
				GroupedValuesPerTonne = groupedValuesPerTonne
			};
		}

        private (bool res, string msg) ValidateAndSaveOutputRow(DxOutput output)
        {
            bool result = true;
            string message = string.Empty;
            // Validation logic           

			var outputDataForValidation = GetOutputDataForValidation(output.AssessmentCode, output.AssessmentLcStageId);

			decimal maximalNonProductValuePerTonne = outputDataForValidation.MaxNonProductValuePerTonne;
			decimal product1ValuePerTonne = outputDataForValidation.Product1ValuePerTonne;
			DxOutputCollection loadedOutputs = outputDataForValidation.OutputCollection;

			decimal editedRowIncome = 0m;
            decimal editedRowWeight = 0m;
			

            if (output.Income != null)
                editedRowIncome = (decimal)output.Income;

			editedRowWeight = outputDataForValidation.GroupedValuesPerTonne
				.FirstOrDefault(x => x.DestinationCode == output.DestinationCode && x.OutputCategoryId == output.OutputCategoryId)?.Weight ?? 0;
			            
            if (IsNonProduct(output.DestinationCode) && editedRowIncome / editedRowWeight >= product1ValuePerTonne)
            {
                result = false;
                message = Locale.GetString(ResourceFiles.AssessmentManager, "Output_grid_NonProductsBiggerThanProductValuePerTonne");
            }
            else if (output.DestinationCode == PRODUCT_1 && editedRowIncome / editedRowWeight <= maximalNonProductValuePerTonne)
            {
                result = false;
                message = Locale.GetString(ResourceFiles.AssessmentManager, "Output_grid_ProductLesserThanNonProductValuePerTonne");
            }
            else
            {
                result = output.Update();
                if (!result)
                    message = Locale.GetString(ResourceFiles.AssessmentManager, "Output_grid_ErrorDuringOutPutRowUpdate");
            }

            return (result, message) ;
        }

		private bool IsNonProduct(string destinationCode)
		{
			return !ProductCodes.Contains(destinationCode);
		}

		private sealed class OutputValidationData
		{
			public DxOutputCollection OutputCollection { get; set; }
			public decimal Product1ValuePerTonne { get; set; } 
			public decimal MaxNonProductValuePerTonne { get; set; }
			public List<OutputGroupedData> GroupedValuesPerTonne { get; set; }
		}

		private sealed class OutputGroupedData
		{
			public string DestinationCode { get; set; }
			public decimal OutputCategoryId { get; set; }
			public decimal? Income { get; set; }
			public decimal? Weight { get; set; }
			public decimal? ValuePerTonne { get; set; }
		}

        private sealed class JsonGridData
        {
            public int total { get; set; }
            public int page { get; set; }
            public int records { get; set; }
            public Array rows { get; set; }
        }
	}
}