using System.Collections.Generic;
using Selerant.DevEx.BusinessLayer.KnowledgeBase.Directory;
using Selerant.DevEx.BusinessLayer.KnowledgeBase.Directory.Extensability;
using Selerant.DevEx.Modules.DIRModule.BackEnd.AssessmentManagement.BusinessObjects;
using Selerant.DevEx.Modules.DIRModule.BackEnd.AssessmentTypeManagement;

namespace Selerant.DevEx.Modules.DIRModule.BackEnd.KnowledgeBase.Directory
{
    public class DIRDirectoryWhichBoundToDxObjectTypeProvider : IDirectoryWhichBoundToDxObjectTypeProvider
    {
        public IEnumerable<DirectoryWhichBoundToDxObjectType> Provide()
        {
            yield return DirectoryWhichBoundToDxObjectType.Create<DxAssessmentType>(@"\DevEx\DxAssessmentType\");
            yield return DirectoryWhichBoundToDxObjectType.Create<DxAssessment>(@"\DevEx\DxAssessment\");
        }
    }
}