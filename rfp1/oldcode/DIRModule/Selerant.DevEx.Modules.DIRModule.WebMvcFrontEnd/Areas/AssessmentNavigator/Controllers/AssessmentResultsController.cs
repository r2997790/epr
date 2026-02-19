using Selerant.DevEx.BusinessLayer;
using Selerant.DevEx.BusinessLayer.Authorization;
using Selerant.DevEx.CommonComponents;
using Selerant.DevEx.Modules.DIRModule.BackEnd.AssessmentManagement.BusinessObjects;
using Selerant.DevEx.Modules.DIRModule.Configuration.Resources;
using Selerant.DevEx.Modules.DIRModule.WebMvcFrontEnd._ThisProject;
using Selerant.DevEx.Modules.DIRModule.WebMvcFrontEnd.Areas.AssessmentNavigator.Models.Panels.AssessmentResults;
using Selerant.DevEx.Modules.DIRModule.WebMvcFrontEnd.Areas.AssessmentNavigator.Security;
using Selerant.DevEx.Modules.DIRModule.WebMvcFrontEnd.Helpers;
using Selerant.DevEx.Web;
using Selerant.DevEx.Web.Security;
using Selerant.DevEx.WebControls;
using Selerant.DevEx.WebModules.LCIAScenarioManager.LCIAScenarioNavigator.Base;
using Selerant.DevEx.WebMvcModules.Helpers;
using Selerant.DevEx.WebMvcModules.Infrastructure.Base;
using Selerant.DevEx.WebMvcModules.Infrastructure.Controls.HtmlGrid;
using Selerant.DevEx.WebMvcModules.Infrastructure.Navigators.Controllers;
using Selerant.DevEx.WebMvcModules.Infrastructure.Navigators.Helpers;
using Selerant.DevEx.WebMvcModules.Infrastructure.Navigators.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.Mvc;

namespace Selerant.DevEx.Modules.DIRModule.WebMvcFrontEnd.Areas.AssessmentNavigator.Controllers
{
    /// <summary>
	/// MVC controller for panel AssessmentResults
	/// </summary>
	[ComponentDescriptor(
		new string[] { DIRModuleComponentIdentifier.ASSESSMENT_NAVIGATOR_ASSESSMENT_RESULTS_PANEL },
        SecurityObjectType = typeof(AssessmentResultsSecurity),
		VerifyControllerData = true,
		VerifyRequestSecurity = true
    )]
    [NavigatorPanel("Selerant.DevEx.Modules.DIRModule.AssessmentNavigator.AssessmentResults")]
    public partial class AssessmentResultsController : DevExNavigatorPanelController<DxAssessment, AssessmentResultsSecurity>
    {
        /// <summary>
        /// Generates a new Index Model
        /// </summary>
        /// <param name="controllerData"></param>
        /// <returns></returns>
        protected override NavigatorPanelModel<DxAssessment> BuildIndexModel(NavigatorPanelControllerData<DxAssessment> data)
        {
            return new AssessmentResultsModel(GetControllerUrl(), data);
        }

        [AcceptVerbs(HttpVerbs.Get | HttpVerbs.Post)]
        [AntiCSRFToken(AntiCSRFTokenVerifyMode.Auto)]
        public virtual JsonResult GetAssessmentResults(NavigatorPanelControllerData<DxAssessment> controllerData, decimal lcStage, string resultType, bool loadData = false)
        {
			SecurityVerifier.AssertHasRights(() => GetSecurityObject(controllerData).CanView);
			SetHasVerifiedRequestSecurity();

			if (!loadData)
				return Json(new byte[0], JsonRequestBehavior.AllowGet);

			var model = new AssessmentResultsModel(GetControllerUrl(), controllerData, lcStage);
			var data = model.GetResults(resultType).OrderByDescending(o => o.SortOrder).ToList();

            var htmlGridHelper = new SecureHtmlGridHelper(controllerData, model.GridResultType[resultType], null, controllerData.SecurityObject);
            var jsonData = new
            {
                total = 1,
                page = 1,
                records = data.Count(),
                rows = htmlGridHelper.ConvertToHtmlGridArray(data)
            };

            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }

        [AcceptVerbs(HttpVerbs.Get | HttpVerbs.Post)]
        [AntiCSRFToken(AntiCSRFTokenVerifyMode.Auto)]
        public virtual JsonResult GetFoodWasteDestinationData(NavigatorPanelControllerData<DxAssessment> controllerData, decimal lcStage, bool loadData = false)
        {
            SecurityVerifier.AssertHasRights(() => GetSecurityObject(controllerData).CanView);
            SetHasVerifiedRequestSecurity();

            if (!loadData)
                return Json(new byte[0], JsonRequestBehavior.AllowGet);

            var model = new AssessmentResultsModel(GetControllerUrl(), controllerData, lcStage);
            var data = model.GetFoodWastePerDestination();

            var htmlGridHelper = new SecureHtmlGridHelper(controllerData, model.FoodWastePerDestinationGridId, null, controllerData.SecurityObject);

            var jsonData = new
            {
                total = 1,
                page = 1,
                records = data.Count(),
                rows = htmlGridHelper.ConvertToHtmlGridArray(data)
            };

            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }

