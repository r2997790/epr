using System;
using System.Collections.Generic;
using Selerant.DevEx.Modules.DIRModule.BackEnd.AssessmentManagement.BusinessObjects;
using Selerant.DevEx.Modules.DIRModule.WebMvcFrontEnd.Areas.AssessmentNavigator.Models.Panels.ResourceManagement.Json;
using Selerant.DevEx.Modules.DIRModule.BackEnd.DestinationManagement;
using Selerant.DevEx.BusinessLayer;
using Selerant.DevEx.Modules.DIRModule.BackEnd.OutputManagement;
using Selerant.DevEx.Modules.DIRModule.BackEnd.InputManagement;
using System.Linq;
using Selerant.DevEx.Modules.DIRModule.BackEnd.Common;
using Selerant.DevEx.Modules.DIRModule.WebMvcFrontEnd.Helpers;

namespace Selerant.DevEx.Modules.DIRModule.WebMvcFrontEnd.Areas.AssessmentNavigator.Models.Panels.ResourceManagement.Shared
{
	public class DestinationRowItemModel
	{
		#region Fields

		private bool sewerDestinationWasPresent;
		private bool sewerDestinationAdded;

		#endregion Fields

		#region Properties

		private DxAssessment Assessment { get; set; }

		private DestinationModel DestinationRow { get; set; }

		private DxOutputCollection Outputs { get; set; }

		protected DxInput Input { get; private set; }

		private decimal InputId { get; set; }

		private decimal OutputCategoryId { get; set; }

		private decimal LcStageId { get; set; }

		private Dictionary<string, decimal> DestinationCodeSortOrder { get; set; }

		private List<DestinationValue> WasteDestinationValues { get; set; }

		private List<string> NonWasteDestinationCodes { get; set; }

		private HashSet<string> NonWasteColumns => (HashSet<string>)GridHelpers.Instance.NonWasteDestinationCodes;

		#endregion Properties

		#region Constructor

		public DestinationRowItemModel(DxAssessment assessment, DestinationModel destRow, bool isFood)
		{
			Assessment = assessment;
			DestinationRow = destRow;
			LcStageId = destRow.LcStageId;
			(InputId, OutputCategoryId) = GetInputIdAndOutputCategoryId(destRow.InputId, destRow.OutputCategoryId, isFood);

			Outputs = LoadOutputs();
			PrepareData();
		}

		#endregion Constructor

		#region Data Preparation Methods

		private (decimal inputId, decimal outputCategoryId) GetInputIdAndOutputCategoryId(string inputIdentifiableString, string outputCategoryIdentifiableString, bool isFood)
		{
			decimal inputId;
			decimal outputCategoryId = DxObject.ParseIdentifiableString<DxOutputCategory>(outputCategoryIdentifiableString).Id;

			if (isFood)
				inputId = DxObject.ParseIdentifiableString<DxFoodInputDestination>(inputIdentifiableString).InputId;
			else
				inputId = DxObject.ParseIdentifiableString<DxNonFoodInputDestination>(inputIdentifiableString).InputId;

			return (inputId, outputCategoryId);
		}

		private void PrepareData()
		{
			string[] destCodes = DestinationRow.DestinationPercentage.Where(x => x.Percentage > 0.0m).Select(r => r.DestinationCode).ToArray();
			DestinationCodeSortOrder = new DxDestinationSortCollection(destCodes, true).ToDictionary();

			Input = new DxInput(InputId, true);
			// filter waste destinations
			WasteDestinationValues = DestinationRow.DestinationPercentage.Where(x => !NonWasteColumns.Contains(x.DestinationCode)).ToList();
			// extract Non Waste columns
			NonWasteDestinationCodes = DestinationRow.DestinationPercentage.Where(x => NonWasteColumns.Contains(x.DestinationCode)).Select(r => r.DestinationCode).ToList();
			// check if sewer is added
			sewerDestinationAdded = DestinationRow.DestinationPercentage.Any(x => x.DestinationCode == ResourceManagementModel.SEWER && x.Percentage > 0);
		}

		private DxOutputCollection LoadOutputs()
		{
			DxOutputCollection outputsCollection = new DxOutputCollection(DxOutputCollection.Filter.AssessmentCodeAndLcStageIdAndOutputCategoryId, Assessment.Code, LcStageId, OutputCategoryId);
			outputsCollection.Load();

			return outputsCollection;
		}

		#endregion  Data Preparation Methods

		#region Private Methods

