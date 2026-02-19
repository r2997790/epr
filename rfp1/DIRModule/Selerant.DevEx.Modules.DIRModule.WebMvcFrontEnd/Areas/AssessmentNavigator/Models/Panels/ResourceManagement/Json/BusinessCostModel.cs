using System;
using Selerant.DevEx.BusinessLayer;
using Selerant.DevEx.Modules.DIRModule.BackEnd.BusinessCostManagement;
using Selerant.DevEx.WebPages;

namespace Selerant.DevEx.Modules.DIRModule.WebMvcFrontEnd.Areas.AssessmentNavigator.Models.Panels.ResourceManagement.Json
{
	public class BusinessCostModel : IJsonSerialized, IToDxObject<DxBusinessCost>
	{
		public bool CreateNew { get; private set; }
		public string IdentifiableString { get; set; }
        public decimal LcStageId { get; set; }
        public string Title { get; set; }
		public decimal Cost { get; set; }

		private (bool createNew, decimal id) ParseIdent()
		{
			if (string.IsNullOrEmpty(IdentifiableString))
			{
				return (true, .0m);
			}
			else
			{
				DxBusinessCostGridItem gridItem = DxObject.ParseIdentifiableString<DxBusinessCostGridItem>(IdentifiableString);

				if (gridItem.Id < 0) // it was non existing carried over row
					return (true, .0m);
				else
					return (false, gridItem.Id);
			}
		}

		public DxBusinessCost ToDxObject()
		{
			DxBusinessCost businessCost;

			(bool createNew, decimal id) = ParseIdent();
			CreateNew = createNew;

			if (createNew)
			{ 
				businessCost = new DxBusinessCost();
				businessCost.AssessmentLcStageId = LcStageId;
			}
			else
			{
                businessCost = new DxBusinessCost(id);
				businessCost.Load();
			}

			businessCost.Title = Title;
			businessCost.Cost = Cost;

			return businessCost;
		}
	}
}