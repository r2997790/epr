using System;
using System.Collections.Generic;
using Selerant.DevEx.Configuration.Infrastructure;
using Selerant.DevEx.Configuration.Navigator;
using Selerant.DevEx.Configuration.Navigator.TabStrip.DTOs;
using Selerant.DevEx.Modules.DIRModule.ModuleBlocks._ThisProject;

namespace Selerant.DevEx.Modules.DIRModule.Configuration.Navigation.Assessment
{
	internal class AssessmentTabStripOverlayModuleRepository : BaseConfigurationRepository<TabStripConfigurationOverlay>, ITabStripOverlayModuleRepository
	{
        #region Fields

        readonly string _ModuleCode;

        #endregion

        #region Properties

        /// <summary>
        /// Module Name
        /// </summary>
        public string ModuleName => DIRModuleInfo.Instance.ModuleName;

        /// <summary>
        /// Priority, in order to define some ordering
        /// </summary>
        public int Priority => 5;

        /// <summary>
        /// <see cref="IConfigurationOverlayModuleRepository{TabStripConfigurationOverlay}.Name"/>
        /// </summary>
        public string Name => NamesRepository.AssessmentTabStripNames.TABSTRIP_ASSESSMENT;

        #endregion

        #region Constructors

        /// <summary>
        /// Default constructor
        /// </summary>
        public AssessmentTabStripOverlayModuleRepository()
        {
            this._ModuleCode = DIRModuleInfo.Instance.ModuleCode;
        }

		#endregion

		#region Methods

		/// <summary>
		/// Method to provide tabstrip configuration
		/// </summary>
		protected override IEnumerable<(Version, Func<TabStripConfigurationOverlay, TabStripConfigurationOverlay>)> ProvideConfigurationVersionHandlers()
		{
			return new List<(Version, Func<TabStripConfigurationOverlay, TabStripConfigurationOverlay>)>
			{
				(new Version(3, 9, 0), Version_3_9_0)
			};
		}

		/// <summary>
		/// Method to provide TabStrip configuration
		/// </summary>
		private TabStripConfigurationOverlay Version_3_9_0(TabStripConfigurationOverlay overlay)
		{
            #region Panels

            #endregion

            #region Layouts

            #endregion

            return overlay;
        }

        #endregion
    }
}
