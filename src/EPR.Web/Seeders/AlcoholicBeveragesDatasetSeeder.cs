using EPR.Domain.Entities;
using EPR.Data;
using Microsoft.EntityFrameworkCore;

namespace EPR.Web.Seeders;

/// <summary>
/// Seeds a comprehensive Alcoholic Beverages dataset: 20 products with packaging, raw materials,
/// packaging groups, suppliers, distribution, and ASN data. Full traceability chain.
/// </summary>
public class AlcoholicBeveragesDatasetSeeder
{
    private const string DatasetKey = "Alcoholic Beverages";
    private readonly EPRDbContext _context;

    public AlcoholicBeveragesDatasetSeeder(EPRDbContext context)
    {
        _context = context;
    }

    public async Task SeedAsync()
    {
        var existingProductCount = await _context.Products.CountAsync(p => p.DatasetKey == DatasetKey);
        var existingAsnCount = await _context.AsnShipments.CountAsync(s => s.DatasetKey == DatasetKey);

        if (existingProductCount > 0 && existingAsnCount > 0)
        {
            await UpdateProductImagesIfMissing();
            Console.WriteLine("Alcoholic Beverages dataset verified (images updated if needed)");
            return;
        }

        if (existingProductCount > 0 && existingAsnCount == 0)
        {
            await EnsureAsnsAsync();
            Console.WriteLine("✓ Alcoholic Beverages ASNs added (linked to existing products)");
            return;
        }

        var glassTax = await EnsureMaterialTaxonomy("GLASS", "Glass", 1);
        var plasticTax = await EnsureMaterialTaxonomy("PLASTIC", "Plastics", 1);
        var paperTax = await EnsureMaterialTaxonomy("PAPER", "Paper & Cardboard", 1);
        var metalTax = await EnsureMaterialTaxonomy("METAL", "Metal", 1);

        // Suppliers: 80% Australian, 20% international (UK)
        var supplierBottles = await EnsureSupplier("BeveragePack Australia", "22 Bottle Lane", "Sydney", "Australia", "sales@beveragepack.com.au");
        var supplierLabels = await EnsureSupplier("LabelPrint Beverages Australia", "5 Print Street", "Melbourne", "Australia", "info@labelprint.com.au");
        var supplierCaps = await EnsureSupplier("CapWorks Australia", "8 Closure Road", "Brisbane", "Australia", "orders@capworks.com.au");
        var supplierFoilUK = await EnsureSupplier("FoilCaps UK", "3 Foil Lane", "Leeds", "UK", "orders@foilcaps.co.uk");

        var wineBottle = await EnsurePackagingLibrary("750ml wine bottle", "ALC-BOTTLE-001", 520m, glassTax.Id, DatasetKey);
        var screwCap = await EnsurePackagingLibrary("Screw cap 28mm", "ALC-CAP-001", 5m, metalTax.Id, DatasetKey);
        var wineLabel = await EnsurePackagingLibrary("Wine bottle label", "ALC-LABEL-001", 8m, paperTax.Id, DatasetKey);
        var neckFoil = await EnsurePackagingLibrary("Neck foil capsule", "ALC-FOIL-001", 3m, plasticTax.Id, DatasetKey);
        var wineBox = await EnsurePackagingLibrary("Wine gift box", "ALC-BOX-001", 180m, paperTax.Id, DatasetKey);
        var shippingCase = await EnsurePackagingLibrary("12-bottle shipping case", "ALC-CASE-001", 800m, paperTax.Id, DatasetKey);

        await LinkPackagingMaterial(wineBottle.Id, glassTax.Id);
        await LinkPackagingMaterial(screwCap.Id, metalTax.Id);
        await LinkPackagingMaterial(wineLabel.Id, paperTax.Id);
        await LinkPackagingMaterial(neckFoil.Id, plasticTax.Id);
        await LinkPackagingMaterial(wineBox.Id, paperTax.Id);
        await LinkPackagingMaterial(shippingCase.Id, paperTax.Id);

        var spBottle = await EnsureSupplierProduct(supplierBottles.Id, "Wine Bottle 750ml", "ALC-BOTTLE-001");
        var spCap = await EnsureSupplierProduct(supplierCaps.Id, "Screw Cap 28mm", "ALC-CAP-001");
        var spLabel = await EnsureSupplierProduct(supplierLabels.Id, "Wine Label 75x50", "ALC-LABEL-001");
        var spFoil = await EnsureSupplierProduct(supplierCaps.Id, "Neck Foil Capsule", "ALC-FOIL-001");
        var spBox = await EnsureSupplierProduct(supplierBottles.Id, "Wine Gift Box", "ALC-BOX-001");
        var spCase = await EnsureSupplierProduct(supplierBottles.Id, "12-Bottle Case", "ALC-CASE-001");
        var spFoilUK = await EnsureSupplierProduct(supplierFoilUK.Id, "Neck Foil Capsule UK", "ALC-FOIL-002");

        await LinkPackagingSupplier(wineBottle.Id, spBottle.Id, true);
        await LinkPackagingSupplier(screwCap.Id, spCap.Id, true);
        await LinkPackagingSupplier(wineLabel.Id, spLabel.Id, true);
        await LinkPackagingSupplier(neckFoil.Id, spFoilUK.Id, true);
        await LinkPackagingSupplier(wineBox.Id, spBox.Id, true);
        await LinkPackagingSupplier(shippingCase.Id, spCase.Id, true);

        var groupShipping = await EnsurePackagingGroup("ALC-SHIP-001", "Alcoholic Beverages Shipping Pack", "Secondary", 1336m, DatasetKey);
        await AddGroupItem(groupShipping.Id, shippingCase.Id, 0);
        await AddGroupItem(groupShipping.Id, wineBox.Id, 1);

        var groupProduct = await EnsurePackagingGroup("ALC-PROD-001", "Alcoholic Beverages Product Pack", "Primary", 716m, DatasetKey);
        await AddGroupItem(groupProduct.Id, wineBottle.Id, 0);
        await AddGroupItem(groupProduct.Id, screwCap.Id, 1);
        await AddGroupItem(groupProduct.Id, wineLabel.Id, 2);
        await AddGroupItem(groupProduct.Id, neckFoil.Id, 3);
        await AddGroupItem(groupProduct.Id, wineBox.Id, 4);

        var prmGlass = await EnsurePackagingRawMaterial("Glass", "Glass materials");
        var prmPlastic = await EnsurePackagingRawMaterial("Plastics", "Plastic materials");
        var prmPaper = await EnsurePackagingRawMaterial("Paper & Cardboard", "Paper and cardboard materials");
        var prmMetal = await EnsurePackagingRawMaterial("Metal", "Metal materials");
        var materialTaxToPrm = new Dictionary<int, int>
        {
            { glassTax.Id, prmGlass.Id },
            { plasticTax.Id, prmPlastic.Id },
            { paperTax.Id, prmPaper.Id },
            { metalTax.Id, prmMetal.Id }
        };

        var ptBottle = await EnsurePackagingTypeFromLibrary(wineBottle, materialTaxToPrm);
        var ptCap = await EnsurePackagingTypeFromLibrary(screwCap, materialTaxToPrm);
        var ptLabel = await EnsurePackagingTypeFromLibrary(wineLabel, materialTaxToPrm);
        var ptFoil = await EnsurePackagingTypeFromLibrary(neckFoil, materialTaxToPrm);
        var ptBox = await EnsurePackagingTypeFromLibrary(wineBox, materialTaxToPrm);
        var ptCase = await EnsurePackagingTypeFromLibrary(shippingCase, materialTaxToPrm);

        var shippingLibs = new[] { shippingCase, wineBox };
        var productLibs = new[] { wineBottle, screwCap, wineLabel, neckFoil, wineBox };
        var libToPt = new Dictionary<int, PackagingType>
        {
            { wineBottle.Id, ptBottle }, { screwCap.Id, ptCap }, { wineLabel.Id, ptLabel },
            { neckFoil.Id, ptFoil }, { wineBox.Id, ptBox }, { shippingCase.Id, ptCase }
        };
        var puShipping = await EnsurePackagingUnitFromGroup(groupShipping, shippingLibs, libToPt);
        var puProduct = await EnsurePackagingUnitFromGroup(groupProduct, productLibs, libToPt);

        var auJurisdiction = await EnsureJurisdiction("AU", "Australia", "AU");
        var sydneyGeo = await EnsureGeography("SYD", "Sydney", auJurisdiction.Id);
        var melbourneGeo = await EnsureGeography("MEL", "Melbourne", auJurisdiction.Id);
        var brisbaneGeo = await EnsureGeography("BNE", "Brisbane", auJurisdiction.Id);
        var ukJurisdiction = await EnsureJurisdiction("UK", "United Kingdom", "GB");
        var londonGeo = await EnsureGeography("LON", "London", ukJurisdiction.Id);
        var manchesterGeo = await EnsureGeography("MAN", "Manchester", ukJurisdiction.Id);
        var birminghamGeo = await EnsureGeography("BIR", "Birmingham", ukJurisdiction.Id);

        var products = new[]
        {
            ("ALC-001", "Merlot Reserve 2022", "Vineyard Estates", "50602234567890", "https://picsum.photos/seed/alc-merlot/200/200"),
            ("ALC-002", "Chardonnay Premium", "Vineyard Estates", "50602234567891", "https://picsum.photos/seed/alc-chardonnay/200/200"),
            ("ALC-003", "IPA Craft Beer 330ml", "BrewMaster", "50602234567892", "https://picsum.photos/seed/alc-ipa/200/200"),
            ("ALC-004", "Lager Classic 500ml", "BrewMaster", "50602234567893", "https://picsum.photos/seed/alc-lager/200/200"),
            ("ALC-005", "Gin London Dry", "SpiritCraft", "50602234567894", "https://picsum.photos/seed/alc-gin/200/200"),
            ("ALC-006", "Vodka Premium 700ml", "SpiritCraft", "50602234567895", "https://picsum.photos/seed/alc-vodka/200/200"),
            ("ALC-007", "Whisky Single Malt 12yr", "Highland Spirit", "50602234567896", "https://picsum.photos/seed/alc-whisky/200/200"),
            ("ALC-008", "Rosé Summer Blend", "Vineyard Estates", "50602234567897", "https://picsum.photos/seed/alc-rose/200/200"),
            ("ALC-009", "Prosecco DOC", "Italian Bubbles", "50602234567898", "https://picsum.photos/seed/alc-prosecco/200/200"),
            ("ALC-010", "Rum Caribbean Gold", "SpiritCraft", "50602234567899", "https://picsum.photos/seed/alc-rum/200/200"),
            ("ALC-011", "Cider Apple Crisp", "BrewMaster", "50602234567900", "https://picsum.photos/seed/alc-cider/200/200"),
            ("ALC-012", "Sauvignon Blanc", "Vineyard Estates", "50602234567901", "https://picsum.photos/seed/alc-sauvignon/200/200"),
            ("ALC-013", "Tequila Silver", "SpiritCraft", "50602234567902", "https://picsum.photos/seed/alc-tequila/200/200"),
            ("ALC-014", "Stout Dark Ale", "BrewMaster", "50602234567903", "https://picsum.photos/seed/alc-stout/200/200"),
            ("ALC-015", "Pinot Noir 2021", "Vineyard Estates", "50602234567904", "https://picsum.photos/seed/alc-pinot/200/200"),
            ("ALC-016", "Brandy VSOP", "SpiritCraft", "50602234567905", "https://picsum.photos/seed/alc-brandy/200/200"),
            ("ALC-017", "Champagne Brut", "Italian Bubbles", "50602234567906", "https://picsum.photos/seed/alc-champagne/200/200"),
            ("ALC-018", "Pilsner Lager 440ml", "BrewMaster", "50602234567907", "https://picsum.photos/seed/alc-pilsner/200/200"),
            ("ALC-019", "Port Ruby Reserve", "Vineyard Estates", "50602234567908", "https://picsum.photos/seed/alc-port/200/200"),
            ("ALC-020", "Bourbon American", "Highland Spirit", "50602234567909", "https://picsum.photos/seed/alc-bourbon/200/200"),
        };

        var supplierProductIds = new[] { spBottle.Id, spCap.Id, spLabel.Id, spFoil.Id, spBox.Id, spCase.Id };
        var productEntities = new List<Product>();
        for (var idx = 0; idx < products.Length; idx++)
        {
            var (sku, name, brand, gtin, imageUrl) = products[idx];
            var isAustralian = idx < 16;
            var p = await EnsureProduct(sku, name, brand, gtin, DatasetKey, imageUrl, isAustralian ? "AU" : "FR");
            productEntities.Add(p);

            await EnsureProductForm(p.Id, gtin, name, brand, "Alcoholic Beverages", "Wines & Spirits", isAustralian ? "AU" : "FR");
            await EnsureProductPackaging(p.Id, puShipping.Id);
            await EnsureProductPackaging(p.Id, puProduct.Id);

            var spId = supplierProductIds[idx % supplierProductIds.Length];
            await EnsureProductPackagingSupplierProduct(p.Id, spId);
        }

        var rnd = new Random(43);
        var auGeos = new[] { (sydneyGeo, "Sydney", "Australia", auJurisdiction.Id), (melbourneGeo, "Melbourne", "Australia", auJurisdiction.Id), (brisbaneGeo, "Brisbane", "Australia", auJurisdiction.Id) };
        var ukGeos = new[] { (londonGeo, "London", "UK", ukJurisdiction.Id), (manchesterGeo, "Manchester", "UK", ukJurisdiction.Id), (birminghamGeo, "Birmingham", "UK", ukJurisdiction.Id) };
        for (var i = 0; i < productEntities.Count; i++)
        {
            var p = productEntities[i];
            var (geo, cityName, country, jurisdictionId) = i < 16 ? auGeos[rnd.Next(auGeos.Length)] : ukGeos[rnd.Next(ukGeos.Length)];
            await EnsureDistribution(p.Id, puShipping.Id, rnd.Next(30, 400), "Liquor Store", cityName, country, geo.Id, jurisdictionId);
        }

        var baseDate = DateTime.UtcNow.AddDays(-14);
        var auReceivers = new[] { ("9390987654321", "WineDirect Sydney", "Sydney"), ("9390987654322", "SpiritHub Melbourne", "Melbourne"), ("9390987654323", "BrewDepot Brisbane", "Brisbane") };
        var ukReceivers = new[] { ("5060987654321", "WineDirect London", "London"), ("5060987654322", "SpiritHub Manchester", "Manchester"), ("5060987654323", "BrewDepot Birmingham", "Birmingham") };
        var auShippers = new[] { ("9390987654324", "BeveragePack Australia"), ("9390987654325", "Vineyard Logistics AU"), ("9390987654326", "SpiritCraft Distribution AU") };
        var ukShippers = new[] { ("5060987654324", "BeveragePack Ltd"), ("5060987654325", "Vineyard Logistics"), ("5060987654326", "SpiritCraft Distribution") };
        int lineNum = 1;
        for (int i = 0; i < 12; i++)
        {
            var isAustralian = i < 10;
            var (recGln, recName, recCity) = isAustralian ? auReceivers[i % auReceivers.Length] : ukReceivers[i % ukReceivers.Length];
            var (shipGln, shipName) = isAustralian ? auShippers[i % auShippers.Length] : ukShippers[i % ukShippers.Length];
            var ship = await EnsureAsnShipment($"ASN-ALC-{1000 + i}", shipGln, shipName, recGln, recName, baseDate.AddDays(i * 2), DatasetKey, isAustralian ? "Sydney" : "Manchester", isAustralian ? "AU" : "GB");
            var palletsPerShipment = 1 + (i % 3);
            for (int p = 0; p < palletsPerShipment; p++)
            {
                var pallet = await EnsureAsnPallet(ship.Id, $"3579223456789012{(i * 3 + p):D2}", recName, recCity, isAustralian ? "AU" : "GB", p + 1, recGln);
                var productsPerPallet = 2 + (i + p) % 4;
                for (int li = 0; li < productsPerPallet; li++)
                {
                    var prod = productEntities[(i * 3 + p * 2 + li) % productEntities.Count];
                    await EnsureAsnLineItem(pallet.Id, prod.Gtin!, prod.Name, 6 + (i + li) % 12, lineNum++);
                }
            }
        }

        await _context.SaveChangesAsync();
        Console.WriteLine($"✓ Alcoholic Beverages dataset seeded: 20 products, packaging, distribution, ASNs (80% AU, 20% international)");
    }

