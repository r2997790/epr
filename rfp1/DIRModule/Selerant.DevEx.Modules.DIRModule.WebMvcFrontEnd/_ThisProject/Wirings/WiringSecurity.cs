using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Autofac;
using Selerant.DevEx.Infrastructure.Security;
using Selerant.DevEx.Modules.DIRModule.WebMvcFrontEnd.Areas.AssessmentNavigator.Models.ObjectCreation;
using Selerant.DevEx.Modules.DIRModule.WebMvcFrontEnd.Areas.AssessmentNavigator.Security;
using Selerant.DevEx.Modules.DIRModule.WebMvcFrontEnd.Areas.DIRAdminTools.Controllers;
using Selerant.DevEx.Modules.DIRModule.WebMvcFrontEnd.Areas.DIRAdminTools.Security;

namespace Selerant.DevEx.Modules.DIRModule.WebMvcFrontEnd._ThisProject.Wirings
{
	public class WiringSecurity
	{
		public void Wiring(ContainerBuilder builder)
		{
            #region Admin Tools

            SecurityObjectProvider.RegisterSecurityObject(builder, AssessmentTypesController.ThisComponentIdentifier, u => new AdminToolsAssessmentTypesSecurity(u));

            #endregion

			#region Create Object

			SecurityObjectProvider.RegisterSecurityObjectForObjectCreation(builder, DIRModuleComponentIdentifier.CREATE_ASSESSMENT, parms => new CreateAssessmentObjectCreationSecurity(parms));

            #endregion
        }
    }
}