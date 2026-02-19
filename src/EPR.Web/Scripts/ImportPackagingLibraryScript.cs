using EPR.Data;
using EPR.Data.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;

namespace EPR.Web.Scripts;

public class ImportPackagingLibraryScript
{
    public static async Task<int> RunAsync(string excelFilePath)
    {
        var builder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
        var configuration = builder.Build();

        var services = new ServiceCollection();
        services.AddDbContext<EPRDbContext>(options =>
            options.UseSqlite(configuration.GetConnectionString("DefaultConnection") ?? "Data Source=epr.db"));
        services.AddScoped<PackagingLibraryImportService>();

        await using (var serviceProvider = services.BuildServiceProvider())
        {
            using (var scope = serviceProvider.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<EPRDbContext>();
                Console.WriteLine("Ensuring database tables exist...");
                
                // Manually create tables if they don't exist
                context.Database.ExecuteSqlRaw(@"
                    CREATE TABLE IF NOT EXISTS PackagingLibraries (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        TaxonomyCode TEXT NOT NULL,
                        Name TEXT NOT NULL,
                        Weight REAL,
                        MaterialTaxonomyId INTEGER,
                        IsActive INTEGER NOT NULL DEFAULT 1,
                        CreatedAt TEXT NOT NULL,
                        UpdatedAt TEXT,
                        FOREIGN KEY (MaterialTaxonomyId) REFERENCES MaterialTaxonomies(Id)
                    );
                    CREATE INDEX IF NOT EXISTS IX_PackagingLibraries_TaxonomyCode ON PackagingLibraries(TaxonomyCode);
                ");
                
                context.Database.ExecuteSqlRaw(@"
                    CREATE TABLE IF NOT EXISTS PackagingGroups (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        PackId TEXT NOT NULL,
                        Name TEXT NOT NULL,
                        PackagingLayer TEXT,
                        Style TEXT,
                        Shape TEXT,
                        Size TEXT,
                        VolumeDimensions TEXT,
                        ColoursAvailable TEXT,
                        RecycledContent TEXT,
                        TotalPackWeight REAL,
                        WeightBasis TEXT,
                        Notes TEXT,
                        ExampleReference TEXT,
                        Source TEXT,
                        Url TEXT,
                        IsActive INTEGER NOT NULL DEFAULT 1,
                        CreatedAt TEXT NOT NULL,
                        UpdatedAt TEXT
                    );
                    CREATE UNIQUE INDEX IF NOT EXISTS IX_PackagingGroups_PackId ON PackagingGroups(PackId);
                ");
                
                context.Database.ExecuteSqlRaw(@"
                    CREATE TABLE IF NOT EXISTS PackagingGroupItems (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        PackagingGroupId INTEGER NOT NULL,
                        PackagingLibraryId INTEGER NOT NULL,
                        SortOrder INTEGER NOT NULL,
                        CreatedAt TEXT NOT NULL,
                        FOREIGN KEY (PackagingGroupId) REFERENCES PackagingGroups(Id) ON DELETE CASCADE,
                        FOREIGN KEY (PackagingLibraryId) REFERENCES PackagingLibraries(Id)
                    );
                    CREATE UNIQUE INDEX IF NOT EXISTS IX_PackagingGroupItems_PackagingGroupId_PackagingLibraryId 
                        ON PackagingGroupItems(PackagingGroupId, PackagingLibraryId);
                ");
                
                Console.WriteLine("Database tables verified.");

                var importService = scope.ServiceProvider.GetRequiredService<PackagingLibraryImportService>();
                Console.WriteLine("Importing packaging library data...");
                await importService.ImportFromExcelAsync(excelFilePath);
                Console.WriteLine("✅ Import completed successfully!");

                // Print summary
                var totalGroups = await context.PackagingGroups.CountAsync();
                var totalLibraryItems = await context.PackagingLibraries.CountAsync();
                var totalGroupItems = await context.PackagingGroupItems.CountAsync();
                Console.WriteLine("\nImport Summary:");
                Console.WriteLine($"  Total Packaging Groups: {totalGroups}");
                Console.WriteLine($"  Total Packaging Library Items: {totalLibraryItems}");
                Console.WriteLine($"  Total Group-Item Relationships: {totalGroupItems}");
            }
        }
        return 0;
    }
}






