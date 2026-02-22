using EPR.Domain.Entities;
using EPR.Data;
using Microsoft.EntityFrameworkCore;

namespace EPR.Web.Seeders;

/// <summary>
/// Seeds Garden dataset: 20 products with packaging, raw materials, suppliers,
/// distribution, and ASN data. Australian addresses (80%), 20% from outside Australia.
/// </summary>
public class GardenDatasetSeeder
{
    private const string DatasetKey = "Garden";
    private readonly EPRDbContext _context;

    public GardenDatasetSeeder(EPRDbContext context) => _context = context;

    public async Task SeedAsync()
    {
        var existingProductCount = await _context.Products.CountAsync(p => p.DatasetKey == DatasetKey);
        var existingAsnCount = await _context.AsnShipments.CountAsync(s => s.DatasetKey == DatasetKey);

        if (existingProductCount > 0 && existingAsnCount > 0) { await UpdateProductImagesIfMissing(); return; }
        if (existingProductCount > 0 && existingAsnCount == 0) { await EnsureAsnsAsync(); return; }

        var plasticTax = await EnsureMaterialTaxonomy("PLASTIC", "Plastics", 1);
        var paperTax = await EnsureMaterialTaxonomy("PAPER", "Paper & Cardboard", 1);

        // 80% Australian (4), 20% non-AU (1 from NZ)
        var supplierPots = await EnsureSupplier("GardenPots Sydney", "18 Nursery Road", "Sydney", "Australia", "sales@gardenpots.com.au");
        var supplierSoil = await EnsureSupplier("SoilCo Melbourne", "25 Compost Lane", "Melbourne", "Australia", "orders@soilco.com.au");
        var supplierLabels = await EnsureSupplier("GardenLabels Brisbane", "9 Plant Street", "Brisbane", "Australia", "info@gardenlabels.com.au");
        var supplierBags = await EnsureSupplier("MulchBag Perth", "14 Garden Way", "Perth", "Australia", "sales@mulchbag.com.au");
        var supplierNz = await EnsureSupplier("KiwiGarden Auckland", "7 Export Drive", "Auckland", "New Zealand", "orders@kiwigarden.co.nz");

        var pot = await EnsurePackagingLibrary("Nursery pot 15cm", "GDN-POT-001", 120m, plasticTax.Id, DatasetKey);
        var seedPacket = await EnsurePackagingLibrary("Seed packet", "GDN-SEED-001", 5m, paperTax.Id, DatasetKey);
        var mulchBag = await EnsurePackagingLibrary("Mulch bag 25L", "GDN-MULCH-001", 800m, plasticTax.Id, DatasetKey);
        var label = await EnsurePackagingLibrary("Plant label", "GDN-LABEL-001", 2m, plasticTax.Id, DatasetKey);
        var tray = await EnsurePackagingLibrary("Seedling tray 6-cell", "GDN-TRAY-001", 45m, plasticTax.Id, DatasetKey);

        await LinkPackagingMaterial(pot.Id, plasticTax.Id);
        await LinkPackagingMaterial(seedPacket.Id, paperTax.Id);
        await LinkPackagingMaterial(mulchBag.Id, plasticTax.Id);
        await LinkPackagingMaterial(label.Id, plasticTax.Id);
        await LinkPackagingMaterial(tray.Id, plasticTax.Id);

        var spPot = await EnsureSupplierProduct(supplierPots.Id, "Nursery Pot 15cm", "GDN-POT-001");
        var spSeed = await EnsureSupplierProduct(supplierLabels.Id, "Seed Packet", "GDN-SEED-001");
        var spMulch = await EnsureSupplierProduct(supplierBags.Id, "Mulch Bag 25L", "GDN-MULCH-001");
        var spLabel = await EnsureSupplierProduct(supplierLabels.Id, "Plant Label", "GDN-LABEL-001");
        var spTray = await EnsureSupplierProduct(supplierPots.Id, "Seedling Tray 6-cell", "GDN-TRAY-001");

        await LinkPackagingSupplier(pot.Id, spPot.Id, true);
        await LinkPackagingSupplier(seedPacket.Id, spSeed.Id, true);
        await LinkPackagingSupplier(mulchBag.Id, spMulch.Id, true);
        await LinkPackagingSupplier(label.Id, spLabel.Id, true);
        await LinkPackagingSupplier(tray.Id, spTray.Id, true);

        var groupShipping = await EnsurePackagingGroup("GDN-SHIP-001", "Garden Shipping Pack", "Secondary", 845m, DatasetKey);
        await AddGroupItem(groupShipping.Id, mulchBag.Id, 0);
        await AddGroupItem(groupShipping.Id, tray.Id, 1);

        var groupProduct = await EnsurePackagingGroup("GDN-PROD-001", "Garden Product Pack", "Primary", 127m, DatasetKey);
        await AddGroupItem(groupProduct.Id, pot.Id, 0);
        await AddGroupItem(groupProduct.Id, seedPacket.Id, 1);
        await AddGroupItem(groupProduct.Id, label.Id, 2);

        var prmPlastic = await EnsurePackagingRawMaterial("Plastics", "Plastic materials");
        var prmPaper = await EnsurePackagingRawMaterial("Paper & Cardboard", "Paper and cardboard materials");
        var materialTaxToPrm = new Dictionary<int, int> { { plasticTax.Id, prmPlastic.Id }, { paperTax.Id, prmPaper.Id } };

        var ptPot = await EnsurePackagingTypeFromLibrary(pot, materialTaxToPrm);
        var ptSeed = await EnsurePackagingTypeFromLibrary(seedPacket, materialTaxToPrm);
        var ptMulch = await EnsurePackagingTypeFromLibrary(mulchBag, materialTaxToPrm);
        var ptLabel = await EnsurePackagingTypeFromLibrary(label, materialTaxToPrm);
        var ptTray = await EnsurePackagingTypeFromLibrary(tray, materialTaxToPrm);

        var shippingLibs = new[] { mulchBag, tray };
        var productLibs = new[] { pot, seedPacket, label };
        var libToPt = new Dictionary<int, PackagingType> { { pot.Id, ptPot }, { seedPacket.Id, ptSeed }, { mulchBag.Id, ptMulch }, { label.Id, ptLabel }, { tray.Id, ptTray } };
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
            ("GDN-001", "Tomato Seeds Heirloom", "GardenGrow", "50605234567890", "https://picsum.photos/seed/gdn-tomato/200/200"),
            ("GDN-002", "Potting Mix 25L", "SoilCo", "50605234567891", "https://picsum.photos/seed/gdn-potting/200/200"),
            ("GDN-003", "Lavender Plant", "GardenGrow", "50605234567892", "https://picsum.photos/seed/gdn-lavender/200/200"),
            ("GDN-004", "Mulch Premium 25L", "SoilCo", "50605234567893", "https://picsum.photos/seed/gdn-mulch/200/200"),
            ("GDN-005", "Basil Herb Seeds", "GardenGrow", "50605234567894", "https://picsum.photos/seed/gdn-basil/200/200"),
            ("GDN-006", "Fertiliser All-Purpose", "SoilCo", "50605234567895", "https://picsum.photos/seed/gdn-fertiliser/200/200"),
            ("GDN-007", "Rose Bush Hybrid", "GardenGrow", "50605234567896", "https://picsum.photos/seed/gdn-rose/200/200"),
            ("GDN-008", "Compost 20L", "SoilCo", "50605234567897", "https://picsum.photos/seed/gdn-compost/200/200"),
            ("GDN-009", "Cucumber Seeds", "GardenGrow", "50605234567898", "https://picsum.photos/seed/gdn-cucumber/200/200"),
            ("GDN-010", "Succulent Assorted", "GardenGrow", "50605234567899", "https://picsum.photos/seed/gdn-succulent/200/200"),
            ("GDN-011", "Pepper Seeds Capsicum", "GardenGrow", "50605234567900", "https://picsum.photos/seed/gdn-pepper/200/200"),
            ("GDN-012", "Bark Mulch 15L", "SoilCo", "50605234567901", "https://picsum.photos/seed/gdn-bark/200/200"),
            ("GDN-013", "Hydrangea Plant", "GardenGrow", "50605234567902", "https://picsum.photos/seed/gdn-hydrangea/200/200"),
            ("GDN-014", "Seedling Mix 10L", "SoilCo", "50605234567903", "https://picsum.photos/seed/gdn-seedling/200/200"),
            ("GDN-015", "Carrot Seeds", "GardenGrow", "50605234567904", "https://picsum.photos/seed/gdn-carrot/200/200"),
            ("GDN-016", "Citrus Tree Dwarf", "GardenGrow", "50605234567905", "https://picsum.photos/seed/gdn-citrus/200/200"),
            ("GDN-017", "Weed Mat 5m", "SoilCo", "50605234567906", "https://picsum.photos/seed/gdn-weedmat/200/200"),
            ("GDN-018", "Parsley Herb Seeds", "GardenGrow", "50605234567907", "https://picsum.photos/seed/gdn-parsley/200/200"),
            ("GDN-019", "Native Plant Mix", "GardenGrow", "50605234567908", "https://picsum.photos/seed/gdn-native/200/200"),
            ("GDN-020", "Cactus Soil 5L", "SoilCo", "50605234567909", "https://picsum.photos/seed/gdn-cactus/200/200"),
        };

