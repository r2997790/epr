using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EPR.Web.Attributes;
using EPR.Data;
using EPR.Web.Services;

namespace EPR.Web.Controllers;

[Authorize]
public class ReportingController : Controller
{
    private readonly EPRDbContext _context;
    private readonly IDatasetService _datasetService;

    public ReportingController(EPRDbContext context, IDatasetService datasetService)
    {
        _context = context;
        _datasetService = datasetService;
    }

    public async Task<IActionResult> Dashboard(string? geography, DateTime? dateFrom, DateTime? dateTo)
    {
        var datasetKey = _datasetService.GetCurrentDataset();
        var productsQuery = _context.Products.AsQueryable();
        if (!string.IsNullOrEmpty(datasetKey))
            productsQuery = productsQuery.Where(p => p.DatasetKey == datasetKey);
        var productIds = await productsQuery.Select(p => p.Id).ToListAsync();

        var dateFromVal = dateFrom ?? DateTime.UtcNow.AddDays(-30);
        var dateToVal = dateTo ?? DateTime.UtcNow;
        ViewBag.DateFrom = dateFromVal.ToString("yyyy-MM-dd");
        ViewBag.DateTo = dateToVal.ToString("yyyy-MM-dd");
        ViewBag.SelectedGeography = geography ?? "";

        var totalProducts = await productsQuery.CountAsync();
        var totalPackagingTypes = await _context.PackagingTypes.CountAsync();
        var asnQuery = _context.AsnShipments.AsQueryable();
        if (!string.IsNullOrEmpty(datasetKey))
            asnQuery = asnQuery.Where(s => s.DatasetKey == datasetKey);
        var totalAsnShipments = await asnQuery.CountAsync();
        var totalProductForms = await _context.ProductForms.CountAsync(pf => productIds.Contains(pf.ProductId));

        var productsByCategory = await productsQuery
            .Where(p => p.ProductCategory != null && p.ProductCategory != "")
            .GroupBy(p => p.ProductCategory)
            .Select(g => new { Category = g.Key ?? "Other", Count = g.Count() })
            .OrderByDescending(x => x.Count)
            .Take(10)
            .ToListAsync();

        var packagingByType = await _context.ProductForms
            .Where(p => productIds.Contains(p.ProductId) && p.PackagingType != null && p.PackagingType != "")
            .GroupBy(p => p.PackagingType)
            .Select(g => new { PackagingType = g.Key ?? "Other", Count = g.Count() })
            .OrderByDescending(x => x.Count)
            .Take(10)
            .ToListAsync();

        var maxCategoryCount = productsByCategory.Any() ? productsByCategory.Max(x => x.Count) : 0;
        var maxPackagingCount = packagingByType.Any() ? packagingByType.Max(x => x.Count) : 0;

        // Packaging distribution for doughnut chart (same as packagingByType, all for chart)
        var packagingDistribution = await _context.ProductForms
            .Where(p => productIds.Contains(p.ProductId) && p.PackagingType != null && p.PackagingType != "")
            .GroupBy(p => p.PackagingType)
            .Select(g => new { PackagingType = g.Key ?? "Other", Count = g.Count() })
            .OrderByDescending(x => x.Count)
            .ToListAsync();

        // Geographic distribution for bar chart (from Distributions by StateProvince), optional geography filter
        var distQuery = _context.Distributions
            .Where(d => d.StateProvince != null && d.StateProvince != "");
        if (!string.IsNullOrEmpty(datasetKey))
            distQuery = distQuery.Where(d => d.DatasetKey == datasetKey);
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
        var topBrands = await productsQuery
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
            .Where(p => productIds.Contains(p.ProductId) && p.TotalPackagingWeight.HasValue)
            .SumAsync(p => (double)(p.TotalPackagingWeight ?? 0));
        var totalWeightTonnes = (decimal)(totalWeightGrams / 1_000_000.0);
        var distinctBrands = await productsQuery.Where(p => p.Brand != null && p.Brand != "").Select(p => p.Brand).Distinct().CountAsync();

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
        var datasetKey = _datasetService.GetCurrentDataset();
        var productsQuery = _context.Products.AsNoTracking();
        if (!string.IsNullOrEmpty(datasetKey))
            productsQuery = productsQuery.Where(p => p.DatasetKey == datasetKey);
        var products = await productsQuery.OrderBy(p => p.Name).Take(500).ToListAsync();
        ViewBag.TotalCount = await productsQuery.CountAsync();
        return View(products);
    }

    public async Task<IActionResult> CostAnalysisReport()
    {
        ViewData["Title"] = "Cost Analysis";
        var datasetKey = _datasetService.GetCurrentDataset();
        var productsQuery = _context.Products.AsNoTracking();
        if (!string.IsNullOrEmpty(datasetKey))
            productsQuery = productsQuery.Where(p => p.DatasetKey == datasetKey);
        var products = await productsQuery.OrderBy(p => p.Name).Take(200).Select(p => new { p.Id, p.Name, p.Brand, p.ProductCategory }).ToListAsync();
        ViewBag.Products = products;
        return View();
    }

    public async Task<IActionResult> SustainabilityReport()
    {
        ViewData["Title"] = "Sustainability Report";
        var datasetKey = _datasetService.GetCurrentDataset();
        var productsQuery = _context.Products.AsQueryable();
        if (!string.IsNullOrEmpty(datasetKey))
            productsQuery = productsQuery.Where(p => p.DatasetKey == datasetKey);
        var productIds = await productsQuery.Select(p => p.Id).ToListAsync();
        var totalProducts = await productsQuery.CountAsync();
        var withForms = await _context.ProductForms.CountAsync(pf => productIds.Contains(pf.ProductId));
        ViewBag.TotalProducts = totalProducts;
        ViewBag.WithPackagingForms = withForms;
        ViewBag.Score = 72;
        return View();
    }

    public async Task<IActionResult> ComplianceReport()
    {
        ViewData["Title"] = "Compliance Report";
        var datasetKey = _datasetService.GetCurrentDataset();
        var productsQuery = _context.Products.AsQueryable();
        if (!string.IsNullOrEmpty(datasetKey))
            productsQuery = productsQuery.Where(p => p.DatasetKey == datasetKey);
        var totalProducts = await productsQuery.CountAsync();
        ViewBag.TotalProducts = totalProducts;
        ViewBag.ComplianceRate = 0.78m;
        return View();
    }
}
