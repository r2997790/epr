using EPR.Domain.Entities;
using EPR.Data;
using Microsoft.EntityFrameworkCore;

namespace EPR.Web.Seeders;

/// <summary>
/// Seeds Fresh Produce dataset: 20 products with packaging, raw materials, suppliers,
/// distribution, and ASN data. Australian addresses (80%), 20% from outside Australia.
/// </summary>
public class FreshProduceDatasetSeeder
{
    private const string DatasetKey = "Fresh Produce";
    private readonly EPRDbContext _context;

    public FreshProduceDatasetSeeder(EPRDbContext context) => _context = context;

    public async Task SeedAsync()
    {
        var existingProductCount = await _context.Products.CountAsync(p => p.DatasetKey == DatasetKey);
        var existingAsnCount = await _context.AsnShipments.CountAsync(s => s.DatasetKey == DatasetKey);

        if (existingProductCount > 0 && existingAsnCount > 0) { await UpdateProductImagesIfMissing(); return; }
        if (existingProductCount > 0 && existingAsnCount == 0) { await EnsureAsnsAsync(); return; }

        var plasticTax = await EnsureMaterialTaxonomy("PLASTIC", "Plastics", 1);
        var paperTax = await EnsureMaterialTaxonomy("PAPER", "Paper & Cardboard", 1);

        // 80% Australian (4), 20% non-AU (1 from NZ)
        var supplierPunnets = await EnsureSupplier("FreshPack Sydney", "45 Produce Way", "Sydney", "Australia", "sales@freshpacksydney.com.au");
        var supplierCrates = await EnsureSupplier("CrateCo Melbourne", "12 Market Street", "Melbourne", "Australia", "orders@crateco.com.au");
        var supplierLabels = await EnsureSupplier("LabelFresh Brisbane", "8 Print Lane", "Brisbane", "Australia", "info@labelfresh.com.au");
        var supplierBags = await EnsureSupplier("BagPro Perth", "22 Packaging Rd", "Perth", "Australia", "sales@bagpro.com.au");
        var supplierNz = await EnsureSupplier("KiwiPack Auckland", "5 Export Drive", "Auckland", "New Zealand", "orders@kiwipack.co.nz");

        var punnet = await EnsurePackagingLibrary("Berry punnet 250g", "FP-PUNNET-001", 15m, plasticTax.Id, DatasetKey);
        var crate = await EnsurePackagingLibrary("Produce crate", "FP-CRATE-001", 450m, paperTax.Id, DatasetKey);
        var label = await EnsurePackagingLibrary("Produce label", "FP-LABEL-001", 2m, paperTax.Id, DatasetKey);
        var bag = await EnsurePackagingLibrary("Mesh produce bag", "FP-BAG-001", 8m, plasticTax.Id, DatasetKey);
        var tray = await EnsurePackagingLibrary("Tomato tray", "FP-TRAY-001", 35m, plasticTax.Id, DatasetKey);

        await LinkPackagingMaterial(punnet.Id, plasticTax.Id);
        await LinkPackagingMaterial(crate.Id, paperTax.Id);
        await LinkPackagingMaterial(label.Id, paperTax.Id);
        await LinkPackagingMaterial(bag.Id, plasticTax.Id);
        await LinkPackagingMaterial(tray.Id, plasticTax.Id);

        var woodTax = await EnsureMaterialTaxonomy("WOOD", "Softwood Timber", 1);
        var ldpeTax = await EnsureMaterialTaxonomy("LDPE", "Low-Density Polyethylene Film", 1);
        var palletLib = await EnsurePackagingLibrary("Wood Pallet", "FP-PLT-001", 22000m, woodTax.Id, DatasetKey);
        var wrapLib = await EnsurePackagingLibrary("Stretch Wrap", "FP-WRAP-PLT-001", 300m, ldpeTax.Id, DatasetKey);
        await LinkPackagingMaterial(palletLib.Id, woodTax.Id);
        await LinkPackagingMaterial(wrapLib.Id, ldpeTax.Id);

        var spPunnet = await EnsureSupplierProduct(supplierPunnets.Id, "Berry Punnet 250g", "FP-PUNNET-001");
        var spCrate = await EnsureSupplierProduct(supplierCrates.Id, "Produce Crate 12pk", "FP-CRATE-001");
        var spLabel = await EnsureSupplierProduct(supplierLabels.Id, "Produce Label", "FP-LABEL-001");
        var spBag = await EnsureSupplierProduct(supplierBags.Id, "Mesh Bag 1kg", "FP-BAG-001");
        var spTray = await EnsureSupplierProduct(supplierPunnets.Id, "Tomato Tray", "FP-TRAY-001");

        await LinkPackagingSupplier(punnet.Id, spPunnet.Id, true);
        await LinkPackagingSupplier(crate.Id, spCrate.Id, true);
        await LinkPackagingSupplier(label.Id, spLabel.Id, true);
        await LinkPackagingSupplier(bag.Id, spBag.Id, true);
        await LinkPackagingSupplier(tray.Id, spTray.Id, true);

        var groupPallet = await EnsurePackagingGroup("FP-PLT-001", "Fresh Produce Pallet", "Tertiary", 22300m, DatasetKey);
        await AddGroupItem(groupPallet.Id, palletLib.Id, 0);
        await AddGroupItem(groupPallet.Id, wrapLib.Id, 1);

        var groupShipping = await EnsurePackagingGroup("FP-SHIP-001", "Fresh Produce Shipping Pack", "Secondary", 510m, DatasetKey, groupPallet.Id, 32);
        await AddGroupItem(groupShipping.Id, crate.Id, 0);
        await AddGroupItem(groupShipping.Id, bag.Id, 1);

        var groupProduct = await EnsurePackagingGroup("FP-PROD-001", "Fresh Produce Product Pack", "Primary", 60m, DatasetKey, groupShipping.Id, 10);
        await AddGroupItem(groupProduct.Id, punnet.Id, 0);
        await AddGroupItem(groupProduct.Id, tray.Id, 1);
        await AddGroupItem(groupProduct.Id, label.Id, 2);

        var prmPlastic = await EnsurePackagingRawMaterial("Plastics", "Plastic materials");
        var prmPaper = await EnsurePackagingRawMaterial("Paper & Cardboard", "Paper and cardboard materials");
        var materialTaxToPrm = new Dictionary<int, int> { { plasticTax.Id, prmPlastic.Id }, { paperTax.Id, prmPaper.Id } };

        var ptPunnet = await EnsurePackagingTypeFromLibrary(punnet, materialTaxToPrm);
        var ptCrate = await EnsurePackagingTypeFromLibrary(crate, materialTaxToPrm);
        var ptLabel = await EnsurePackagingTypeFromLibrary(label, materialTaxToPrm);
        var ptBag = await EnsurePackagingTypeFromLibrary(bag, materialTaxToPrm);
        var ptTray = await EnsurePackagingTypeFromLibrary(tray, materialTaxToPrm);

        var shippingLibs = new[] { crate, bag };
        var productLibs = new[] { punnet, tray, label };
        var libToPt = new Dictionary<int, PackagingType> { { punnet.Id, ptPunnet }, { crate.Id, ptCrate }, { label.Id, ptLabel }, { bag.Id, ptBag }, { tray.Id, ptTray } };
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
            ("FP-001", "Strawberries 250g", "FreshFields", "50604234567890", "https://picsum.photos/seed/fp-strawberry/200/200"),
            ("FP-002", "Blueberries 125g", "FreshFields", "50604234567891", "https://picsum.photos/seed/fp-blueberry/200/200"),
            ("FP-003", "Cherry Tomatoes 400g", "GardenFresh", "50604234567892", "https://picsum.photos/seed/fp-cherry/200/200"),
            ("FP-004", "Raspberries 150g", "FreshFields", "50604234567893", "https://picsum.photos/seed/fp-raspberry/200/200"),
            ("FP-005", "Mixed Salad 200g", "GardenFresh", "50604234567894", "https://picsum.photos/seed/fp-salad/200/200"),
            ("FP-006", "Avocados 4pk", "FreshFields", "50604234567895", "https://picsum.photos/seed/fp-avocado/200/200"),
            ("FP-007", "Blackberries 170g", "FreshFields", "50604234567896", "https://picsum.photos/seed/fp-blackberry/200/200"),
            ("FP-008", "Baby Spinach 100g", "GardenFresh", "50604234567897", "https://picsum.photos/seed/fp-spinach/200/200"),
            ("FP-009", "Grapes 500g", "FreshFields", "50604234567898", "https://picsum.photos/seed/fp-grapes/200/200"),
            ("FP-010", "Rocket 80g", "GardenFresh", "50604234567899", "https://picsum.photos/seed/fp-rocket/200/200"),
            ("FP-011", "Figs 200g", "FreshFields", "50604234567900", "https://picsum.photos/seed/fp-figs/200/200"),
            ("FP-012", "Mixed Berries 300g", "FreshFields", "50604234567901", "https://picsum.photos/seed/fp-mixed/200/200"),
            ("FP-013", "Kale 150g", "GardenFresh", "50604234567902", "https://picsum.photos/seed/fp-kale/200/200"),
            ("FP-014", "Plums 400g", "FreshFields", "50604234567903", "https://picsum.photos/seed/fp-plums/200/200"),
            ("FP-015", "Watercress 75g", "GardenFresh", "50604234567904", "https://picsum.photos/seed/fp-watercress/200/200"),
            ("FP-016", "Apricots 350g", "FreshFields", "50604234567905", "https://picsum.photos/seed/fp-apricot/200/200"),
            ("FP-017", "Peaches 500g", "FreshFields", "50604234567906", "https://picsum.photos/seed/fp-peach/200/200"),
            ("FP-018", "Herb Mix 30g", "GardenFresh", "50604234567907", "https://picsum.photos/seed/fp-herbs/200/200"),
            ("FP-019", "Nectarines 450g", "FreshFields", "50604234567908", "https://picsum.photos/seed/fp-nectarine/200/200"),
            ("FP-020", "Mixed Leaves 120g", "GardenFresh", "50604234567909", "https://picsum.photos/seed/fp-leaves/200/200"),
        };