        var supplierProductIds = new[] { spPot.Id, spSeed.Id, spMulch.Id, spLabel.Id, spTray.Id };
        var productEntities = new List<Product>();
        for (var idx = 0; idx < products.Length; idx++)
        {
            var (sku, name, brand, gtin, imageUrl) = products[idx];
            var p = await EnsureProduct(sku, name, brand, gtin, DatasetKey, imageUrl);
            productEntities.Add(p);
            await EnsureProductForm(p.Id, gtin, name, brand, "Garden", "Plants & Soil");
            await EnsureProductPackaging(p.Id, puShipping.Id);
            await EnsureProductPackaging(p.Id, puProduct.Id);
            await EnsureProductPackagingSupplierProduct(p.Id, supplierProductIds[idx % supplierProductIds.Length]);
        }

        var rnd = new Random(46);
        var geos = new[] { (sydneyGeo, "Sydney", auJurisdiction.Id, "AU"), (melbourneGeo, "Melbourne", auJurisdiction.Id, "AU"), (brisbaneGeo, "Brisbane", auJurisdiction.Id, "AU"), (aucklandGeo, "Auckland", nzJurisdiction.Id, "NZ") };
        foreach (var p in productEntities)
        {
            var (geo, cityName, jurId, country) = geos[rnd.Next(geos.Length)];
            await EnsureDistribution(p.Id, puShipping.Id, rnd.Next(40, 350), "Garden Centre", cityName, country, geo.Id, jurId);
        }

