using EPR.Domain.Entities;
using EPR.Data;
using Microsoft.EntityFrameworkCore;

namespace EPR.Web.Seeders;

/// <summary>
/// Seeds Pharmaceuticals dataset: 20 products with packaging, raw materials, suppliers,
/// distribution, and ASN data. Australian addresses (80%), 20% from outside Australia.
/// </summary>
public class PharmaceuticalsDatasetSeeder
{
    private const string DatasetKey = "Pharmaceuticals";
    private readonly EPRDbContext _context;

    public PharmaceuticalsDatasetSeeder(EPRDbContext context) => _context = context;

    public async Task SeedAsync()
    {
        var existingProductCount = await _context.Products.CountAsync(p => p.DatasetKey == DatasetKey);
        var existingAsnCount = await _context.AsnShipments.CountAsync(s => s.DatasetKey == DatasetKey);

        if (existingProductCount > 0 && existingAsnCount > 0) { await UpdateProductImagesIfMissing(); return; }
        if (existingProductCount > 0 && existingAsnCount == 0) { await EnsureAsnsAsync(); return; }

        var plasticTax = await EnsureMaterialTaxonomy("PLASTIC", "Plastics", 1);
        var paperTax = await EnsureMaterialTaxonomy("PAPER", "Paper & Cardboard", 1);
        var foilTax = await EnsureMaterialTaxonomy("FOIL", "Aluminium Foil", 1);

        var supplierBlister = await EnsureSupplier("PharmaPack Sydney", "42 Pharma Lane", "Sydney", "Australia", "sales@pharmapack.com.au");
        var supplierBottles = await EnsureSupplier("PharmaBottle Melbourne", "18 Medicine Rd", "Melbourne", "Australia", "orders@pharmabottle.com.au");
        var supplierLabels = await EnsureSupplier("PharmaLabels Brisbane", "7 Script Street", "Brisbane", "Australia", "info@pharmalabels.com.au");
        var supplierBoxes = await EnsureSupplier("PharmaBox Perth", "25 Clinical Way", "Perth", "Australia", "sales@pharmabox.com.au");
        var supplierNz = await EnsureSupplier("KiwiPharma Auckland", "10 Export Drive", "Auckland", "New Zealand", "orders@kiwipharma.co.nz");

        var blister = await EnsurePackagingLibrary("Blister pack 10", "PHARM-BLISTER-001", 8m, foilTax.Id, DatasetKey);
        var bottle = await EnsurePackagingLibrary("Medicine bottle 100ml", "PHARM-BOTTLE-001", 35m, plasticTax.Id, DatasetKey);
        var carton = await EnsurePackagingLibrary("Medicine carton", "PHARM-CARTON-001", 45m, paperTax.Id, DatasetKey);
        var label = await EnsurePackagingLibrary("Product label", "PHARM-LABEL-001", 2m, paperTax.Id, DatasetKey);
        var cap = await EnsurePackagingLibrary("Child-resistant cap", "PHARM-CAP-001", 6m, plasticTax.Id, DatasetKey);

        await LinkPackagingMaterial(blister.Id, foilTax.Id);
        await LinkPackagingMaterial(bottle.Id, plasticTax.Id);
        await LinkPackagingMaterial(carton.Id, paperTax.Id);
        await LinkPackagingMaterial(label.Id, paperTax.Id);
        await LinkPackagingMaterial(cap.Id, plasticTax.Id);

        var woodTax = await EnsureMaterialTaxonomy("WOOD", "Softwood Timber", 1);
        var ldpeTax = await EnsureMaterialTaxonomy("LDPE", "Low-Density Polyethylene Film", 1);
        var palletLib = await EnsurePackagingLibrary("Wood Pallet", "PHARM-PLT-001", 22000m, woodTax.Id, DatasetKey);
        var wrapLib = await EnsurePackagingLibrary("Stretch Wrap", "PHARM-WRAP-PLT-001", 300m, ldpeTax.Id, DatasetKey);
        await LinkPackagingMaterial(palletLib.Id, woodTax.Id);
        await LinkPackagingMaterial(wrapLib.Id, ldpeTax.Id);

        var spBlister = await EnsureSupplierProduct(supplierBlister.Id, "Blister Pack 10", "PHARM-BLISTER-001");
        var spBottle = await EnsureSupplierProduct(supplierBottles.Id, "Medicine Bottle 100ml", "PHARM-BOTTLE-001");
        var spCarton = await EnsureSupplierProduct(supplierBoxes.Id, "Medicine Carton", "PHARM-CARTON-001");
        var spLabel = await EnsureSupplierProduct(supplierLabels.Id, "Pharma Label", "PHARM-LABEL-001");
        var spCap = await EnsureSupplierProduct(supplierBottles.Id, "Child-Resistant Cap", "PHARM-CAP-001");

        await LinkPackagingSupplier(blister.Id, spBlister.Id, true);
        await LinkPackagingSupplier(bottle.Id, spBottle.Id, true);
        await LinkPackagingSupplier(carton.Id, spCarton.Id, true);
        await LinkPackagingSupplier(label.Id, spLabel.Id, true);
        await LinkPackagingSupplier(cap.Id, spCap.Id, true);

        var groupPallet = await EnsurePackagingGroup("PHARM-PLT-001", "Pharmaceuticals Pallet", "Tertiary", 22300m, DatasetKey);
        await AddGroupItem(groupPallet.Id, palletLib.Id, 0);
        await AddGroupItem(groupPallet.Id, wrapLib.Id, 1);

        var groupShipping = await EnsurePackagingGroup("PHARM-SHIP-001", "Pharmaceuticals Shipping Pack", "Secondary", 96m, DatasetKey, groupPallet.Id, 40);
        await AddGroupItem(groupShipping.Id, carton.Id, 0);

        var groupProduct = await EnsurePackagingGroup("PHARM-PROD-001", "Pharmaceuticals Product Pack", "Primary", 96m, DatasetKey, groupShipping.Id, 24);
        await AddGroupItem(groupProduct.Id, blister.Id, 0);
        await AddGroupItem(groupProduct.Id, bottle.Id, 1);
        await AddGroupItem(groupProduct.Id, label.Id, 2);
        await AddGroupItem(groupProduct.Id, cap.Id, 3);

        var prmPlastic = await EnsurePackagingRawMaterial("Plastics", "Plastic materials");
        var prmPaper = await EnsurePackagingRawMaterial("Paper & Cardboard", "Paper and cardboard materials");
        var prmFoil = await EnsurePackagingRawMaterial("Aluminium Foil", "Aluminium foil materials");
        var materialTaxToPrm = new Dictionary<int, int> { { plasticTax.Id, prmPlastic.Id }, { paperTax.Id, prmPaper.Id }, { foilTax.Id, prmFoil.Id } };

        var ptBlister = await EnsurePackagingTypeFromLibrary(blister, materialTaxToPrm);
        var ptBottle = await EnsurePackagingTypeFromLibrary(bottle, materialTaxToPrm);
        var ptCarton = await EnsurePackagingTypeFromLibrary(carton, materialTaxToPrm);
        var ptLabel = await EnsurePackagingTypeFromLibrary(label, materialTaxToPrm);
        var ptCap = await EnsurePackagingTypeFromLibrary(cap, materialTaxToPrm);

        var shippingLibs = new[] { carton };
        var productLibs = new[] { blister, bottle, label, cap };
        var libToPt = new Dictionary<int, PackagingType> { { blister.Id, ptBlister }, { bottle.Id, ptBottle }, { carton.Id, ptCarton }, { label.Id, ptLabel }, { cap.Id, ptCap } };
        var puShipping = await EnsurePackagingUnitFromGroup(groupShipping, shippingLibs, libToPt);
        var puProduct = await EnsurePackagingUnitFromGroup(groupProduct, productLibs, libToPt);

        var auJurisdiction = await EnsureJurisdiction("AU", "Australia", "AU");
        var nzJurisdiction = await EnsureJurisdiction("NZ", "New Zealand", "NZ");
        var sydneyGeo = await EnsureGeography("SYD", "Sydney", auJurisdiction.Id);
        var melbourneGeo = await EnsureGeography("MEL", "Melbourne", auJurisdiction.Id);
        var brisbaneGeo = await EnsureGeography("BNE", "Brisbane", auJurisdiction.Id);
        var aucklandGeo = await EnsureGeography("AKL", "Auckland", nzJurisdiction.Id);

        var products = new[]
        {
            ("PHARM-001", "Paracetamol 500mg 24pk", "MediCare", "50609234567890", "https://picsum.photos/seed/pharm-paracetamol/200/200"),
            ("PHARM-002", "Ibuprofen 200mg 24pk", "MediCare", "50609234567891", "https://picsum.photos/seed/pharm-ibuprofen/200/200"),
            ("PHARM-003", "Vitamin C 1000mg 60pk", "MediCare", "50609234567892", "https://picsum.photos/seed/pharm-vitc/200/200"),
            ("PHARM-004", "Multivitamin Daily 30pk", "MediCare", "50609234567893", "https://picsum.photos/seed/pharm-multivit/200/200"),
            ("PHARM-005", "Cough Syrup 100ml", "MediCare", "50609234567894", "https://picsum.photos/seed/pharm-cough/200/200"),
            ("PHARM-006", "Antihistamine 10mg 24pk", "MediCare", "50609234567895", "https://picsum.photos/seed/pharm-antihist/200/200"),
            ("PHARM-007", "Vitamin D 1000IU 60pk", "MediCare", "50609234567896", "https://picsum.photos/seed/pharm-vitd/200/200"),
            ("PHARM-008", "Fish Oil 1000mg 60pk", "MediCare", "50609234567897", "https://picsum.photos/seed/pharm-fishoil/200/200"),
            ("PHARM-009", "Calcium 600mg 60pk", "MediCare", "50609234567898", "https://picsum.photos/seed/pharm-calcium/200/200"),
            ("PHARM-010", "Magnesium 250mg 60pk", "MediCare", "50609234567899", "https://picsum.photos/seed/pharm-magnesium/200/200"),
            ("PHARM-011", "Zinc 50mg 30pk", "MediCare", "50609234567900", "https://picsum.photos/seed/pharm-zinc/200/200"),
            ("PHARM-012", "Probiotic 10pk", "MediCare", "50609234567901", "https://picsum.photos/seed/pharm-probiotic/200/200"),
            ("PHARM-013", "Iron 25mg 30pk", "MediCare", "50609234567902", "https://picsum.photos/seed/pharm-iron/200/200"),
            ("PHARM-014", "B12 1000mcg 60pk", "MediCare", "50609234567903", "https://picsum.photos/seed/pharm-b12/200/200"),
            ("PHARM-015", "Echinacea 60pk", "MediCare", "50609234567904", "https://picsum.photos/seed/pharm-echinacea/200/200"),
            ("PHARM-016", "Glucosamine 1500mg 60pk", "MediCare", "50609234567905", "https://picsum.photos/seed/pharm-glucosamine/200/200"),
            ("PHARM-017", "CoQ10 100mg 30pk", "MediCare", "50609234567906", "https://picsum.photos/seed/pharm-coq10/200/200"),
            ("PHARM-018", "Melatonin 3mg 60pk", "MediCare", "50609234567907", "https://picsum.photos/seed/pharm-melatonin/200/200"),
            ("PHARM-019", "Osteoporosis 1000mg 60pk", "MediCare", "50609234567908", "https://picsum.photos/seed/pharm-osteo/200/200"),
            ("PHARM-020", "Prenatal Vitamin 30pk", "MediCare", "50609234567909", "https://picsum.photos/seed/pharm-prenatal/200/200"),
        };

        var supplierProductIds = new[] { spBlister.Id, spBottle.Id, spCarton.Id, spLabel.Id, spCap.Id };
        var productEntities = new List<Product>();
        for (var idx = 0; idx < products.Length; idx++)
        {
            var (sku, name, brand, gtin, imageUrl) = products[idx];
            var p = await EnsureProduct(sku, name, brand, gtin, DatasetKey, imageUrl);
            productEntities.Add(p);
            await EnsureProductForm(p.Id, gtin, name, brand, "Pharmaceuticals", "OTC & Supplements");
            await EnsureProductPackaging(p.Id, puShipping.Id);
            await EnsureProductPackaging(p.Id, puProduct.Id);
            await EnsureProductPackagingSupplierProduct(p.Id, supplierProductIds[idx % supplierProductIds.Length]);
        }

        var rnd = new Random(50);
        var geos = new[] { (sydneyGeo, "Sydney", auJurisdiction.Id, "AU"), (melbourneGeo, "Melbourne", auJurisdiction.Id, "AU"), (brisbaneGeo, "Brisbane", auJurisdiction.Id, "AU"), (aucklandGeo, "Auckland", nzJurisdiction.Id, "NZ") };
        foreach (var p in productEntities)
        {
            var (geo, cityName, jurId, country) = geos[rnd.Next(geos.Length)];
            await EnsureDistribution(p.Id, puShipping.Id, rnd.Next(40, 350), "Pharmacy", cityName, country, geo.Id, jurId);
        }

        var baseDate = DateTime.UtcNow.AddDays(-14);
        var receivers = new[] { ("5061987654351", "PharmaMart Sydney", "Sydney", "AU"), ("5061987654352", "PharmaHub Melbourne", "Melbourne", "AU"), ("5061987654353", "KiwiPharma Auckland", "Auckland", "NZ") };
        var shippers = new[] { ("5061987654354", "PharmaPack Sydney", "Sydney", "AU"), ("5061987654355", "PharmaBottle Melbourne", "Melbourne", "AU"), ("5061987654356", "KiwiPharma Auckland", "Auckland", "NZ") };
        int lineNum = 1;
        for (int i = 0; i < 12; i++)
        {
            var (recGln, recName, recCity, recCountry) = receivers[i % receivers.Length];
            var (shipGln, shipName, shipCity, shipCountry) = shippers[i % shippers.Length];
            var ship = await EnsureAsnShipment($"ASN-PHARM-{1000 + i}", shipGln, shipName, recGln, recName, baseDate.AddDays(i * 2), DatasetKey, shipCity, shipCountry);
            for (int p = 0; p < 1 + (i % 3); p++)
            {
                var pallet = await EnsureAsnPallet(ship.Id, $"3579923456789012{(i * 3 + p):D2}", recName, recCity, recCountry, p + 1, recGln);
                for (int li = 0; li < 2 + (i + p) % 4; li++)
                {
                    var prod = productEntities[(i * 3 + p * 2 + li) % productEntities.Count];
                    await EnsureAsnLineItem(pallet.Id, prod.Gtin!, prod.Name, 12 + (i + li) % 24, lineNum++);
                }
            }
        }

        await _context.SaveChangesAsync();
        Console.WriteLine($"✓ Pharmaceuticals dataset seeded: 20 products, packaging, distribution, ASNs (AU/NZ)");
    }