        var supplierProductIds = new[] { spPunnet.Id, spCrate.Id, spLabel.Id, spBag.Id, spTray.Id };
        var productEntities = new List<Product>();
        for (var idx = 0; idx < products.Length; idx++)
        {
            var (sku, name, brand, gtin, imageUrl) = products[idx];
            var p = await EnsureProduct(sku, name, brand, gtin, DatasetKey, imageUrl);
            productEntities.Add(p);
            await EnsureProductForm(p.Id, gtin, name, brand, "Fresh Produce", "Fruits & Vegetables");
            await EnsureProductPackaging(p.Id, puShipping.Id);
            await EnsureProductPackaging(p.Id, puProduct.Id);
            await EnsureProductPackagingSupplierProduct(p.Id, supplierProductIds[idx % supplierProductIds.Length]);
        }

        var rnd = new Random(45);
        var geos = new[] { (sydneyGeo, "Sydney", auJurisdiction.Id, "AU"), (melbourneGeo, "Melbourne", auJurisdiction.Id, "AU"), (brisbaneGeo, "Brisbane", auJurisdiction.Id, "AU"), (aucklandGeo, "Auckland", nzJurisdiction.Id, "NZ") };
        foreach (var p in productEntities)
        {
            var (geo, cityName, jurId, country) = geos[rnd.Next(geos.Length)];
            await EnsureDistribution(p.Id, puShipping.Id, rnd.Next(50, 400), "Fresh Market", cityName, country, geo.Id, jurId);
        }

