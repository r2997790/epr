using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;
using Selerant.DevEx.Modules.DIRModule.WebMvcFrontEnd.Areas.AssessmentNavigator.Security.Interfaces;
using Selerant.DevEx.Web;
using Selerant.DevEx.WebControls;
using Selerant.DevEx.WebMvcModules.Infrastructure.Controls;
using Selerant.DevEx.WebPages;

namespace Selerant.DevEx.Modules.DIRModule.WebMvcFrontEnd.Areas.AssessmentNavigator.Models.Dialogs.ManageAssessment
{
	public class ManageAssessmentDialogModel : DialogViewIndexModel<IManageAssessmentSecurity>
	{
		#region Inner Classes

		public class ManageRequest : IJsonNetSerialized
		{
			[JsonProperty(PropertyName = "entityType")]
			public EntityType EntityType { get; set; }

			[JsonProperty(PropertyName = "asmtIdentifiableString")]
			public string AsmtIdentifiableString { get; set; }

			[JsonProperty(PropertyName = "lcStageId")]
			public decimal? LcStageId { get; set; }

			[JsonProperty(PropertyName = "removedEntities")]
			public List<string> RemovedEntities { get; set; }

			[JsonProperty(PropertyName = "addedEntities")]
			public List<string> AddedEntities { get; set; }
		}

		#endregion

		#region Properties

		public EntityManager SpecificViewData { get; }

		#endregion

		public ManageAssessmentDialogModel(string controllerUrl, ViewControlControllerData controllerData, EntityManager specificViewData)
			: base(controllerUrl, controllerData)
		{
			SpecificViewData = specificViewData;
		}

		#region Overrides

		protected override void FillScriptControlDescriptor(GxScriptControlDescriptor scriptControlDescriptor)
		{
			base.FillScriptControlDescriptor(scriptControlDescriptor);
			scriptControlDescriptor.TypeName = "DX.DIRModule.AssessmentNavigator.ManageAssessmentDialog";

			scriptControlDescriptor.Data.Add("entityType", SpecificViewData.EntityType);
			scriptControlDescriptor.Data.Add("asmtIdentifiableString", SpecificViewData.Assessment.IdentifiableString);

			if (SpecificViewData.EntityType == EntityType.Destination)
				scriptControlDescriptor.Data.Add("currentlcStageId", SpecificViewData.LcStageId);
		}

		/// <summary>
		/// Initializes static resources
		/// </summary>
		/// <param name="resources"></param>
		protected override void InitializeResources(List<ControlResource> resources)
		{
			base.InitializeResources(resources);
			resources.Add(new ControlJsFile(DIRLinks.Areas.AssessmentNavigator.Views.ManageAssessmentDialog.ManageAssessmentDialog_ts));
		}

		protected override void InitializeForView()
		{
			base.InitializeForView();
			MenuModel.AddButton("CancelButton", ResText.Controls.GetString("DlgCancel"))
					 .AddButton("OkButton", ResText.Controls.GetString("BtnSave"), "confirm");
		}

		#endregion

	}
}