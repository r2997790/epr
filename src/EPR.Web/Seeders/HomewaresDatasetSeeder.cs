using EPR.Domain.Entities;
using EPR.Data;
using Microsoft.EntityFrameworkCore;

namespace EPR.Web.Seeders;

/// <summary>
/// Seeds Homewares dataset: 20 products with packaging, raw materials, suppliers,
/// distribution, and ASN data. Australian addresses (80%), 20% from outside Australia.
/// </summary>
public class HomewaresDatasetSeeder
{
    private const string DatasetKey = "Homewares";
    private readonly EPRDbContext _context;

    public HomewaresDatasetSeeder(EPRDbContext context) => _context = context;

    public async Task SeedAsync()
    {
        var existingProductCount = await _context.Products.CountAsync(p => p.DatasetKey == DatasetKey);
        var existingAsnCount = await _context.AsnShipments.CountAsync(s => s.DatasetKey == DatasetKey);

        if (existingProductCount > 0 && existingAsnCount > 0) { await UpdateProductImagesIfMissing(); return; }
        if (existingProductCount > 0 && existingAsnCount == 0) { await EnsureAsnsAsync(); return; }

        var plasticTax = await EnsureMaterialTaxonomy("PLASTIC", "Plastics", 1);
        var paperTax = await EnsureMaterialTaxonomy("PAPER", "Paper & Cardboard", 1);

        // 80% Australian (4), 20% non-AU (1 from NZ)
        var supplierBoxes = await EnsureSupplier("HomePack Sydney", "32 Home Lane", "Sydney", "Australia", "sales@homepack.com.au");
        var supplierCushions = await EnsureSupplier("CushionCo Melbourne", "11 Fabric Street", "Melbourne", "Australia", "orders@cushionco.com.au");
        var supplierLabels = await EnsureSupplier("HomeLabels Brisbane", "6 Print Road", "Brisbane", "Australia", "info@homelabels.com.au");
        var supplierVases = await EnsureSupplier("GlassHome Perth", "19 Vase Way", "Perth", "Australia", "sales@glasshome.com.au");
        var supplierNz = await EnsureSupplier("KiwiHome Auckland", "4 Export Drive", "Auckland", "New Zealand", "orders@kiwihome.co.nz");

        var giftBox = await EnsurePackagingLibrary("Homewares gift box", "HW-BOX-001", 95m, paperTax.Id, DatasetKey);
        var cushionBag = await EnsurePackagingLibrary("Cushion dust bag", "HW-BAG-001", 25m, plasticTax.Id, DatasetKey);
        var vaseWrap = await EnsurePackagingLibrary("Vase wrap tissue", "HW-WRAP-001", 15m, paperTax.Id, DatasetKey);
        var label = await EnsurePackagingLibrary("Product label", "HW-LABEL-001", 3m, paperTax.Id, DatasetKey);
        var shippingBox = await EnsurePackagingLibrary("Homewares shipping box", "HW-SHIP-001", 380m, paperTax.Id, DatasetKey);

        await LinkPackagingMaterial(giftBox.Id, paperTax.Id);
        await LinkPackagingMaterial(cushionBag.Id, plasticTax.Id);
        await LinkPackagingMaterial(vaseWrap.Id, paperTax.Id);
        await LinkPackagingMaterial(label.Id, paperTax.Id);
        await LinkPackagingMaterial(shippingBox.Id, paperTax.Id);

        var spBox = await EnsureSupplierProduct(supplierBoxes.Id, "Gift Box A4", "HW-BOX-001");
        var spBag = await EnsureSupplierProduct(supplierCushions.Id, "Dust Bag 40x40", "HW-BAG-001");
        var spWrap = await EnsureSupplierProduct(supplierBoxes.Id, "Tissue Wrap", "HW-WRAP-001");
        var spLabel = await EnsureSupplierProduct(supplierLabels.Id, "Homewares Label", "HW-LABEL-001");
        var spShip = await EnsureSupplierProduct(supplierBoxes.Id, "Shipping Box 24pk", "HW-SHIP-001");

        await LinkPackagingSupplier(giftBox.Id, spBox.Id, true);
        await LinkPackagingSupplier(cushionBag.Id, spBag.Id, true);
        await LinkPackagingSupplier(vaseWrap.Id, spWrap.Id, true);
        await LinkPackagingSupplier(label.Id, spLabel.Id, true);
        await LinkPackagingSupplier(shippingBox.Id, spShip.Id, true);

        var groupShipping = await EnsurePackagingGroup("HW-SHIP-001", "Homewares Shipping Pack", "Secondary", 405m, DatasetKey);
        await AddGroupItem(groupShipping.Id, shippingBox.Id, 0);
        await AddGroupItem(groupShipping.Id, cushionBag.Id, 1);

        var groupProduct = await EnsurePackagingGroup("HW-PROD-001", "Homewares Product Pack", "Primary", 138m, DatasetKey);
        await AddGroupItem(groupProduct.Id, giftBox.Id, 0);
        await AddGroupItem(groupProduct.Id, vaseWrap.Id, 1);
        await AddGroupItem(groupProduct.Id, label.Id, 2);

        var prmPlastic = await EnsurePackagingRawMaterial("Plastics", "Plastic materials");
        var prmPaper = await EnsurePackagingRawMaterial("Paper & Cardboard", "Paper and cardboard materials");
        var materialTaxToPrm = new Dictionary<int, int> { { plasticTax.Id, prmPlastic.Id }, { paperTax.Id, prmPaper.Id } };

        var ptBox = await EnsurePackagingTypeFromLibrary(giftBox, materialTaxToPrm);
        var ptBag = await EnsurePackagingTypeFromLibrary(cushionBag, materialTaxToPrm);
        var ptWrap = await EnsurePackagingTypeFromLibrary(vaseWrap, materialTaxToPrm);
        var ptLabel = await EnsurePackagingTypeFromLibrary(label, materialTaxToPrm);
        var ptShip = await EnsurePackagingTypeFromLibrary(shippingBox, materialTaxToPrm);

        var shippingLibs = new[] { shippingBox, cushionBag };
        var productLibs = new[] { giftBox, vaseWrap, label };
        var libToPt = new Dictionary<int, PackagingType> { { giftBox.Id, ptBox }, { cushionBag.Id, ptBag }, { vaseWrap.Id, ptWrap }, { label.Id, ptLabel }, { shippingBox.Id, ptShip } };
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
            ("HW-001", "Ceramic Vase 25cm", "HomeStyle", "50606234567890", "https://picsum.photos/seed/hw-vase/200/200"),
            ("HW-002", "Cushion Cover Linen", "HomeStyle", "50606234567891", "https://picsum.photos/seed/hw-cushion/200/200"),
            ("HW-003", "Candle Holder Glass", "HomeStyle", "50606234567892", "https://picsum.photos/seed/hw-candle/200/200"),
            ("HW-004", "Throw Blanket Wool", "HomeStyle", "50606234567893", "https://picsum.photos/seed/hw-throw/200/200"),
            ("HW-005", "Picture Frame A4", "HomeStyle", "50606234567894", "https://picsum.photos/seed/hw-frame/200/200"),
            ("HW-006", "Decorative Bowl Ceramic", "HomeStyle", "50606234567895", "https://picsum.photos/seed/hw-bowl/200/200"),
            ("HW-007", "Table Runner Linen", "HomeStyle", "50606234567896", "https://picsum.photos/seed/hw-runner/200/200"),
            ("HW-008", "Mirror Round 40cm", "HomeStyle", "50606234567897", "https://picsum.photos/seed/hw-mirror/200/200"),
            ("HW-009", "Coaster Set 4pk", "HomeStyle", "50606234567898", "https://picsum.photos/seed/hw-coaster/200/200"),
            ("HW-010", "Storage Basket Woven", "HomeStyle", "50606234567899", "https://picsum.photos/seed/hw-basket/200/200"),
            ("HW-011", "Scented Candle Lavender", "HomeStyle", "50606234567900", "https://picsum.photos/seed/hw-scented/200/200"),
            ("HW-012", "Placemat Set 4pk", "HomeStyle", "50606234567901", "https://picsum.photos/seed/hw-placemat/200/200"),
            ("HW-013", "Wall Art Print", "HomeStyle", "50606234567902", "https://picsum.photos/seed/hw-wallart/200/200"),
            ("HW-014", "Napkin Ring Set", "HomeStyle", "50606234567903", "https://picsum.photos/seed/hw-napkin/200/200"),
            ("HW-015", "Soap Dispenser", "HomeStyle", "50606234567904", "https://picsum.photos/seed/hw-soap/200/200"),
            ("HW-016", "Towel Set 2pk", "HomeStyle", "50606234567905", "https://picsum.photos/seed/hw-towel/200/200"),
            ("HW-017", "Bookend Pair", "HomeStyle", "50606234567906", "https://picsum.photos/seed/hw-bookend/200/200"),
            ("HW-018", "Trinket Dish", "HomeStyle", "50606234567907", "https://picsum.photos/seed/hw-trinket/200/200"),
            ("HW-019", "Door Mat 60x40", "HomeStyle", "50606234567908", "https://picsum.photos/seed/hw-doormat/200/200"),
            ("HW-020", "Jar Set 3pk", "HomeStyle", "50606234567909", "https://picsum.photos/seed/hw-jar/200/200"),
        };

