using Selerant.DevEx.BusinessLayer;
using Selerant.DevEx.Infrastructure.SearchManager.BaseClasses;
using Selerant.DevEx.Modules.DIRModule.BackEnd.AssessmentManagement.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Selerant.DevEx.Modules.DIRModule.WebMvcFrontEnd.Areas.AssessmentSearch.Controllers
{
    public class AssessmentSearchManagerHelperExtension : SearchManagerHelperExtension
    {
        public override DxAttributeScope Scope => new DxAssessmentAttributeScope();
    }
}