        var baseDate = DateTime.UtcNow.AddDays(-14);
        var receivers = new[] { ("5061987654321", "FreshMart Sydney", "Sydney", "AU"), ("5061987654322", "ProduceHub Melbourne", "Melbourne", "AU"), ("5061987654323", "KiwiFresh Auckland", "Auckland", "NZ") };
        var shippers = new[] { ("5061987654324", "FreshPack Sydney"), ("5061987654325", "CrateCo Melbourne"), ("5061987654326", "KiwiPack Auckland") };
        int lineNum = 1;
        for (int i = 0; i < 12; i++)
        {
            var (recGln, recName, recCity, recCountry) = receivers[i % receivers.Length];
            var (shipGln, shipName) = shippers[i % shippers.Length];
            var ship = await EnsureAsnShipment($"ASN-FP-{1000 + i}", shipGln, shipName, recGln, recName, baseDate.AddDays(i * 2), DatasetKey, recCountry == "NZ" ? "Auckland" : "Sydney");
            var palletsPerShipment = 1 + (i % 3);
            for (int p = 0; p < palletsPerShipment; p++)
            {
                var pallet = await EnsureAsnPallet(ship.Id, $"3579423456789012{(i * 3 + p):D2}", recName, recCity, recCountry, p + 1, recGln);
                var productsPerPallet = 2 + (i + p) % 4;
                for (int li = 0; li < productsPerPallet; li++)
                {
                    var prod = productEntities[(i * 3 + p * 2 + li) % productEntities.Count];
                    await EnsureAsnLineItem(pallet.Id, prod.Gtin!, prod.Name, 20 + (i + li) % 40, lineNum++);
                }
            }
        }

