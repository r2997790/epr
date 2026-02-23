using EPR.Data;
using EPR.Web.Seeders;
using Microsoft.EntityFrameworkCore;

namespace EPR.Web.Scripts;

/// <summary>
/// Deletes existing data for all dataset seeders, then re-runs them to apply
/// the packaging hierarchy (ParentPackagingGroupId, QuantityInParent) and
/// 80% Australia / 20% international split.
/// </summary>
public static class ReseedDatasets
{
    private static readonly string[] DatasetKeys = {
        "Electronics", "Alcoholic Beverages", "Confectionary",
        "Homewares", "Pharmaceuticals", "Pet Care",
        "Personal Care", "Garden", "Fresh Produce"
    };

    public static async Task RunAsync(EPRDbContext context)
    {
        Console.WriteLine("Reseeding all datasets...");
        Console.WriteLine();

        // Delete stale dummy PackagingGroups (null DatasetKey) so the dummy seeder
        // can recreate them with correct ParentPackagingGroupId on next startup.
        try
        {
            await DeleteDummyPackagingGroupsAsync(context);
            Console.WriteLine("✓ Deleted stale dummy packaging groups");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"⚠ Delete dummy groups: {ex.Message}");
        }

        foreach (var key in DatasetKeys)
        {
            try
            {
                await DeleteDatasetDataAsync(context, key);
                Console.WriteLine($"✓ Deleted existing {key} data");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"⚠ Delete {key}: {ex.Message}");
            }
        }

        Console.WriteLine();

        foreach (var key in DatasetKeys)
        {
            try
            {
                switch (key)
                {
                    case "Electronics":
                        await new ElectronicsDatasetSeeder(context).SeedAsync(); break;
                    case "Alcoholic Beverages":
                        await new AlcoholicBeveragesDatasetSeeder(context).SeedAsync(); break;
                    case "Confectionary":
                        await new ConfectionaryDatasetSeeder(context).SeedAsync(); break;
                    case "Homewares":
                        await new HomewaresDatasetSeeder(context).SeedAsync(); break;
                    case "Pharmaceuticals":
                        await new PharmaceuticalsDatasetSeeder(context).SeedAsync(); break;
                    case "Pet Care":
                        await new PetCareDatasetSeeder(context).SeedAsync(); break;
                    case "Personal Care":
                        await new PersonalCareDatasetSeeder(context).SeedAsync(); break;
                    case "Garden":
                        await new GardenDatasetSeeder(context).SeedAsync(); break;
                    case "Fresh Produce":
                        await new FreshProduceDatasetSeeder(context).SeedAsync(); break;
                }
                Console.WriteLine($"✓ Seeded {key}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"⚠ Seed {key}: {ex.Message}");
                throw;
            }
        }

        Console.WriteLine();
        Console.WriteLine("✅ Reseed complete for all datasets.");
    }

    private static async Task DeleteDatasetDataAsync(EPRDbContext context, string datasetKey)
    {
        // 1. Delete ASN data (AsnShipment cascades to AsnPallet -> AsnLineItem)
        var shipments = await context.AsnShipments
            .Where(s => s.DatasetKey == datasetKey)
            .ToListAsync();
        context.AsnShipments.RemoveRange(shipments);
        await context.SaveChangesAsync();

        // 2. Delete Products (cascades to ProductForm, ProductPackaging, Distribution)
        var products = await context.Products
            .Where(p => p.DatasetKey == datasetKey)
            .ToListAsync();
        context.Products.RemoveRange(products);
        await context.SaveChangesAsync();

        // 3. Delete PackagingGroupItems, then PackagingGroups (null out self-FK first)
        var groups = await context.PackagingGroups
            .Where(g => g.DatasetKey == datasetKey)
            .Include(g => g.Items)
            .ToListAsync();
        var groupItems = groups.SelectMany(g => g.Items).ToList();
        context.PackagingGroupItems.RemoveRange(groupItems);

        foreach (var g in groups)
            g.ParentPackagingGroupId = null;
        await context.SaveChangesAsync();

        context.PackagingGroups.RemoveRange(groups);
        await context.SaveChangesAsync();

        // 4. Now safe to delete PackagingLibraries
        var libraries = await context.PackagingLibraries
            .Where(l => l.DatasetKey == datasetKey)
            .ToListAsync();
        if (libraries.Any())
        {
            var libIds = libraries.Select(l => l.Id).ToHashSet();
            var libMaterials = await context.PackagingLibraryMaterials
                .Where(m => libIds.Contains(m.PackagingLibraryId))
                .ToListAsync();
            context.PackagingLibraryMaterials.RemoveRange(libMaterials);

            var libSupplierProducts = await context.PackagingLibrarySupplierProducts
                .Where(sp => libIds.Contains(sp.PackagingLibraryId))
                .ToListAsync();
            context.PackagingLibrarySupplierProducts.RemoveRange(libSupplierProducts);

            context.PackagingLibraries.RemoveRange(libraries);
            await context.SaveChangesAsync();
        }
    }

    private static async Task DeleteDummyPackagingGroupsAsync(EPRDbContext context)
    {
        // 1. Delete dummy PackagingGroups (null DatasetKey) and their items first,
        //    because PackagingGroupItems FK-reference PackagingLibraries.
        var dummyGroups = await context.PackagingGroups
            .Where(g => g.DatasetKey == null)
            .Include(g => g.Items)
            .ToListAsync();

        if (dummyGroups.Any())
        {
            var allGroupItems = dummyGroups.SelectMany(g => g.Items).ToList();
            context.PackagingGroupItems.RemoveRange(allGroupItems);

            foreach (var g in dummyGroups)
                g.ParentPackagingGroupId = null;
            await context.SaveChangesAsync();

            context.PackagingGroups.RemoveRange(dummyGroups);
            await context.SaveChangesAsync();
        }

        // 2. Now safe to delete dummy PackagingLibraries (null DatasetKey)
        var dummyLibraries = await context.PackagingLibraries
            .Where(l => l.DatasetKey == null)
            .ToListAsync();

        if (dummyLibraries.Any())
        {
            var dummyLibIds = dummyLibraries.Select(l => l.Id).ToHashSet();
            var dummyLibMaterials = await context.PackagingLibraryMaterials
                .Where(m => dummyLibIds.Contains(m.PackagingLibraryId))
                .ToListAsync();
            context.PackagingLibraryMaterials.RemoveRange(dummyLibMaterials);

            var dummyLibSupplierProducts = await context.PackagingLibrarySupplierProducts
                .Where(sp => dummyLibIds.Contains(sp.PackagingLibraryId))
                .ToListAsync();
            context.PackagingLibrarySupplierProducts.RemoveRange(dummyLibSupplierProducts);

            context.PackagingLibraries.RemoveRange(dummyLibraries);
            await context.SaveChangesAsync();
        }
    }
}
