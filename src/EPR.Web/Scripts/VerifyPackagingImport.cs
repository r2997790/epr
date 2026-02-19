using EPR.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;

namespace EPR.Web.Scripts;

public class VerifyPackagingImport
{
    public static async Task<int> RunAsync()
    {
        var builder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
        var configuration = builder.Build();

        var services = new ServiceCollection();
        services.AddDbContext<EPRDbContext>(options =>
            options.UseSqlite(configuration.GetConnectionString("DefaultConnection") ?? "Data Source=epr.db"));

        await using (var serviceProvider = services.BuildServiceProvider())
        {
            using (var scope = serviceProvider.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<EPRDbContext>();
                
                Console.WriteLine("=== Packaging Import Verification ===\n");
                
                // Count totals
                var totalGroups = await context.PackagingGroups.CountAsync();
                var totalLibraryItems = await context.PackagingLibraries.CountAsync();
                var totalGroupItems = await context.PackagingGroupItems.CountAsync();
                
                Console.WriteLine($"Total Packaging Groups: {totalGroups}");
                Console.WriteLine($"Total Packaging Library Items: {totalLibraryItems}");
                Console.WriteLine($"Total Group-Item Relationships: {totalGroupItems}\n");
                
                // Show sample groups
                Console.WriteLine("=== Sample Packaging Groups (first 5) ===");
                var sampleGroups = await context.PackagingGroups
                    .Include(g => g.Items)
                        .ThenInclude(i => i.PackagingLibrary)
                    .Take(5)
                    .ToListAsync();
                
                foreach (var group in sampleGroups)
                {
                    Console.WriteLine($"\nGroup: {group.Name} (PackID: {group.PackId})");
                    Console.WriteLine($"  Packaging Layer: {group.PackagingLayer ?? "N/A"}");
                    Console.WriteLine($"  Style: {group.Style ?? "N/A"}");
                    Console.WriteLine($"  Shape: {group.Shape ?? "N/A"}");
                    Console.WriteLine($"  Size: {group.Size ?? "N/A"}");
                    Console.WriteLine($"  Volume Dimensions: {group.VolumeDimensions ?? "N/A"}");
                    Console.WriteLine($"  Total Pack Weight: {group.TotalPackWeight?.ToString() ?? "N/A"}g");
                    Console.WriteLine($"  Items ({group.Items.Count}):");
                    foreach (var item in group.Items.OrderBy(i => i.SortOrder))
                    {
                        Console.WriteLine($"    - {item.PackagingLibrary.Name} (Taxonomy: {item.PackagingLibrary.TaxonomyCode}, Weight: {item.PackagingLibrary.Weight?.ToString() ?? "N/A"}g)");
                    }
                }
                
                // Show sample library items
                Console.WriteLine("\n=== Sample Packaging Library Items (first 5) ===");
                var sampleLibraryItems = await context.PackagingLibraries
                    .Include(l => l.MaterialTaxonomy)
                    .Take(5)
                    .ToListAsync();
                
                foreach (var item in sampleLibraryItems)
                {
                    Console.WriteLine($"\n{item.Name} (Taxonomy Code: {item.TaxonomyCode})");
                    Console.WriteLine($"  Weight: {item.Weight?.ToString() ?? "N/A"}g");
                    Console.WriteLine($"  Material Taxonomy: {item.MaterialTaxonomy?.DisplayName ?? "Not linked"}");
                }
                
                // Check for groups with no items
                var groupsWithNoItems = await context.PackagingGroups
                    .Where(g => !g.Items.Any())
                    .CountAsync();
                
                if (groupsWithNoItems > 0)
                {
                    Console.WriteLine($"\n⚠️  Warning: {groupsWithNoItems} packaging groups have no items");
                }
                
                // Check for library items not in any group
                var orphanedItems = await context.PackagingLibraries
                    .Where(l => !l.PackagingGroupItems.Any())
                    .CountAsync();
                
                if (orphanedItems > 0)
                {
                    Console.WriteLine($"⚠️  Warning: {orphanedItems} packaging library items are not in any group");
                }
                
                Console.WriteLine("\n✅ Verification complete!");
            }
        }
        return 0;
    }
}






