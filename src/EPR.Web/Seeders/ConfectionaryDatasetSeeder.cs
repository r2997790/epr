using EPR.Domain.Entities;
using EPR.Data;
using Microsoft.EntityFrameworkCore;

namespace EPR.Web.Seeders;

/// <summary>
/// Seeds a comprehensive Confectionary dataset: 20 products with packaging, raw materials,
/// packaging groups, suppliers, distribution, and ASN data. Full traceability chain.
/// </summary>
public class ConfectionaryDatasetSeeder
{
    private const string DatasetKey = "Confectionary";
    private readonly EPRDbContext _context;

    public ConfectionaryDatasetSeeder(EPRDbContext context)
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
            Console.WriteLine("Confectionary dataset verified (images updated if needed)");
            return;
        }

        if (existingProductCount > 0 && existingAsnCount == 0)
        {
            await EnsureAsnsAsync();
            Console.WriteLine("✓ Confectionary ASNs added (linked to existing products)");
            return;
        }

        var plasticTax = await EnsureMaterialTaxonomy("PLASTIC", "Plastics", 1);
        var paperTax = await EnsureMaterialTaxonomy("PAPER", "Paper & Cardboard", 1);
        var foilTax = await EnsureMaterialTaxonomy("FOIL", "Aluminium Foil", 1);

        var supplierBoxes = await EnsureSupplier("SweetPack Solutions", "15 Candy Lane", "Manchester", "UK", "sales@sweetpack.co.uk");
        var supplierWrappers = await EnsureSupplier("WrapperTech UK", "7 Wrap Street", "Leeds", "UK", "info@wrappertech.co.uk");
        var supplierLabels = await EnsureSupplier("LabelCandy Ltd", "3 Print Road", "Birmingham", "UK", "orders@labelcandy.co.uk");

        var chocolateBox = await EnsurePackagingLibrary("Chocolate gift box 250g", "CONF-BOX-001", 85m, paperTax.Id, DatasetKey);
        var plasticTray = await EnsurePackagingLibrary("Plastic chocolate tray", "CONF-TRAY-001", 25m, plasticTax.Id, DatasetKey);
        var foilWrapper = await EnsurePackagingLibrary("Foil chocolate wrapper", "CONF-WRAP-001", 3m, foilTax.Id, DatasetKey);
        var productLabel = await EnsurePackagingLibrary("Product label", "CONF-LABEL-001", 2m, paperTax.Id, DatasetKey);
        var shippingBox = await EnsurePackagingLibrary("Confectionary shipping box", "CONF-SHIP-001", 400m, paperTax.Id, DatasetKey);
        var innerBags = await EnsurePackagingLibrary("Inner plastic bags", "CONF-BAG-001", 15m, plasticTax.Id, DatasetKey);

        await LinkPackagingMaterial(chocolateBox.Id, paperTax.Id);
        await LinkPackagingMaterial(plasticTray.Id, plasticTax.Id);
        await LinkPackagingMaterial(foilWrapper.Id, foilTax.Id);
        await LinkPackagingMaterial(productLabel.Id, paperTax.Id);
        await LinkPackagingMaterial(shippingBox.Id, paperTax.Id);
        await LinkPackagingMaterial(innerBags.Id, plasticTax.Id);

        var spBox = await EnsureSupplierProduct(supplierBoxes.Id, "Chocolate Gift Box 250g", "CONF-BOX-001");
        var spTray = await EnsureSupplierProduct(supplierBoxes.Id, "Plastic Tray 12-Cavity", "CONF-TRAY-001");
        var spWrap = await EnsureSupplierProduct(supplierWrappers.Id, "Foil Wrapper 50x80", "CONF-WRAP-001");
        var spLabel = await EnsureSupplierProduct(supplierLabels.Id, "Confectionary Label", "CONF-LABEL-001");
        var spShip = await EnsureSupplierProduct(supplierBoxes.Id, "Shipping Box 24pk", "CONF-SHIP-001");
        var spBag = await EnsureSupplierProduct(supplierWrappers.Id, "Inner Bags 100pk", "CONF-BAG-001");

        await LinkPackagingSupplier(chocolateBox.Id, spBox.Id, true);
        await LinkPackagingSupplier(plasticTray.Id, spTray.Id, true);
        await LinkPackagingSupplier(foilWrapper.Id, spWrap.Id, true);
        await LinkPackagingSupplier(productLabel.Id, spLabel.Id, true);
        await LinkPackagingSupplier(shippingBox.Id, spShip.Id, true);
        await LinkPackagingSupplier(innerBags.Id, spBag.Id, true);

        var groupShipping = await EnsurePackagingGroup("CONF-SHIP-001", "Confectionary Shipping Pack", "Secondary", 415m, DatasetKey);
        await AddGroupItem(groupShipping.Id, shippingBox.Id, 0);
        await AddGroupItem(groupShipping.Id, innerBags.Id, 1);

        var groupProduct = await EnsurePackagingGroup("CONF-PROD-001", "Confectionary Product Pack", "Primary", 115m, DatasetKey);
        await AddGroupItem(groupProduct.Id, chocolateBox.Id, 0);
        await AddGroupItem(groupProduct.Id, plasticTray.Id, 1);
        await AddGroupItem(groupProduct.Id, foilWrapper.Id, 2);
        await AddGroupItem(groupProduct.Id, productLabel.Id, 3);

        var prmPlastic = await EnsurePackagingRawMaterial("Plastics", "Plastic materials");
        var prmPaper = await EnsurePackagingRawMaterial("Paper & Cardboard", "Paper and cardboard materials");
        var prmFoil = await EnsurePackagingRawMaterial("Aluminium Foil", "Aluminium foil materials");
        var materialTaxToPrm = new Dictionary<int, int>
        {
            { plasticTax.Id, prmPlastic.Id },
            { paperTax.Id, prmPaper.Id },
            { foilTax.Id, prmFoil.Id }
        };

        var ptBox = await EnsurePackagingTypeFromLibrary(chocolateBox, materialTaxToPrm);
        var ptTray = await EnsurePackagingTypeFromLibrary(plasticTray, materialTaxToPrm);
        var ptWrap = await EnsurePackagingTypeFromLibrary(foilWrapper, materialTaxToPrm);
        var ptLabel = await EnsurePackagingTypeFromLibrary(productLabel, materialTaxToPrm);
        var ptShip = await EnsurePackagingTypeFromLibrary(shippingBox, materialTaxToPrm);
        var ptBag = await EnsurePackagingTypeFromLibrary(innerBags, materialTaxToPrm);

        var shippingLibs = new[] { shippingBox, innerBags };
        var productLibs = new[] { chocolateBox, plasticTray, foilWrapper, productLabel };
        var libToPt = new Dictionary<int, PackagingType>
        {
            { chocolateBox.Id, ptBox }, { plasticTray.Id, ptTray }, { foilWrapper.Id, ptWrap },
            { productLabel.Id, ptLabel }, { shippingBox.Id, ptShip }, { innerBags.Id, ptBag }
        };
        var puShipping = await EnsurePackagingUnitFromGroup(groupShipping, shippingLibs, libToPt);
        var puProduct = await EnsurePackagingUnitFromGroup(groupProduct, productLibs, libToPt);

        var ukJurisdiction = await EnsureJurisdiction("UK", "United Kingdom", "GB");
        var londonGeo = await EnsureGeography("LON", "London", ukJurisdiction.Id);
        var manchesterGeo = await EnsureGeography("MAN", "Manchester", ukJurisdiction.Id);
        var birminghamGeo = await EnsureGeography("BIR", "Birmingham", ukJurisdiction.Id);

        var products = new[]
        {
            ("CONF-001", "Milk Chocolate Bar 100g", "SweetTreats", "50603234567890", "https://picsum.photos/seed/conf-milk/200/200"),
            ("CONF-002", "Dark Chocolate 70% 85g", "SweetTreats", "50603234567891", "https://picsum.photos/seed/conf-dark/200/200"),
            ("CONF-003", "Assorted Truffles 200g", "ChocoLuxe", "50603234567892", "https://picsum.photos/seed/conf-truffles/200/200"),
            ("CONF-004", "Fruit Gummies 150g", "SweetTreats", "50603234567893", "https://picsum.photos/seed/conf-gummies/200/200"),
            ("CONF-005", "Hazelnut Pralines 180g", "ChocoLuxe", "50603234567894", "https://picsum.photos/seed/conf-pralines/200/200"),
            ("CONF-006", "Caramel Toffees 120g", "SweetTreats", "50603234567895", "https://picsum.photos/seed/conf-toffees/200/200"),
            ("CONF-007", "White Chocolate Bar 90g", "SweetTreats", "50603234567896", "https://picsum.photos/seed/conf-white/200/200"),
            ("CONF-008", "Mint Chocolates 100g", "ChocoLuxe", "50603234567897", "https://picsum.photos/seed/conf-mint/200/200"),
            ("CONF-009", "Jelly Beans 200g", "SweetTreats", "50603234567898", "https://picsum.photos/seed/conf-jelly/200/200"),
            ("CONF-010", "Orange Chocolate 80g", "ChocoLuxe", "50603234567899", "https://picsum.photos/seed/conf-orange/200/200"),
            ("CONF-011", "Liquorice Allsorts 175g", "SweetTreats", "50603234567900", "https://picsum.photos/seed/conf-liquorice/200/200"),
            ("CONF-012", "Sea Salt Caramel 95g", "ChocoLuxe", "50603234567901", "https://picsum.photos/seed/conf-caramel/200/200"),
            ("CONF-013", "Sour Candies 130g", "SweetTreats", "50603234567902", "https://picsum.photos/seed/conf-sour/200/200"),
            ("CONF-014", "Almond Chocolate 110g", "ChocoLuxe", "50603234567903", "https://picsum.photos/seed/conf-almond/200/200"),
            ("CONF-015", "Strawberry Creams 140g", "SweetTreats", "50603234567904", "https://picsum.photos/seed/conf-strawberry/200/200"),
            ("CONF-016", "Raspberry Dark Choc 88g", "ChocoLuxe", "50603234567905", "https://picsum.photos/seed/conf-raspberry/200/200"),
            ("CONF-017", "Peanut Brittle 160g", "SweetTreats", "50603234567906", "https://picsum.photos/seed/conf-brittle/200/200"),
            ("CONF-018", "Coffee Chocolates 105g", "ChocoLuxe", "50603234567907", "https://picsum.photos/seed/conf-coffee/200/200"),
            ("CONF-019", "Cola Bottles 125g", "SweetTreats", "50603234567908", "https://picsum.photos/seed/conf-cola/200/200"),
            ("CONF-020", "Gianduja Hazelnut 92g", "ChocoLuxe", "50603234567909", "https://picsum.photos/seed/conf-gianduja/200/200"),
        };

        var supplierProductIds = new[] { spBox.Id, spTray.Id, spWrap.Id, spLabel.Id, spShip.Id, spBag.Id };
        var productEntities = new List<Product>();
        for (var idx = 0; idx < products.Length; idx++)
        {
            var (sku, name, brand, gtin, imageUrl) = products[idx];
            var p = await EnsureProduct(sku, name, brand, gtin, DatasetKey, imageUrl);
            productEntities.Add(p);

            await EnsureProductForm(p.Id, gtin, name, brand, "Confectionary", "Chocolates & Sweets");
            await EnsureProductPackaging(p.Id, puShipping.Id);
            await EnsureProductPackaging(p.Id, puProduct.Id);

            var spId = supplierProductIds[idx % supplierProductIds.Length];
            await EnsureProductPackagingSupplierProduct(p.Id, spId);
        }

        var rnd = new Random(44);
        var geos = new[] { (londonGeo, "London"), (manchesterGeo, "Manchester"), (birminghamGeo, "Birmingham") };
        foreach (var p in productEntities)
        {
            var (geo, cityName) = geos[rnd.Next(geos.Length)];
            await EnsureDistribution(p.Id, puShipping.Id, rnd.Next(80, 600), "Sweet Shop", cityName, "UK", geo.Id, ukJurisdiction.Id);
        }

        var baseDate = DateTime.UtcNow.AddDays(-14);
        var receivers = new[] { ("5060987654321", "SweetRetail London", "London"), ("5060987654322", "CandyHub Manchester", "Manchester"), ("5060987654323", "ChocoDepot Birmingham", "Birmingham") };
        var shippers = new[] { ("5060987654324", "SweetPack Solutions"), ("5060987654325", "ChocoLuxe Logistics"), ("5060987654326", "SweetTreats Distribution") };
        int lineNum = 1;
        for (int i = 0; i < 12; i++)
        {
            var (recGln, recName, recCity) = receivers[i % receivers.Length];
            var (shipGln, shipName) = shippers[i % shippers.Length];
            var ship = await EnsureAsnShipment($"ASN-CONF-{1000 + i}", shipGln, shipName, recGln, recName, baseDate.AddDays(i * 2), DatasetKey);
            var palletsPerShipment = 1 + (i % 3);
            for (int p = 0; p < palletsPerShipment; p++)
            {
                var pallet = await EnsureAsnPallet(ship.Id, $"3579323456789012{(i * 3 + p):D2}", recName, recCity, "GB", p + 1, recGln);
                var productsPerPallet = 2 + (i + p) % 4;
                for (int li = 0; li < productsPerPallet; li++)
                {
                    var prod = productEntities[(i * 3 + p * 2 + li) % productEntities.Count];
                    await EnsureAsnLineItem(pallet.Id, prod.Gtin!, prod.Name, 24 + (i + li) % 48, lineNum++);
                }
            }
        }

        await _context.SaveChangesAsync();
        Console.WriteLine($"✓ Confectionary dataset seeded: 20 products, packaging, distribution, ASNs");
    }

    private async Task EnsureAsnsAsync()
    {
        var productEntities = await _context.Products
            .Where(p => p.DatasetKey == DatasetKey && p.Gtin != null)
            .OrderBy(p => p.Sku)
            .ToListAsync();
        if (productEntities.Count == 0) return;

        var baseDate = DateTime.UtcNow.AddDays(-14);
        var receivers = new[] { ("5060987654321", "SweetRetail London", "London"), ("5060987654322", "CandyHub Manchester", "Manchester"), ("5060987654323", "ChocoDepot Birmingham", "Birmingham") };
        var shippers = new[] { ("5060987654324", "SweetPack Solutions"), ("5060987654325", "ChocoLuxe Logistics"), ("5060987654326", "SweetTreats Distribution") };
        int lineNum = 1;
        for (int i = 0; i < 12; i++)
        {
            var (recGln, recName, recCity) = receivers[i % receivers.Length];
            var (shipGln, shipName) = shippers[i % shippers.Length];
            var ship = await EnsureAsnShipment($"ASN-CONF-{1000 + i}", shipGln, shipName, recGln, recName, baseDate.AddDays(i * 2), DatasetKey);
            var palletsPerShipment = 1 + (i % 3);
            for (int p = 0; p < palletsPerShipment; p++)
            {
                var pallet = await EnsureAsnPallet(ship.Id, $"3579323456789012{(i * 3 + p):D2}", recName, recCity, "GB", p + 1, recGln);
                var productsPerPallet = 2 + (i + p) % 4;
                for (int li = 0; li < productsPerPallet; li++)
                {
                    var prod = productEntities[(i * 3 + p * 2 + li) % productEntities.Count];
                    await EnsureAsnLineItem(pallet.Id, prod.Gtin!, prod.Name, 24 + (i + li) % 48, lineNum++);
                }
            }
        }
        await _context.SaveChangesAsync();
    }

    private async Task UpdateProductImagesIfMissing()
    {
        var productImages = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        var seeds = new[] { "conf-milk", "conf-dark", "conf-truffles", "conf-gummies", "conf-pralines", "conf-toffees", "conf-white", "conf-mint", "conf-jelly", "conf-orange", "conf-liquorice", "conf-caramel", "conf-sour", "conf-almond", "conf-strawberry", "conf-raspberry", "conf-brittle", "conf-coffee", "conf-cola", "conf-gianduja" };
        for (int i = 1; i <= 20; i++)
            productImages[$"CONF-{i:D3}"] = $"https://picsum.photos/seed/{seeds[(i - 1) % seeds.Length]}/200/200";
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

    private async Task<Product> EnsureProduct(string sku, string name, string brand, string gtin, string datasetKey, string? imageUrl = null)
    {
        var p = await _context.Products.FirstOrDefaultAsync(x => x.Sku == sku);
        if (p == null)
        {
            p = new Product { Sku = sku, Name = name, Brand = brand, Gtin = gtin, ProductCategory = "Confectionary", ProductSubCategory = "Chocolates & Sweets", CountryOfOrigin = "BE", DatasetKey = datasetKey, ImageUrl = imageUrl };
            _context.Products.Add(p);
            await _context.SaveChangesAsync();
        }
        else if (!string.IsNullOrEmpty(imageUrl) && string.IsNullOrEmpty(p.ImageUrl)) { p.ImageUrl = imageUrl; await _context.SaveChangesAsync(); }
        return p;
    }

    private async Task EnsureProductForm(int productId, string gtin, string productName, string brand, string category, string subCategory)
    {
        if (await _context.ProductForms.AnyAsync(pf => pf.ProductId == productId)) return;
        _context.ProductForms.Add(new ProductForm { ProductId = productId, Gtin = gtin, ProductName = productName, Brand = brand, ProductCategory = category, ProductSubCategory = subCategory, CountryOfOrigin = "BE", PackagingLevel = "Consumer Unit", PackagingType = "Box", PackagingConfiguration = "Multi-component", Status = "submitted" });
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

    private async Task<AsnShipment> EnsureAsnShipment(string asnNumber, string shipperGln, string shipperName, string receiverGln, string receiverName, DateTime shipDate, string datasetKey)
    {
        var s = new AsnShipment { AsnNumber = asnNumber, ShipperGln = shipperGln, ShipperName = shipperName, ShipperCity = "Manchester", ShipperCountryCode = "GB", ReceiverGln = receiverGln, ReceiverName = receiverName, ShipDate = shipDate, DeliveryDate = shipDate.AddDays(1), CarrierName = "FastFreight UK", TransportMode = "ROAD", TotalPackages = 1, TotalWeight = 25.5m, SourceFormat = "GS1_XML", Status = "DELIVERED", DatasetKey = datasetKey };
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
