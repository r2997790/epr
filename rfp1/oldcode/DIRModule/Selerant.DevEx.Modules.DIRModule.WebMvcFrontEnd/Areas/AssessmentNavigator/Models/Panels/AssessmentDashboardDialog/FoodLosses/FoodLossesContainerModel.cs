using Selerant.DevEx.Web;
using Selerant.DevEx.WebControls;
using Selerant.DevEx.WebMvcModules.Infrastructure.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Selerant.DevEx.Modules.DIRModule.WebMvcFrontEnd.Areas.AssessmentNavigator.Models.Panels.AssessmentDashboardDialog
{
    public class FoodLossesContainerModel : ViewControlIndexModel
    {
        public IList<FoodLossModel> FoodLosses { get; }
        public FoodLossType Type { get; }

        public FoodLossesContainerModel(string controllerUrl, ViewControlControllerData controllerData, FoodLossType foodLossType, IList<FoodLossModel> foodLosses) 
            : base(controllerUrl, controllerData)
        {
            Type = foodLossType;
            FoodLosses = foodLosses;
        }

        protected override void FillScriptControlDescriptor(GxScriptControlDescriptor scriptControlDescriptor)
        {
            base.FillScriptControlDescriptor(scriptControlDescriptor);

            scriptControlDescriptor.TypeName = "DX.DIRModule.AssessmentNavigator.FoodLossContainer";
            scriptControlDescriptor.Data["foodLossType"] = Type;
        }

        protected override void InitializeResources(List<ControlResource> resources)
        {
            base.InitializeResources(resources);

            resources.Add(new ControlJsFile(DIRLinks.Areas.AssessmentNavigator.Views.AssessmentDashboardDialog.FoodLosses.FoodLossesContainer_ts));
        }
    }
}