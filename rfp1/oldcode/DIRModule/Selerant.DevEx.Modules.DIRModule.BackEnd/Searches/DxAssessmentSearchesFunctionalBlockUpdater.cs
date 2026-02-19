using Selerant.DevEx.BusinessLayer;
using Selerant.DevEx.Modules.DIRModule.BackEnd.AssessmentManagement.BusinessObjects;
using Selerant.DevEx.Modules.DIRModule.BackEnd.Common;
using Selerant.DevEx.Modules.DIRModule.Configuration.Searches.Assessment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Selerant.DevEx.Modules.DIRModule.BackEnd.Searches
{
    internal sealed class DxAssessmentSearchesFunctionalBlockUpdater : ISearchesFunctionalBlocksUpdater
    {
        public void Update(SearchStaticStructure searchStaticStructure)
        {
            searchStaticStructure.WithScope<DxAssessment>(Constants.AssessmentFB.ASSESSMENT_SEARCH, null)
                .AddNode(AssessmentSearchPanelNames.PANEL_AssessmentBasicPanel, Constants.AssessmentFB.ASSESSMENT_SEARCH)
                .AddNode("CustomQueries", Constants.AssessmentFB.ASMT_SEARCH_CUSTOM_QUERY)
                .AddNode("CustomQueriesPanels", Constants.AssessmentFB.ASMT_SEARCH_CUSTOM_QUERY)
                .AddNode(AssessmentSearchPanelNames.PANEL_AssessmentRecent, Constants.AssessmentFB.ASSESSMENT_SEARCH);
        }
    }
}
