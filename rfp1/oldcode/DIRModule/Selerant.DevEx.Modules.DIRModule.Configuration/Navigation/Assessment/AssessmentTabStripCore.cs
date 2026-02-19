using System;
using System.Collections.Generic;
using System.Linq;
using Selerant.DevEx.Configuration.Infrastructure;
using Selerant.DevEx.Configuration.Navigator;
using Selerant.DevEx.Configuration.Navigator.DTOs;
using Selerant.DevEx.Configuration.Navigator.TabStrip.DTOs;
using Selerant.DevEx.Modules.DIRModule.ModuleBlocks._ThisProject;

namespace Selerant.DevEx.Modules.DIRModule.Configuration.Navigation.Assessment
{
    /// <summary>
    /// Assessment Tab Strip Configuration
    /// </summary>
    public class AssessmentTabStripCore : BaseConfigurationRepository<TabStripConfiguration>, ICoreTabStripConfigurationRepository
    {
        readonly string _moduleCode;

        #region Properties

        /// <summary>
        /// The name of the TabStrip
        /// </summary>
        public string TabStripName => NamesRepository.AssessmentTabStripNames.TABSTRIP_ASSESSMENT;
        /// <summary>
        /// Shared panel
        /// </summary>
        //public const string SharedQuestionnairesPanel = "Selerant.DevEx.Modules.DIRModule.WebMvcFrontEnd.Areas.AssessmentNavigator.Controllers.AssessmentPanelController";

        #endregion

        /// <summary>
        /// Default Configuration
        /// </summary>
        public AssessmentTabStripCore()
        {
            this._moduleCode = DIRModuleInfo.Instance.ModuleCode;
        }

		#region Methods

		/// <summary>
		/// Method to provide core tabstrip configuration
		/// </summary>
		protected override IEnumerable<(Version, Func<TabStripConfiguration, TabStripConfiguration>)> ProvideConfigurationVersionHandlers()
		{
			return new List<(Version, Func<TabStripConfiguration, TabStripConfiguration>)>
			{
				(new Version(3, 9, 0), Version_3_9_0)
			};
		}

		/// <summary>
		/// Method to provide core tabstrip configuration
		/// </summary>
		private TabStripConfiguration Version_3_9_0(TabStripConfiguration config)
		{
			config.ResourceFileName = "assessmentmanager";

            #region UDF

            #endregion

            #region MVC Tabs

            config.Controls.Add(TabStripPanel.AsModuleMvcPanel(NamesRepository.AssessmentPanelNames.TAB_GENERAL, Locale.GetString("DIR_AssessmentManager", "ANNNavTabGeneral"), "Selerant.DevEx.Modules.DIRModule.AssessmentNavigator.GeneralPanel", this._moduleCode).InDialog());
            config.Controls.Add(TabStripPanel.AsModuleMvcPanel(NamesRepository.AssessmentPanelNames.TAB_RESOURCE_MANAGEMENT, Locale.GetString("DIR_AssessmentManager", "ANNNavTabResourceManagement"), "Selerant.DevEx.Modules.DIRModule.AssessmentNavigator.ResourceManagement", this._moduleCode).InDialog());
            config.Controls.Add(TabStripPanel.AsModuleMvcPanel(NamesRepository.AssessmentPanelNames.TAB_ASSESSMENT_RESULTS, Locale.GetString("DIR_AssessmentManager", "ANNNavTabAssessmentResults"), "Selerant.DevEx.Modules.DIRModule.AssessmentNavigator.AssessmentResults", this._moduleCode).InDialog());

            #endregion

            #region Layouts

            var defaultLayout = new Layout("DEFAULT")
            {
                Selectable = false,
            };

            defaultLayout.AddControl(config.Controls.Single(x => x.Id == NamesRepository.AssessmentPanelNames.TAB_GENERAL), this._moduleCode);
            defaultLayout.AddControl(config.Controls.Single(x => x.Id == NamesRepository.AssessmentPanelNames.TAB_RESOURCE_MANAGEMENT), this._moduleCode);
            defaultLayout.AddControl(config.Controls.Single(x => x.Id == NamesRepository.AssessmentPanelNames.TAB_ASSESSMENT_RESULTS), this._moduleCode);

            config.Layouts.Add(defaultLayout);

            config.Layouts.ForEach(x => x.DefinedBy = this._moduleCode);

            #endregion

            return config;
        }

        #endregion
    }
}

