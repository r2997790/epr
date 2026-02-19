using Selerant.DevEx.BusinessLayer;
using Selerant.DevEx.BusinessLayer.Authorization;
using Selerant.DevEx.Modules.DIRModule.BackEnd.AssessmentManagement.BusinessObjects;
using Selerant.DevEx.Modules.DIRModule.Configuration.Grids;
using Selerant.DevEx.Modules.DIRModule.Configuration.Resources;
using Selerant.DevEx.Modules.DIRModule.ModuleBlocks._ThisProject;
using Selerant.DevEx.Modules.DIRModule.WebMvcFrontEnd._ThisProject;
using Selerant.DevEx.Modules.DIRModule.WebMvcFrontEnd.Areas.AssessmentNavigator.Models.Panels.AssessmentDashboardDialog;
using Selerant.DevEx.Modules.DIRModule.WebMvcFrontEnd.Areas.AssessmentNavigator.Models.Panels.AssessmentResults;
using Selerant.DevEx.Modules.DIRModule.WebMvcFrontEnd.Areas.AssessmentNavigator.Security;
using Selerant.DevEx.Modules.DIRModule.WebMvcFrontEnd.Helpers;
using Selerant.DevEx.Web;
using Selerant.DevEx.Web.Security;
using Selerant.DevEx.WebControls;
using Selerant.DevEx.WebMvcModules.Helpers;
using Selerant.DevEx.WebMvcModules.Helpers.jqGrid;
using Selerant.DevEx.WebMvcModules.Infrastructure.Base;
using Selerant.DevEx.WebMvcModules.Infrastructure.Controllers;
using Selerant.DevEx.WebMvcModules.Infrastructure.Controls.HtmlGrid;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.Mvc;

namespace Selerant.DevEx.Modules.DIRModule.WebMvcFrontEnd.Areas.AssessmentNavigator.Controllers
{
    [ComponentDescriptor(
    DIRModuleComponentIdentifier.ASSESSMENT_NAVIGATOR,
    SecurityObjectType = typeof(AssessmentNavigatorSecurity),
    VerifyControllerData = true,
    VerifyRequestSecurity = true)]
    public partial class AssessmentDashboardDialogController : DevExBaseController
    {
        #region Public Actions

        [HttpGet]
        [AntiCSRFToken(AntiCSRFTokenVerifyMode.Ignore)]
        public virtual System.Web.Mvc.ActionResult AssessmentDashboardDialogIndex(ViewControlControllerData controllerData, string objectIdentifiableString)
        {
            SecurityVerifier.AssertHasRights(() => GetSecurityObject(controllerData).CanView);
            SetHasVerifiedRequestSecurity();

            return AssessmentDashboardDialogOpen(controllerData, objectIdentifiableString);
        }

        [AcceptVerbs(HttpVerbs.Get | HttpVerbs.Post)]
        [AntiCSRFToken(AntiCSRFTokenVerifyMode.Auto)]
        public virtual DataAjaxResult GetAssessmentNames(ViewControlControllerData controllerData, List<string> objectIdentifiableStrings)
        {
            SecurityVerifier.AssertHasRights(() => GetSecurityObject(controllerData).CanView);
            SetHasVerifiedRequestSecurity();

            List<DxAssessment> assessments = new List<DxAssessment>();

            objectIdentifiableStrings.ForEach(identifiableString =>
            {
                var assessment = DxObject.ParseIdentifiableString<DxAssessment>(identifiableString);

                if (assessment.PersistenceStatus == DxPersistenceStatus.Unknown)
                    assessment.Load();

                assessments.Add(assessment);
            });

            return new DataAjaxResult(new { assessmentDescriptions = assessments.Select(assessment => AssessmentDashboardDataStore.GetAssessmentTitle(assessment)) });
        }

        [AcceptVerbs(HttpVerbs.Get | HttpVerbs.Post)]
        [AntiCSRFToken(AntiCSRFTokenVerifyMode.Auto)]
        public virtual DataAjaxResult MassCostChange(ViewControlControllerData controllerData, string hotSpotValue, List<string> objectIdentifiableStrings)
        {
            SecurityVerifier.AssertHasRights(() => GetSecurityObject(controllerData).CanView);
            SetHasVerifiedRequestSecurity();

            var model = new IndexModel(GetControllerUrl(), controllerData, objectIdentifiableStrings);
            var legend = model.GetAssessmentNames();
            var data = model.GetHotSpotChartData((HotSpotValueType)Enum.Parse(typeof(HotSpotValueType), hotSpotValue, true));
            List<object> itemsJSONData = GetJSONifiedChartData(data);

            return new DataAjaxResult(new { chartData = itemsJSONData, legendTitles = legend });
        }

