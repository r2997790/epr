using EPR.Domain.Entities;
using OfficeOpenXml;
using Microsoft.EntityFrameworkCore;

namespace EPR.Data.Services;

/// <summary>
/// Service to import material taxonomy data from Excel file
/// </summary>
public class MaterialTaxonomyImportService
{
    private readonly EPRDbContext _context;
    
    public MaterialTaxonomyImportService(EPRDbContext context)
    {
        _context = context;
        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
    }
    
    /// <summary>
    /// Import taxonomy data from Excel file
    /// </summary>
    public async Task ImportFromExcelAsync(string filePath)
    {
        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException($"Excel file not found: {filePath}");
        }
        
        using var package = new ExcelPackage(new FileInfo(filePath));
        
        // Find Taxonomy sheet (try different possible names)
        var taxonomySheet = package.Workbook.Worksheets["Taxonomy"] 
            ?? package.Workbook.Worksheets.FirstOrDefault(ws => 
                ws.Name.Contains("Taxonomy", StringComparison.OrdinalIgnoreCase) ||
                ws.Name.Contains("Tax", StringComparison.OrdinalIgnoreCase));
        
        if (taxonomySheet == null && package.Workbook.Worksheets.Count > 0)
        {
            // Use first sheet if no Taxonomy sheet found
            taxonomySheet = package.Workbook.Worksheets[0];
            Console.WriteLine($"Warning: Using first sheet '{taxonomySheet.Name}' as Taxonomy sheet");
        }
        
        if (taxonomySheet == null)
        {
            throw new InvalidOperationException("No worksheets found in Excel file");
        }
        
        Console.WriteLine($"Importing from sheet: {taxonomySheet.Name}");
        
