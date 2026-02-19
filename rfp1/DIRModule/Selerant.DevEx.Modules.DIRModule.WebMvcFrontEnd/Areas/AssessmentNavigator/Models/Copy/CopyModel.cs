using Selerant.DevEx.BusinessLayer;
using Selerant.DevEx.Modules.DIRModule.BackEnd.AssessmentManagement.BusinessObjects;
using Selerant.DevEx.Modules.DIRModule.BackEnd.BusinessCostManagement;
using Selerant.DevEx.Modules.DIRModule.BackEnd.InputManagement;
using Selerant.DevEx.Modules.DIRModule.BackEnd.OutputManagement;
using Selerant.DevEx.Modules.DIRModule.BackEnd.ResourceNote;
using Selerant.DevEx.Web;
using Selerant.DevEx.WebControls;
using Selerant.DevEx.WebMvcModules.Infrastructure.Controls;
using Selerant.DevEx.WebPages;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Web.Mvc;

namespace Selerant.DevEx.Modules.DIRModule.WebMvcFrontEnd.Areas.AssessmentNavigator.Models.Copy
{
	public class CopyModel : DialogViewIndexModel
	{
		public DxAssessment Assessment { get; private set; }

		#region View Properties

		public DxPhraseText ProdClassificationPhraseText => DxPhraseText.GetBest(DxPhraseTypeCategory.ListOfValuesStandard, "DXDIR_ASSESSMENT.PROD_CLASSIF", this.ProdClassification, DxUser.CurrentUser.ProgramCulture);
		public string CompanyName { get; set; }
		public string Comments { get; set; }
		public DxUser Completionist { get; set; }
		public string Description { get; set; }
		public string ProdClassification { get; set; }
		public DxCountry Location { get; set; }
		public string OrgStructure { get; set; }
		public string Industry { get; set; }
		public string DataQuality { get; set; }
		public string Phone { get; set; }
		public NullableDateTime TimeframeFrom { get; set; }
		public NullableDateTime TimeframeTo { get; set; }

		#endregion View Properties

		#region Constructors

		public CopyModel(string controllerUrl, ViewControlControllerData data, DxAssessment targetAssessment)
			: base(controllerUrl, data)
		{
			SetModelProperties(targetAssessment);
		}

		public CopyModel(DxAssessment targetAssessment)
			: base(null)
		{
			Assessment = targetAssessment;
			Assessment.LoadEntity();
		}

		#endregion Constructors

		#region View Methods

		public void SetModelProperties(DxAssessment targetAssessment)
		{
			Assessment = targetAssessment;
			Assessment.LoadEntity();

			CompanyName = Assessment.CompanyName;
			Comments = Assessment.Comments;
			Completionist = Assessment.Completionist;
			TimeframeFrom = Assessment.TimeframeFrom;
			TimeframeTo = Assessment.TimeframeTo;
			Phone = Assessment.Phone;
			Description = Assessment.Description;
			ProdClassification = Assessment.ProdClassification;
			Location = !string.IsNullOrEmpty(Assessment.Location) ? DxCountry.Get(Assessment.Location) : null;
			OrgStructure = Assessment.OrgStructure;
			Industry = Assessment.AssessmentType.Code;
			DataQuality = Assessment.DataQuality;
		}

		public List<SelectListItem> GetAssessmentTypeList()
		{
			return Helpers.LovListFactory.GetAssessmentTypes();
		}

		public WebListItemCollection GetOrgStructureList()
		{
			return Helpers.LovListFactory.GetOrgStructures();
		}

		public WebListItemCollection GetDataQualityList()
		{
			return Helpers.LovListFactory.GetDataQualities();
		}

		#endregion View Methods

		#region Assessment Copy Methods