        [AcceptVerbs(HttpVerbs.Get | HttpVerbs.Post)]
        [AntiCSRFToken(AntiCSRFTokenVerifyMode.Auto)]
        public virtual DataAjaxResult CostOfWasteChange(ViewControlControllerData controllerData, List<string> objectIdentifiableStrings)
        {
            SecurityVerifier.AssertHasRights(() => GetSecurityObject(controllerData).CanView);
            SetHasVerifiedRequestSecurity();

            var model = new IndexModel(GetControllerUrl(), controllerData, objectIdentifiableStrings);
            var legend = model.GetAssessmentNames();
            var data = model.GetCostOfWasteChartData();
            List<object> itemsJSONData = GetJSONifiedChartData(data);

            return new DataAjaxResult(new { chartData = itemsJSONData, legendTitles = legend });
        }

        [AcceptVerbs(HttpVerbs.Get | HttpVerbs.Post)]
        [AntiCSRFToken(AntiCSRFTokenVerifyMode.Auto)]
        public virtual DataAjaxResult KeyFinantialIndicatorsChange(ViewControlControllerData controllerData, List<string> objectIdentifiableStrings)
        {
            SecurityVerifier.AssertHasRights(() => GetSecurityObject(controllerData).CanView);
            SetHasVerifiedRequestSecurity();

            var model = new IndexModel(GetControllerUrl(), controllerData, objectIdentifiableStrings);
            var legend = model.GetAssessmentNames();
            var data = model.GetResultsChartData(AssessmentResultType.PERCENTAGE);
            List<object> itemsJSONData = GetJSONifiedChartData(data);

            return new DataAjaxResult(new { chartData = itemsJSONData });
        }

        [AcceptVerbs(HttpVerbs.Get | HttpVerbs.Post)]
        [AntiCSRFToken(AntiCSRFTokenVerifyMode.Auto)]
        public virtual JsonResult GetResultsGridData(ViewControlControllerData controllerData, GridSettings gridSettings, string objectIdentifiableStrings, string resultType)
        {
            SecurityVerifier.AssertHasRights(() => GetSecurityObject(controllerData).CanView);
            SetHasVerifiedRequestSecurity();

            var model = new IndexModel(GetControllerUrl(), controllerData, objectIdentifiableStrings.Split(IndexModel.IDENTIFIABLE_STRING_SEPARATOR).ToList());
            var data = model.GetResultsGridData(resultType);

            var htmlGridHelper = new SecureHtmlGridHelper(controllerData, model.GridResultType[resultType], null, controllerData.SecurityObject);

            var jsonData = new
            {
                total = (int)Math.Ceiling(data.Count / (float)gridSettings.pageSize),
                page = gridSettings.pageIndex,
                records = data.Count(),
                rows = htmlGridHelper.ConvertToHtmlGridArray(data.AsQueryable()
                                                                 .Paginate(gridSettings)
                                                                 .ToList())
            };

            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }

        [AcceptVerbs(HttpVerbs.Get | HttpVerbs.Post)]
        [AntiCSRFToken(AntiCSRFTokenVerifyMode.Auto)]
        public virtual JsonResult GetHotSpotGridData(ViewControlControllerData controllerData, GridSettings gridSettings, string objectIdentifiableStrings, string hotSpotValue)
        {
            SecurityVerifier.AssertHasRights(() => GetSecurityObject(controllerData).CanView);
            SetHasVerifiedRequestSecurity();

            var model = new IndexModel(GetControllerUrl(), controllerData, objectIdentifiableStrings.Split(IndexModel.IDENTIFIABLE_STRING_SEPARATOR).ToList());
            var data = model.GetHotSpotGridData((HotSpotValueType)Enum.Parse(typeof(HotSpotValueType), hotSpotValue, true));

            var htmlGridHelper = new SecureHtmlGridHelper(controllerData, model.HotSpotGridId, null, controllerData.SecurityObject);

            var jsonData = new
            {
                total = (int)Math.Ceiling(data.Count / (float)gridSettings.pageSize),
                page = gridSettings.pageIndex,
                records = data.Count(),
                rows = htmlGridHelper.ConvertToHtmlGridArray(data.AsQueryable()
                                                                 .Paginate(gridSettings)
                                                                 .ToList())
            };

            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }

        [AcceptVerbs(HttpVerbs.Get | HttpVerbs.Post)]
        [AntiCSRFToken(AntiCSRFTokenVerifyMode.Auto)]
        public virtual JsonResult GetCostOfWasteGridData(ViewControlControllerData controllerData, GridSettings gridSettings, string objectIdentifiableStrings)
        {
            SecurityVerifier.AssertHasRights(() => GetSecurityObject(controllerData).CanView);
            SetHasVerifiedRequestSecurity();

            var model = new IndexModel(GetControllerUrl(), controllerData, objectIdentifiableStrings.Split(IndexModel.IDENTIFIABLE_STRING_SEPARATOR).ToList());
            var data = model.GetCostOfWasteGridData();

            var htmlGridHelper = new SecureHtmlGridHelper(controllerData, model.CostOfWasteGridId, null, controllerData.SecurityObject);

            var jsonData = new
            {
                total = (int)Math.Ceiling(data.Count / (float)gridSettings.pageSize),
                page = gridSettings.pageIndex,
                records = data.Count(),
                rows = htmlGridHelper.ConvertToHtmlGridArray(data.AsQueryable()
                                                                 .Paginate(gridSettings)
                                                                 .ToList())
            };

            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }

        [AcceptVerbs(HttpVerbs.Get | HttpVerbs.Post)]
        [AntiCSRFToken(AntiCSRFTokenVerifyMode.Auto)]
        public virtual DataAjaxResult RefreshResultsGrid(ViewControlControllerData controllerData, string objectIdentifiableStrings, string resultType)
        {
            SecurityVerifier.AssertHasRights(() => GetSecurityObject(controllerData).CanView);
            SetHasVerifiedRequestSecurity();

            return new DataAjaxResult(new Dictionary<string, object>
            {
                { "gridUrl", Url.Action(MVC_DIR.AssessmentNavigator.AssessmentDashboardDialog.GetResultsGridData(controllerData, null, objectIdentifiableStrings, resultType))
                }
            });
        }

        [AcceptVerbs(HttpVerbs.Get | HttpVerbs.Post)]
        [AntiCSRFToken(AntiCSRFTokenVerifyMode.Auto)]
        public virtual DataAjaxResult RefreshCostOfWasteGrid(ViewControlControllerData controllerData, string objectIdentifiableStrings)
        {
            SecurityVerifier.AssertHasRights(() => GetSecurityObject(controllerData).CanView);
            SetHasVerifiedRequestSecurity();

            return new DataAjaxResult(new Dictionary<string, object>
            {
                { "gridUrl", Url.Action(MVC_DIR.AssessmentNavigator.AssessmentDashboardDialog.GetCostOfWasteGridData(controllerData, null, objectIdentifiableStrings))
                }
            });
        }

        [AcceptVerbs(HttpVerbs.Get | HttpVerbs.Post)]
        [AntiCSRFToken(AntiCSRFTokenVerifyMode.Auto)]
        public virtual DataAjaxResult RefreshHotSpotGrid(ViewControlControllerData controllerData, string objectIdentifiableStrings, string hotSpotValue)
        {
            SecurityVerifier.AssertHasRights(() => GetSecurityObject(controllerData).CanView);
            SetHasVerifiedRequestSecurity();

            return new DataAjaxResult(new Dictionary<string, object>
            {
                { "gridUrl", Url.Action(MVC_DIR.AssessmentNavigator.AssessmentDashboardDialog.GetHotSpotGridData(controllerData, null, objectIdentifiableStrings, hotSpotValue))
                }
            });
        }

        [AcceptVerbs(HttpVerbs.Get | HttpVerbs.Post)]
        [AntiCSRFToken(AntiCSRFTokenVerifyMode.Auto)]
        public virtual DataAjaxResult KeyProductionRatiosChange(ViewControlControllerData controllerData, List<string> objectIdentifiableStrings)
        {
            SecurityVerifier.AssertHasRights(() => GetSecurityObject(controllerData).CanView);
            SetHasVerifiedRequestSecurity();

            var model = new IndexModel(GetControllerUrl(), controllerData, objectIdentifiableStrings);
            var legend = model.GetAssessmentNames();
            var data = model.GetResultsChartData(AssessmentResultType.MASS);
            List<object> itemsJSONData = GetJSONifiedChartData(data);

            return new DataAjaxResult(new { chartData = itemsJSONData });
        }