    private async Task EnsureAsnsAsync()
    {
        var productEntities = await _context.Products.Where(p => p.DatasetKey == DatasetKey && p.Gtin != null).OrderBy(p => p.Sku).ToListAsync();
        if (productEntities.Count == 0) return;
        var baseDate = DateTime.UtcNow.AddDays(-14);
        var receivers = new[] { ("5061987654351", "PharmaMart Sydney", "Sydney", "AU"), ("5061987654352", "PharmaHub Melbourne", "Melbourne", "AU"), ("5061987654353", "KiwiPharma Auckland", "Auckland", "NZ") };
        var shippers = new[] { ("5061987654354", "PharmaPack Sydney", "Sydney", "AU"), ("5061987654355", "PharmaBottle Melbourne", "Melbourne", "AU"), ("5061987654356", "KiwiPharma Auckland", "Auckland", "NZ") };
        int lineNum = 1;
        for (int i = 0; i < 12; i++)
        {
            var (recGln, recName, recCity, recCountry) = receivers[i % receivers.Length];
            var (shipGln, shipName, shipCity, shipCountry) = shippers[i % shippers.Length];
            var ship = await EnsureAsnShipment($"ASN-PHARM-{1000 + i}", shipGln, shipName, recGln, recName, baseDate.AddDays(i * 2), DatasetKey, shipCity, shipCountry);
            for (int p = 0; p < 1 + (i % 3); p++)
            {
                var pallet = await EnsureAsnPallet(ship.Id, $"3579923456789012{(i * 3 + p):D2}", recName, recCity, recCountry, p + 1, recGln);
                for (int li = 0; li < 2 + (i + p) % 4; li++)
                {
                    var prod = productEntities[(i * 3 + p * 2 + li) % productEntities.Count];
                    await EnsureAsnLineItem(pallet.Id, prod.Gtin!, prod.Name, 12 + (i + li) % 24, lineNum++);
                }
            }
        }
        await _context.SaveChangesAsync();
    }