    private async Task EnsureAsnsAsync()
    {
        var productEntities = await _context.Products
            .Where(p => p.DatasetKey == DatasetKey && p.Gtin != null)
            .OrderBy(p => p.Sku)
            .ToListAsync();
        if (productEntities.Count == 0) return;

        var baseDate = DateTime.UtcNow.AddDays(-14);
        var auReceivers = new[] { ("9390987654321", "WineDirect Sydney", "Sydney"), ("9390987654322", "SpiritHub Melbourne", "Melbourne"), ("9390987654323", "BrewDepot Brisbane", "Brisbane") };
        var ukReceivers = new[] { ("5060987654321", "WineDirect London", "London"), ("5060987654322", "SpiritHub Manchester", "Manchester"), ("5060987654323", "BrewDepot Birmingham", "Birmingham") };
        var auShippers = new[] { ("9390987654324", "BeveragePack Australia"), ("9390987654325", "Vineyard Logistics AU"), ("9390987654326", "SpiritCraft Distribution AU") };
        var ukShippers = new[] { ("5060987654324", "BeveragePack Ltd"), ("5060987654325", "Vineyard Logistics"), ("5060987654326", "SpiritCraft Distribution") };
        int lineNum = 1;
        for (int i = 0; i < 12; i++)
        {
            var isAustralian = i < 10;
            var (recGln, recName, recCity) = isAustralian ? auReceivers[i % auReceivers.Length] : ukReceivers[i % ukReceivers.Length];
            var (shipGln, shipName) = isAustralian ? auShippers[i % auShippers.Length] : ukShippers[i % ukShippers.Length];
            var ship = await EnsureAsnShipment($"ASN-ALC-{1000 + i}", shipGln, shipName, recGln, recName, baseDate.AddDays(i * 2), DatasetKey, isAustralian ? "Sydney" : "Manchester", isAustralian ? "AU" : "GB");
            var palletsPerShipment = 1 + (i % 3);
            for (int p = 0; p < palletsPerShipment; p++)
            {
                var pallet = await EnsureAsnPallet(ship.Id, $"3579223456789012{(i * 3 + p):D2}", recName, recCity, isAustralian ? "AU" : "GB", p + 1, recGln);
                var productsPerPallet = 2 + (i + p) % 4;
                for (int li = 0; li < productsPerPallet; li++)
                {
                    var prod = productEntities[(i * 3 + p * 2 + li) % productEntities.Count];
                    await EnsureAsnLineItem(pallet.Id, prod.Gtin!, prod.Name, 6 + (i + li) % 12, lineNum++);
                }
            }
        }
        await _context.SaveChangesAsync();
    }

