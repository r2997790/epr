using Selerant.DevEx.Configuration.Infrastructure;
using Selerant.DevEx.Configuration.Infrastructure.TreeLayerServices;
using Selerant.DevEx.Configuration.Navigator.DialogTabStrip;
using Selerant.DevEx.Configuration.Navigator.DialogTabStrip.DTOs;
using Selerant.DevEx.Configuration.Navigator.DTOs;
using Selerant.DevEx.Configuration.Navigator.TabStrip.DTOs;
using Selerant.DevEx.Modules.DIRModule.ModuleBlocks._ThisProject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Selerant.DevEx.Modules.DIRModule.Configuration.Navigation.Assessment
{
    internal class AssessmentDialogTabStripCore : BaseConfigurationRepository<DialogTabStripConfiguration>, ICoreDialogTabStripRepository
    {
        private readonly string _moduleCode;

        /// <summary>
        /// Default constructor
        /// </summary>
        public AssessmentDialogTabStripCore()
        {
            _moduleCode = DIRModuleInfo.Instance.ModuleCode;

            ModuleName = DIRModuleInfo.Instance.ModuleName;
            DialogTabStripName = NamesRepository.AssessmentTabStripNames.TABSTRIP_ASSESSMENT_DIALOG;
            Priority = 5;
        }

        /// <summary>
        /// Name identifier of dialog tabstrip name
        /// </summary>
        public string DialogTabStripName { get; }

        /// <summary>
        /// Module name
        /// </summary>
        public string ModuleName { get; }

        /// <summary>
        /// Priority, in order to define some ordering
        /// </summary>
        public int Priority { get; }

        /// <summary>
        /// <see cref="BaseConfigurationRepository{TConfiguration}.ProvideConfigurationVersionHandlers()"/>
        /// </summary>
        /// <returns></returns>
        protected override IEnumerable<(Version, Func<DialogTabStripConfiguration, DialogTabStripConfiguration>)> ProvideConfigurationVersionHandlers()
        {
            return new List<(Version, Func<DialogTabStripConfiguration, DialogTabStripConfiguration>)>
            {
                (new Version(3, 9), Version_39)
            };
        }

        /// <summary>
        /// Method to provide dialog tabstrip configuration
        /// </summary>

        private DialogTabStripConfiguration Version_39(DialogTabStripConfiguration config)
        {
            config.ResourceFileName = "assessmentmanager";

            #region UDF

            #endregion

            #region MVC Tabs

            config.Controls.Add(DialogTabStripPanel.AsModuleMvcPanel(NamesRepository.AssessmentPanelNames.TAB_GENERAL, "ANNNavTabGeneral", "Selerant.DevEx.Modules.DIRModule.AssessmentNavigator.GeneralPanel", this._moduleCode).InDialog());
            //config.Controls.Add(TabStripPanel.AsModuleMvcPanel(NamesRepository.AssessmentPanelNames.TAB_RESOURCE_MANAGEMENT, "ANNNavTabResourceManagement", "Selerant.DevEx.Modules.DIRModule.AssessmentNavigator.ResourceManagement", this._moduleCode).InDialog());
            //config.Controls.Add(TabStripPanel.AsModuleMvcPanel(NamesRepository.AssessmentPanelNames.TAB_ASSESSMENT_RESULTS, "ANNNavTabAssessmentResults", "Selerant.DevEx.Modules.DIRModule.AssessmentNavigator.AssessmentResults", this._moduleCode).InDialog());

            #endregion

            #region Layouts

            var defaultLayout = new Layout("DEFAULT")
            {
                Selectable = false,
            };

            defaultLayout.AddControl(config.Controls.Single(x => x.Id == NamesRepository.AssessmentPanelNames.TAB_GENERAL), this._moduleCode);
            //defaultLayout.AddControl(config.Controls.Single(x => x.Id == NamesRepository.AssessmentPanelNames.TAB_RESOURCE_MANAGEMENT), this._moduleCode);
            //defaultLayout.AddControl(config.Controls.Single(x => x.Id == NamesRepository.AssessmentPanelNames.TAB_ASSESSMENT_RESULTS), this._moduleCode);

            config.Layouts.Add(defaultLayout);

            config.Layouts.ForEach(x => x.DefinedBy = this._moduleCode);

            #endregion

            return config;
        }
    }
}
