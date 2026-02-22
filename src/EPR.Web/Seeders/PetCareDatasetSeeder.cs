using EPR.Domain.Entities;
using EPR.Data;
using Microsoft.EntityFrameworkCore;

namespace EPR.Web.Seeders;

/// <summary>
/// Seeds Pet Care dataset: 20 products with packaging, raw materials, suppliers,
/// distribution, and ASN data. Australian addresses (80%), 20% from outside Australia.
/// </summary>
public class PetCareDatasetSeeder
{
    private const string DatasetKey = "Pet Care";
    private readonly EPRDbContext _context;

    public PetCareDatasetSeeder(EPRDbContext context) => _context = context;

    public async Task SeedAsync()
    {
        var existingProductCount = await _context.Products.CountAsync(p => p.DatasetKey == DatasetKey);
        var existingAsnCount = await _context.AsnShipments.CountAsync(s => s.DatasetKey == DatasetKey);

        if (existingProductCount > 0 && existingAsnCount > 0) { await UpdateProductImagesIfMissing(); return; }
        if (existingProductCount > 0 && existingAsnCount == 0) { await EnsureAsnsAsync(); return; }

        var plasticTax = await EnsureMaterialTaxonomy("PLASTIC", "Plastics", 1);
        var paperTax = await EnsureMaterialTaxonomy("PAPER", "Paper & Cardboard", 1);

        var supplierBags = await EnsureSupplier("PetPack Sydney", "35 Pet Lane", "Sydney", "Australia", "sales@petpack.com.au");
        var supplierCans = await EnsureSupplier("CanPet Melbourne", "16 Pet Food Rd", "Melbourne", "Australia", "orders@canpet.com.au");
        var supplierLabels = await EnsureSupplier("PetLabels Brisbane", "9 Animal Street", "Brisbane", "Australia", "info@petlabels.com.au");
        var supplierBoxes = await EnsureSupplier("BoxPet Perth", "22 Pet Way", "Perth", "Australia", "sales@boxpet.com.au");
        var supplierNz = await EnsureSupplier("KiwiPet Auckland", "8 Export Drive", "Auckland", "New Zealand", "orders@kiwipet.co.nz");

        var bag = await EnsurePackagingLibrary("Pet food bag 2kg", "PET-BAG-001", 45m, plasticTax.Id, DatasetKey);
        var can = await EnsurePackagingLibrary("Pet food can 400g", "PET-CAN-001", 85m, plasticTax.Id, DatasetKey);
        var box = await EnsurePackagingLibrary("Treat box 12pk", "PET-BOX-001", 120m, paperTax.Id, DatasetKey);
        var label = await EnsurePackagingLibrary("Product label", "PET-LABEL-001", 3m, paperTax.Id, DatasetKey);
        var pouch = await EnsurePackagingLibrary("Wet food pouch 85g", "PET-POUCH-001", 12m, plasticTax.Id, DatasetKey);

        await LinkPackagingMaterial(bag.Id, plasticTax.Id);
        await LinkPackagingMaterial(can.Id, plasticTax.Id);
        await LinkPackagingMaterial(box.Id, paperTax.Id);
        await LinkPackagingMaterial(label.Id, paperTax.Id);
        await LinkPackagingMaterial(pouch.Id, plasticTax.Id);

        var spBag = await EnsureSupplierProduct(supplierBags.Id, "Pet Food Bag 2kg", "PET-BAG-001");
        var spCan = await EnsureSupplierProduct(supplierCans.Id, "Pet Food Can 400g", "PET-CAN-001");
        var spBox = await EnsureSupplierProduct(supplierBoxes.Id, "Treat Box 12pk", "PET-BOX-001");
        var spLabel = await EnsureSupplierProduct(supplierLabels.Id, "Pet Label", "PET-LABEL-001");
        var spPouch = await EnsureSupplierProduct(supplierBags.Id, "Wet Food Pouch 85g", "PET-POUCH-001");

        await LinkPackagingSupplier(bag.Id, spBag.Id, true);
        await LinkPackagingSupplier(can.Id, spCan.Id, true);
        await LinkPackagingSupplier(box.Id, spBox.Id, true);
        await LinkPackagingSupplier(label.Id, spLabel.Id, true);
        await LinkPackagingSupplier(pouch.Id, spPouch.Id, true);

        var groupShipping = await EnsurePackagingGroup("PET-SHIP-001", "Pet Care Shipping Pack", "Secondary", 265m, DatasetKey);
        await AddGroupItem(groupShipping.Id, box.Id, 0);
        await AddGroupItem(groupShipping.Id, bag.Id, 1);

        var groupProduct = await EnsurePackagingGroup("PET-PROD-001", "Pet Care Product Pack", "Primary", 265m, DatasetKey);
        await AddGroupItem(groupProduct.Id, bag.Id, 0);
        await AddGroupItem(groupProduct.Id, can.Id, 1);
        await AddGroupItem(groupProduct.Id, pouch.Id, 2);
        await AddGroupItem(groupProduct.Id, label.Id, 3);

        var prmPlastic = await EnsurePackagingRawMaterial("Plastics", "Plastic materials");
        var prmPaper = await EnsurePackagingRawMaterial("Paper & Cardboard", "Paper and cardboard materials");
        var materialTaxToPrm = new Dictionary<int, int> { { plasticTax.Id, prmPlastic.Id }, { paperTax.Id, prmPaper.Id } };

        var ptBag = await EnsurePackagingTypeFromLibrary(bag, materialTaxToPrm);
        var ptCan = await EnsurePackagingTypeFromLibrary(can, materialTaxToPrm);
        var ptBox = await EnsurePackagingTypeFromLibrary(box, materialTaxToPrm);
        var ptLabel = await EnsurePackagingTypeFromLibrary(label, materialTaxToPrm);
        var ptPouch = await EnsurePackagingTypeFromLibrary(pouch, materialTaxToPrm);

        var shippingLibs = new[] { box, bag };
        var productLibs = new[] { bag, can, pouch, label };
        var libToPt = new Dictionary<int, PackagingType> { { bag.Id, ptBag }, { can.Id, ptCan }, { box.Id, ptBox }, { label.Id, ptLabel }, { pouch.Id, ptPouch } };
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
            ("PET-001", "Dry Dog Food Chicken 2kg", "PetPure", "50608234567890", "https://picsum.photos/seed/pet-dogfood/200/200"),
            ("PET-002", "Dry Cat Food Salmon 1.5kg", "PetPure", "50608234567891", "https://picsum.photos/seed/pet-catfood/200/200"),
            ("PET-003", "Dog Treats Chicken 200g", "PetPure", "50608234567892", "https://picsum.photos/seed/pet-dogtreats/200/200"),
            ("PET-004", "Wet Dog Food Beef 400g", "PetPure", "50608234567893", "https://picsum.photos/seed/pet-wetdog/200/200"),
            ("PET-005", "Wet Cat Food Tuna 85g", "PetPure", "50608234567894", "https://picsum.photos/seed/pet-wetcat/200/200"),
            ("PET-006", "Puppy Food 2kg", "PetPure", "50608234567895", "https://picsum.photos/seed/pet-puppy/200/200"),
            ("PET-007", "Kitten Food 1kg", "PetPure", "50608234567896", "https://picsum.photos/seed/pet-kitten/200/200"),
            ("PET-008", "Dental Sticks Dog 6pk", "PetPure", "50608234567897", "https://picsum.photos/seed/pet-dental/200/200"),
            ("PET-009", "Cat Treats Salmon 100g", "PetPure", "50608234567898", "https://picsum.photos/seed/pet-cattreats/200/200"),
            ("PET-010", "Senior Dog Food 2kg", "PetPure", "50608234567899", "https://picsum.photos/seed/pet-senior/200/200"),
            ("PET-011", "Grain-Free Dog 2kg", "PetPure", "50608234567900", "https://picsum.photos/seed/pet-grainfree/200/200"),
            ("PET-012", "Indoor Cat Food 1.5kg", "PetPure", "50608234567901", "https://picsum.photos/seed/pet-indoor/200/200"),
            ("PET-013", "Training Treats 150g", "PetPure", "50608234567902", "https://picsum.photos/seed/pet-training/200/200"),
            ("PET-014", "Multi-Pack Pouches 12pk", "PetPure", "50608234567903", "https://picsum.photos/seed/pet-multipack/200/200"),
            ("PET-015", "Hypoallergenic Dog 2kg", "PetPure", "50608234567904", "https://picsum.photos/seed/pet-hypo/200/200"),
            ("PET-016", "Hairball Cat Food 1.5kg", "PetPure", "50608234567905", "https://picsum.photos/seed/pet-hairball/200/200"),
            ("PET-017", "Weight Control Dog 2kg", "PetPure", "50608234567906", "https://picsum.photos/seed/pet-weight/200/200"),
            ("PET-018", "Natural Cat Treats 80g", "PetPure", "50608234567907", "https://picsum.photos/seed/pet-natural/200/200"),
            ("PET-019", "Lamb & Rice Dog 2kg", "PetPure", "50608234567908", "https://picsum.photos/seed/pet-lamb/200/200"),
            ("PET-020", "Fish Cat Food 1.5kg", "PetPure", "50608234567909", "https://picsum.photos/seed/pet-fish/200/200"),
        };

