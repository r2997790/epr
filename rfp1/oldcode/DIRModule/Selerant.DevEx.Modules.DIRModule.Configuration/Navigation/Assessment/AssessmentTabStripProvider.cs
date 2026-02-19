using Selerant.DevEx.Configuration.Navigator.TabStrip;
using System;

namespace Selerant.DevEx.Modules.DIRModule.Configuration.Navigation.Assessment
{
    /// <summary>
    /// Provider for vendor definition
    /// </summary>
    public class AssessementTabStripProvider : INeedToAddTabStrip
    {
        private readonly ITabStripCustomerOverlayRepository _tabStripCustomerOverlayRepository;
        private readonly ITabStripUserOverlayRepository _tabStripUserOverlayRepository;

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="tabStripOverlayRepository">Repository which provides storage for customer settings as overlay</param>
        /// <param name="tabStripUserOverlayRepository">Repository which provides storage for user settings as overlay</param>
        /// <exception cref="ArgumentNullException">If <paramref name="tabStripOverlayRepository"/></exception>
        public AssessementTabStripProvider(ITabStripCustomerOverlayRepository tabStripOverlayRepository,
            ITabStripUserOverlayRepository tabStripUserOverlayRepository)
        {
            _tabStripCustomerOverlayRepository = tabStripOverlayRepository ?? throw new ArgumentNullException(nameof(tabStripOverlayRepository));
            _tabStripUserOverlayRepository = tabStripUserOverlayRepository ?? throw new ArgumentNullException(nameof(tabStripUserOverlayRepository));
        }

        /// <summary>
        /// Gets the definition of navigator
        /// </summary>
        /// <returns></returns>
        public TabStripDefinitionVO ProvideDefinition()
        {
            var repository = new AssessmentTabStripCore();
            var vo = new TabStripDefinitionVO(repository.TabStripName, "ASSESSMENT", repository,
                _tabStripCustomerOverlayRepository, _tabStripUserOverlayRepository);

            return vo;
        }
    }
}