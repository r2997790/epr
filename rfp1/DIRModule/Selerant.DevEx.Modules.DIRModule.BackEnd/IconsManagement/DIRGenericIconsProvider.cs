using Selerant.ApplicationBlocks.PathManagement;
using Selerant.DevEx.BusinessLayer.IconManagement;
using Selerant.DevEx.Modules.DIRModule.ModuleBlocks._ThisProject;

namespace Selerant.DevEx.Modules.DIRModule.BackEnd.IconsManagement
{
	public class DIRGenericIconsProvider : BaseGenericIconsProvider
	{
		public DIRGenericIconsProvider(PathFinder pathFinder)
			: base(DIRModuleInfo.MODULE_NAME,
				  PathFinder.FolderKeys.DxIconsFolder,
				  new EmbeddedResourceBasedIconsRepository("Selerant.DevEx.Modules.DIRModule.BackEnd.IconsManagement.Images", typeof(DIRGenericIconsProvider).Assembly),
				  pathFinder)
		{
		}
	}
}