    private async Task UpdateProductImagesIfMissing()
    {
        var productImages = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        for (int i = 1; i <= 20; i++)
        {
            var sku = $"ALC-{i:D3}";
            var seeds = new[] { "alc-merlot", "alc-chardonnay", "alc-ipa", "alc-lager", "alc-gin", "alc-vodka", "alc-whisky", "alc-rose", "alc-prosecco", "alc-rum", "alc-cider", "alc-sauvignon", "alc-tequila", "alc-stout", "alc-pinot", "alc-brandy", "alc-champagne", "alc-pilsner", "alc-port", "alc-bourbon" };
            productImages[sku] = $"https://picsum.photos/seed/{seeds[(i - 1) % seeds.Length]}/200/200";
        }
        var products = await _context.Products.Where(p => p.DatasetKey == DatasetKey && (p.ImageUrl == null || p.ImageUrl == "")).ToListAsync();
        foreach (var p in products)
        {
            if (p.Sku != null && productImages.TryGetValue(p.Sku, out var url))
                p.ImageUrl = url;
        }
        if (products.Count > 0) await _context.SaveChangesAsync();
    }

    private async Task<MaterialTaxonomy> EnsureMaterialTaxonomy(string code, string displayName, int level)
    {
        var t = await _context.MaterialTaxonomies.FirstOrDefaultAsync(m => m.Code == code);
        if (t == null)
        {
            t = new MaterialTaxonomy { Code = code, DisplayName = displayName, Level = level, IsActive = true, SortOrder = 0 };
            _context.MaterialTaxonomies.Add(t);
            await _context.SaveChangesAsync();
        }
        return t;
    }

