using Selerant.DevEx.BusinessLayer.Authorization;

namespace Selerant.DevEx.Modules.DIRModule.WebMvcFrontEnd.Areas.AssessmentNavigator.Security.Interfaces
{
	public interface ICreateAssessmentSecurity : ISecurityObject
	{
		bool CanCreateAssessment { get; }

		bool CanViewResourceManagement { get; }
	}
}