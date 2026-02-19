using Microsoft.Extensions.Logging;

namespace EPR.Web.Services;

public interface IBrowserTabsService
{
    void Initialize();
}

public class BrowserTabsService : IBrowserTabsService
{
    private readonly ILogger<BrowserTabsService>? _logger;

    public BrowserTabsService(ILogger<BrowserTabsService>? logger = null)
    {
        _logger = logger;
    }

    public void Initialize()
    {
        _logger?.LogInformation("BrowserTabsService initialized");
    }
}