        [AcceptVerbs(HttpVerbs.Get | HttpVerbs.Post)]
        [AntiCSRFToken(AntiCSRFTokenVerifyMode.Auto)]
        public virtual System.Web.Mvc.ActionResult RefreshFoodLossPartial(ViewControlControllerData controllerData, List<string> objectIdentifiableStrings, FoodLossType foodLossType)
        {
            SecurityVerifier.AssertHasRights(() => GetSecurityObject(controllerData).CanView);
            SetHasVerifiedRequestSecurity();

            var model = new IndexModel(GetControllerUrl(), controllerData, objectIdentifiableStrings);

            return PartialView(DIRModuleInfo.Instance, MVC_DIR.AssessmentNavigator.AssessmentDashboardDialog.Views.FoodLosses.FoodLossesContainer, model.FoodLosses[foodLossType]);
        }

        [AcceptVerbs(HttpVerbs.Get | HttpVerbs.Post)]
        [AntiCSRFToken(AntiCSRFTokenVerifyMode.Auto)]
        public virtual DataAjaxResult ExportResults(ViewControlControllerData controllerData, List<string> objectIdentifiableStrings)
        {
            SecurityVerifier.AssertHasRights(() => GetSecurityObject(controllerData).CanView);
            SetHasVerifiedRequestSecurity();

            var model = new IndexModel(GetControllerUrl(), controllerData, objectIdentifiableStrings);

            var hotSpotMass = model.GetHotSpotGridData(HotSpotValueType.MASS);
            var hotSpotCost = model.GetHotSpotGridData(HotSpotValueType.COST);
            var costOfWaste = model.GetCostOfWasteGridData();
            var productRatios = model.GetResultsGridData(AssessmentResultType.MASS);
            var keyFinancialIndicators = model.GetResultsGridData(AssessmentResultType.PERCENTAGE);

            ExcelExportScriptGenerator exportScriptGenerator = new ExcelExportScriptGenerator(controllerData, $"DashboardResults_{Guid.NewGuid()}.xlsx");

            exportScriptGenerator
                .AddData(model.HotSpotGridId, hotSpotMass, Locale.GetString(ResourceFiles.AssessmentManager, "AssessmentDashboard_MassHotspotTitle"))
                .AddData(model.HotSpotGridId, hotSpotCost, Locale.GetString(ResourceFiles.AssessmentManager, "AssessmentDashboard_CostHotspotTitle"))
                .AddData(model.CostOfWasteGridId, costOfWaste, Locale.GetString(ResourceFiles.AssessmentManager, "AssessmentDashboard_CostOfWaste"))
                .AddData(model.ProductionRatiosGridId, productRatios, Locale.GetString(ResourceFiles.AssessmentManager, "ResultsProdRatiosChartTitleNoDate"))
                .AddData(model.KeyFinantialIndicatorsGridId, keyFinancialIndicators, Locale.GetString(ResourceFiles.AssessmentManager, "ResultsFinIndChartTitleNoDate"));

            AddFoodLossesToExport(exportScriptGenerator, model.FoodLossesNotIncludedInedibleParts, Locale.GetString(ResourceFiles.AssessmentManager, "FoodMatLossesNotIncIndblPartsExcelSheetName"));
            AddFoodLossesToExport(exportScriptGenerator, model.FoodLossesInediblePartsOnly, Locale.GetString(ResourceFiles.AssessmentManager, "FoodMatLossesIndblPartsOnlyExcelSheetName"));

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

        private void AddFoodLossesToExport(ExcelExportScriptGenerator exportScriptGenerator, FoodLossesContainerModel model, string title)
        {
            model.FoodLosses.ForEach(foodLoss =>
            {
                exportScriptGenerator.AddData(GridNames.FOOD_WASTE_PER_DESTINATION, foodLoss.GridData, $"({foodLoss.Assessment.Code})-{title}");
            });
        }

        private List<object> GetJSONifiedChartData(IEnumerable<IChartData> data)
        {
            List<object> itemsJSONData = new List<object>();

            if (data != null && data.Count() > 0)
            {
                data.ForEach(item => itemsJSONData.Add(item.ConvertToJSONExchangeableObject()));
            }

            return itemsJSONData;
        }

        #endregion

        #region Private Actions

        private ViewResult AssessmentDashboardDialogOpen(ViewControlControllerData controllerData, string objectIdentifiableString)
        {
			var objectIdentifiableStrings = new List<string>() { objectIdentifiableString };
			var indexModel = new IndexModel(GetControllerUrl(), controllerData, objectIdentifiableStrings);
			return View(DIRModuleInfo.Instance, MVC_DIR.AssessmentNavigator.AssessmentDashboardDialog.Views.Index, indexModel);
		}

        #endregion

        #region Private Methods

        private AssessmentNavigatorSecurity GetSecurityObject(ViewControlControllerData controllerData)
        {
            return controllerData.SecurityObject as AssessmentNavigatorSecurity;
        }

        #endregion        
    }
}