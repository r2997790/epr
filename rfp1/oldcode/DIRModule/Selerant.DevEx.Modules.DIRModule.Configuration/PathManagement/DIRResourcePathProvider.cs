using System.Collections.Generic;
using System.Linq;
using Selerant.ApplicationBlocks.PathManagement;
using Selerant.ApplicationBlocks.PathManagement.Interfaces;
using Selerant.DevEx.Modules.DIRModule.ModuleBlocks._ThisProject;

namespace Selerant.DevEx.Modules.DIRModule.Configuration.PathManagement
{
	/// <summary>
	/// Implementation of the <see cref="IResourcePathProvider"/> for path resources for DIR module
	/// </summary>
	public class DIRResourcePathProvider : IResourcePathProvider
	{
		#region Fields

		private IReadOnlyList<VirtualResourcePathInfo> _modulePaths;

		#endregion

		public string ModuleName => DIRModuleInfo.Instance.ModuleName;

		#region Constructors

		/// <summary>
		/// Default COnstructor
		/// </summary>
		public DIRResourcePathProvider()
		{
			_modulePaths = new List<VirtualResourcePathInfo>
			{
				// NOTE: Need to be changed "~/WritableFolder" to "@" after udfs can be created without passing physical or web application virtual pass
				new VirtualResourcePathInfo(FolderKeys.AssessmentUserFormsFolder, $"@/Modules/{DIRModuleInfo.Instance.ModuleName}/UserForms/Assessment/", PathDescendantType.Folder),
				new VirtualResourcePathInfo(FolderKeys.AssessmentMvcUserFormsFolder, $"@/Modules/{DIRModuleInfo.Instance.ModuleName}/UserFormsMvc/Assessment/", PathDescendantType.Folder),

				new VirtualResourcePathInfo(FileKeys.DIRAutoNumberingFile, $"@/Modules/{DIRModuleInfo.Instance.ModuleName}/AutoNumbering/AutoNumbering_DIR.cs", PathDescendantType.File)
			};
		}

		#endregion

		#region IResourcePathProvider Implementation

		public IReadOnlyList<VirtualResourcePathInfo> GetFiles()
		{
			return _modulePaths
				.Where(pathResource => pathResource.IsFile())
				.ToList();
		}

		public IReadOnlyList<VirtualResourcePathInfo> GetFolders()
		{
			return _modulePaths
				   .Where(pathResource => pathResource.IsFolder())
				   .ToList();
		}

		public IReadOnlyList<VirtualResourcePathInfo> GetResourcePaths()
		{
			return _modulePaths;
		}

		#endregion
	}
}