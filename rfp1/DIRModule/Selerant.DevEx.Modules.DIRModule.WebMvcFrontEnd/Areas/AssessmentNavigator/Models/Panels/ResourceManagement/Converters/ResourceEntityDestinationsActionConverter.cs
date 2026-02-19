using System.Collections.Generic;
using Selerant.DevEx.BusinessLayer;
using Selerant.DevEx.BusinessLayer.Authorization;
using Selerant.DevEx.Modules.DIRModule.BackEnd.OutputManagement;
using Selerant.DevEx.Modules.DIRModule.WebMvcFrontEnd._ThisProject;
using Selerant.DevEx.Modules.DIRModule.WebMvcFrontEnd.Areas.AssessmentNavigator.Security;
using Selerant.DevEx.Web;
using Selerant.DevEx.WebMvcModules.Infrastructure.Controls.HtmlGrid;

namespace Selerant.DevEx.Modules.DIRModule.WebMvcFrontEnd.Areas.AssessmentNavigator.Models.Panels.ResourceManagement
{
	/// <summary>
	/// Inputs grid Action column converter
	/// </summary>
	[ComponentDescriptor(DIRModuleComponentIdentifier.ASSESSMENT_NAVIGATOR_RESOURCE_MANAGEMENT_PANEL, SecurityObjectType = typeof(ResourceManagementSecurity), VerifyControllerData = true, VerifyRequestSecurity = true)]
	public class ResourceEntityDestinationsActionConverter : BaseActionConverter<ResourceManagementSecurity>
	{
		protected override void FillActionLinks(HtmlGridActionLinks htmlGridActionLinks, object[] values, object collectionItem)
		{
			var actionLinks = new List<HtmlGridActionLink>();
			string identifiableString = (string)values[0];

			DxObject obj = DxObject.ParseIdentifiableString(identifiableString);

			if (SecurityVerifier.CheckRightsForRendering(() => SecurityObject.CanEditOrDelete) && !(obj is DxOutputCategory))
			{
				actionLinks.Add(HtmlGridActionLink.New("Edit", Locale.GetString("DIR_Controls", "GridAction_Edit"), Locale.GetString("DIR_Controls", "GridAction_EditTT"),
				new Dictionary<string, string>
				{
				{ "id",  identifiableString }
				}));
			}

			htmlGridActionLinks.ActionLinks.AddRange(actionLinks);
			htmlGridActionLinks.InlineActionsNumber = 1;
		}
	}
}