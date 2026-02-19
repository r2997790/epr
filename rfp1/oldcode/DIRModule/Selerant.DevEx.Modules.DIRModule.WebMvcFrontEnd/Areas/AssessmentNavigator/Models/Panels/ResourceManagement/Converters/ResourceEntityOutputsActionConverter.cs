using Selerant.DevEx.BusinessLayer;
using Selerant.DevEx.BusinessLayer.Authorization;
using Selerant.DevEx.Modules.DIRModule.BackEnd.OutputManagement.GridItem;
using Selerant.DevEx.Modules.DIRModule.WebMvcFrontEnd._ThisProject;
using Selerant.DevEx.Modules.DIRModule.WebMvcFrontEnd.Areas.AssessmentNavigator.Security;
using Selerant.DevEx.Web;
using Selerant.DevEx.WebMvcModules.Infrastructure.Controls.HtmlGrid;
using System.Collections.Generic;

namespace Selerant.DevEx.Modules.DIRModule.WebMvcFrontEnd.Areas.AssessmentNavigator.Models.Panels.ResourceManagement.Converters
{
	[ComponentDescriptor(DIRModuleComponentIdentifier.ASSESSMENT_NAVIGATOR_RESOURCE_MANAGEMENT_PANEL, SecurityObjectType = typeof(ResourceManagementSecurity), VerifyControllerData = true, VerifyRequestSecurity = true)]
	public class ResourceEntityOutputsActionConverter : BaseActionConverter<ResourceManagementSecurity>
    {
        private const string NON_EDITABLE_DESTINATION = "ENVIRONMENT_LOSS";

        protected override void FillActionLinks(HtmlGridActionLinks htmlGridActionLinks, object[] values, object collectionItem)
        {
            string identifiableString = values[0] as string;
            string destinationCode = values[1] as string;

            DxObject obj = DxObject.ParseIdentifiableString(identifiableString);

            var actionLinks = new List<HtmlGridActionLink>();

            if (obj is DxOutputGridItem)
            {
                if (destinationCode != NON_EDITABLE_DESTINATION && SecurityVerifier.CheckRightsForRendering(() => SecurityObject.CanEditOrDelete))
                {
                    actionLinks.Add(HtmlGridActionLink.New("Edit", Locale.GetString("DIR_Controls", "GridAction_Edit"), Locale.GetString("DIR_Controls", "GridAction_EditTT"),
                    new Dictionary<string, string>
                    {
                        { "id",  identifiableString }
                    }));
                }
            }

            htmlGridActionLinks.ActionLinks.AddRange(actionLinks);
            htmlGridActionLinks.InlineActionsNumber = 2;
        }
    }
}