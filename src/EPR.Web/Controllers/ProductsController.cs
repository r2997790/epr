using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EPR.Web.Attributes;
using EPR.Data;
using EPR.Domain.Entities;
using EPR.Web.Services;

namespace EPR.Web.Controllers;

[Authorize]
public class ProductsController : Controller
{
    private readonly EPRDbContext _context;
    private readonly IDatasetService _datasetService;

    public ProductsController(EPRDbContext context, IDatasetService datasetService)
    {
        _context = context;
        _datasetService = datasetService;
    }

    public async Task<IActionResult> Index(string search = "", int page = 1, int pageSize = 12)
    {
        const int maxPageSize = 100;
        if (pageSize <= 0 || pageSize > maxPageSize) pageSize = 12;
        if (page <= 0) page = 1;

        var datasetKey = _datasetService.GetCurrentDataset();
        var query = _context.Products.AsNoTracking();
        if (!string.IsNullOrEmpty(datasetKey))
            query = query.Where(p => p.DatasetKey == datasetKey);
        var searchTerm = (search ?? "").Trim();
        if (!string.IsNullOrEmpty(searchTerm))
        {
            var term = $"%{searchTerm}%";
            query = query.Where(p =>
                EF.Functions.Like(p.Name, term) ||
                (p.Brand != null && EF.Functions.Like(p.Brand, term)) ||
                (p.Sku != null && EF.Functions.Like(p.Sku, term)) ||
                (p.Gtin != null && EF.Functions.Like(p.Gtin, term)));
        }

        var totalCount = await query.CountAsync();
        var items = await query
            .OrderBy(p => p.Name)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(p => new ProductListItemVm
            {
                Id = p.Id,
                Name = p.Name,
                Brand = p.Brand ?? "",
                Manufacturer = p.Brand ?? "",
                Sku = p.Sku ?? "",
                ProductCategory = p.ProductCategory ?? "",
                CountryOfOrigin = p.CountryOfOrigin ?? "",
                ImageUrl = p.ImageUrl
            })
            .ToListAsync();

        var vm = new ProductListVm
        {
            Products = items,
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize,
            Search = searchTerm
        };
        return View(vm);
    }

    public async Task<IActionResult> Detail(int id, string? returnUrl = null)
    {
        var datasetKey = _datasetService.GetCurrentDataset();
        var query = _context.Products.AsQueryable();
        if (!string.IsNullOrEmpty(datasetKey))
            query = query.Where(p => p.DatasetKey == datasetKey);
        var product = await query.FirstOrDefaultAsync(p => p.Id == id);
        if (product == null)
            return NotFound();

        var productForm = await _context.ProductForms
            .FirstOrDefaultAsync(pf => pf.ProductId == id);

        var relatedAsns = await GetRelatedAsnShipments(product.Gtin, datasetKey);

        var vm = new ProductDetailVm
        {
            Product = product,
            ProductForm = productForm,
            ReturnUrl = returnUrl,
            RelatedAsns = relatedAsns
        };
        return View(vm);
    }

    private static string NormalizeGtin(string? g) =>
        string.IsNullOrWhiteSpace(g) ? "" : g.Trim().PadLeft(14, '0').Length > 14 ? g.Trim() : g.Trim().PadLeft(14, '0');

    private async Task<List<RelatedAsnItem>> GetRelatedAsnShipments(string? productGtin, string? datasetKey)
    {
        var list = new List<RelatedAsnItem>();
        var normalized = NormalizeGtin(productGtin);
        if (string.IsNullOrEmpty(normalized)) return list;

        var lineItems = await (
            from li in _context.AsnLineItems
            join p in _context.AsnPallets on li.AsnPalletId equals p.Id
            join s in _context.AsnShipments on p.AsnShipmentId equals s.Id
            where li.Gtin != null && (string.IsNullOrEmpty(datasetKey) || s.DatasetKey == datasetKey)
            select new { li.Gtin, s.Id }
        ).ToListAsync();

        var shipmentIds = lineItems
            .Where(x => NormalizeGtin(x.Gtin) == normalized)
            .Select(x => x.Id)
            .Distinct()
            .ToList();

        if (shipmentIds.Count == 0) return list;

        return await _context.AsnShipments
            .Where(s => shipmentIds.Contains(s.Id))
            .OrderByDescending(s => s.ShipDate)
            .Take(15)
            .Select(s => new RelatedAsnItem { Id = s.Id, AsnNumber = s.AsnNumber!, ShipDate = s.ShipDate })
            .ToListAsync();
    }

    public class ProductListItemVm
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public string Brand { get; set; } = "";
        public string Manufacturer { get; set; } = "";
        public string Sku { get; set; } = "";
        public string ProductCategory { get; set; } = "";
        public string CountryOfOrigin { get; set; } = "";
        public string? ImageUrl { get; set; }
    }

    public class ProductListVm
    {
        public List<ProductListItemVm> Products { get; set; } = new();
        public int TotalCount { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public string Search { get; set; } = "";
    }

    public class ProductDetailVm
    {
        public Product Product { get; set; } = null!;
        public ProductForm? ProductForm { get; set; }
        public string? ReturnUrl { get; set; }
        public List<RelatedAsnItem> RelatedAsns { get; set; } = new();
    }

    public class RelatedAsnItem
    {
        public int Id { get; set; }
        public string AsnNumber { get; set; } = "";
        public DateTime? ShipDate { get; set; }
    }
}









