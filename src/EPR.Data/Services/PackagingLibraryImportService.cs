using EPR.Domain.Entities;
using OfficeOpenXml;
using Microsoft.EntityFrameworkCore;

namespace EPR.Data.Services;

/// <summary>
/// Service to import packaging library and packaging groups data from Excel file
/// </summary>
public class PackagingLibraryImportService
{
    private readonly EPRDbContext _context;
    
    public PackagingLibraryImportService(EPRDbContext context)
    {
        _context = context;
        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
    }
    
    /// <summary>
    /// Import packaging library and groups from Excel file
    /// </summary>
    public async Task ImportFromExcelAsync(string filePath)
    {
        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException($"Excel file not found: {filePath}");
        }
        
        using var package = new ExcelPackage(new FileInfo(filePath));
        
        // Find Packaging_Library sheet
        var packagingSheet = package.Workbook.Worksheets["Packaging_Library"] 
            ?? package.Workbook.Worksheets.FirstOrDefault(ws => 
                ws.Name.Contains("Packaging", StringComparison.OrdinalIgnoreCase) &&
                ws.Name.Contains("Library", StringComparison.OrdinalIgnoreCase));
        
        if (packagingSheet == null)
        {
            throw new InvalidOperationException("Packaging_Library sheet not found in Excel file");
        }
        
        Console.WriteLine($"Importing from sheet: {packagingSheet.Name}");
        
        // Clear existing data to avoid duplicates
        var existingGroups = await _context.PackagingGroups.ToListAsync();
        var existingLibraries = await _context.PackagingLibraries.ToListAsync();
        var existingGroupItems = await _context.PackagingGroupItems.ToListAsync();
        
        if (existingGroupItems.Any())
        {
            _context.PackagingGroupItems.RemoveRange(existingGroupItems);
            await _context.SaveChangesAsync();
        }
        if (existingGroups.Any())
        {
            _context.PackagingGroups.RemoveRange(existingGroups);
            await _context.SaveChangesAsync();
        }
        if (existingLibraries.Any())
        {
            _context.PackagingLibraries.RemoveRange(existingLibraries);
            await _context.SaveChangesAsync();
        }
        
        // Read header row to find column indices
        int headerRow = 1;
        int maxRow = packagingSheet.Dimension?.End.Row ?? headerRow;
        int maxCol = packagingSheet.Dimension?.End.Column ?? 0;
        
        Dictionary<string, int> columnMap = new Dictionary<string, int>();
        for (int col = 1; col <= maxCol; col++)
        {
            var headerValue = packagingSheet.Cells[headerRow, col].GetValue<string>()?.Trim();
            if (!string.IsNullOrEmpty(headerValue))
            {
                columnMap[headerValue] = col;
                Console.WriteLine($"Column {col}: {headerValue}");
            }
        }
        
        // Map column names (case-insensitive)
        int GetColumnIndex(string[] possibleNames)
        {
            foreach (var name in possibleNames)
            {
                var key = columnMap.Keys.FirstOrDefault(k => 
                    k.Equals(name, StringComparison.OrdinalIgnoreCase) ||
                    k.Contains(name, StringComparison.OrdinalIgnoreCase));
                if (key != null)
                {
                    return columnMap[key];
                }
            }
            return -1;
        }
        
