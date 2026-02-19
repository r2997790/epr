using Microsoft.AspNetCore.Mvc;
using EPR.Web.Attributes;
using EPR.Domain.Entities;
using EPR.Data;
using Microsoft.EntityFrameworkCore;
using System.IO;

namespace EPR.Web.Controllers;

[Authorize]
public class VisualEditorController : Controller
{
    private readonly EPRDbContext _context;

    public VisualEditorController(EPRDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public IActionResult Index()
    {
        return View();
    }

    // API Endpoints for Visual Editor

    [HttpGet]
    [Route("api/visual-editor/raw-materials")]
    public async Task<IActionResult> GetRawMaterials()
    {
        try
        {
            // Try to get Level 1 taxonomy items for the toolbar
            var level1Taxonomies = await _context.MaterialTaxonomies
                .Where(t => t.Level == 1 && t.IsActive)
                .OrderBy(t => t.SortOrder)
                .ThenBy(t => t.DisplayName)
                .Select(t => new
                {
                    id = t.Id,
                    code = t.Code ?? string.Empty,
                    name = t.DisplayName,
                    description = t.Description,
                    iconClass = t.IconClass ?? "bi-circle"
                })
                .ToListAsync();
            
            // If taxonomy data exists, return it
            if (level1Taxonomies.Any())
            {
                return Json(level1Taxonomies);
            }
        }
        catch (Exception)
        {
            // If MaterialTaxonomies table doesn't exist or query fails, fall back to PackagingRawMaterials
        }
        
        // Fall back to old PackagingRawMaterials if no taxonomy data exists
        try
        {
            var materials = await _context.PackagingRawMaterials
                .Where(m => m.ParentMaterialId == null)
                .OrderBy(m => m.Name)
                .Select(m => new
                {
                    id = m.Id,
                    code = string.Empty,
                    name = m.Name,
                    description = m.Description,
                    iconClass = GetDefaultIconForMaterial(m.Name)
                })
                .ToListAsync();
            return Json(materials);
        }
        catch (Exception)
        {
            // If both fail, return empty array
            return Json(new List<object>());
        }
    }
    
    private string GetDefaultIconForMaterial(string materialName)
    {
        if (string.IsNullOrEmpty(materialName)) return "bi-circle";
        
        var nameLower = materialName.ToLower();
        if (nameLower.Contains("glass")) return "bi-circle";
        if (nameLower.Contains("plastic")) return "bi-circle-fill";
        if (nameLower.Contains("cardboard") || nameLower.Contains("paper")) return "bi-box";
        if (nameLower.Contains("foil") || nameLower.Contains("metal")) return "bi-square";
        if (nameLower.Contains("wrap")) return "bi-rectangle";
        if (nameLower.Contains("tape") || nameLower.Contains("sellotape")) return "bi-dash";
        
        return "bi-circle";
    }
    
    [HttpGet]
    [Route("api/visual-editor/material-taxonomy/{id}/children")]
    public async Task<IActionResult> GetTaxonomyChildren(int id, [FromQuery] int level)
    {
        var children = await _context.MaterialTaxonomies
            .Where(t => t.ParentTaxonomyId == id && t.Level == level && t.IsActive)
            .OrderBy(t => t.SortOrder)
            .ThenBy(t => t.DisplayName)
            .Select(t => new
            {
                id = t.Id,
                code = t.Code,
                name = t.DisplayName,
                description = t.Description,
                level = t.Level,
                hasChildren = t.ChildTaxonomies.Any()
            })
            .ToListAsync();
        
        return Json(children);
    }
    
    [HttpGet]
    [Route("api/visual-editor/material-taxonomy/{id}")]
    public async Task<IActionResult> GetMaterialTaxonomy(int id)
    {
        var taxonomy = await _context.MaterialTaxonomies
            .Where(t => t.Id == id && t.IsActive)
            .Select(t => new
            {
                id = t.Id,
                code = t.Code,
                displayName = t.DisplayName,
                name = t.Name,
                description = t.Description,
                iconClass = t.IconClass,
                level = t.Level
            })
            .FirstOrDefaultAsync();
        
        if (taxonomy == null)
        {
            return NotFound();
        }
        
        return Json(taxonomy);
    }
    
    [HttpGet]
    [Route("api/visual-editor/material-taxonomy/requirements")]
    public async Task<IActionResult> GetTaxonomyRequirements([FromQuery] int taxonomyId, [FromQuery] string[] countryCodes)
    {
        var requirements = await _context.MaterialTaxonomyCountryRequirements
            .Where(r => r.MaterialTaxonomyId == taxonomyId && 
                       r.IsActive && 
                       countryCodes.Contains(r.CountryCode))
            .Select(r => new
            {
                countryCode = r.CountryCode,
                countryName = r.CountryName,
                requiredLevel = r.RequiredLevel
            })
            .ToListAsync();
        
        // Get the maximum required level across all countries
        var maxRequiredLevel = requirements.Any() ? requirements.Max(r => r.requiredLevel) : 1;
        
        return Json(new
        {
            maxRequiredLevel = maxRequiredLevel,
            requirements = requirements
        });
    }

    [HttpGet]
    [Route("api/visual-editor/packaging-types")]
    public async Task<IActionResult> GetPackagingTypes([FromQuery] string? search = null)
    {
        var query = _context.PackagingTypes.AsQueryable();
        
        if (!string.IsNullOrEmpty(search))
        {
            query = query.Where(p => p.Name.Contains(search) || (p.Description != null && p.Description.Contains(search)));
        }

        var types = await query
            .OrderBy(p => p.Name)
            .Select(p => new
            {
                id = p.Id,
                name = p.Name,
                description = p.Description,
                height = p.Height,
                weight = p.Weight,
                depth = p.Depth,
                volume = p.Volume,
                imageUrl = p.ImageUrl,
                isFromLibrary = p.IsFromLibrary,
                librarySource = p.LibrarySource,
                materials = p.Materials.Select(m => new
                {
                    id = m.MaterialId,
                    name = m.Material.Name
                }).ToList()
            })
            .ToListAsync();
        return Json(types);
    }

    [HttpGet]
    [Route("api/visual-editor/packaging-type/{id}")]
    public async Task<IActionResult> GetPackagingType(int id)
    {
        var type = await _context.PackagingTypes
            .Include(p => p.Materials)
                .ThenInclude(m => m.Material)
            .FirstOrDefaultAsync(p => p.Id == id);
        
        if (type == null) return NotFound();

        return Json(new
        {
            id = type.Id,
            name = type.Name,
            description = type.Description,
            height = type.Height,
            weight = type.Weight,
            depth = type.Depth,
            volume = type.Volume,
            imageUrl = type.ImageUrl,
            materials = type.Materials.Select(m => new
            {
                id = m.MaterialId,
                name = m.Material.Name
            }).ToList()
        });
    }

    [HttpGet]
    [Route("api/visual-editor/products")]
    public async Task<IActionResult> GetProducts([FromQuery] string? search = null)
    {
        var query = _context.Products.AsQueryable();
        
        if (!string.IsNullOrEmpty(search))
        {
            query = query.Where(p => p.Name.Contains(search) || p.Sku.Contains(search));
        }

        var products = await query
            .Include(p => p.ProductPackagings)
                .ThenInclude(pp => pp.PackagingUnit)
                    .ThenInclude(pu => pu.Items)
                        .ThenInclude(pi => pi.PackagingType)
                            .ThenInclude(pt => pt.Materials)
                                .ThenInclude(ptm => ptm.Material)
            .OrderBy(p => p.Name)
            .Select(p => new
            {
                id = p.Id,
                sku = p.Sku,
                name = p.Name,
                description = p.Description,
                size = p.Size,
                weight = p.Weight,
                height = p.Height,
                quantity = p.Quantity,
                imageUrl = p.ImageUrl,
                packagingUnits = p.ProductPackagings.Select(pp => new
                {
                    id = pp.PackagingUnitId,
                    name = pp.PackagingUnit.Name,
                    items = pp.PackagingUnit.Items.Select(pi => new
                    {
                        id = pi.PackagingTypeId,
                        name = pi.PackagingType.Name,
                        materials = pi.PackagingType.Materials.Select(ptm => new
                        {
                            id = ptm.MaterialId,
                            name = ptm.Material.Name
                        }).ToList()
                    }).ToList()
                }).ToList()
            })
            .ToListAsync();
        return Json(products);
    }

    [HttpGet]
    [Route("api/visual-editor/product/{id}")]
    public async Task<IActionResult> GetProduct(int id)
    {
        var product = await _context.Products
            .Include(p => p.ProductPackagings)
                .ThenInclude(pp => pp.PackagingUnit)
                    .ThenInclude(pu => pu.Items)
                        .ThenInclude(pi => pi.PackagingType)
                            .ThenInclude(pt => pt.Materials)
                                .ThenInclude(ptm => ptm.Material)
            .FirstOrDefaultAsync(p => p.Id == id);
        
        if (product == null) return NotFound();

        return Json(new
        {
            id = product.Id,
            sku = product.Sku,
            name = product.Name,
            description = product.Description,
            size = product.Size,
            weight = product.Weight,
            height = product.Height,
            quantity = product.Quantity,
            imageUrl = product.ImageUrl,
            packagingUnits = product.ProductPackagings.Select(pp => new
            {
                id = pp.PackagingUnitId,
                name = pp.PackagingUnit.Name,
                items = pp.PackagingUnit.Items.Select(pi => new
                {
                    id = pi.PackagingTypeId,
                    name = pi.PackagingType.Name,
                    materials = pi.PackagingType.Materials.Select(ptm => new
                    {
                        id = ptm.MaterialId,
                        name = ptm.Material.Name
                    }).ToList()
                }).ToList()
            }).ToList()
        });
    }

    [HttpGet]
    [Route("api/visual-editor/geographies")]
    public async Task<IActionResult> GetGeographies([FromQuery] int? parentId = null)
    {
        var query = _context.Geographies.AsQueryable();
        
        if (parentId.HasValue)
        {
            query = query.Where(g => g.ParentGeographyId == parentId);
        }
        else
        {
            query = query.Where(g => g.ParentGeographyId == null);
        }

        var geographies = await query
            .OrderBy(g => g.Name)
            .Select(g => new
            {
                id = g.Id,
                name = g.Name,
                code = g.Code,
                parentId = g.ParentGeographyId,
                hasChildren = g.ChildGeographies.Any()
            })
            .ToListAsync();
        return Json(geographies);
    }

    [HttpGet]
    [Route("api/visual-editor/jurisdictions")]
    public async Task<IActionResult> GetJurisdictions()
    {
        var jurisdictions = await _context.Jurisdictions
            .OrderBy(j => j.Name)
            .Select(j => new
            {
                id = j.Id,
                name = j.Name,
                code = j.Code
            })
            .ToListAsync();
        return Json(jurisdictions);
    }

    [HttpPost]
    [Route("api/visual-editor/project")]
    public async Task<IActionResult> SaveProject([FromBody] ProjectData projectData)
    {
        try
        {
            var projectJson = System.Text.Json.JsonSerializer.Serialize(projectData);
            var projectKey = projectData.ProjectName?.ToLower().Replace(" ", "-") ?? $"project-{DateTime.UtcNow.Ticks}";
            
            var existingProject = await _context.VisualEditorProjects
                .FirstOrDefaultAsync(p => p.Key == projectKey);
            
            if (existingProject != null)
            {
                existingProject.ProjectDataJson = projectJson;
                existingProject.UpdatedAt = DateTime.UtcNow;
            }
            else
            {
                var newProject = new VisualEditorProject
                {
                    Key = projectKey,
                    Name = projectData.ProjectName ?? "Untitled Project",
                    ProjectDataJson = projectJson,
                    CreatedAt = DateTime.UtcNow
                };
                _context.VisualEditorProjects.Add(newProject);
            }
            
            await _context.SaveChangesAsync();
            return Json(new { success = true, message = "Project saved successfully", key = projectKey });
        }
        catch (Exception ex)
        {
            return Json(new { success = false, message = ex.Message });
        }
    }

    [HttpGet]
    [Route("api/visual-editor/project/{id}")]
    public async Task<IActionResult> GetProject(int id)
    {
        try
        {
            var project = await _context.VisualEditorProjects.FindAsync(id);
            if (project == null)
            {
                return Json(new { success = false, message = "Project not found" });
            }
            
            // Return project with both metadata and parsed JSON data
            var projectData = System.Text.Json.JsonSerializer.Deserialize<System.Text.Json.JsonElement>(project.ProjectDataJson);
            
            // Extract nodes, connections, and groups as proper objects
            var nodes = projectData.TryGetProperty("nodes", out var nodesElement) ? 
                System.Text.Json.JsonSerializer.Deserialize<object>(nodesElement.GetRawText()) : null;
            var connections = projectData.TryGetProperty("connections", out var connectionsElement) ? 
                System.Text.Json.JsonSerializer.Deserialize<object>(connectionsElement.GetRawText()) : null;
            var groups = projectData.TryGetProperty("groups", out var groupsElement) ? 
                System.Text.Json.JsonSerializer.Deserialize<object>(groupsElement.GetRawText()) : null;
            
            return Json(new { 
                success = true, 
                data = new {
                    id = project.Id,
                    key = project.Key,
                    name = project.Name,
                    createdAt = project.CreatedAt,
                    updatedAt = project.UpdatedAt,
                    projectDataJson = project.ProjectDataJson,
                    // Include parsed data as proper objects
                    nodes = nodes,
                    connections = connections,
                    groups = groups
                }
            });
        }
        catch (Exception ex)
        {
            return Json(new { success = false, message = ex.Message });
        }
    }

    [HttpGet]
    [Route("api/visual-editor/project/key/{key}")]
    public async Task<IActionResult> GetProjectByKey(string key)
    {
        try
        {
            var project = await _context.VisualEditorProjects
                .FirstOrDefaultAsync(p => p.Key == key);
            
            if (project == null)
            {
                return Json(new { success = false, message = "Project not found" });
            }
            
            // Parse JSON and return as dynamic object
            var projectData = System.Text.Json.JsonSerializer.Deserialize<System.Text.Json.JsonElement>(project.ProjectDataJson);
            return Json(new { success = true, data = projectData });
        }
        catch (Exception ex)
        {
            return Json(new { success = false, message = ex.Message });
        }
    }

    [HttpGet]
    [Route("api/visual-editor/projects")]
    public async Task<IActionResult> GetAllProjects()
    {
        try
        {
            var projects = await _context.VisualEditorProjects
                .OrderByDescending(p => p.CreatedAt)
                .Select(p => new
                {
                    id = p.Id,
                    key = p.Key,
                    name = p.Name,
                    createdAt = p.CreatedAt,
                    updatedAt = p.UpdatedAt
                })
                .ToListAsync();
            
            return Json(new { success = true, data = projects });
        }
        catch (Exception ex)
        {
            return Json(new { success = false, message = ex.Message });
        }
    }

    [HttpGet]
    [Route("api/visual-editor/distribution-groups")]
    public async Task<IActionResult> GetDistributionGroups()
    {
        try
        {
            // Get all VisualEditorProjects - these can be distribution groups
            // Filter by key pattern or check if project data contains distribution nodes
            var projects = await _context.VisualEditorProjects
                .OrderByDescending(p => p.CreatedAt)
                .Select(p => new
                {
                    id = p.Id,
                    key = p.Key,
                    name = p.Name,
                    createdAt = p.CreatedAt,
                    updatedAt = p.UpdatedAt
                })
                .ToListAsync();
            
            return Json(new { success = true, data = projects });
        }
        catch (Exception ex)
        {
            return Json(new { success = false, message = ex.Message });
        }
    }

    [HttpGet]
    [Route("api/visual-editor/packaging-groups")]
    public async Task<IActionResult> GetPackagingGroups()
    {
        var groups = await _context.PackagingGroups
            .Where(g => g.IsActive)
            .Include(g => g.Items)
                .ThenInclude(i => i.PackagingLibrary)
                    .ThenInclude(l => l!.MaterialTaxonomy)
            .Include(g => g.Items)
                .ThenInclude(i => i.PackagingLibrary)
                    .ThenInclude(l => l!.PackagingLibraryMaterials)
                        .ThenInclude(plm => plm.MaterialTaxonomy)
            .OrderBy(g => g.Name)
            .ToListAsync();

        var groupsDto = groups.Select(g => new
        {
            id = g.Id,
            packId = g.PackId,
            name = g.Name,
            packagingLayer = g.PackagingLayer,
            style = g.Style,
            shape = g.Shape,
            size = g.Size,
            volumeDimensions = g.VolumeDimensions,
            coloursAvailable = g.ColoursAvailable,
            recycledContent = g.RecycledContent,
            totalPackWeight = g.TotalPackWeight,
            weightBasis = g.WeightBasis,
            notes = g.Notes,
            exampleReference = g.ExampleReference,
            source = g.Source,
            url = g.Url,
            items = g.Items.OrderBy(i => i.SortOrder).Select(i =>
            {
                var lib = i.PackagingLibrary;
                var rawMatIds = lib.PackagingLibraryMaterials.OrderBy(plm => plm.SortOrder).Select(plm => plm.MaterialTaxonomyId).ToList();
                return new
                {
                    id = lib.Id,
                    taxonomyCode = lib.TaxonomyCode,
                    name = lib.Name,
                    weight = lib.Weight,
                    materialTaxonomyId = lib.MaterialTaxonomyId,
                    materialTaxonomyCode = lib.MaterialTaxonomy != null ? lib.MaterialTaxonomy.Code : null,
                    rawMaterialIds = rawMatIds
                };
            })
        }).ToList();

        return Json(groupsDto);
    }

    [HttpGet]
    [Route("api/visual-editor/packaging-library")]
    public async Task<IActionResult> GetPackagingLibrary()
    {
        var libraryItems = await _context.PackagingLibraries
            .Where(l => l.IsActive)
            .Include(l => l.MaterialTaxonomy)
            .Include(l => l.PackagingLibraryMaterials)
                .ThenInclude(plm => plm.MaterialTaxonomy)
            .OrderBy(l => l.Name)
            .ToListAsync();

        var libraryItemsDto = libraryItems.Select(l => new
        {
            id = l.Id,
            taxonomyCode = l.TaxonomyCode,
            name = l.Name,
            weight = l.Weight,
            materialTaxonomyId = l.MaterialTaxonomyId,
            materialTaxonomyCode = l.MaterialTaxonomy != null ? l.MaterialTaxonomy.Code : null,
            rawMaterialIds = l.PackagingLibraryMaterials.OrderBy(plm => plm.SortOrder).Select(plm => plm.MaterialTaxonomyId).ToList()
        }).ToList();

        var supplierProducts = await _context.PackagingSupplierProducts
            .Include(p => p.PackagingSupplier)
            .Where(p => p.PackagingSupplier.IsActive)
            .OrderBy(p => p.PackagingSupplier.Name)
            .ThenBy(p => p.Name)
            .Select(p => new
            {
                id = 500000 + p.Id,
                taxonomyCode = p.TaxonomyCode,
                name = p.Name + " (" + p.PackagingSupplier.Name + ")",
                weight = (decimal?)null,
                materialTaxonomyId = (int?)null,
                materialTaxonomyCode = (string?)null
            })
            .ToListAsync();

        var combined = libraryItemsDto.Cast<object>().Concat(supplierProducts.Cast<object>()).ToList();
        return Json(combined);
    }

    [HttpGet]
    [Route("api/visual-editor/packaging-group/{id}")]
    public async Task<IActionResult> GetPackagingGroup(int id)
    {
        var group = await _context.PackagingGroups
            .Where(g => g.Id == id && g.IsActive)
            .Include(g => g.Items)
                .ThenInclude(i => i.PackagingLibrary)
                    .ThenInclude(l => l!.MaterialTaxonomy)
            .Include(g => g.Items)
                .ThenInclude(i => i.PackagingLibrary)
                    .ThenInclude(l => l!.PackagingLibraryMaterials)
                        .ThenInclude(plm => plm.MaterialTaxonomy)
            .FirstOrDefaultAsync();

        if (group == null)
        {
            return NotFound();
        }

        return Json(new
        {
            id = group.Id,
            packId = group.PackId,
            name = group.Name,
            packagingLayer = group.PackagingLayer,
            style = group.Style,
            shape = group.Shape,
            size = group.Size,
            volumeDimensions = group.VolumeDimensions,
            coloursAvailable = group.ColoursAvailable,
            recycledContent = group.RecycledContent,
            totalPackWeight = group.TotalPackWeight,
            weightBasis = group.WeightBasis,
            notes = group.Notes,
            exampleReference = group.ExampleReference,
            source = group.Source,
            url = group.Url,
            items = group.Items.OrderBy(i => i.SortOrder).Select(i =>
            {
                var lib = i.PackagingLibrary;
                var rawMatIds = lib.PackagingLibraryMaterials.OrderBy(plm => plm.SortOrder).Select(plm => plm.MaterialTaxonomyId).ToList();
                return new
                {
                    id = lib.Id,
                    taxonomyCode = lib.TaxonomyCode,
                    name = lib.Name,
                    weight = lib.Weight,
                    materialTaxonomyId = lib.MaterialTaxonomyId,
                    materialTaxonomyCode = lib.MaterialTaxonomy != null ? lib.MaterialTaxonomy.Code : null,
                    rawMaterialIds = rawMatIds
                };
            })
        });
    }

    [HttpGet]
    [Route("api/visual-editor/supplier-packaging")]
    public async Task<IActionResult> GetSupplierPackaging([FromQuery] string? search = null)
    {
        var query = _context.PackagingSupplierProducts
            .Include(p => p.PackagingSupplier)
            .Where(p => p.PackagingSupplier.IsActive)
            .AsQueryable();

        if (!string.IsNullOrEmpty(search))
        {
            query = query.Where(p =>
                p.Name.Contains(search) ||
                (p.Description != null && p.Description.Contains(search)) ||
                (p.ProductCode != null && p.ProductCode.Contains(search)) ||
                p.PackagingSupplier.Name.Contains(search));
        }

        var items = await query
            .OrderBy(p => p.PackagingSupplier.Name)
            .ThenBy(p => p.Name)
            .Select(p => new
            {
                id = p.Id,
                name = p.Name,
                description = p.Description,
                productCode = p.ProductCode,
                taxonomyCode = p.TaxonomyCode,
                supplierId = p.PackagingSupplierId,
                supplierName = p.PackagingSupplier.Name
            })
            .ToListAsync();

        return Json(items);
    }

    [HttpPost]
    [Route("api/visual-editor/product/{productId}/supplier-packaging/{packagingSupplierProductId}")]
    public async Task<IActionResult> AttachSupplierPackagingToProduct(int productId, int packagingSupplierProductId)
    {
        var product = await _context.Products.FindAsync(productId);
        if (product == null) return NotFound();

        var supplierProduct = await _context.PackagingSupplierProducts
            .Include(p => p.PackagingSupplier)
            .FirstOrDefaultAsync(p => p.Id == packagingSupplierProductId && p.PackagingSupplier.IsActive);
        if (supplierProduct == null) return NotFound();

        var exists = await _context.ProductPackagingSupplierProducts
            .AnyAsync(pp => pp.ProductId == productId && pp.PackagingSupplierProductId == packagingSupplierProductId);
        if (exists) return Json(new { success = true, message = "Already attached" });

        _context.ProductPackagingSupplierProducts.Add(new ProductPackagingSupplierProduct
        {
            ProductId = productId,
            PackagingSupplierProductId = packagingSupplierProductId
        });
        await _context.SaveChangesAsync();
        return Json(new { success = true });
    }

    [HttpDelete]
    [Route("api/visual-editor/product/{productId}/supplier-packaging/{packagingSupplierProductId}")]
    public async Task<IActionResult> DetachSupplierPackagingFromProduct(int productId, int packagingSupplierProductId)
    {
        var link = await _context.ProductPackagingSupplierProducts
            .FirstOrDefaultAsync(pp => pp.ProductId == productId && pp.PackagingSupplierProductId == packagingSupplierProductId);
        if (link == null) return NotFound();

        _context.ProductPackagingSupplierProducts.Remove(link);
        await _context.SaveChangesAsync();
        return Json(new { success = true });
    }

    [HttpGet]
    [Route("api/visual-editor/product/{productId}/supplier-packaging")]
    public async Task<IActionResult> GetProductSupplierPackaging(int productId)
    {
        var links = await _context.ProductPackagingSupplierProducts
            .Where(pp => pp.ProductId == productId)
            .Include(pp => pp.PackagingSupplierProduct)
                .ThenInclude(p => p.PackagingSupplier)
            .Select(pp => new
            {
                id = pp.PackagingSupplierProduct.Id,
                name = pp.PackagingSupplierProduct.Name,
                supplierName = pp.PackagingSupplierProduct.PackagingSupplier.Name
            })
            .ToListAsync();

        return Json(links);
    }

    [HttpPost]
    [Route("api/visual-editor/import-ms-network")]
    public async Task<IActionResult> ImportMSNetwork([FromForm] IFormFile file)
    {
        try
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest(new { success = false, message = "No file uploaded" });
            }

            // Save uploaded file temporarily
            var tempPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString() + ".xlsx");
            using (var stream = new FileStream(tempPath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            try
            {
                // Run the import script
                var json = await EPR.Web.Scripts.ImportMSNetworkDistributionGroup.RunAsync(tempPath);
                
                // Return JSON directly - let the client parse it
                return Content(json, "application/json");
            }
            finally
            {
                // Clean up temp file
                if (System.IO.File.Exists(tempPath))
                {
                    System.IO.File.Delete(tempPath);
                }
            }
        }
        catch (Exception ex)
        {
            return Json(new { success = false, message = ex.Message, stackTrace = ex.StackTrace });
        }
    }
}

public class ProjectData
{
    public string? ProjectName { get; set; }
    public List<NodeData> Nodes { get; set; } = new();
    public List<ConnectionData> Connections { get; set; } = new();
}

public class NodeData
{
    public string Id { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty; // raw-material, packaging, product, distribution
    public int? EntityId { get; set; }
    public double X { get; set; }
    public double Y { get; set; }
    public Dictionary<string, object>? Parameters { get; set; }
}

public class ConnectionData
{
    public string From { get; set; } = string.Empty;
    public string To { get; set; } = string.Empty;
    public Dictionary<string, object>? Properties { get; set; }
}

