using System;
using System.Web.Mvc;
using Selerant.ApplicationBlocks.Html;
using Selerant.DevEx.BusinessLayer;
using Selerant.DevEx.BusinessLayer.IconManagement;
using Selerant.DevEx.Modules.DIRModule.BackEnd.OutputManagement;
using Selerant.DevEx.Modules.DIRModule.Configuration.Resources;
using Selerant.DevEx.Modules.DIRModule.WebMvcFrontEnd._ThisProject;
using Selerant.DevEx.Modules.DIRModule.WebMvcFrontEnd.Areas.AssessmentNavigator.Security;
using Selerant.DevEx.Modules.DIRModule.WebMvcFrontEnd.Helpers;
using Selerant.DevEx.Web;
using Selerant.DevEx.WebMvcModules.Infrastructure.Controls.HtmlGrid;
using System.Linq;

namespace Selerant.DevEx.Modules.DIRModule.WebMvcFrontEnd.Areas.AssessmentNavigator.Models.Panels.ResourceManagement.Converters
{
	[ComponentDescriptor(DIRModuleComponentIdentifier.ASSESSMENT_NAVIGATOR_RESOURCE_MANAGEMENT_PANEL,
		VerifyControllerData = true,
		VerifyRequestSecurity = true,
		SecurityObjectType = typeof(ResourceManagementSecurity))]
	public class CategoryDestinationsReferenceValueConverter : HtmlGridValueConverter
	{
		protected override object ConvertValues(object[] values, object collectionItem)
		{
			if (values.Length > 1)
			{
				string categoryMaterialIdentifiableString = values[0] as string;
				var cellText = values[1] as string;
                var productSource = values.ElementAtOrDefault(2) as string;

				DxObject obj = DxObject.ParseIdentifiableString(categoryMaterialIdentifiableString);

				if (obj is DxMaterial)
				{
                    string gridCellText = "";
                    string gridCellTooltip = "";

                    if (!string.IsNullOrEmpty(productSource))
                    {
                        string productSourceShortened = GridHelpers.GetProductSourceShortened(productSource);
                        gridCellText = $"({productSourceShortened}) {cellText}";
                        gridCellTooltip = GridHelpers.GetProductSourceTooltipText(productSourceShortened);
                    }
                    else
                    {
                        gridCellText = gridCellTooltip = cellText;
                    }

					if (CanUserGoToNavigator(obj))
					{
						HtmlGridLink htmlLink = BuildLinkOpenHostPageTab(UrlHelper.Action(MVC.MaterialNavigator.Home.Index(categoryMaterialIdentifiableString)), gridCellText).SetImageUrl(UrlHelper.Content(IconManager.Default.GetImageUrlByEntity(obj, IconLogicalSize.Small)));
						htmlLink.SetDataValue("identifiable", categoryMaterialIdentifiableString);
                        htmlLink.SetToolTip(gridCellTooltip);
						return RenderLink(htmlLink);
					}
					else
					{
						return new HtmlPlaceHolderWriter()
							.AppendWriter(new TagWriter("img").Attribute("src", UrlHelper.Content(IconManager.Default.GetImageUrlByEntity(obj, IconLogicalSize.Small))))
							.AppendText(gridCellText)
							.Write();
					}
				}
				else if (obj is DxOutputCategory)
				{
					return new HtmlPlaceHolderWriter()
							.AppendText(cellText)
							.Write();
				}
			}
			else
				throw new ArgumentException("Not enough supplied values to converter", nameof(values));

			return string.Empty;
		}
	}
}