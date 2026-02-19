using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Selerant.DevEx.Modules.DIRModule.BackEnd.AssessmentManagement.BusinessObjects;

namespace Selerant.DevEx.Modules.DIRModule.WebMvcFrontEnd.Areas.AssessmentNavigator.Models.Dialogs.ManageAssessment
{
	public abstract class EntityManager
	{
		#region Properties

		public DxAssessment Assessment { get; }
		public virtual decimal LcStageId { get; }
		public EntityType EntityType { get; }

		public string AvailableListboxTitle { get; protected set; }
		public string UsedListboxTitle { get; protected set; }

		public List<SelectListItem> AvailableListboxItems { get; protected set; }
		public List<SelectListItem> UsedListBoxItems { get; protected set; }

		#endregion

		#region Constructors

		public EntityManager(DxAssessment assessment, EntityType entityType)
		{
			Assessment = assessment;
			EntityType = entityType;
		}

		#endregion

		/// <summary>
		/// Call this method only when is instanced for view, to populate data to be displayed on view
		/// </summary>
		/// <returns></returns>
		public abstract EntityManager SetupViewData();

		public abstract bool Manage(List<string> removedEntities, List<string> addedEntities);
	}
}