		public DxAssessment Copy(CopyAssessmentDataJson assessmentData)
		{
			DxAssessment sourceAssessment = Assessment;
			var sourceAssessmentType = Assessment.AssessmentType.Code;
			var newAssessmentCode = DxAssessment.GetAutoNumberingKey(sourceAssessmentType, DxUser.CurrentUser);
			DxAssessment newAssessment = null;

			using (DxUnitOfWork unitOfWork = DxUnitOfWork.New())
			{
				newAssessment = new DxAssessment(newAssessmentCode);
				sourceAssessment.CopyTo(newAssessment);
			
				newAssessment.Description = assessmentData.Description;
				newAssessment.ProdClassification = DxObject.ParseIdentifiableString<DxPhraseText>(assessmentData.ProdClassification).Code;
				newAssessment.TimeframeFrom = assessmentData.TimeframeFrom;
				newAssessment.TimeframeTo = assessmentData.TimeframeTo;
				newAssessment.Location = DxObject.ParseIdentifiableString<DxCountry>(assessmentData.Location).Code;

				bool result = newAssessment.Create();

				if (DxUser.CurrentUser.IsExternal)
					result &= newAssessment.CreatePartnerAssessment();

				if (!string.IsNullOrWhiteSpace(assessmentData.Comments))
					result &= newAssessment.CreateOrUpdateAttributeValue(newAssessment.CommentsAttributeName, assessmentData.Comments);
				// DataQuality attribute
				result &= newAssessment.CreateOrUpdateAttributeValue(newAssessment.DataQualityAttributeName, assessmentData.DataQuality);

				result &= CopyLcStages(sourceAssessment, newAssessment, out Dictionary<decimal, decimal> sourceNewLCStages);

				result &= CopyAssessmentDestinations(sourceAssessment.Code, newAssessment.Code);

				result &= CopyInputCategories(sourceAssessment, newAssessment, out Dictionary<decimal, decimal> sourceNewInputCategories);
				
				(bool createInputs, bool createInputDestinations) = CopyInputsAndInputDestinations(sourceAssessment, newAssessment, sourceNewInputCategories, sourceNewLCStages, out Dictionary<decimal, decimal> sourceNewInputs);
				result &= createInputs;
				result &= createInputDestinations;

				result &= createInputProductCoProductSpread(sourceNewInputs);

				result &= CopyOutputs(sourceAssessment, newAssessment, sourceNewLCStages, sourceNewInputs);

				result &= CopyBusinessCosts(sourceAssessment, newAssessment, sourceNewLCStages);

				result &= CopyResourceNote(sourceAssessment, newAssessment, sourceNewLCStages);

				if (result)
					unitOfWork.Commit();
				else
					unitOfWork.Abort();
			}

			return newAssessment;
		}

		private bool CopyAssessmentDestinations(string sourceAssessmentCode, string newAssessmentCode)
		{
			DxAssessmentDestinationCollection destinations = new DxAssessmentDestinationCollection(DxAssessmentDestinationCollection.Filter.AssessmentCode, sourceAssessmentCode);
			destinations.Load();

			DxAssessmentDestinationCollection assessmentDestCollection = new DxAssessmentDestinationCollection();
			foreach (var wasteDestination in destinations)
			{
				DxAssessmentDestination wasteDest = new DxAssessmentDestination(newAssessmentCode, wasteDestination.DestinationCode);
				assessmentDestCollection.Add(wasteDest);
			}

			return assessmentDestCollection.Create();
		}

		private bool CopyLcStages(DxAssessment sourceAssessment, DxAssessment newAssessment, out Dictionary<decimal, decimal> sourceNewLCStages)
		{
			DxAssessmentLcStageCollection lcStages = new DxAssessmentLcStageCollection(sourceAssessment);
			lcStages.Load();

			sourceNewLCStages = new Dictionary<decimal, decimal>(lcStages.Count);

			DxAssessmentLcStageCollection newLcStages = new DxAssessmentLcStageCollection();
			foreach (var lcStage in lcStages)
			{
				DxAssessmentLcStage assessmentLcStage = new DxAssessmentLcStage();
				lcStage.CopyTo(assessmentLcStage);
				assessmentLcStage.AutogeneratedIdentityKey = false;
				assessmentLcStage.SetNextIdentityKey();
				assessmentLcStage.AssessmentCode = newAssessment.Code;
				assessmentLcStage.Visible = true;

				sourceNewLCStages.Add(lcStage.Id, assessmentLcStage.Id);

				newLcStages.Add(assessmentLcStage);
			}

			var result = newLcStages.Create();

			// connect with copied carried over source stage
			foreach (var newLcStage in newLcStages)
			{
				if (newLcStage.SourceAssessmentLcStageId.HasValue && sourceNewLCStages.ContainsKey(newLcStage.SourceAssessmentLcStageId.Value))
					newLcStage.SourceAssessmentLcStageId = sourceNewLCStages[newLcStage.SourceAssessmentLcStageId.Value];
				else
					newLcStage.SourceAssessmentLcStageId = null;
			}

			return result & newLcStages.Update();
		}

