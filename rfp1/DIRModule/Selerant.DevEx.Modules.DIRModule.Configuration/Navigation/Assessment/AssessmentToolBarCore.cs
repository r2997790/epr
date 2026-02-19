using System;
using System.Collections.Generic;
using System.Linq;
using Selerant.DevEx.Configuration.Infrastructure;
using Selerant.DevEx.Configuration.Infrastructure.TreeLayerServices;
using Selerant.DevEx.Configuration.Navigator.DTOs;
using Selerant.DevEx.Configuration.Navigator.ToolBar;
using Selerant.DevEx.Configuration.Navigator.ToolBar.DTOs;
using Selerant.DevEx.Configuration.Navigator.ToolBar.Helpers;
using Selerant.DevEx.Modules.DIRModule.Configuration.Resources;
using Selerant.DevEx.Modules.DIRModule.ModuleBlocks._ThisProject;

namespace Selerant.DevEx.Modules.DIRModule.Configuration.Navigation.Assessment
{
	/// <summary>
	/// Implementation of the <see cref="ICoreToolBarConfigurationRepository"/> for Assessment
	/// </summary>
	public class AssessmentToolBarCore : BaseConfigurationRepository<ToolBarConfiguration>, ICoreToolBarConfigurationRepository
    {
		private const string MENU_BUTTON_KEY_SHARING = "Sharing";
		private const string MENU_BUTTON_KEY_DELETE = "Delete";
        private const string MENU_BUTTON_DASHBOARD = "AssessmentDashboard";

		readonly string _moduleCode;

        /// <summary>
        /// <see cref="ICoreToolBarConfigurationRepository.ToolBarName"/>
        /// </summary>
        public string ToolBarName => NamesRepository.AssessmentToolBarNames.TOOLBAR_ASSESSMENT;

        /// <summary>
        /// Default Configuration
        /// </summary>
        public AssessmentToolBarCore()
        {
            this._moduleCode = DIRModuleInfo.Instance.ModuleCode;
        }

		/// <summary>
		/// <see cref="ICoreConfigurationRepository{T}.Provide"/>
		/// </summary>
		protected override IEnumerable<(Version, Func<ToolBarConfiguration, ToolBarConfiguration>)> ProvideConfigurationVersionHandlers()
		{
			return new List<(Version, Func<ToolBarConfiguration, ToolBarConfiguration>)>
			{
				(new Version(3, 9, 0), Version_3_9_0)
			};
		}

		/// <summary>
		/// <see cref="ICoreToolBarConfigurationRepository.Provide"/>
		/// </summary>
		private ToolBarConfiguration Version_3_9_0(ToolBarConfiguration config)
		{
			config.ResourceFileName = "assessmentmanager";

            #region Core Controls

            config
               .WithControl(ExNavigateToolBarButton
                   .AsButton(MENU_BUTTON_KEY_SHARING, "SHARE", "DxShare.png", Locale.GetString(ResourceFiles.AssessmentManager, "ANNToolbarSharing"), this._moduleCode)
                   .WithNeedRights(2)
                   .WithTooltip("Sharing_Sharing"));

            config
                .WithControl(ExNavigateToolBarButton
                    .AsButton(MENU_BUTTON_DASHBOARD, "REPORT", "DxReports.png", Locale.GetString(ResourceFiles.AssessmentManager, "ANNToolbarDashboard"), this._moduleCode)
                    .WithNeedRights(-1)
                    .WithTooltip(Locale.GetString(ResourceFiles.AssessmentManager, "ANNToolbarDashboard")));

            config
                .WithControl(ExNavigateToolBarButton
                    .AsButton("Copy", "ACTIONS", "DxCopy.png", Locale.GetString(ResourceFiles.AssessmentManager, "ANNToolbarCopy"), this._moduleCode)
                    .WithNeedRights(-1)
					.WithTooltip(Locale.GetString(ResourceFiles.AssessmentManager, "ANNToolbarCopyTT")));
			config
				.WithControl(ExNavigateToolBarButton
                    .AsButton(MENU_BUTTON_KEY_DELETE, "ACTIONS", "delete.png", Locale.GetString(ResourceFiles.AssessmentManager, "ANNToolbarDelete"), this._moduleCode)
                    .WithNeedRights(-1)
					.WithTooltip(Locale.GetString(ResourceFiles.AssessmentManager, "ANNToolbarDeleteTT")));

			#endregion

			#region Layouts

			var defaultLayout = new Layout("DEFAULT") { Selectable = true };

            config.Layouts.Add(defaultLayout);
            defaultLayout.Controls.Add(new OverlayControlReference(config.Controls.Single(x => x.Id == "Sharing")));
            defaultLayout.AddControl(config.Controls.Single(x => x.Id == "Copy"), "CORE");
            //defaultLayout.AddControl(config.Controls.Single(x => x.Id == "SCPSync"), "CORE");
            defaultLayout.Controls.Add(new OverlayControlReference(config.Controls.Single(x => x.Id == MENU_BUTTON_DASHBOARD)));
            defaultLayout.Controls.Add(new OverlayControlReference(config.Controls.Single(x => x.Id == "Delete")));


            config.Layouts.ForEach(x => x.DefinedBy = this._moduleCode);

            #endregion

            return config;
        }
    }
}
