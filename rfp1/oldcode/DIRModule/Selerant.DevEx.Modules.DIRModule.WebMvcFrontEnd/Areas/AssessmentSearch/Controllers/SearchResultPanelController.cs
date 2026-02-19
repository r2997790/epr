using Selerant.DevEx.Modules.DIRModule.BackEnd.AssessmentManagement.BusinessObjects;
using Selerant.DevEx.WebMvcModules.Infrastructure.Searches.Controllers;
using Selerant.DevEx.WebMvcModules.Infrastructure.Searches.Models;
using Selerant.DevEx.Modules.DIRModule.WebMvcFrontEnd.Areas.AssessmentSearch.Models.Panels.Result;
using Selerant.DevEx.Web;
using Selerant.DevEx.WebMvcModules.Infrastructure.Searches.Helpers;
using Selerant.DevEx.Modules.DIRModule.BackEnd.AssessmentManagement.Attributes;
using Selerant.DevEx.Modules.DIRModule.WebMvcFrontEnd.Areas.AssessmentSearch.Security;
using Selerant.DevEx.Modules.DIRModule.WebMvcFrontEnd._ThisProject;
using Selerant.DevEx.WebMvcModules.Infrastructure.Base;
using Selerant.DevEx.Web.Security;
using Selerant.DevEx.BusinessLayer.Authorization;
using Selerant.DevEx.CommonComponents;
using System.Web.Mvc;
using Selerant.DevEx.Modules.DIRModule.Configuration.Resources;

namespace Selerant.DevEx.Modules.DIRModule.WebMvcFrontEnd.Areas.AssessmentSearch.Controllers
{
    [ComponentDescriptor(DIRModuleComponentIdentifier.SEARCH_ASSESSMENT, SecurityObjectType = typeof(AssessmentSearchSecurity), VerifyControllerData = true, VerifyRequestSecurity = true)]
    [SearchPanel("ASSESSMENTSearchResult", DxAssessmentAttributeScope.NAME)]
    public partial class SearchResultPanelController : SearchResultPanelBaseController<DxAssessment, AssessmentSearchSecurity>
    {
        protected override DevExSearchPanelIndexModel BuildIndexModel(SearchControllerData data, ISearchController searchController, string nodeId)
        {
            return new SearchResultIndexModel(GetControllerUrl(), data, searchController, nodeId);
        }

		[HttpPost]
		[AntiCSRFToken(AntiCSRFTokenVerifyMode.Verify)]
		public virtual JSActivityAjaxResult ContinueDrafAssessment(SearchControllerData data, string identifiableString)
		{
			SecurityVerifier.AssertHasRights(() => GetSecurityObject(data).CanCreateAssessment);
			SetHasVerifiedRequestSecurity();

			var activityAssessmentBussinessDlg = new JSOpenDialogActivity()
			{
				Url = Url.Action(MVC_DIR.AssessmentNavigator.CreateAssessmentDialog.BusinessDataDialogIndex(data, identifiableString, false)),
				Width = short.MaxValue,
				Height = short.MaxValue,
				IsResizable = false
			}
			.SetCaption(Locale.GetString(ResourceFiles.AssessmentManager, "ANNNavTabResourceManagement"));

			return new JSActivityAjaxResult(activityAssessmentBussinessDlg);
		}
	}
}