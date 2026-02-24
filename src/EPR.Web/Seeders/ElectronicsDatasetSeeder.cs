using EPR.Domain.Entities;
using EPR.Data;
using Microsoft.EntityFrameworkCore;

namespace EPR.Web.Seeders;

/// <summary>
/// Seeds a comprehensive Electronics dataset: 20 products with packaging, raw materials,
/// packaging groups, suppliers, distribution, and ASN data.
/// </summary>
public class ElectronicsDatasetSeeder
{
    private const string DatasetKey = "Electronics";
    private readonly EPRDbContext _context;

    public ElectronicsDatasetSeeder(EPRDbContext context)
    {
        _context = context;
    }

    public async Task SeedAsync()
    {
        var existingProductCount = await _context.Products.CountAsync(p => p.DatasetKey == DatasetKey);
        var existingAsnCount = await _context.AsnShipments.CountAsync(s => s.DatasetKey == DatasetKey);

        // If both products and ASNs exist, verify images and geography
        if (existingProductCount > 0 && existingAsnCount > 0)
        {
            await UpdateProductImagesIfMissing();
            await UpdateGeographyIfNeeded();
            await EnsureSupplyChainMatrixDataAsync();
            Console.WriteLine("Electronics dataset verified (images + geography + matrix links updated if needed)");
            return;
        }

        // If products exist but ASNs missing, add ASNs only (fixes empty Distribution tab)
        if (existingProductCount > 0 && existingAsnCount == 0)
        {
            await EnsureElectronicsAsnsAsync();
            Console.WriteLine("✓ Electronics ASNs added (linked to existing products)");
            return;
        }

        // Ensure MaterialTaxonomy exists (create basic plastics/cardboard if none)
        var plasticTax = await EnsureMaterialTaxonomy("PLASTIC", "Plastics", 1);
        var paperTax = await EnsureMaterialTaxonomy("PAPER", "Paper & Cardboard", 1);
        var foamTax = await EnsureMaterialTaxonomy("FOAM", "Foam", 1);

        // Suppliers: 80% Australian, 20% international (UK)
        var supplierBox = await EnsureSupplier("TechPack Solutions Pty Ltd", "12 Industrial Way", "Sydney", "Australia", "techpack@techpack.com.au");
        var supplierPlastic = await EnsureSupplier("PlastiForm Industries Australia", "45 Manufacturing Rd", "Melbourne", "Australia", "sales@plastiform.com.au");
        var supplierLabels = await EnsureSupplier("LabelTech Australia", "8 Print Street", "Brisbane", "Australia", "info@labeltech.com.au");
        var supplierLabelsUK = await EnsureSupplier("LabelTech UK", "8 Print Street", "Leeds", "UK", "info@labeltech.co.uk");

        // Packaging Library items (raw materials / packaging items)
        var boxCardboard = await EnsurePackagingLibrary("Cardboard shipping box 30x20x15cm", "ELEC-BOX-001", 450m, paperTax.Id, DatasetKey);
        var innerFoam = await EnsurePackagingLibrary("Foam insert for electronics", "ELEC-FOAM-001", 80m, foamTax.Id, DatasetKey);
        var plasticWrap = await EnsurePackagingLibrary("Anti-static plastic wrap", "ELEC-WRAP-001", 25m, plasticTax.Id, DatasetKey);
        var productBox = await EnsurePackagingLibrary("Product display box", "ELEC-PBOX-001", 120m, paperTax.Id, DatasetKey);
        var label = await EnsurePackagingLibrary("Product label", "ELEC-LBL-001", 5m, paperTax.Id, DatasetKey);

        await LinkPackagingMaterial(boxCardboard.Id, paperTax.Id);
        await LinkPackagingMaterial(innerFoam.Id, foamTax.Id);
        await LinkPackagingMaterial(plasticWrap.Id, plasticTax.Id);
        await LinkPackagingMaterial(productBox.Id, paperTax.Id);
        await LinkPackagingMaterial(label.Id, paperTax.Id);

        // Supplier products: 80% from AU suppliers, 20% from UK
        var spBox = await EnsureSupplierProduct(supplierBox.Id, "TechBox 30x20x15", "ELEC-BOX-001");
        var spFoam = await EnsureSupplierProduct(supplierPlastic.Id, "E-Foam Insert S", "ELEC-FOAM-001");
        var spWrap = await EnsureSupplierProduct(supplierPlastic.Id, "AntiStatic Wrap 500mm", "ELEC-WRAP-001");
        var spPBox = await EnsureSupplierProduct(supplierBox.Id, "Display Box A4", "ELEC-PBOX-001");
        var spLabel = await EnsureSupplierProduct(supplierLabelsUK.Id, "Electronics Label 50x30", "ELEC-LBL-001");

        await LinkPackagingSupplier(boxCardboard.Id, spBox.Id, true);
        await LinkPackagingSupplier(innerFoam.Id, spFoam.Id, true);
        await LinkPackagingSupplier(plasticWrap.Id, spWrap.Id, true);
        await LinkPackagingSupplier(productBox.Id, spPBox.Id, true);
        await LinkPackagingSupplier(label.Id, spLabel.Id, true);

        // Multiple suppliers per packaging item: product display box also from LabelTech UK
        var spPBox2 = await EnsureSupplierProduct(supplierLabelsUK.Id, "Display Box A4 UK", "ELEC-PBOX-002");
        await LinkPackagingSupplier(productBox.Id, spPBox2.Id, false);

        // Multiple suppliers per raw material: MaterialTaxonomySupplierProduct links
        var spPaperTechPack = await EnsureSupplierProduct(supplierBox.Id, "Paper Rolls 500mm", "ELEC-PAPER-001");
        var spPaperLabelTech = await EnsureSupplierProduct(supplierLabels.Id, "Cardboard Sheets A4", "ELEC-PAPER-002");
        await LinkMaterialTaxonomySupplier(paperTax.Id, spPaperTechPack.Id, true);
        await LinkMaterialTaxonomySupplier(paperTax.Id, spPaperLabelTech.Id, false);

        var spPlasticPlastiForm = await EnsureSupplierProduct(supplierPlastic.Id, "Plastic Pellets LDPE", "ELEC-PLASTIC-001");
        var spPlasticLabelTech = await EnsureSupplierProduct(supplierLabels.Id, "Plastic Film 200mic", "ELEC-PLASTIC-002");
        await LinkMaterialTaxonomySupplier(plasticTax.Id, spPlasticPlastiForm.Id, true);
        await LinkMaterialTaxonomySupplier(plasticTax.Id, spPlasticLabelTech.Id, false);

        // Upstream suppliers: LabelTech UK supplied by LabelTech Australia
        if (!supplierLabelsUK.SuppliedBySupplierId.HasValue)
        {
            supplierLabelsUK.SuppliedBySupplierId = supplierLabels.Id;
            await _context.SaveChangesAsync();
        }

        // Pallet packaging items
        var woodTax = await EnsureMaterialTaxonomy("WOOD", "Softwood Timber", 1);
        var ldpeTax = await EnsureMaterialTaxonomy("LDPE", "Low-Density Polyethylene Film", 1);
        var inkTax = await EnsureMaterialTaxonomy("INK", "Printing Ink", 1);
        var adhesiveTax = await EnsureMaterialTaxonomy("ADHESIVE", "Packaging Adhesive", 1);
        var palletLib = await EnsurePackagingLibrary("Wood Pallet", "ELEC-PLT-001", 22000m, woodTax.Id, DatasetKey);
        var wrapLib = await EnsurePackagingLibrary("Stretch Wrap", "ELEC-WRAP-PLT-001", 300m, ldpeTax.Id, DatasetKey);
        await LinkPackagingMaterial(palletLib.Id, woodTax.Id);
        await LinkPackagingMaterial(wrapLib.Id, ldpeTax.Id);

        // Multi-material: cardboard box uses both paper and printing ink
        await LinkPackagingMaterial(boxCardboard.Id, inkTax.Id);
        // Product display box uses paper and adhesive
        await LinkPackagingMaterial(productBox.Id, adhesiveTax.Id);

        // Supplier products for pallet items
        var spPallet = await EnsureSupplierProduct(supplierBox.Id, "Standard Pallet 1200x1000", "ELEC-PLT-001");
        var spStretchWrap = await EnsureSupplierProduct(supplierPlastic.Id, "Stretch Wrap 500mm", "ELEC-WRAP-PLT-001");
        await LinkPackagingSupplier(palletLib.Id, spPallet.Id, true);
        await LinkPackagingSupplier(wrapLib.Id, spStretchWrap.Id, true);

        // Packaging Groups (Tertiary first, then Secondary, then Primary)
        var groupPallet = await EnsurePackagingGroup("ELEC-PLT-001", "Electronics Pallet", "Tertiary", 22300m, DatasetKey);
        await AddGroupItem(groupPallet.Id, palletLib.Id, 0);
        await AddGroupItem(groupPallet.Id, wrapLib.Id, 1);

        var groupShipping = await EnsurePackagingGroup("ELEC-SHIP-001", "Electronics Shipping Pack", "Secondary", 655m, DatasetKey, groupPallet.Id, 48);
        await AddGroupItem(groupShipping.Id, boxCardboard.Id, 0);
        await AddGroupItem(groupShipping.Id, innerFoam.Id, 1);
        await AddGroupItem(groupShipping.Id, plasticWrap.Id, 2);

        var groupProduct = await EnsurePackagingGroup("ELEC-PROD-001", "Electronics Product Pack", "Primary", 125m, DatasetKey, groupShipping.Id, 12);
        await AddGroupItem(groupProduct.Id, productBox.Id, 0);
        await AddGroupItem(groupProduct.Id, label.Id, 1);

        // PackagingRawMaterial (bridge for PackagingType -> raw materials for GetAsnProductPackaging traceability)
        var prmPlastic = await EnsurePackagingRawMaterial("Plastics", "Plastic materials");
        var prmPaper = await EnsurePackagingRawMaterial("Paper & Cardboard", "Paper and cardboard materials");
        var prmFoam = await EnsurePackagingRawMaterial("Foam", "Foam materials");
        var materialTaxToPrm = new Dictionary<int, int> { { plasticTax.Id, prmPlastic.Id }, { paperTax.Id, prmPaper.Id }, { foamTax.Id, prmFoam.Id } };

        // PackagingType from each PackagingLibrary (for PackagingUnitItem -> PackagingType -> PackagingRawMaterial chain)
        var ptBox = await EnsurePackagingTypeFromLibrary(boxCardboard, materialTaxToPrm);
        var ptFoam = await EnsurePackagingTypeFromLibrary(innerFoam, materialTaxToPrm);
        var ptWrap = await EnsurePackagingTypeFromLibrary(plasticWrap, materialTaxToPrm);
        var ptProductBox = await EnsurePackagingTypeFromLibrary(productBox, materialTaxToPrm);
        var ptLabel = await EnsurePackagingTypeFromLibrary(label, materialTaxToPrm);

        // PackagingUnits from PackagingGroups (with PackagingUnitItems for full traceability)
        var shippingLibs = new[] { boxCardboard, innerFoam, plasticWrap };
        var productLibs = new[] { productBox, label };
        var libToPt = new Dictionary<int, PackagingType> { { boxCardboard.Id, ptBox }, { innerFoam.Id, ptFoam }, { plasticWrap.Id, ptWrap }, { productBox.Id, ptProductBox }, { label.Id, ptLabel } };
        var puShipping = await EnsurePackagingUnitFromGroup(groupShipping, shippingLibs, libToPt);
        var puProduct = await EnsurePackagingUnitFromGroup(groupProduct, productLibs, libToPt);

        // Geographies and Jurisdictions: 80% Australia, 20% UK
        var auJurisdiction = await EnsureJurisdiction("AU", "Australia", "AU");
        var sydneyGeo = await EnsureGeography("SYD", "Sydney", auJurisdiction.Id);
        var melbourneGeo = await EnsureGeography("MEL", "Melbourne", auJurisdiction.Id);
        var brisbaneGeo = await EnsureGeography("BNE", "Brisbane", auJurisdiction.Id);
        var ukJurisdiction = await EnsureJurisdiction("UK", "United Kingdom", "GB");
        var londonGeo = await EnsureGeography("LON", "London", ukJurisdiction.Id);
        var manchesterGeo = await EnsureGeography("MAN", "Manchester", ukJurisdiction.Id);
        var birminghamGeo = await EnsureGeography("BIR", "Birmingham", ukJurisdiction.Id);

        // 20 Electronics products with placeholder images (picsum.photos - unique per product)
        var products = new[]
        {
            ("ELEC-001", "Wireless Bluetooth Earbuds Pro", "TechSound", "50601234567890", "https://picsum.photos/seed/elec-earbuds/200/200"),
            ("ELEC-002", "USB-C Fast Charger 65W", "PowerFlow", "50601234567891", "https://picsum.photos/seed/elec-charger/200/200"),
            ("ELEC-003", "Portable Power Bank 20000mAh", "PowerFlow", "50601234567892", "https://picsum.photos/seed/elec-powerbank/200/200"),
            ("ELEC-004", "HD Webcam 1080p", "VisionTech", "50601234567893", "https://picsum.photos/seed/elec-webcam/200/200"),
            ("ELEC-005", "Mechanical Gaming Keyboard", "KeyMaster", "50601234567894", "https://picsum.photos/seed/elec-keyboard/200/200"),
            ("ELEC-006", "Wireless Mouse Ergonomic", "KeyMaster", "50601234567895", "https://picsum.photos/seed/elec-mouse/200/200"),
            ("ELEC-007", "Bluetooth Speaker Waterproof", "TechSound", "50601234567896", "https://picsum.photos/seed/elec-speaker/200/200"),
            ("ELEC-008", "Tablet Stand Adjustable", "DeskPro", "50601234567897", "https://picsum.photos/seed/elec-stand/200/200"),
            ("ELEC-009", "Phone Holder Car Mount", "DeskPro", "50601234567898", "https://picsum.photos/seed/elec-phoneholder/200/200"),
            ("ELEC-010", "HDMI Cable 2m", "CableTech", "50601234567899", "https://picsum.photos/seed/elec-hdmi/200/200"),
            ("ELEC-011", "USB Hub 4-Port", "CableTech", "50601234567900", "https://picsum.photos/seed/elec-usbhub/200/200"),
            ("ELEC-012", "Screen Protector Glass", "VisionTech", "50601234567901", "https://picsum.photos/seed/elec-screen/200/200"),
            ("ELEC-013", "Laptop Sleeve 15\"", "DeskPro", "50601234567902", "https://picsum.photos/seed/elec-sleeve/200/200"),
            ("ELEC-014", "Smart Watch Band Silicone", "WearTech", "50601234567903", "https://picsum.photos/seed/elec-watch/200/200"),
            ("ELEC-015", "Earbud Tips Set S/M/L", "TechSound", "50601234567904", "https://picsum.photos/seed/elec-tips/200/200"),
            ("ELEC-016", "Cable Organiser Box", "DeskPro", "50601234567905", "https://picsum.photos/seed/elec-organiser/200/200"),
            ("ELEC-017", "LED Desk Lamp USB", "LightPro", "50601234567906", "https://picsum.photos/seed/elec-lamp/200/200"),
            ("ELEC-018", "Webcam Cover Slider", "VisionTech", "50601234567907", "https://picsum.photos/seed/elec-cover/200/200"),
            ("ELEC-019", "Laptop Cooling Pad", "DeskPro", "50601234567908", "https://picsum.photos/seed/elec-cooling/200/200"),
            ("ELEC-020", "Multi-Port Adapter USB-C", "CableTech", "50601234567909", "https://picsum.photos/seed/elec-adapter/200/200"),
        };

        var supplierProductIds = new[] { spBox.Id, spFoam.Id, spWrap.Id, spPBox.Id, spLabel.Id };
        var productEntities = new List<Product>();
        for (var idx = 0; idx < products.Length; idx++)
        {
            var (sku, name, brand, gtin, imageUrl) = products[idx];
            var isAustralian = idx < 16;
            var p = await EnsureProduct(sku, name, brand, gtin, DatasetKey, imageUrl, isAustralian ? "AU" : "CN");
            productEntities.Add(p);

            await EnsureProductForm(p.Id, gtin, name, brand, "Electronics", "Consumer Electronics", isAustralian ? "AU" : "CN");

            await EnsureProductPackaging(p.Id, puShipping.Id);
            await EnsureProductPackaging(p.Id, puProduct.Id);

            var spId = supplierProductIds[idx % supplierProductIds.Length];
            await EnsureProductPackagingSupplierProduct(p.Id, spId);
        }

        // Distribution: 80% Australian destinations, 20% UK
        var rnd = new Random(42);
        var auGeos = new[] { (sydneyGeo, "Sydney", "Australia", auJurisdiction.Id), (melbourneGeo, "Melbourne", "Australia", auJurisdiction.Id), (brisbaneGeo, "Brisbane", "Australia", auJurisdiction.Id) };
        var ukGeos = new[] { (londonGeo, "London", "UK", ukJurisdiction.Id), (manchesterGeo, "Manchester", "UK", ukJurisdiction.Id), (birminghamGeo, "Birmingham", "UK", ukJurisdiction.Id) };
        for (var i = 0; i < productEntities.Count; i++)
        {
            var p = productEntities[i];
            var (geo, cityName, country, jurisdictionId) = i < 16 ? auGeos[rnd.Next(auGeos.Length)] : ukGeos[rnd.Next(ukGeos.Length)];
            await EnsureDistribution(p.Id, puShipping.Id, rnd.Next(50, 500), "Retail Store", cityName, country, geo.Id, jurisdictionId);
        }

        // ASN Shipments: 80% Australian receivers/shippers, 20% UK
        var baseDate = DateTime.UtcNow.AddDays(-14);
        var auReceivers = new[] { ("9390987654321", "TechRetail DC Sydney", "Sydney"), ("9390987654322", "ElectroStore Melbourne", "Melbourne"), ("9390987654323", "GadgetHub Brisbane", "Brisbane") };
        var ukReceivers = new[] { ("5060987654321", "TechRetail DC London", "London"), ("5060987654322", "ElectroStore Manchester", "Manchester"), ("5060987654323", "GadgetHub Birmingham", "Birmingham") };
        var auShippers = new[] { ("9390987654324", "TechPack Solutions Pty Ltd"), ("9390987654325", "PowerFlow Distribution AU"), ("9390987654326", "CableTech Logistics AU") };
        var ukShippers = new[] { ("5060987654324", "TechPack Solutions Ltd"), ("5060987654325", "PowerFlow Distribution"), ("5060987654326", "CableTech Logistics") };
        int lineNum = 1;
        for (int i = 0; i < 12; i++)
        {
            var isAustralian = i < 10;
            var (recGln, recName, recCity) = isAustralian ? auReceivers[i % auReceivers.Length] : ukReceivers[i % ukReceivers.Length];
            var (shipGln, shipName) = isAustralian ? auShippers[i % auShippers.Length] : ukShippers[i % ukShippers.Length];
            var ship = await EnsureAsnShipment(
                $"ASN-ELEC-{1000 + i}",
                shipGln,
                shipName,
                recGln,
                recName,
                baseDate.AddDays(i * 2),
                DatasetKey,
                isAustralian ? "Sydney" : "Manchester",
                isAustralian ? "AU" : "GB");
            var palletsPerShipment = 1 + (i % 3);
            for (int p = 0; p < palletsPerShipment; p++)
            {
                var pallet = await EnsureAsnPallet(ship.Id, $"3579123456789012{(i * 3 + p):D2}", recName, recCity, isAustralian ? "AU" : "GB", p + 1, recGln);
                var productsPerPallet = 2 + (i + p) % 4;
                for (int li = 0; li < productsPerPallet; li++)
                {
                    var prod = productEntities[(i * 3 + p * 2 + li) % productEntities.Count];
                    await EnsureAsnLineItem(pallet.Id, prod.Gtin!, prod.Name, 12 + (i + li) % 24, lineNum++);
                }
            }
        }

        await _context.SaveChangesAsync();
        Console.WriteLine($"✓ Electronics dataset seeded: 20 products, packaging, distribution, ASNs (80% AU, 20% international)");
    }

