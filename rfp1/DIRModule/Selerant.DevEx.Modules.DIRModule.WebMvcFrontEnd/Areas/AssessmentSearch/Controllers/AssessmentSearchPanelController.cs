using Selerant.DevEx.Modules.DIRModule.BackEnd.AssessmentManagement.BusinessObjects;
using Selerant.DevEx.Modules.DIRModule.WebMvcFrontEnd._ThisProject;
using Selerant.DevEx.Modules.DIRModule.WebMvcFrontEnd.Areas.AssessmentSearch.Security;
using Selerant.DevEx.Web;
using Selerant.DevEx.WebMvcModules.Infrastructure.Searches.Controllers;

namespace Selerant.DevEx.Modules.DIRModule.WebMvcFrontEnd.Areas.AssessmentSearch.Controllers
{
    [ComponentDescriptor(DIRModuleComponentIdentifier.SEARCH_ASSESSMENT, SecurityObjectType = typeof(AssessmentSearchSecurity), VerifyControllerData = true, VerifyRequestSecurity = true)]
    public abstract partial class AssessmentSearchPanelController : DevExSearchPanelController<DxAssessment, AssessmentSearchSecurity>
    {
    }
}