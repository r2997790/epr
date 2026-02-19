using System.Collections.Generic;
using Selerant.DevEx.BusinessLayer;
using Selerant.DevEx.Modules.DIRModule.BackEnd.AssessmentManagement.Attributes;
using Selerant.DevEx.Modules.DIRModule.ModuleBlocks._ThisProject;

namespace Selerant.DevEx.Modules.DIRModule.BackEnd._ThisProject.Infrastructure.AttributesProvider
{
	public class DIRAttributeScopesProvider : IAttributeScopeProvider
	{
		/// <summary>
		/// who is providing the scopes, generally module name
		/// </summary>
		public string ProvidedFrom => DIRModuleInfo.Instance.ModuleCode;

		/// <summary>
		/// Gets the list of scopes from module
		/// </summary>
		/// <returns></returns>
		public List<DxAttributeScope> ProvideScopes()
		{
			List<DxAttributeScope> scopeList = new List<DxAttributeScope>()
			{
				new DxAssessmentAttributeScope()
			};
			return scopeList;
		}
	}
}