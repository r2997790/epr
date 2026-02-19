using Selerant.DevEx.BusinessLayer;
using Selerant.DevEx.Modules.DIRModule.BackEnd.AssessmentTypeManagement;
using Selerant.DevEx.Modules.DIRModule.Configuration.Resources;
using Selerant.DevEx.Modules.DIRModule.ModuleBlocks._ThisProject;
using Selerant.DevEx.Modules.DIRModule.WebMvcFrontEnd.Areas.DIRAdminTools.Security;
using Selerant.DevEx.WebMvcModules.Areas.AdminTools.Models;
using Selerant.DevEx.WebMvcModules.Helpers;
using Selerant.DevEx.WebMvcModules.Helpers.jqGrid;
using Selerant.DevEx.WebMvcModules.Infrastructure.Base;
using Selerant.DevEx.WebMvcModules.Infrastructure.Controls;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Selerant.DevEx.Modules.DIRModule.WebMvcFrontEnd.Areas.DIRAdminTools.Models
{
    public class AssessmentTypesIndexModel : AdminToolIndexModel<AdminToolsAssessmentTypesSecurity>
    {
        #region Classes

        public class AssessmentTypesItem
        {
            #region jqTree members

            public String Parent { get; set; }
            public int Level { get; set; }
            public bool IsLeaf { get; set; }
            public bool Expanded { get; set; }

            #endregion

            #region Properties

            public string IdentifiableString { get; set; }
            public string Code { get; set; }
            public string Description { get; set; }
            public string Active { get; set; }
            public DxAssessmentType TargetObject { get; set; }

            #endregion
        }

        #endregion

        #region Constants

        public readonly string GRID_ID = $"{DIRModuleInfo.Instance.ModuleName}_AdminToolsAssessmentTypes";
		public readonly string GRID_LOGIC_ID = $"{DIRModuleInfo.Instance.ModuleName}_AdminToolsAssessmentTypes";
		private const int GRID_PAGE_SIZE = 10;

        #endregion

        #region Properties		

        public int PageSize { get { return GRID_PAGE_SIZE; } }

        #endregion

        #region Constructor

        public AssessmentTypesIndexModel(String controllerUrl, AdminToolControllerData controllerData)
            : base(controllerUrl, controllerData)
        {
            GridId = GRID_ID;
        }

        #endregion

        #region Methods

        #endregion

        #region Lifecycle

        protected override void InnerInitializeForView()
        {
            base.InnerInitializeForView();
            ShowEditSettingsButton = false;
            ShowRefreshButton = false;
            ShowMenu = true;

            CommonData = new AssessmentTypesCommonData();

            if (SecurityObject.CanAdd)
            {
                var menuItemMvc = new MenuItem()
                    .SetKey("AddAssessmentType")
                    .SetCaption(Locale.GetString(Locale.ModControls, "NewButtonText"))
                    .SetToolTip(Locale.GetString(Locale.ModControls, "GridNavAdd"))
                    .SetImageUrl("DxNew.png");

                HtmlMenu.AddMenuItem(menuItemMvc);
            }

            HtmlMenu.AddMenuItem(new MenuItem().AsFilterButton(HtmlMenuHelper.FilterBtnResourseArea.AdminTools));

            SetJsCustomData("gridID", GRID_ID);
        }

        protected override void InitializeResources(List<ControlResource> resources)
        {
            base.InitializeResources(resources);

            resources.Add(new ControlJsFile(DIRLinks.Areas.DIRAdminTools.Views.AssessmentTypes.Index_ts));
            resources.Add(new ControlJsFile(DIRLinks.Areas.DIRAdminTools.Views.AssessmentTypes.Index_css));
            JsType = "DX.DIRModule.DIRAdminTools.AssessmentTypes.Index";
        }

        public IQueryable<AssessmentTypesItem> GetGridData(GridSettings clientGridRequest, out int totalRecords, out String gridId)
        {
            gridId = GridId;

            DxAssessmentTypeCollection assessmentTypeCollection = !DxUser.CurrentUser.IsExternal ? new DxAssessmentTypeCollection(true) : DxAssessmentTypeCollection.NewByPartnerOrgCode(DxUser.CurrentUser.PartnerOrganizationCode);

            IQueryable<DxAssessmentType> dxAssessmentTypesCollection = assessmentTypeCollection.AsQueryable()
                    .Sort(clientGridRequest, gridId)
                    .Where(x => !x.Code.StartsWith("$TEMP"))
                    .ApplyDataBaseFilter(clientGridRequest);

            totalRecords = dxAssessmentTypesCollection.Count();

            var assessmentTypes = new List<AssessmentTypesItem>();

            foreach (var dxAssmType in dxAssessmentTypesCollection)
            {
                var item = new AssessmentTypesItem
                {
                    IdentifiableString = dxAssmType.ToIdentifiableString(),
                    Code = dxAssmType.Code,
                    Description = dxAssmType.Description,
                    Active = dxAssmType.Active? Locale.GetString(ResourceFiles.AssessmentManager, "ATAddActiveStatusYes") : Locale.GetString(ResourceFiles.AssessmentManager, "ATAddActiveStatusNo"),
                    TargetObject = dxAssmType
                };
                assessmentTypes.Add(item);
            }
            return assessmentTypes.AsQueryable();
        }

        #endregion
    }
}