using Microsoft.AspNetCore.Mvc;
using EPR.Web.Attributes;
using EPR.Domain.Entities;
using EPR.Data;
using Microsoft.EntityFrameworkCore;

namespace EPR.Web.Controllers;

[Authorize]
public class PackagingManagementController : Controller
{
    private readonly EPRDbContext _context;

    public PackagingManagementController(EPRDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public IActionResult Index(string type = "suppliers", int page = 1, int pageSize = 20, string sortBy = "name", string sortDir = "asc", string filter = "")
    {
        ViewData["Type"] = type;
        ViewData["Page"] = page;
        ViewData["PageSize"] = pageSize;
        ViewData["SortBy"] = sortBy;
        ViewData["SortDir"] = sortDir;
        ViewData["Filter"] = filter;
        return View();
    }

    [HttpGet]
    [Route("api/packaging-management/list/{type}")]
    public async Task<IActionResult> GetList(string type, int page = 1, int pageSize = 20, string sortBy = "name", string sortDir = "asc", string filter = "")
    {
        try
        {
            object result = type.ToLower() switch
            {
                "raw-materials" => await GetRawMaterialsList(page, pageSize, sortBy, sortDir, filter),
                "packaging-items" => await GetPackagingItemsList(page, pageSize, sortBy, sortDir, filter),
                "packaging-groups" => await GetPackagingGroupsList(page, pageSize, sortBy, sortDir, filter),
                "suppliers" => await GetSuppliersList(page, pageSize, sortBy, sortDir, filter),
                _ => new { error = "Invalid type" }
            };

            return Json(result);
        }
        catch (Exception ex)
        {
            return Json(new { error = ex.Message });
        }
    }

    [HttpGet]
    [Route("api/packaging-management/{type}/{id}")]
    public async Task<IActionResult> GetDetail(string type, int id)
    {
        try
        {
            object? result = type.ToLower() switch
            {
                "raw-materials" => await GetRawMaterialDetail(id),
                "packaging-items" => await GetPackagingItemDetail(id),
                "packaging-groups" => await GetPackagingGroupDetail(id),
                "suppliers" => await GetSupplierDetail(id),
                _ => null
            };

            if (result == null)
            {
                return NotFound();
            }

            return Json(result);
        }
        catch (Exception ex)
        {
            return Json(new { error = ex.Message });
        }
    }

    private async Task<object> GetRawMaterialsList(int page, int pageSize, string sortBy, string sortDir, string filter)
    {
        var query = _context.MaterialTaxonomies
            .Where(t => t.Level == 1 && t.IsActive)
            .AsQueryable();

        // Apply filter
        if (!string.IsNullOrEmpty(filter))
        {
            query = query.Where(t => 
                t.DisplayName.Contains(filter) || 
                t.Code.Contains(filter) ||
                (t.Description != null && t.Description.Contains(filter)));
        }

        // Apply sorting
        query = sortBy.ToLower() switch
        {
            "code" => sortDir == "asc" ? query.OrderBy(t => t.Code) : query.OrderByDescending(t => t.Code),
            "description" => sortDir == "asc" ? query.OrderBy(t => t.Description ?? "") : query.OrderByDescending(t => t.Description ?? ""),
            _ => sortDir == "asc" ? query.OrderBy(t => t.DisplayName) : query.OrderByDescending(t => t.DisplayName)
        };

        // Count total parents (for pagination)
        var parentCount = await query.CountAsync();
        
        // Get items with pagination and include all nested children (up to 5 levels deep)
        var itemsQuery = query
            .Include(t => t.ChildTaxonomies.Where(c => c.IsActive))
                .ThenInclude(c => c.ChildTaxonomies.Where(c2 => c2.IsActive))
                .ThenInclude(c2 => c2.ChildTaxonomies.Where(c3 => c3.IsActive))
                .ThenInclude(c3 => c3.ChildTaxonomies.Where(c4 => c4.IsActive))
                .ThenInclude(c4 => c4.ChildTaxonomies.Where(c5 => c5.IsActive))
            .Skip((page - 1) * pageSize)
            .Take(pageSize);
            
        // Load items
        var itemsList = await itemsQuery.ToListAsync();
        
        // Recursive function to collect all material IDs (including nested children)
        HashSet<int> CollectAllMaterialIds(MaterialTaxonomy taxonomy)
        {
            var ids = new HashSet<int> { taxonomy.Id };
            foreach (var child in taxonomy.ChildTaxonomies.Where(c => c.IsActive))
            {
                ids.UnionWith(CollectAllMaterialIds(child));
            }
            return ids;
        }
        
        // Get all material IDs including nested children
        var allMaterialIds = itemsList.SelectMany(t => CollectAllMaterialIds(t)).ToList();
        
        // Get packaging items that use each raw material (via PackagingLibraryMaterials OR legacy MaterialTaxonomyId)
        var packagingItemsByMaterialFromJoin = await _context.PackagingLibraryMaterials
            .Where(plm => allMaterialIds.Contains(plm.MaterialTaxonomyId))
            .Join(_context.PackagingLibraries.Where(l => l.IsActive), plm => plm.PackagingLibraryId, l => l.Id, (plm, l) => new { materialTaxonomyId = plm.MaterialTaxonomyId, id = l.Id, name = l.Name, taxonomyCode = l.TaxonomyCode })
            .ToListAsync();
        var packagingItemsByMaterialFromLegacy = await _context.PackagingLibraries
            .Where(l => l.IsActive && l.MaterialTaxonomyId != null && allMaterialIds.Contains(l.MaterialTaxonomyId!.Value))
            .Select(l => new { materialTaxonomyId = l.MaterialTaxonomyId!.Value, id = l.Id, name = l.Name, taxonomyCode = l.TaxonomyCode })
            .ToListAsync();
        var packagingItemsByMaterial = packagingItemsByMaterialFromJoin
            .Union(packagingItemsByMaterialFromLegacy)
            .GroupBy(x => (x.materialTaxonomyId, x.id))
            .Select(g => g.First())
            .ToList();
        
        // Flatten structure: parent + children as separate rows
        var items = new List<object>();
        
        // Recursive function to add children at all levels
        void AddChildrenRecursive(MaterialTaxonomy parent, int depth, int? parentId, string parentName)
        {
            foreach (var child in parent.ChildTaxonomies.Where(c => c.IsActive).OrderBy(c => c.DisplayName))
            {
                items.Add(new
                {
                    id = child.Id,
                    name = child.DisplayName,
                    code = child.Code,
                    description = child.Description,
                    iconClass = child.IconClass,
                    level = child.Level,
                    parentId = parentId,
                    parentName = parentName,
                    isChild = true,
                    depth = depth,
                    packagingItems = packagingItemsByMaterial
                        .Where(p => p.materialTaxonomyId == child.Id)
                        .Select(p => new
                        {
                            id = p.id,
                            name = p.name,
                            taxonomyCode = p.taxonomyCode
                        }).ToList()
                });
                
                // Recursively add children of this child
                AddChildrenRecursive(child, depth + 1, child.Id, child.DisplayName);
            }
        }
        foreach (var parent in itemsList)
        {
            // Add parent row
            items.Add(new
            {
                id = parent.Id,
                name = parent.DisplayName,
                code = parent.Code,
                description = parent.Description,
                iconClass = parent.IconClass,
                level = parent.Level,
                parentId = (int?)null,
                parentName = (string?)null,
                isChild = false,
                depth = 0,
                packagingItems = packagingItemsByMaterial
                    .Where(p => p.materialTaxonomyId == parent.Id)
                    .Select(p => new
                    {
                        id = p.id,
                        name = p.name,
                        taxonomyCode = p.taxonomyCode
                    }).ToList()
            });
            
            // Add all children recursively
            AddChildrenRecursive(parent, 1, parent.Id, parent.DisplayName);
        }
        
        // Calculate total count including all nested children (apply same filter)
        var filterQuery = _context.MaterialTaxonomies
            .Where(t => t.Level == 1 && t.IsActive);
            
        if (!string.IsNullOrEmpty(filter))
        {
            filterQuery = filterQuery.Where(t => 
                t.DisplayName.Contains(filter) || 
                t.Code.Contains(filter) ||
                (t.Description != null && t.Description.Contains(filter)));
        }
        
        // Recursive function to count all nested children
        int CountAllChildren(MaterialTaxonomy taxonomy)
        {
            int count = 1; // Count self
            foreach (var child in taxonomy.ChildTaxonomies.Where(c => c.IsActive))
            {
                count += CountAllChildren(child);
            }
            return count;
        }
        
        var filteredParents = await filterQuery
            .Include(t => t.ChildTaxonomies.Where(c => c.IsActive))
                .ThenInclude(c => c.ChildTaxonomies.Where(c2 => c2.IsActive))
                .ThenInclude(c2 => c2.ChildTaxonomies.Where(c3 => c3.IsActive))
                .ThenInclude(c3 => c3.ChildTaxonomies.Where(c4 => c4.IsActive))
            .ToListAsync();
        
        var totalCount = filteredParents.Sum(t => CountAllChildren(t));

        return new
        {
            items,
            totalCount,
            page,
            pageSize,
            totalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
        };
    }

    private async Task<object> GetPackagingItemsList(int page, int pageSize, string sortBy, string sortDir, string filter)
    {
        var query = _context.PackagingLibraries
            .Where(l => l.IsActive)
            .AsQueryable();

        // Apply filter
        if (!string.IsNullOrEmpty(filter))
        {
            query = query.Where(l => 
                l.Name.Contains(filter) || 
                l.TaxonomyCode.Contains(filter));
        }

        // Apply sorting - handle material sorting separately
        if (sortBy.ToLower() == "material")
        {
            // For material sorting, we need to join
            query = sortDir == "asc" 
                ? query.OrderBy(l => l.MaterialTaxonomy != null ? l.MaterialTaxonomy.DisplayName : "")
                : query.OrderByDescending(l => l.MaterialTaxonomy != null ? l.MaterialTaxonomy.DisplayName : "");
        }
        else
        {
            query = sortBy.ToLower() switch
            {
                "taxonomycode" => sortDir == "asc" ? query.OrderBy(l => l.TaxonomyCode) : query.OrderByDescending(l => l.TaxonomyCode),
                "weight" => sortDir == "asc" ? query.OrderBy(l => l.Weight ?? 0) : query.OrderByDescending(l => l.Weight ?? 0),
                _ => sortDir == "asc" ? query.OrderBy(l => l.Name) : query.OrderByDescending(l => l.Name)
            };
        }

        // Count total packaging items (for pagination)
        var parentCount = await query.CountAsync();
        
        // Get items with pagination
        var itemsQuery = query
            .Skip((page - 1) * pageSize)
            .Take(pageSize);
            
        // Load items efficiently with supply chain data
        var itemsList = await itemsQuery
            .Include(l => l.MaterialTaxonomy)
                .ThenInclude(mt => mt.ChildTaxonomies.Where(c => c.IsActive))
            .Include(l => l.PackagingLibraryMaterials)
                .ThenInclude(plm => plm.MaterialTaxonomy)
            .Include(l => l.PackagingLibrarySupplierProducts)
                .ThenInclude(plsp => plsp.PackagingSupplierProduct)
                    .ThenInclude(psp => psp.PackagingSupplier)
                        .ThenInclude(ps => ps!.SuppliedBySupplier)
            .Include(l => l.PackagingGroupItems)
                .ThenInclude(gi => gi.PackagingGroup)
            .ToListAsync();
            
        // Flatten structure: packaging item + raw material children (from PackagingLibraryMaterials or legacy MaterialTaxonomy)
        var items = new List<object>();
        foreach (var packagingItem in itemsList)
        {
            var rawMaterials = packagingItem.PackagingLibraryMaterials
                .OrderBy(plm => plm.SortOrder)
                .Select(plm => plm.MaterialTaxonomy)
                .Where(mt => mt != null && mt.IsActive)
                .Cast<MaterialTaxonomy>()
                .ToList();
            var legacyMaterial = packagingItem.MaterialTaxonomy;
            if (rawMaterials.Count == 0 && legacyMaterial != null)
                rawMaterials = new List<MaterialTaxonomy?> { legacyMaterial };

            var materialNames = rawMaterials.Where(m => m != null).Select(m => m!.DisplayName).Distinct().ToList();
            var materialTaxonomyName = materialNames.Count > 0 ? string.Join(", ", materialNames) : null;
            var primaryMaterialId = rawMaterials.FirstOrDefault()?.Id;
            var primaryMaterialCode = rawMaterials.FirstOrDefault()?.Code;

            // Add packaging item row (parent)
            items.Add(new
            {
                id = packagingItem.Id,
                name = packagingItem.Name,
                taxonomyCode = packagingItem.TaxonomyCode,
                weight = packagingItem.Weight,
                materialTaxonomyName = materialTaxonomyName,
                materialTaxonomyId = primaryMaterialId,
                materialTaxonomyCode = primaryMaterialCode,
                rawMaterials = rawMaterials.Where(m => m != null).Select(m => new { id = m!.Id, name = m.DisplayName, code = m.Code }).ToList(),
                supplyChain = packagingItem.PackagingLibrarySupplierProducts.Select(plsp => new
                {
                    productName = plsp.PackagingSupplierProduct.Name,
                    supplierName = plsp.PackagingSupplierProduct.PackagingSupplier.Name,
                    suppliedBy = plsp.PackagingSupplierProduct.PackagingSupplier.SuppliedBySupplier != null ? plsp.PackagingSupplierProduct.PackagingSupplier.SuppliedBySupplier.Name : null
                }).ToList(),
                isChild = false,
                parentId = (int?)null,
                parentName = (string?)null,
                groups = packagingItem.PackagingGroupItems.Select(gi => new
                {
                    id = gi.PackagingGroup.Id,
                    name = gi.PackagingGroup.Name,
                    packId = gi.PackagingGroup.PackId
                }).ToList()
            });
            
            // Add raw material rows (children) - one per raw material
            foreach (var material in rawMaterials.Where(m => m != null))
            {
                items.Add(new
                {
                    id = material!.Id,
                    name = material.DisplayName,
                    taxonomyCode = material.Code,
                    weight = (decimal?)null,
                    materialTaxonomyName = material.DisplayName,
                    materialTaxonomyId = material.Id,
                    materialTaxonomyCode = material.Code,
                    rawMaterials = new object[0],
                    supplyChain = new object[0],
                    isChild = true,
                    parentId = packagingItem.Id,
                    parentName = packagingItem.Name,
                    groups = new object[0]
                });
            }
        }
        
        // Calculate total count including children (apply same filter)
        var filterQuery = _context.PackagingLibraries
            .Where(l => l.IsActive);
            
        if (!string.IsNullOrEmpty(filter))
        {
            filterQuery = filterQuery.Where(l => 
                l.Name.Contains(filter) || 
                l.TaxonomyCode.Contains(filter));
        }
        
        var totalPackagingItemsWithChildren = await filterQuery
            .Include(l => l.PackagingLibraryMaterials)
            .Include(l => l.MaterialTaxonomy)
                .ThenInclude(mt => mt.ChildTaxonomies.Where(c => c.IsActive))
            .ToListAsync();
        var totalCount = totalPackagingItemsWithChildren.Sum(l =>
        {
            var rmCount = l.PackagingLibraryMaterials.Count;
            if (rmCount == 0 && l.MaterialTaxonomy != null)
                rmCount = 1 + l.MaterialTaxonomy.ChildTaxonomies.Count(c => c.IsActive);
            return 1 + rmCount;
        });

        return new
        {
            items,
            totalCount,
            page,
            pageSize,
            totalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
        };
    }

    private async Task<object> GetPackagingGroupsList(int page, int pageSize, string sortBy, string sortDir, string filter)
    {
        var query = _context.PackagingGroups
            .Where(g => g.IsActive)
            .AsQueryable();

        // Apply filter
        if (!string.IsNullOrEmpty(filter))
        {
            query = query.Where(g => 
                g.Name.Contains(filter) || 
                g.PackId.Contains(filter) ||
                (g.PackagingLayer != null && g.PackagingLayer.Contains(filter)));
        }

        // Apply sorting
        query = sortBy.ToLower() switch
        {
            "packid" => sortDir == "asc" ? query.OrderBy(g => g.PackId) : query.OrderByDescending(g => g.PackId),
            "packaginglayer" => sortDir == "asc" ? query.OrderBy(g => g.PackagingLayer ?? "") : query.OrderByDescending(g => g.PackagingLayer ?? ""),
            "totalweight" => sortDir == "asc" ? query.OrderBy(g => g.TotalPackWeight ?? 0) : query.OrderByDescending(g => g.TotalPackWeight ?? 0),
            _ => sortDir == "asc" ? query.OrderBy(g => g.Name) : query.OrderByDescending(g => g.Name)
        };

        var totalCount = await query.CountAsync();
        
        // Get groups with pagination
        var itemsQuery = query
            .Skip((page - 1) * pageSize)
            .Take(pageSize);
            
        // Load groups with items and supply chain
        var groupsList = await itemsQuery
            .Include(g => g.Items)
                .ThenInclude(i => i.PackagingLibrary)
                    .ThenInclude(l => l!.PackagingLibraryMaterials)
                        .ThenInclude(plm => plm.MaterialTaxonomy)
            .Include(g => g.Items)
                .ThenInclude(i => i.PackagingLibrary)
                    .ThenInclude(l => l!.PackagingLibrarySupplierProducts)
                        .ThenInclude(plsp => plsp.PackagingSupplierProduct)
                            .ThenInclude(psp => psp.PackagingSupplier)
                                .ThenInclude(ps => ps!.SuppliedBySupplier)
            .Include(g => g.Items)
                .ThenInclude(i => i.PackagingLibrary)
                    .ThenInclude(l => l!.MaterialTaxonomy)
            .ToListAsync();
            
        var items = groupsList.Select(g => new
        {
            id = g.Id,
            name = g.Name,
            packId = g.PackId,
            packagingLayer = g.PackagingLayer,
            style = g.Style,
            shape = g.Shape,
            totalPackWeight = g.TotalPackWeight,
            itemCount = g.Items.Count,
            items = g.Items.OrderBy(i => i.SortOrder).Select(i =>
            {
                var lib = i.PackagingLibrary;
                var rawMats = lib.PackagingLibraryMaterials
                    .OrderBy(plm => plm.SortOrder)
                    .Select(plm => plm.MaterialTaxonomy?.DisplayName)
                    .Where(n => n != null)
                    .ToList();
                if (rawMats.Count == 0 && lib.MaterialTaxonomy != null)
                    rawMats = new List<string?> { lib.MaterialTaxonomy.DisplayName };
                var supplyChain = lib.PackagingLibrarySupplierProducts.Select(plsp => new
                {
                    supplier = plsp.PackagingSupplierProduct.PackagingSupplier.Name,
                    suppliedBy = plsp.PackagingSupplierProduct.PackagingSupplier.SuppliedBySupplier?.Name
                }).ToList();
                return new
                {
                    id = lib.Id,
                    name = lib.Name,
                    taxonomyCode = lib.TaxonomyCode,
                    weight = lib.Weight,
                    rawMaterials = rawMats,
                    supplyChain = supplyChain
                };
            }).ToList()
        }).ToList();

        return new
        {
            items,
            totalCount,
            page,
            pageSize,
            totalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
        };
    }

    private async Task<object?> GetRawMaterialDetail(int id)
    {
        var material = await _context.MaterialTaxonomies
            .Where(t => t.Id == id && t.IsActive)
            .Include(t => t.ChildTaxonomies.Where(c => c.IsActive))
            .Include(t => t.ParentTaxonomy)
            .FirstOrDefaultAsync();

        if (material == null) return null;

        // Get packaging items that use this material (via PackagingLibraryMaterials OR legacy MaterialTaxonomyId)
        var packagingItemsFromJoin = await _context.PackagingLibraryMaterials
            .Where(plm => plm.MaterialTaxonomyId == id)
            .Join(_context.PackagingLibraries.Where(l => l.IsActive), plm => plm.PackagingLibraryId, l => l.Id, (plm, l) => new { id = l.Id, name = l.Name, taxonomyCode = l.TaxonomyCode, weight = l.Weight })
            .ToListAsync();
        var packagingItemsFromLegacy = await _context.PackagingLibraries
            .Where(l => l.MaterialTaxonomyId == id && l.IsActive)
            .Select(l => new { id = l.Id, name = l.Name, taxonomyCode = l.TaxonomyCode, weight = l.Weight })
            .ToListAsync();
        var packagingItems = packagingItemsFromJoin.Union(packagingItemsFromLegacy).GroupBy(x => x.id).Select(g => g.First()).ToList();

        // Supply chain: suppliers of this raw material (with their upstream suppliers)
        var supplyChain = await _context.MaterialTaxonomySupplierProducts
            .Where(msp => msp.MaterialTaxonomyId == id)
            .Include(msp => msp.PackagingSupplierProduct)
                .ThenInclude(psp => psp.PackagingSupplier)
                    .ThenInclude(ps => ps!.SuppliedBySupplier)
            .ToListAsync();

        var suppliersList = supplyChain.Select(msp => msp.PackagingSupplierProduct).Select(psp => new
        {
            id = psp.Id,
            productName = psp.Name,
            productCode = psp.ProductCode,
            supplier = new
            {
                id = psp.PackagingSupplier.Id,
                name = psp.PackagingSupplier.Name,
                suppliedBy = psp.PackagingSupplier.SuppliedBySupplier != null ? new
                {
                    id = psp.PackagingSupplier.SuppliedBySupplier.Id,
                    name = psp.PackagingSupplier.SuppliedBySupplier.Name
                } : null
            }
        }).ToList();

        return new
        {
            id = material.Id,
            name = material.DisplayName,
            code = material.Code,
            description = material.Description,
            iconClass = material.IconClass,
            level = material.Level,
            parent = material.ParentTaxonomy != null ? new
            {
                id = material.ParentTaxonomy.Id,
                name = material.ParentTaxonomy.DisplayName,
                code = material.ParentTaxonomy.Code
            } : null,
            children = material.ChildTaxonomies.Select(c => new
            {
                id = c.Id,
                name = c.DisplayName,
                code = c.Code,
                level = c.Level
            }).ToList(),
            packagingItems = packagingItems,
            supplyChain = suppliersList
        };
    }

    private async Task<object?> GetPackagingItemDetail(int id)
    {
        var item = await _context.PackagingLibraries
            .Where(l => l.Id == id && l.IsActive)
            .Include(l => l.MaterialTaxonomy)
            .Include(l => l.PackagingLibraryMaterials)
                .ThenInclude(plm => plm.MaterialTaxonomy)
            .Include(l => l.PackagingLibrarySupplierProducts)
                .ThenInclude(plsp => plsp.PackagingSupplierProduct)
                    .ThenInclude(psp => psp.PackagingSupplier)
                        .ThenInclude(ps => ps!.SuppliedBySupplier)
            .Include(l => l.PackagingGroupItems)
                .ThenInclude(gi => gi.PackagingGroup)
            .FirstOrDefaultAsync();

        if (item == null) return null;

        var rawMaterials = item.PackagingLibraryMaterials
            .OrderBy(plm => plm.SortOrder)
            .Select(plm => plm.MaterialTaxonomy)
            .Where(mt => mt != null)
            .Select(mt => (object)new { id = mt!.Id, name = mt!.DisplayName, code = mt.Code })
            .ToList();
        if (rawMaterials.Count == 0 && item.MaterialTaxonomy != null)
            rawMaterials = new List<object> { new { id = item.MaterialTaxonomy.Id, name = item.MaterialTaxonomy.DisplayName, code = item.MaterialTaxonomy.Code } };

        var supplyChain = item.PackagingLibrarySupplierProducts.Select(plsp => new
        {
            productId = plsp.PackagingSupplierProduct.Id,
            productName = plsp.PackagingSupplierProduct.Name,
            productCode = plsp.PackagingSupplierProduct.ProductCode,
            supplier = new
            {
                id = plsp.PackagingSupplierProduct.PackagingSupplier.Id,
                name = plsp.PackagingSupplierProduct.PackagingSupplier.Name,
                suppliedBy = plsp.PackagingSupplierProduct.PackagingSupplier.SuppliedBySupplier != null ? new
                {
                    id = plsp.PackagingSupplierProduct.PackagingSupplier.SuppliedBySupplier.Id,
                    name = plsp.PackagingSupplierProduct.PackagingSupplier.SuppliedBySupplier.Name
                } : null
            }
        }).ToList();

        return new
        {
            id = item.Id,
            name = item.Name,
            taxonomyCode = item.TaxonomyCode,
            weight = item.Weight,
            materialTaxonomy = item.MaterialTaxonomy != null ? new
            {
                id = item.MaterialTaxonomy.Id,
                name = item.MaterialTaxonomy.DisplayName,
                code = item.MaterialTaxonomy.Code
            } : null,
            rawMaterials = rawMaterials,
            supplyChain = supplyChain,
            packagingGroups = item.PackagingGroupItems.Select(gi => new
            {
                id = gi.PackagingGroup.Id,
                name = gi.PackagingGroup.Name,
                packId = gi.PackagingGroup.PackId,
                sortOrder = gi.SortOrder
            }).ToList()
        };
    }

    private async Task<object> GetSuppliersList(int page, int pageSize, string sortBy, string sortDir, string filter)
    {
        var query = _context.PackagingSuppliers
            .Where(s => s.IsActive)
            .AsQueryable();

        if (!string.IsNullOrEmpty(filter))
        {
            query = query.Where(s =>
                s.Name.Contains(filter) ||
                (s.Address != null && s.Address.Contains(filter)) ||
                (s.City != null && s.City.Contains(filter)) ||
                (s.State != null && s.State.Contains(filter)) ||
                (s.Country != null && s.Country.Contains(filter)) ||
                (s.Email != null && s.Email.Contains(filter)));
        }

        query = sortBy.ToLower() switch
        {
            "address" => sortDir == "asc" ? query.OrderBy(s => s.Address ?? "") : query.OrderByDescending(s => s.Address ?? ""),
            "city" => sortDir == "asc" ? query.OrderBy(s => s.City ?? "") : query.OrderByDescending(s => s.City ?? ""),
            "state" => sortDir == "asc" ? query.OrderBy(s => s.State ?? "") : query.OrderByDescending(s => s.State ?? ""),
            "country" => sortDir == "asc" ? query.OrderBy(s => s.Country ?? "") : query.OrderByDescending(s => s.Country ?? ""),
            "email" => sortDir == "asc" ? query.OrderBy(s => s.Email ?? "") : query.OrderByDescending(s => s.Email ?? ""),
            _ => sortDir == "asc" ? query.OrderBy(s => s.Name) : query.OrderByDescending(s => s.Name)
        };

        var totalCount = await query.CountAsync();
        var itemsList = await query
            .Include(s => s.Contacts)
            .Include(s => s.Products)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        var items = itemsList.Select(s => new
        {
            id = s.Id,
            name = s.Name,
            address = s.Address,
            city = s.City,
            state = s.State,
            country = s.Country,
            phone = s.Phone,
            email = s.Email,
            contactCount = s.Contacts.Count,
            productCount = s.Products.Count
        }).ToList();

        return new
        {
            items,
            totalCount,
            page,
            pageSize,
            totalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
        };
    }

    private async Task<object?> GetSupplierDetail(int id)
    {
        var supplier = await _context.PackagingSuppliers
            .Where(s => s.Id == id && s.IsActive)
            .Include(s => s.SuppliedBySupplier)
            .Include(s => s.Contacts)
            .Include(s => s.Products)
            .FirstOrDefaultAsync();

        if (supplier == null) return null;

        return new
        {
            id = supplier.Id,
            name = supplier.Name,
            address = supplier.Address,
            city = supplier.City,
            state = supplier.State,
            country = supplier.Country,
            phone = supplier.Phone,
            email = supplier.Email,
            website = supplier.Website,
            suppliedBySupplierId = supplier.SuppliedBySupplierId,
            suppliedBySupplier = supplier.SuppliedBySupplier != null ? new { id = supplier.SuppliedBySupplier.Id, name = supplier.SuppliedBySupplier.Name } : null,
            contacts = supplier.Contacts.Select(c => new
            {
                id = c.Id,
                name = c.Name,
                title = c.Title,
                phone = c.Phone,
                email = c.Email
            }).ToList(),
            products = supplier.Products.Select(p => new
            {
                id = p.Id,
                name = p.Name,
                description = p.Description,
                productCode = p.ProductCode,
                taxonomyCode = p.TaxonomyCode
            }).ToList()
        };
    }

    [HttpPost]
    [Route("api/packaging-management/packaging-items/{id}/raw-materials")]
    public async Task<IActionResult> AddRawMaterialToPackagingItem(int id, [FromBody] AddRawMaterialRequest request)
    {
        var item = await _context.PackagingLibraries.FindAsync(id);
        if (item == null) return NotFound();
        var material = await _context.MaterialTaxonomies.FindAsync(request.MaterialTaxonomyId);
        if (material == null) return BadRequest("Raw material not found");
        var exists = await _context.PackagingLibraryMaterials.AnyAsync(plm => plm.PackagingLibraryId == id && plm.MaterialTaxonomyId == request.MaterialTaxonomyId);
        if (exists) return BadRequest("Raw material already linked");
        var maxOrder = await _context.PackagingLibraryMaterials.Where(plm => plm.PackagingLibraryId == id).MaxAsync(plm => (int?)plm.SortOrder) ?? -1;
        _context.PackagingLibraryMaterials.Add(new PackagingLibraryMaterial { PackagingLibraryId = id, MaterialTaxonomyId = request.MaterialTaxonomyId, SortOrder = maxOrder + 1 });
        await _context.SaveChangesAsync();
        return Json(new { success = true });
    }

    [HttpDelete]
    [Route("api/packaging-management/packaging-items/{id}/raw-materials/{materialId}")]
    public async Task<IActionResult> RemoveRawMaterialFromPackagingItem(int id, int materialId)
    {
        var link = await _context.PackagingLibraryMaterials.FirstOrDefaultAsync(plm => plm.PackagingLibraryId == id && plm.MaterialTaxonomyId == materialId);
        if (link == null) return NotFound();
        _context.PackagingLibraryMaterials.Remove(link);
        await _context.SaveChangesAsync();
        return Json(new { success = true });
    }

    [HttpPost]
    [Route("api/packaging-management/packaging-items/{id}/suppliers")]
    public async Task<IActionResult> AddSupplierToPackagingItem(int id, [FromBody] AddSupplierProductRequest request)
    {
        var item = await _context.PackagingLibraries.FindAsync(id);
        if (item == null) return NotFound();
        var product = await _context.PackagingSupplierProducts.FindAsync(request.PackagingSupplierProductId);
        if (product == null) return BadRequest("Supplier product not found");
        var exists = await _context.PackagingLibrarySupplierProducts.AnyAsync(plsp => plsp.PackagingLibraryId == id && plsp.PackagingSupplierProductId == request.PackagingSupplierProductId);
        if (exists) return BadRequest("Supplier product already linked");
        _context.PackagingLibrarySupplierProducts.Add(new PackagingLibrarySupplierProduct { PackagingLibraryId = id, PackagingSupplierProductId = request.PackagingSupplierProductId });
        await _context.SaveChangesAsync();
        return Json(new { success = true });
    }

    [HttpDelete]
    [Route("api/packaging-management/packaging-items/{id}/suppliers/{productId}")]
    public async Task<IActionResult> RemoveSupplierFromPackagingItem(int id, int productId)
    {
        var link = await _context.PackagingLibrarySupplierProducts.FirstOrDefaultAsync(plsp => plsp.PackagingLibraryId == id && plsp.PackagingSupplierProductId == productId);
        if (link == null) return NotFound();
        _context.PackagingLibrarySupplierProducts.Remove(link);
        await _context.SaveChangesAsync();
        return Json(new { success = true });
    }

    [HttpPost]
    [Route("api/packaging-management/raw-materials/{id}/suppliers")]
    public async Task<IActionResult> AddSupplierToRawMaterial(int id, [FromBody] AddSupplierProductRequest request)
    {
        var material = await _context.MaterialTaxonomies.FindAsync(id);
        if (material == null) return NotFound();
        var product = await _context.PackagingSupplierProducts.FindAsync(request.PackagingSupplierProductId);
        if (product == null) return BadRequest("Supplier product not found");
        var exists = await _context.MaterialTaxonomySupplierProducts.AnyAsync(msp => msp.MaterialTaxonomyId == id && msp.PackagingSupplierProductId == request.PackagingSupplierProductId);
        if (exists) return BadRequest("Supplier product already linked");
        _context.MaterialTaxonomySupplierProducts.Add(new MaterialTaxonomySupplierProduct { MaterialTaxonomyId = id, PackagingSupplierProductId = request.PackagingSupplierProductId });
        await _context.SaveChangesAsync();
        return Json(new { success = true });
    }

    [HttpDelete]
    [Route("api/packaging-management/raw-materials/{id}/suppliers/{productId}")]
    public async Task<IActionResult> RemoveSupplierFromRawMaterial(int id, int productId)
    {
        var link = await _context.MaterialTaxonomySupplierProducts.FirstOrDefaultAsync(msp => msp.MaterialTaxonomyId == id && msp.PackagingSupplierProductId == productId);
        if (link == null) return NotFound();
        _context.MaterialTaxonomySupplierProducts.Remove(link);
        await _context.SaveChangesAsync();
        return Json(new { success = true });
    }

    [HttpGet]
    [Route("api/packaging-management/taxonomy-tree")]
    public async Task<IActionResult> GetTaxonomyTree()
    {
        try
        {
            var rootMaterials = await _context.MaterialTaxonomies
                .Where(t => t.Level == 1 && t.IsActive)
                .Include(t => t.ChildTaxonomies.Where(c => c.IsActive))
                    .ThenInclude(c => c.ChildTaxonomies.Where(c2 => c2.IsActive))
                    .ThenInclude(c2 => c2.ChildTaxonomies.Where(c3 => c3.IsActive))
                    .ThenInclude(c3 => c3.ChildTaxonomies.Where(c4 => c4.IsActive))
                    .ThenInclude(c4 => c4.ChildTaxonomies.Where(c5 => c5.IsActive))
                .OrderBy(t => t.SortOrder).ThenBy(t => t.DisplayName)
                .ToListAsync();

            HashSet<int> CollectAllMaterialIds(MaterialTaxonomy taxonomy)
            {
                var ids = new HashSet<int> { taxonomy.Id };
                foreach (var child in taxonomy.ChildTaxonomies.Where(c => c.IsActive))
                    ids.UnionWith(CollectAllMaterialIds(child));
                return ids;
            }

            var packagingItemsByMaterial = await _context.PackagingLibraryMaterials
                .Include(plm => plm.PackagingLibrary)
                    .ThenInclude(l => l!.PackagingGroupItems)
                        .ThenInclude(gi => gi.PackagingGroup)
                .Include(plm => plm.MaterialTaxonomy)
                .Where(plm => plm.PackagingLibrary != null && plm.PackagingLibrary.IsActive)
                .ToListAsync();
            var legacyItems = await _context.PackagingLibraries
                .Where(l => l.IsActive && l.MaterialTaxonomyId != null)
                .Include(l => l.PackagingGroupItems)
                    .ThenInclude(gi => gi.PackagingGroup)
                .ToListAsync();

            object BuildNode(MaterialTaxonomy mt)
            {
                var allIds = CollectAllMaterialIds(mt);
                var itemsFromJoin = packagingItemsByMaterial
                    .Where(plm => allIds.Contains(plm.MaterialTaxonomyId) && plm.PackagingLibrary != null)
                    .Select(plm => plm.PackagingLibrary!)
                    .Distinct()
                    .ToList();
                var itemsFromLegacy = legacyItems.Where(l => l.MaterialTaxonomyId.HasValue && allIds.Contains(l.MaterialTaxonomyId!.Value)).ToList();
                var packagingItems = itemsFromJoin.Union(itemsFromLegacy).GroupBy(l => l.Id).Select(g => g.First()).ToList();

                var children = mt.ChildTaxonomies.Where(c => c.IsActive).OrderBy(c => c.SortOrder).ThenBy(c => c.DisplayName)
                    .Select(c => BuildNode(c)).ToList();

                var itemNodes = packagingItems.Select(pi => new
                {
                    id = pi.Id,
                    type = "packaging-item",
                    name = pi.Name,
                    taxonomyCode = pi.TaxonomyCode,
                    weight = pi.Weight,
                    packagingGroups = pi.PackagingGroupItems
                        .Where(gi => gi.PackagingGroup != null && gi.PackagingGroup.IsActive)
                        .Select(gi => new
                        {
                            id = gi.PackagingGroup!.Id,
                            type = "packaging-group",
                            name = gi.PackagingGroup.Name,
                            packId = gi.PackagingGroup.PackId
                        }).ToList()
                }).ToList();

                return new
                {
                    id = mt.Id,
                    type = "raw-material",
                    name = mt.DisplayName,
                    code = mt.Code,
                    level = mt.Level,
                    children = children,
                    packagingItems = itemNodes
                };
            }

            var tree = rootMaterials.Select(mt => BuildNode(mt)).ToList();

            var orphanItems = await _context.PackagingLibraries
                .Where(l => l.IsActive)
                .Where(l => !l.PackagingLibraryMaterials.Any() && l.MaterialTaxonomyId == null)
                .Include(l => l.PackagingGroupItems)
                    .ThenInclude(gi => gi.PackagingGroup)
                .ToListAsync();

            var orphanNodes = orphanItems.Select(pi => new
            {
                id = pi.Id,
                type = "packaging-item",
                name = pi.Name,
                taxonomyCode = pi.TaxonomyCode,
                weight = pi.Weight,
                packagingGroups = pi.PackagingGroupItems
                    .Where(gi => gi.PackagingGroup != null && gi.PackagingGroup.IsActive)
                    .Select(gi => new
                    {
                        id = gi.PackagingGroup!.Id,
                        type = "packaging-group",
                        name = gi.PackagingGroup.Name,
                        packId = gi.PackagingGroup.PackId
                    }).ToList()
            }).ToList<object>();

            var standaloneGroupsList = await _context.PackagingGroups
                .Where(g => g.IsActive && !g.Items.Any())
                .OrderBy(g => g.Name)
                .Select(g => new
                {
                    id = g.Id,
                    type = "packaging-group",
                    name = g.Name,
                    packId = g.PackId,
                    packagingGroups = Array.Empty<object>()
                })
                .ToListAsync();
            var standaloneGroups = standaloneGroupsList.Cast<object>().ToList();

            var allPackagingItemsList = await _context.PackagingLibraries
                .Where(l => l.IsActive)
                .Include(l => l.PackagingGroupItems)
                    .ThenInclude(gi => gi.PackagingGroup)
                .OrderBy(l => l.Name)
                .ToListAsync();
            var allPackagingItems = allPackagingItemsList.Select(l => (object)new
            {
                id = l.Id,
                type = "packaging-item",
                name = l.Name,
                taxonomyCode = l.TaxonomyCode,
                weight = l.Weight,
                packagingGroups = l.PackagingGroupItems
                    .Where(gi => gi.PackagingGroup != null && gi.PackagingGroup.IsActive)
                    .Select(gi => (object)new
                    {
                        id = gi.PackagingGroup!.Id,
                        type = "packaging-group",
                        name = gi.PackagingGroup.Name,
                        packId = gi.PackagingGroup.PackId
                    }).ToList()
            }).ToList();

            var allPackagingGroupsList = await _context.PackagingGroups
                .Where(g => g.IsActive)
                .OrderBy(g => g.Name)
                .Select(g => new
                {
                    id = g.Id,
                    type = "packaging-group",
                    name = g.Name,
                    packId = g.PackId,
                    packagingGroups = Array.Empty<object>()
                })
                .ToListAsync();
            var allPackagingGroups = allPackagingGroupsList.Cast<object>().ToList();

            return Json(new { tree, orphanItems = orphanNodes, standaloneGroups, allPackagingItems, allPackagingGroups });
        }
        catch (Exception ex)
        {
            return Json(new { error = ex.Message });
        }
    }

    [HttpGet]
    [Route("api/packaging-management/options/raw-materials")]
    public async Task<IActionResult> GetRawMaterialOptions()
    {
        var items = await _context.MaterialTaxonomies
            .Where(t => t.IsActive)
            .OrderBy(t => t.Level).ThenBy(t => t.DisplayName)
            .Select(t => new { id = t.Id, name = t.DisplayName, code = t.Code, level = t.Level })
            .ToListAsync();
        return Json(items);
    }

    [HttpGet]
    [Route("api/packaging-management/options/packaging-items")]
    public async Task<IActionResult> GetPackagingItemOptions()
    {
        var items = await _context.PackagingLibraries
            .Where(l => l.IsActive)
            .OrderBy(l => l.Name)
            .Select(l => new { id = l.Id, name = l.Name, taxonomyCode = l.TaxonomyCode })
            .ToListAsync();
        return Json(items);
    }

    [HttpGet]
    [Route("api/packaging-management/options/supplier-products")]
    public async Task<IActionResult> GetSupplierProductOptions()
    {
        var items = await _context.PackagingSupplierProducts
            .Include(p => p.PackagingSupplier)
            .Where(p => p.PackagingSupplier != null && p.PackagingSupplier.IsActive)
            .OrderBy(p => p.Name)
            .Select(p => new { id = p.Id, name = p.Name, supplierName = p.PackagingSupplier!.Name })
            .ToListAsync();
        return Json(items);
    }

    [HttpPost]
    [Route("api/packaging-management/raw-materials")]
    public async Task<IActionResult> CreateRawMaterial([FromBody] CreateRawMaterialRequest request)
    {
        try
        {
            var level = request.Level ?? 1;
            int? parentId = request.ParentTaxonomyId > 0 ? request.ParentTaxonomyId : null;
            if (parentId.HasValue)
            {
                var parent = await _context.MaterialTaxonomies.FindAsync(parentId.Value);
                if (parent == null) return BadRequest("Parent not found");
                level = parent.Level + 1;
            }
            var maxOrder = await _context.MaterialTaxonomies
                .Where(t => t.ParentTaxonomyId == parentId)
                .MaxAsync(t => (int?)t.SortOrder) ?? -1;
            var material = new MaterialTaxonomy
            {
                DisplayName = request.Name ?? "",
                Code = request.Code ?? "",
                Description = request.Description,
                Level = level,
                ParentTaxonomyId = parentId,
                SortOrder = maxOrder + 1,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };
            _context.MaterialTaxonomies.Add(material);
            await _context.SaveChangesAsync();
            return Json(new { id = material.Id, success = true });
        }
        catch (Exception ex)
        {
            return Json(new { error = ex.Message, success = false });
        }
    }

    [HttpPost]
    [Route("api/packaging-management/packaging-items")]
    public async Task<IActionResult> CreatePackagingItem([FromBody] CreatePackagingItemRequest request)
    {
        try
        {
            var item = new PackagingLibrary
            {
                Name = request.Name ?? "",
                TaxonomyCode = request.TaxonomyCode ?? "",
                Weight = request.Weight,
                MaterialTaxonomyId = request.MaterialTaxonomyId > 0 ? request.MaterialTaxonomyId : null,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };
            _context.PackagingLibraries.Add(item);
            await _context.SaveChangesAsync();
            if (request.MaterialTaxonomyIds != null && request.MaterialTaxonomyIds.Count > 0)
            {
                int order = 0;
                foreach (var mid in request.MaterialTaxonomyIds)
                {
                    if (mid <= 0) continue;
                    _context.PackagingLibraryMaterials.Add(new PackagingLibraryMaterial
                    {
                        PackagingLibraryId = item.Id,
                        MaterialTaxonomyId = mid,
                        SortOrder = order++
                    });
                }
                await _context.SaveChangesAsync();
            }
            if (request.PackagingSupplierProductIds != null && request.PackagingSupplierProductIds.Count > 0)
            {
                foreach (var pid in request.PackagingSupplierProductIds)
                {
                    if (pid <= 0) continue;
                    _context.PackagingLibrarySupplierProducts.Add(new PackagingLibrarySupplierProduct
                    {
                        PackagingLibraryId = item.Id,
                        PackagingSupplierProductId = pid
                    });
                }
                await _context.SaveChangesAsync();
            }
            return Json(new { id = item.Id, success = true });
        }
        catch (Exception ex)
        {
            return Json(new { error = ex.Message, success = false });
        }
    }

    [HttpPost]
    [Route("api/packaging-management/packaging-groups")]
    public async Task<IActionResult> CreatePackagingGroup([FromBody] CreatePackagingGroupRequest request)
    {
        try
        {
            var group = new PackagingGroup
            {
                Name = request.Name ?? "",
                PackId = request.PackId ?? "",
                PackagingLayer = request.PackagingLayer,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };
            _context.PackagingGroups.Add(group);
            await _context.SaveChangesAsync();
            if (request.PackagingLibraryIds != null && request.PackagingLibraryIds.Count > 0)
            {
                int order = 0;
                foreach (var lid in request.PackagingLibraryIds)
                {
                    if (lid <= 0) continue;
                    _context.PackagingGroupItems.Add(new PackagingGroupItem
                    {
                        PackagingGroupId = group.Id,
                        PackagingLibraryId = lid,
                        SortOrder = order++
                    });
                }
                await _context.SaveChangesAsync();
            }
            return Json(new { id = group.Id, success = true });
        }
        catch (Exception ex)
        {
            return Json(new { error = ex.Message, success = false });
        }
    }

    [HttpPost]
    [Route("api/packaging-management/suppliers")]
    public async Task<IActionResult> CreateSupplier([FromBody] CreateSupplierRequest request)
    {
        try
        {
            var supplier = new PackagingSupplier
            {
                Name = request.Name ?? "",
                Address = request.Address,
                City = request.City,
                State = request.State,
                Country = request.Country,
                Phone = request.Phone,
                Email = request.Email,
                Website = request.Website,
                SuppliedBySupplierId = request.SuppliedBySupplierId,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };
            _context.PackagingSuppliers.Add(supplier);
            await _context.SaveChangesAsync();

            if (request.Contacts != null && request.Contacts.Count > 0)
            {
                foreach (var c in request.Contacts)
                {
                    if (string.IsNullOrWhiteSpace(c.Name)) continue;
                    _context.PackagingSupplierContacts.Add(new PackagingSupplierContact
                    {
                        PackagingSupplierId = supplier.Id,
                        Name = c.Name.Trim(),
                        Title = c.Title,
                        Phone = c.Phone,
                        Email = c.Email
                    });
                }
                await _context.SaveChangesAsync();
            }

            return Json(new { id = supplier.Id, success = true });
        }
        catch (Exception ex)
        {
            return Json(new { error = ex.Message, success = false });
        }
    }

    [HttpPost]
    [Route("api/packaging-management/suppliers/{supplierId}/products")]
    public async Task<IActionResult> CreateSupplierProduct(int supplierId, [FromBody] CreateSupplierProductRequest request)
    {
        try
        {
            var supplier = await _context.PackagingSuppliers.FindAsync(supplierId);
            if (supplier == null) return NotFound();

            var product = new PackagingSupplierProduct
            {
                PackagingSupplierId = supplierId,
                Name = request.Name ?? "",
                Description = request.Description,
                ProductCode = request.ProductCode,
                TaxonomyCode = request.TaxonomyCode
            };
            _context.PackagingSupplierProducts.Add(product);
            await _context.SaveChangesAsync();
            return Json(new { id = product.Id, success = true });
        }
        catch (Exception ex)
        {
            return Json(new { error = ex.Message, success = false });
        }
    }

    [HttpPost]
    [Route("api/packaging-management/suppliers/{supplierId}/contacts")]
    public async Task<IActionResult> CreateSupplierContact(int supplierId, [FromBody] CreateSupplierContactRequest request)
    {
        try
        {
            var supplier = await _context.PackagingSuppliers.FindAsync(supplierId);
            if (supplier == null) return NotFound();

            var contact = new PackagingSupplierContact
            {
                PackagingSupplierId = supplierId,
                Name = request.Name ?? "",
                Title = request.Title,
                Phone = request.Phone,
                Email = request.Email
            };
            _context.PackagingSupplierContacts.Add(contact);
            await _context.SaveChangesAsync();
            return Json(new { id = contact.Id, success = true });
        }
        catch (Exception ex)
        {
            return Json(new { error = ex.Message, success = false });
        }
    }

    private async Task<object?> GetPackagingGroupDetail(int id)
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
            .Include(g => g.Items)
                .ThenInclude(i => i.PackagingLibrary)
                    .ThenInclude(l => l!.PackagingLibrarySupplierProducts)
                        .ThenInclude(plsp => plsp.PackagingSupplierProduct)
                            .ThenInclude(psp => psp.PackagingSupplier)
                                .ThenInclude(ps => ps!.SuppliedBySupplier)
            .FirstOrDefaultAsync();

        if (group == null) return null;

        return new
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
                var rawMatsList = lib.PackagingLibraryMaterials
                    .OrderBy(plm => plm.SortOrder)
                    .Select(plm => plm.MaterialTaxonomy)
                    .Where(mt => mt != null)
                    .Select(mt => new { id = mt!.Id, name = mt.DisplayName, code = mt.Code })
                    .ToList();
                var rawMats = rawMatsList.Count > 0
                    ? rawMatsList.Cast<object>().ToList()
                    : (lib.MaterialTaxonomy != null
                        ? new List<object> { new { id = lib.MaterialTaxonomy.Id, name = lib.MaterialTaxonomy.DisplayName, code = lib.MaterialTaxonomy.Code } }
                        : new List<object>());
                var supplyChain = lib.PackagingLibrarySupplierProducts.Select(plsp => new
                {
                    productName = plsp.PackagingSupplierProduct.Name,
                    supplier = new
                    {
                        id = plsp.PackagingSupplierProduct.PackagingSupplier.Id,
                        name = plsp.PackagingSupplierProduct.PackagingSupplier.Name,
                        suppliedBy = plsp.PackagingSupplierProduct.PackagingSupplier.SuppliedBySupplier != null ? new
                        {
                            id = plsp.PackagingSupplierProduct.PackagingSupplier.SuppliedBySupplier.Id,
                            name = plsp.PackagingSupplierProduct.PackagingSupplier.SuppliedBySupplier.Name
                        } : null
                    }
                }).ToList();
                return new
                {
                    id = lib.Id,
                    name = lib.Name,
                    taxonomyCode = lib.TaxonomyCode,
                    weight = lib.Weight,
                    materialTaxonomy = lib.MaterialTaxonomy != null ? new
                    {
                        id = lib.MaterialTaxonomy.Id,
                        name = lib.MaterialTaxonomy.DisplayName,
                        code = lib.MaterialTaxonomy.Code
                    } : null,
                    rawMaterials = rawMats,
                    supplyChain = supplyChain,
                    sortOrder = i.SortOrder
                };
            }).ToList()
        };
    }
}

