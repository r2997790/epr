using Microsoft.EntityFrameworkCore;
using EPR.Data;
using EPR.Domain.Entities;
using Microsoft.Extensions.Logging;

namespace EPR.Web.Scripts;

/// <summary>
/// Script to create dummy ASN, Product, and Packaging data for testing
/// Run this from a controller endpoint or console command
/// </summary>
public class CreateDummyAsnData
{
    private readonly EPRDbContext _context;
    private readonly ILogger<CreateDummyAsnData> _logger;

    public CreateDummyAsnData(EPRDbContext context, ILogger<CreateDummyAsnData> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task ExecuteAsync()
    {
        _logger.LogInformation("Starting dummy data creation...");

        try
        {
            // Create Raw Materials
            var petPlastic = await CreateRawMaterial("PET Plastic", "Polyethylene Terephthalate - Recyclable plastic");
            var paperLabel = await CreateRawMaterial("Paper Label", "Paper label material");
            var cardboard = await CreateRawMaterial("Cardboard", "Corrugated cardboard for boxes");
            var glass = await CreateRawMaterial("Glass", "Glass material for bottles");
            var aluminum = await CreateRawMaterial("Aluminum", "Aluminum foil for packaging");

            // Create Packaging Types
            var bottleType = await CreatePackagingType("Plastic Bottle 500ml", "Bottle", "Secondary", petPlastic.Id);
            var labelType = await CreatePackagingType("Product Label", "Label", "Secondary", paperLabel.Id);
            var caseType = await CreatePackagingType("Cardboard Case 12pk", "Case", "Tertiary", cardboard.Id);
            var glassBottleType = await CreatePackagingType("Glass Bottle 750ml", "Bottle", "Secondary", glass.Id);
            var foilType = await CreatePackagingType("Aluminum Foil Wrap", "Wrap", "Secondary", aluminum.Id);

            // Create Packaging Units
            var secondaryUnit1 = await CreatePackagingUnit("Bottle + Label Unit", "Secondary");
            await AddPackagingUnitItem(secondaryUnit1.Id, bottleType.Id);
            await AddPackagingUnitItem(secondaryUnit1.Id, labelType.Id);

            var secondaryUnit2 = await CreatePackagingUnit("Glass Bottle Unit", "Secondary");
            await AddPackagingUnitItem(secondaryUnit2.Id, glassBottleType.Id);

            var tertiaryUnit1 = await CreatePackagingUnit("Case Unit", "Tertiary");
            await AddPackagingUnitItem(tertiaryUnit1.Id, caseType.Id);

            // ---- 25 Products with varying complexity ----
            var product1 = await CreateProduct("05012345678900", "Premium Coffee Beans 1kg", "High-quality arabica coffee beans",
                "Acme Coffee", "Food & Beverage", "Coffee", 1.0m, "kg", null, null, "CO");
            var product2 = await CreateProduct("05012345678917", "Organic Tea Selection Box", "Assorted organic tea selection",
                "GreenLeaf", "Food & Beverage", "Tea", 0.35m, "kg", null, null, "GB");
            var product3 = await CreateProduct("05012345678924", "Gourmet Chocolate Bars", "Premium dark chocolate bars",
                "ChocMaster", "Food & Beverage", "Confectionery", 0.1m, "kg", null, null, "BE");
            var product4 = await CreateProduct("05012345678931", "Premium Olive Oil 500ml", "Extra virgin olive oil",
                "Mediterranean Gold", "Food & Beverage", "Oils", 0.5m, "kg", 0.5m, "L", "ES");
            var product5 = await CreateProduct("05012345678948", "Artisan Pasta 500g", "Handmade Italian pasta",
                "Pasta Fresca", "Food & Beverage", "Pasta", 0.5m, "kg", null, null, "IT");
            var product6 = await CreateProduct("05012345678955", "Smartphone Model X Pro", "Latest smartphone model",
                "TechCorp", "Electronics", "Mobile Phones", 0.2m, "kg", null, null, "CN");
            var product7 = await CreateProduct("05012345678962", "Wireless Earbuds Pro", "Premium wireless earbuds",
                "TechCorp", "Electronics", "Audio", 0.05m, "kg", null, null, "VN");
            var product8 = await CreateProduct("05012345678979", "Organic Honey 340g", "Pure organic set honey");
            var product9 = await CreateProduct("05012345678986", "Coconut Water 1L", "Natural coconut water no added sugar",
                "Tropical", "Food & Beverage", "Beverages", 1.02m, "kg", 1m, "L", "PH");
            var product10 = await CreateProduct("05012345678993", "Wholegrain Breakfast Cereal 750g",
                "High fibre wholegrain cereal", "MorningStart", "Food & Beverage", "Cereal", 0.75m, "kg", null, null, "GB");
            var product11 = await CreateProduct("05012345679009", "Hand Soap Refill 1L", "Gentle hand soap refill");
            var product12 = await CreateProduct("05012345679016", "Recycled Kitchen Roll 6pk", "100% recycled paper",
                "EcoHome", "Household", "Paper Products", null, null, null, null, "GB");
            var product13 = await CreateProduct("05012345679023", "Canned Tomatoes 400g", "Chopped tomatoes in juice",
                "Kitchen Basics", "Food & Beverage", "Canned Goods", 0.4m, "kg", 0.4m, "L", "IT");
            var product14 = await CreateProduct("05012345679030", "Frozen Peas 1kg", "Garden peas frozen",
                "FreshFrost", "Food & Beverage", "Frozen", 1.0m, "kg", null, null, "GB");
            var product15 = await CreateProduct("05012345679047", "LED Desk Lamp", "Adjustable LED desk lamp",
                "BrightLife", "Home & Garden", "Lighting", 0.6m, "kg", null, null, "CN");
            var product16 = await CreateProduct("05012345679054", "Stainless Steel Water Bottle 750ml", "Insulated bottle");
            var product17 = await CreateProduct("05012345679061", "Protein Bar 60g", "Chocolate protein bar",
                "FitFuel", "Food & Beverage", "Snacks", 0.06m, "kg", null, null, "GB");
            var product18 = await CreateProduct("05012345679078", "Baby Wipes Sensitive 12x80", "Fragrance-free wipes",
                "SoftCare", "Personal Care", "Baby Care", null, null, null, null, "IE");
            var product19 = await CreateProduct("05012345679085", "Dry Dog Food 4kg", "Adult complete nutrition",
                "PetChef", "Pet Care", "Dog Food", 4.0m, "kg", null, null, "GB");
            var product20 = await CreateProduct("05012345679092", "Fabric Softener 2L", "Spring fresh fabric softener");
            var product21 = await CreateProduct("05012345679108", "Sparking Water 24x330ml", "Natural mineral water",
                "ClearSpring", "Food & Beverage", "Beverages", 8.5m, "kg", 7.92m, "L", "GB");
            var product22 = await CreateProduct("05012345679115", "USB-C Hub 7-in-1", "Multi-port USB-C hub",
                "ConnectTech", "Electronics", "Accessories", 0.15m, "kg", null, null, "TW");
            var product23 = await CreateProduct("05012345679122", "Yoga Mat 6mm", "Non-slip exercise mat");
            var product24 = await CreateProduct("05012345679139", "Instant Noodles 5pk", "Chicken flavour instant noodles",
                "QuickBite", "Food & Beverage", "Instant Meals", 0.5m, "kg", null, null, "SG");
            var product25 = await CreateProduct("05012345679146", "Sunscreen SPF50 200ml", "Broad spectrum sunscreen",
                "SunSafe", "Personal Care", "Skincare", 0.22m, "kg", 0.2m, "L", "FR");

            var products = new[] { product1, product2, product3, product4, product5, product6, product7, product8, product9, product10,
                product11, product12, product13, product14, product15, product16, product17, product18, product19, product20,
                product21, product22, product23, product24, product25 };

            // Link all products to packaging (for Distribution visual view)
            foreach (var p in products)
            {
                await LinkProductPackaging(p.Id, secondaryUnit1.Id);
                await LinkProductPackaging(p.Id, tertiaryUnit1.Id);
            }
            await LinkProductPackaging(product4.Id, secondaryUnit2.Id);

            // ---- 10 ASN records of varying complexity ----
            var baseDate = DateTime.UtcNow;
            var ssccBase = 37612345678901240;

            // ASN 1: Simple – 1 pallet, 1 line item
            var asn1 = await CreateAsnShipment(new AsnShipment
            {
                AsnNumber = "ASN-SEED-001",
                ShipperGln = "1234567890001",
                ShipperName = "ACME Manufacturing Ltd",
                ShipperAddress = "123 Industrial Park",
                ShipperCity = "Manchester",
                ShipperPostalCode = "M1 1AA",
                ShipperCountryCode = "GB",
                ReceiverGln = "1234567890002",
                ReceiverName = "Central Distribution Hub",
                ShipDate = baseDate.AddDays(-10),
                DeliveryDate = baseDate.AddDays(-8),
                CarrierName = "Swift Logistics",
                TransportMode = "ROAD",
                VehicleRegistration = "AB12 CDE",
                TotalWeight = 320m,
                TotalPackages = 1,
                SourceFormat = "GS1_XML",
                Status = "DELIVERED",
                ImportedAt = baseDate.AddDays(-10)
            });
            var p1 = await CreatePallet(asn1.Id, 1, $"{ssccBase + 0}", "PLT", 320m, "1234567890006", "Northern Retail Store", "50 High Street", "Leeds", "LS1 1AA", "GB");
            await CreateLineItem(p1.Id, 1, product1.Gtin!, product1.Name, 250m, "EA", "BATCH-2026-001", baseDate.AddMonths(6));

            // ASN 2: 1 pallet, 3 line items
            var asn2 = await CreateAsnShipment(new AsnShipment
            {
                AsnNumber = "ASN-SEED-002",
                ShipperGln = "1234567890001",
                ShipperName = "ACME Manufacturing Ltd",
                ShipperAddress = "123 Industrial Park",
                ShipperCity = "Manchester",
                ShipperPostalCode = "M1 1AA",
                ShipperCountryCode = "GB",
                ReceiverGln = "1234567890007",
                ReceiverName = "Southern Retail Store",
                ShipDate = baseDate.AddDays(-7),
                DeliveryDate = baseDate.AddDays(-5),
                CarrierName = "Swift Logistics",
                TransportMode = "ROAD",
                TotalWeight = 410m,
                TotalPackages = 1,
                SourceFormat = "GS1_XML",
                Status = "DELIVERED",
                ImportedAt = baseDate.AddDays(-7)
            });
            var p2 = await CreatePallet(asn2.Id, 1, $"{ssccBase + 1}", "PLT", 410m, "1234567890007", "Southern Retail Store", "75 Market Square", "Southampton", "SO14 0AA", "GB");
            await CreateLineItem(p2.Id, 1, product2.Gtin!, product2.Name, 120m, "EA", "BATCH-2026-002", baseDate.AddMonths(10));
            await CreateLineItem(p2.Id, 2, product3.Gtin!, product3.Name, 80m, "EA", "BATCH-2026-003", baseDate.AddMonths(8));
            await CreateLineItem(p2.Id, 3, product8.Gtin!, product8.Name, 200m, "EA", "BATCH-2026-008", baseDate.AddMonths(12));

            // ASN 3: 2 pallets, 2 destinations
            var asn3 = await CreateAsnShipment(new AsnShipment
            {
                AsnNumber = "ASN-SEED-003",
                ShipperGln = "1234567890010",
                ShipperName = "Tech Manufacturing Corp",
                ShipperAddress = "456 Tech Park Drive",
                ShipperCity = "Cambridge",
                ShipperPostalCode = "CB2 1TN",
                ShipperCountryCode = "GB",
                ReceiverGln = "1234567890011",
                ReceiverName = "Electronics Distribution Network",
                ShipDate = baseDate.AddDays(-5),
                DeliveryDate = baseDate.AddDays(-3),
                CarrierName = "Secure Transport Ltd",
                TransportMode = "ROAD",
                TotalWeight = 185m,
                TotalPackages = 2,
                SourceFormat = "GS1_XML",
                Status = "IN_TRANSIT",
                ImportedAt = baseDate.AddDays(-5)
            });
            var p3a = await CreatePallet(asn3.Id, 1, $"{ssccBase + 2}", "PLT", 95m, "1234567890012", "Tech Store A", "25 High Street", "Manchester", "M1 1AA", "GB");
            await CreateLineItem(p3a.Id, 1, product6.Gtin!, product6.Name, 40m, "EA", "BATCH-XPRO-001", null);
            var p3b = await CreatePallet(asn3.Id, 2, $"{ssccBase + 3}", "PLT", 90m, "1234567890013", "Tech Store B", "10 Station Rd", "Birmingham", "B2 4AA", "GB");
            await CreateLineItem(p3b.Id, 1, product7.Gtin!, product7.Name, 60m, "EA", "BATCH-EARBUDS-001", null);

            // ASN 4: 3 pallets, 3 destinations (complex)
            var asn4 = await CreateAsnShipment(new AsnShipment
            {
                AsnNumber = "ASN-SEED-004",
                ShipperGln = "1234567890001",
                ShipperName = "ACME Manufacturing Ltd",
                ShipperAddress = "123 Industrial Park",
                ShipperCity = "Manchester",
                ShipperPostalCode = "M1 1AA",
                ShipperCountryCode = "GB",
                ReceiverGln = "1234567890002",
                ReceiverName = "Central Distribution Hub",
                ShipDate = baseDate.AddDays(-4),
                DeliveryDate = baseDate.AddDays(-2),
                CarrierName = "Swift Logistics",
                TransportMode = "ROAD",
                VehicleRegistration = "XY99 ZZZ",
                TotalWeight = 1250.5m,
                TotalPackages = 3,
                SourceFormat = "GS1_XML",
                Status = "DELIVERED",
                ImportedAt = baseDate.AddDays(-4)
            });
            var p4a = await CreatePallet(asn4.Id, 1, $"{ssccBase + 4}", "PLT", 312.5m, "1234567890006", "Northern Retail Store", "50 High Street", "Leeds", "LS1 1AA", "GB");
            await CreateLineItem(p4a.Id, 1, product1.Gtin!, product1.Name, 250m, "EA", "BATCH-2026-001", baseDate.AddMonths(6));
            var p4b = await CreatePallet(asn4.Id, 2, $"{ssccBase + 5}", "PLT", 313m, "1234567890007", "Southern Retail Store", "75 Market Square", "Southampton", "SO14 0AA", "GB");
            await CreateLineItem(p4b.Id, 1, product2.Gtin!, product2.Name, 150m, "EA", "BATCH-2026-002", baseDate.AddMonths(10));
            await CreateLineItem(p4b.Id, 2, product3.Gtin!, product3.Name, 100m, "EA", "BATCH-2026-003", baseDate.AddMonths(8));
            var p4c = await CreatePallet(asn4.Id, 3, $"{ssccBase + 6}", "PLT", 625m, "1234567890008", "Eastern Retail Store", "100 Commerce Way", "Norwich", "NR1 1AA", "GB");
            await CreateLineItem(p4c.Id, 1, product1.Gtin!, product1.Name, 250m, "EA", "BATCH-2026-001", baseDate.AddMonths(6));
            await CreateLineItem(p4c.Id, 2, product4.Gtin!, product4.Name, 200m, "EA", "BATCH-2026-004", baseDate.AddMonths(8));

            // ASN 5: 2 pallets, 1 with missing destination (warnings)
            var asn5 = await CreateAsnShipment(new AsnShipment
            {
                AsnNumber = "ASN-SEED-005",
                ShipperGln = "1234567890008",
                ShipperName = "Global Food Distributors",
                ShipperAddress = "789 Global Trade Center",
                ShipperCity = "London",
                ShipperPostalCode = "E1 1AA",
                ShipperCountryCode = "GB",
                ReceiverGln = "1234567890009",
                ReceiverName = "Metro Retail Chain HQ",
                ShipDate = baseDate.AddDays(-3),
                CarrierName = "National Express Logistics",
                TransportMode = "ROAD",
                TotalWeight = 400.5m,
                TotalPackages = 2,
                SourceFormat = "GS1_XML",
                Status = "PENDING",
                ImportedAt = baseDate.AddDays(-3)
            });
            var p5a = await CreatePallet(asn5.Id, 1, $"{ssccBase + 7}", "PLT", 200m, "1234567890014", "Store A", "100 Main Street", "Birmingham", "B1 1AA", "GB");
            await CreateLineItem(p5a.Id, 1, product4.Gtin!, product4.Name, 200m, "EA", "BATCH-OIL-001", baseDate.AddMonths(8));
            var p5b = await CreatePallet(asn5.Id, 2, $"{ssccBase + 8}", "PLT", 200.5m, null, "", null, null, null, null);
            await CreateLineItem(p5b.Id, 1, product5.Gtin!, product5.Name, 150m, "EA", "BATCH-PASTA-001", baseDate.AddMonths(11));
            await CreateLineItem(p5b.Id, 2, product3.Gtin!, product3.Name, 100m, "EA", "BATCH-CHOC-001", baseDate.AddMonths(9));

            // ASN 6: Simulated ASN, 1 pallet
            var asn6 = await CreateAsnShipment(new AsnShipment
            {
                AsnNumber = "ASN-SEED-006-SIM",
                ShipperGln = "1234567890015",
                ShipperName = "Simulated Shipper Ltd",
                ShipperAddress = "1 Simulation Way",
                ShipperCity = "London",
                ShipperPostalCode = "SW1A 1AA",
                ShipperCountryCode = "GB",
                ReceiverGln = "1234567890016",
                ReceiverName = "Simulated Receiver",
                ShipDate = baseDate.AddDays(-2),
                DeliveryDate = baseDate.AddDays(1),
                CarrierName = "Modelled Transport",
                TransportMode = "ROAD",
                TotalWeight = 180m,
                TotalPackages = 1,
                SourceFormat = "GS1_XML",
                Status = "PENDING",
                ImportedAt = baseDate.AddDays(-2),
                IsSimulated = true
            });
            var p6 = await CreatePallet(asn6.Id, 1, $"{ssccBase + 9}", "PLT", 180m, "1234567890016", "Simulated Receiver", "2 Model Lane", "Reading", "RG1 1AA", "GB");
            await CreateLineItem(p6.Id, 1, product9.Gtin!, product9.Name, 144m, "EA", "BATCH-COCO-001", baseDate.AddMonths(6));
            await CreateLineItem(p6.Id, 2, product10.Gtin!, product10.Name, 96m, "EA", "BATCH-CEREAL-001", baseDate.AddMonths(14));

            // ASN 7: IN_TRANSIT, 2 pallets same destination
            var asn7 = await CreateAsnShipment(new AsnShipment
            {
                AsnNumber = "ASN-SEED-007",
                ShipperGln = "1234567890017",
                ShipperName = "Bulk Foods Co",
                ShipperAddress = "200 Warehouse Road",
                ShipperCity = "Bristol",
                ShipperPostalCode = "BS1 1AA",
                ShipperCountryCode = "GB",
                ReceiverGln = "1234567890018",
                ReceiverName = "Superstore Bristol",
                ShipDate = baseDate.AddDays(-1),
                DeliveryDate = baseDate.AddDays(2),
                PoReference = "PO-2026-7890",
                CarrierName = "Regional Haulage",
                TransportMode = "ROAD",
                TotalWeight = 920m,
                TotalPackages = 2,
                SourceFormat = "GS1_XML",
                Status = "IN_TRANSIT",
                ImportedAt = baseDate.AddDays(-1)
            });
            var p7a = await CreatePallet(asn7.Id, 1, $"{ssccBase + 10}", "PLT", 460m, "1234567890018", "Superstore Bristol", "500 Retail Park", "Bristol", "BS2 0AA", "GB");
            await CreateLineItem(p7a.Id, 1, product13.Gtin!, product13.Name, 800m, "EA", "BATCH-TOM-001", baseDate.AddMonths(18));
            await CreateLineItem(p7a.Id, 2, product14.Gtin!, product14.Name, 400m, "EA", "BATCH-PEAS-001", baseDate.AddMonths(12));
            var p7b = await CreatePallet(asn7.Id, 2, $"{ssccBase + 11}", "PLT", 460m, "1234567890018", "Superstore Bristol", "500 Retail Park", "Bristol", "BS2 0AA", "GB");
            await CreateLineItem(p7b.Id, 1, product21.Gtin!, product21.Name, 200m, "EA", "BATCH-SPARK-001", baseDate.AddMonths(24));

            // ASN 8: DELIVERED, 4 pallets, 4 destinations
            var asn8 = await CreateAsnShipment(new AsnShipment
            {
                AsnNumber = "ASN-SEED-008",
                ShipperGln = "1234567890019",
                ShipperName = "National FMCG Distributor",
                ShipperAddress = "1000 Distribution Way",
                ShipperCity = "Milton Keynes",
                ShipperPostalCode = "MK9 1AA",
                ShipperCountryCode = "GB",
                ReceiverGln = "1234567890020",
                ReceiverName = "Regional Hub South",
                ShipDate = baseDate.AddDays(-14),
                DeliveryDate = baseDate.AddDays(-12),
                CarrierName = "National Freight",
                TransportMode = "ROAD",
                TotalWeight = 2100m,
                TotalPackages = 4,
                SourceFormat = "GS1_XML",
                Status = "DELIVERED",
                ImportedAt = baseDate.AddDays(-14)
            });
            for (int i = 0; i < 4; i++)
            {
                var destGln = $"12345678900{21 + i}";
                var destName = new[] { "Store Cardiff", "Store Edinburgh", "Store Belfast", "Store Plymouth" }[i];
                var city = new[] { "Cardiff", "Edinburgh", "Belfast", "Plymouth" }[i];
                var post = new[] { "CF10 1AA", "EH1 1AA", "BT1 1AA", "PL1 1AA" }[i];
                var pp = await CreatePallet(asn8.Id, i + 1, $"{ssccBase + 12 + i}", "PLT", 525m, destGln, destName, "High Street", city, post, "GB");
                await CreateLineItem(pp.Id, 1, products[i * 3].Gtin!, products[i * 3].Name, 300m, "EA", $"BATCH-{i + 1}", baseDate.AddMonths(6));
                await CreateLineItem(pp.Id, 2, products[i * 3 + 1].Gtin!, products[i * 3 + 1].Name, 200m, "EA", $"BATCH-{i + 1}-B", baseDate.AddMonths(8));
            }

            // ASN 9: PENDING, 1 pallet, 4 line items
            var asn9 = await CreateAsnShipment(new AsnShipment
            {
                AsnNumber = "ASN-SEED-009",
                ShipperGln = "1234567890025",
                ShipperName = "Health & Home Ltd",
                ShipperAddress = "88 Wellness Road",
                ShipperCity = "Guildford",
                ShipperPostalCode = "GU1 1AA",
                ShipperCountryCode = "GB",
                ReceiverGln = "1234567890026",
                ReceiverName = "Health Store Chain",
                ShipDate = baseDate.AddDays(1),
                DeliveryDate = baseDate.AddDays(4),
                CarrierName = "Eco Deliveries",
                TransportMode = "ROAD",
                TotalWeight = 155m,
                TotalPackages = 1,
                SourceFormat = "GS1_XML",
                Status = "PENDING",
                ImportedAt = baseDate
            });
            var p9 = await CreatePallet(asn9.Id, 1, $"{ssccBase + 16}", "PLT", 155m, "1234567890026", "Health Store Chain", "15 Wellness Lane", "Guildford", "GU2 2BB", "GB");
            await CreateLineItem(p9.Id, 1, product17.Gtin!, product17.Name, 500m, "EA", "BATCH-PRO-001", baseDate.AddMonths(9));
            await CreateLineItem(p9.Id, 2, product18.Gtin!, product18.Name, 120m, "EA", "BATCH-WIPES-001", baseDate.AddMonths(24));
            await CreateLineItem(p9.Id, 3, product23.Gtin!, product23.Name, 80m, "EA", "BATCH-YOGA-001", null);
            await CreateLineItem(p9.Id, 4, product25.Gtin!, product25.Name, 200m, "EA", "BATCH-SUN-001", baseDate.AddMonths(36));

            // ASN 10: Mixed – 3 pallets, one destination missing
            var asn10 = await CreateAsnShipment(new AsnShipment
            {
                AsnNumber = "ASN-SEED-010",
                ShipperGln = "1234567890027",
                ShipperName = "Pet & Home Supplies",
                ShipperAddress = "45 Animal Way",
                ShipperCity = "Swindon",
                ShipperPostalCode = "SN1 1AA",
                ShipperCountryCode = "GB",
                ReceiverGln = "1234567890028",
                ReceiverName = "Pet Store National",
                ShipDate = baseDate.AddDays(-6),
                DeliveryDate = baseDate.AddDays(-4),
                CarrierName = "Pet Logistics",
                TransportMode = "ROAD",
                TotalWeight = 650m,
                TotalPackages = 3,
                SourceFormat = "GS1_XML",
                Status = "DELIVERED",
                ImportedAt = baseDate.AddDays(-6)
            });
            var p10a = await CreatePallet(asn10.Id, 1, $"{ssccBase + 17}", "PLT", 220m, "1234567890028", "Pet Store National", "1 Pet Street", "Swindon", "SN2 2BB", "GB");
            await CreateLineItem(p10a.Id, 1, product19.Gtin!, product19.Name, 50m, "EA", "BATCH-DOG-001", baseDate.AddMonths(18));
            var p10b = await CreatePallet(asn10.Id, 2, $"{ssccBase + 18}", "PLT", 210m, "1234567890029", "Pet Store Leeds", "20 Park Lane", "Leeds", "LS2 2AA", "GB");
            await CreateLineItem(p10b.Id, 1, product19.Gtin!, product19.Name, 48m, "EA", "BATCH-DOG-002", baseDate.AddMonths(18));
            var p10c = await CreatePallet(asn10.Id, 3, $"{ssccBase + 19}", "PLT", 220m, null, "TBC", null, null, null, null);
            await CreateLineItem(p10c.Id, 1, product20.Gtin!, product20.Name, 180m, "EA", "BATCH-SOFT-001", baseDate.AddMonths(24));

            // ---- ProductForms and Distributions for Reporting dashboard ----
            var packagingTypesForForms = new[] { "Plastic Bottle", "Cardboard Case", "Glass Bottle", "Product Label", "Aluminum Wrap", "Plastic Tub", "Paper Sleeve", "Corrugated Case" };
            var states = new[] { "NSW", "VIC", "QLD", "WA", "SA", "TAS", "ACT", "NT" };
            var rnd = new Random(42);
            for (var i = 0; i < products.Length; i++)
            {
                var p = products[i];
                var hasForm = await _context.ProductForms.AnyAsync(f => f.ProductId == p.Id);
                if (!hasForm)
                {
                    var packagingType = packagingTypesForForms[i % packagingTypesForForms.Length];
                    var weight = 50m + (rnd.Next(1, 90) * 5m);
                    _context.ProductForms.Add(new ProductForm
                    {
                        ProductId = p.Id,
                        Gtin = p.Gtin ?? p.Sku,
                        ProductName = p.Name,
                        Brand = p.Brand ?? "Unknown",
                        ProductCategory = p.ProductCategory ?? "General",
                        ProductSubCategory = p.ProductSubCategory ?? "",
                        CountryOfOrigin = p.CountryOfOrigin ?? "GB",
                        PackagingLevel = "Consumer Unit",
                        PackagingType = packagingType,
                        PackagingConfiguration = "Single component",
                        TotalPackagingWeight = weight,
                        Status = "submitted",
                        CreatedAt = baseDate.AddDays(-rnd.Next(1, 180))
                    });
                }
            }
            await _context.SaveChangesAsync();

            var distributionProductIds = (await _context.Products.OrderBy(x => x.Id).Take(20).Select(x => x.Id).ToListAsync());
            var packagingUnitIds = await _context.PackagingUnits.OrderBy(x => x.Id).Take(2).Select(x => x.Id).ToListAsync();
            var packUnitId = packagingUnitIds.FirstOrDefault();
            if (packUnitId != 0)
            {
                foreach (var productId in distributionProductIds)
                {
                    var numDist = rnd.Next(1, 4);
                    for (var d = 0; d < numDist; d++)
                    {
                        var state = states[rnd.Next(states.Length)];
                        var city = state == "NSW" ? "Sydney" : state == "VIC" ? "Melbourne" : state == "QLD" ? "Brisbane" : state == "WA" ? "Perth" : state == "SA" ? "Adelaide" : "Hobart";
                        var existingDist = await _context.Distributions
                            .AnyAsync(x => x.ProductId == productId && x.StateProvince == state && x.City == city);
                        if (!existingDist)
                        {
                            _context.Distributions.Add(new Distribution
                            {
                                ProductId = productId,
                                PackagingUnitId = packUnitId,
                                Quantity = (decimal)(rnd.Next(10, 500)),
                                City = city,
                                StateProvince = state,
                                Country = "AU",
                                DispatchDate = baseDate.AddDays(-rnd.Next(1, 365)),
                                CreatedAt = baseDate
                            });
                        }
                    }
                }
            }

            await _context.SaveChangesAsync();
            _logger.LogInformation("Dummy data created successfully!");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating dummy data");
            throw;
        }
    }

    private async Task<PackagingRawMaterial> CreateRawMaterial(string name, string description)
    {
        var existing = await _context.PackagingRawMaterials.FirstOrDefaultAsync(m => m.Name == name);
        if (existing != null) return existing;

        var material = new PackagingRawMaterial
        {
            Name = name,
            Description = description,
            CreatedAt = DateTime.UtcNow
        };
        _context.PackagingRawMaterials.Add(material);
        await _context.SaveChangesAsync();
        return material;
    }

    private async Task<PackagingType> CreatePackagingType(string name, string librarySource, string notes, int? materialId)
    {
        var existing = await _context.PackagingTypes.FirstOrDefaultAsync(t => t.Name == name);
        if (existing != null) return existing;

        var type = new PackagingType
        {
            Name = name,
            IsFromLibrary = true,
            LibrarySource = librarySource,
            Notes = notes,
            CreatedAt = DateTime.UtcNow
        };
        _context.PackagingTypes.Add(type);
        await _context.SaveChangesAsync();

        if (materialId.HasValue)
        {
            var materialLink = new PackagingTypeMaterial
            {
                PackagingTypeId = type.Id,
                MaterialId = materialId.Value
            };
            _context.PackagingTypeMaterials.Add(materialLink);
            await _context.SaveChangesAsync();
        }

        return type;
    }

    private async Task<PackagingUnit> CreatePackagingUnit(string name, string unitLevel)
    {
        var existing = await _context.PackagingUnits.FirstOrDefaultAsync(u => u.Name == name);
        if (existing != null) return existing;

        var unit = new PackagingUnit
        {
            Name = name,
            UnitLevel = unitLevel,
            CreatedAt = DateTime.UtcNow
        };
        _context.PackagingUnits.Add(unit);
        await _context.SaveChangesAsync();
        return unit;
    }

    private async Task AddPackagingUnitItem(int unitId, int typeId)
    {
        var existing = await _context.PackagingUnitItems
            .FirstOrDefaultAsync(i => i.PackagingUnitId == unitId && i.PackagingTypeId == typeId);
        if (existing != null) return;

        var item = new PackagingUnitItem
        {
            PackagingUnitId = unitId,
            PackagingTypeId = typeId,
            CollectionName = "Default"
        };
        _context.PackagingUnitItems.Add(item);
        await _context.SaveChangesAsync();
    }

    private async Task<Product> CreateProduct(string gtin, string name, string description,
        string? brand = null, string? productCategory = null, string? productSubCategory = null,
        decimal? productWeight = null, string? productWeightUnit = null, decimal? productVolume = null,
        string? productVolumeUnit = null, string? countryOfOrigin = null)
    {
        var sku = gtin;
        var existing = await _context.Products.FirstOrDefaultAsync(p => p.Sku == sku || (p.Gtin != null && p.Gtin == gtin));
        if (existing != null) return existing;

        var product = new Product
        {
            Sku = sku,
            Gtin = gtin,
            Name = name,
            Description = description,
            Brand = brand,
            ProductCategory = productCategory,
            ProductSubCategory = productSubCategory,
            ProductWeight = productWeight,
            ProductWeightUnit = productWeightUnit,
            ProductVolume = productVolume,
            ProductVolumeUnit = productVolumeUnit,
            CountryOfOrigin = countryOfOrigin,
            CreatedAt = DateTime.UtcNow
        };
        _context.Products.Add(product);
        await _context.SaveChangesAsync();
        return product;
    }

    private async Task LinkProductPackaging(int productId, int packagingUnitId)
    {
        var existing = await _context.ProductPackagings
            .FirstOrDefaultAsync(pp => pp.ProductId == productId && pp.PackagingUnitId == packagingUnitId);
        if (existing != null) return;

        var link = new ProductPackaging
        {
            ProductId = productId,
            PackagingUnitId = packagingUnitId
        };
        _context.ProductPackagings.Add(link);
        await _context.SaveChangesAsync();
    }

    private async Task<AsnShipment> CreateAsnShipment(AsnShipment shipment)
    {
        var existing = await _context.AsnShipments.FirstOrDefaultAsync(s => s.AsnNumber == shipment.AsnNumber);
        if (existing != null) return existing;

        _context.AsnShipments.Add(shipment);
        await _context.SaveChangesAsync();
        return shipment;
    }

    private async Task<AsnPallet> CreatePallet(int shipmentId, int sequence, string sscc, string packageType, 
        decimal? weight, string? destGln, string? destName, string? destAddress, string? destCity, 
        string? destPostal, string? destCountry)
    {
        var pallet = new AsnPallet
        {
            AsnShipmentId = shipmentId,
            SequenceNumber = sequence,
            Sscc = sscc,
            PackageTypeCode = packageType,
            GrossWeight = weight,
            DestinationGln = destGln ?? "",
            DestinationName = destName ?? "",
            DestinationAddress = destAddress,
            DestinationCity = destCity,
            DestinationPostalCode = destPostal,
            DestinationCountryCode = destCountry
        };
        _context.AsnPallets.Add(pallet);
        await _context.SaveChangesAsync();
        return pallet;
    }

    private async Task<AsnLineItem> CreateLineItem(int palletId, int lineNumber, string gtin, string description, 
        decimal quantity, string unit, string? batchNumber, DateTime? bestBefore)
    {
        var item = new AsnLineItem
        {
            AsnPalletId = palletId,
            LineNumber = lineNumber,
            Gtin = gtin,
            Description = description,
            Quantity = quantity,
            UnitOfMeasure = unit,
            BatchNumber = batchNumber,
            BestBeforeDate = bestBefore
        };
        _context.AsnLineItems.Add(item);
        await _context.SaveChangesAsync();
        return item;
    }

    /// <summary>
    /// Seeds only ProductForms and Distributions for reporting charts (call when products exist but reporting data is missing).
    /// </summary>
    public async Task SeedReportingDataAsync()
    {
        var productCount = await _context.Products.CountAsync();
        if (productCount == 0) return;

        var packagingTypesForForms = new[] { "Plastic Bottle", "Cardboard Case", "Glass Bottle", "Product Label", "Aluminum Wrap", "Plastic Tub", "Paper Sleeve", "Corrugated Case" };
        var states = new[] { "NSW", "VIC", "QLD", "WA", "SA", "TAS", "ACT", "NT" };
        var rnd = new Random(42);
        var baseDate = DateTime.UtcNow;
        var products = await _context.Products.OrderBy(p => p.Id).ToListAsync();
        var packUnitId = await _context.PackagingUnits.OrderBy(x => x.Id).Select(x => x.Id).FirstOrDefaultAsync();
        if (packUnitId == 0) packUnitId = await _context.PackagingUnits.Select(x => x.Id).FirstOrDefaultAsync();

        foreach (var (p, i) in products.Select((p, i) => (p, i)))
        {
            var hasForm = await _context.ProductForms.AnyAsync(f => f.ProductId == p.Id);
            if (!hasForm)
            {
                var packagingType = packagingTypesForForms[i % packagingTypesForForms.Length];
                var weight = 50m + (rnd.Next(1, 90) * 5m);
                _context.ProductForms.Add(new ProductForm
                {
                    ProductId = p.Id,
                    Gtin = p.Gtin ?? p.Sku,
                    ProductName = p.Name,
                    Brand = p.Brand ?? "Unknown",
                    ProductCategory = p.ProductCategory ?? "General",
                    ProductSubCategory = p.ProductSubCategory ?? "",
                    CountryOfOrigin = p.CountryOfOrigin ?? "GB",
                    PackagingLevel = "Consumer Unit",
                    PackagingType = packagingType,
                    PackagingConfiguration = "Single component",
                    TotalPackagingWeight = weight,
                    Status = "submitted",
                    CreatedAt = baseDate.AddDays(-rnd.Next(1, 180))
                });
            }
        }
        await _context.SaveChangesAsync();

        if (packUnitId != 0 && !await _context.Distributions.AnyAsync())
        {
            foreach (var p in products.Take(20))
            {
                for (var d = 0; d < rnd.Next(1, 4); d++)
                {
                    var state = states[rnd.Next(states.Length)];
                    var city = state == "NSW" ? "Sydney" : state == "VIC" ? "Melbourne" : state == "QLD" ? "Brisbane" : state == "WA" ? "Perth" : state == "SA" ? "Adelaide" : "Hobart";
                    _context.Distributions.Add(new Distribution
                    {
                        ProductId = p.Id,
                        PackagingUnitId = packUnitId,
                        Quantity = (decimal)rnd.Next(10, 500),
                        City = city,
                        StateProvince = state,
                        Country = "AU",
                        DispatchDate = baseDate.AddDays(-rnd.Next(1, 365)),
                        CreatedAt = baseDate
                    });
                }
            }
            await _context.SaveChangesAsync();
        }

        _logger.LogInformation("Reporting data (ProductForms, Distributions) seeded.");
    }
}
