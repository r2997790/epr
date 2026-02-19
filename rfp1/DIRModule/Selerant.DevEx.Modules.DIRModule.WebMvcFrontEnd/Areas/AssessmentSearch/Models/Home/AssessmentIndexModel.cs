using Selerant.DevEx.Modules.DIRModule.BackEnd.AssessmentManagement.BusinessObjects;
using Selerant.DevEx.Modules.DIRModule.WebMvcFrontEnd._ThisProject;
using Selerant.DevEx.Modules.DIRModule.WebMvcFrontEnd._ThisProject.Infrastructure.Bundling;
using Selerant.DevEx.WebControls;
using Selerant.DevEx.WebMvcModules.Infrastructure.Controls;
using Selerant.DevEx.WebMvcModules.Infrastructure.Searches.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Selerant.DevEx.Modules.DIRModule.WebMvcFrontEnd.Areas.AssessmentSearch.Models.Home
{
    public class AssessmentIndexModel : DevExSearchIndexModel<DxAssessment>
    {
        public AssessmentIndexModel(string controllerUrl, SearchControllerData data, ISearchController searchController, DevExSearchSettings settings) 
            : base(controllerUrl, data, searchController, settings)
        {
        }

        protected override void InitializeResources(List<ControlResource> resources)
        {
            base.InitializeResources(resources);

            resources.Add(new ControlFilesGroup(DIRStaticResourcesConfigurationUpdater.ASSESSMENT_SEARCH_BUNDLE_NAME));
        }

        protected override void FillScriptControlDescriptor(GxScriptControlDescriptor scriptControlDescriptor)
        {
            base.FillScriptControlDescriptor(scriptControlDescriptor);

            scriptControlDescriptor.TypeName = "DX.DIRModule.AssessmentSearch.Index";
        }
    }
}