    private async Task UpdateProductImagesIfMissing()
    {
        var seeds = new[] { "pharm-paracetamol", "pharm-ibuprofen", "pharm-vitc", "pharm-multivit", "pharm-cough", "pharm-antihist", "pharm-vitd", "pharm-fishoil", "pharm-calcium", "pharm-magnesium", "pharm-zinc", "pharm-probiotic", "pharm-iron", "pharm-b12", "pharm-echinacea", "pharm-glucosamine", "pharm-coq10", "pharm-melatonin", "pharm-osteo", "pharm-prenatal" };
        var products = await _context.Products.Where(p => p.DatasetKey == DatasetKey && (p.ImageUrl == null || p.ImageUrl == "")).ToListAsync();
        foreach (var p in products)
        {
            if (p.Sku != null && p.Sku.StartsWith("PHARM-") && int.TryParse(p.Sku.AsSpan(6), out var n) && n >= 1 && n <= 20)
                p.ImageUrl = $"https://picsum.photos/seed/{seeds[n - 1]}/200/200";
        }
        if (products.Count > 0) await _context.SaveChangesAsync();
    }

    private async Task<MaterialTaxonomy> EnsureMaterialTaxonomy(string code, string displayName, int level)
    {
        var t = await _context.MaterialTaxonomies.FirstOrDefaultAsync(m => m.Code == code);
        if (t == null) { t = new MaterialTaxonomy { Code = code, DisplayName = displayName, Level = level, IsActive = true, SortOrder = 0 }; _context.MaterialTaxonomies.Add(t); await _context.SaveChangesAsync(); }
        return t;
    }

