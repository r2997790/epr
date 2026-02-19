using EPR.Web.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace EPR.Web.Extensions;

public static class BrowserTabsExtensions
{
    public static IServiceCollection AddBrowserTabs(this IServiceCollection services)
    {
        services.AddScoped<IBrowserTabsService, BrowserTabsService>();
        return services;
    }

    public static IApplicationBuilder UseBrowserTabs(this IApplicationBuilder app)
    {
        // Browser tabs middleware is handled via JavaScript
        // This method exists for API consistency
        return app;
    }
}

