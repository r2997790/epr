using EPR.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using EPR.Data;

namespace EPR.Web.Seeders;

/// <summary>
/// Seeds default users into the database
/// </summary>
public class UserSeeder
{
    private readonly EPRDbContext _context;

    public UserSeeder(EPRDbContext context)
    {
        _context = context;
    }

    public async Task SeedAsync()
    {
        // Check if admin user already exists
        var adminUser = await _context.Users
            .FirstOrDefaultAsync(u => u.Username == "admin" || u.Email == "admin@epr.local");

        if (adminUser == null)
        {
            // Create admin user
            adminUser = new User
            {
                Username = "admin",
                Email = "admin@epr.local",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("admin123"),
                FirstName = "System",
                LastName = "Administrator",
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            _context.Users.Add(adminUser);
            await _context.SaveChangesAsync();

            Console.WriteLine("✅ Admin user created successfully!");
            Console.WriteLine($"   Username: admin");
            Console.WriteLine($"   Password: admin123");
        }
        else
        {
            Console.WriteLine("ℹ️  Admin user already exists in database.");
            
            // Optionally reset the password if needed
            // Uncomment the following lines to reset the admin password:
            // adminUser.PasswordHash = BCrypt.Net.BCrypt.HashPassword("admin123");
            // await _context.SaveChangesAsync();
            // Console.WriteLine("✅ Admin password has been reset to: admin123");
        }
    }
}
