    private async Task<PackagingSupplier> EnsureSupplier(string name, string address, string city, string country, string email)
    {
        var s = await _context.PackagingSuppliers.FirstOrDefaultAsync(x => x.Name == name);
        if (s == null) { s = new PackagingSupplier { Name = name, Address = address, City = city, Country = country, Email = email, IsActive = true }; _context.PackagingSuppliers.Add(s); await _context.SaveChangesAsync(); }
        return s;
    }

    private async Task<PackagingLibrary> EnsurePackagingLibrary(string name, string taxonomyCode, decimal weight, int materialTaxonomyId, string datasetKey)
    {
        var lib = new PackagingLibrary { Name = name, TaxonomyCode = taxonomyCode, Weight = weight, MaterialTaxonomyId = materialTaxonomyId, DatasetKey = datasetKey, IsActive = true };
        _context.PackagingLibraries.Add(lib);
        await _context.SaveChangesAsync();
        return lib;
    }

    private async Task LinkPackagingMaterial(int packagingLibraryId, int materialTaxonomyId)
    {
        if (await _context.PackagingLibraryMaterials.AnyAsync(plm => plm.PackagingLibraryId == packagingLibraryId && plm.MaterialTaxonomyId == materialTaxonomyId)) return;
        _context.PackagingLibraryMaterials.Add(new PackagingLibraryMaterial { PackagingLibraryId = packagingLibraryId, MaterialTaxonomyId = materialTaxonomyId, SortOrder = 0 });
        await _context.SaveChangesAsync();
    }

