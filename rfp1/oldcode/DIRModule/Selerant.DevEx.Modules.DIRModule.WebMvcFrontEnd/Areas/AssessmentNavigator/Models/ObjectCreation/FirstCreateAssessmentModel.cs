using Selerant.DevEx.BusinessLayer;
using Selerant.DevEx.Web;
using Selerant.DevEx.WebControls;
using Selerant.DevEx.WebMvcModules.Infrastructure.Controls;

namespace Selerant.DevEx.Modules.DIRModule.WebMvcFrontEnd.Areas.AssessmentNavigator.Models.ObjectCreation
{
	public sealed class FirstCreateAssessmentModel : DialogViewIndexModel
	{
		#region Constructors

		public FirstCreateAssessmentModel()
			: base(string.Empty)
		{
		}

		public FirstCreateAssessmentModel(string controllerUrl, ViewControlControllerData controllerData)
			: base(controllerUrl, controllerData)
		{
		}

		#endregion

		#region Methods

		public WebListItemCollection GetProdClassificationList()
		{
			return Helpers.LovListFactory.GetProdClassifications();
		}

		#endregion
	}
}