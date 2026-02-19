using Selerant.DevEx.Modules.DIRModule.BackEnd.AssessmentManagement.BusinessObjects;
using Selerant.DevEx.Modules.DIRModule.WebMvcFrontEnd.Areas.AssessmentNavigator.Security;
using Selerant.DevEx.WebControls;
using Selerant.DevEx.WebMvcModules.Infrastructure.Controls;
using Selerant.DevEx.WebMvcModules.Infrastructure.Navigators.Models;

namespace Selerant.DevEx.Modules.DIRModule.WebMvcFrontEnd.Areas.AssessmentNavigator.Models.Panels.ResourceManagement.Shared
{
	public abstract class BaseGridTabModel : ViewControlIndexModel
	{
		public enum ViewMode
		{
			Navigator,
			CreationDialog
		}

		public DxAssessment Assessment { get; private set; }
		public decimal LcStageId { get; private set; }
        public abstract string GridUrl { get; }
		
        public string GridEmptyBlockID => "GridEmpty_" + GridID;
		public NotesModel Notes { get; set; }

		public ResourceManagementSecurity SecurityObject => ControllerData.SecurityObject as ResourceManagementSecurity;
		public abstract string  ResourceType { get; }
		public abstract string GridID { get; }
		public abstract string EmptyGridMessage { get; }


		public BaseGridTabModel(string controllerUrl, NavigatorPanelControllerData<DxAssessment> navigatorPanelControllerData, decimal lcStageId)
			: base(controllerUrl, navigatorPanelControllerData)
		{
			Assessment = navigatorPanelControllerData.TargetObject;
			LcStageId = lcStageId;
		}

		protected override void FillScriptControlDescriptor(GxScriptControlDescriptor scriptControlDescriptor)
		{
			base.FillScriptControlDescriptor(scriptControlDescriptor);
			scriptControlDescriptor.Data.Add("gridID", GridID);
			scriptControlDescriptor.Data.Add("gridUrl", GridUrl);
			scriptControlDescriptor.Data.Add("lcStageId", LcStageId);
			scriptControlDescriptor.Data.Add("canEditOrDelete", SecurityObject.CanEditOrDelete);
		}
	}
}