        var supplierProductIds = new[] { spBox.Id, spBag.Id, spWrap.Id, spLabel.Id, spShip.Id };
        var productEntities = new List<Product>();
        for (var idx = 0; idx < products.Length; idx++)
        {
            var (sku, name, brand, gtin, imageUrl) = products[idx];
            var p = await EnsureProduct(sku, name, brand, gtin, DatasetKey, imageUrl);
            productEntities.Add(p);
            await EnsureProductForm(p.Id, gtin, name, brand, "Homewares", "Home Decor");
            await EnsureProductPackaging(p.Id, puShipping.Id);
            await EnsureProductPackaging(p.Id, puProduct.Id);
            await EnsureProductPackagingSupplierProduct(p.Id, supplierProductIds[idx % supplierProductIds.Length]);
        }

        var rnd = new Random(47);
        var geos = new[] { (sydneyGeo, "Sydney", auJurisdiction.Id, "AU"), (melbourneGeo, "Melbourne", auJurisdiction.Id, "AU"), (brisbaneGeo, "Brisbane", auJurisdiction.Id, "AU"), (aucklandGeo, "Auckland", nzJurisdiction.Id, "NZ") };
        foreach (var p in productEntities)
        {
            var (geo, cityName, jurId, country) = geos[rnd.Next(geos.Length)];
            await EnsureDistribution(p.Id, puShipping.Id, rnd.Next(60, 450), "Home Store", cityName, country, geo.Id, jurId);
        }

