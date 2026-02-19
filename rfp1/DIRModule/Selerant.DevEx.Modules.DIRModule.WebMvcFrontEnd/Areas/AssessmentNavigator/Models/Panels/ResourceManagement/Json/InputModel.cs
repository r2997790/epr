using System;
using Selerant.DevEx.BusinessLayer;
using ResourceMaterial = Selerant.DevEx.Modules.DIRModule.BackEnd.Common.Constants.ResourceMaterial;
using Selerant.DevEx.Modules.DIRModule.BackEnd.Common;
using Selerant.DevEx.Modules.DIRModule.BackEnd.InputManagement;
using Selerant.DevEx.WebPages;
using System.Collections.Generic;
using System.Linq;

namespace Selerant.DevEx.Modules.DIRModule.WebMvcFrontEnd.Areas.AssessmentNavigator.Models.Panels.ResourceManagement.Json
{
	public interface IToDxObject<T> where T : DxObject
	{
		T ToDxObject();
	}

	public enum ResourceRowType
	{
		Input,
		Destination,
		Output,
		BusinessCost
	}

	public class InputModel : IJsonSerialized, IToDxObject<DxInput>
	{
		#region Fields

		private DxInput input;
		private string oldMaterialPlant;
		private string oldMaterialCode;
		private decimal oldInedibleParts;

		#endregion

		#region Properties

		public bool CreateNew { get { return string.IsNullOrEmpty(IdentifiableString); } }

		public string IdentifiableString { get; set; }

		public string CategoryIdentifiableString { get; set; }

		public string CategoryType { get; set; }

		public decimal LcStageId { get; set; }

		public string MaterialIdentifiableString { get; set; }

		/// <summary>
		/// Product Co-Product spread client data 
		/// </summary>
		public string[] PartOfProductCoproduct { get; set; }

		public bool Packaging { get; set; }

		public decimal Mass { get; set; }

		public decimal Cost { get; set; }

		public decimal InedibleParts { get; set; }

		public int Measurement { get; set; }
        public string ProductSource { get; set; }

        #endregion

        public DxInput ToDxObject()
		{
			if (string.IsNullOrEmpty(IdentifiableString))
			{
				input = new DxInput() { AssessmentLcStageId = LcStageId };
			}
			else
			{
				input = DxObject.ParseIdentifiableString<DxInput>(IdentifiableString);
				input.LoadEntity();

				oldMaterialPlant = input.MaterialPlant;
				oldMaterialCode = input.MaterialCode;
				oldInedibleParts = input.InedibleParts ?? 0;
			}

			input.InputCategory = DxObject.ParseIdentifiableString<DxInputCategory>(CategoryIdentifiableString);
			input.Material = DxObject.ParseIdentifiableString<DxMaterial>(MaterialIdentifiableString);
			input.PartOfProductCoproduct = PartOfProductCoproduct.Length > 0;

			if (CategoryType != Constants.InputType.FOOD)
				input.Packaging = Packaging;

			input.Mass = Mass;
			input.Cost = Cost;
			input.InedibleParts = InedibleParts / 100;
			input.Measurement = (DxInput.Measure)Measurement;
            input.ProductSource = ProductSource;

			return input;
		}

		/// <summary>
		/// Use only on Update
		/// </summary>
		/// <returns>true if update from or to Water material</returns>
		public bool IsWaterMaterialInterchanged()
		{
			bool DetectChange(params string[] materialCodes)
			{
				// same material, so no change
				if (materialCodes[0] == materialCodes[1])
					return false;

				HashSet<string> waterCodes = new HashSet<string>() { ResourceMaterial.DIR_FOOD_WATER, ResourceMaterial.DIR_NONFOOD_WATER };

				return materialCodes.Any(a => waterCodes.Contains(a));
			};

			if (oldMaterialPlant != DxPlant.NONE || input.MaterialPlant != DxPlant.NONE) // water allways have plant NONE
			{
				return false;
			}
			else
			{
				return DetectChange(oldMaterialCode, input.MaterialCode);
			}
		}

		/// <summary>
		/// Changes to inedible parts also introduces new row, chnage from 0 to some positive value
		/// </summary>
		/// <returns></returns>
		public bool AddedIneibleParts()
		{
			return oldInedibleParts == decimal.Zero && (input.InedibleParts ?? 0) > decimal.Zero;
		}
	}
}