    private async Task<PackagingSupplierProduct> EnsureSupplierProduct(int supplierId, string name, string productCode)
    {
        var sp = await _context.PackagingSupplierProducts.FirstOrDefaultAsync(x => x.PackagingSupplierId == supplierId && x.ProductCode == productCode);
        if (sp == null) { sp = new PackagingSupplierProduct { PackagingSupplierId = supplierId, Name = name, ProductCode = productCode, TaxonomyCode = productCode }; _context.PackagingSupplierProducts.Add(sp); await _context.SaveChangesAsync(); }
        return sp;
    }

    private async Task LinkPackagingSupplier(int packagingLibraryId, int supplierProductId, bool isPrimary)
    {
        if (await _context.PackagingLibrarySupplierProducts.AnyAsync(plsp => plsp.PackagingLibraryId == packagingLibraryId && plsp.PackagingSupplierProductId == supplierProductId)) return;
        _context.PackagingLibrarySupplierProducts.Add(new PackagingLibrarySupplierProduct { PackagingLibraryId = packagingLibraryId, PackagingSupplierProductId = supplierProductId, IsPrimary = isPrimary });
        await _context.SaveChangesAsync();
    }

    private async Task<PackagingGroup> EnsurePackagingGroup(string packId, string name, string layer, decimal totalWeight, string datasetKey, int? parentGroupId = null, int? quantityInParent = null)
    {
        var g = new PackagingGroup { PackId = packId, Name = name, PackagingLayer = layer, TotalPackWeight = totalWeight, DatasetKey = datasetKey, IsActive = true, ParentPackagingGroupId = parentGroupId, QuantityInParent = quantityInParent };
        _context.PackagingGroups.Add(g);
        await _context.SaveChangesAsync();
        return g;
    }

