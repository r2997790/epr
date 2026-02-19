using Selerant.DevEx.Modules.DIRModule.Configuration.Resources;
using Selerant.DevEx.Modules.DIRModule.ModuleBlocks._ThisProject;
using Selerant.DevEx.WebMvcModules.Infrastructure.Resources.Text;
using System.Collections.Generic;

namespace Selerant.DevEx.Modules.DIRModule.WebMvcFrontEnd._ThisProject.Infrastructure.Resources
{
    /// <summary>
    /// provider of resources file for DIR module
    /// </summary>
    public class DIRResourceFileProvider : IResourceFileProvider
    {
        /// <summary>
        /// The resource file for DIR
        /// </summary>
        /// <returns></returns>
        public static IResourceType ControlsResource()
        {
            return new ResourceFile(ResourceFiles.Controls);
        }
        /// <summary>
        /// The resource file for Assessment
        /// </summary>
        /// <returns></returns>
        public static IResourceType AssessmentManagerResource()
        {
            return new ResourceFile(ResourceFiles.AssessmentManager);
        }
        /// <summary>
        /// The resource file for DIR Admin Tools
        /// </summary>
        /// <returns></returns>
        public static IResourceType AdminToolsResource()
        {
            return new ResourceFile(ResourceFiles.AdminTools);
        }
        /// <summary>
        /// <see cref="IResourceFileProvider.GetResources()"/>
        /// </summary>
        /// <returns></returns>
        public IEnumerable<IResourceModule> GetResources()
        {
            return new List<IResourceModule>()
            {
                new ResourceFile(ResourceFiles.Controls),
                new ResourceFile(ResourceFiles.AssessmentManager),
                new ResourceFile(ResourceFiles.AdminTools)
            };
        }

    }

    /// <summary>
    /// class to wrap a single resource file (*.resx)
    /// </summary>
    public class ResourceFile : IResourceModule, IResourceType
    {
        readonly IResourceModule _Module;

        #region IResourceModule Properties

        /// <summary>
        /// <see cref="IResourceModule.Name"/>
        /// </summary>
        public string Name { get { return _Module.Name; } }
        /// <summary>
        /// <see cref="IResourceModule.ProvidedBy"/>
        /// </summary>
        public string ProvidedBy { get { return _Module.ProvidedBy; } }

        #endregion

        #region IResourceType Properties

        /// <summary>
        /// <see cref="IResourceType.FileName"/>
        /// </summary>
        public string FileName { get { return _Module.Name; } }

        #endregion

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="name"></param>
        public ResourceFile(string name)
        {
            var module = new DIRModuleInfo();
            this._Module = new ResourceModule(name, module.ModuleName);
        }

        /// <summary>
        /// <see cref="IResourceModule.GetLocalizedString(string)"/>
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public string GetLocalizedString(string key)
        {
            return this._Module.GetLocalizedString(key);
        }
    }
}