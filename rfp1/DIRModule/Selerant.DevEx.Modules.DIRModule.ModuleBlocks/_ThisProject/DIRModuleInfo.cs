using Selerant.Infrastructure.Modules;

namespace Selerant.DevEx.Modules.DIRModule.ModuleBlocks._ThisProject
{
	/// <summary>
	/// Container for Information about DIRECT Module
	/// </summary>
	public class DIRModuleInfo : IModuleInfo
	{
		private static DIRModuleInfo _instance;
		private static object _instanceLock = new object();

		/// <summary>
		/// Name for DIRECT module.
		/// </summary>
		public const string MODULE_NAME = "DIRModule";
		/// <summary>
		/// Code for DIRECT module, used also for DB Manager
		/// </summary>
		public const string MODULE_CODE = "DIR";

		/// <summary>
		/// Name for DIRECT module.
		/// </summary>
		public string ModuleName => MODULE_NAME;

		/// <summary>
		/// Code for DIRECT module, used also for DB Manager
		/// </summary>
		public string ModuleCode => MODULE_CODE;

		public DIRModuleInfo()
		{
		}

		public static DIRModuleInfo Instance
		{
			get
			{
				if (_instance == null)
				{
					lock (_instanceLock)
					{
						if (_instance == null)
							_instance = new DIRModuleInfo();
					}
				}

				return _instance;
			}
		}
	}
}
