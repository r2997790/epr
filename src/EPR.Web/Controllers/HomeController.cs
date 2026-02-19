using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EPR.Web.Attributes;
using EPR.Data;

namespace EPR.Web.Controllers;

[Authorize]
public class HomeController : Controller
{
    private readonly EPRDbContext _context;

    public HomeController(EPRDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var totalProducts = await _context.Products.CountAsync();

        var productsByCategory = await _context.Products
            .Where(p => p.ProductCategory != null && p.ProductCategory != "")
            .GroupBy(p => p.ProductCategory)
            .Select(g => new { Type = g.Key ?? "Other", Count = g.Count() })
            .OrderByDescending(x => x.Count)
            .Take(10)
            .ToListAsync();

        var packagingByWeight = await _context.ProductForms
            .Where(p => p.PackagingType != null && p.PackagingType != "" && p.TotalPackagingWeight.HasValue)
            .GroupBy(p => p.PackagingType)
            .Select(g => new { PackagingType = g.Key ?? "Other", TotalWeight = g.Sum(x => (double)(x.TotalPackagingWeight ?? 0)), Count = g.Count() })
            .OrderByDescending(x => x.TotalWeight)
            .Take(10)
            .ToListAsync();

        var distributionCount = await _context.Distributions.CountAsync();

        ViewBag.TotalProducts = totalProducts;
        ViewBag.ProductsByType = productsByCategory;
        ViewBag.PackagingByWeight = packagingByWeight;
        ViewBag.DistributionCount = distributionCount;

        return View();
    }
}