        // Clear existing data to avoid duplicates
        try
        {
            var existingTaxonomies = await _context.MaterialTaxonomies.ToListAsync();
            if (existingTaxonomies.Any())
            {
                Console.WriteLine($"Clearing {existingTaxonomies.Count} existing taxonomies...");
                _context.MaterialTaxonomies.RemoveRange(existingTaxonomies);
                await _context.SaveChangesAsync();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Warning: Could not clear existing taxonomies: {ex.Message}");
            // Continue anyway - might be first import
        }
        
        var taxonomies = new Dictionary<string, MaterialTaxonomy>();
        var level1Taxonomies = new List<MaterialTaxonomy>();
        
        // Read taxonomy data starting from row 2 (assuming row 1 is header)
        int startRow = 2;
        int maxRow = taxonomySheet.Dimension?.End.Row ?? startRow;
        
        Console.WriteLine($"Reading rows {startRow} to {maxRow}...");
        
        // Excel structure based on header:
        // Column 1: NodeType
        // Column 2: TaxonID (Code)
        // Column 3: ParentTaxonID
        // Column 4: Level
        // Column 5: HighLevelType
        // Column 6: DisplayName
        
        int importedCount = 0;
        for (int row = startRow; row <= maxRow; row++)
        {
            // Get Level from column 4
            var levelValue = taxonomySheet.Cells[row, 4].Value;
            if (levelValue == null) continue;
            
            int? level = null;
            if (levelValue is int intLevel)
            {
                level = intLevel;
            }
            else if (levelValue is double doubleLevel)
            {
                level = (int)doubleLevel;
            }
            else if (int.TryParse(levelValue.ToString(), out int parsedLevel))
            {
                level = parsedLevel;
            }
            else
            {
                continue; // Skip invalid rows
            }
            
            if (!level.HasValue || level.Value < 1 || level.Value > 5) 
                continue;
            
            // Column 2: TaxonID (Code)
            var code = taxonomySheet.Cells[row, 2].Value?.ToString()?.Trim();
            // Column 3: ParentTaxonID (for building relationships)
            var parentTaxonId = taxonomySheet.Cells[row, 3].Value?.ToString()?.Trim();
            // Column 6: DisplayName
            var displayName = taxonomySheet.Cells[row, 6].Value?.ToString()?.Trim();
            // Column 5: HighLevelType (use as Name)
            var name = taxonomySheet.Cells[row, 5].Value?.ToString()?.Trim();
            // Description and IconClass not in this sheet structure
            var description = "";
            var iconClass = GetDefaultIconForLevel1(displayName ?? "");
            
            if (string.IsNullOrEmpty(code) || string.IsNullOrEmpty(displayName))
            {
                if (row <= startRow + 2)
                {
                    Console.WriteLine($"Skipping row {row}: Missing code or displayName. Code='{code}', DisplayName='{displayName}'");
                }
                continue;
            }
            
            importedCount++;
            if (importedCount <= 5)
            {
                Console.WriteLine($"Importing row {row}: Level={level}, Code={code}, DisplayName={displayName}");
            }
            
            var taxonomy = new MaterialTaxonomy
            {
                Level = level.Value,
                Code = code ?? "",
                DisplayName = displayName ?? code ?? "",
                Name = name ?? displayName ?? code ?? "",
                Description = description,
                IconClass = iconClass,
                SortOrder = row - startRow + 1,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };
            
            _context.MaterialTaxonomies.Add(taxonomy);
            await _context.SaveChangesAsync();
            
            taxonomies[code ?? ""] = taxonomy;
            if (level.Value == 1)
            {
                level1Taxonomies.Add(taxonomy);
            }
            
            importedCount++;
        }
        
        Console.WriteLine($"Imported {taxonomies.Count} taxonomies (processed {importedCount} valid rows). Building parent relationships...");
        
        // Build parent relationships using ParentTaxonID from Excel
        foreach (var kvp in taxonomies)
        {
            var taxonomy = kvp.Value;
            if (taxonomy.Level > 1)
            {
                // Find parent by TaxonID (code) - need to look up parentTaxonId from the row
                // Since we've already saved, we need to re-read to get parentTaxonId
                // For now, use code-based parent lookup
                var parentCode = GetParentCode(taxonomy.Code, taxonomy.Level);
                if (!string.IsNullOrEmpty(parentCode) && taxonomies.ContainsKey(parentCode))
                {
                    taxonomy.ParentTaxonomyId = taxonomies[parentCode].Id;
                    await _context.SaveChangesAsync();
                }
            }
        }
        
        // Second pass: build relationships using ParentTaxonID from Excel
        for (int row = startRow; row <= maxRow; row++)
        {
            var code = taxonomySheet.Cells[row, 2].Value?.ToString()?.Trim();
            var parentTaxonId = taxonomySheet.Cells[row, 3].Value?.ToString()?.Trim();
            
            if (string.IsNullOrEmpty(code) || string.IsNullOrEmpty(parentTaxonId) || !taxonomies.ContainsKey(code))
                continue;
            
            var taxonomy = taxonomies[code];
            if (taxonomy.Level > 1 && taxonomies.ContainsKey(parentTaxonId))
            {
                taxonomy.ParentTaxonomyId = taxonomies[parentTaxonId].Id;
                await _context.SaveChangesAsync();
            }
        }
        
        // Import country requirements from other sheets
        foreach (var sheet in package.Workbook.Worksheets)
        {
            if (sheet.Name.Equals("Taxonomy", StringComparison.OrdinalIgnoreCase))
                continue;
            
            Console.WriteLine($"Processing sheet: {sheet.Name}");
            
            // Try to detect if this is a country requirements sheet
            if (sheet.Name.Contains("Country", StringComparison.OrdinalIgnoreCase) ||
                sheet.Name.Contains("Requirement", StringComparison.OrdinalIgnoreCase))
            {
                await ImportCountryRequirementsAsync(sheet, taxonomies);
            }
            // Try to detect fees sheet
            else if (sheet.Name.Contains("Fee", StringComparison.OrdinalIgnoreCase))
            {
                await ImportFeesAsync(sheet, taxonomies);
            }
        }
        
        Console.WriteLine("Import completed!");
    }
    
    private string? GetParentCode(string code, int level)
    {
        // Extract parent code based on code structure
        // This is a placeholder - adjust based on actual code structure in Excel
        if (level <= 1) return null;
        
        // Example: if code is "1.2.3", parent would be "1.2"
        var parts = code.Split('.');
        if (parts.Length >= level)
        {
            return string.Join(".", parts.Take(level - 1));
        }
        return null;
    }
    
    private string GetDefaultIconForLevel1(string displayName)
    {
        // Map common material types to Bootstrap icons
        var iconMap = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            { "Glass", "bi-circle" },
            { "Plastic", "bi-circle-fill" },
            { "Cardboard", "bi-box" },
            { "Paper", "bi-file-text" },
            { "Metal", "bi-square" },
            { "Wood", "bi-tree" },
            { "Textile", "bi-grid" },
            { "Composite", "bi-layers" }
        };
        
        foreach (var kvp in iconMap)
        {
            if (displayName.Contains(kvp.Key, StringComparison.OrdinalIgnoreCase))
            {
                return kvp.Value;
            }
        }
        
