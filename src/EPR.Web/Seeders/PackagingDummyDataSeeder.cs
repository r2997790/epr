using EPR.Domain.Entities;
using EPR.Data;
using Microsoft.EntityFrameworkCore;

namespace EPR.Web.Seeders;

/// <summary>
/// Seeds dummy raw materials, packaging items, packaging groups and their relationships for testing.
/// Run after PackagingSupplierSeeder (suppliers must exist for supply chain links).
/// </summary>
public class PackagingDummyDataSeeder
{
    private readonly EPRDbContext _context;

    public PackagingDummyDataSeeder(EPRDbContext context)
    {
        _context = context;
    }

    public async Task SeedAsync()
    {
        var now = DateTime.UtcNow;

        // 1. Seed MaterialTaxonomy (raw materials) if empty
        if (!await _context.MaterialTaxonomies.AnyAsync())
        {
            var materials = new List<MaterialTaxonomy>
            {
                new() { Level = 1, Code = "PET", DisplayName = "Polyethylene Terephthalate", Name = "PET", SortOrder = 1, IsActive = true, CreatedAt = now },
                new() { Level = 1, Code = "PAP", DisplayName = "Paper & Cardboard", Name = "Paper", SortOrder = 2, IsActive = true, CreatedAt = now },
                new() { Level = 1, Code = "GLS", DisplayName = "Glass", Name = "Glass", SortOrder = 3, IsActive = true, CreatedAt = now },
                new() { Level = 1, Code = "ALU", DisplayName = "Aluminium", Name = "Aluminium", SortOrder = 4, IsActive = true, CreatedAt = now },
                new() { Level = 1, Code = "PP", DisplayName = "Polypropylene", Name = "PP", SortOrder = 5, IsActive = true, CreatedAt = now },
                new() { Level = 1, Code = "HDPE", DisplayName = "High-Density Polyethylene", Name = "HDPE", SortOrder = 6, IsActive = true, CreatedAt = now },
                new() { Level = 1, Code = "PLA", DisplayName = "Polylactic Acid (Compostable)", Name = "PLA", SortOrder = 7, IsActive = true, CreatedAt = now }
            };
            _context.MaterialTaxonomies.AddRange(materials);
            await _context.SaveChangesAsync();
        }

        // 2. Seed PackagingLibrary (packaging items) if empty
        if (!await _context.PackagingLibraries.AnyAsync())
        {
            var pet = await _context.MaterialTaxonomies.FirstOrDefaultAsync(t => t.Code == "PET");
            var pap = await _context.MaterialTaxonomies.FirstOrDefaultAsync(t => t.Code == "PAP");
            var hdpe = await _context.MaterialTaxonomies.FirstOrDefaultAsync(t => t.Code == "HDPE");
            var pp = await _context.MaterialTaxonomies.FirstOrDefaultAsync(t => t.Code == "PP");
            var pla = await _context.MaterialTaxonomies.FirstOrDefaultAsync(t => t.Code == "PLA");
            var gls = await _context.MaterialTaxonomies.FirstOrDefaultAsync(t => t.Code == "GLS");
            var alu = await _context.MaterialTaxonomies.FirstOrDefaultAsync(t => t.Code == "ALU");

            var items = new List<PackagingLibrary>
            {
                new() { TaxonomyCode = "PET.BF.BT.500", Name = "PET Bottle 500ml", Weight = 25, MaterialTaxonomyId = pet?.Id, IsActive = true, CreatedAt = now },
                new() { TaxonomyCode = "HDPE.CAP.28", Name = "HDPE Cap 28mm", Weight = 2, MaterialTaxonomyId = hdpe?.Id, IsActive = true, CreatedAt = now },
                new() { TaxonomyCode = "PAP.LBL", Name = "Paper Label", Weight = 1, MaterialTaxonomyId = pap?.Id, IsActive = true, CreatedAt = now },
                new() { TaxonomyCode = "CB.BOX.SML", Name = "Cardboard Box Small", Weight = 50, MaterialTaxonomyId = pap?.Id, IsActive = true, CreatedAt = now },
                new() { TaxonomyCode = "GLS.JAR.250", Name = "Glass Jar 250g", Weight = 180, MaterialTaxonomyId = gls?.Id, IsActive = true, CreatedAt = now },
                new() { TaxonomyCode = "PP.CON.500", Name = "PP Container 500ml", Weight = 30, MaterialTaxonomyId = pp?.Id, IsActive = true, CreatedAt = now },
                new() { TaxonomyCode = "PLA.FILM", Name = "Compostable Film", Weight = 5, MaterialTaxonomyId = pla?.Id, IsActive = true, CreatedAt = now },
                new() { TaxonomyCode = "ALU.TRAY", Name = "Aluminium Tray", Weight = 15, MaterialTaxonomyId = alu?.Id, IsActive = true, CreatedAt = now }
            };
            _context.PackagingLibraries.AddRange(items);
            await _context.SaveChangesAsync();

            // Add PackagingLibraryMaterials for each item (many-to-many raw materials)
            foreach (var item in items.Where(i => i.MaterialTaxonomyId != null))
            {
                _context.PackagingLibraryMaterials.Add(new PackagingLibraryMaterial
                {
                    PackagingLibraryId = item.Id,
                    MaterialTaxonomyId = item.MaterialTaxonomyId!.Value,
                    SortOrder = 0,
                    CreatedAt = now
                });
            }
            await _context.SaveChangesAsync();
        }

        // 4. Seed PackagingGroups if empty
        if (!await _context.PackagingGroups.AnyAsync())
        {
            var petBottle = await _context.PackagingLibraries.FirstOrDefaultAsync(l => l.TaxonomyCode == "PET.BF.BT.500");
            var hdpeCap = await _context.PackagingLibraries.FirstOrDefaultAsync(l => l.TaxonomyCode == "HDPE.CAP.28");
            var paperLabel = await _context.PackagingLibraries.FirstOrDefaultAsync(l => l.TaxonomyCode == "PAP.LBL");
            var cbBox2 = await _context.PackagingLibraries.FirstOrDefaultAsync(l => l.TaxonomyCode == "CB.BOX.SML");
            var glassJar = await _context.PackagingLibraries.FirstOrDefaultAsync(l => l.TaxonomyCode == "GLS.JAR.250");
            var ppContainer = await _context.PackagingLibraries.FirstOrDefaultAsync(l => l.TaxonomyCode == "PP.CON.500");

            var groups = new List<PackagingGroup>
            {
                new()
                {
                    PackId = "PG-001", Name = "Bottle + Cap + Label", PackagingLayer = "Primary", Style = "Beverage",
                    TotalPackWeight = 28, IsActive = true, CreatedAt = now
                },
                new()
                {
                    PackId = "PG-002", Name = "Gift Box Set", PackagingLayer = "Secondary", Style = "Retail",
                    TotalPackWeight = 80, IsActive = true, CreatedAt = now
                },
                new()
                {
                    PackId = "PG-003", Name = "Jar with Lid", PackagingLayer = "Primary", Style = "Food",
                    TotalPackWeight = 200, IsActive = true, CreatedAt = now
                }
            };
            _context.PackagingGroups.AddRange(groups);
            await _context.SaveChangesAsync();

            // Add group items
            var g1 = groups[0];
            var g2 = groups[1];
            var g3 = groups[2];

            if (petBottle != null) _context.PackagingGroupItems.Add(new PackagingGroupItem { PackagingGroupId = g1.Id, PackagingLibraryId = petBottle.Id, SortOrder = 0, CreatedAt = now });
            if (hdpeCap != null) _context.PackagingGroupItems.Add(new PackagingGroupItem { PackagingGroupId = g1.Id, PackagingLibraryId = hdpeCap.Id, SortOrder = 1, CreatedAt = now });
            if (paperLabel != null) _context.PackagingGroupItems.Add(new PackagingGroupItem { PackagingGroupId = g1.Id, PackagingLibraryId = paperLabel.Id, SortOrder = 2, CreatedAt = now });

            if (cbBox2 != null) _context.PackagingGroupItems.Add(new PackagingGroupItem { PackagingGroupId = g2.Id, PackagingLibraryId = cbBox2.Id, SortOrder = 0, CreatedAt = now });
            if (petBottle != null) _context.PackagingGroupItems.Add(new PackagingGroupItem { PackagingGroupId = g2.Id, PackagingLibraryId = petBottle.Id, SortOrder = 1, CreatedAt = now });

            if (glassJar != null) _context.PackagingGroupItems.Add(new PackagingGroupItem { PackagingGroupId = g3.Id, PackagingLibraryId = glassJar.Id, SortOrder = 0, CreatedAt = now });
            if (ppContainer != null) _context.PackagingGroupItems.Add(new PackagingGroupItem { PackagingGroupId = g3.Id, PackagingLibraryId = ppContainer.Id, SortOrder = 1, CreatedAt = now });

            await _context.SaveChangesAsync();
        }

        // 5. Migrate legacy MaterialTaxonomyId to PackagingLibraryMaterials if not already done
        var libsNeedingMigration = await _context.PackagingLibraries
            .Where(l => l.MaterialTaxonomyId != null)
            .Select(l => new { l.Id, l.MaterialTaxonomyId })
            .ToListAsync();
        foreach (var lib in libsNeedingMigration)
        {
            var exists = await _context.PackagingLibraryMaterials
                .AnyAsync(plm => plm.PackagingLibraryId == lib.Id && plm.MaterialTaxonomyId == lib.MaterialTaxonomyId!.Value);
            if (!exists)
            {
                _context.PackagingLibraryMaterials.Add(new PackagingLibraryMaterial
                {
                    PackagingLibraryId = lib.Id,
                    MaterialTaxonomyId = lib.MaterialTaxonomyId!.Value,
                    SortOrder = 0,
                    CreatedAt = now
                });
            }
        }
        await _context.SaveChangesAsync();

        // 6. Link supplier products to packaging items and raw materials (supply chain)
        var supplierProducts = await _context.PackagingSupplierProducts
            .Include(p => p.PackagingSupplier)
            .ToListAsync();
        if (supplierProducts.Any())
        {
            var petBottle = await _context.PackagingLibraries.FirstOrDefaultAsync(l => l.TaxonomyCode == "PET.BF.BT.500");
            var rpetProduct = supplierProducts.FirstOrDefault(p => p.ProductCode == "RPET-500");
            var petMat = await _context.MaterialTaxonomies.FirstOrDefaultAsync(t => t.Code == "PET");
            var papMat = await _context.MaterialTaxonomies.FirstOrDefaultAsync(t => t.Code == "PAP");
            var paperLabelsProduct = supplierProducts.FirstOrDefault(p => p.ProductCode == "PL-100");
            var cardboardProduct = supplierProducts.FirstOrDefault(p => p.ProductCode == "CB-SLEEVE");

            if (petBottle != null && rpetProduct != null)
            {
                var exists = await _context.PackagingLibrarySupplierProducts
                    .AnyAsync(plsp => plsp.PackagingLibraryId == petBottle.Id && plsp.PackagingSupplierProductId == rpetProduct.Id);
                if (!exists)
                {
                    _context.PackagingLibrarySupplierProducts.Add(new PackagingLibrarySupplierProduct
                    { PackagingLibraryId = petBottle.Id, PackagingSupplierProductId = rpetProduct.Id, CreatedAt = now });
                }
            }
            if (petMat != null && rpetProduct != null)
            {
                var exists = await _context.MaterialTaxonomySupplierProducts
                    .AnyAsync(msp => msp.MaterialTaxonomyId == petMat.Id && msp.PackagingSupplierProductId == rpetProduct.Id);
                if (!exists)
                {
                    _context.MaterialTaxonomySupplierProducts.Add(new MaterialTaxonomySupplierProduct
                    { MaterialTaxonomyId = petMat.Id, PackagingSupplierProductId = rpetProduct.Id, CreatedAt = now });
                }
            }
            if (papMat != null && paperLabelsProduct != null)
            {
                var exists = await _context.MaterialTaxonomySupplierProducts
                    .AnyAsync(msp => msp.MaterialTaxonomyId == papMat.Id && msp.PackagingSupplierProductId == paperLabelsProduct.Id);
                if (!exists)
                {
                    _context.MaterialTaxonomySupplierProducts.Add(new MaterialTaxonomySupplierProduct
                    { MaterialTaxonomyId = papMat.Id, PackagingSupplierProductId = paperLabelsProduct.Id, CreatedAt = now });
                }
            }
            if (papMat != null && cardboardProduct != null)
            {
                var exists = await _context.MaterialTaxonomySupplierProducts
                    .AnyAsync(msp => msp.MaterialTaxonomyId == papMat.Id && msp.PackagingSupplierProductId == cardboardProduct.Id);
                if (!exists)
                {
                    _context.MaterialTaxonomySupplierProducts.Add(new MaterialTaxonomySupplierProduct
                    { MaterialTaxonomyId = papMat.Id, PackagingSupplierProductId = cardboardProduct.Id, CreatedAt = now });
                }
            }
            await _context.SaveChangesAsync();
        }
    }
}