        int packIdCol = GetColumnIndex(new[] { "PackID", "Pack ID" });
        int nameCol = GetColumnIndex(new[] { "Packaging Name", "Name" });
        int packagingGroupCol = GetColumnIndex(new[] { "Packaging Group", "Packaging Layer" });
        int materialsCol = GetColumnIndex(new[] { "Materials", "Materials (TaxonID + weight)" });
        int styleCol = GetColumnIndex(new[] { "Style" });
        int shapeCol = GetColumnIndex(new[] { "Shape" });
        int sizeVolumeCol = GetColumnIndex(new[] { "Size/Volume Dimensions", "Size/Volume", "Size", "Volume Dimensions" });
        int dimensionsCol = GetColumnIndex(new[] { "Dimensions" });
        int coloursCol = GetColumnIndex(new[] { "Colours Available", "Colours" });
        int recycledCol = GetColumnIndex(new[] { "Recycled Content", "Recycled" });
        int totalWeightCol = GetColumnIndex(new[] { "Total Pack Weight (g)", "Total Pack Weight", "Weight" });
        int weightBasisCol = GetColumnIndex(new[] { "Weight Basis" });
        int notesCol = GetColumnIndex(new[] { "Notes" });
        int exampleRefCol = GetColumnIndex(new[] { "Example reference", "Example Reference" });
        int sourceCol = GetColumnIndex(new[] { "Source" });
        int urlCol = GetColumnIndex(new[] { "Source URL", "URL", "Url" });
        
        // Dictionary to store packaging library items by unique key (taxonomyCode + name)
        Dictionary<string, PackagingLibrary> libraryItems = new Dictionary<string, PackagingLibrary>();
        
        int importedGroups = 0;
        int importedLibraryItems = 0;
        