    /// <summary>
    /// Add Electronics ASNs when products exist but ASNs are missing (fixes empty Distribution tab).
    /// ASN line items use product GTINs, linking ASNs to products. Products already have ProductPackaging links.
    /// </summary>
    private async Task EnsureElectronicsAsnsAsync()
    {
        var productEntities = await _context.Products
            .Where(p => p.DatasetKey == DatasetKey && p.Gtin != null)
            .OrderBy(p => p.Sku)
            .ToListAsync();
        if (productEntities.Count == 0) return;

        var baseDate = DateTime.UtcNow.AddDays(-14);
        var auReceivers = new[] { ("9390987654321", "TechRetail DC Sydney", "Sydney"), ("9390987654322", "ElectroStore Melbourne", "Melbourne"), ("9390987654323", "GadgetHub Brisbane", "Brisbane") };
        var ukReceivers = new[] { ("5060987654321", "TechRetail DC London", "London"), ("5060987654322", "ElectroStore Manchester", "Manchester"), ("5060987654323", "GadgetHub Birmingham", "Birmingham") };
        var auShippers = new[] { ("9390987654324", "TechPack Solutions Pty Ltd"), ("9390987654325", "PowerFlow Distribution AU"), ("9390987654326", "CableTech Logistics AU") };
        var ukShippers = new[] { ("5060987654324", "TechPack Solutions Ltd"), ("5060987654325", "PowerFlow Distribution"), ("5060987654326", "CableTech Logistics") };
        int lineNum = 1;
        for (int i = 0; i < 12; i++)
        {
            var isAustralian = i < 10;
            var (recGln, recName, recCity) = isAustralian ? auReceivers[i % auReceivers.Length] : ukReceivers[i % ukReceivers.Length];
            var (shipGln, shipName) = isAustralian ? auShippers[i % auShippers.Length] : ukShippers[i % ukShippers.Length];
            var ship = await EnsureAsnShipment(
                $"ASN-ELEC-{1000 + i}",
                shipGln,
                shipName,
                recGln,
                recName,
                baseDate.AddDays(i * 2),
                DatasetKey,
                isAustralian ? "Sydney" : "Manchester",
                isAustralian ? "AU" : "GB");
            var palletsPerShipment = 1 + (i % 3);
            for (int p = 0; p < palletsPerShipment; p++)
            {
                var pallet = await EnsureAsnPallet(ship.Id, $"3579123456789012{(i * 3 + p):D2}", recName, recCity, isAustralian ? "AU" : "GB", p + 1, recGln);
                var productsPerPallet = 2 + (i + p) % 4;
                for (int li = 0; li < productsPerPallet; li++)
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
        var productImages = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            ["ELEC-001"] = "https://picsum.photos/seed/elec-earbuds/200/200",
            ["ELEC-002"] = "https://picsum.photos/seed/elec-charger/200/200",
            ["ELEC-003"] = "https://picsum.photos/seed/elec-powerbank/200/200",
            ["ELEC-004"] = "https://picsum.photos/seed/elec-webcam/200/200",
            ["ELEC-005"] = "https://picsum.photos/seed/elec-keyboard/200/200",
            ["ELEC-006"] = "https://picsum.photos/seed/elec-mouse/200/200",
            ["ELEC-007"] = "https://picsum.photos/seed/elec-speaker/200/200",
            ["ELEC-008"] = "https://picsum.photos/seed/elec-stand/200/200",
            ["ELEC-009"] = "https://picsum.photos/seed/elec-phoneholder/200/200",
            ["ELEC-010"] = "https://picsum.photos/seed/elec-hdmi/200/200",
            ["ELEC-011"] = "https://picsum.photos/seed/elec-usbhub/200/200",
            ["ELEC-012"] = "https://picsum.photos/seed/elec-screen/200/200",
            ["ELEC-013"] = "https://picsum.photos/seed/elec-sleeve/200/200",
            ["ELEC-014"] = "https://picsum.photos/seed/elec-watch/200/200",
            ["ELEC-015"] = "https://picsum.photos/seed/elec-tips/200/200",
            ["ELEC-016"] = "https://picsum.photos/seed/elec-organiser/200/200",
            ["ELEC-017"] = "https://picsum.photos/seed/elec-lamp/200/200",
            ["ELEC-018"] = "https://picsum.photos/seed/elec-cover/200/200",
            ["ELEC-019"] = "https://picsum.photos/seed/elec-cooling/200/200",
            ["ELEC-020"] = "https://picsum.photos/seed/elec-adapter/200/200",
        };
        var products = await _context.Products.Where(p => p.DatasetKey == DatasetKey && (p.ImageUrl == null || p.ImageUrl == "")).ToListAsync();
        foreach (var p in products)
        {
            if (p.Sku != null && productImages.TryGetValue(p.Sku, out var url))
            {
                p.ImageUrl = url;
            }
        }
        if (products.Count > 0)
            await _context.SaveChangesAsync();
    }

    private async Task UpdateGeographyIfNeeded()
    {
        var auCities = new[] { "Sydney", "Melbourne", "Brisbane" };
        var ukCities = new[] { "London", "Manchester", "Birmingham" };

        // Products: first 80% AU, last 20% CN
        var products = await _context.Products.Where(p => p.DatasetKey == DatasetKey).OrderBy(p => p.Sku).ToListAsync();
        if (products.Count == 0) return;
        var auCount = (int)Math.Ceiling(products.Count * 0.8);
        var changed = false;
        for (int i = 0; i < products.Count; i++)
        {
            var expected = i < auCount ? "AU" : "CN";
            if (products[i].CountryOfOrigin != expected) { products[i].CountryOfOrigin = expected; changed = true; }
        }

        // ProductForms
        var productIds = products.Select(p => p.Id).ToList();
        var forms = await _context.ProductForms.Where(pf => productIds.Contains(pf.ProductId)).ToListAsync();
        var auProductIds = new HashSet<int>(products.Take(auCount).Select(p => p.Id));
        foreach (var f in forms)
        {
            var expected = auProductIds.Contains(f.ProductId) ? "AU" : "CN";
            if (f.CountryOfOrigin != expected) { f.CountryOfOrigin = expected; changed = true; }
        }

        // Distributions
        var distributions = await _context.Distributions.Where(d => d.DatasetKey == DatasetKey).ToListAsync();
        var distByProduct = distributions.GroupBy(d => d.ProductId).ToDictionary(g => g.Key, g => g.ToList());
        var rnd = new Random(42);
        foreach (var p in products)
        {
            if (!distByProduct.TryGetValue(p.Id, out var dists)) continue;
            var isAu = auProductIds.Contains(p.Id);
            foreach (var d in dists)
            {
                var cities = isAu ? auCities : ukCities;
                var expectedCountry = isAu ? "Australia" : "UK";
                if (d.Country != expectedCountry)
                {
                    d.City = cities[rnd.Next(cities.Length)];
                    d.StateProvince = d.City;
                    d.Country = expectedCountry;
                    changed = true;
                }
            }
        }

        // ASN Shipments: first ~83% AU, rest UK
        var shipments = await _context.AsnShipments.Where(s => s.DatasetKey == DatasetKey).OrderBy(s => s.AsnNumber).ToListAsync();
        var auShipCount = (int)Math.Ceiling(shipments.Count * 0.8);
        var auReceivers = new[] { ("9390987654321", "TechRetail DC Sydney", "Sydney"), ("9390987654322", "ElectroStore Melbourne", "Melbourne"), ("9390987654323", "GadgetHub Brisbane", "Brisbane") };
        var ukReceivers = new[] { ("5060987654321", "TechRetail DC London", "London"), ("5060987654322", "ElectroStore Manchester", "Manchester"), ("5060987654323", "GadgetHub Birmingham", "Birmingham") };
        for (int i = 0; i < shipments.Count; i++)
        {
            var s = shipments[i];
            var isAu = i < auShipCount;
            var expectedCC = isAu ? "AU" : "GB";
            if (s.ShipperCountryCode != expectedCC)
            {
                var rec = isAu ? auReceivers[i % auReceivers.Length] : ukReceivers[i % ukReceivers.Length];
                s.ShipperCity = isAu ? "Sydney" : "Manchester";
                s.ShipperCountryCode = expectedCC;
                s.ReceiverGln = rec.Item1;
                s.ReceiverName = rec.Item2;
                changed = true;
            }
        }

        // ASN Pallets
        var shipmentIds = shipments.Select(s => s.Id).ToList();
        var pallets = await _context.AsnPallets.Where(p => shipmentIds.Contains(p.AsnShipmentId)).ToListAsync();
        var auShipmentIds = new HashSet<int>(shipments.Take(auShipCount).Select(s => s.Id));
        foreach (var pal in pallets)
        {
            var isAu = auShipmentIds.Contains(pal.AsnShipmentId);
            var expectedCC = isAu ? "AU" : "GB";
            if (pal.DestinationCountryCode != expectedCC)
            {
                var cities2 = isAu ? auCities : ukCities;
                pal.DestinationCity = cities2[rnd.Next(cities2.Length)];
                pal.DestinationCountryCode = expectedCC;
                changed = true;
            }
        }

        if (changed)
        {
            await _context.SaveChangesAsync();
            Console.WriteLine("  ✓ Electronics geography updated to 80% AU / 20% international");
        }
    }

    /// <summary>
    /// Ensures all packaging items have supplier links and demonstrates multi-material relationships.
    /// Runs as a fixup when the dataset already exists.
    /// </summary>
    private async Task EnsureSupplyChainMatrixDataAsync()
    {
        var changed = false;

        // Ensure Printing Ink and Adhesive material taxonomies exist
        var inkTax = await EnsureMaterialTaxonomy("INK", "Printing Ink", 1);
        var adhesiveTax = await EnsureMaterialTaxonomy("ADHESIVE", "Packaging Adhesive", 1);

        // Find existing packaging items that need fixup
        var palletLib = await _context.PackagingLibraries
            .FirstOrDefaultAsync(l => l.TaxonomyCode == "ELEC-PLT-001" && l.DatasetKey == DatasetKey && l.IsActive);
        var wrapLib = await _context.PackagingLibraries
            .FirstOrDefaultAsync(l => l.TaxonomyCode == "ELEC-WRAP-PLT-001" && l.DatasetKey == DatasetKey && l.IsActive);
        var boxLib = await _context.PackagingLibraries
            .FirstOrDefaultAsync(l => l.TaxonomyCode == "ELEC-BOX-001" && l.DatasetKey == DatasetKey && l.IsActive);
        var productBoxLib = await _context.PackagingLibraries
            .FirstOrDefaultAsync(l => l.TaxonomyCode == "ELEC-PBOX-001" && l.DatasetKey == DatasetKey && l.IsActive);

        // Find suppliers for linking
        var supplierBox = await _context.PackagingSuppliers.FirstOrDefaultAsync(s => s.Name == "TechPack Solutions Pty Ltd");
        var supplierPlastic = await _context.PackagingSuppliers.FirstOrDefaultAsync(s => s.Name == "PlastiForm Industries Australia");

        // Ensure pallet items have supplier products
        if (palletLib != null && supplierBox != null)
        {
            var spPallet = await EnsureSupplierProduct(supplierBox.Id, "Standard Pallet 1200x1000", "ELEC-PLT-001");
            if (!await _context.PackagingLibrarySupplierProducts.AnyAsync(x => x.PackagingLibraryId == palletLib.Id))
            {
                await LinkPackagingSupplier(palletLib.Id, spPallet.Id, true);
                changed = true;
            }
        }

        if (wrapLib != null && supplierPlastic != null)
        {
            var spWrap = await EnsureSupplierProduct(supplierPlastic.Id, "Stretch Wrap 500mm", "ELEC-WRAP-PLT-001");
            if (!await _context.PackagingLibrarySupplierProducts.AnyAsync(x => x.PackagingLibraryId == wrapLib.Id))
            {
                await LinkPackagingSupplier(wrapLib.Id, spWrap.Id, true);
                changed = true;
            }
        }

        // Multi-material: cardboard box also uses printing ink
        if (boxLib != null)
        {
            if (!await _context.PackagingLibraryMaterials.AnyAsync(x => x.PackagingLibraryId == boxLib.Id && x.MaterialTaxonomyId == inkTax.Id))
            {
                await LinkPackagingMaterial(boxLib.Id, inkTax.Id);
                changed = true;
            }
        }

        // Multi-material: product display box also uses adhesive
        if (productBoxLib != null)
        {
            if (!await _context.PackagingLibraryMaterials.AnyAsync(x => x.PackagingLibraryId == productBoxLib.Id && x.MaterialTaxonomyId == adhesiveTax.Id))
            {
                await LinkPackagingMaterial(productBoxLib.Id, adhesiveTax.Id);
                changed = true;
            }
        }

        // Multiple suppliers per raw material: MaterialTaxonomySupplierProduct links
        var paperTax = await _context.MaterialTaxonomies.FirstOrDefaultAsync(m => m.Code == "PAPER");
        var plasticTax = await _context.MaterialTaxonomies.FirstOrDefaultAsync(m => m.Code == "PLASTIC");
        var supplierLabels = await _context.PackagingSuppliers.FirstOrDefaultAsync(s => s.Name == "LabelTech Australia");
        var supplierLabelsUK = await _context.PackagingSuppliers.FirstOrDefaultAsync(s => s.Name == "LabelTech UK");
        if (paperTax != null && supplierBox != null && supplierLabels != null)
        {
            var spPaperTechPack = await EnsureSupplierProduct(supplierBox.Id, "Paper Rolls 500mm", "ELEC-PAPER-001");
            var spPaperLabelTech = await EnsureSupplierProduct(supplierLabels.Id, "Cardboard Sheets A4", "ELEC-PAPER-002");
            if (!await _context.MaterialTaxonomySupplierProducts.AnyAsync(m => m.MaterialTaxonomyId == paperTax.Id && m.PackagingSupplierProductId == spPaperTechPack.Id))
            {
                await LinkMaterialTaxonomySupplier(paperTax.Id, spPaperTechPack.Id, true);
                changed = true;
            }
            if (!await _context.MaterialTaxonomySupplierProducts.AnyAsync(m => m.MaterialTaxonomyId == paperTax.Id && m.PackagingSupplierProductId == spPaperLabelTech.Id))
            {
                await LinkMaterialTaxonomySupplier(paperTax.Id, spPaperLabelTech.Id, false);
                changed = true;
            }
        }
        if (plasticTax != null && supplierPlastic != null && supplierLabels != null)
        {
            var spPlasticPlastiForm = await EnsureSupplierProduct(supplierPlastic.Id, "Plastic Pellets LDPE", "ELEC-PLASTIC-001");
            var spPlasticLabelTech = await EnsureSupplierProduct(supplierLabels.Id, "Plastic Film 200mic", "ELEC-PLASTIC-002");
            if (!await _context.MaterialTaxonomySupplierProducts.AnyAsync(m => m.MaterialTaxonomyId == plasticTax.Id && m.PackagingSupplierProductId == spPlasticPlastiForm.Id))
            {
                await LinkMaterialTaxonomySupplier(plasticTax.Id, spPlasticPlastiForm.Id, true);
                changed = true;
            }
            if (!await _context.MaterialTaxonomySupplierProducts.AnyAsync(m => m.MaterialTaxonomyId == plasticTax.Id && m.PackagingSupplierProductId == spPlasticLabelTech.Id))
            {
                await LinkMaterialTaxonomySupplier(plasticTax.Id, spPlasticLabelTech.Id, false);
                changed = true;
            }
        }

        // Multiple suppliers per packaging item: product display box
        if (productBoxLib != null && supplierLabelsUK != null)
        {
            var spPBox2 = await EnsureSupplierProduct(supplierLabelsUK.Id, "Display Box A4 UK", "ELEC-PBOX-002");
            if (!await _context.PackagingLibrarySupplierProducts.AnyAsync(plsp => plsp.PackagingLibraryId == productBoxLib.Id && plsp.PackagingSupplierProductId == spPBox2.Id))
            {
                await LinkPackagingSupplier(productBoxLib.Id, spPBox2.Id, false);
                changed = true;
            }
        }

        // Upstream suppliers: LabelTech UK supplied by LabelTech Australia
        if (supplierLabels != null && supplierLabelsUK != null && !supplierLabelsUK.SuppliedBySupplierId.HasValue)
        {
            supplierLabelsUK.SuppliedBySupplierId = supplierLabels.Id;
            changed = true;
        }

        if (changed)
        {
            await _context.SaveChangesAsync();
            Console.WriteLine("  ✓ Electronics supply chain matrix data fixed (supplier links + multi-material + multi-supplier)");
        }
    }

    private async Task<MaterialTaxonomy> EnsureMaterialTaxonomy(string code, string displayName, int level)
    {
        var t = await _context.MaterialTaxonomies.FirstOrDefaultAsync(m => m.Code == code);
        if (t == null)
        {
            t = new MaterialTaxonomy
            {
                Code = code,
                DisplayName = displayName,
                Level = level,
                IsActive = true,
                SortOrder = 0
            };
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
        var lib = new PackagingLibrary
        {
            Name = name,
            TaxonomyCode = taxonomyCode,
            Weight = weight,
            MaterialTaxonomyId = materialTaxonomyId,
            DatasetKey = datasetKey,
            IsActive = true
        };
        _context.PackagingLibraries.Add(lib);
        await _context.SaveChangesAsync();
        return lib;
    }

    private async Task LinkPackagingMaterial(int packagingLibraryId, int materialTaxonomyId)
    {
        if (await _context.PackagingLibraryMaterials.AnyAsync(plm => plm.PackagingLibraryId == packagingLibraryId && plm.MaterialTaxonomyId == materialTaxonomyId))
            return;
        _context.PackagingLibraryMaterials.Add(new PackagingLibraryMaterial
        {
            PackagingLibraryId = packagingLibraryId,
            MaterialTaxonomyId = materialTaxonomyId,
            SortOrder = 0
        });
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
        if (await _context.PackagingLibrarySupplierProducts.AnyAsync(plsp => plsp.PackagingLibraryId == packagingLibraryId && plsp.PackagingSupplierProductId == supplierProductId))
            return;
        _context.PackagingLibrarySupplierProducts.Add(new PackagingLibrarySupplierProduct
        {
            PackagingLibraryId = packagingLibraryId,
            PackagingSupplierProductId = supplierProductId,
            IsPrimary = isPrimary
        });
        await _context.SaveChangesAsync();
    }

    private async Task LinkMaterialTaxonomySupplier(int materialTaxonomyId, int supplierProductId, bool isPrimary = true)
    {
        if (await _context.MaterialTaxonomySupplierProducts.AnyAsync(mtsp => mtsp.MaterialTaxonomyId == materialTaxonomyId && mtsp.PackagingSupplierProductId == supplierProductId))
            return;
        _context.MaterialTaxonomySupplierProducts.Add(new MaterialTaxonomySupplierProduct
        {
            MaterialTaxonomyId = materialTaxonomyId,
            PackagingSupplierProductId = supplierProductId,
            IsPrimary = isPrimary
        });
        await _context.SaveChangesAsync();
    }

    private async Task<PackagingGroup> EnsurePackagingGroup(string packId, string name, string layer, decimal totalWeight, string datasetKey, int? parentGroupId = null, int? quantityInParent = null)
    {
        var g = new PackagingGroup
        {
            PackId = packId,
            Name = name,
            PackagingLayer = layer,
            TotalPackWeight = totalWeight,
            DatasetKey = datasetKey,
            IsActive = true,
            ParentPackagingGroupId = parentGroupId,
            QuantityInParent = quantityInParent
        };
        _context.PackagingGroups.Add(g);
        await _context.SaveChangesAsync();
        return g;
    }

    private async Task AddGroupItem(int groupId, int libraryId, int sortOrder)
    {
        if (await _context.PackagingGroupItems.AnyAsync(gi => gi.PackagingGroupId == groupId && gi.PackagingLibraryId == libraryId))
            return;
        _context.PackagingGroupItems.Add(new PackagingGroupItem { PackagingGroupId = groupId, PackagingLibraryId = libraryId, SortOrder = sortOrder });
        await _context.SaveChangesAsync();
    }

    private async Task<PackagingRawMaterial> EnsurePackagingRawMaterial(string name, string? description)
    {
        var prm = await _context.PackagingRawMaterials.FirstOrDefaultAsync(m => m.Name == name);
        if (prm == null)
        {
            prm = new PackagingRawMaterial { Name = name, Description = description };
            _context.PackagingRawMaterials.Add(prm);
            await _context.SaveChangesAsync();
        }
        return prm;
    }

    private async Task<PackagingType> EnsurePackagingTypeFromLibrary(PackagingLibrary lib, Dictionary<int, int> materialTaxonomyIdToPrmId)
    {
        var pt = await _context.PackagingTypes.FirstOrDefaultAsync(t => t.Name == lib.Name);
        if (pt == null)
        {
            pt = new PackagingType
            {
                Name = lib.Name,
                Weight = lib.Weight,
                IsFromLibrary = true,
                LibrarySource = lib.TaxonomyCode ?? "PackagingLibrary",
                Notes = $"Linked from PackagingLibrary (Id={lib.Id})"
            };
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
            pu = new PackagingUnit
            {
                Name = group.Name,
                UnitLevel = group.PackagingLayer ?? "Primary",
                Notes = $"Linked from PackagingGroup {group.PackId}"
            };
            _context.PackagingUnits.Add(pu);
            await _context.SaveChangesAsync();
        }
        foreach (var lib in libs)
        {
            if (!libIdToPackagingType.TryGetValue(lib.Id, out var pt))
                continue;
            if (await _context.PackagingUnitItems.AnyAsync(pui => pui.PackagingUnitId == pu.Id && pui.PackagingTypeId == pt.Id))
                continue;
            _context.PackagingUnitItems.Add(new PackagingUnitItem
            {
                PackagingUnitId = pu.Id,
                PackagingTypeId = pt.Id,
                CollectionName = "Default",
                Quantity = 1
            });
        }
        await _context.SaveChangesAsync();
        return pu;
    }

    private async Task EnsureProductPackagingSupplierProduct(int productId, int packagingSupplierProductId)
    {
        if (await _context.ProductPackagingSupplierProducts.AnyAsync(pp => pp.ProductId == productId && pp.PackagingSupplierProductId == packagingSupplierProductId))
            return;
        _context.ProductPackagingSupplierProducts.Add(new ProductPackagingSupplierProduct
        {
            ProductId = productId,
            PackagingSupplierProductId = packagingSupplierProductId
        });
        await _context.SaveChangesAsync();
    }

    private async Task<Jurisdiction> EnsureJurisdiction(string code, string name, string countryCode)
    {
        var j = await _context.Jurisdictions.FirstOrDefaultAsync(x => x.Code == code);
        if (j == null)
        {
            j = new Jurisdiction { Code = code, Name = name, CountryCode = countryCode };
            _context.Jurisdictions.Add(j);
            await _context.SaveChangesAsync();
        }
        return j;
    }

    private async Task<Geography> EnsureGeography(string code, string name, int jurisdictionId)
    {
        var g = await _context.Geographies.FirstOrDefaultAsync(x => x.Code == code);
        if (g == null)
        {
            g = new Geography { Code = code, Name = name, JurisdictionId = jurisdictionId };
            _context.Geographies.Add(g);
            await _context.SaveChangesAsync();
        }
        return g;
    }

    private async Task<Product> EnsureProduct(string sku, string name, string brand, string gtin, string datasetKey, string? imageUrl = null, string? countryOfOrigin = "AU")
    {
        var p = await _context.Products.FirstOrDefaultAsync(x => x.Sku == sku);
        if (p == null)
        {
            p = new Product
            {
                Sku = sku,
                Name = name,
                Brand = brand,
                Gtin = gtin,
                ProductCategory = "Electronics",
                ProductSubCategory = "Consumer Electronics",
                CountryOfOrigin = countryOfOrigin ?? "AU",
                DatasetKey = datasetKey,
                ImageUrl = imageUrl
            };
            _context.Products.Add(p);
            await _context.SaveChangesAsync();
        }
        else if (!string.IsNullOrEmpty(imageUrl) && string.IsNullOrEmpty(p.ImageUrl))
        {
            p.ImageUrl = imageUrl;
            await _context.SaveChangesAsync();
        }
        return p;
    }

    private async Task EnsureProductForm(int productId, string gtin, string productName, string brand, string category, string subCategory, string? countryOfOrigin = "AU")
    {
        if (await _context.ProductForms.AnyAsync(pf => pf.ProductId == productId))
            return;
        _context.ProductForms.Add(new ProductForm
        {
            ProductId = productId,
            Gtin = gtin,
            ProductName = productName,
            Brand = brand,
            ProductCategory = category,
            ProductSubCategory = subCategory,
            CountryOfOrigin = countryOfOrigin ?? "AU",
            PackagingLevel = "Consumer Unit",
            PackagingType = "Box",
            PackagingConfiguration = "Single component",
            Status = "submitted"
        });
        await _context.SaveChangesAsync();
    }

    private async Task EnsureProductPackaging(int productId, int packagingUnitId)
    {
        if (await _context.ProductPackagings.AnyAsync(pp => pp.ProductId == productId && pp.PackagingUnitId == packagingUnitId))
            return;
        _context.ProductPackagings.Add(new ProductPackaging { ProductId = productId, PackagingUnitId = packagingUnitId });
        await _context.SaveChangesAsync();
    }

    private async Task EnsureDistribution(int productId, int packagingUnitId, int quantity, string retailer, string city, string country, int geographyId, int jurisdictionId)
    {
        _context.Distributions.Add(new Distribution
        {
            ProductId = productId,
            PackagingUnitId = packagingUnitId,
            Quantity = quantity,
            RetailerName = retailer,
            City = city,
            StateProvince = city,
            County = "",
            PostcodeZipcode = "",
            Country = country,
            DispatchDate = DateTime.UtcNow.AddDays(-30),
            GeographyId = geographyId,
            JurisdictionId = jurisdictionId,
            DatasetKey = DatasetKey
        });
        await _context.SaveChangesAsync();
    }

    private async Task<AsnShipment> EnsureAsnShipment(string asnNumber, string shipperGln, string shipperName, string receiverGln, string receiverName, DateTime shipDate, string datasetKey, string? shipperCity = "Sydney", string? shipperCountryCode = "AU")
    {
        var s = new AsnShipment
        {
            AsnNumber = asnNumber,
            ShipperGln = shipperGln,
            ShipperName = shipperName,
            ShipperCity = shipperCity ?? "Sydney",
            ShipperCountryCode = shipperCountryCode ?? "AU",
            ReceiverGln = receiverGln,
            ReceiverName = receiverName,
            ShipDate = shipDate,
            DeliveryDate = shipDate.AddDays(1),
            CarrierName = "FastFreight AU",
            TransportMode = "ROAD",
            TotalPackages = 1,
            TotalWeight = 25.5m,
            SourceFormat = "GS1_XML",
            Status = "DELIVERED",
            DatasetKey = datasetKey
        };
        _context.AsnShipments.Add(s);
        await _context.SaveChangesAsync();
        return s;
    }

    private async Task<AsnPallet> EnsureAsnPallet(int shipmentId, string sscc, string destName, string destCity, string destCountry, int seq, string destGln)
    {
        var p = new AsnPallet
        {
            AsnShipmentId = shipmentId,
            Sscc = sscc,
            PackageTypeCode = "PLT",
            GrossWeight = 25m,
            DestinationGln = destGln,
            DestinationName = destName,
            DestinationCity = destCity,
            DestinationCountryCode = destCountry,
            SequenceNumber = seq
        };
        _context.AsnPallets.Add(p);
        await _context.SaveChangesAsync();
        return p;
    }

    private async Task EnsureAsnLineItem(int palletId, string gtin, string description, decimal quantity, int lineNum)
    {
        _context.AsnLineItems.Add(new AsnLineItem
        {
            AsnPalletId = palletId,
            LineNumber = lineNum,
            Gtin = gtin,
            Description = description,
            Quantity = quantity,
            UnitOfMeasure = "PCE"
        });
        await _context.SaveChangesAsync();
    }
}