    private async Task<PackagingSupplier> EnsureSupplier(string name, string address, string city, string country, string email)
    {
        var s = await _context.PackagingSuppliers.FirstOrDefaultAsync(x => x.Name == name);
        if (s == null)
        {
            s = new PackagingSupplier { Name = name, Address = address, City = city, Country = country, Email = email, IsActive = true };
            _context.PackagingSuppliers.Add(s);
            await _context.SaveChangesAsync();
        }
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
        if (sp == null)
        {
            sp = new PackagingSupplierProduct { PackagingSupplierId = supplierId, Name = name, ProductCode = productCode, TaxonomyCode = productCode };
            _context.PackagingSupplierProducts.Add(sp);
            await _context.SaveChangesAsync();
        }
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
        if (pt == null)
        {
            pt = new PackagingType { Name = lib.Name, Weight = lib.Weight, IsFromLibrary = true, LibrarySource = lib.TaxonomyCode ?? "PackagingLibrary", Notes = $"Linked from PackagingLibrary (Id={lib.Id})" };
            _context.PackagingTypes.Add(pt);
            await _context.SaveChangesAsync();
        }
        if (lib.MaterialTaxonomyId.HasValue && materialTaxonomyIdToPrmId.TryGetValue(lib.MaterialTaxonomyId.Value, out var prmId))
        {
            if (!await _context.PackagingTypeMaterials.AnyAsync(ptm => ptm.PackagingTypeId == pt.Id && ptm.MaterialId == prmId))
            {
                _context.PackagingTypeMaterials.Add(new PackagingTypeMaterial { PackagingTypeId = pt.Id, MaterialId = prmId });
                await _context.SaveChangesAsync();
            }
        }
        return pt;
    }

