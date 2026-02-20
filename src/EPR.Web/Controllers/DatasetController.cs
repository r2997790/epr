using Microsoft.AspNetCore.Mvc;
using EPR.Web.Attributes;
using EPR.Web.Services;

namespace EPR.Web.Controllers;

[Authorize]
[ApiController]
[Route("api/dataset")]
public class DatasetController : ControllerBase
{
    private readonly IDatasetService _datasetService;

    public DatasetController(IDatasetService datasetService)
    {
        _datasetService = datasetService;
    }

    [HttpPost("select")]
    [IgnoreAntiforgeryToken]
    public IActionResult Select([FromBody] SelectDatasetRequest request)
    {
        if (string.IsNullOrWhiteSpace(request?.DatasetKey))
            return BadRequest(new { error = "DatasetKey is required" });

        _datasetService.SetCurrentDataset(request.DatasetKey.Trim());
        return Ok(new { success = true, datasetKey = request.DatasetKey.Trim() });
    }

    [HttpGet("current")]
    public IActionResult GetCurrent()
    {
        var key = _datasetService.GetCurrentDataset();
        return Ok(new { datasetKey = key });
    }

    public class SelectDatasetRequest
    {
        public string? DatasetKey { get; set; }
    }
}
