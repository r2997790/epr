using Selerant.DevEx.Modules.DIRModule.BackEnd.AssessmentTypeManagement;
using Selerant.DevEx.Modules.DIRModule.BackEnd.AssessmentTypeManagement.LcStageTemplate;
using Selerant.DevEx.Modules.DIRModule.BackEnd.DestinationManagement;
using Selerant.DevEx.Modules.DIRModule.WebMvcFrontEnd.Helpers;
using Selerant.DevEx.Web;
using Selerant.DevEx.WebControls;
using Selerant.DevEx.WebMvcModules.Infrastructure.Controls;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.UI.WebControls;
//using Selerant.DevEx.BusinessLayer.Linq;

namespace Selerant.DevEx.Modules.DIRModule.WebMvcFrontEnd.Areas.AssessmentNavigator.Models.ObjectCreation
{
	public sealed class SecondCreateAssessmentModel : DialogViewIndexModel
	{
        public string DataQuality { get; set; }

		#region Constructors

		public SecondCreateAssessmentModel()
			: base(string.Empty)
		{
		}

		public SecondCreateAssessmentModel(string controllerUrl, ViewControlControllerData controllerData)
			: base(controllerUrl, controllerData)
		{
		}

		#endregion

		#region Methods

		public WebListItemCollection GetOrgStructureList()
		{
			return Helpers.LovListFactory.GetOrgStructures();
		}

		public List<SelectListItem> GetWasteDestinationsList()
		{
			var baseList = new DxDestinationCollection();
			baseList.Load();

			return baseList.Where(x => !GridHelpers.Instance.DefaultDestinations.Contains(x.Code)).Select(destination => new SelectListItem
			{
				Text = destination.Title,
				Value = destination.Code,
				Selected = false
			})
			.ToList();
		}

		public List<SelectListItem> GetAssessmentTypeList()
		{
			return Helpers.LovListFactory.GetAssessmentTypes();
		}

		public List<SelectListItem> FillLifecycleStages()
		{
			var result = new List<SelectListItem>();

			DxAssessmentTypeCollection assessmentTypes = new DxAssessmentTypeCollection();
			assessmentTypes.Load();

			DxLcStageTemplateCollection baseList = new DxLcStageTemplateCollection(assessmentTypes.First().Code);
			baseList.Load();

			foreach (DxLcStageTemplate item in baseList)
			{
				result.Add(new SelectListItem
				{
					Text = item.Title,
					Value = item.IdentityKey,
					Selected = false
				});
			}

			return result;
		}

        public WebListItemCollection GetDataQualityList()
        {
            return Helpers.LovListFactory.GetDataQualities();
        }

        #endregion
    }
}