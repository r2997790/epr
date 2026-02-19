using System;
using System.Collections.Generic;
using Selerant.DevEx.Configuration.Infrastructure;
using Selerant.DevEx.Configuration.Infrastructure.TreeLayerServices;
using Selerant.DevEx.Configuration.Navigator.ToolBar;
using Selerant.DevEx.Configuration.Navigator.ToolBar.DTOs;
using Selerant.DevEx.Modules.DIRModule.ModuleBlocks._ThisProject;

namespace Selerant.DevEx.Modules.DIRModule.Configuration.Navigation.Assessment
{
	internal class AssessmentToolBarOverlayModuleRepository : BaseConfigurationRepository<ToolBarConfigurationOverlay>, IToolBarOverlayModuleRepository
	{
        #region Properties

        /// <summary>
        /// Module Name
        /// </summary>
        public string ModuleName => DIRModuleInfo.Instance.ModuleName;
        /// <summary>
        /// ToolBar Name
        /// </summary>
        public string ToolBarName => NamesRepository.AssessmentToolBarNames.TOOLBAR_ASSESSMENT;

        /// <summary>
        /// Priority, in order to define some ordering
        /// </summary>
        public int Priority => 5;

        #endregion

        #region Constructors

        /// <summary>
        /// Default constructor
        /// </summary>
        public AssessmentToolBarOverlayModuleRepository()
        {

        }

		#endregion

		#region Methods

		/// <summary>
		/// <see cref="IConfigurationOverlayModuleRepository{T}.Provide"/>
		/// </summary>
		protected override IEnumerable<(Version, Func<ToolBarConfigurationOverlay, ToolBarConfigurationOverlay>)> ProvideConfigurationVersionHandlers()
		{
			return new List<(Version, Func<ToolBarConfigurationOverlay, ToolBarConfigurationOverlay>)>
			{
				(new Version(3, 9, 0), Version_3_9_0)
			};
		}

		/// <summary>
		/// Method to provide ToolBar configuration
		/// </summary>
		private ToolBarConfigurationOverlay Version_3_9_0(ToolBarConfigurationOverlay resultOverlayConfig)
		{
            #region Controls

            #endregion

            #region Layouts

            #endregion

            return resultOverlayConfig;
        }

        #endregion
    }
}