        await _context.SaveChangesAsync();
        Console.WriteLine($"✓ Fresh Produce dataset seeded: 20 products, packaging, distribution, ASNs (AU/NZ)");
    }

    private async Task EnsureAsnsAsync()
    {
        var productEntities = await _context.Products.Where(p => p.DatasetKey == DatasetKey && p.Gtin != null).OrderBy(p => p.Sku).ToListAsync();
        if (productEntities.Count == 0) return;
        var baseDate = DateTime.UtcNow.AddDays(-14);
        var receivers = new[] { ("5061987654321", "FreshMart Sydney", "Sydney", "AU"), ("5061987654322", "ProduceHub Melbourne", "Melbourne", "AU"), ("5061987654323", "KiwiFresh Auckland", "Auckland", "NZ") };
        var shippers = new[] { ("5061987654324", "FreshPack Sydney", "Sydney", "AU"), ("5061987654325", "CrateCo Melbourne", "Melbourne", "AU"), ("5061987654326", "KiwiPack Auckland", "Auckland", "NZ") };
        int lineNum = 1;
        for (int i = 0; i < 12; i++)
        {
            var (recGln, recName, recCity, recCountry) = receivers[i % receivers.Length];
            var (shipGln, shipName, shipCity, shipCountry) = shippers[i % shippers.Length];
            var ship = await EnsureAsnShipment($"ASN-FP-{1000 + i}", shipGln, shipName, recGln, recName, baseDate.AddDays(i * 2), DatasetKey, shipCity, shipCountry);
            for (int p = 0; p < 1 + (i % 3); p++)
            {
                var pallet = await EnsureAsnPallet(ship.Id, $"3579423456789012{(i * 3 + p):D2}", recName, recCity, recCountry, p + 1, recGln);
                for (int li = 0; li < 2 + (i + p) % 4; li++)
                {
                    var prod = productEntities[(i * 3 + p * 2 + li) % productEntities.Count];
                    await EnsureAsnLineItem(pallet.Id, prod.Gtin!, prod.Name, 20 + (i + li) % 40, lineNum++);
                }
            }
        }
        await _context.SaveChangesAsync();
    }

    private async Task UpdateProductImagesIfMissing()
    {
        var seeds = new[] { "fp-strawberry", "fp-blueberry", "fp-cherry", "fp-raspberry", "fp-salad", "fp-avocado", "fp-blackberry", "fp-spinach", "fp-grapes", "fp-rocket", "fp-figs", "fp-mixed", "fp-kale", "fp-plums", "fp-watercress", "fp-apricot", "fp-peach", "fp-herbs", "fp-nectarine", "fp-leaves" };
        var products = await _context.Products.Where(p => p.DatasetKey == DatasetKey && (p.ImageUrl == null || p.ImageUrl == "")).ToListAsync();
        foreach (var p in products)
        {
            if (p.Sku != null && p.Sku.StartsWith("FP-") && int.TryParse(p.Sku.AsSpan(3), out var n) && n >= 1 && n <= 20)
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
        if (p == null) { p = new Product { Sku = sku, Name = name, Brand = brand, Gtin = gtin, ProductCategory = "Fresh Produce", ProductSubCategory = "Fruits & Vegetables", CountryOfOrigin = "AU", DatasetKey = datasetKey, ImageUrl = imageUrl }; _context.Products.Add(p); await _context.SaveChangesAsync(); }
        else if (!string.IsNullOrEmpty(imageUrl) && string.IsNullOrEmpty(p.ImageUrl)) { p.ImageUrl = imageUrl; await _context.SaveChangesAsync(); }
        return p;
    }

    private async Task EnsureProductForm(int productId, string gtin, string productName, string brand, string category, string subCategory)
    {
        if (await _context.ProductForms.AnyAsync(pf => pf.ProductId == productId)) return;
        _context.ProductForms.Add(new ProductForm { ProductId = productId, Gtin = gtin, ProductName = productName, Brand = brand, ProductCategory = category, ProductSubCategory = subCategory, CountryOfOrigin = "AU", PackagingLevel = "Consumer Unit", PackagingType = "Punnet", PackagingConfiguration = "Single component", Status = "submitted" });
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
