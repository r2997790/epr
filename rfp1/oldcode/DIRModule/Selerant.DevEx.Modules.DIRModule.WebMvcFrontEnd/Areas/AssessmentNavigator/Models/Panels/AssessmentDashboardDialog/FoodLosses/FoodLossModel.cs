using Newtonsoft.Json;
using Selerant.DevEx.BusinessLayer;
using Selerant.DevEx.Modules.DIRModule.BackEnd.AssessmentManagement.BusinessObjects;
using Selerant.DevEx.Modules.DIRModule.Configuration.Grids;
using Selerant.DevEx.Modules.DIRModule.WebMvcFrontEnd.Areas.AssessmentNavigator.Models.Panels.AssessmentResults.RowItem;
using Selerant.DevEx.Web;
using Selerant.DevEx.WebControls;
using Selerant.DevEx.WebModules.LCIAScenarioManager.LCIAScenarioNavigator.Base;
using Selerant.DevEx.WebMvcModules.Helpers.jqGrid;
using Selerant.DevEx.WebMvcModules.Infrastructure.Controls;
using Selerant.DevEx.WebMvcModules.Infrastructure.Controls.HtmlGrid;
using Selerant.DevEx.WebMvcModules.Infrastructure.Controls.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Selerant.DevEx.Modules.DIRModule.WebMvcFrontEnd.Areas.AssessmentNavigator.Models.Panels.AssessmentDashboardDialog
{
    public class FoodLossModel : ViewControlIndexModel
    {
        public const string GRID_NAME = GridNames.FOOD_WASTE_PER_DESTINATION_DASHBOARD;

        #region Properties

        public DxAssessment Assessment { get; set; }
        public FoodLossType FoodLossType { get; set; }
        public IList<string> ChartItemColors { get; set; }
        public IList<IChartData> ChartData { get; set; }
        public IList<IDashboardRowItem> GridData { get; set; }
        public string ModelLogicIdSuffix => $"_{Assessment.Code}_{FoodLossType.ToString().ToUpper()}";
        public string ChartId => $"FoodLossChart{ModelLogicIdSuffix}";
        public string GridId => $"{GRID_NAME}{ModelLogicIdSuffix}";
        public ChartControlIndexModel Chart { get; private set; }
        public jqGrid Grid { get; private set; }

        #endregion

        public FoodLossModel(string controllerUrl, ViewControlControllerData controllerData) 
            : base(controllerUrl, controllerData)
        {
        }

        public string GetJsonGridData()
        {
            var htmlGridHelper = new SecureHtmlGridHelper(ControllerData, GRID_NAME, null, ControllerData.SecurityObject);

            var jsonData = new
            {
                total = 1,
                page = 1,
                records = GridData.Count(),
                rows = htmlGridHelper.ConvertToHtmlGridArray(GridData)
            };

            return JsonConvert.SerializeObject(jsonData);
        }

        #region Overrides

        protected override void FillScriptControlDescriptor(GxScriptControlDescriptor scriptControlDescriptor)
        {
            base.FillScriptControlDescriptor(scriptControlDescriptor);

            scriptControlDescriptor.TypeName = "DX.DIRModule.AssessmentNavigator.FoodLoss";
            scriptControlDescriptor.Data["chartId"] = ChartId;
        }

        protected override void InitializeResources(List<ControlResource> resources)
        {
            base.InitializeResources(resources);

            resources.Add(new ControlJsFile(DIRLinks.Areas.AssessmentNavigator.Views.AssessmentDashboardDialog.FoodLosses.FoodLoss_ts));
        }

        #endregion
    }
}