    private async Task AddGroupItem(int groupId, int libraryId, int sortOrder)
    {
        if (await _context.PackagingGroupItems.AnyAsync(gi => gi.PackagingGroupId == groupId && gi.PackagingLibraryId == libraryId)) return;
        _context.PackagingGroupItems.Add(new PackagingGroupItem { PackagingGroupId = groupId, PackagingLibraryId = libraryId, SortOrder = sortOrder });
        await _context.SaveChangesAsync();
    }

    private async Task<PackagingRawMaterial> EnsurePackagingRawMaterial(string name, string? description)
    {
        var prm = await _context.PackagingRawMaterials.FirstOrDefaultAsync(m => m.Name == name);
        if (prm == null) { prm = new PackagingRawMaterial { Name = name, Description = description }; _context.PackagingRawMaterials.Add(prm); await _context.SaveChangesAsync(); }
        return prm;
    }

    private async Task<PackagingType> EnsurePackagingTypeFromLibrary(PackagingLibrary lib, Dictionary<int, int> materialTaxonomyIdToPrmId)
    {
        var pt = await _context.PackagingTypes.FirstOrDefaultAsync(t => t.Name == lib.Name);
        if (pt == null) { pt = new PackagingType { Name = lib.Name, Weight = lib.Weight, IsFromLibrary = true, LibrarySource = lib.TaxonomyCode ?? "PackagingLibrary", Notes = $"Linked from PackagingLibrary (Id={lib.Id})" }; _context.PackagingTypes.Add(pt); await _context.SaveChangesAsync(); }
        if (lib.MaterialTaxonomyId.HasValue && materialTaxonomyIdToPrmId.TryGetValue(lib.MaterialTaxonomyId.Value, out var prmId) && !await _context.PackagingTypeMaterials.AnyAsync(ptm => ptm.PackagingTypeId == pt.Id && ptm.MaterialId == prmId))
        { _context.PackagingTypeMaterials.Add(new PackagingTypeMaterial { PackagingTypeId = pt.Id, MaterialId = prmId }); await _context.SaveChangesAsync(); }
        return pt;
    }