        // Read data starting from row 2
        for (int row = 2; row <= maxRow; row++)
        {
            var packId = packIdCol > 0 ? packagingSheet.Cells[row, packIdCol].GetValue<string>()?.Trim() : null;
            var packagingName = nameCol > 0 ? packagingSheet.Cells[row, nameCol].GetValue<string>()?.Trim() : null;
            
            if (string.IsNullOrEmpty(packId) || string.IsNullOrEmpty(packagingName))
            {
                Console.WriteLine($"Skipping row {row}: Missing PackID or Packaging Name");
                continue;
            }
            
            // Parse Size/Volume Dimensions - split into Size and VolumeDimensions
            string? size = null;
            string? volumeDimensions = null;
            
            // Check if Dimensions is a separate column
            if (dimensionsCol > 0)
            {
                volumeDimensions = packagingSheet.Cells[row, dimensionsCol].GetValue<string>()?.Trim();
            }
            
            if (sizeVolumeCol > 0)
            {
                var sizeVolume = packagingSheet.Cells[row, sizeVolumeCol].GetValue<string>()?.Trim();
                if (!string.IsNullOrEmpty(sizeVolume))
                {
                    // If Dimensions column exists separately, use Size/Volume as size
                    if (dimensionsCol > 0)
                    {
                        size = sizeVolume;
                    }
                    else
                    {
                        // Try to split by common delimiters
                        var parts = sizeVolume.Split(new[] { ';', '|', '/', '\\' }, StringSplitOptions.RemoveEmptyEntries);
                        if (parts.Length >= 2)
                        {
                            size = parts[0].Trim();
                            if (string.IsNullOrEmpty(volumeDimensions))
                            {
                                volumeDimensions = parts[1].Trim();
                            }
                        }
                        else
                        {
                            // If no delimiter, check if it contains "volume" or "dimensions"
                            if (sizeVolume.Contains("volume", StringComparison.OrdinalIgnoreCase) ||
                                sizeVolume.Contains("dimensions", StringComparison.OrdinalIgnoreCase))
                            {
                                volumeDimensions = sizeVolume;
                            }
                            else
                            {
                                size = sizeVolume;
                            }
                        }
                    }
                }
            }
            
            // Create Packaging Group
            var packagingGroup = new PackagingGroup
            {
                PackId = packId,
                Name = packagingName,
                PackagingLayer = packagingGroupCol > 0 ? packagingSheet.Cells[row, packagingGroupCol].GetValue<string>()?.Trim() : null,
                Style = styleCol > 0 ? packagingSheet.Cells[row, styleCol].GetValue<string>()?.Trim() : null,
                Shape = shapeCol > 0 ? packagingSheet.Cells[row, shapeCol].GetValue<string>()?.Trim() : null,
                Size = size,
                VolumeDimensions = volumeDimensions,
                ColoursAvailable = coloursCol > 0 ? packagingSheet.Cells[row, coloursCol].GetValue<string>()?.Trim() : null,
                RecycledContent = recycledCol > 0 ? packagingSheet.Cells[row, recycledCol].GetValue<string>()?.Trim() : null,
                TotalPackWeight = totalWeightCol > 0 ? packagingSheet.Cells[row, totalWeightCol].GetValue<decimal?>() : null,
                WeightBasis = weightBasisCol > 0 ? packagingSheet.Cells[row, weightBasisCol].GetValue<string>()?.Trim() : null,
                Notes = notesCol > 0 ? packagingSheet.Cells[row, notesCol].GetValue<string>()?.Trim() : null,
                ExampleReference = exampleRefCol > 0 ? packagingSheet.Cells[row, exampleRefCol].GetValue<string>()?.Trim() : null,
                Source = sourceCol > 0 ? packagingSheet.Cells[row, sourceCol].GetValue<string>()?.Trim() : null,
                Url = urlCol > 0 ? packagingSheet.Cells[row, urlCol].GetValue<string>()?.Trim() : null,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };
            
            _context.PackagingGroups.Add(packagingGroup);
            await _context.SaveChangesAsync(); // Save to get ID
            
            // Parse Materials field - semicolon-delimited: TaxonID+weight+name(in brackets)
            if (materialsCol > 0)
            {
                var materialsStr = packagingSheet.Cells[row, materialsCol].GetValue<string>()?.Trim();
                if (!string.IsNullOrEmpty(materialsStr))
                {
                    var materialParts = materialsStr.Split(';', StringSplitOptions.RemoveEmptyEntries);
                    int sortOrder = 0;
                    
                    foreach (var materialPart in materialParts)
                    {
                        var trimmed = materialPart.Trim();
                        if (string.IsNullOrEmpty(trimmed)) continue;
                        
                        // Parse format: TaxonID+weight+name(in brackets)
                        // Example: "PL.PET.BF.TR.CLEAR+21.0g+Bottle body" or "PL.PET.BF.TR.CLEAR 21.0g+Bottle body"
                        string? taxonomyCode = null;
                        decimal? weight = null;
                        string? itemName = null;
                        
                        // Try to extract name in brackets first
                        var bracketStart = trimmed.LastIndexOf('(');
                        var bracketEnd = trimmed.LastIndexOf(')');
                        if (bracketStart > 0 && bracketEnd > bracketStart)
                        {
                            itemName = trimmed.Substring(bracketStart + 1, bracketEnd - bracketStart - 1).Trim();
                            trimmed = trimmed.Substring(0, bracketStart).Trim();
                        }
                        
                        // Split by + to get parts
                        var parts = trimmed.Split('+', StringSplitOptions.RemoveEmptyEntries);
                        
                        // First part should contain taxonomy code and possibly weight
                        if (parts.Length >= 1)
                        {
                            var firstPart = parts[0].Trim();
                            
                            // Check if weight is embedded in the taxonomy code (e.g., "PL.PET.BF.TR.CLEAR 21.0g")
                            var weightMatch = System.Text.RegularExpressions.Regex.Match(firstPart, @"\s+(\d+\.?\d*)\s*g\s*$", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                            if (weightMatch.Success)
                            {
                                taxonomyCode = firstPart.Substring(0, weightMatch.Index).Trim();
                                if (decimal.TryParse(weightMatch.Groups[1].Value, out decimal w))
                                {
                                    weight = w;
                                }
                            }
                            else
                            {
                                taxonomyCode = firstPart;
                            }
                        }
                        
                        // Second part might be weight if not already extracted
                        if (parts.Length >= 2 && !weight.HasValue)
                        {
                            var weightStr = parts[1].Trim().Replace("g", "").Replace("G", "").Trim();
                            if (decimal.TryParse(weightStr, out decimal w))
                            {
                                weight = w;
                            }
                        }
                        
                        // Third part might be the name if not in brackets
                        if (parts.Length >= 3 && string.IsNullOrEmpty(itemName))
                        {
                            itemName = parts[2].Trim();
                        }
                        
                        if (string.IsNullOrEmpty(taxonomyCode))
                        {
                            Console.WriteLine($"Warning: Could not parse taxonomy code from '{materialPart}'");
                            continue;
                        }
                        
                        // Use taxonomy code as key to ensure uniqueness (same taxonomy = same library item)
                        // Update name if we have a better one, but reuse the same library item
                        if (!libraryItems.ContainsKey(taxonomyCode))
                        {
                            // Try to find matching MaterialTaxonomy - search by exact code first
                            var materialTaxonomy = await _context.MaterialTaxonomies
                                .FirstOrDefaultAsync(mt => mt.Code == taxonomyCode);
                            
                            // If not found, try to find by code prefix (e.g., "PL.PET" for "PL.PET.BF.TR.CLEAR")
                            if (materialTaxonomy == null && taxonomyCode.Contains('.'))
                            {
                                var codeParts = taxonomyCode.Split('.');
                                // Try progressively shorter prefixes
                                for (int prefixLen = codeParts.Length - 1; prefixLen >= 1; prefixLen--)
                                {
                                    var prefixCode = string.Join(".", codeParts.Take(prefixLen));
                                    materialTaxonomy = await _context.MaterialTaxonomies
                                        .FirstOrDefaultAsync(mt => mt.Code == prefixCode);
                                    if (materialTaxonomy != null)
                                    {
                                        Console.WriteLine($"Matched taxonomy code '{taxonomyCode}' to '{prefixCode}' (ID: {materialTaxonomy.Id})");
                                        break;
                                    }
                                }
                            }
                            
                            var libraryItem = new PackagingLibrary
                            {
                                TaxonomyCode = taxonomyCode,
                                Name = itemName ?? taxonomyCode,
                                Weight = weight,
                                MaterialTaxonomyId = materialTaxonomy?.Id,
                                IsActive = true,
                                CreatedAt = DateTime.UtcNow
                            };
                            
                            _context.PackagingLibraries.Add(libraryItem);
                            await _context.SaveChangesAsync(); // Save to get ID
                            
                            libraryItems[taxonomyCode] = libraryItem;
                            importedLibraryItems++;
                            
                            if (materialTaxonomy != null)
                            {
                                Console.WriteLine($"Created PackagingLibrary '{libraryItem.Name}' (Taxonomy: {taxonomyCode}) linked to MaterialTaxonomy '{materialTaxonomy.DisplayName}'");
                            }
                            else
                            {
                                Console.WriteLine($"Warning: Created PackagingLibrary '{libraryItem.Name}' (Taxonomy: {taxonomyCode}) but no MaterialTaxonomy found");
                            }
                        }
                        else
                        {
                            // Update the existing library item if we have a better name or weight
                            var existingItem = libraryItems[taxonomyCode];
                            if (!string.IsNullOrEmpty(itemName) && existingItem.Name == taxonomyCode)
                            {
                                existingItem.Name = itemName;
                                _context.PackagingLibraries.Update(existingItem);
                            }
                            if (weight.HasValue && !existingItem.Weight.HasValue)
                            {
                                existingItem.Weight = weight;
                                _context.PackagingLibraries.Update(existingItem);
                            }
                        }
                        
                        // Link library item to packaging group
                        var groupItem = new PackagingGroupItem
                        {
                            PackagingGroupId = packagingGroup.Id,
                            PackagingLibraryId = libraryItems[taxonomyCode].Id,
                            SortOrder = sortOrder++,
                            CreatedAt = DateTime.UtcNow
                        };
                        
                        _context.PackagingGroupItems.Add(groupItem);
                    }
                }
            }
            
            await _context.SaveChangesAsync();
            importedGroups++;
        }
        
        Console.WriteLine($"Import completed!");
        Console.WriteLine($"  Imported {importedGroups} Packaging Groups");
        Console.WriteLine($"  Imported {importedLibraryItems} Packaging Library Items");
    }
}

