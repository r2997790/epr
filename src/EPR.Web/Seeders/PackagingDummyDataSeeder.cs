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

        // 1. Seed MaterialTaxonomy (raw materials) — ensure each individually to avoid bulk-insert failures
        async Task<MaterialTaxonomy> EnsureMaterial(string code, string displayName, string name, int sortOrder)
        {
            var existing = await _context.MaterialTaxonomies.FirstOrDefaultAsync(t => t.Code == code);
            if (existing != null) return existing;
            var m = new MaterialTaxonomy
            {
                Level = 1, Code = code, DisplayName = displayName,
                Name = name, SortOrder = sortOrder, IsActive = true, CreatedAt = now
            };
            _context.MaterialTaxonomies.Add(m);
            await _context.SaveChangesAsync();
            return m;
        }

        var pet   = await EnsureMaterial("PET",   "Polyethylene Terephthalate",    "PET",       1);
        var pap   = await EnsureMaterial("PAP",   "Paper & Cardboard",             "Paper",     2);
        var gls   = await EnsureMaterial("GLS",   "Glass",                         "Glass",     3);
        var alu   = await EnsureMaterial("ALU",   "Aluminium",                     "Aluminium", 4);
        var pp    = await EnsureMaterial("PP",    "Polypropylene",                 "PP",        5);
        var hdpe  = await EnsureMaterial("HDPE",  "High-Density Polyethylene",     "HDPE",      6);
        var pla   = await EnsureMaterial("PLA",   "Polylactic Acid (Compostable)", "PLA",       7);
        var wood  = await EnsureMaterial("WOOD",  "Softwood Timber",               "Wood",      8);
        var ldpe  = await EnsureMaterial("LDPE",  "Low-Density Polyethylene Film", "LDPE",      9);
        var steel = await EnsureMaterial("STEEL", "Steel (Nails/Fasteners)",       "Steel",    10);

        // 2. Seed PackagingLibrary (packaging items) — ensure each individually
        async Task<PackagingLibrary> EnsureLibrary(string taxonomyCode, string name, decimal weight, int? materialTaxonomyId)
        {
            var existing = await _context.PackagingLibraries.FirstOrDefaultAsync(l => l.TaxonomyCode == taxonomyCode);
            if (existing != null) return existing;
            var lib = new PackagingLibrary
            {
                TaxonomyCode = taxonomyCode, Name = name, Weight = weight,
                MaterialTaxonomyId = materialTaxonomyId, IsActive = true, CreatedAt = now
            };
            _context.PackagingLibraries.Add(lib);
            await _context.SaveChangesAsync();

            if (materialTaxonomyId != null)
            {
                _context.PackagingLibraryMaterials.Add(new PackagingLibraryMaterial
                {
                    PackagingLibraryId = lib.Id,
                    MaterialTaxonomyId = materialTaxonomyId.Value,
                    SortOrder = 0,
                    CreatedAt = now
                });
                await _context.SaveChangesAsync();
            }
            return lib;
        }

        await EnsureLibrary("PET.BF.BT.500", "PET Bottle 500ml",   25,    pet.Id);
        await EnsureLibrary("HDPE.CAP.28",    "HDPE Cap 28mm",      2,     hdpe.Id);
        await EnsureLibrary("PAP.LBL",        "Paper Label",        1,     pap.Id);
        await EnsureLibrary("CB.BOX.SML",     "Cardboard Box Small", 50,   pap.Id);
        await EnsureLibrary("GLS.JAR.250",    "Glass Jar 250g",     180,   gls.Id);
        await EnsureLibrary("PP.CON.500",     "PP Container 500ml", 30,    pp.Id);
        await EnsureLibrary("PLA.FILM",       "Compostable Film",   5,     pla.Id);
        await EnsureLibrary("ALU.TRAY",       "Aluminium Tray",     15,    alu.Id);
        await EnsureLibrary("WOOD.PLT",       "Wood Pallet",        22000, wood.Id);
        await EnsureLibrary("LDPE.WRAP",      "Stretch Wrap",       300,   ldpe.Id);
        await EnsureLibrary("STEEL.NAIL",     "Pallet Nails",       200,   steel.Id);

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
