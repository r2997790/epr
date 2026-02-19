using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Selerant.DevEx.BusinessLayer;
using Selerant.DevEx.BusinessLayer.Navigation;
using Selerant.DevEx.Modules.DIRModule.BackEnd.AssessmentManagement.BusinessObjects;
using Selerant.DevEx.Modules.DIRModule.BackEnd.AssessmentTypeManagement.LcStageTemplate;
using Selerant.DevEx.Modules.DIRModule.Configuration.Resources;
using Selerant.DevEx.Modules.DIRModule.WebMvcFrontEnd.Areas.AssessmentNavigator.Security.Interfaces;
using Selerant.DevEx.Web;
using Selerant.DevEx.WebControls;
using Selerant.DevEx.WebMvcModules.Infrastructure.Controls;
using Selerant.DevEx.WebMvcModules.Infrastructure.Controls.Menu.Items;
using Selerant.DevEx.WebMvcModules.Infrastructure.Navigators.Models;
using Selerant.DevEx.WebPages;

namespace Selerant.DevEx.Modules.DIRModule.WebMvcFrontEnd.Areas.AssessmentNavigator.Models.ObjectCreation
{
	public class IndexModel : DialogViewIndexModel
    {
        public class CreateNewAssessmentDialogFormData : IJsonNetSerialized
        {
            [JsonProperty(PropertyName = "UserName")]
            public string UserName { get; set; }

            [JsonProperty(PropertyName = "CompanyName")]
            public string CompanyName { get; set; }

            [JsonProperty(PropertyName = "ProductName")]
            public string ProductName { get; set; }

            [JsonProperty(PropertyName = "TimeFrameFrom")]
            public DateTime TimeFrameFrom { get; set; }

			[JsonProperty(PropertyName = "TimeFrameTo")]
			public DateTime TimeFrameTo { get; set; }

			[JsonProperty(PropertyName = "ProdClassification")]
			public string ProdClassification { get; set; }

			[JsonProperty(PropertyName = "OrgStructure")]
			public string OrgStructure { get; set; }

			[JsonProperty(PropertyName = "Location")]
			public string Location { get; set; }

			[JsonProperty(PropertyName = "WasteDestinationCode")]
			public string[] WasteDestinations { get; set; }

			[JsonProperty(PropertyName = "Industry")]
			public string Industry { get; set; }

			[JsonProperty(PropertyName = "LifecycleStages")]
			public decimal[] LifecycleStages { get; set; }

			[JsonProperty(PropertyName = "DataQuality")]
			public string DataQuality { get; set; }
		}

		public FirstCreateAssessmentModel FirstStepModel { get; private set; }
		public SecondCreateAssessmentModel SecondStepModel { get; private set; }
		public ICreateAssessmentSecurity SecurityObject { get; set; }

		public IndexModel(string controllerUrl, ViewControlControllerData controllerData, DialogController dialogController, DevExObjectCreationSettings settings) 
            : base(controllerUrl, controllerData)
		{

        }

        public IndexModel(string controllerUrl, ViewControlControllerData controllerData)
            : base(controllerUrl, controllerData)
        {

			FirstStepModel = new FirstCreateAssessmentModel(controllerUrl, controllerData);
			SecondStepModel = new SecondCreateAssessmentModel(controllerUrl, controllerData);
			SecurityObject = controllerData.SecurityObject as ICreateAssessmentSecurity;

			MenuModel.AddItem(new ButtonMenuItem(DevExDialogPage.BUTTON_PREV_ID)
			{
				IsVisible = false,
			}.SetText(ResText.Controls.GetString("DlgPrev")))
			.AddButton(DevExDialogPage.BUTTON_CANCEL_ID, ResText.Controls.GetString("DlgCancel"))
			.AddButton(DevExDialogPage.BUTTON_NEXT_ID, ResText.Controls.GetString("DlgNext"), "confirm")
			.AddItem(new ButtonMenuItem(DevExDialogPage.BUTTON_FINISH_ID)
			{
				IsVisible = false,
				CssClass = "confirm"
			}.SetText(ResText.Controls.GetString("DlgFinish")));
		}

        #region Methods

        protected override void FillScriptControlDescriptor(GxScriptControlDescriptor scriptControlDescriptor)
        {
            base.FillScriptControlDescriptor(scriptControlDescriptor);
            scriptControlDescriptor.TypeName = "DX.DIRModule.AssessmentNavigator.CreateAssessmentDialog";

			scriptControlDescriptor.Data.Add("canOpenBusinessDataDialog", SecurityObject.CanViewResourceManagement);
		}

