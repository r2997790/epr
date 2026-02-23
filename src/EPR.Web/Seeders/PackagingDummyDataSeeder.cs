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

        // 1. Seed MaterialTaxonomy (raw materials) if the dummy-specific entries don't exist
        if (!await _context.MaterialTaxonomies.AnyAsync(t => t.Code == "PET"))
        {
            var materials = new List<MaterialTaxonomy>
            {
                new() { Level = 1, Code = "PET", DisplayName = "Polyethylene Terephthalate", Name = "PET", SortOrder = 1, IsActive = true, CreatedAt = now },
                new() { Level = 1, Code = "PAP", DisplayName = "Paper & Cardboard", Name = "Paper", SortOrder = 2, IsActive = true, CreatedAt = now },
                new() { Level = 1, Code = "GLS", DisplayName = "Glass", Name = "Glass", SortOrder = 3, IsActive = true, CreatedAt = now },
                new() { Level = 1, Code = "ALU", DisplayName = "Aluminium", Name = "Aluminium", SortOrder = 4, IsActive = true, CreatedAt = now },
                new() { Level = 1, Code = "PP", DisplayName = "Polypropylene", Name = "PP", SortOrder = 5, IsActive = true, CreatedAt = now },
                new() { Level = 1, Code = "HDPE", DisplayName = "High-Density Polyethylene", Name = "HDPE", SortOrder = 6, IsActive = true, CreatedAt = now },
                new() { Level = 1, Code = "PLA", DisplayName = "Polylactic Acid (Compostable)", Name = "PLA", SortOrder = 7, IsActive = true, CreatedAt = now },
                new() { Level = 1, Code = "WOOD", DisplayName = "Softwood Timber", Name = "Wood", SortOrder = 8, IsActive = true, CreatedAt = now },
                new() { Level = 1, Code = "LDPE", DisplayName = "Low-Density Polyethylene Film", Name = "LDPE", SortOrder = 9, IsActive = true, CreatedAt = now },
                new() { Level = 1, Code = "STEEL", DisplayName = "Steel (Nails/Fasteners)", Name = "Steel", SortOrder = 10, IsActive = true, CreatedAt = now }
            };
            _context.MaterialTaxonomies.AddRange(materials);
            await _context.SaveChangesAsync();
        }

        // 2. Seed PackagingLibrary (packaging items) if the dummy-specific entries don't exist
        if (!await _context.PackagingLibraries.AnyAsync(l => l.TaxonomyCode == "PET.BF.BT.500"))
        {
            var pet = await _context.MaterialTaxonomies.FirstOrDefaultAsync(t => t.Code == "PET");
            var pap = await _context.MaterialTaxonomies.FirstOrDefaultAsync(t => t.Code == "PAP");
            var hdpe = await _context.MaterialTaxonomies.FirstOrDefaultAsync(t => t.Code == "HDPE");
            var pp = await _context.MaterialTaxonomies.FirstOrDefaultAsync(t => t.Code == "PP");
            var pla = await _context.MaterialTaxonomies.FirstOrDefaultAsync(t => t.Code == "PLA");
            var gls = await _context.MaterialTaxonomies.FirstOrDefaultAsync(t => t.Code == "GLS");
            var alu = await _context.MaterialTaxonomies.FirstOrDefaultAsync(t => t.Code == "ALU");
            var wood = await _context.MaterialTaxonomies.FirstOrDefaultAsync(t => t.Code == "WOOD");
            var ldpe = await _context.MaterialTaxonomies.FirstOrDefaultAsync(t => t.Code == "LDPE");
            var steel = await _context.MaterialTaxonomies.FirstOrDefaultAsync(t => t.Code == "STEEL");

            var items = new List<PackagingLibrary>
            {
                new() { TaxonomyCode = "PET.BF.BT.500", Name = "PET Bottle 500ml", Weight = 25, MaterialTaxonomyId = pet?.Id, IsActive = true, CreatedAt = now },
                new() { TaxonomyCode = "HDPE.CAP.28", Name = "HDPE Cap 28mm", Weight = 2, MaterialTaxonomyId = hdpe?.Id, IsActive = true, CreatedAt = now },
                new() { TaxonomyCode = "PAP.LBL", Name = "Paper Label", Weight = 1, MaterialTaxonomyId = pap?.Id, IsActive = true, CreatedAt = now },
                new() { TaxonomyCode = "CB.BOX.SML", Name = "Cardboard Box Small", Weight = 50, MaterialTaxonomyId = pap?.Id, IsActive = true, CreatedAt = now },
                new() { TaxonomyCode = "GLS.JAR.250", Name = "Glass Jar 250g", Weight = 180, MaterialTaxonomyId = gls?.Id, IsActive = true, CreatedAt = now },
                new() { TaxonomyCode = "PP.CON.500", Name = "PP Container 500ml", Weight = 30, MaterialTaxonomyId = pp?.Id, IsActive = true, CreatedAt = now },
                new() { TaxonomyCode = "PLA.FILM", Name = "Compostable Film", Weight = 5, MaterialTaxonomyId = pla?.Id, IsActive = true, CreatedAt = now },
                new() { TaxonomyCode = "ALU.TRAY", Name = "Aluminium Tray", Weight = 15, MaterialTaxonomyId = alu?.Id, IsActive = true, CreatedAt = now },
                new() { TaxonomyCode = "WOOD.PLT", Name = "Wood Pallet", Weight = 22000, MaterialTaxonomyId = wood?.Id, IsActive = true, CreatedAt = now },
                new() { TaxonomyCode = "LDPE.WRAP", Name = "Stretch Wrap", Weight = 300, MaterialTaxonomyId = ldpe?.Id, IsActive = true, CreatedAt = now },
                new() { TaxonomyCode = "STEEL.NAIL", Name = "Pallet Nails", Weight = 200, MaterialTaxonomyId = steel?.Id, IsActive = true, CreatedAt = now }
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

        // 4. Seed PackagingGroups if the specific dummy groups don't exist
        if (!await _context.PackagingGroups.AnyAsync(g => g.PackId == "PG-001"))
        {
            var petBottle = await _context.PackagingLibraries.FirstOrDefaultAsync(l => l.TaxonomyCode == "PET.BF.BT.500");
            var hdpeCap = await _context.PackagingLibraries.FirstOrDefaultAsync(l => l.TaxonomyCode == "HDPE.CAP.28");
            var paperLabel = await _context.PackagingLibraries.FirstOrDefaultAsync(l => l.TaxonomyCode == "PAP.LBL");
            var cbBox2 = await _context.PackagingLibraries.FirstOrDefaultAsync(l => l.TaxonomyCode == "CB.BOX.SML");
            var glassJar = await _context.PackagingLibraries.FirstOrDefaultAsync(l => l.TaxonomyCode == "GLS.JAR.250");
            var ppContainer = await _context.PackagingLibraries.FirstOrDefaultAsync(l => l.TaxonomyCode == "PP.CON.500");
            var woodPallet = await _context.PackagingLibraries.FirstOrDefaultAsync(l => l.TaxonomyCode == "WOOD.PLT");
            var stretchWrap = await _context.PackagingLibraries.FirstOrDefaultAsync(l => l.TaxonomyCode == "LDPE.WRAP");
            var palletNails = await _context.PackagingLibraries.FirstOrDefaultAsync(l => l.TaxonomyCode == "STEEL.NAIL");

            // Create tertiary first (no parent)
            var gTertiary = new PackagingGroup
            {
                PackId = "PG-PLT", Name = "Shipping Pallet", PackagingLayer = "Tertiary", Style = "Logistics",
                TotalPackWeight = 22500, IsActive = true, CreatedAt = now
            };
            _context.PackagingGroups.Add(gTertiary);
            await _context.SaveChangesAsync();

            // Create secondary (parent = tertiary)
            var gSecondary = new PackagingGroup
            {
                PackId = "PG-002", Name = "Shipping Carton", PackagingLayer = "Secondary", Style = "Retail",
                TotalPackWeight = 80, IsActive = true, CreatedAt = now,
                ParentPackagingGroupId = gTertiary.Id, QuantityInParent = 40
            };
            _context.PackagingGroups.Add(gSecondary);
            await _context.SaveChangesAsync();

            // Create primaries (parent = secondary)
            var gPrimary = new PackagingGroup
            {
                PackId = "PG-001", Name = "Bottle + Cap + Label", PackagingLayer = "Primary", Style = "Beverage",
                TotalPackWeight = 28, IsActive = true, CreatedAt = now,
                ParentPackagingGroupId = gSecondary.Id, QuantityInParent = 20
            };
            var gPrimary2 = new PackagingGroup
            {
                PackId = "PG-003", Name = "Jar with Lid", PackagingLayer = "Primary", Style = "Food",
                TotalPackWeight = 200, IsActive = true, CreatedAt = now
            };
            _context.PackagingGroups.AddRange(gPrimary, gPrimary2);
            await _context.SaveChangesAsync();

            // Primary items
            if (petBottle != null) _context.PackagingGroupItems.Add(new PackagingGroupItem { PackagingGroupId = gPrimary.Id, PackagingLibraryId = petBottle.Id, SortOrder = 0, CreatedAt = now });
            if (hdpeCap != null) _context.PackagingGroupItems.Add(new PackagingGroupItem { PackagingGroupId = gPrimary.Id, PackagingLibraryId = hdpeCap.Id, SortOrder = 1, CreatedAt = now });
            if (paperLabel != null) _context.PackagingGroupItems.Add(new PackagingGroupItem { PackagingGroupId = gPrimary.Id, PackagingLibraryId = paperLabel.Id, SortOrder = 2, CreatedAt = now });

            // Secondary items
            if (cbBox2 != null) _context.PackagingGroupItems.Add(new PackagingGroupItem { PackagingGroupId = gSecondary.Id, PackagingLibraryId = cbBox2.Id, SortOrder = 0, CreatedAt = now });
            if (paperLabel != null) _context.PackagingGroupItems.Add(new PackagingGroupItem { PackagingGroupId = gSecondary.Id, PackagingLibraryId = paperLabel.Id, SortOrder = 1, CreatedAt = now });

            // Tertiary items
            if (woodPallet != null) _context.PackagingGroupItems.Add(new PackagingGroupItem { PackagingGroupId = gTertiary.Id, PackagingLibraryId = woodPallet.Id, SortOrder = 0, CreatedAt = now });
            if (stretchWrap != null) _context.PackagingGroupItems.Add(new PackagingGroupItem { PackagingGroupId = gTertiary.Id, PackagingLibraryId = stretchWrap.Id, SortOrder = 1, CreatedAt = now });
            if (palletNails != null) _context.PackagingGroupItems.Add(new PackagingGroupItem { PackagingGroupId = gTertiary.Id, PackagingLibraryId = palletNails.Id, SortOrder = 2, CreatedAt = now });

            // Jar with Lid items
            if (glassJar != null) _context.PackagingGroupItems.Add(new PackagingGroupItem { PackagingGroupId = gPrimary2.Id, PackagingLibraryId = glassJar.Id, SortOrder = 0, CreatedAt = now });
            if (ppContainer != null) _context.PackagingGroupItems.Add(new PackagingGroupItem { PackagingGroupId = gPrimary2.Id, PackagingLibraryId = ppContainer.Id, SortOrder = 1, CreatedAt = now });

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