public class CreateSupplierRequest
{
    public string? Name { get; set; }
    public string? Address { get; set; }
    public string? City { get; set; }
    public string? State { get; set; }
    public string? Country { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public string? Website { get; set; }
    public int? SuppliedBySupplierId { get; set; }
    public List<CreateSupplierContactRequest>? Contacts { get; set; }
}

public class AddRawMaterialRequest
{
    public int MaterialTaxonomyId { get; set; }
}

public class AddSupplierProductRequest
{
    public int PackagingSupplierProductId { get; set; }
}

public class CreateSupplierProductRequest
{
    public string? Name { get; set; }
    public string? Description { get; set; }
    public string? ProductCode { get; set; }
    public string? TaxonomyCode { get; set; }
}

public class CreateSupplierContactRequest
{
    public string? Name { get; set; }
    public string? Title { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
}

public class CreateRawMaterialRequest
{
    public string? Name { get; set; }
    public string? Code { get; set; }
    public string? Description { get; set; }
    public int? Level { get; set; }
    public int? ParentTaxonomyId { get; set; }
}

public class CreatePackagingItemRequest
{
    public string? Name { get; set; }
    public string? TaxonomyCode { get; set; }
    public decimal? Weight { get; set; }
    public int? MaterialTaxonomyId { get; set; }
    public List<int>? MaterialTaxonomyIds { get; set; }
    public List<int>? PackagingSupplierProductIds { get; set; }
}

public class CreatePackagingGroupRequest
{
    public string? Name { get; set; }
    public string? PackId { get; set; }
    public string? PackagingLayer { get; set; }
    public List<int>? PackagingLibraryIds { get; set; }
}

