using EPR.Data;
using EPR.Data.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace EPR.Web.Scripts;

/// <summary>
/// Script to import material taxonomy data from Excel file
/// Run this with: dotnet run --project src/EPR.Web -- import-taxonomy "path/to/file.xlsx"
/// </summary>
public class ImportMaterialTaxonomyScript
{
    public static async Task RunAsync(string excelFilePath)
    {
        Console.WriteLine("Starting Material Taxonomy Import...");
        Console.WriteLine($"Excel File: {excelFilePath}");
        
        if (!File.Exists(excelFilePath))
        {
            Console.WriteLine($"ERROR: File not found: {excelFilePath}");
            return;
        }
        
        // Setup services
        var host = Host.CreateDefaultBuilder()
            .ConfigureServices((context, services) =>
            {
                var connectionString = context.Configuration.GetConnectionString("DefaultConnection") 
                    ?? "Data Source=epr.db";
                
                services.AddDbContext<EPRDbContext>(options =>
                    options.UseSqlite(connectionString));
                
                services.AddScoped<MaterialTaxonomyImportService>();
            })
            .Build();
        
        using (var scope = host.Services.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<EPRDbContext>();
            var importService = scope.ServiceProvider.GetRequiredService<MaterialTaxonomyImportService>();
            
            try
            {
                // Ensure database is created
                Console.WriteLine("Ensuring database exists...");
                await context.Database.EnsureCreatedAsync();
                
                // Manually create MaterialTaxonomies table if it doesn't exist
                await context.Database.ExecuteSqlRawAsync(@"
                    CREATE TABLE IF NOT EXISTS MaterialTaxonomies (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        Level INTEGER NOT NULL,
                        Code TEXT NOT NULL,
                        DisplayName TEXT NOT NULL,
                        Name TEXT,
                        Description TEXT,
                        IconClass TEXT,
                        ParentTaxonomyId INTEGER,
                        SortOrder INTEGER NOT NULL,
                        IsActive INTEGER NOT NULL DEFAULT 1,
                        CreatedAt TEXT NOT NULL,
                        UpdatedAt TEXT,
                        FOREIGN KEY (ParentTaxonomyId) REFERENCES MaterialTaxonomies(Id)
                    );
                    CREATE UNIQUE INDEX IF NOT EXISTS IX_MaterialTaxonomies_Level_Code ON MaterialTaxonomies(Level, Code);
                    CREATE INDEX IF NOT EXISTS IX_MaterialTaxonomies_ParentTaxonomyId ON MaterialTaxonomies(ParentTaxonomyId);
                ");
                
                // Create MaterialTaxonomyCountryRequirements table
                await context.Database.ExecuteSqlRawAsync(@"
                    CREATE TABLE IF NOT EXISTS MaterialTaxonomyCountryRequirements (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        MaterialTaxonomyId INTEGER NOT NULL,
                        CountryCode TEXT NOT NULL,
                        CountryName TEXT NOT NULL,
                        RequiredLevel INTEGER NOT NULL,
                        IsActive INTEGER NOT NULL DEFAULT 1,
                        CreatedAt TEXT NOT NULL,
                        UpdatedAt TEXT,
                        FOREIGN KEY (MaterialTaxonomyId) REFERENCES MaterialTaxonomies(Id) ON DELETE CASCADE
                    );
                    CREATE UNIQUE INDEX IF NOT EXISTS IX_MaterialTaxonomyCountryRequirements_MaterialTaxonomyId_CountryCode 
                        ON MaterialTaxonomyCountryRequirements(MaterialTaxonomyId, CountryCode);
                ");
                
                Console.WriteLine("Database tables verified.");
                
                // Import the data
                Console.WriteLine("Importing taxonomy data...");
                await importService.ImportFromExcelAsync(excelFilePath);
                
                Console.WriteLine("✅ Import completed successfully!");
                
                // Display summary
                var taxonomyCount = await context.MaterialTaxonomies.CountAsync();
                var level1Count = await context.MaterialTaxonomies.CountAsync(t => t.Level == 1);
                var requirementsCount = await context.MaterialTaxonomyCountryRequirements.CountAsync();
                
                Console.WriteLine($"\nImport Summary:");
                Console.WriteLine($"  Total Taxonomies: {taxonomyCount}");
                Console.WriteLine($"  Level 1 Taxonomies: {level1Count}");
                Console.WriteLine($"  Country Requirements: {requirementsCount}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ERROR: {ex.Message}");
                Console.WriteLine($"Stack Trace: {ex.StackTrace}");
                throw;
            }
        }
    }
}

