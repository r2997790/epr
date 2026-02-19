using Microsoft.AspNetCore.Mvc;
using EPR.Web.Attributes;
using EPR.Data.Services;
using EPR.Data;

namespace EPR.Web.Controllers;

[Authorize]
public class MaterialTaxonomyController : Controller
{
    private readonly EPRDbContext _context;
    private readonly MaterialTaxonomyImportService _importService;
    
    public MaterialTaxonomyController(EPRDbContext context, MaterialTaxonomyImportService importService)
    {
        _context = context;
        _importService = importService;
    }
    
    [HttpGet]
    public IActionResult Index()
    {
        var taxonomies = _context.MaterialTaxonomies
            .Where(t => t.Level == 1)
            .OrderBy(t => t.SortOrder)
            .ThenBy(t => t.DisplayName)
            .ToList();
        return View(taxonomies);
    }
    
    [HttpPost]
    public async Task<IActionResult> ImportExcel(IFormFile file)
    {
        if (file == null || file.Length == 0)
        {
            return Json(new { success = false, message = "No file uploaded" });
        }
        
        try
        {
            // Save uploaded file temporarily
            var tempPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString() + Path.GetExtension(file.FileName));
            using (var stream = new FileStream(tempPath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }
            
            // Import the data
            await _importService.ImportFromExcelAsync(tempPath);
            
            // Clean up temp file
            System.IO.File.Delete(tempPath);
            
            return Json(new { success = true, message = "Taxonomy data imported successfully" });
        }
        catch (Exception ex)
        {
            return Json(new { success = false, message = $"Error importing data: {ex.Message}" });
        }
    }
}






