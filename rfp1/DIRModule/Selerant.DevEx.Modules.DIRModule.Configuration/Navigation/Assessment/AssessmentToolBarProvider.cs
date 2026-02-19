using Selerant.DevEx.Configuration.Navigator.ToolBar;
using System;

namespace Selerant.DevEx.Modules.DIRModule.Configuration.Navigation.Assessment
{
    /// <summary>
    /// toolbar provider for questionnaire definition
    /// </summary>
    public class AssessmentToolBarProvider : INeedToAddToolbar
    {
        private readonly IToolBarOverlayRepository _rToolbarOverlayRepository;

        public AssessmentToolBarProvider(IToolBarOverlayRepository toolbarOverlayRepository)
        {
            _rToolbarOverlayRepository = toolbarOverlayRepository.ThrowIfArgumentIsNull(nameof(toolbarOverlayRepository));
        }

        /// <summary>
        /// <see cref="INeedToAddToolbar.ProvideDefinition"/>
        /// </summary>
        public ToolbarDefinitionVO ProvideDefinition()
        {
            var repo = new AssessmentToolBarCore();
            var vo = new ToolbarDefinitionVO(repo.ToolBarName, "ASSESSMENT", repo,
                _rToolbarOverlayRepository);

            return vo;
        }
    }
}