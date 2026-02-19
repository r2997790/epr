using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Selerant.DevEx.Configuration.Grids.HtmlGrid;

namespace Selerant.DevEx.Modules.DIRModule.Configuration.Grids
{
	internal sealed class ModuleHtmlGridProvider : INeedToAddHtmlGrid
	{
		private readonly IHtmlGridOverlayCustomerRepository _overlayRepository;
        private readonly IHtmlGridOverlayUserRepository _overlayUserRepository;

		/// <summary>
		/// Default constructor
		/// </summary>
		/// <param name="overlayRepository">Repository which provides storage for customer settings as overlay</param>
		/// <param name="overlayUserRepository">Repository which provides storage for customer settings as overlay</param>
		public ModuleHtmlGridProvider(IHtmlGridOverlayCustomerRepository overlayRepository, IHtmlGridOverlayUserRepository overlayUserRepository)
		{
			_overlayRepository = overlayRepository ?? throw new ArgumentNullException(nameof(overlayRepository));
            _overlayUserRepository = overlayUserRepository ?? throw new ArgumentNullException(nameof(overlayUserRepository));
		}

		public IEnumerable<HtmlGridDefinitionVO> ProvideDefinitions()
		{
			var definitions = new List<HtmlGridDefinitionVO>();
			var types = GetType().Assembly.GetTypes().Where(x => x.IsClass && !x.IsAbstract && typeof(ICoreHtmlGridConfigurationRepository).IsAssignableFrom(x));
			foreach (var type in types)
			{
				definitions.Add(new HtmlGridDefinitionVO((ICoreHtmlGridConfigurationRepository)Activator.CreateInstance(type), _overlayRepository));
			}
			return definitions;
		}
	}
}
