using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EPR.Web.Attributes;
using EPR.Data;
using EPR.Web.Services;

namespace EPR.Web.Controllers;

[Authorize]
public class HomeController : Controller
{
    private readonly EPRDbContext _context;
    private readonly IDatasetService _datasetService;

    public HomeController(EPRDbContext context, IDatasetService datasetService)
    {
        _context = context;
        _datasetService = datasetService;
    }

    public async Task<IActionResult> Index()
    {
        var datasetKey = _datasetService.GetCurrentDataset();
        ViewBag.DatasetKey = datasetKey;

        var productsQuery = _context.Products.AsQueryable();
        if (!string.IsNullOrEmpty(datasetKey))
            productsQuery = productsQuery.Where(p => p.DatasetKey == datasetKey);

        var totalProducts = await productsQuery.CountAsync();

        var productsByCategory = await productsQuery
            .Where(p => p.ProductCategory != null && p.ProductCategory != "")
            .GroupBy(p => p.ProductCategory)
            .Select(g => new { Type = g.Key ?? "Other", Count = g.Count() })
            .OrderByDescending(x => x.Count)
            .Take(10)
            .ToListAsync();

        var productIds = await productsQuery.Select(p => p.Id).ToListAsync();
        var packagingByWeight = await _context.ProductForms
            .Where(p => productIds.Contains(p.ProductId) && p.PackagingType != null && p.PackagingType != "" && p.TotalPackagingWeight.HasValue)
            .GroupBy(p => p.PackagingType)
            .Select(g => new { PackagingType = g.Key ?? "Other", TotalWeight = g.Sum(x => (double)(x.TotalPackagingWeight ?? 0)), Count = g.Count() })
            .OrderByDescending(x => x.TotalWeight)
            .Take(10)
            .ToListAsync();

        var distributionQuery = _context.Distributions.AsQueryable();
        if (!string.IsNullOrEmpty(datasetKey))
            distributionQuery = distributionQuery.Where(d => d.DatasetKey == datasetKey);
        var distributionCount = await distributionQuery.CountAsync();

        ViewBag.TotalProducts = totalProducts;
        ViewBag.ProductsByType = productsByCategory;
        ViewBag.PackagingByWeight = packagingByWeight;
        ViewBag.DistributionCount = distributionCount;

        return View();
    }

    [HttpGet]
    public IActionResult ResetDataset()
    {
        _datasetService.ClearCurrentDataset();
        return RedirectToAction(nameof(Index));
    }
}

