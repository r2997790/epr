using Selerant.DevEx.BusinessLayer.Authorization;
using Selerant.DevEx.CommonComponents;
using Selerant.DevEx.Modules.DIRModule.Configuration.Resources;
using Selerant.DevEx.Modules.DIRModule.ModuleBlocks._ThisProject;
using Selerant.DevEx.Modules.DIRModule.WebMvcFrontEnd.Areas.DIRAdminTools.Models;
using Selerant.DevEx.Modules.DIRModule.WebMvcFrontEnd.Areas.DIRAdminTools.Security;
using Selerant.DevEx.Web;
using Selerant.DevEx.Web.Security;
using Selerant.DevEx.WebControls;
using Selerant.DevEx.WebMvcModules.Areas.AdminTools.Controllers;
using Selerant.DevEx.WebMvcModules.Areas.AdminTools.Models;
using Selerant.DevEx.WebMvcModules.Helpers.jqGrid;
using Selerant.DevEx.WebMvcModules.Infrastructure.Base;
using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace Selerant.DevEx.Modules.DIRModule.WebMvcFrontEnd.Areas.DIRAdminTools.Controllers
{
    [ComponentDescriptor(ThisComponentIdentifier,
         SecurityObjectType = typeof(AdminToolsAssessmentTypesSecurity),
         VerifyControllerData = true,
         VerifyRequestSecurity = true)]
    public partial class AssessmentTypesController : AdminToolController<AdminToolsAssessmentTypesSecurity>
    {
		#region Contants

		[LogicComponentIdentifier]
		public const string ThisComponentIdentifier = DIRModuleInfo.MODULE_CODE + "_AdminTools_AssessmentTypes";

        #endregion

        #region Index

        [ControllerEntryPoint]
        [Web.Security.AntiCSRFToken(Web.Security.AntiCSRFTokenVerifyMode.Ignore)]
        public virtual System.Web.Mvc.ActionResult Index()
        {
            return Index(DIRModuleInfo.Instance, MVC_DIR.DIRAdminTools.AssessmentTypes.Views.Index, new AssessmentTypesIndexModel(GetControllerUrl(),
                new AdminToolControllerData(ComponentDescriptorHelper.GetIdentifierForType(GetType()))));
        }

        #endregion

        #region Actions

        [HttpPost]
        [AntiCSRFToken(AntiCSRFTokenVerifyMode.Verify)]
        public virtual DataAjaxResult AddAssessmentTypeClick(AdminToolControllerData controllerData, string identifiableString)
        {
            SecurityVerifier.AssertHasRights(() => GetSecurityObject(controllerData).CanCreate);
            SetHasVerifiedRequestSecurity();

            var caption = new TextDescriptor(Locale.GetString(ResourceFiles.AssessmentManager, "ATAddAssessmentTypeTabHeader"));
            var returnValue = new Dictionary<string, object>();

            JSOpenDialogActivity openDialogActivity = new JSOpenDialogActivity()
            {
                Url = ComponentDataHelper.AddDataToUrl(Url.Action(MVC_DIR.DIRAdminTools.AssessmentTypes.AddAssessmentTypeDialogIndex(null)), controllerData),
                CaptionDescriptor = caption,
                Width = 800,
                Height = 600
            }; 

			returnValue["activity"] = openDialogActivity;
			returnValue["message"] = Locale.GetString(ResourceFiles.AssessmentManager, "ATAddAssessmentTypeMessage");


			return new DataAjaxResult(returnValue);
        }

        [HttpGet]
        [AntiCSRFToken(AntiCSRFTokenVerifyMode.Ignore)]
        public virtual System.Web.Mvc.ActionResult AddAssessmentTypeDialogIndex(AdminToolControllerData controllerData)
        {
            SecurityVerifier.AssertHasRights(() => GetSecurityObject(controllerData).CanCreate);

            SetHasVerifiedRequestSecurity();

            var model = new EditAssessmentTypeModel(GetControllerUrl(), controllerData);

            model.PrepareData(String.Empty);
            model.IsEditMode = true;

            return View(DIRModuleInfo.Instance, MVC_DIR.DIRAdminTools.AssessmentTypes.Views.EditAssessmentTypeDialogIndex,
                new AssessmentTypeDialogModel(GetControllerUrl(),
                new AdminToolControllerData(ComponentDescriptorHelper.GetIdentifierForType(GetType())))
                {
                    IsEditMode = true,
                    PropertiesModel = model
                });
        }

        [HttpGet]
        [AntiCSRFToken(AntiCSRFTokenVerifyMode.Ignore)]
        public virtual System.Web.Mvc.ActionResult OpenEditAssessmentTypeDialogIndex(AdminToolControllerData controllerData, string identifiableString, bool isEditMode)
        {
            if (isEditMode)
                SecurityVerifier.AssertHasRights(() => GetSecurityObject(controllerData).CanEdit);
            else
                SecurityVerifier.AssertHasRights(() => GetSecurityObject(controllerData).CanShowContent);

            SetHasVerifiedRequestSecurity();

            var model = new EditAssessmentTypeModel(GetControllerUrl(), controllerData);

            model.PrepareData(identifiableString);
            model.IsEditMode = isEditMode;

            return View(DIRModuleInfo.Instance, MVC_DIR.DIRAdminTools.AssessmentTypes.Views.EditAssessmentTypeDialogIndex,
                new AssessmentTypeDialogModel(GetControllerUrl(),
                new AdminToolControllerData(ComponentDescriptorHelper.GetIdentifierForType(GetType())))
                {
                    IsEditMode = true,
                    PropertiesModel = model
                });
        }

        [HttpPost]
        [AntiCSRFToken(AntiCSRFTokenVerifyMode.Verify)]
        public virtual DataAjaxResult SaveAssessmentType(AdminToolControllerData controllerData, EditAssessmentTypeModel.CreateNewAssessmentTypeDialogFormData formData)
        {
            SecurityVerifier.AssertHasRights(() => GetSecurityObject(controllerData).CanCreateOrUpdate);
            SetHasVerifiedRequestSecurity();

            var model = new EditAssessmentTypeModel(GetControllerUrl(), controllerData);
            string message = String.Empty;

            //var returnValue = new Dictionary<string, object>();
            //var invalidControls = new List<string>();
            //var errorMessages = model.ValidateData(formData, out invalidControls);
            //var assmTypeIdentifiableString = model.
            //var dxAssesmentType = (DxAssessmentType)DxObject.ParseIdentifiableString(assmTypeIdentifiableString);

            if (ModelState.IsValid)
                model.SaveAssessmentType(formData, out message);

            return new DataAjaxResult(new Dictionary<string, object>
                {
                    { "message", message }
                });            
        }

        [HttpPost]
        [AntiCSRFToken(AntiCSRFTokenVerifyMode.Verify)]
        public virtual DataAjaxResult ActivateAssessmentType(AdminToolControllerData controllerData, string identifiableString)
        {
            SecurityVerifier.AssertHasRights(() => GetSecurityObject(controllerData).CanEdit);
            SetHasVerifiedRequestSecurity();

            var model = new EditAssessmentTypeModel(GetControllerUrl(), controllerData);
            string message = Locale.GetString("DIR_AssessmentManager", "ATAssessmentTypeActivationFailed");

            if (ModelState.IsValid)
                model.ActivateAssessmentType(identifiableString, out message);

            return new DataAjaxResult(new Dictionary<string, object>
                {
                    { "message", message }
                });
        }

        [HttpPost]
        [AntiCSRFToken(AntiCSRFTokenVerifyMode.Verify)]
        public virtual DataAjaxResult EditAssessmentType(AdminToolControllerData controllerData, string identifiableString)
        {
            SecurityVerifier.AssertHasRights(() => GetSecurityObject(controllerData).CanEdit);
            SetHasVerifiedRequestSecurity();

            var model = new EditAssessmentTypeModel(GetControllerUrl(), controllerData);
            var caption = new TextDescriptor(Locale.GetString("DIR_AssessmentManager", "ATEditAssessmentTypeTabHeader"));
            var returnValue = new Dictionary<string, object>();

            string url = Utilities.MapUrlPath(Url.Action(MVC_DIR.DIRAdminTools.AssessmentTypes.OpenEditAssessmentTypeDialogIndex(null, identifiableString, true)));
            url = ComponentDataHelper.AddDataToUrl(url, controllerData);

            JSOpenDialogActivity openDialogActivity = new JSOpenDialogActivity()
            {
                Url = url,
                CaptionDescriptor = caption,
                Width = 800,
                Height = 600
            };
            returnValue["activity"] = openDialogActivity;
            returnValue["message"] = Locale.GetString("DIR_AssessmentManager", "ATAssessmentTypeEditSuccess");

            return new DataAjaxResult(returnValue);
        }

        [HttpPost]
        [AntiCSRFToken(AntiCSRFTokenVerifyMode.Verify)]
        public virtual DataAjaxResult DeleteAssessmentType(AdminToolControllerData controllerData, string identifiableString)
        {
            SecurityVerifier.AssertHasRights(() => GetSecurityObject(controllerData).CanDelete);
            SetHasVerifiedRequestSecurity();

            var model = new EditAssessmentTypeModel(GetControllerUrl(), controllerData);
            string message = Locale.GetString("DIR_AssessmentManager", "ATAssessmentTypeDeletionFailed");

            if (ModelState.IsValid)
                model.DeleteAssessmentType(identifiableString, out message);

            return new DataAjaxResult(new Dictionary<string, object>
                {
                    { "message", message }
                });
        }

        #endregion

        #region Grid Methods

        protected override GetGridDataResult GetGridData(AdminToolControllerData controllerData, GridSettings clientGridRequest, String nodeId)
        {
            var model = new AssessmentTypesIndexModel(GetControllerUrl(), controllerData);
            int totalRecords;
            string gridId;
            var dataRows = model.GetGridData(clientGridRequest, out totalRecords, out gridId);

            var result = new GetGridDataResult(gridId, controllerData.SecurityObject);
            result.TotalRecords = totalRecords;
            result.Items = dataRows;
            return result;
        }

        #endregion
    }
}