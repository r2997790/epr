namespace EPR.Web.Services;

/// <summary>
/// Service to get/set the current dataset for the user session.
/// Uses both session and cookie so dataset persists across Railway multi-instance and tab loads.
/// </summary>
public interface IDatasetService
{
    string? GetCurrentDataset();
    void SetCurrentDataset(string? datasetKey);
    void ClearCurrentDataset();
}

public class DatasetService : IDatasetService
{
    public const string SessionKey = "DatasetKey";
    public const string CookieKey = "EPR_DatasetKey";
    private const int CookieMaxAgeDays = 30;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public DatasetService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public string? GetCurrentDataset()
    {
        var ctx = _httpContextAccessor.HttpContext;
        if (ctx == null) return null;

        // Session first (primary)
        var fromSession = ctx.Session?.GetString(SessionKey);
        if (!string.IsNullOrEmpty(fromSession)) return fromSession;

        // Cookie fallback (persists across instances, tab loads, and when session is lost)
        if (ctx.Request.Cookies.TryGetValue(CookieKey, out var fromCookie) && !string.IsNullOrEmpty(fromCookie))
            return fromCookie;

        return null;
    }

    public void SetCurrentDataset(string? datasetKey)
    {
        var ctx = _httpContextAccessor.HttpContext;
        if (ctx == null) return;

        if (ctx.Session != null)
        {
            if (string.IsNullOrEmpty(datasetKey))
                ctx.Session.Remove(SessionKey);
            else
                ctx.Session.SetString(SessionKey, datasetKey);
        }

        // Also set/clear cookie so dataset persists across Railway instances and tab loads
        if (string.IsNullOrEmpty(datasetKey))
            ctx.Response.Cookies.Delete(CookieKey);
        else
            ctx.Response.Cookies.Append(CookieKey, datasetKey, new CookieOptions
            {
                HttpOnly = true,
                SecurePolicy = CookieSecurePolicy.SameAsRequest,
                SameSite = SameSiteMode.Lax,
                MaxAge = TimeSpan.FromDays(CookieMaxAgeDays),
                Path = "/"
            });
    }

    public void ClearCurrentDataset()
    {
        SetCurrentDataset(null);
    }
}