        return "bi-circle"; // Default icon
    }
    
    private async Task ImportCountryRequirementsAsync(ExcelWorksheet sheet, Dictionary<string, MaterialTaxonomy> taxonomies)
    {
        int startRow = 2;
        int maxRow = sheet.Dimension?.End.Row ?? startRow;
        int imported = 0;
        
        // Country_Classifications sheet structure:
        // Column 1: Country
        // Column 2: Scheme
        // Column 3: EffectiveFrom
        // Column 4: CategoryCode (TaxonID - the taxonomy code)
        // Column 5: CategoryName
        // Column 6: CategoryDefinition
        
        // Group by country and taxonomy to determine required levels
        var countryTaxonomyGroups = new Dictionary<string, Dictionary<string, int>>();
        
        for (int row = startRow; row <= maxRow; row++)
        {
            var country = sheet.Cells[row, 1].Value?.ToString()?.Trim();
            var categoryCode = sheet.Cells[row, 4].Value?.ToString()?.Trim(); // TaxonID
            
            if (string.IsNullOrEmpty(country) || string.IsNullOrEmpty(categoryCode))
                continue;
            
            // Find the taxonomy by code
            if (!taxonomies.ContainsKey(categoryCode))
                continue;
            
            var taxonomy = taxonomies[categoryCode];
            
            // Track the maximum level required for each country-taxonomy combination
            if (!countryTaxonomyGroups.ContainsKey(country))
                countryTaxonomyGroups[country] = new Dictionary<string, int>();
            
            if (!countryTaxonomyGroups[country].ContainsKey(categoryCode) || 
                countryTaxonomyGroups[country][categoryCode] < taxonomy.Level)
            {
                countryTaxonomyGroups[country][categoryCode] = taxonomy.Level;
            }
        }
        
        // Create requirements - for each country, find the Level 1 parent and set required level
        foreach (var countryGroup in countryTaxonomyGroups)
        {
            var countryName = countryGroup.Key;
            var countryCode = countryName.Length >= 2 ? countryName.Substring(0, 2).ToUpper() : countryName.ToUpper();
            
            // Group by Level 1 taxonomy (find parent)
            var level1Requirements = new Dictionary<string, int>();
            
            foreach (var taxonomyGroup in countryGroup.Value)
            {
                var taxonomyCode = taxonomyGroup.Key;
                var requiredLevel = taxonomyGroup.Value;
                
                if (!taxonomies.ContainsKey(taxonomyCode))
                    continue;
                
                var taxonomy = taxonomies[taxonomyCode];
                
                // Find the Level 1 parent
                var level1Taxonomy = taxonomy;
                while (level1Taxonomy.Level > 1 && level1Taxonomy.ParentTaxonomyId.HasValue)
                {
                    var parent = taxonomies.Values.FirstOrDefault(t => t.Id == level1Taxonomy.ParentTaxonomyId);
                    if (parent == null) break;
                    level1Taxonomy = parent;
                }
                
                if (level1Taxonomy.Level == 1)
                {
                    var level1Code = level1Taxonomy.Code;
                    if (!level1Requirements.ContainsKey(level1Code) || level1Requirements[level1Code] < requiredLevel)
                    {
                        level1Requirements[level1Code] = requiredLevel;
                    }
                }
            }
            
            // Create requirements for Level 1 taxonomies
            foreach (var req in level1Requirements)
            {
                if (!taxonomies.ContainsKey(req.Key))
                    continue;
                
                var requirement = new MaterialTaxonomyCountryRequirement
                {
                    MaterialTaxonomyId = taxonomies[req.Key].Id,
                    CountryCode = countryCode,
                    CountryName = countryName,
                    RequiredLevel = req.Value,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                };
                
                _context.MaterialTaxonomyCountryRequirements.Add(requirement);
                imported++;
            }
        }
        
        if (imported > 0)
        {
            await _context.SaveChangesAsync();
            Console.WriteLine($"Imported {imported} country requirements");
        }
        else
        {
            Console.WriteLine("No country requirements imported - check sheet structure");
        }
    }
    
    private async Task ImportFeesAsync(ExcelWorksheet sheet, Dictionary<string, MaterialTaxonomy> taxonomies)
    {
        // This is a placeholder for fees import if needed
        // You can extend this based on your fees structure
        Console.WriteLine($"Fees sheet '{sheet.Name}' detected but not yet implemented");
    }
}