        [AcceptVerbs(HttpVerbs.Get | HttpVerbs.Post)]
        [AntiCSRFToken(AntiCSRFTokenVerifyMode.Auto)]
        public virtual JsonResult GetCostOfWasteData(NavigatorPanelControllerData<DxAssessment> controllerData, decimal lcStage, bool loadData = false)
        {
            SecurityVerifier.AssertHasRights(() => GetSecurityObject(controllerData).CanView);
            SetHasVerifiedRequestSecurity();

            if (!loadData)
                return Json(new byte[0], JsonRequestBehavior.AllowGet);

            var model = new AssessmentResultsModel(GetControllerUrl(), controllerData, lcStage);
            var data = model.GetCostOfWasteGridData();

            var htmlGridHelper = new SecureHtmlGridHelper(controllerData, model.CostOfWasteGridId, null, controllerData.SecurityObject);

            var jsonData = new
            {
                total = 1,
                page = 1,
                records = data.Count(),
                rows = htmlGridHelper.ConvertToHtmlGridArray(data)
            };

            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Dashboard for DIRECT Assessment
        /// </summary>
        [HttpPost]
        [AntiCSRFToken(AntiCSRFTokenVerifyMode.Verify)]
        public virtual DataAjaxResult AssessmentDashboard(NavigatorPanelControllerData<DxAssessment> controllerData, string objectIdentifiableString)
        {
            SecurityVerifier.AssertHasRights(() => GetSecurityObject(controllerData).CanView);
            SetHasVerifiedRequestSecurity();
                        
            string dialogUrl = Url.Action(MVC_DIR.AssessmentNavigator.AssessmentDashboardDialog.AssessmentDashboardDialogIndex(controllerData, objectIdentifiableString));

            JSOpenDialogActivity activity = new JSOpenDialogActivity()
            {
                Url = ComponentDataHelper.AddDataToUrl(dialogUrl, controllerData),
                Width = short.MaxValue,
                Height = short.MaxValue,
                IsResizable = false
            };

            activity.SetCaption(Locale.GetString(ResourceFiles.AssessmentManager, "AssessmentDashboard"));
            // add notification Notification in case of error
            string Notification = string.Empty;

            return new DataAjaxResult(new Dictionary<string, object>
            {
                { "Activity", activity },
                { "ErrorMessage", Notification}
            });
        }

        [HttpPost]
        [AntiCSRFToken(AntiCSRFTokenVerifyMode.Verify)]
        public virtual DataAjaxResult ExportResults(NavigatorPanelControllerData<DxAssessment> controllerData, decimal lcStage)
        {
            SecurityVerifier.AssertHasRights(() => GetSecurityObject(controllerData).CanView);
            SetHasVerifiedRequestSecurity();

            AssessmentResultsModel model = new AssessmentResultsModel(GetControllerUrl(), controllerData, lcStage);

            var productionRatiosData = model.GetResults(AssessmentResultType.MASS).OrderByDescending(o => o.SortOrder).ToList();
            var keyFinantialIndicatorsData = model.GetResults(AssessmentResultType.PERCENTAGE).OrderByDescending(o => o.SortOrder).ToList();
            var costOfWasteData = model.GetCostOfWasteGridData();
            var foodWasteDestinationData = model.GetFoodWastePerDestination();

            DxAssessmentLcStage stage = new DxAssessmentLcStage(lcStage);

            if (stage.PersistenceStatus == DxPersistenceStatus.Unknown)
                stage.Load();

            string fileName = $"{controllerData.TargetObject.Description} ({controllerData.TargetObject.Code})_{stage.Title}_{Guid.NewGuid()}.xlsx";

            ExcelExportScriptGenerator exportScriptGenerator = new ExcelExportScriptGenerator(controllerData, fileName);

            exportScriptGenerator
                .AddData(model.ProductionRatiosGridId, productionRatiosData, Locale.GetString(ResourceFiles.AssessmentManager, "ResultsProdRatiosChartTitle"))
                .AddData(model.KeyFinantialIndicatorsGridId, keyFinantialIndicatorsData, Locale.GetString(ResourceFiles.AssessmentManager, "ResultsFinIndChartTitle"))
                .AddData(model.CostOfWasteGridId, costOfWasteData, Locale.GetString(ResourceFiles.AssessmentManager, "ResultsCostOfWasteChartTitle"))
                .AddData(model.FoodWastePerDestinationGridId, foodWasteDestinationData, Locale.GetString(ResourceFiles.AssessmentManager, "ResultsPyramidChartFoodDestination"));

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

        #region Grid Action Methods

        [HttpPost]
        [AntiCSRFToken(AntiCSRFTokenVerifyMode.Verify)]
        public virtual DataAjaxResult RefreshResultsGrid(NavigatorPanelControllerData<DxAssessment> controllerData, decimal lcStage, string resultType)
        {
            SecurityVerifier.AssertHasRights(() => GetSecurityObject(controllerData).CanView);
            SetHasVerifiedRequestSecurity();

            return new DataAjaxResult(new Dictionary<string, object>
            {
                { "gridUrl", Url.Action(MVC_DIR.AssessmentNavigator.AssessmentResults.GetAssessmentResults(controllerData, lcStage, resultType, true)) },
            });
        }

        [HttpPost]
        [AntiCSRFToken(AntiCSRFTokenVerifyMode.Verify)]
        public virtual DataAjaxResult RefreshFoodWastePerDestinationGrid(NavigatorPanelControllerData<DxAssessment> controllerData, decimal lcStage)
        {
            SecurityVerifier.AssertHasRights(() => GetSecurityObject(controllerData).CanView);
            SetHasVerifiedRequestSecurity();

            return new DataAjaxResult(new Dictionary<string, object>
            {
                { "gridUrl", Url.Action(MVC_DIR.AssessmentNavigator.AssessmentResults.GetFoodWasteDestinationData(controllerData, lcStage, true)) }
            });
        }

        [HttpPost]
        [AntiCSRFToken(AntiCSRFTokenVerifyMode.Verify)]
        public virtual DataAjaxResult RefreshCostOfWasteGrid(NavigatorPanelControllerData<DxAssessment> controllerData, decimal lcStage)
        {
            SecurityVerifier.AssertHasRights(() => GetSecurityObject(controllerData).CanView);
            SetHasVerifiedRequestSecurity();

            return new DataAjaxResult(new Dictionary<string, object>
            {
                { "gridUrl", Url.Action(MVC_DIR.AssessmentNavigator.AssessmentResults.GetCostOfWasteData(controllerData, lcStage, true)) }
            });
        }

        #endregion

        #region Chart Action Methods

        [AcceptVerbs(HttpVerbs.Get | HttpVerbs.Post)]
        [AntiCSRFToken(AntiCSRFTokenVerifyMode.Auto)]
        public virtual DataAjaxResult RefreshCharts(NavigatorPanelControllerData<DxAssessment> controllerData, decimal lcStage)
        {
            SecurityVerifier.AssertHasRights(() => GetSecurityObject(controllerData).CanView);
            SetHasVerifiedRequestSecurity();

            AssessmentResultsModel model = new AssessmentResultsModel(GetControllerUrl(), controllerData, lcStage);

            ICollection<NameValueChartData> costOfWasteChartData = model.GetCostOfWasteChartData();
            IReadOnlyCollection<string> costOfWasteLegendData = model.GetLegendNames(costOfWasteChartData);
            ICollection<DictionaryNameValueChartData> keyFinantialIndicatorsChartData = model.GetKeyFinantialIndicators();
            ICollection<DictionaryNameValueChartData> productionRatiosChartData = model.GetProductionRatiosChartData();
            ICollection<NameValueChartData> pyramidChartData = model.GetPyramidDestinationData();
            ICollection<string> pyramidColors = model.GetPyramidColorData();

            var results = new
            {
                timeFrom = model.AssessmentTimeFrameFrom,
                timeTo = model.AssessementTimeFrameTo,
                costOfWasteChartData = GetJSONifiedChartData(costOfWasteChartData),
                costOfWasteLegendData,
                keyFinantialIndicatorsChartData = GetJSONifiedChartData(keyFinantialIndicatorsChartData),
                productionRatiosChartData = GetJSONifiedChartData(productionRatiosChartData),
                pyramidChartData = GetJSONifiedChartData(pyramidChartData),
                pyramidColors
            };

            return new DataAjaxResult(results);
        }

        #endregion

        #region Private Methods

        private List<object> GetJSONifiedChartData(IEnumerable<IChartData> data)
        {
            List<object> itemsJSONData = new List<object>();

            if (data != null && data.Count() > 0)
                data.ForEach(item => itemsJSONData.Add(item.ConvertToJSONExchangeableObject()));

            return itemsJSONData;
        }

        #endregion
    }
}