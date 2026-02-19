using Selerant.DevEx.BusinessLayer;
using Selerant.DevEx.BusinessLayer.BusinessObjects.Recent;
using Selerant.DevEx.Modules.DIRModule.BackEnd.AssessmentManagement.BusinessObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Selerant.DevEx.Modules.DIRModule.BackEnd.Searches
{
    public class RecentObjectProvider : IRecentObjectProvider
    {
        public DxRecentObject GetRecentObjectFor(DxUser user, DxObject obj)
        {
            if (obj == null)
                return null;

            if (obj is DxAssessment)
                return new DxRecentAssessment(user, (DxAssessment)obj, true);

            return null;
        }
    }
}
