using System;
using System.Web.Mvc;
using Selerant.DevEx.BusinessLayer;
using Selerant.DevEx.BusinessLayer.Authorization;
using Selerant.DevEx.CommonComponents;
using Selerant.DevEx.Modules.DIRModule.BackEnd.AssessmentManagement.BusinessObjects;
using Selerant.DevEx.Modules.DIRModule.WebMvcFrontEnd._ThisProject;
using Selerant.DevEx.Modules.DIRModule.WebMvcFrontEnd.Areas.AssessmentNavigator.Models.Dialogs.ManageAssessment;
using Selerant.DevEx.Modules.DIRModule.WebMvcFrontEnd.Areas.AssessmentNavigator.Models.Home;
using Selerant.DevEx.Modules.DIRModule.WebMvcFrontEnd.Areas.AssessmentNavigator.Security;
using Selerant.DevEx.Web;
using Selerant.DevEx.Web.Security;
using Selerant.DevEx.WebMvcModules.Helpers;
using Selerant.DevEx.WebMvcModules.Infrastructure.Base;
using Selerant.DevEx.WebMvcModules.Infrastructure.Navigators.Controllers;
using Selerant.DevEx.WebMvcModules.Infrastructure.Navigators.Models;

namespace Selerant.DevEx.Modules.DIRModule.WebMvcFrontEnd.Areas.AssessmentNavigator.Controllers
{
    /// <summary>
    /// Main controller that renders the Navigator 
    /// Index action is the default action and can be accessed by Url /WebMvcModules/AssessmentNavigator/Home
    /// AssessmentNavigator/Views/Home/Index.cshtml - main entry point	
    /// Navigator has 2 assets files on client side:
    /// AssessmentNavigator/Views/Home/Index.css
    /// AssessmentNavigator/Views/Home/Index.ts	
    /// </summary>
    [ComponentDescriptor(
        DIRModuleComponentIdentifier.ASSESSMENT_NAVIGATOR,
        SecurityObjectType = typeof(AssessmentNavigatorSecurity),
		VerifyControllerData = true,
		VerifyRequestSecurity = true)]
	public partial class HomeController : DevExNavigatorController<DxAssessment, AssessmentNavigatorSecurity>
	{
		public HomeController
        (
            WebMvcModules.Infrastructure.Navigators.MenuCommands.IMenuCommandFactory<DxAssessment> menuCommandFactory,
            DevEx.Configuration.Navigator.INavigationConfigurationGateway configGateway,
            DevEx.Configuration.Infrastructure.IUserDataProvider userDataProvider) : base(menuCommandFactory, configGateway, userDataProvider)
        {
		}

		/// <summary>
		/// Renders main navigator page, id - identifiableString        
		/// </summary>
		/// <returns></returns>
		[ControllerEntryPoint]
		[Selerant.DevEx.Web.Security.AntiCSRFToken(Selerant.DevEx.Web.Security.AntiCSRFTokenVerifyMode.Ignore)]
		public virtual ViewResult Index(string code, string layout = null)
		{
			DxAssessment targetObject = DxObject.ParseIdentifiableString<DxAssessment>(code);
			targetObject.Load();
            var controllerData = new NavigatorControllerData<DxAssessment>(ComponentDescriptorHelper.GetIdentifierForType(GetType()), targetObject, NavigatorModel.Modes.Standard, layout);
            var indexModel = new AssessmentNavigatorIndexModel(GetControllerUrl(), controllerData);
			return Index(indexModel);
		}

        [ExcludeControllerData]
        [AcceptVerbs(HttpVerbs.Get | HttpVerbs.Post)]
        [AntiCSRFToken(AntiCSRFTokenVerifyMode.Auto)]
        public virtual ViewResult IndexEdit(string id, String panelID, String customParams, Boolean isCreating = false, bool saveOnCancel = false)
        {
            DxAssessment targetObject = (DxAssessment)DxObject.ParseIdentifiableString(id);
            targetObject.Load();

            var controllerData = new NavigatorControllerData<DxAssessment>(ComponentDescriptorHelper.GetIdentifierForType(GetType()), targetObject, isCreating ? NavigatorModel.Modes.EditInCreation : NavigatorModel.Modes.Edit, null);

            return RouteViewByRights(() => { return CheckUserHasRightsOnIndexAction(controllerData); },
                () =>
                {
                    var indexModel = new AssessmentNavigatorIndexModel(GetControllerUrl(), controllerData);
                    var indexEditModel = new AssessmentNavigatorIndexEditModel(GetControllerUrl(), indexModel, isCreating);
                    return IndexEdit(indexEditModel, panelID, customParams, isCreating, saveOnCancel);
                },
                String.Format("Index Edit Action on {0}: no rights", this.GetType().Name));
        }

        protected override System.Web.Mvc.ActionResult OnHeaderMenuItemClick(NavigatorControllerData<DxAssessment> controllerData, HtmlMenuItemClickEventArgs param)
        {
            switch (param.ItemKey)
            {
                case NavigatorHeaderModel.MENU_BUTTON_KEY_DELETE:
                    {
                        SecurityVerifier.AssertHasRights(() => GetSecurityObject(controllerData).CanDelete);
                        SetHasVerifiedRequestSecurity();
                        return DefaultExecuteMenuButtonDelete(controllerData);
                    }
            }

            return null;
        }

		/// <summary>
		/// Invokes LCStages managment dialog
		/// </summary>
		[HttpPost]
		[AntiCSRFToken(AntiCSRFTokenVerifyMode.Verify)]
		public virtual DataAjaxResult ManageAssessmentRaiseDialog(NavigatorControllerData<DxAssessment> controllerData, EntityType entityType)
		{
			SecurityVerifier.AssertHasRights(() => GetSecurityObject(controllerData).CanUpdateLcStages);
			SetHasVerifiedRequestSecurity();

			string assessmentIdent = controllerData.TargetObject.IdentifiableString;
			return ManageAssessmentUtility.CreateManageDlgActivityActionResult(Url, controllerData, entityType, assessmentIdent, null);
		}

		#region Private methods - Security

		private bool CheckUserHasRightsOnIndexAction(NavigatorControllerData<DxAssessment> controllerData)
        {
            var security = this.GetSecurityObject(controllerData);
            bool result = SecurityVerifier.CheckRightsForRendering(() => security.CanViewContent);
            SetHasVerifiedRequestSecurity();
            return result;
        }

        #endregion
    }
}