    private async Task<PackagingUnit> EnsurePackagingUnitFromGroup(PackagingGroup group, PackagingLibrary[] libs, Dictionary<int, PackagingType> libIdToPackagingType)
    {
        var pu = await _context.PackagingUnits.FirstOrDefaultAsync(u => u.Name == group.Name);
        if (pu == null) { pu = new PackagingUnit { Name = group.Name, UnitLevel = group.PackagingLayer ?? "Primary", Notes = $"Linked from PackagingGroup {group.PackId}" }; _context.PackagingUnits.Add(pu); await _context.SaveChangesAsync(); }
        foreach (var lib in libs)
        {
            if (!libIdToPackagingType.TryGetValue(lib.Id, out var pt) || await _context.PackagingUnitItems.AnyAsync(pui => pui.PackagingUnitId == pu.Id && pui.PackagingTypeId == pt.Id)) continue;
            _context.PackagingUnitItems.Add(new PackagingUnitItem { PackagingUnitId = pu.Id, PackagingTypeId = pt.Id, CollectionName = "Default", Quantity = 1 });
        }
        await _context.SaveChangesAsync();
        return pu;
    }

    private async Task EnsureProductPackagingSupplierProduct(int productId, int packagingSupplierProductId)
    {
        if (await _context.ProductPackagingSupplierProducts.AnyAsync(pp => pp.ProductId == productId && pp.PackagingSupplierProductId == packagingSupplierProductId)) return;
        _context.ProductPackagingSupplierProducts.Add(new ProductPackagingSupplierProduct { ProductId = productId, PackagingSupplierProductId = packagingSupplierProductId });
        await _context.SaveChangesAsync();
    }

    private async Task<Jurisdiction> EnsureJurisdiction(string code, string name, string countryCode)
    {
        var j = await _context.Jurisdictions.FirstOrDefaultAsync(x => x.Code == code);
        if (j == null) { j = new Jurisdiction { Code = code, Name = name, CountryCode = countryCode }; _context.Jurisdictions.Add(j); await _context.SaveChangesAsync(); }
        return j;
    }

    private async Task<Geography> EnsureGeography(string code, string name, int jurisdictionId)
    {
        var g = await _context.Geographies.FirstOrDefaultAsync(x => x.Code == code);
        if (g == null) { g = new Geography { Code = code, Name = name, JurisdictionId = jurisdictionId }; _context.Geographies.Add(g); await _context.SaveChangesAsync(); }
        return g;
    }

    private async Task<Product> EnsureProduct(string sku, string name, string brand, string gtin, string datasetKey, string? imageUrl = null)
    {
        var p = await _context.Products.FirstOrDefaultAsync(x => x.Sku == sku);
        if (p == null) { p = new Product { Sku = sku, Name = name, Brand = brand, Gtin = gtin, ProductCategory = "Pharmaceuticals", ProductSubCategory = "OTC & Supplements", CountryOfOrigin = "AU", DatasetKey = datasetKey, ImageUrl = imageUrl }; _context.Products.Add(p); await _context.SaveChangesAsync(); }
        else if (!string.IsNullOrEmpty(imageUrl) && string.IsNullOrEmpty(p.ImageUrl)) { p.ImageUrl = imageUrl; await _context.SaveChangesAsync(); }
        return p;
    }

