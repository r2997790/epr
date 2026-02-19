using Selerant.Infrastructure.Modules;
using Selerant.Infrastructure.DependencyContainer.Wirings;

namespace Selerant.DevEx.Modules.DIRModule.ModuleBlocks._ThisProject
{
	public abstract class DIRWiring : SelerantModuleContainerLoader
	{
		public override IModuleInfo ModuleInfo => DIRModuleInfo.Instance;
	}
}