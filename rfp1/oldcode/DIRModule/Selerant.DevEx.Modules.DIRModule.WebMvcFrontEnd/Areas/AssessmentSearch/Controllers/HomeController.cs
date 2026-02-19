using Selerant.DevEx.BusinessLayer;
using Selerant.DevEx.BusinessLayer.IconManagement;
using Selerant.DevEx.Modules.DIRModule.BackEnd.AssessmentManagement.Attributes;
using Selerant.DevEx.Modules.DIRModule.BackEnd.AssessmentManagement.BusinessObjects;
using Selerant.DevEx.Modules.DIRModule.Configuration.Resources;
using Selerant.DevEx.Modules.DIRModule.ModuleBlocks._ThisProject;
using Selerant.DevEx.Modules.DIRModule.WebMvcFrontEnd._ThisProject;
using Selerant.DevEx.Modules.DIRModule.WebMvcFrontEnd.Areas.AssessmentSearch.Helpers;
using Selerant.DevEx.Modules.DIRModule.WebMvcFrontEnd.Areas.AssessmentSearch.Models.Home;
using Selerant.DevEx.Modules.DIRModule.WebMvcFrontEnd.Areas.AssessmentSearch.Security;
using Selerant.DevEx.Web;
using Selerant.DevEx.WebMvcModules.Infrastructure.Searches.Controllers;
using Selerant.DevEx.WebMvcModules.Infrastructure.Searches.Helpers;
using Selerant.DevEx.WebMvcModules.Infrastructure.Searches.Models;
using Selerant.DevEx.Web.Security;
using System;
using System.Web.Mvc;

namespace Selerant.DevEx.Modules.DIRModule.WebMvcFrontEnd.Areas.AssessmentSearch.Controllers
{
    internal sealed class SearchStaticUIDataProvider : BaseSearchStaticUIDataProvider
    {
        public override string ProvidedBy => DIRModuleInfo.MODULE_CODE;

        public override SearchStaticUIData Provide()
        {
            var structure = new SearchStaticUIData();
            structure.ProvidedBy = DIRModuleInfo.MODULE_CODE;
            structure.AttributeScope = new DxAssessmentAttributeScope();
            structure.ComponentIdentifier = new ComponentIdentifier(DIRModuleComponentIdentifier.SEARCH_ASSESSMENT);
            structure.HomeControllerType = typeof(HomeController);

            structure.Configuration = new SearchPageConfiguration
            {
                CaptionIcon = IconManager.Default.GetImageUrlByType(typeof(DxAssessment), IconLogicalSize.Small),
				CaptionText = Locale.GetString(ResourceFiles.AssessmentManager, "DIR_NavAssessmentType_SearchPanel"),
                NewButtonImage = String.Empty,
                NewButtonText = String.Empty,
                NewButtonToolTip = String.Empty,
                NewButtonVisible = false
            };

            return structure;
        }
    }

    [ComponentDescriptor(DIRModuleComponentIdentifier.SEARCH_ASSESSMENT, SecurityObjectType = typeof(AssessmentSearchSecurity), VerifyControllerData = true, VerifyRequestSecurity = true)]
    public partial class HomeController : DevExSearchController<DxAssessment>
    {
        [ControllerEntryPoint]
        [HttpGet]
        [AntiCSRFToken(AntiCSRFTokenVerifyMode.Ignore)]
        public virtual ViewResult Index()
        {
            return base.Index();
        }

        protected override DevExSearchIndexModel<DxAssessment> BuildIndexModel(DevExSearchSettings settings)
        {
            return new AssessmentIndexModel(GetControllerUrl(), NewSearchControllerData(), SearchController, settings);
        }

        protected override SearchHelper CreateSearchHelper(DxQuery query, string searchOptionId)
        {
            return new AssessmentBasicSearchHelper(query, RefinementOperator, Scope, searchOptionId);
        }
    }
}