using Selerant.DevEx.Modules.DIRModule.BackEnd.AssessmentManagement.BusinessObjects;
using Selerant.DevEx.WebMvcModules.Infrastructure.Navigators.Models;
using System.Collections.Generic;
using Selerant.DevEx.WebControls;
using Selerant.DevEx.WebMvcModules.Infrastructure.Controls;
using Selerant.DevEx.Modules.DIRModule.WebMvcFrontEnd.Areas.AssessmentNavigator.Security;

namespace Selerant.DevEx.Modules.DIRModule.WebMvcFrontEnd.Areas.AssessmentNavigator.Models.Panels.GeneralPanel
{
    public class GeneralPanelModel : NavigatorPanelModel<DxAssessment, GeneralPanelSecurity>
    {
        #region Consts

        public static readonly string ASSESMENT_GENERAL_PARTIAL = MVC_DIR.AssessmentNavigator.GeneralPanel.Views.GeneralPanelPartial;

        #endregion Consts

        #region Properties

        public GeneralModelPartial GeneralModelPartial { get; set; }
        #endregion Properties

        public GeneralPanelModel(string controllerUrl, NavigatorPanelControllerData<DxAssessment> data)
        : base(controllerUrl, data)
        {
            data.TargetObject.Load();

            // hide settings button: no grid is inside the panel
            StandardPanelButtons.EditSettings = false;
            SettingsConfigurationDialogUrl = WebMvcModules.Controllers.GridConfigurationController.GetIndexUrl(GridId, null);

            GeneralModelPartial = new GeneralModelPartial(data.TargetObject) { DisplayMode = GeneralModelPartial.DisplayModeType.View };
        }

        /// <summary>
        /// Name of the grid to be used also for settings
        /// </summary>
        //public static string GRID_NAME = "DIR_AssessmentNavigator_General";
        /// <summary>
        /// Sets the view
        /// </summary>
        protected override void InnerInitializeForView()
        {
            base.InnerInitializeForView();
            AddCssClass("dir-module-bus-nav-general-panel");
        }

        /// <summary>
        /// Initializes static resources
        /// </summary>
        /// <param name="resources"></param>
        protected override void InitializeResources(List<ControlResource> resources)
		{
			base.InitializeResources(resources);
            resources.Add(new ControlJsFile(DIRLinks.Areas.AssessmentNavigator.Views.GeneralPanel.GeneralPanel_ts));
        }

		/// <summary>
		/// Set client definition of model
		/// </summary>
		/// <param name="scriptControlDescriptor"></param>
		protected override void FillScriptControlDescriptor(GxScriptControlDescriptor scriptControlDescriptor)
        {
            base.FillScriptControlDescriptor(scriptControlDescriptor);
            scriptControlDescriptor.TypeName = "DX.DIRModule.AssessmentNavigator.GeneralPanel";
        }
	}
}