    private async Task EnsureProductForm(int productId, string gtin, string productName, string brand, string category, string subCategory)
    {
        if (await _context.ProductForms.AnyAsync(pf => pf.ProductId == productId)) return;
        _context.ProductForms.Add(new ProductForm { ProductId = productId, Gtin = gtin, ProductName = productName, Brand = brand, ProductCategory = category, ProductSubCategory = subCategory, CountryOfOrigin = "AU", PackagingLevel = "Consumer Unit", PackagingType = "Blister", PackagingConfiguration = "Single component", Status = "submitted" });
        await _context.SaveChangesAsync();
    }

    private async Task EnsureProductPackaging(int productId, int packagingUnitId)
    {
        if (await _context.ProductPackagings.AnyAsync(pp => pp.ProductId == productId && pp.PackagingUnitId == packagingUnitId)) return;
        _context.ProductPackagings.Add(new ProductPackaging { ProductId = productId, PackagingUnitId = packagingUnitId });
        await _context.SaveChangesAsync();
    }

    private async Task EnsureDistribution(int productId, int packagingUnitId, int quantity, string retailer, string city, string country, int geographyId, int jurisdictionId)
    {
        _context.Distributions.Add(new Distribution { ProductId = productId, PackagingUnitId = packagingUnitId, Quantity = quantity, RetailerName = retailer, City = city, StateProvince = city, County = "", PostcodeZipcode = "", Country = country, DispatchDate = DateTime.UtcNow.AddDays(-30), GeographyId = geographyId, JurisdictionId = jurisdictionId, DatasetKey = DatasetKey });
        await _context.SaveChangesAsync();
    }

    private async Task<AsnShipment> EnsureAsnShipment(string asnNumber, string shipperGln, string shipperName, string receiverGln, string receiverName, DateTime shipDate, string datasetKey, string shipperCity = "Sydney", string shipperCountry = "AU")
    {
        var s = new AsnShipment { AsnNumber = asnNumber, ShipperGln = shipperGln, ShipperName = shipperName, ShipperCity = shipperCity, ShipperCountryCode = shipperCountry, ReceiverGln = receiverGln, ReceiverName = receiverName, ShipDate = shipDate, DeliveryDate = shipDate.AddDays(1), CarrierName = "FastFreight AU", TransportMode = "ROAD", TotalPackages = 1, TotalWeight = 25.5m, SourceFormat = "GS1_XML", Status = "DELIVERED", DatasetKey = datasetKey };
        _context.AsnShipments.Add(s);
        await _context.SaveChangesAsync();
        return s;
    }

    private async Task<AsnPallet> EnsureAsnPallet(int shipmentId, string sscc, string destName, string destCity, string destCountry, int seq, string destGln)
    {
        var p = new AsnPallet { AsnShipmentId = shipmentId, Sscc = sscc, PackageTypeCode = "PLT", GrossWeight = 25m, DestinationGln = destGln, DestinationName = destName, DestinationCity = destCity, DestinationCountryCode = destCountry, SequenceNumber = seq };
        _context.AsnPallets.Add(p);
        await _context.SaveChangesAsync();
        return p;
    }

    private async Task EnsureAsnLineItem(int palletId, string gtin, string description, decimal quantity, int lineNum)
    {
        _context.AsnLineItems.Add(new AsnLineItem { AsnPalletId = palletId, LineNumber = lineNum, Gtin = gtin, Description = description, Quantity = quantity, UnitOfMeasure = "PCE" });
        await _context.SaveChangesAsync();
    }
}
