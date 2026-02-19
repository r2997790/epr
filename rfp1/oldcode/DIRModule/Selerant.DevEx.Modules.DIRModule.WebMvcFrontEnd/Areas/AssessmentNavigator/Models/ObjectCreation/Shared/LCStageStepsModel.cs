using Selerant.DevEx.Modules.DIRModule.BackEnd.AssessmentManagement.BusinessObjects;
using Selerant.DevEx.WebControls;
using Selerant.DevEx.WebMvcModules.Infrastructure.Controls;
using Selerant.DevEx.WebMvcModules.Infrastructure.Navigators.Models;
using System.Collections.Generic;
using System.Linq;

namespace Selerant.DevEx.Modules.DIRModule.WebMvcFrontEnd.Areas.AssessmentNavigator.Models.ObjectCreation.Shared
{
	public class LCStageStepsModel : ViewControlIndexModel
    {
        public enum ViewMode
        {
            Navigator,
            CreationDialog
        }

		#region Properties

		private DxAssessment Assessment { get; set; }
		private List<DxAssessmentLcStage> LcStages { get; set; }
		private ViewMode CtrlViewMode { get; set; }

		public decimal InitialLcStage { get; private set; }

		#endregion

		#region Constructors

		public LCStageStepsModel(DxAssessment assessment) 
            : base(null, null)
        {
            InitializeData(assessment);
            CtrlViewMode = ViewMode.CreationDialog;
        }

        public LCStageStepsModel(string controllerUrl, NavigatorControllerData<DxAssessment> data) 
            : base(controllerUrl, data)
        {
            InitializeData(data.TargetObject);
            CtrlViewMode = ViewMode.Navigator;
        }

		#endregion

		private void InitializeData(DxAssessment assessment)
        {
            Assessment = assessment;

			var dxLcStages = new DxAssessmentLcStageCollection(Assessment);
			dxLcStages.Load();

			LcStages = dxLcStages.OrderBy(o => o.SortOrder).ToList();
			InitialLcStage = LcStages.First().Id;
		}

        protected override void InitializeResources(List<ControlResource> resources)
        {
            base.InitializeResources(resources);

            resources.Add(new ControlFilesGroup(Infrastructure.StaticResources.CoreBundleNames.BUNDLE_MVCSIMPLECONTROLS));
            resources.Add(new ControlJsFile(DIRLinks.Areas.AssessmentNavigator.Views.CreateAssessmentDialog.Shared.LCStageSteps_ts));
            resources.Add(new ControlCssFile(DIRLinks.Areas.AssessmentNavigator.Views.CreateAssessmentDialog.Shared.LCStageSteps_css));
        }

        protected override void FillScriptControlDescriptor(GxScriptControlDescriptor scriptControlDescriptor)
        {
            base.FillScriptControlDescriptor(scriptControlDescriptor);
            scriptControlDescriptor.TypeName = "DX.DIRModule.AssessmentNavigator.LCStageSteps";
            scriptControlDescriptor.Data["lcs"] = LcStages.Select(r => new { r.IdentifiableString, r.Id, r.Title}).ToList();
            scriptControlDescriptor.Data["vmode"] = CtrlViewMode;
        }
    }
}