        var supplierProductIds = new[] { spBag.Id, spCan.Id, spBox.Id, spLabel.Id, spPouch.Id };
        var productEntities = new List<Product>();
        for (var idx = 0; idx < products.Length; idx++)
        {
            var (sku, name, brand, gtin, imageUrl) = products[idx];
            var p = await EnsureProduct(sku, name, brand, gtin, DatasetKey, imageUrl);
            productEntities.Add(p);
            await EnsureProductForm(p.Id, gtin, name, brand, "Pet Care", "Pet Food");
            await EnsureProductPackaging(p.Id, puShipping.Id);
            await EnsureProductPackaging(p.Id, puProduct.Id);
            await EnsureProductPackagingSupplierProduct(p.Id, supplierProductIds[idx % supplierProductIds.Length]);
        }

        var rnd = new Random(49);
        var geos = new[] { (sydneyGeo, "Sydney", auJurisdiction.Id, "AU"), (melbourneGeo, "Melbourne", auJurisdiction.Id, "AU"), (brisbaneGeo, "Brisbane", auJurisdiction.Id, "AU"), (aucklandGeo, "Auckland", nzJurisdiction.Id, "NZ") };
        foreach (var p in productEntities)
        {
            var (geo, cityName, jurId, country) = geos[rnd.Next(geos.Length)];
            await EnsureDistribution(p.Id, puShipping.Id, rnd.Next(50, 400), "Pet Store", cityName, country, geo.Id, jurId);
        }