    private async Task<PackagingUnit> EnsurePackagingUnitFromGroup(PackagingGroup group, PackagingLibrary[] libs, Dictionary<int, PackagingType> libIdToPackagingType)
    {
        var pu = await _context.PackagingUnits.FirstOrDefaultAsync(u => u.Name == group.Name);
        if (pu == null)
        {
            pu = new PackagingUnit { Name = group.Name, UnitLevel = group.PackagingLayer ?? "Primary", Notes = $"Linked from PackagingGroup {group.PackId}" };
            _context.PackagingUnits.Add(pu);
            await _context.SaveChangesAsync();
        }
        foreach (var lib in libs)
        {
            if (!libIdToPackagingType.TryGetValue(lib.Id, out var pt)) continue;
            if (await _context.PackagingUnitItems.AnyAsync(pui => pui.PackagingUnitId == pu.Id && pui.PackagingTypeId == pt.Id)) continue;
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

    private async Task<Product> EnsureProduct(string sku, string name, string brand, string gtin, string datasetKey, string? imageUrl = null, string? countryOfOrigin = "AU")
    {
        var p = await _context.Products.FirstOrDefaultAsync(x => x.Sku == sku);
        if (p == null)
        {
            p = new Product { Sku = sku, Name = name, Brand = brand, Gtin = gtin, ProductCategory = "Alcoholic Beverages", ProductSubCategory = "Wines & Spirits", CountryOfOrigin = countryOfOrigin ?? "AU", DatasetKey = datasetKey, ImageUrl = imageUrl };
            _context.Products.Add(p);
            await _context.SaveChangesAsync();
        }
        else if (!string.IsNullOrEmpty(imageUrl) && string.IsNullOrEmpty(p.ImageUrl)) { p.ImageUrl = imageUrl; await _context.SaveChangesAsync(); }
        return p;
    }

    private async Task EnsureProductForm(int productId, string gtin, string productName, string brand, string category, string subCategory, string? countryOfOrigin = "AU")
    {
        if (await _context.ProductForms.AnyAsync(pf => pf.ProductId == productId)) return;
        _context.ProductForms.Add(new ProductForm { ProductId = productId, Gtin = gtin, ProductName = productName, Brand = brand, ProductCategory = category, ProductSubCategory = subCategory, CountryOfOrigin = countryOfOrigin ?? "AU", PackagingLevel = "Consumer Unit", PackagingType = "Bottle", PackagingConfiguration = "Single component", Status = "submitted" });
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

    private async Task<AsnShipment> EnsureAsnShipment(string asnNumber, string shipperGln, string shipperName, string receiverGln, string receiverName, DateTime shipDate, string datasetKey, string? shipperCity = "Sydney", string? shipperCountryCode = "AU")
    {
        var s = new AsnShipment { AsnNumber = asnNumber, ShipperGln = shipperGln, ShipperName = shipperName, ShipperCity = shipperCity ?? "Sydney", ShipperCountryCode = shipperCountryCode ?? "AU", ReceiverGln = receiverGln, ReceiverName = receiverName, ShipDate = shipDate, DeliveryDate = shipDate.AddDays(1), CarrierName = "FastFreight AU", TransportMode = "ROAD", TotalPackages = 1, TotalWeight = 25.5m, SourceFormat = "GS1_XML", Status = "DELIVERED", DatasetKey = datasetKey };
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