        var baseDate = DateTime.UtcNow.AddDays(-14);
        var receivers = new[] { ("5061987654327", "GardenMart Sydney", "Sydney", "AU"), ("5061987654328", "PlantHub Melbourne", "Melbourne", "AU"), ("5061987654329", "KiwiGarden Auckland", "Auckland", "NZ") };
        var shippers = new[] { ("5061987654330", "GardenPots Sydney", "Sydney", "AU"), ("5061987654331", "SoilCo Melbourne", "Melbourne", "AU"), ("5061987654332", "KiwiGarden Auckland", "Auckland", "NZ") };
        int lineNum = 1;
        for (int i = 0; i < 12; i++)
        {
            var (recGln, recName, recCity, recCountry) = receivers[i % receivers.Length];
            var (shipGln, shipName, shipCity, shipCountry) = shippers[i % shippers.Length];
            var ship = await EnsureAsnShipment($"ASN-GDN-{1000 + i}", shipGln, shipName, recGln, recName, baseDate.AddDays(i * 2), DatasetKey, shipCity, shipCountry);
            for (int p = 0; p < 1 + (i % 3); p++)
            {
                var pallet = await EnsureAsnPallet(ship.Id, $"3579523456789012{(i * 3 + p):D2}", recName, recCity, recCountry, p + 1, recGln);
                for (int li = 0; li < 2 + (i + p) % 4; li++)
                {
                    var prod = productEntities[(i * 3 + p * 2 + li) % productEntities.Count];
                    await EnsureAsnLineItem(pallet.Id, prod.Gtin!, prod.Name, 10 + (i + li) % 20, lineNum++);
                }
            }
        }

