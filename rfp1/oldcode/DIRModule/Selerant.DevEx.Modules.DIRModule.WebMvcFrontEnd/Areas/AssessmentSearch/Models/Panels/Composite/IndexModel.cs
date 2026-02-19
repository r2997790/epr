using Selerant.DevEx.Modules.DIRModule.BackEnd.AssessmentManagement.BusinessObjects;
using Selerant.DevEx.Modules.DIRModule.ModuleBlocks._ThisProject;
using Selerant.DevEx.WebControls;
using Selerant.DevEx.WebMvcModules.Infrastructure.Controls;
using Selerant.DevEx.WebMvcModules.Infrastructure.Searches.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Selerant.DevEx.Modules.DIRModule.WebMvcFrontEnd.Areas.AssessmentSearch.Models.Panels.Composite
{
    public class IndexModel : DevExSearchCompositePanelModel<DxAssessment>
    {
        public IndexModel(string controllerUrl, SearchControllerData data, ISearchController searchController, string nodeId) : base(controllerUrl, data, searchController, nodeId)
        {
        }

        protected override void InitializeResources(List<ControlResource> resources)
        {
            base.InitializeResources(resources);

            resources.Add(new ControlJsFile(DIRLinks.Areas.AssessmentSearch.Views.CompositePanel.IndexModel_ts));
        }

        protected override void FillScriptControlDescriptor(GxScriptControlDescriptor scriptControlDescriptor)
        {
            base.FillScriptControlDescriptor(scriptControlDescriptor);

            scriptControlDescriptor.TypeName = "DX.DIRModule.AssessmentSearch.CompositePanelIndex";
        }
    }
}