using Selerant.DevEx.BusinessLayer;
using Selerant.DevEx.Modules.DIRModule.BackEnd.AssessmentManagement.BusinessObjects;
using Selerant.DevEx.Modules.DIRModule.ModuleBlocks._ThisProject;
using Selerant.DevEx.Modules.DIRModule.WebMvcFrontEnd._ThisProject.Infrastructure.Bundling;
using Selerant.DevEx.WebControls;
using Selerant.DevEx.WebMvcModules.Infrastructure.Controls;
using Selerant.DevEx.WebMvcModules.Infrastructure.Navigators.Controllers;
using Selerant.DevEx.WebMvcModules.Infrastructure.Navigators.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Selerant.DevEx.Modules.DIRModule.WebMvcFrontEnd.Areas.AssessmentNavigator.Models.Panels.GeneralPanel
{
    public class EditGeneralPanelDialogModel : DialogViewIndexModel 
    {
        public GeneralModelPartial GeneralModelPartial { get; set; }

        #region Constructor
        public EditGeneralPanelDialogModel(string controllerUrl, NavigatorPanelControllerData<DxAssessment> data)
       : base(controllerUrl, data)
        {
            if (data.TargetObject.PersistenceStatus == DxPersistenceStatus.Unknown)
                data.TargetObject.Load();

            GeneralModelPartial = new GeneralModelPartial(data.TargetObject) { DisplayMode = GeneralModelPartial.DisplayModeType.Edit };
        }

        public EditGeneralPanelDialogModel(DxAssessment target, string controllerUrl)
            : base(controllerUrl)
        {
            if (target.PersistenceStatus == DxPersistenceStatus.Unknown)
                target.Load();

            GeneralModelPartial = new GeneralModelPartial(target) { DisplayMode = GeneralModelPartial.DisplayModeType.Edit };
        }

       
        #endregion Constructor

        #region Overrides

        protected override void InitializeResources(List<ControlResource> resources)
		{
			base.InitializeResources(resources);
            resources.Add(new ControlJsFile(DIRLinks.Areas.AssessmentNavigator.Views.GeneralPanel.EditGeneralPanelDialog_ts));
            resources.Add(new ControlJsFile(DIRLinks.Areas.AssessmentNavigator.Views.GeneralPanel.GeneralPanelPartial_ts));

		}

		protected override void FillScriptControlDescriptor(GxScriptControlDescriptor scriptControlDescriptor)
		{
			base.FillScriptControlDescriptor(scriptControlDescriptor);
			scriptControlDescriptor.TypeName = "DX.DIRModule.AssessmentNavigator.EditGeneralPanelDialog";
		}

		#endregion Overrides
    }
}