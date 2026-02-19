using EPR.Domain.Entities;
using EPR.Data;
using Microsoft.EntityFrameworkCore;

namespace EPR.Web.Seeders;

/// <summary>
/// Seeds dummy packaging suppliers and their products for demo/review
/// </summary>
public class PackagingSupplierSeeder
{
    private readonly EPRDbContext _context;

    public PackagingSupplierSeeder(EPRDbContext context)
    {
        _context = context;
    }

    public async Task SeedAsync()
    {
        if (await _context.PackagingSuppliers.AnyAsync())
            return;

        var suppliers = new List<PackagingSupplier>
        {
            new PackagingSupplier
            {
                Name = "EcoPack Solutions Ltd",
                Address = "123 Green Industrial Park",
                City = "Manchester",
                State = "Greater Manchester",
                Country = "UK",
                Phone = "+44 161 555 0100",
                Email = "sales@ecopacksolutions.co.uk",
                Website = "https://www.ecopacksolutions.co.uk",
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                Contacts = new List<PackagingSupplierContact>
                {
                    new PackagingSupplierContact { Name = "Sarah Mitchell", Title = "Sales Director", Phone = "+44 161 555 0101", Email = "s.mitchell@ecopacksolutions.co.uk" },
                    new PackagingSupplierContact { Name = "James Wilson", Title = "Technical Support", Phone = "+44 161 555 0102", Email = "j.wilson@ecopacksolutions.co.uk" }
                },
                Products = new List<PackagingSupplierProduct>
                {
                    new PackagingSupplierProduct { Name = "Recycled PET Bottles 500ml", Description = "Food-grade rPET bottles", ProductCode = "RPET-500", TaxonomyCode = "PET" },
                    new PackagingSupplierProduct { Name = "HDPE Caps", Description = "High-density polyethylene caps", ProductCode = "HDPE-CAP-28", TaxonomyCode = "HDPE" },
                    new PackagingSupplierProduct { Name = "Paper Labels", Description = "FSC-certified paper labels", ProductCode = "PL-100", TaxonomyCode = "PAP" }
                }
            },
            new PackagingSupplier
            {
                Name = "Sustainable Packaging Co",
                Address = "45 Circular Economy Way",
                City = "Bristol",
                State = "South West",
                Country = "UK",
                Phone = "+44 117 555 0200",
                Email = "info@sustainablepack.co.uk",
                Website = "https://www.sustainablepack.co.uk",
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                Contacts = new List<PackagingSupplierContact>
                {
                    new PackagingSupplierContact { Name = "Emma Thompson", Title = "Account Manager", Phone = "+44 117 555 0201", Email = "emma@sustainablepack.co.uk" }
                },
                Products = new List<PackagingSupplierProduct>
                {
                    new PackagingSupplierProduct { Name = "Compostable Film", Description = "Home-compostable flexible packaging", ProductCode = "CF-200", TaxonomyCode = "PLA" },
                    new PackagingSupplierProduct { Name = "Cardboard Sleeves", Description = "Recycled cardboard packaging sleeves", ProductCode = "CB-SLEEVE", TaxonomyCode = "PAP" },
                    new PackagingSupplierProduct { Name = "Glass Jars 250g", Description = "Amber glass jars for preserves", ProductCode = "GJ-250", TaxonomyCode = "GLS" }
                }
            },
            new PackagingSupplier
            {
                Name = "FlexiPack Industries",
                Address = "Unit 7, Riverside Estate",
                City = "Birmingham",
                State = "West Midlands",
                Country = "UK",
                Phone = "+44 121 555 0300",
                Email = "enquiries@flexipack.co.uk",
                Website = null,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                Contacts = new List<PackagingSupplierContact>
                {
                    new PackagingSupplierContact { Name = "David Chen", Title = "Sales Manager", Phone = "+44 121 555 0301", Email = "d.chen@flexipack.co.uk" },
                    new PackagingSupplierContact { Name = "Lisa Brown", Title = "Quality Assurance", Phone = "+44 121 555 0302", Email = "l.brown@flexipack.co.uk" }
                },
                Products = new List<PackagingSupplierProduct>
                {
                    new PackagingSupplierProduct { Name = "PP Rigid Containers", Description = "Polypropylene food containers", ProductCode = "PP-RC-500", TaxonomyCode = "PP" },
                    new PackagingSupplierProduct { Name = "PE Shrink Wrap", Description = "Polyethylene shrink film", ProductCode = "PE-SW-100", TaxonomyCode = "PE" },
                    new PackagingSupplierProduct { Name = "Aluminium Foil Trays", Description = "Recyclable aluminium trays", ProductCode = "AL-TRAY", TaxonomyCode = "ALU" }
                }
            }
        };

        foreach (var s in suppliers)
        {
            _context.PackagingSuppliers.Add(s);
        }
        await _context.SaveChangesAsync();
    }
}
