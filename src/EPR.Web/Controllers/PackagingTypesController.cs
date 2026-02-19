using Microsoft.AspNetCore.Mvc;
using EPR.Web.Attributes;
using EPR.Domain.Entities;
using EPR.Data;
using Microsoft.EntityFrameworkCore;

namespace EPR.Web.Controllers;

[Authorize]
public class PackagingTypesController : Controller
{
    private readonly EPRDbContext _context;

    public PackagingTypesController(EPRDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var types = await _context.PackagingTypes
            .OrderBy(t => t.Name)
            .ToListAsync();
        return View(types);
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View(new PackagingType());
    }

    [HttpPost]
    public async Task<IActionResult> Create(PackagingType packagingType, IFormFile? imageFile)
    {
        if (ModelState.IsValid)
        {
            packagingType.CreatedAt = DateTime.UtcNow;
            packagingType.IsUserCreated = true;
            
            if (imageFile != null && imageFile.Length > 0)
            {
                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(imageFile.FileName);
                var path = Path.Combine("wwwroot", "images", "packaging", fileName);
                Directory.CreateDirectory(Path.GetDirectoryName(path)!);
                using (var stream = new FileStream(path, FileMode.Create))
                {
                    await imageFile.CopyToAsync(stream);
                }
                packagingType.ImageUrl = $"/images/packaging/{fileName}";
            }

            _context.PackagingTypes.Add(packagingType);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        return View(packagingType);
    }

    [HttpGet]
    public IActionResult PackagingBuilder()
    {
        return View();
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var type = await _context.PackagingTypes.FindAsync(id);
        if (type == null) return NotFound();
        return View(type);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(int id, PackagingType packagingType)
    {
        if (id != packagingType.Id) return NotFound();

        if (ModelState.IsValid)
        {
            packagingType.UpdatedAt = DateTime.UtcNow;
            _context.Update(packagingType);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        return View(packagingType);
    }
}
