using Selerant.DevEx.BusinessLayer;
using Selerant.DevEx.Modules.DIRModule.BackEnd.AssessmentManagement.BusinessObjects;
using Selerant.DevEx.Modules.DIRModule.BackEnd.Common;
using Selerant.DevEx.Modules.DIRModule.Configuration.Navigation.Assessment;

namespace Selerant.DevEx.Modules.DIRModule.BackEnd.Configuration.NavigatorFunctionalBlock.Assessment
{
	internal sealed class AssessmentFunctionalBlocksUpdater : INavigatorsFunctionalBlocksUpdater
	{
		public void Update(NavigatorStaticStructure navigatorStaticStructure)
		{
            navigatorStaticStructure.WithScope<DxAssessment>(Constants.AssessmentNavigatorFB.ASSESSMENT_NAVIGATOR,
                                                             Constants.AssessmentNavigatorFB.ASSESSMENT_NAVIGATOR_EXTRA_TABS,
                                                             Constants.AssessmentNavigatorFB.ASSESSMENT_NAVIGATOR_TOOLBAR_BTNS)
                                    .AddNode(NamesRepository.AssessmentPanelNames.TAB_GENERAL, Constants.AssessmentNavigatorFB.ASSESSMENT_GENERAL_TAB)
                                    .AddNode(NamesRepository.AssessmentPanelNames.TAB_RESOURCE_MANAGEMENT, Constants.AssessmentNavigatorFB.ASSESSMENT_RESOURCE_MANAGEMENT_TAB)
                                    .AddNode(NamesRepository.AssessmentPanelNames.TAB_ASSESSMENT_RESULTS, Constants.AssessmentNavigatorFB.ASSESSMENT_RESULT_TAB);
                                // TODO: add other Panel - FuncBlck here

        }
	}
}