        var baseDate = DateTime.UtcNow.AddDays(-14);
        var receivers = new[] { ("5061987654345", "PetMart Sydney", "Sydney", "AU"), ("5061987654346", "PetHub Melbourne", "Melbourne", "AU"), ("5061987654347", "KiwiPet Auckland", "Auckland", "NZ") };
        var shippers = new[] { ("5061987654348", "PetPack Sydney", "Sydney", "AU"), ("5061987654349", "CanPet Melbourne", "Melbourne", "AU"), ("5061987654350", "KiwiPet Auckland", "Auckland", "NZ") };
        int lineNum = 1;
        for (int i = 0; i < 12; i++)
        {
            var (recGln, recName, recCity, recCountry) = receivers[i % receivers.Length];
            var (shipGln, shipName, shipCity, shipCountry) = shippers[i % shippers.Length];
            var ship = await EnsureAsnShipment($"ASN-PET-{1000 + i}", shipGln, shipName, recGln, recName, baseDate.AddDays(i * 2), DatasetKey, shipCity, shipCountry);
            for (int p = 0; p < 1 + (i % 3); p++)
            {
                var pallet = await EnsureAsnPallet(ship.Id, $"3579823456789012{(i * 3 + p):D2}", recName, recCity, recCountry, p + 1, recGln);
                for (int li = 0; li < 2 + (i + p) % 4; li++)
                {
                    var prod = productEntities[(i * 3 + p * 2 + li) % productEntities.Count];
                    await EnsureAsnLineItem(pallet.Id, prod.Gtin!, prod.Name, 20 + (i + li) % 40, lineNum++);
                }
            }
        }

