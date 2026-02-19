using Selerant.DevEx.Infrastructure.Modules;
using Selerant.DevEx.Modules.DIRModule.ModuleBlocks._ThisProject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Selerant.DevEx.Modules.DIRModule.WebMvcFrontEnd.Areas.AssessmentSearch
{
    public class AssessmentSearchAreaRegistration : ModuleAreaRegistration
    {
        public override string ModuleName => DIRModuleInfo.Instance.ModuleName;

        public override string AreaName => "AssessmentSearch";
    }
}