using EPR.Data;
using Microsoft.EntityFrameworkCore;

namespace EPR.Web.Scripts;

public static class InitializeAsnDatabase
{
    public static async Task RunAsync()
    {
        Console.WriteLine("Initializing ASN Database...");
        
        var optionsBuilder = new DbContextOptionsBuilder<EPRDbContext>();
        optionsBuilder.UseSqlite("Data Source=epr.db");
        
        using var context = new EPRDbContext(optionsBuilder.Options);
        
        try
        {
            // Ensure database is created
            Console.WriteLine("Ensuring database exists...");
            await context.Database.EnsureCreatedAsync();
            Console.WriteLine("✓ Database created/verified");
            
            // Check if ASN tables exist by trying to query them
            Console.WriteLine("Checking ASN tables...");
            var shipmentCount = await context.AsnShipments.CountAsync();
            Console.WriteLine($"✓ AsnShipments table exists with {shipmentCount} records");
            
            var palletCount = await context.AsnPallets.CountAsync();
            Console.WriteLine($"✓ AsnPallets table exists with {palletCount} records");
            
            var itemCount = await context.AsnLineItems.CountAsync();
            Console.WriteLine($"✓ AsnLineItems table exists with {itemCount} records");
            
            Console.WriteLine("\n✅ ASN Database initialization complete!");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Error initializing database: {ex.Message}");
            Console.WriteLine($"Stack trace: {ex.StackTrace}");
            throw;
        }
    }
}
