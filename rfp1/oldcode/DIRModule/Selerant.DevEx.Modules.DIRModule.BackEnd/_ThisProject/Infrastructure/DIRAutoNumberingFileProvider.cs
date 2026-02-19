using System;
using System.Collections.Generic;
using Selerant.ApplicationBlocks.PathManagement;
using Selerant.DevEx.BusinessLayer;
using Selerant.DevEx.Modules.DIRModule.Configuration.PathManagement;
using Selerant.DevEx.Modules.DIRModule.ModuleBlocks._ThisProject;

namespace Selerant.DevEx.Modules.DIRModule.BackEnd._ThisProject.Infrastructure
{
	public class DIRAutoNumberingFileProvider : IAutoNumberingFileProvider
	{
		public string ModuleCode => DIRModuleInfo.MODULE_CODE;

        public List<string> ProvideFileVirtualPaths()
        {
            return new List<string>()
            {
                PathFinder.Instance.GetFileVirtualPath(FileKeys.DIRAutoNumberingFile)
            };
        }
    }
}