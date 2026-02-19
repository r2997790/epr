using System.Collections.Generic;
using Selerant.DevEx.Infrastructure.ComponentsData;
using Selerant.DevEx.WebMvcModules.Infrastructure.Navigators.Controllers;
using Selerant.DevEx.WebMvcModules.Infrastructure.Controls.HtmlGrid;
using Selerant.DevEx.Modules.DIRModule.ModuleBlocks._ThisProject;

namespace Selerant.DevEx.Modules.DIRModule.WebMvcFrontEnd._ThisProject
{
	internal class ComponentDescriptorExtensionProvider : IComponentDescriptorExtensionProvider
	{
		public string ProvidedBy => DIRModuleInfo.Instance.ModuleName;
		public IEnumerable<ComponentDescriptorExtension> Provide()
		{
			return new[] {
				new ComponentDescriptorExtension(typeof(SharingController), new Web.ComponentIdentifier(DIRModuleComponentIdentifier.ASSESSMENT_NAVIGATOR)),
				new ComponentDescriptorExtension(typeof(SharingActionsConverter), new Web.ComponentIdentifier(DIRModuleComponentIdentifier.ASSESSMENT_NAVIGATOR))
			};
		}
	}
}