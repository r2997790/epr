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
        var adminPassword = "admin123";
        var adminHash = BCrypt.Net.BCrypt.HashPassword(adminPassword);

        var adminUser = await _context.Users
            .FirstOrDefaultAsync(u => u.Username == "admin" || u.Email == "admin@epr.local");

        if (adminUser == null)
        {
            adminUser = new User
            {
                Username = "admin",
                Email = "admin@epr.local",
                PasswordHash = adminHash,
                FirstName = "System",
                LastName = "Administrator",
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };
            _context.Users.Add(adminUser);
            await _context.SaveChangesAsync();
            Console.WriteLine("✅ Admin user created (admin / admin123)");
        }
        else
        {
            // Always ensure password is correct (fixes corruption, BCrypt version drift, etc.)
            adminUser.PasswordHash = adminHash;
            adminUser.IsActive = true;
            await _context.SaveChangesAsync();
            Console.WriteLine("✅ Admin user verified/reset (admin / admin123)");
        }
    }
}
