        await _context.SaveChangesAsync();
        Console.WriteLine($"✓ Pet Care dataset seeded: 20 products, packaging, distribution, ASNs (AU/NZ)");
    }

    private async Task EnsureAsnsAsync()
    {
        var productEntities = await _context.Products.Where(p => p.DatasetKey == DatasetKey && p.Gtin != null).OrderBy(p => p.Sku).ToListAsync();
        if (productEntities.Count == 0) return;
        var baseDate = DateTime.UtcNow.AddDays(-14);
        var receivers = new[] { ("5061987654345", "PetMart Sydney", "Sydney", "AU"), ("5061987654346", "PetHub Melbourne", "Melbourne", "AU"), ("5061987654347", "KiwiPet Auckland", "Auckland", "NZ") };
        var shippers = new[] { ("5061987654348", "PetPack Sydney", "Sydney", "AU"), ("5061987654349", "CanPet Melbourne", "Melbourne", "AU"), ("5061987654350", "KiwiPet Auckland", "Auckland", "NZ") };
        int lineNum = 1;
        for (int i = 0; i < 12; i++)
        {
            var (recGln, recName, recCity, recCountry) = receivers[i % receivers.Length];
            var (shipGln, shipName, shipCity, shipCountry) = shippers[i % shippers.Length];
            var ship = await EnsureAsnShipment($"ASN-PET-{1000 + i}", shipGln, shipName, recGln, recName, baseDate.AddDays(i * 2), DatasetKey, shipCity, shipCountry);
            for (int p = 0; p < 1 + (i % 3); p++)
            {
                var pallet = await EnsureAsnPallet(ship.Id, $"3579823456789012{(i * 3 + p):D2}", recName, recCity, recCountry, p + 1, recGln);
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
        var seeds = new[] { "pet-dogfood", "pet-catfood", "pet-dogtreats", "pet-wetdog", "pet-wetcat", "pet-puppy", "pet-kitten", "pet-dental", "pet-cattreats", "pet-senior", "pet-grainfree", "pet-indoor", "pet-training", "pet-multipack", "pet-hypo", "pet-hairball", "pet-weight", "pet-natural", "pet-lamb", "pet-fish" };
        var products = await _context.Products.Where(p => p.DatasetKey == DatasetKey && (p.ImageUrl == null || p.ImageUrl == "")).ToListAsync();
        foreach (var p in products)
        {
            if (p.Sku != null && p.Sku.StartsWith("PET-") && int.TryParse(p.Sku.AsSpan(4), out var n) && n >= 1 && n <= 20)
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
        if (p == null) { p = new Product { Sku = sku, Name = name, Brand = brand, Gtin = gtin, ProductCategory = "Pet Care", ProductSubCategory = "Pet Food", CountryOfOrigin = "AU", DatasetKey = datasetKey, ImageUrl = imageUrl }; _context.Products.Add(p); await _context.SaveChangesAsync(); }
        else if (!string.IsNullOrEmpty(imageUrl) && string.IsNullOrEmpty(p.ImageUrl)) { p.ImageUrl = imageUrl; await _context.SaveChangesAsync(); }
        return p;
    }

    private async Task EnsureProductForm(int productId, string gtin, string productName, string brand, string category, string subCategory)
    {
        if (await _context.ProductForms.AnyAsync(pf => pf.ProductId == productId)) return;
        _context.ProductForms.Add(new ProductForm { ProductId = productId, Gtin = gtin, ProductName = productName, Brand = brand, ProductCategory = category, ProductSubCategory = subCategory, CountryOfOrigin = "AU", PackagingLevel = "Consumer Unit", PackagingType = "Bag", PackagingConfiguration = "Single component", Status = "submitted" });
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
