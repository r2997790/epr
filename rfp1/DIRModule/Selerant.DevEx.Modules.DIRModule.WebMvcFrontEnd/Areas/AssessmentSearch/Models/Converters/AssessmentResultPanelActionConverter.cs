using Selerant.DevEx.Modules.DIRModule.BackEnd.AssessmentManagement.BusinessObjects;
using Selerant.DevEx.Modules.DIRModule.WebMvcFrontEnd._ThisProject;
using Selerant.DevEx.Modules.DIRModule.WebMvcFrontEnd.Areas.AssessmentSearch.Security;
using Selerant.DevEx.Web;
using Selerant.DevEx.WebMvcModules.Infrastructure.Searches.Models;
using Selerant.DevEx.WebMvcModules.Infrastructure.Searches.Models.Converters;

namespace Selerant.DevEx.Modules.DIRModule.WebMvcFrontEnd.Areas.AssessmentSearch.Models.Converters
{
	[ComponentDescriptor(new string[] { DIRModuleComponentIdentifier.SEARCH_ASSESSMENT },
		VerifyControllerData = true,
		VerifyRequestSecurity = true,
		SecurityObjectType = typeof(AssessmentSearchSecurity))]
	public class AssessmentResultPanelActionConverter : SearchResultBaseActionConverter
	{
		protected override HtmlGridActionLink InnerBuildActionLink(BuildActionLinkData buildActionLinkData)
		{
			var assessment = (buildActionLinkData.CollectionItem as SearchResultGridDataItem).TargetObject as DxAssessment;
			DxAssessment.AssessmentStatus status = assessment.Status;

			string action = buildActionLinkData.Action.Name.ToUpper(System.Globalization.CultureInfo.InvariantCulture);

			if ((action == "OPEN" || action == "ADDRECENT") && status != DxAssessment.AssessmentStatus.DRAFT)
			{
				return base.InnerBuildActionLink(buildActionLinkData);
			}
			else if (action == "CONTINUE" && status == DxAssessment.AssessmentStatus.DRAFT)
			{
				return base.InnerBuildActionLink(buildActionLinkData);
			}

			return null;
		}
	}
}