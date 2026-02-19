using System;
using System.Collections.Generic;
using System.Linq;
using Selerant.DevEx.BusinessLayer.Authorization;
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
	public class ResourceEntityActionConverter : BaseActionConverter<ResourceManagementSecurity>
	{
		protected override void FillActionLinks(HtmlGridActionLinks htmlGridActionLinks, object[] values, object collectionItem)
		{
			var actionLinks = new List<HtmlGridActionLink>();

			if (SecurityVerifier.CheckRightsForRendering(() => SecurityObject.CanEditOrDelete))
			{
				string identifiableString = (string)values[0];

				if (identifiableString.StartsWith("DxInputCategory"))
				{
					actionLinks.Add(HtmlGridActionLink.New("DeleteInputCategory", Locale.GetString("DIR_Controls", "GridAction_Delete"), Locale.GetString("DIR_Controls", "GridAction_DeleteTT"),
						new Dictionary<string, string>
						{
							{ "id",  identifiableString }
						}));
				}
				else if (identifiableString.StartsWith("DxBusinessCostGridItem(") )
				{
					actionLinks.Add(HtmlGridActionLink.New("Edit", Locale.GetString("DIR_Controls", "GridAction_Edit"), Locale.GetString("DIR_Controls", "GridAction_EditTT"),
						new Dictionary<string, string> { { "id", identifiableString } }));

					bool isCarriedOverCost = (bool)values[1];
					if (!isCarriedOverCost)
					{
						actionLinks.Add(HtmlGridActionLink.New("Delete", Locale.GetString("DIR_Controls", "GridAction_Delete"), Locale.GetString("DIR_Controls", "GridAction_DeleteTT"),
							new Dictionary<string, string> { { "id", identifiableString } }));
					}
				}
				else if (identifiableString.StartsWith("Dx"))
				{
					actionLinks.Add(HtmlGridActionLink.New("Edit", Locale.GetString("DIR_Controls", "GridAction_Edit"), Locale.GetString("DIR_Controls", "GridAction_EditTT"),
						new Dictionary<string, string>
						{
								{ "id", identifiableString }
						}));

					actionLinks.Add(HtmlGridActionLink.New("Delete", Locale.GetString("DIR_Controls", "GridAction_Delete"), Locale.GetString("DIR_Controls", "GridAction_DeleteTT"),
						new Dictionary<string, string>
						{
								{ "id", identifiableString }
						}));
				}
				else if (identifiableString != InputRowItem.TOTALROW_ID)
					throw new ArgumentException("Use identifiablestring for manipulation", nameof(values));
			}

			htmlGridActionLinks.ActionLinks.AddRange(actionLinks);
			htmlGridActionLinks.InlineActionsNumber = 2;
		}
	}
}