        var baseDate = DateTime.UtcNow.AddDays(-14);
        var receivers = new[] { ("5061987654333", "HomeMart Sydney", "Sydney", "AU"), ("5061987654334", "HomeHub Melbourne", "Melbourne", "AU"), ("5061987654335", "KiwiHome Auckland", "Auckland", "NZ") };
        var shippers = new[] { ("5061987654336", "HomePack Sydney", "Sydney", "AU"), ("5061987654337", "CushionCo Melbourne", "Melbourne", "AU"), ("5061987654338", "KiwiHome Auckland", "Auckland", "NZ") };
        int lineNum = 1;
        for (int i = 0; i < 12; i++)
        {
            var (recGln, recName, recCity, recCountry) = receivers[i % receivers.Length];
            var (shipGln, shipName, shipCity, shipCountry) = shippers[i % shippers.Length];
            var ship = await EnsureAsnShipment($"ASN-HW-{1000 + i}", shipGln, shipName, recGln, recName, baseDate.AddDays(i * 2), DatasetKey, shipCity, shipCountry);
            for (int p = 0; p < 1 + (i % 3); p++)
            {
                var pallet = await EnsureAsnPallet(ship.Id, $"3579623456789012{(i * 3 + p):D2}", recName, recCity, recCountry, p + 1, recGln);
                for (int li = 0; li < 2 + (i + p) % 4; li++)
                {
                    var prod = productEntities[(i * 3 + p * 2 + li) % productEntities.Count];
                    await EnsureAsnLineItem(pallet.Id, prod.Gtin!, prod.Name, 8 + (i + li) % 16, lineNum++);
                }
            }
        }