		private bool UpdateWasteOutputs(DestinationValue input)
		{
			IEnumerable<DxOutput> outputChildren = Outputs.Where(x => x.DestinationCode == input.DestinationCode);

			bool result = true;
			if (input.Percentage == 0)
			{
				DxOutput child = outputChildren.FirstOrDefault(x => x.InputId == InputId);
				if (child != null)
					result &= child.Delete();

				IEnumerable<DxOutput> remainingChildren = outputChildren.Where(x => !x.PersistenceStatus.IsDeleted && x.InputId != null);
				if (remainingChildren.Count() == 0)
				{
					DxOutput parent = outputChildren.FirstOrDefault(x => x.InputId == null);
					if (parent != null)
						result &= parent.Delete();
				}
			}
			else
			{
				DxOutput parent = outputChildren.FirstOrDefault(x => x.InputId == null);
				if (parent == null)
				{
					result &= CreateOutputParent(input);
					result &= CreateOutputChild(input);
				}
				else
				{
					DxOutput child = outputChildren.FirstOrDefault(x => x.InputId == InputId);
					if (child == null)
						result &= CreateOutputChild(input);
					else
						result &= UpdateOutputChild(input, child);
				}
			}

			return result;
		}

		private bool CreateOutputParent(DestinationValue destinationRow)
		{
			DxOutput parentOutput = new DxOutput();
			parentOutput.AssessmentCode = Assessment.Code;
			parentOutput.DestinationCode = destinationRow.DestinationCode;
			parentOutput.AssessmentLcStageId = LcStageId;
			parentOutput.OutputCategoryId = OutputCategoryId;
			parentOutput.InputId = null;
			parentOutput.SortOrder = DestinationCodeSortOrder[destinationRow.DestinationCode];

			return parentOutput.Create();
		}

		private bool CreateOutputChild(DestinationValue destinationRow)
		{
			DxOutput childOutput = new DxOutput();
			childOutput.AssessmentCode = Assessment.Code;
			childOutput.DestinationCode = destinationRow.DestinationCode;
			childOutput.AssessmentLcStageId = LcStageId;
			childOutput.OutputCategoryId = OutputCategoryId;
			childOutput.InputId = InputId;

			decimal? cost;
			decimal? weight;
			(cost, weight) = CalculateCostAndWeight(destinationRow);
			childOutput.Cost = cost;
			childOutput.Weight = weight;

			return childOutput.Create();
		}

		private bool UpdateOutputChild(DestinationValue input, DxOutput childOutput)
		{
			(decimal? cost, decimal? weight) = CalculateCostAndWeight(input);

			if (childOutput.Cost != cost || childOutput.Weight != weight)
			{
				childOutput.Cost = cost;
				childOutput.Weight = weight;

				return childOutput.Update();
			}
			else
			{
				return true;
			}
		}

		private (decimal? cost, decimal? weight) CalculateCostAndWeight(DestinationValue destination)
		{
			decimal? cost;
			decimal? weight;

			if (OutputCategoryId == Constants.OutputType.FOOD_ID)
			{
				cost = (destination.Percentage / 100) * (1 - Input.InedibleParts) * Input.Cost;
				weight = (destination.Percentage / 100) * (1 - Input.InedibleParts) * Input.Mass;
			}
			else if (OutputCategoryId == Constants.OutputType.INEDIBLE_ID)
			{
				cost = (destination.Percentage / 100) * Input.InedibleParts * Input.Cost;
				weight = (destination.Percentage / 100) * Input.InedibleParts * Input.Mass;
			}
			else
			{
				cost = (destination.Percentage / 100) * Input.Cost;
				weight = (destination.Percentage / 100) * Input.Mass;
			}

			return (cost, weight);
		}

		#endregion Private Methods

		#region Public Methods

		public bool DeleteInputDestinations()
		{
			DxInputDestinationCollection inputDestinations = new DxInputDestinationCollection(DxInputDestinationCollection.Filter.InputIdAndOutputCategoryId, InputId, OutputCategoryId);
			inputDestinations.Load();

			sewerDestinationWasPresent = inputDestinations.Any(a => a.DestinationCode == ResourceManagementModel.SEWER);

			return inputDestinations.Delete();
		}

		public bool InsertInputDestinations()
		{
			DxInputDestinationCollection inputDestinations = new DxInputDestinationCollection();
			foreach (DestinationValue inputValue in DestinationRow.DestinationPercentage)
			{
				if (inputValue.Percentage > 0)
				{
					DxInputDestination inputDestination = new DxInputDestination(InputId, inputValue.DestinationCode, OutputCategoryId);
					inputDestination.Percentage = inputValue.Percentage;

					inputDestinations.Add(inputDestination);
				}
			}

			return inputDestinations.Create();
		}

		public bool UpdateWasteDestinations()
		{
			bool result = true;

			WasteDestinationValues.ForEach(inputValue =>
			{
				result &= UpdateWasteOutputs(inputValue);
			});

			return result;
		}

		public void InsertOrUpdateNonWasteColumns()
		{
			NonWasteDestinationCodes.ForEach(nonWasteDestinationCode =>
			{
				DxOutput.InsertOrUpdateNonWasteRecord(Assessment.Code, nonWasteDestinationCode, LcStageId);
			});
		}

		public void ManageOutputWasteWater()
		{
			if (sewerDestinationWasPresent || sewerDestinationAdded)
				Input.ManageOutputWastewater();
		}

		#endregion Methods
	}
}