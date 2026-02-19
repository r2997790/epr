using Selerant.DevEx.BusinessLayer.Authorization;

namespace Selerant.DevEx.Modules.DIRModule.WebMvcFrontEnd.Areas.AssessmentNavigator.Security.Interfaces
{
	public interface IManageAssessmentSecurity : ISecurityObject
	{
		bool CanUpdateLcStages { get; }
		bool CanUpdateDestinations { get; }
	}
}
