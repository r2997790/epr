using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Selerant.DevEx.BusinessLayer;
using Selerant.DevEx.Modules.DIRModule.BackEnd.AssessmentManagement.BusinessObjects;
using Selerant.DevEx.WebControls;
using Selerant.DevEx.WebMvcModules.Infrastructure.Controls;
using Selerant.DevEx.WebPages;

namespace Selerant.DevEx.Modules.DIRModule.WebMvcFrontEnd.Areas.AssessmentNavigator.Models.Panels.GeneralPanel
{
	public sealed class GeneralModelPartial : ViewControlIndexModel
	{
		#region Properties

		public DxAssessment TargetAssessment { get; set; }

		public bool EditMode
		{
			get
			{
				switch (DisplayMode)
				{
					case DisplayModeType.Edit:
						return true;
					case DisplayModeType.View:
					default:
						return false;
				}
			}
		}

		public DxPhraseText ProdClassificationPhraseText => DxPhraseText.GetBest(DxPhraseTypeCategory.ListOfValuesStandard, "DXDIR_ASSESSMENT.PROD_CLASSIF", this.ProdClassification, DxUser.CurrentUser.ProgramCulture);

        public DxPhraseText DataQualityPhraseText => DxPhraseText.GetBest(DxPhraseTypeCategory.ListOfValuesAttributes, "99511", DataQuality, DxUser.CurrentUser.ProgramCulture);

		public string CompanyName { get; set; }
		public string Comments { get; set; }
		public DxUser Completionist { get; set; }
		public string Description { get; set; }
		public string ProdClassification { get; set; }
		public DxCountry Location { get; set; }
		public string OrgStructure { get; set; }
		public string Industry { get; set; }
		public string Phone { get; set; }
		public string Email { get; set; }
		public DisplayModeType DisplayMode { get; set; }
		public NullableDateTime TimeframeFrom { get; set; }
		public NullableDateTime TimeframeTo { get; set; }
		public string DataQuality { get; set; }

		#endregion Properties

		#region Constructor
		public GeneralModelPartial(string controllerUrl) : base(controllerUrl)
        {
        }

		public GeneralModelPartial(DxAssessment target)
			: base(null)
		{
			this.TargetAssessment = target;

			if (target.PersistenceStatus == DxPersistenceStatus.Unknown)
				target.Load();

			this.CompanyName = target.CompanyName;
			this.Comments = target.Comments;
			this.Completionist = target.Completionist;
			this.Email = target.Email;
			this.TimeframeFrom = target.TimeframeFrom;
			this.TimeframeTo = target.TimeframeTo;
			this.Phone = target.Phone;
			this.Description = target.Description;
			this.ProdClassification = target.ProdClassification;
			this.Location = !string.IsNullOrEmpty(target.Location) ? DxCountry.Get(target.Location) : null;
			this.OrgStructure = target.OrgStructure;
			this.Industry = target.AssessmentType.Code;
			this.DataQuality = target.DataQuality;
		}

		#endregion Constructor

		#region Overrides

		protected override void InitializeResources(List<ControlResource> resources)
		{
			base.InitializeResources(resources);
			resources.Add(new ControlJsFile(DIRLinks.Areas.AssessmentNavigator.Views.GeneralPanel.GeneralPanelPartial_ts));
			resources.Add(new ControlCssFile(DIRLinks.Areas.AssessmentNavigator.Views.GeneralPanel.GeneralPanel_css));
		}

		protected override void FillScriptControlDescriptor(GxScriptControlDescriptor scriptControlDescriptor)
		{
			base.FillScriptControlDescriptor(scriptControlDescriptor);
			scriptControlDescriptor.TypeName = "DX.DIRModule.AssessmentNavigator.GeneralPanelPartial";

			scriptControlDescriptor.DomData["DisplayMode"] = this.DisplayMode;
		}

		#endregion Overrides

		#region Methods

		public WebListItemCollection GetProdClassificationList()
		{
			return Helpers.LovListFactory.GetProdClassifications();
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

		#endregion

		#region Inner Classes
		public sealed class GeneralModelPartialJson : IJsonSerialized
		{
			public string TargetIdentifiableString { get; set; }
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

			public void UpdateTestTemplateFromJson(DxAssessment assessment)
			{
				if (assessment == null)
					return;

				if (assessment.PersistenceStatus == DxPersistenceStatus.Unknown)
					assessment.Load();

				assessment.CompanyName = this.CompanyName;
				assessment.Description = this.Description;
				assessment.Phone = this.PhoneNumber;
				assessment.TimeframeFrom = this.TimeframeFrom;
				assessment.TimeframeTo = this.TimeframeTo;
				assessment.ProdClassification = DxObject.ParseIdentifiableString<DxPhraseText>(this.ProdClassification).Code;
				assessment.OrgStructure = this.OrgStructure;
				assessment.ModDate = DateTime.Now;

				var location = DxObject.ParseIdentifiableString<DxCountry>(this.Location);

				assessment.Location = location.Code;

				assessment.CreateOrUpdateAttributeValue(assessment.CommentsAttributeName, this.Comments);
				assessment.CreateOrUpdateAttributeValue(assessment.DataQualityAttributeName, this.DataQuality);
				assessment.Update();
			}
			public void UpdateTestTemplateFromJson(string testTemplateIdentifiableString)
			{
				if (string.IsNullOrEmpty(testTemplateIdentifiableString))
					return;

				DxAssessment testTemplate = DxObject.ParseIdentifiableString(testTemplateIdentifiableString) as DxAssessment;
				UpdateTestTemplateFromJson(testTemplate);
			}
		}

		public enum DisplayModeType { Edit, View }

		#endregion Inner Classes
	}
}