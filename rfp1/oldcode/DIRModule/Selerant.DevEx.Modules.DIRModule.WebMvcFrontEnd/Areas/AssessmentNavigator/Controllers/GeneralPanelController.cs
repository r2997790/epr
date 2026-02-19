using System.Collections.Generic;
using System.Web.Mvc;
using Selerant.DevEx.Web;
using Selerant.DevEx.Modules.DIRModule.ModuleBlocks._ThisProject;
using Selerant.DevEx.WebMvcModules.Infrastructure.Base;
using Selerant.DevEx.WebMvcModules.Infrastructure.Navigators.Controllers;
using Selerant.DevEx.WebMvcModules.Infrastructure.Navigators.Helpers;
using Selerant.DevEx.WebMvcModules.Infrastructure.Navigators.Models;
using Selerant.DevEx.Modules.DIRModule.BackEnd.AssessmentManagement.BusinessObjects;
using Selerant.DevEx.Modules.DIRModule.WebMvcFrontEnd._ThisProject;
using Selerant.DevEx.Modules.DIRModule.WebMvcFrontEnd.Areas.AssessmentNavigator.Models.Panels.GeneralPanel;
using Selerant.DevEx.CommonComponents;
using Selerant.DevEx.BusinessLayer;
using Selerant.DevEx.Modules.DIRModule.WebMvcFrontEnd.Areas.AssessmentNavigator.Security;
using Selerant.DevEx.Web.Security;
using Selerant.DevEx.BusinessLayer.Authorization;
using Selerant.DevEx.Modules.DIRModule.WebMvcFrontEnd.Helpers;
using System;
using Selerant.DevEx.Modules.DIRModule.Configuration.Resources;
using Selerant.DevEx.Modules.DIRModule.Configuration.Grids;

namespace Selerant.DevEx.Modules.DIRModule.WebMvcFrontEnd.Areas.AssessmentNavigator.Controllers
{
    /// <summary>
	/// MVS controller for panel General
	/// </summary>
	[ComponentDescriptor(
        new string[] { DIRModuleComponentIdentifier.ASSESSMENT_NAVIGATOR_GENERAL_PANEL },
        SecurityObjectType = typeof(GeneralPanelSecurity),
        VerifyControllerData = true,
        VerifyRequestSecurity = true
    )]
    [NavigatorPanel("Selerant.DevEx.Modules.DIRModule.AssessmentNavigator.GeneralPanel")]
    public partial class GeneralPanelController : DevExNavigatorPanelController<DxAssessment, GeneralPanelSecurity>
    {
        /// <summary>
        /// Generates a new Index Model
        /// </summary>
        /// <param name="controllerData"></param>
        /// <returns></returns>
        protected override NavigatorPanelModel<DxAssessment> BuildIndexModel(NavigatorPanelControllerData<DxAssessment> data)
        {
            return new GeneralPanelModel(GetControllerUrl(), data);
        }

        [HttpPost]
        [AntiCSRFToken(AntiCSRFTokenVerifyMode.Verify)]
        public virtual DataAjaxResult EditGeneralPanelDialogActivity(NavigatorPanelControllerData<DxAssessment> controllerData)
        {
            SecurityVerifier.AssertHasRights(() => GetSecurityObject(controllerData).CanEdit);
            SetHasVerifiedRequestSecurity();

            OpenInDialogCommandResolverParameter commandResolver = new OpenInDialogCommandResolverParameter(controllerData.TargetObject);
            commandResolver.EditMode = true;
            Dictionary<string, object> returnValue = new Dictionary<string, object>();
            JSOpenDialogActivity activity = CommandResolverHelper.GetOpenInDialogReference(commandResolver).Activity;
            activity.UrlParameters.Add("targetIdentifiableString", controllerData.TargetObject.IdentifiableString);
            activity.MinWidth = 1000;
            activity.MinHeight = 400;
            returnValue["EditGeneralPanelDialogActivity"] = activity;
            return new DataAjaxResult(returnValue);
        }

        #region Dialogs

