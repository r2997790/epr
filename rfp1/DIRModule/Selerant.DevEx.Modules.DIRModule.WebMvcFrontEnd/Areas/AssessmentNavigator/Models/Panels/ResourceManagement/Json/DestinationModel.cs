using Selerant.DevEx.WebPages;
using System.Collections.Generic;

namespace Selerant.DevEx.Modules.DIRModule.WebMvcFrontEnd.Areas.AssessmentNavigator.Models.Panels.ResourceManagement.Json
{
	public class DestinationValue: IJsonSerialized
	{
		public string DestinationCode { get; set; }

		public decimal Percentage { get; set; }
	}

	public class DestinationModel : IJsonSerialized
	{
		public string InputId { get; set; }

		public string OutputCategoryId { get; set; }

		public decimal LcStageId { get; set; }

		public List<DestinationValue> DestinationPercentage { get; set; }
	}
}