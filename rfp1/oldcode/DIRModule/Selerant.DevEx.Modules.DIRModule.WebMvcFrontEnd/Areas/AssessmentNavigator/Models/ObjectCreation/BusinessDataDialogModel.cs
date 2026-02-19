using System;
using System.Collections.Generic;
using System.Linq;
using Selerant.DevEx.Modules.DIRModule.BackEnd.AssessmentManagement.BusinessObjects;
using Selerant.DevEx.Modules.DIRModule.WebMvcFrontEnd.Areas.AssessmentNavigator.Models.ObjectCreation.Shared;
using Selerant.DevEx.Modules.DIRModule.WebMvcFrontEnd.Areas.AssessmentNavigator.Models.Panels.ResourceManagement.Shared;
using Selerant.DevEx.Web;
using Selerant.DevEx.WebControls;
using Selerant.DevEx.WebMvcModules.Infrastructure.Controls;
using Selerant.DevEx.WebMvcModules.Infrastructure.Controls.Menu.Items;

namespace Selerant.DevEx.Modules.DIRModule.WebMvcFrontEnd.Areas.AssessmentNavigator.Models.ObjectCreation
{
	public class BusinessDataDialogModel : DialogViewIndexModel
	{
		private const string DLG_VIEW_CONTAINER_CLASS = "dir-module-dlg-business-data";
		private decimal? initialLcStage;
		private readonly bool _cancelOrCloseDeletes = true;

		public DxAssessment Assessment { get; set; }
        public LCStageStepsModel LcStageSteps { get; set; }
		public decimal InitialLcStage
		{
			get => initialLcStage.HasValue ? initialLcStage.Value : LcStageSteps.InitialLcStage;
			set => initialLcStage = value;
		}
		public BaseGridTabModel.ViewMode ViewMode { get => BaseGridTabModel.ViewMode.CreationDialog; }

		#region Constructors

		public BusinessDataDialogModel(string controllerUrl, ViewControlControllerData controllerData, DxAssessment assessment, bool cancelOrCloseDeletes)
			: base(controllerUrl, controllerData)
		{
			Assessment = assessment;
			LcStageSteps = new LCStageStepsModel(Assessment);
			_cancelOrCloseDeletes = cancelOrCloseDeletes;
			SetMenuButtons();
		}

		#endregion

		protected override void InitializeForView()
		{
			base.InitializeForView();
			AddCssClass(DLG_VIEW_CONTAINER_CLASS);
		}

		protected override void FillScriptControlDescriptor(GxScriptControlDescriptor scriptControlDescriptor)
		{
			base.FillScriptControlDescriptor(scriptControlDescriptor);
			scriptControlDescriptor.TypeName = "DX.DIRModule.AssessmentNavigator.BusinessDataDialog";

			scriptControlDescriptor.Data.Add("viewContainerClass", DLG_VIEW_CONTAINER_CLASS);
			scriptControlDescriptor.Data.Add("identifiableString", Assessment.IdentifiableString);
			scriptControlDescriptor.Data.Add("cancelOrCloseDeletes", _cancelOrCloseDeletes);
		}

		protected override void InitializeResources(List<ControlResource> resources)
		{
			base.InitializeResources(resources);
			resources.Add(new ControlFilesGroup(Infrastructure.StaticResources.CoreBundleNames.BUNDLE_MVCSIMPLECONTROLS));
			resources.Add(new ControlJsFile(DIRLinks.Areas.AssessmentNavigator.Views.CreateAssessmentDialog.BusinessDataDialog_ts));
			resources.Add(new ControlJsFile(DIRLinks.Areas.AssessmentNavigator.Views.ResourceManagement.Shared.GridTab_ts));
			resources.Add(new ControlCssFile(DIRLinks.Areas.AssessmentNavigator.Views.CreateAssessmentDialog.BusinessDataDialog_css));
		}

		private void SetMenuButtons()
        {
            MenuModel
            .AddButton(WebPages.DevExDialogPage.BUTTON_CANCEL_ID, ResText.Controls.GetString("DlgCancel"))
            .AddItem(new ButtonMenuItem(WebPages.DevExDialogPage.BUTTON_BACK_ID)
            {
                IsVisible = false,
            }.SetText(ResText.Controls.GetString("BackButtonText")))            
            .AddButton(WebPages.DevExDialogPage.BUTTON_NEXT_ID, ResText.Controls.GetString("DlgNext"), "confirm")
            .AddItem(new ButtonMenuItem(WebPages.DevExDialogPage.BUTTON_FINISH_ID)
            {
                IsVisible = false,
                CssClass = "confirm"
            }.SetText(ResText.Controls.GetString("DlgFinish")));
        }

	}
}