        [HttpGet]
        [ExcludeControllerData]
        [ExcludeVerifySecurity]
        [AntiCSRFToken(AntiCSRFTokenVerifyMode.Ignore)]
        public virtual ViewResult EditGeneralPanelDialog(string assessmentIdentifiableString)
        {
            DxAssessment assessment = (DxAssessment)DxObject.ParseIdentifiableString(assessmentIdentifiableString);
            EditGeneralPanelDialogModel editGeneralPanelDialogModel = new EditGeneralPanelDialogModel(assessment, GetControllerUrl());
            return View(DIRModuleInfo.Instance, MVC_DIR.AssessmentNavigator.GeneralPanel.Views.EditGeneralPanelDialog, editGeneralPanelDialogModel);
        }

        [HttpPost]
        [ExcludeControllerData]
        [ExcludeVerifySecurity]
        [AntiCSRFToken(AntiCSRFTokenVerifyMode.Verify)]
        public virtual DataAjaxResult Update(GeneralModelPartial.GeneralModelPartialJson generalModelPartialJson)
        {
            Dictionary<string, object> returnValue = new Dictionary<string, object>();
            generalModelPartialJson.UpdateTestTemplateFromJson(generalModelPartialJson.TargetIdentifiableString);
            return new DataAjaxResult(returnValue);
        }

        #endregion

        [AcceptVerbs(HttpVerbs.Get | HttpVerbs.Post)]
        [AntiCSRFToken(AntiCSRFTokenVerifyMode.Auto)]
        public virtual DataAjaxResult Export(NavigatorPanelControllerData<DxAssessment> controllerData)
        {
            SecurityVerifier.AssertHasRights(() => GetSecurityObject(controllerData).CanView);
            SetHasVerifiedRequestSecurity();

            var model = new GeneralModelPartial(controllerData.TargetObject);

            ExcelExportScriptGenerator exportScriptGenerator = new ExcelExportScriptGenerator(controllerData, $"{model.TargetAssessment.Description} ({model.TargetAssessment.Code}).xlsx");

            var assessmentExportData = GetExportData(model);

            exportScriptGenerator.AddData(GridNames.GENERAL, assessmentExportData.Values, assessmentExportData.SheetTitle);

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

        private (ICollection<GeneralPanelRowItem> Values, string SheetTitle) GetExportData(GeneralModelPartial model)
        {
            string GetResource(string resourceKey)
            {
                return Locale.GetString(ResourceFiles.AssessmentManager, resourceKey);
            }

            ICollection<GeneralPanelRowItem> assessmentValues = new List<GeneralPanelRowItem>
            {
                GeneralPanelRowItem.Of(GetResource("DIR_GeneralPanel_ProductName"), model.Description),
                GeneralPanelRowItem.Of(GetResource("DIR_GeneralPanel_ProductClassification"), model.ProdClassificationPhraseText?.Text),
                GeneralPanelRowItem.Of(GetResource("DIR_GeneralPanel_TimeFrame_From"),  model.TimeframeFrom != null ? model.TimeframeFrom.Value.ToShortDateString() : ""),
                GeneralPanelRowItem.Of(GetResource("DIR_GeneralPanel_TimeFrame_To"), model.TimeframeTo != null ?model.TimeframeTo.Value.ToShortDateString() : ""),
                GeneralPanelRowItem.Of(GetResource("DIR_GeneralPanel_Location"), model.Location.Description),
                GeneralPanelRowItem.Of(GetResource("DIR_GeneralPanel_DataQuality"), model.DataQualityPhraseText?.Text),
                GeneralPanelRowItem.Of(GetResource("DIR_GeneralPanel_CreatedBy"), model.Completionist.FullName),
                GeneralPanelRowItem.Of(GetResource("PhoneNumber"), model.Phone),
                GeneralPanelRowItem.Of(GetResource("CompanyName"), model.CompanyName),
                GeneralPanelRowItem.Of(GetResource("DIR_GeneralPanel_OrgStructure"), model.OrgStructure),
                GeneralPanelRowItem.Of(GetResource("Comments"), model.Comments),
            };

            return (assessmentValues, GetResource("ANNNavTabGeneral"));
        }
    }
}