using System;
using System.Collections.Generic;
using System.Linq;
using Selerant.ApplicationBlocks.Html;
using Selerant.DevEx.BusinessLayer;
using Selerant.DevEx.BusinessLayer.IconManagement;
using Selerant.DevEx.Modules.DIRModule.BackEnd.AssessmentManagement.BusinessObjects;
using Selerant.DevEx.Modules.DIRModule.WebMvcFrontEnd._ThisProject;
using Selerant.DevEx.Modules.DIRModule.WebMvcFrontEnd.Areas.AssessmentSearch.Security;
using Selerant.DevEx.Web;
using Selerant.DevEx.WebMvcModules.Infrastructure.Controls.HtmlGrid;
using Selerant.DevEx.WebMvcModules.Infrastructure.Searches.Models;

namespace Selerant.DevEx.Modules.DIRModule.WebMvcFrontEnd.Areas.AssessmentSearch.Models.Converters
{
	[ComponentDescriptor(
		new string[] {
				DIRModuleComponentIdentifier.SEARCH_ASSESSMENT
		},
		VerifyControllerData = true,
		VerifyRequestSecurity = true,
		SecurityObjectType = typeof(AssessmentSearchSecurity))]
	public class AssessmentSearchResultPanelReferenceConverter : HtmlGridValueConverter
	{
		public const string IS_DIALOG = "IsDialog";

		protected bool isDialog;

		protected override void InnerInitialize()
		{
			base.InnerInitialize();

			isDialog = (bool)HelperParameters[IS_DIALOG];
		}

		protected override object ConvertValues(object[] values, object collectionItem)
		{
			if (values == null)
				throw new NullReferenceException("You should pass in this method any data.");

			if (values.Length > 1 && !string.IsNullOrWhiteSpace(values[1] as string))
			{
				var obj = DxObject.ParseIdentifiableString((string)values[0]);

				if (isDialog)
				{
					return RenderLink(new HtmlGridLink(values[1]?.ToString(), null, null, new Dictionary<string, string>()
					{
						{"onclick", "CloseDialogAndReturnData"},
						{"id", values[0]?.ToString()}
					}));
				}

				// Full search page
				if (!CanUserGoToNavigator(obj))
				{
					return RenderLink(BuildLinkAlertHasNoRightsOnPage(values[1]?.ToString())
										.SetImageUrl(UrlHelper.Content(IconManager.Default.GetImageUrlByEntity(obj, IconLogicalSize.Small)))
										.SetKey("OPEN"));
				}
				else
				{
					var assessment = (collectionItem as SearchResultGridDataItem).TargetObject as DxAssessment;
					DxAssessment.AssessmentStatus status = assessment.Status;

					if (status == DxAssessment.AssessmentStatus.DRAFT)
					{
						return new HtmlPlaceHolderWriter()
							.AppendWriter(new TagWriter("img").Attribute("src", UrlHelper.Content(IconManager.Default.GetImageUrlByEntity(obj, IconLogicalSize.Small))))
							.AppendText(assessment.Description)
							.Write();
					}
					else
					{
						return RenderLink(BuildLinkOpenHostPageTab(UrlHelper.Content(BusinessLayer.Navigation.NavigationUtilities.GetNavigatorUrl((INavigableObject)obj)), values[1]?.ToString())
								.SetImageUrl(UrlHelper.Content(IconManager.Default.GetImageUrlByEntity(obj, IconLogicalSize.Small)))
								.SetKey("OPEN"));
					}
				}

			}
			else
				return string.Empty;
		}

	}
}