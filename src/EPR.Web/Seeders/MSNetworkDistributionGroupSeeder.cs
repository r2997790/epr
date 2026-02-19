using EPR.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using EPR.Data;

namespace EPR.Web.Seeders;

/// <summary>
/// Seeds the M&S UK Network distribution group into the database
/// </summary>
public class MSNetworkDistributionGroupSeeder
{
    private readonly EPRDbContext _context;
    private readonly string _excelFilePath;

    public MSNetworkDistributionGroupSeeder(EPRDbContext context, string excelFilePath)
    {
        _context = context;
        _excelFilePath = excelFilePath;
    }

    public async Task SeedAsync()
    {
        // Check if already seeded and delete if exists (to allow re-import)
        var existingProject = await _context.VisualEditorProjects
            .FirstOrDefaultAsync(p => p.Key == "ms-uk-network");

        if (existingProject != null)
        {
            Console.WriteLine("M&S UK Network already exists in database. Deleting existing project to re-import...");
            _context.VisualEditorProjects.Remove(existingProject);
            await _context.SaveChangesAsync();
            Console.WriteLine("Existing project deleted.");
        }

        // Generate the project JSON using the import script
        Console.WriteLine("Generating distribution group from Excel file...");
        var json = await EPR.Web.Scripts.ImportMSNetworkDistributionGroup.RunAsync(_excelFilePath);

        // Create the VisualEditorProject entity
        var project = new VisualEditorProject
        {
            Key = "ms-uk-network",
            Name = "M&S UK Network",
            ProjectDataJson = json,
            CreatedAt = DateTime.UtcNow
        };

        _context.VisualEditorProjects.Add(project);
        await _context.SaveChangesAsync();

        Console.WriteLine($"✅ M&S UK Network distribution group seeded successfully!");
        Console.WriteLine($"   Project ID: {project.Id}");
        Console.WriteLine($"   Project Key: {project.Key}");
    }
}


