using EPR.Data;
using EPR.Web.Seeders;
using Microsoft.EntityFrameworkCore;

namespace EPR.Web.Scripts;

/// <summary>
/// Deletes existing data for Electronics, Alcoholic Beverages, and Confectionary datasets,
/// then re-runs their seeders to apply the 80% Australia / 20% international split.
/// </summary>
public static class ReseedDatasets
{
    private static readonly string[] DatasetKeys = { "Electronics", "Alcoholic Beverages", "Confectionary" };

    public static async Task RunAsync(EPRDbContext context)
    {
        Console.WriteLine("Reseeding Electronics, Alcoholic Beverages, and Confectionary datasets...");
        Console.WriteLine("(Deleting existing data, then re-seeding with 80% AU / 20% international)");
        Console.WriteLine();

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

        await context.SaveChangesAsync();
        Console.WriteLine();

        foreach (var key in DatasetKeys)
        {
            try
            {
                if (key == "Electronics")
                {
                    var seeder = new ElectronicsDatasetSeeder(context);
                    await seeder.SeedAsync();
                }
                else if (key == "Alcoholic Beverages")
                {
                    var seeder = new AlcoholicBeveragesDatasetSeeder(context);
                    await seeder.SeedAsync();
                }
                else if (key == "Confectionary")
                {
                    var seeder = new ConfectionaryDatasetSeeder(context);
                    await seeder.SeedAsync();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"⚠ Seed {key}: {ex.Message}");
                throw;
            }
        }

        Console.WriteLine();
        Console.WriteLine("✅ Reseed complete. Electronics, Alcoholic Beverages, and Confectionary now use 80% AU / 20% international.");
    }

    private static async Task DeleteDatasetDataAsync(EPRDbContext context, string datasetKey)
    {
        // 1. Delete ASN data (AsnShipment cascades to AsnPallet -> AsnLineItem)
        var shipments = await context.AsnShipments
            .Where(s => s.DatasetKey == datasetKey)
            .ToListAsync();
        context.AsnShipments.RemoveRange(shipments);

        // 2. Delete Products (cascades to ProductForm, ProductPackaging, ProductPackagingSupplierProduct, Distribution)
        var products = await context.Products
            .Where(p => p.DatasetKey == datasetKey)
            .ToListAsync();
        context.Products.RemoveRange(products);

        // 3. Delete PackagingGroups (cascades to PackagingGroupItems)
        var groups = await context.PackagingGroups
            .Where(g => g.DatasetKey == datasetKey)
            .ToListAsync();
        context.PackagingGroups.RemoveRange(groups);

        // 4. Delete PackagingLibraries (cascades to PackagingLibraryMaterials, PackagingLibrarySupplierProducts)
        var libraries = await context.PackagingLibraries
            .Where(l => l.DatasetKey == datasetKey)
            .ToListAsync();
        context.PackagingLibraries.RemoveRange(libraries);

        // Distributions cascade-delete when Products are removed
    }
}
