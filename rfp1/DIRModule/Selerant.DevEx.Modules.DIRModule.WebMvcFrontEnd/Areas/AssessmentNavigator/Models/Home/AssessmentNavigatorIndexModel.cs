using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Selerant.DevEx.BusinessLayer;
using Selerant.DevEx.Modules.DIRModule.BackEnd.AssessmentManagement.BusinessObjects;
using Selerant.DevEx.Modules.DIRModule.WebMvcFrontEnd._ThisProject.Infrastructure.Bundling;
using Selerant.DevEx.Modules.DIRModule.WebMvcFrontEnd.Areas.AssessmentNavigator.Security;
using Selerant.DevEx.WebControls;
using Selerant.DevEx.WebMvcModules.Infrastructure.Controls;
using Selerant.DevEx.WebMvcModules.Infrastructure.Navigators.Models;
using Selerant.DevEx.Modules.DIRModule.BackEnd.InputManagement;

namespace Selerant.DevEx.Modules.DIRModule.WebMvcFrontEnd.Areas.AssessmentNavigator.Models.Home
{
    /// <summary>
    /// This is main model which us used in AssessmentNavigator/Views/Home/Index.cshtml and AssessmentNavigator/Controllers/HomeController
    /// You should also assign HeaderModel in .ctr
    /// </summary>
    public class AssessmentNavigatorIndexModel : NavigatorModel<DxAssessment, AssessmentNavigatorSecurity>
	{
        public string AssessmentCode { get; set; }

		public AssessmentNavigatorIndexModel(string controllerUrl, NavigatorControllerData<DxAssessment> controllerData)
			: base(controllerUrl, controllerData)
		{
            HeaderModel = new AssessmentNavigatorHeaderModel(controllerUrl, controllerData);
            AssessmentCode = controllerData.TargetObject.Code;
        }

		protected override void FillScriptControlDescriptor(GxScriptControlDescriptor scriptControlDescriptor)
		{
			base.FillScriptControlDescriptor(scriptControlDescriptor);

			scriptControlDescriptor.TypeName = "DX.DIRModule.AssessmentNavigator.Home.Index";

            scriptControlDescriptor.DomData["warningMessageDestination"] = CheckValidationInputDestination();

        }

		protected override void InitializeResources(List<ControlResource> resources)
		{
			base.InitializeResources(resources);

			resources.Add(new ControlFilesGroup(DIRStaticResourcesConfigurationUpdater.ASSESSMENT_NAVIGATOR_BUNDLE_NAME));
		}

		/// <summary>
		/// Initializes the view
		/// </summary>
		protected override void InitializeForView()
		{
			base.InitializeForView();
			NavigatorContainerCss = "AssessmentNavigator";

			PageTitle = TargetObject.ToString(WebEntityFormatterUtilities.NAVIGATOR_HEADER_SUBCAPTION_FORMAT, DxUser.CurrentUser.GetDxCulture());
		}

		private string CheckValidationInputDestination()
		{
			string stages = string.Empty;
			string errorMessage = string.Empty;

			DxDestinationValidationCollection dxDestinationValidation = new DxDestinationValidationCollection(AssessmentCode, true);

			// TODO: returns only for first invalid LC Stage with not massbalanced Destinations, and user can bi on correct LC Stage
			if (dxDestinationValidation.Count > 0)
			{
				stages = (dxDestinationValidation.Count == 1) ?
					string.Format(Locale.GetString("DIR_AssessmentManager", "HomeAlerMassageStage"), dxDestinationValidation.FirstOrDefault().Title)
				  : string.Format(Locale.GetString("DIR_AssessmentManager", "HomeAlerMassageStages"), string.Join(",", dxDestinationValidation.Select(des => des.Title).ToArray()));

				errorMessage = string.Format(Locale.GetString("DIR_AssessmentManager", "AlertErrorMassage"), stages);
			}

			return errorMessage;
		}
	}
}