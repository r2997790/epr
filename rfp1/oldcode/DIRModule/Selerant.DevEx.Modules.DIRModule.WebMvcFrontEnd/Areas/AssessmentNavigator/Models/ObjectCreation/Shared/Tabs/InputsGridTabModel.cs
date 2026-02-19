using System;
using System.Collections.Generic;
using System.Linq;
using Selerant.DevEx.Modules.DIRModule.WebMvcFrontEnd.Areas.AssessmentNavigator.Models.Panels.ResourceManagement;
using Selerant.DevEx.WebControls;
using Selerant.DevEx.WebMvcModules.Infrastructure.Controls;
using Selerant.DevEx.WebPages;

namespace Selerant.DevEx.Modules.DIRModule.WebMvcFrontEnd.Areas.AssessmentNavigator.Models.ObjectCreation.Shared.Tabs
{
	public class InputsGridTabModel : ViewControlIndexModel
	{

		public InputsGridTabModel(string controllerUrl) : base(controllerUrl)
		{
		}


		#region Overrides

		protected override void InitializeResources(List<ControlResource> resources)
		{
			base.InitializeResources(resources);
			resources.Add(new ControlFilesGroup(Infrastructure.StaticResources.CoreBundleNames.BUNDLE_MVCSIMPLECONTROLS));
			resources.Add(new ControlJsFile(DIRLinks.Areas.AssessmentNavigator.Views.CreateAssessmentDialog.Shared.Tabs.InputsGridTab_ts));

		}

		protected override void FillScriptControlDescriptor(GxScriptControlDescriptor scriptControlDescriptor)
		{
			base.FillScriptControlDescriptor(scriptControlDescriptor);
			scriptControlDescriptor.TypeName = "DX.DIRModule.AssessmentNavigator.Shared.Tabs.InputsGridTab";

		}

		#endregion Overrides
	}
}