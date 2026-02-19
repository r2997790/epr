using Selerant.DevEx.BusinessLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Selerant.DevEx.Modules.DIRModule.BackEnd.Searches
{
    public static class CustomizedGridNames
    {
        public static string SearchAssessmentBasic = "SearchASSESSMENTBasic";
    }

    internal class CustomizedGridRepository : ICustomizedGridRepository
    {
        public IEnumerable<CustomizedGridConfiguration> Provide()
        {
            return new [] 
            {
                new CustomizedGridConfiguration { Id = CustomizedGridNames.SearchAssessmentBasic, FileName = $"{CustomizedGridNames.SearchAssessmentBasic}.xml" }
            };
        }
    }
}
