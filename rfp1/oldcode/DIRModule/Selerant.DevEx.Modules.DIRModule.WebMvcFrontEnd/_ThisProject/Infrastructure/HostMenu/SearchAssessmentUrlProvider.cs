using System.Collections.Generic;
using Selerant.DevEx.Modules.DIRModule.WebMvcFrontEnd.Areas.AssessmentSearch.Security;
using Selerant.DevEx.Web;
using Selerant.DevEx.Web.Infrastructure;
using Selerant.DevEx.WebMvcModules.Infrastructure.Searches.Models;

namespace Selerant.DevEx.Modules.DIRModule.WebMvcFrontEnd._ThisProject.Infrastructure.HostMenu
{
	public class SearchAssessmentUrlProvider : ModuleComponentUrlProvider
	{
		#region Constructors

		public SearchAssessmentUrlProvider()
			: base($"Search|{DIRModuleComponentIdentifier.SEARCH_ASSESSMENT}", MVC_DIR.AssessmentSearch.Home.Index())
		{
		}

		#endregion

		#region Methods overrides

		protected override bool CanAccessComponent(BaseControllerData controllerData)
		{
			return ((AssessmentSearchSecurity)controllerData.SecurityObject).CanUseStandard;
		}

		protected override BaseControllerData CreateControllerData(Dictionary<string, string> parameters)
		{
            var controllerData = new SearchControllerData(DIRModuleComponentIdentifier.SEARCH_ASSESSMENT, parameters);

            return controllerData;
		}

		#endregion
	}
}