using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EPR.Web.Attributes;
using EPR.Data;

namespace EPR.Web.Controllers;

[Authorize]
public class ReportingController : Controller
{
    private readonly EPRDbContext _context;

    public ReportingController(EPRDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Dashboard(string? geography, DateTime? dateFrom, DateTime? dateTo)
    {
        var dateFromVal = dateFrom ?? DateTime.UtcNow.AddDays(-30);
        var dateToVal = dateTo ?? DateTime.UtcNow;
        ViewBag.DateFrom = dateFromVal.ToString("yyyy-MM-dd");
        ViewBag.DateTo = dateToVal.ToString("yyyy-MM-dd");
        ViewBag.SelectedGeography = geography ?? "";

        var totalProducts = await _context.Products.CountAsync();
        var totalPackagingTypes = await _context.PackagingTypes.CountAsync();
        var totalAsnShipments = await _context.AsnShipments.CountAsync();
        var totalProductForms = await _context.ProductForms.CountAsync();

        var productsByCategory = await _context.Products
            .Where(p => p.ProductCategory != null && p.ProductCategory != "")
            .GroupBy(p => p.ProductCategory)
            .Select(g => new { Category = g.Key ?? "Other", Count = g.Count() })
            .OrderByDescending(x => x.Count)
            .Take(10)
            .ToListAsync();

        var packagingByType = await _context.ProductForms
            .Where(p => p.PackagingType != null && p.PackagingType != "")
            .GroupBy(p => p.PackagingType)
            .Select(g => new { PackagingType = g.Key ?? "Other", Count = g.Count() })
            .OrderByDescending(x => x.Count)
            .Take(10)
            .ToListAsync();

        var maxCategoryCount = productsByCategory.Any() ? productsByCategory.Max(x => x.Count) : 0;
        var maxPackagingCount = packagingByType.Any() ? packagingByType.Max(x => x.Count) : 0;

        // Packaging distribution for doughnut chart (same as packagingByType, all for chart)
        var packagingDistribution = await _context.ProductForms
            .Where(p => p.PackagingType != null && p.PackagingType != "")
            .GroupBy(p => p.PackagingType)
            .Select(g => new { PackagingType = g.Key ?? "Other", Count = g.Count() })
            .OrderByDescending(x => x.Count)
            .ToListAsync();

        // Geographic distribution for bar chart (from Distributions by StateProvince), optional geography filter
        var distQuery = _context.Distributions
            .Where(d => d.StateProvince != null && d.StateProvince != "");
        if (!string.IsNullOrEmpty(geography))
            distQuery = distQuery.Where(d => d.StateProvince == geography);
        if (dateFrom.HasValue)
            distQuery = distQuery.Where(d => d.DispatchDate >= dateFrom.Value);
        if (dateTo.HasValue)
            distQuery = distQuery.Where(d => d.DispatchDate <= dateTo.Value);
        var geographicDistribution = await distQuery
            .GroupBy(d => d.StateProvince)
            .Select(g => new { Region = g.Key ?? "Other", Count = g.Count() })
            .OrderByDescending(x => x.Count)
            .ToListAsync();

        // Material composition for pie chart (from PackagingTypeMaterial -> PackagingRawMaterial, or fallback PackagingType)
        var materialFromTypes = await _context.PackagingTypeMaterials
            .Include(ptm => ptm.Material)
            .GroupBy(ptm => ptm.Material.Name)
            .Select(g => new { MaterialType = g.Key ?? "Other", Count = g.Count() })
            .OrderByDescending(x => x.Count)
            .Take(10)
            .ToListAsync();
        var materialComposition = materialFromTypes.Any()
            ? materialFromTypes
            : packagingByType.Select(p => new { MaterialType = p.PackagingType, Count = p.Count }).ToList();

        // Top "companies" (Brands) for table
        var topBrands = await _context.Products
            .Where(p => p.Brand != null && p.Brand != "")
            .GroupBy(p => p.Brand)
            .Select(g => new { CompanyName = g.Key ?? "Other", ProductCount = g.Count() })
            .OrderByDescending(x => x.ProductCount)
            .Take(10)
            .ToListAsync();
        var companyPerformance = topBrands.Select(b => new
        {
            b.CompanyName,
            b.ProductCount,
            AvgCost = 0m,
            ComplianceRate = 0.85m
        }).ToList();

        // Total weight in tonnes (from ProductForms.TotalPackagingWeight)
        var totalWeightGrams = await _context.ProductForms
            .Where(p => p.TotalPackagingWeight.HasValue)
            .SumAsync(p => (double)(p.TotalPackagingWeight ?? 0));
        var totalWeightTonnes = (decimal)(totalWeightGrams / 1_000_000.0);
        var distinctBrands = await _context.Products.Where(p => p.Brand != null && p.Brand != "").Select(p => p.Brand).Distinct().CountAsync();

        ViewBag.TotalProducts = totalProducts;
        ViewBag.TotalPackagingTypes = totalPackagingTypes;
        ViewBag.TotalAsnShipments = totalAsnShipments;
        ViewBag.TotalProductForms = totalProductForms;
        ViewBag.ProductsByCategory = productsByCategory;
        ViewBag.PackagingByType = packagingByType;
        ViewBag.MaxCategoryCount = maxCategoryCount;
        ViewBag.MaxPackagingCount = maxPackagingCount;
        ViewBag.PackagingDistribution = packagingDistribution;
        ViewBag.GeographicDistribution = geographicDistribution;
        ViewBag.MaterialComposition = materialComposition;
        ViewBag.CompanyPerformance = companyPerformance;
        ViewBag.TotalWeightTonnes = totalWeightTonnes;
        ViewBag.TotalCompanies = distinctBrands;
        ViewBag.AvgCostPerUnit = 0m;
        ViewBag.ComplianceRate = 0.78m;
        ViewBag.SustainabilityScore = 72;

        return View();
    }

    public async Task<IActionResult> PortfolioReport()
    {
        ViewData["Title"] = "Portfolio Report";
        var products = await _context.Products.AsNoTracking().OrderBy(p => p.Name).Take(500).ToListAsync();
        ViewBag.TotalCount = await _context.Products.CountAsync();
        return View(products);
    }

    public async Task<IActionResult> CostAnalysisReport()
    {
        ViewData["Title"] = "Cost Analysis";
        var products = await _context.Products.AsNoTracking().OrderBy(p => p.Name).Take(200).Select(p => new { p.Id, p.Name, p.Brand, p.ProductCategory }).ToListAsync();
        ViewBag.Products = products;
        return View();
    }

    public async Task<IActionResult> SustainabilityReport()
    {
        ViewData["Title"] = "Sustainability Report";
        var totalProducts = await _context.Products.CountAsync();
        var withForms = await _context.ProductForms.CountAsync();
        ViewBag.TotalProducts = totalProducts;
        ViewBag.WithPackagingForms = withForms;
        ViewBag.Score = 72;
        return View();
    }

    public async Task<IActionResult> ComplianceReport()
    {
        ViewData["Title"] = "Compliance Report";
        var totalProducts = await _context.Products.CountAsync();
        ViewBag.TotalProducts = totalProducts;
        ViewBag.ComplianceRate = 0.78m;
        return View();
    }
}
