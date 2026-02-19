using Selerant.DevEx.Modules.DIRModule.WebMvcFrontEnd.Areas.DIRAdminTools.Controllers;
using Selerant.DevEx.Modules.DIRModule.WebMvcFrontEnd.Areas.DIRAdminTools.Security;
using Selerant.DevEx.Web;
using Selerant.DevEx.WebMvcModules.Infrastructure.Controls.HtmlGrid;
using System.Collections.Generic;

namespace Selerant.DevEx.Modules.DIRModule.WebMvcFrontEnd.Areas.DIRAdminTools.Converters
{
    [ComponentDescriptor(AssessmentTypesController.ThisComponentIdentifier, SecurityObjectType = typeof(AdminToolsAssessmentTypesSecurity), VerifyControllerData = true, VerifyRequestSecurity = true)]
    public class AssessmentTypesActionsConverter : BaseActionConverter
    {
        protected override void FillActionLinks(HtmlGridActionLinks htmlGridActionLinks, object[] values, object collectionItem)
        {
            var actionLinks = new List<HtmlGridActionLink>();

            if (((AdminToolsAssessmentTypesSecurity) SecurityObject).CanEdit)
            {
                actionLinks.Add(HtmlGridActionLink.New("Activate", Locale.GetString("DIR_Controls", "GridAction_Activate"), Locale.GetString("DIR_AssessmentManager", "GridAction_ActivateTT"),
                    new Dictionary<string, string>
                    {
                        { "id",  (values[0].ToString().Length>0)? values[0].ToString():string.Empty}
                    }));

                actionLinks.Add(HtmlGridActionLink.New("Edit", Locale.GetString("DIR_Controls", "GridAction_Edit"), Locale.GetString("DIR_AssessmentManager", "GridAction_EditTT"),
                    new Dictionary<string, string>
                    {
                        { "id",  (values[0].ToString().Length>0)? values[0].ToString():string.Empty }
                    }));

                actionLinks.Add(HtmlGridActionLink.New("Delete", Locale.GetString("DIR_Controls", "GridAction_Delete"), Locale.GetString("DIR_AssessmentManager", "GridAction_DeleteTT"),
                    new Dictionary<string, string>
                    {
                        { "id",  (values[0].ToString().Length>0)? values[0].ToString():string.Empty }
                    }));
            }

            htmlGridActionLinks.ActionLinks.AddRange(actionLinks);
            htmlGridActionLinks.InlineActionsNumber = 3;
        }
    }
}