        await _context.SaveChangesAsync();
        Console.WriteLine($"✓ Homewares dataset seeded: 20 products, packaging, distribution, ASNs (AU/NZ)");
    }

    private async Task EnsureAsnsAsync()
    {
        var productEntities = await _context.Products.Where(p => p.DatasetKey == DatasetKey && p.Gtin != null).OrderBy(p => p.Sku).ToListAsync();
        if (productEntities.Count == 0) return;
        var baseDate = DateTime.UtcNow.AddDays(-14);
        var receivers = new[] { ("5061987654333", "HomeMart Sydney", "Sydney", "AU"), ("5061987654334", "HomeHub Melbourne", "Melbourne", "AU"), ("5061987654335", "KiwiHome Auckland", "Auckland", "NZ") };
        var shippers = new[] { ("5061987654336", "HomePack Sydney", "Sydney", "AU"), ("5061987654337", "CushionCo Melbourne", "Melbourne", "AU"), ("5061987654338", "KiwiHome Auckland", "Auckland", "NZ") };
        int lineNum = 1;
        for (int i = 0; i < 12; i++)
        {
            var (recGln, recName, recCity, recCountry) = receivers[i % receivers.Length];
            var (shipGln, shipName, shipCity, shipCountry) = shippers[i % shippers.Length];
            var ship = await EnsureAsnShipment($"ASN-HW-{1000 + i}", shipGln, shipName, recGln, recName, baseDate.AddDays(i * 2), DatasetKey, shipCity, shipCountry);
            for (int p = 0; p < 1 + (i % 3); p++)
            {
                var pallet = await EnsureAsnPallet(ship.Id, $"3579623456789012{(i * 3 + p):D2}", recName, recCity, recCountry, p + 1, recGln);
                for (int li = 0; li < 2 + (i + p) % 4; li++)
                {
                    var prod = productEntities[(i * 3 + p * 2 + li) % productEntities.Count];
                    await EnsureAsnLineItem(pallet.Id, prod.Gtin!, prod.Name, 8 + (i + li) % 16, lineNum++);
                }
            }
        }
        await _context.SaveChangesAsync();
    }

    private async Task UpdateProductImagesIfMissing()
    {
        var seeds = new[] { "hw-vase", "hw-cushion", "hw-candle", "hw-throw", "hw-frame", "hw-bowl", "hw-runner", "hw-mirror", "hw-coaster", "hw-basket", "hw-scented", "hw-placemat", "hw-wallart", "hw-napkin", "hw-soap", "hw-towel", "hw-bookend", "hw-trinket", "hw-doormat", "hw-jar" };
        var products = await _context.Products.Where(p => p.DatasetKey == DatasetKey && (p.ImageUrl == null || p.ImageUrl == "")).ToListAsync();
        foreach (var p in products)
        {
            if (p.Sku != null && p.Sku.StartsWith("HW-") && int.TryParse(p.Sku.AsSpan(3), out var n) && n >= 1 && n <= 20)
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
        if (p == null) { p = new Product { Sku = sku, Name = name, Brand = brand, Gtin = gtin, ProductCategory = "Homewares", ProductSubCategory = "Home Decor", CountryOfOrigin = "AU", DatasetKey = datasetKey, ImageUrl = imageUrl }; _context.Products.Add(p); await _context.SaveChangesAsync(); }
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
