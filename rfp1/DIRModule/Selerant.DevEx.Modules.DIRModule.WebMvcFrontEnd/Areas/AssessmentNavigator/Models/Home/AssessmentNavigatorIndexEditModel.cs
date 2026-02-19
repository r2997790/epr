using Selerant.DevEx.Modules.DIRModule.BackEnd.AssessmentManagement.BusinessObjects;
using Selerant.DevEx.Modules.DIRModule.WebMvcFrontEnd._ThisProject.Infrastructure.Bundling;
using Selerant.DevEx.WebControls;
using Selerant.DevEx.WebMvcModules.Infrastructure.Controls;
using Selerant.DevEx.WebMvcModules.Infrastructure.Navigators.Models;
using Selerant.DevEx.WebPages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Selerant.DevEx.Modules.DIRModule.WebMvcFrontEnd.Areas.AssessmentNavigator.Models.Home
{
    public class AssessmentNavigatorIndexEditModel : NavigatorEditModel<DxAssessment>
    {
        #region Properties
        
        public bool IsCreating { get; set; }

        #endregion

        #region Constructors

        public AssessmentNavigatorIndexEditModel(String controllerUrl, NavigatorModel navigationModel, Boolean isCreating) : base(controllerUrl, navigationModel)
        {
            IsCreating = isCreating;
            MenuModel.GetItem("OkButton").IsVisible = true;
        }

        #endregion

        #region Methods

        protected override DevExDialogPage.DialogButtonSet? GetButtonSetLayout()
        {
            return DevExDialogPage.DialogButtonSet.OkCancel;
        }

        protected override void InitializeResources(List<ControlResource> resources)
        {
            base.InitializeResources(resources);

            //resources.Add(new ControlFilesGroup(DIRStaticResourcesConfigurationUpdater.ASSESSMENT_NAVIGATOR_BUNDLE_NAME));
            resources.Add(new ControlJsFile(DIRLinks.Areas.AssessmentNavigator.Views.Home.IndexEdit_ts));
            resources.Add(new ControlCssFile(DIRLinks.Areas.AssessmentNavigator.Views.Home.IndexEdit_css));
        }

        protected override void FillScriptControlDescriptor(GxScriptControlDescriptor scriptControlDescriptor)
        {
            base.FillScriptControlDescriptor(scriptControlDescriptor);

            scriptControlDescriptor.TypeName = "DX.DIRModule.AssessmentNavigator.Home.IndexEdit";
            scriptControlDescriptor.DomData["isCreating"] = IsCreating;
        }

        #endregion
    }
}