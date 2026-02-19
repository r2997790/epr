using Selerant.DevEx.BusinessLayer;
using Selerant.DevEx.BusinessLayer.Authorization;
using Selerant.DevEx.Infrastructure.Modules.AdminTools.Home;
using Selerant.DevEx.Modules.DIRModule.WebMvcFrontEnd.Areas.DIRAdminTools.Models;
using Selerant.DevEx.Modules.DIRModule.WebMvcFrontEnd.Areas.DIRAdminTools.Security;
using Selerant.DevEx.Web.Security;
using Selerant.DevEx.WebMvcModules.Areas.AdminTools.Views.Home;
using Selerant.DevEx.WebPages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Selerant.DevEx.Modules.DIRModule.WebMvcFrontEnd._ThisProject.AdminTools
{
    public class DIRAdminToolsRegistry : IRegisterAdminToolsButtons
    {

        public void RegisterAdminToolsButtons(AdminToolsUI admin)
        {
			//Temporary DIRECT Tool Management should be hidden 
			//DIRECT
			//Func<AdminGroup> createDIRGroup = () => new AdminGroup(Locale.GetString("DIR_AdminTools", "ATDIRToolManagement"), "DIRSection", "left");
			//var group = GetGroup("DIRSection", admin, createDIRGroup);

			//group.AddItem(new AdminItem("AssessmentTypes")
			//{
			//AutomationId = string.Format("{0}_{1}", group.Id, "AssessmentTypes"),
			//URL = CurrentRequestData.Instance.MvcUrlHelper.Action(MVC_DIR.DIRAdminTools.AssessmentTypes.Index()),
			//CommonData = new AssessmentTypesCommonData(),
			//CanUse = CalculateUserRights(new AdminToolsAssessmentTypesSecurity(DxUser.CurrentUser))
			//});
		}

		#region Methods

		private AdminGroup GetGroup(string id, AdminToolsUI admin, Func<AdminGroup> CreateGroup)
        {
            if (!admin.GetGroups().Any(x => x.Id == id))
                admin.AddGroup(CreateGroup());

            var adminGroup = admin.GetGroups().Single(x => x.Id == id);

            return adminGroup;
        }

        private Boolean CalculateUserRights(AdminToolSecurity security)
        {
            return SecurityVerifier.CheckRightsForRendering(() => security.CanShowContent);
        }

        #endregion
    }
}