		protected override void InitializeResources(List<ControlResource> resources)
        {
            base.InitializeResources(resources);

            resources.Add(new ControlJsFile(DIRLinks.Areas.AssessmentNavigator.Views.CreateAssessmentDialog.CreateAssessmentDialog_ts));
            resources.Add(new ControlCssFile(DIRLinks.Areas.AssessmentNavigator.Views.CreateAssessmentDialog.CreateAssessmentDialog_css));
        }

		protected override void InitializeForView()
		{
			base.InitializeForView();
			AddCssClass("dir-module-dlg-create-assessment");
		}

		/// <summary>
		/// Create New Assessment
		/// </summary>
		/// <param name="viewModel"></param>
		/// <param name="message">Outcoming message</param>
		/// <returns>True if creation of a new Assessment is successful, false if not</returns>
		public bool CreateNewAssessment(CreateNewAssessmentDialogFormData formData, out string identifiableString, out string errorMessage)
        {
            bool result = false;
            identifiableString = string.Empty;
            string assmTypeCode = string.Empty;
            string assessmentCode = string.Empty;
			var assessmentType = formData.Industry;
			assessmentCode = DxAssessment.GetAutoNumberingKey(assessmentType, DxUser.CurrentUser);

			using (DxUnitOfWork unitOfWork = DxUnitOfWork.New())
			{
				DxAssessment newAssessment = new DxAssessment(assessmentCode);

				newAssessment.TypeCode = assessmentType;
				newAssessment.Description = formData.ProductName;
				newAssessment.Creator = DxUser.CurrentUser;
				newAssessment.CreatDate = DateTime.Now;
				newAssessment.Status = DxAssessment.AssessmentStatus.DRAFT;
				newAssessment.Completionist = DxUser.CurrentUser;
				newAssessment.ProdClassification = DxObject.ParseIdentifiableString<DxPhraseText>(formData.ProdClassification).Code;
				newAssessment.TimeframeFrom = formData.TimeFrameFrom;
				newAssessment.TimeframeTo = formData.TimeFrameTo;
				newAssessment.CompanyName = formData.CompanyName;
				newAssessment.OrgStructure = formData.OrgStructure;
				newAssessment.Location = DxObject.ParseIdentifiableString<DxCountry>(formData.Location).Code;

                newAssessment.Phone = newAssessment.Creator.Phone;

                bool assessmentCreate = newAssessment.Create();

				identifiableString = newAssessment.IdentifiableString;

				DxAttribute attribute = newAssessment.GetOrLoadAttribute("DXDIR_DATA_QUALITY");
				DxAttributeValue newValue = attribute.NewValue();
                newValue.Data = formData.DataQuality;

                attribute.AddValue(newValue);
				assessmentCreate &= attribute.Create();

				bool partnerAssessmentCreate = true;
				if (DxUser.CurrentUser.IsExternal)
					partnerAssessmentCreate = newAssessment.CreatePartnerAssessment();

				partnerAssessmentCreate = assessmentCreate && partnerAssessmentCreate;

				DxLcStageTemplateCollection lcStageTemplates = new DxLcStageTemplateCollection();
				lcStageTemplates.Load();

				DxAssessmentLcStageCollection assessmentLcStageCollection = new DxAssessmentLcStageCollection();
				foreach (var lcStage in formData.LifecycleStages)
				{
					DxAssessmentLcStage assessmentLcStage = new DxAssessmentLcStage();
					assessmentLcStage.SetNextIdentityKey();
					assessmentLcStage.AssessmentCode = newAssessment.Code;

					assessmentLcStage.Title = lcStageTemplates[lcStage].Title;
					assessmentLcStage.SortOrder = lcStageTemplates[lcStage].SortOrder;
					assessmentLcStage.Visible = true;
					assessmentLcStageCollection.Add(assessmentLcStage);
				}

				bool lcStageCreate = partnerAssessmentCreate && assessmentLcStageCollection.Create();

				DxAssessmentDestinationCollection assessmentDestCollection = new DxAssessmentDestinationCollection();
				foreach(var wasteDestination in formData.WasteDestinations)
				{
					DxAssessmentDestination wasteDest = new DxAssessmentDestination(assessmentCode, wasteDestination);
					assessmentDestCollection.Add(wasteDest);
				}

				bool wasteDestCreate = lcStageCreate && assessmentDestCollection.Create();

				if (wasteDestCreate)
				{
					result = true;
					errorMessage = "";
					unitOfWork.Commit();
				}
				else
				{
					result = false;
					errorMessage = Locale.GetString(ResourceFiles.AssessmentManager, "AssessmentCreationFailed");
					unitOfWork.Abort();
				}
			}

			return result;
		}

		#endregion
	}
}