        await _context.SaveChangesAsync();
        Console.WriteLine($"✓ Garden dataset seeded: 20 products, packaging, distribution, ASNs (AU/NZ)");
    }

    private async Task EnsureAsnsAsync()
    {
        var productEntities = await _context.Products.Where(p => p.DatasetKey == DatasetKey && p.Gtin != null).OrderBy(p => p.Sku).ToListAsync();
        if (productEntities.Count == 0) return;
        var baseDate = DateTime.UtcNow.AddDays(-14);
        var receivers = new[] { ("5061987654327", "GardenMart Sydney", "Sydney", "AU"), ("5061987654328", "PlantHub Melbourne", "Melbourne", "AU"), ("5061987654329", "KiwiGarden Auckland", "Auckland", "NZ") };
        var shippers = new[] { ("5061987654330", "GardenPots Sydney", "Sydney", "AU"), ("5061987654331", "SoilCo Melbourne", "Melbourne", "AU"), ("5061987654332", "KiwiGarden Auckland", "Auckland", "NZ") };
        int lineNum = 1;
        for (int i = 0; i < 12; i++)
        {
            var (recGln, recName, recCity, recCountry) = receivers[i % receivers.Length];
            var (shipGln, shipName, shipCity, shipCountry) = shippers[i % shippers.Length];
            var ship = await EnsureAsnShipment($"ASN-GDN-{1000 + i}", shipGln, shipName, recGln, recName, baseDate.AddDays(i * 2), DatasetKey, shipCity, shipCountry);
            for (int p = 0; p < 1 + (i % 3); p++)
            {
                var pallet = await EnsureAsnPallet(ship.Id, $"3579523456789012{(i * 3 + p):D2}", recName, recCity, recCountry, p + 1, recGln);
                for (int li = 0; li < 2 + (i + p) % 4; li++)
                {
                    var prod = productEntities[(i * 3 + p * 2 + li) % productEntities.Count];
                    await EnsureAsnLineItem(pallet.Id, prod.Gtin!, prod.Name, 10 + (i + li) % 20, lineNum++);
                }
            }
        }
        await _context.SaveChangesAsync();
    }

    private async Task UpdateProductImagesIfMissing()
    {
        var seeds = new[] { "gdn-tomato", "gdn-potting", "gdn-lavender", "gdn-mulch", "gdn-basil", "gdn-fertiliser", "gdn-rose", "gdn-compost", "gdn-cucumber", "gdn-succulent", "gdn-pepper", "gdn-bark", "gdn-hydrangea", "gdn-seedling", "gdn-carrot", "gdn-citrus", "gdn-weedmat", "gdn-parsley", "gdn-native", "gdn-cactus" };
        var products = await _context.Products.Where(p => p.DatasetKey == DatasetKey && (p.ImageUrl == null || p.ImageUrl == "")).ToListAsync();
        foreach (var p in products)
        {
            if (p.Sku != null && p.Sku.StartsWith("GDN-") && int.TryParse(p.Sku.AsSpan(4), out var n) && n >= 1 && n <= 20)
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

    private async Task<PackagingGroup> EnsurePackagingGroup(string packId, string name, string layer, decimal totalWeight, string datasetKey)
    {
        var g = new PackagingGroup { PackId = packId, Name = name, PackagingLayer = layer, TotalPackWeight = totalWeight, DatasetKey = datasetKey, IsActive = true };
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
        if (p == null) { p = new Product { Sku = sku, Name = name, Brand = brand, Gtin = gtin, ProductCategory = "Garden", ProductSubCategory = "Plants & Soil", CountryOfOrigin = "AU", DatasetKey = datasetKey, ImageUrl = imageUrl }; _context.Products.Add(p); await _context.SaveChangesAsync(); }
        else if (!string.IsNullOrEmpty(imageUrl) && string.IsNullOrEmpty(p.ImageUrl)) { p.ImageUrl = imageUrl; await _context.SaveChangesAsync(); }
        return p;
    }

    private async Task EnsureProductForm(int productId, string gtin, string productName, string brand, string category, string subCategory)
    {
        if (await _context.ProductForms.AnyAsync(pf => pf.ProductId == productId)) return;
        _context.ProductForms.Add(new ProductForm { ProductId = productId, Gtin = gtin, ProductName = productName, Brand = brand, ProductCategory = category, ProductSubCategory = subCategory, CountryOfOrigin = "AU", PackagingLevel = "Consumer Unit", PackagingType = "Box", PackagingConfiguration = "Single component", Status = "submitted" });
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
