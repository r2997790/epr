using Selerant.DevEx.Modules.DIRModule.BackEnd.AssessmentManagement.BusinessObjects;
using Selerant.DevEx.Modules.DIRModule.BackEnd.BusinessCostManagement;
using Selerant.DevEx.Modules.DIRModule.BackEnd.ResourceNote;
using Selerant.DevEx.Modules.DIRModule.Configuration.Resources;
using Selerant.DevEx.Web;
using Selerant.DevEx.WebControls;
using Selerant.DevEx.WebMvcModules.Infrastructure.Controls;
using Selerant.DevEx.WebMvcModules.Infrastructure.Navigators.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Selerant.DevEx.Modules.DIRModule.WebMvcFrontEnd.Areas.AssessmentNavigator.Models.Panels.ResourceManagement.Shared
{
	public class NotesModel : ViewControlIndexModel
	{
		#region Properties

		public DxAssessment Assessment { get; set; }
		public string ResourceType { get; set; }
		public decimal LCStageId { get; set; }
		public bool HasBusinessCostOther { get; set; }
		private BaseGridTabModel.ViewMode CtrlViewMode { get; set; }

		#endregion

		#region Constructors

		public NotesModel(string controllerUrl, NavigatorPanelControllerData<DxAssessment> controllerData, string resourceType, decimal lcStageId, string assessmentCode, BaseGridTabModel.ViewMode viewMode)
			: base(controllerUrl, controllerData)
		{
			Assessment = new DxAssessment(assessmentCode);
			ResourceType = resourceType;
			LCStageId = lcStageId;
			CtrlViewMode = viewMode;
		}

		public NotesModel(string controllerUrl, NavigatorPanelControllerData<DxAssessment> controllerData, string resourceType, decimal lcStageId, string assessmentCode)
			: base(controllerUrl, controllerData)
		{
			Assessment = new DxAssessment(assessmentCode);
			ResourceType = resourceType;
			LCStageId = lcStageId;
		}

		#endregion

		#region Lifecycle

		protected override void InitializeResources(List<ControlResource> resources)
		{
			base.InitializeResources(resources);

			resources.Add(new ControlJsFile(DIRLinks.Areas.AssessmentNavigator.Views.ResourceManagement.Shared.ResourceNote_ts));
		}

		protected override void FillScriptControlDescriptor(GxScriptControlDescriptor scriptControlDescriptor)
		{
			base.FillScriptControlDescriptor(scriptControlDescriptor);
			scriptControlDescriptor.TypeName = "DX.DIRModule.AssessmentNavigator.ResourceManagement.Shared.ResourceNote";

			scriptControlDescriptor.Data.Add("resourceType", ResourceType);
			scriptControlDescriptor.Data.Add("lcStageId", LCStageId);
			scriptControlDescriptor.Data.Add("assessmentCode", Assessment.Code);
			scriptControlDescriptor.Data.Add("viewMode", CtrlViewMode.ToString());

		}

		#endregion

		#region Methods

		public bool SaveNotes(string notes, out string message)
		{
			var resourceNote = new DxResourceNote(Assessment.Code, LCStageId, ResourceType);
			message = string.Empty;

			if (resourceNote.Exists())
			{
				resourceNote.Load();
				resourceNote.Note = notes;
				message = Locale.GetString(ResourceFiles.AssessmentManager, "ResMngmt_NotesUpdated");

				return resourceNote.Update();
			}
			else
			{
				resourceNote.Note = notes;
				message = Locale.GetString(ResourceFiles.AssessmentManager, "ResMngmt_NotesAdded");

				return resourceNote.Create();
			}
		}

		public string GetNotesByResourceType(string resourceType)
		{
			var resourceNote = new DxResourceNote(Assessment.Code, LCStageId, resourceType);
			resourceNote.Load();

			return resourceNote.Note;
		}

		public void DeleteBusinessCostOtherNotes()
		{
			var resourceNote = new DxResourceNote(Assessment.Code, LCStageId, ResourceType);
			
			if (resourceNote.Exists())
			{
				resourceNote.Delete();
			}
		}

		public bool DoesHaveOtherBusinessCost()
		{
			var businessCosts = new DxBusinessCostCollection(Assessment.Code, LCStageId, true);

			return businessCosts.Any(x => x.Title == BusinessCostGridTabModel.OTHER);
		}

		#endregion
	}
}