// Standalone tool to create admin user in database
// Compile with: csc CreateAdminUserTool.cs /r:"path\to\BCrypt.Net-Next.dll" /r:"path\to\System.Data.SQLite.dll"
// Or use: dotnet script CreateAdminUserTool.cs

using System;
using System.Data.SQLite;
using BCrypt.Net;

class CreateAdminUserTool
{
    static void Main(string[] args)
    {
        string dbPath = args.Length > 0 ? args[0] : "epr.db";
        
        Console.WriteLine("========================================");
        Console.WriteLine("Create Admin User Tool");
        Console.WriteLine("========================================");
        Console.WriteLine();
        
        if (!System.IO.File.Exists(dbPath))
        {
            Console.WriteLine($"ERROR: Database file not found: {dbPath}");
            Console.WriteLine("Please ensure the database has been created first.");
            return;
        }
        
        Console.WriteLine($"Database: {dbPath}");
        Console.WriteLine("Generating password hash...");
        
        string passwordHash = BCrypt.HashPassword("admin123");
        Console.WriteLine($"Password hash generated.");
        Console.WriteLine();
        
        string connectionString = $"Data Source={dbPath};Version=3;";
        
        using (var connection = new SQLiteConnection(connectionString))
        {
            connection.Open();
            
            // Check if user already exists
            using (var checkCmd = new SQLiteCommand(
                "SELECT COUNT(*) FROM Users WHERE Username = 'admin' OR Email = 'admin@epr.local'",
                connection))
            {
                long count = (long)checkCmd.ExecuteScalar();
                if (count > 0)
                {
                    Console.WriteLine("ℹ️  Admin user already exists in database.");
                    Console.WriteLine("   Username: admin");
                    Console.WriteLine("   Password: admin123");
                    return;
                }
            }
            
            // Insert admin user
            using (var insertCmd = new SQLiteCommand(
                @"INSERT INTO Users (Username, Email, PasswordHash, FirstName, LastName, IsActive, CreatedAt)
                  VALUES (@username, @email, @passwordHash, @firstName, @lastName, @isActive, @createdAt)",
                connection))
            {
                insertCmd.Parameters.AddWithValue("@username", "admin");
                insertCmd.Parameters.AddWithValue("@email", "admin@epr.local");
                insertCmd.Parameters.AddWithValue("@passwordHash", passwordHash);
                insertCmd.Parameters.AddWithValue("@firstName", "System");
                insertCmd.Parameters.AddWithValue("@lastName", "Administrator");
                insertCmd.Parameters.AddWithValue("@isActive", 1);
                insertCmd.Parameters.AddWithValue("@createdAt", DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"));
                
                insertCmd.ExecuteNonQuery();
            }
            
            Console.WriteLine("✅ Admin user created successfully!");
            Console.WriteLine();
            Console.WriteLine("Default credentials:");
            Console.WriteLine("  Username: admin");
            Console.WriteLine("  Password: admin123");
        }
    }
}
















