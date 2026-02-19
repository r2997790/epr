using System;
using System.Web.Mvc;
using Selerant.ApplicationBlocks.Html;
using Selerant.DevEx.BusinessLayer;
using Selerant.DevEx.BusinessLayer.IconManagement;
using Selerant.DevEx.Modules.DIRModule.WebMvcFrontEnd._ThisProject;
using Selerant.DevEx.Modules.DIRModule.WebMvcFrontEnd.Areas.AssessmentNavigator.Security;
using Selerant.DevEx.Web;
using Selerant.DevEx.WebMvcModules.Infrastructure.Controls.HtmlGrid;

namespace Selerant.DevEx.Modules.DIRModule.WebMvcFrontEnd.Areas.AssessmentNavigator.Models.Panels.ResourceManagement.Converters
{
	[ComponentDescriptor(DIRModuleComponentIdentifier.ASSESSMENT_NAVIGATOR_RESOURCE_MANAGEMENT_PANEL,
		VerifyControllerData = true,
		VerifyRequestSecurity = true,
		SecurityObjectType = typeof(ResourceManagementSecurity))]
	public class CategoryMaterialReferenceValueConverter : HtmlGridValueConverter
	{
		protected override object ConvertValues(object[] values, object collectionItem)
		{
			if (values.Length > 1)
			{
				string categoryMaterialIdentifiableString = values[0] as string;
				var cellText = values[1] as string;

				// Category - row (parent) OR Total - row
				if (string.IsNullOrEmpty(categoryMaterialIdentifiableString))
					return cellText;

				DxObject obj = DxObject.ParseIdentifiableString(categoryMaterialIdentifiableString);

				// Input - row (child)
				if (obj is DxMaterial)
				{
					if (CanUserGoToNavigator(obj))
					{
						HtmlGridLink htmlLink = BuildLinkOpenHostPageTab(UrlHelper.Action(MVC.MaterialNavigator.Home.Index(categoryMaterialIdentifiableString)), cellText).SetImageUrl(UrlHelper.Content(IconManager.Default.GetImageUrlByEntity(obj, IconLogicalSize.Small)));
						htmlLink.SetDataValue("identifiable", categoryMaterialIdentifiableString);
						return RenderLink(htmlLink);
					}
					else
					{
						return new HtmlPlaceHolderWriter()
							.AppendWriter(new TagWriter("img").Attribute("src", UrlHelper.Content(IconManager.Default.GetImageUrlByEntity(obj, IconLogicalSize.Small))))
							.AppendText(cellText)
							.Write();
					}
				}
			}
			else
				throw new ArgumentException("Not enough supplied values to converter", nameof(values));

			return string.Empty;
		}
	}
}