		private bool CopyInputCategories(DxAssessment sourceAssessment, DxAssessment newAssessment, out Dictionary<decimal, decimal> sourceNewInputCategories)
		{
			DxInputCategoryCollection sourceInputCategories = new DxInputCategoryCollection(DxInputCategoryCollection.Filter.AssessmentCode, sourceAssessment.Code);
			sourceInputCategories.Load();

			sourceNewInputCategories = new Dictionary<decimal, decimal>(sourceInputCategories.Count);

			DxInputCategoryCollection newInputCategories = new DxInputCategoryCollection();
			foreach (var inputCategory in sourceInputCategories)
			{
				DxInputCategory newInputCategory = new DxInputCategory();
				inputCategory.CopyTo(newInputCategory);
				newInputCategory.AutogeneratedIdentityKey = false;
				newInputCategory.SetNextIdentityKey();
				newInputCategory.AssessmentCode = newAssessment.Code;

				sourceNewInputCategories.Add(inputCategory.Id, newInputCategory.Id);

				newInputCategories.Add(newInputCategory);
			}

			return newInputCategories.Create();
		}

		private (bool, bool) CopyInputsAndInputDestinations(DxAssessment sourceAssessment, 
			                                               DxAssessment newAssessment, 
														   Dictionary<decimal, decimal> sourceNewInputCategories, 
														   Dictionary<decimal, decimal> sourceNewLCStages, 
														   out Dictionary<decimal, decimal> sourceNewInputs)
		{
			DxInputCollection sourceInputs = new DxInputCollection(sourceAssessment.Code, true);
			DxInputCollection newInputs = new DxInputCollection();
			DxInputDestinationCollection newInputDestinations = new DxInputDestinationCollection();

			sourceNewInputs = new Dictionary<decimal, decimal>(sourceInputs.Count);

			foreach (var input in sourceInputs)
			{
				DxInputDestinationCollection sourceInputDestinations = new DxInputDestinationCollection(DxInputDestinationCollection.Filter.InputId, input.Id);
				sourceInputDestinations.Load();

				DxInput newInput = new DxInput();
				input.CopyTo(newInput);
				newInput.AutogeneratedIdentityKey = false;
				newInput.SetNextIdentityKey();
				newInput.AssessmentCode = newAssessment.Code;
				newInput.InputCategoryId = sourceNewInputCategories[input.InputCategoryId];
				newInput.AssessmentLcStageId = sourceNewLCStages[input.AssessmentLcStageId];

				// mapping source input - new input
				sourceNewInputs.Add(input.Id, newInput.Id);

				foreach (var inputDestination in sourceInputDestinations)
				{
					DxInputDestination newInputDestination = new DxInputDestination(newInput.Id, inputDestination.DestinationCode, inputDestination.OutputCategoryId);
					newInputDestination.Percentage = inputDestination.Percentage;

					newInputDestinations.Add(newInputDestination);
				}

				newInputs.Add(newInput);
			}

			return (newInputs.Create(), newInputDestinations.Create());
		}

		/// <summary>
		/// Copies InputProductCoProductSpread table
		/// </summary>
		/// <param name="sourceNewInputs">source input - new input</param>
		/// <returns></returns>
		private bool createInputProductCoProductSpread(Dictionary<decimal, decimal> sourceNewInputs)
		{
			decimal[] sourceInputIds = sourceNewInputs.Keys.ToArray();
			var sourceInputsProductCoProductSpread = new DxInputProductCoProductSpreadCollection(sourceInputIds, true);

			if (sourceInputsProductCoProductSpread.Count > 0)
			{
				var newInputsProductCoProductSpread = new DxInputProductCoProductSpreadCollection();
				Dictionary<decimal, List<string>> sourceInputsPCPList = sourceInputsProductCoProductSpread.ToDictionary();

				foreach (decimal sourceInputId in sourceInputIds)
				{
					if (sourceInputsPCPList.TryGetValue(sourceInputId, out List<string> productCoProductDestsCodes))
					{
						decimal newInputId = sourceNewInputs[sourceInputId];
						{
							productCoProductDestsCodes.ForEach(pcpDestCode =>
							{
								newInputsProductCoProductSpread.AddItem(new DxInputProductCoProductSpread(newInputId, pcpDestCode));
							});
						}
					}
				}

				return newInputsProductCoProductSpread.Create();
			}
			else
				return true;
		}

