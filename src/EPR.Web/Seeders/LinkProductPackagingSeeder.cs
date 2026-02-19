using EPR.Domain.Entities;
using EPR.Data;
using Microsoft.EntityFrameworkCore;

namespace EPR.Web.Seeders;

/// <summary>
/// Links product data to packaging data so relationships between packaging, product and distribution
/// are visible. Creates PackagingUnits from PackagingGroups, links all products to packaging,
/// and ensures Distribution uses correct PackagingUnit per product.
/// Run after PackagingDummyDataSeeder and CreateDummyAsnData.
/// </summary>
public class LinkProductPackagingSeeder
{
    private readonly EPRDbContext _context;

    public LinkProductPackagingSeeder(EPRDbContext context)
    {
        _context = context;
    }

    public async Task SeedAsync()
    {
        var now = DateTime.UtcNow;

        // 1. Ensure PackagingLibrary and PackagingGroups exist
        var packagingLibraries = await _context.PackagingLibraries.ToListAsync();
        var packagingGroups = await _context.PackagingGroups
            .Include(g => g.Items)
            .ThenInclude(gi => gi.PackagingLibrary)
            .ToListAsync();

        if (!packagingLibraries.Any() || !packagingGroups.Any())
        {
            // PackagingDummyDataSeeder hasn't run or has no data - nothing to link
            return;
        }

        // 2. Get or create PackagingType for each PackagingLibrary (bridge Product system to Packaging Management)
        var libToType = new Dictionary<int, int>();
        foreach (var lib in packagingLibraries)
        {
            var pt = await _context.PackagingTypes.FirstOrDefaultAsync(t => t.Name == lib.Name);
            if (pt == null)
            {
                pt = new PackagingType
                {
                    Name = lib.Name,
                    Weight = lib.Weight,
                    IsFromLibrary = true,
                    LibrarySource = lib.TaxonomyCode ?? "PackagingLibrary",
                    Notes = $"Linked from PackagingLibrary (Id={lib.Id})",
                    CreatedAt = now
                };
                _context.PackagingTypes.Add(pt);
                await _context.SaveChangesAsync();
            }
            libToType[lib.Id] = pt.Id;
        }

        // 3. Create PackagingUnits from PackagingGroups
        var groupToUnit = new Dictionary<int, int>();
        foreach (var group in packagingGroups)
        {
            var unit = await _context.PackagingUnits.FirstOrDefaultAsync(u => u.Name == group.Name);
            if (unit == null)
            {
                unit = new PackagingUnit
                {
                    Name = group.Name,
                    UnitLevel = group.PackagingLayer ?? "Primary",
                    Notes = $"Linked from PackagingGroup {group.PackId}",
                    CreatedAt = now
                };
                _context.PackagingUnits.Add(unit);
                await _context.SaveChangesAsync();
            }
            groupToUnit[group.Id] = unit.Id;

            // Add PackagingUnitItems for each PackagingGroupItem
            foreach (var gi in group.Items ?? Enumerable.Empty<PackagingGroupItem>())
            {
                if (!libToType.TryGetValue(gi.PackagingLibraryId, out var typeId))
                    continue;

                var exists = await _context.PackagingUnitItems
                    .AnyAsync(i => i.PackagingUnitId == unit.Id && i.PackagingTypeId == typeId);
                if (!exists)
                {
                    _context.PackagingUnitItems.Add(new PackagingUnitItem
                    {
                        PackagingUnitId = unit.Id,
                        PackagingTypeId = typeId,
                        CollectionName = "Default",
                        Quantity = 1
                    });
                }
            }
        }
        await _context.SaveChangesAsync();

        // 4. Get all products and all PackagingUnit ids (from both CreateDummyAsnData and our new ones)
        var products = await _context.Products.OrderBy(p => p.Id).ToListAsync();
        var allPackagingUnits = await _context.PackagingUnits.OrderBy(u => u.Id).ToListAsync();

        if (!products.Any() || !allPackagingUnits.Any())
            return;

        // 5. Link ALL products to ProductPackaging (each product gets at least one PackagingUnit)
        var unitIds = allPackagingUnits.Select(u => u.Id).ToList();
        var rnd = new Random(42);
        foreach (var product in products)
        {
            var existingLinks = await _context.ProductPackagings
                .Where(pp => pp.ProductId == product.Id)
                .Select(pp => pp.PackagingUnitId)
                .ToListAsync();

            if (!existingLinks.Any())
            {
                // Assign 1–2 PackagingUnits per product
                var toLink = unitIds.OrderBy(_ => rnd.Next()).Take(rnd.Next(1, 3)).Distinct().ToList();
                foreach (var unitId in toLink)
                {
                    _context.ProductPackagings.Add(new ProductPackaging
                    {
                        ProductId = product.Id,
                        PackagingUnitId = unitId
                    });
                }
            }
            else
            {
                // Product already has some links - ensure it also has at least one from our PackagingGroups
                var fromGroups = groupToUnit.Values.ToList();
                var hasGroupUnit = existingLinks.Any(id => fromGroups.Contains(id));
                if (!hasGroupUnit && fromGroups.Any())
                {
                    var unitId = fromGroups[rnd.Next(fromGroups.Count)];
                    if (!existingLinks.Contains(unitId))
                    {
                        _context.ProductPackagings.Add(new ProductPackaging
                        {
                            ProductId = product.Id,
                            PackagingUnitId = unitId
                        });
                    }
                }
            }
        }
        await _context.SaveChangesAsync();

        // 6. Link products to ProductPackagingSupplierProduct (Product -> PackagingSupplierProduct)
        var supplierProducts = await (from psp in _context.PackagingSupplierProducts
            join ps in _context.PackagingSuppliers on psp.PackagingSupplierId equals ps.Id
            where ps.IsActive
            select psp.Id).ToListAsync();

        if (supplierProducts.Any())
        {
            foreach (var product in products)
            {
                var existing = await _context.ProductPackagingSupplierProducts
                    .AnyAsync(pp => pp.ProductId == product.Id);
                if (!existing)
                {
                    var spId = supplierProducts[rnd.Next(supplierProducts.Count)];
                    _context.ProductPackagingSupplierProducts.Add(new ProductPackagingSupplierProduct
                    {
                        ProductId = product.Id,
                        PackagingSupplierProductId = spId
                    });
                }
            }
            await _context.SaveChangesAsync();
        }

        // 7. Fix Distribution: ensure each record uses a PackagingUnit that the product is linked to
        var distributions = await _context.Distributions.ToListAsync();
        foreach (var dist in distributions)
        {
            var productPackagingUnitIds = await _context.ProductPackagings
                .Where(pp => pp.ProductId == dist.ProductId)
                .Select(pp => pp.PackagingUnitId)
                .ToListAsync();

            if (productPackagingUnitIds.Any() && !productPackagingUnitIds.Contains(dist.PackagingUnitId))
            {
                dist.PackagingUnitId = productPackagingUnitIds[rnd.Next(productPackagingUnitIds.Count)];
            }
        }
        await _context.SaveChangesAsync();

        // 8. Ensure products without Distribution get at least one
        var productIdsWithDist = await _context.Distributions.Select(d => d.ProductId).Distinct().ToListAsync();
        var productsWithoutDist = products.Where(p => !productIdsWithDist.Contains(p.Id)).ToList();
        var states = new[] { "NSW", "VIC", "QLD", "WA", "SA", "TAS", "ACT", "NT" };

        foreach (var product in productsWithoutDist)
        {
            var productPackagingUnitIds = await _context.ProductPackagings
                .Where(pp => pp.ProductId == product.Id)
                .Select(pp => pp.PackagingUnitId)
                .ToListAsync();

            var packUnitId = productPackagingUnitIds.FirstOrDefault();
            if (packUnitId == 0)
                packUnitId = unitIds.First();

            var state = states[rnd.Next(states.Length)];
            var city = state == "NSW" ? "Sydney" : state == "VIC" ? "Melbourne" : state == "QLD" ? "Brisbane" : state == "WA" ? "Perth" : state == "SA" ? "Adelaide" : "Hobart";

            _context.Distributions.Add(new Distribution
            {
                ProductId = product.Id,
                PackagingUnitId = packUnitId,
                Quantity = (decimal)(rnd.Next(10, 500)),
                City = city,
                StateProvince = state,
                Country = "AU",
                DispatchDate = now.AddDays(-rnd.Next(1, 365)),
                CreatedAt = now
            });
        }
        await _context.SaveChangesAsync();
    }
}
