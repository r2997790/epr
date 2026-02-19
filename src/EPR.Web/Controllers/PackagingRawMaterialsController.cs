using Microsoft.AspNetCore.Mvc;
using EPR.Web.Attributes;
using EPR.Domain.Entities;
using EPR.Data;
using Microsoft.EntityFrameworkCore;

namespace EPR.Web.Controllers;

[Authorize]
public class PackagingRawMaterialsController : Controller
{
    private readonly EPRDbContext _context;

    public PackagingRawMaterialsController(EPRDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var materials = await _context.PackagingRawMaterials
            .Include(m => m.SubMaterials)
            .Include(m => m.ParentMaterial)
            .Include(m => m.Jurisdictions)
                .ThenInclude(j => j.Jurisdiction)
            .OrderBy(m => m.Name)
            .ToListAsync();
        return View(materials);
    }

    [HttpGet]
    public IActionResult Create()
    {
        ViewBag.ParentMaterials = _context.PackagingRawMaterials
            .Where(m => m.ParentMaterialId == null)
            .OrderBy(m => m.Name)
            .ToList();
        return View(new PackagingRawMaterial());
    }

    [HttpPost]
    public async Task<IActionResult> Create(PackagingRawMaterial material)
    {
        if (ModelState.IsValid)
        {
            material.CreatedAt = DateTime.UtcNow;
            _context.PackagingRawMaterials.Add(material);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        ViewBag.ParentMaterials = _context.PackagingRawMaterials
            .Where(m => m.ParentMaterialId == null)
            .OrderBy(m => m.Name)
            .ToList();
        return View(material);
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var material = await _context.PackagingRawMaterials
            .Include(m => m.Jurisdictions)
            .FirstOrDefaultAsync(m => m.Id == id);
        if (material == null) return NotFound();
        
        ViewBag.ParentMaterials = _context.PackagingRawMaterials
            .Where(m => m.ParentMaterialId == null && m.Id != id)
            .OrderBy(m => m.Name)
            .ToList();
        return View(material);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(int id, PackagingRawMaterial material)
    {
        if (id != material.Id) return NotFound();

        if (ModelState.IsValid)
        {
            material.UpdatedAt = DateTime.UtcNow;
            _context.Update(material);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        ViewBag.ParentMaterials = _context.PackagingRawMaterials
            .Where(m => m.ParentMaterialId == null && m.Id != id)
            .OrderBy(m => m.Name)
            .ToList();
        return View(material);
    }

    [HttpGet]
    public async Task<IActionResult> AddJurisdiction(int materialId)
    {
        var material = await _context.PackagingRawMaterials.FindAsync(materialId);
        if (material == null) return NotFound();

        ViewBag.Material = material;
        ViewBag.Jurisdictions = await _context.Jurisdictions.OrderBy(j => j.Name).ToListAsync();
        return View(new MaterialJurisdiction { MaterialId = materialId, EffectiveFrom = DateTime.UtcNow });
    }

    [HttpPost]
    public async Task<IActionResult> AddJurisdiction(MaterialJurisdiction materialJurisdiction)
    {
        if (ModelState.IsValid)
        {
            materialJurisdiction.CreatedAt = DateTime.UtcNow;
            _context.MaterialJurisdictions.Add(materialJurisdiction);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Edit), new { id = materialJurisdiction.MaterialId });
        }
        ViewBag.Material = await _context.PackagingRawMaterials.FindAsync(materialJurisdiction.MaterialId);
        ViewBag.Jurisdictions = await _context.Jurisdictions.OrderBy(j => j.Name).ToListAsync();
        return View(materialJurisdiction);
    }

    [HttpGet]
    public Task<IActionResult> Export(int? id)
    {
        // Export to XLSX functionality
        return Task.FromResult<IActionResult>(Json(new { message = "Export functionality coming soon" }));
    }
}