		private bool CopyOutputs(DxAssessment sourceAssessment, DxAssessment newAssessment, Dictionary<decimal, decimal> lcStages, Dictionary<decimal, decimal> sourceNewInputs)
		{
			DxOutputCollection sourceOutputs = new DxOutputCollection(DxOutputCollection.Filter.AssessmentCode, sourceAssessment.Code);
			sourceOutputs.Load();

			DxOutputCollection newOutputs = new DxOutputCollection();
			foreach (var output in sourceOutputs)
			{
				DxOutput newOutput = new DxOutput();
				output.CopyTo(newOutput);
				newOutput.AutogeneratedIdentityKey = false;
				newOutput.SetNextIdentityKey();
				newOutput.AssessmentCode = newAssessment.Code;
				newOutput.AssessmentLcStageId = lcStages[output.AssessmentLcStageId];
				if (output.InputId.HasValue)
					newOutput.InputId = sourceNewInputs[output.InputId.Value];

				newOutputs.Add(newOutput);
			}

			return newOutputs.Create();
		}

		private bool CopyBusinessCosts(DxAssessment sourceAssessment, DxAssessment newAssessment, Dictionary<decimal, decimal> lcStages)
		{
			DxBusinessCostCollection sourceBusinessCosts = new DxBusinessCostCollection(sourceAssessment.Code);
			sourceBusinessCosts.Load();

			DxBusinessCostCollection newBusinessCosts = new DxBusinessCostCollection();
			foreach (var businessCost in sourceBusinessCosts)
			{
				DxBusinessCost newBusinessCost = new DxBusinessCost();
				businessCost.CopyTo(newBusinessCost);
				newBusinessCost.AutogeneratedIdentityKey = false;
				newBusinessCost.SetNextIdentityKey();
				newBusinessCost.AssessmentCode = newAssessment.Code;
				newBusinessCost.AssessmentLcStageId = lcStages[businessCost.AssessmentLcStageId];

				newBusinessCosts.Add(newBusinessCost);
			}

			return newBusinessCosts.Create();
		}

		private bool CopyResourceNote(DxAssessment sourceAssessment, DxAssessment newAssessment, Dictionary<decimal, decimal> lcStages)
		{
			var sourceResourceNotes = new DxResourceNoteCollection(DxResourceNoteCollection.Filter.AssessmentCode, sourceAssessment.Code);
			sourceResourceNotes.Load();

			var newResourceNotes = new DxResourceNoteCollection();
			foreach (var resourceNote in sourceResourceNotes)
			{
				var newResourceNote = new DxResourceNote();
				resourceNote.CopyTo(newResourceNote);
				newResourceNote.AutogeneratedIdentityKey = false;
				newResourceNote.SetNextIdentityKey();
				newResourceNote.AssessmentCode = newAssessment.Code;
				newResourceNote.LCStageId = lcStages[resourceNote.LCStageId];
				newResourceNote.Type = resourceNote.Type;
				newResourceNote.Note = resourceNote.Note;

				newResourceNotes.Add(newResourceNote);
			}

			return newResourceNotes.Create();
		}

		#endregion Assessment Copy Methods

		#region Overrides

		protected override void InitializeResources(List<ControlResource> resources)
		{
			base.InitializeResources(resources);
			resources.Add(new ControlJsFile(DIRLinks.Areas.AssessmentNavigator.Views.CopyAssessment.CopyAssessment_ts));
		}

		protected override void FillScriptControlDescriptor(WebControls.GxScriptControlDescriptor scriptControlDescriptor)
		{
			base.FillScriptControlDescriptor(scriptControlDescriptor);

			scriptControlDescriptor.TypeName = "DX.DIRModule.AssessmentNavigator.CopyModel";
		}

		protected override DevExDialogPage.DialogButtonSet? GetButtonSetLayout()
		{
			return DevExDialogPage.DialogButtonSet.OkCancel;
		}

		#endregion Overrides
	}

	public sealed class CopyAssessmentDataJson : IJsonSerialized
	{
		public string Description { get; set; }
		public string ProdClassification { get; set; }
		public NullableDateTime TimeframeFrom { get; set; }
		public NullableDateTime TimeframeTo { get; set; }
		public string CompanyName { get; set; }
		public string Comments { get; set; }
		public string PhoneNumber { get; set; }
		public string OrgStructure { get; set; }
		public string Location { get; set; }
		public string DataQuality { get; set; }
	}
}