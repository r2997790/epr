namespace EPR.Web.Services;

/// <summary>
/// Service to get/set the current dataset for the user session.
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
    private readonly IHttpContextAccessor _httpContextAccessor;

    public DatasetService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public string? GetCurrentDataset()
    {
        return _httpContextAccessor.HttpContext?.Session?.GetString(SessionKey);
    }

    public void SetCurrentDataset(string? datasetKey)
    {
        if (_httpContextAccessor.HttpContext?.Session != null)
        {
            if (string.IsNullOrEmpty(datasetKey))
                _httpContextAccessor.HttpContext.Session.Remove(SessionKey);
            else
                _httpContextAccessor.HttpContext.Session.SetString(SessionKey, datasetKey);
        }
    }

    public void ClearCurrentDataset()
    {
        SetCurrentDataset(null);
    }
}
