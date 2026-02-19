using Selerant.DevEx.BusinessLayer.Authorization;
using Selerant.DevEx.Modules.DIRModule.BackEnd.AssessmentManagement.BusinessObjects;
using Selerant.DevEx.Modules.DIRModule.WebMvcFrontEnd._ThisProject;
using Selerant.DevEx.Modules.DIRModule.WebMvcFrontEnd.Areas.AssessmentNavigator.Models.Copy;
using Selerant.DevEx.Modules.DIRModule.WebMvcFrontEnd.Areas.AssessmentNavigator.Security;
using Selerant.DevEx.Web;
using Selerant.DevEx.Web.Security;
using Selerant.DevEx.WebMvcModules.Infrastructure.Base;
using Selerant.DevEx.WebMvcModules.Infrastructure.Controllers;
using System.Web.Mvc;

namespace Selerant.DevEx.Modules.DIRModule.WebMvcFrontEnd.Areas.AssessmentNavigator.Controllers
{
	[ComponentDescriptor
		(
			DIRModuleComponentIdentifier.ASSESSMENT_NAVIGATOR,
			SecurityObjectType = typeof(AssessmentNavigatorSecurity),
			VerifyControllerData = true,
			VerifyRequestSecurity = true
		)
	]
	public partial class CopyAssessmentController : DevExBaseController
	{
		private AssessmentNavigatorSecurity GetSecurity(ViewControlControllerData controllerData)
		{
			return controllerData.SecurityObject as AssessmentNavigatorSecurity;
		}

		[AcceptVerbs(HttpVerbs.Get)]
		[AntiCSRFToken(AntiCSRFTokenVerifyMode.Ignore)]
		public virtual ActionResult CopyAssessment(ViewControlControllerData controllerData, string identifiableString)
		{
			SecurityVerifier.AssertHasRights(() => GetSecurity(controllerData).CanCopy);
			SetHasVerifiedRequestSecurity();

			DxAssessment sourceAssessment = DxAssessment.ParseIdentifiableString<DxAssessment>(identifiableString);
			sourceAssessment.Load();

			CopyModel model = new CopyModel(GetControllerUrl(), controllerData, sourceAssessment);

			return View(model);
		}

		[AcceptVerbs(HttpVerbs.Post)]
		[AntiCSRFToken(AntiCSRFTokenVerifyMode.Verify)]
		public virtual ActionResult CopyAssessment(ViewControlControllerData controllerData, CopyAssessmentDataJson assessmentData)
		{
			SecurityVerifier.AssertHasRights(() => GetSecurity(controllerData).CanCopy);
			SetHasVerifiedRequestSecurity();

			DxAssessment sourceAssessment = ((AssessmentNavigatorSecurity)controllerData.SecurityObject).TargetObject;
			CopyModel model = new CopyModel(sourceAssessment);

			DxAssessment newAssessment = model.Copy(assessmentData);

			return new DataAjaxResult(new
			{
				Success = newAssessment != null ? true : false,
				Url = BusinessLayer.Navigation.NavigationUtilities.GetNavigatorUrl(newAssessment)
			});
		}
	}
}