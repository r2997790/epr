using EPR.Application.Services;
using EPR.Data;
using EPR.Web.Seeders;
using EPR.Web.Extensions;
using EPR.Web.Scripts;
using Microsoft.EntityFrameworkCore;
using EPR.Web.Attributes;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Data.Sqlite;
using System.Data.Common;
using static EPR.Web.Scripts.InitializeAsnDatabase;

static string? GetSqliteDbPath(string connectionString)
{
    if (string.IsNullOrWhiteSpace(connectionString)) return null;
    var ds = "Data Source=";
    var idx = connectionString.IndexOf(ds, StringComparison.OrdinalIgnoreCase);
    if (idx < 0) return null;
    var start = idx + ds.Length;
    var end = connectionString.IndexOf(';', start);
    var path = (end > start ? connectionString.Substring(start, end - start) : connectionString.Substring(start)).Trim();
    if (string.IsNullOrEmpty(path)) return null;
    return Path.IsPathRooted(path) ? path : Path.GetFullPath(path);
}

var builder = WebApplication.CreateBuilder(args);

// Railway: listen on PORT when set
var port = Environment.GetEnvironmentVariable("PORT");
if (!string.IsNullOrEmpty(port))
{
    builder.WebHost.UseUrls($"http://0.0.0.0:{port}");
}

// Check for initialize ASN database command
if (args.Length > 0 && args[0] == "init-asn-db")
{
    await InitializeAsnDatabase.RunAsync();
    return;
}

// Check for import command
if (args.Length > 0 && args[0] == "import-taxonomy" && args.Length > 1)
{
    var excelPath = args[1];
    await ImportMaterialTaxonomyScript.RunAsync(excelPath);
    return;
}

if (args.Length > 0 && args[0] == "import-packaging" && args.Length > 1)
{
    var excelPath = args[1];
    await ImportPackagingLibraryScript.RunAsync(excelPath);
    return;
}

if (args.Length > 0 && args[0] == "verify-packaging")
{
    await VerifyPackagingImport.RunAsync();
    return;
}

// Check for seed admin user command
if (args.Length > 0 && args[0] == "seed-admin-user")
{
    var dbConnectionString = builder.Configuration.GetConnectionString("DefaultConnection") 
        ?? "Data Source=epr.db";
    
    var optionsBuilder = new DbContextOptionsBuilder<EPRDbContext>();
    optionsBuilder.UseSqlite(dbConnectionString);
    
    using var context = new EPRDbContext(optionsBuilder.Options);
    
    // Ensure database is created with all tables
    Console.WriteLine("Initializing database...");
    await context.Database.EnsureCreatedAsync();
    
    Console.WriteLine("Seeding admin user...");
    var userSeeder = new UserSeeder(context);
    await userSeeder.SeedAsync();
    return;
}

// Check for seed M&S Network command
if (args.Length > 0 && args[0] == "seed-ms-network" && args.Length > 1)
{
    var excelPath = args[1];
    var dbConnectionString = builder.Configuration.GetConnectionString("DefaultConnection") 
        ?? "Data Source=epr.db";
    
    var optionsBuilder = new DbContextOptionsBuilder<EPRDbContext>();
    optionsBuilder.UseSqlite(dbConnectionString);
    
    using var context = new EPRDbContext(optionsBuilder.Options);
    
    // Ensure database is created with all tables
    Console.WriteLine("Initializing database...");
    
    // EnsureCreatedAsync should create the database and all tables
    var created = await context.Database.EnsureCreatedAsync();
    if (created)
    {
        Console.WriteLine("Database created successfully.");
    }
    else
    {
        Console.WriteLine("Database already exists.");
    }
    
    // Verify the table exists by trying to query it
    try
    {
        var testQuery = await context.VisualEditorProjects.CountAsync();
        Console.WriteLine($"Database verified. VisualEditorProjects table exists (contains {testQuery} records).");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"ERROR: Database table verification failed: {ex.Message}");
        Console.WriteLine("Attempting to recreate database...");
        
        // Try to delete and recreate
        await context.Database.EnsureDeletedAsync();
        await context.Database.EnsureCreatedAsync();
        Console.WriteLine("Database recreated.");
    }
    
    Console.WriteLine("Starting import...");
    
    var seeder = new MSNetworkDistributionGroupSeeder(context, excelPath);
    await seeder.SeedAsync();
    return;
}

// Add services to the container
builder.Services.AddControllersWithViews();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Database configuration - use PostgreSQL when DATABASE_URL is set (Railway), otherwise SQLite
var databaseUrl = Environment.GetEnvironmentVariable("DATABASE_URL");
var connectionString = !string.IsNullOrEmpty(databaseUrl)
    ? databaseUrl
    : (builder.Configuration.GetConnectionString("DefaultConnection") ?? "Data Source=epr.db");

// Ensure Railway PostgreSQL has SSL mode
var isPostgres = !string.IsNullOrEmpty(databaseUrl) && databaseUrl.StartsWith("postgres", StringComparison.OrdinalIgnoreCase);
if (isPostgres && !connectionString.Contains("sslmode", StringComparison.OrdinalIgnoreCase))
{
    var separator = connectionString.Contains('?') ? "&" : "?";
    connectionString = connectionString + separator + "sslmode=Require";
}

builder.Services.AddDbContext<EPRDbContext>(options =>
{
    if (isPostgres)
    {
        options.UseNpgsql(connectionString);
    }
    else
    {
        options.UseSqlite(connectionString);
    }
});

// Register EPR services
builder.Services.AddScoped<IFlowBlueprintService, FlowBlueprintService>();
builder.Services.AddScoped<EPR.Application.Services.IAuthenticationService, EPR.Application.Services.AuthenticationService>();
builder.Services.AddScoped<EPR.Data.Services.MaterialTaxonomyImportService>();
builder.Services.AddScoped<EPR.Data.Services.PackagingLibraryImportService>();
builder.Services.AddScoped<EPR.Web.Services.AsnParserService>();

// Session requires IDistributedCache - use in-memory for single instance
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(60);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
    options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
    options.Cookie.SameSite = SameSiteMode.Lax;
});

// Register EmpauerLocal shared services
builder.Services.AddBrowserTabs();

// Add HTTP context accessor (required by EmpauerLocal modules)
builder.Services.AddHttpContextAccessor();

var app = builder.Build();

// Seed admin user on startup if database is empty
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<EPRDbContext>();
    bool initDone = false;
    for (int attempt = 0; !initDone && attempt < 2; attempt++)
    {
        try
        {
            await context.Database.EnsureCreatedAsync();
            Console.WriteLine("✓ Database verified/created");
        
        // Verify ASN tables exist (PostgreSQL: EnsureCreated handles this; SQLite: may need manual creation)
        try
        {
            await context.AsnShipments.AnyAsync();
            Console.WriteLine("✓ ASN tables verified");
        }
        catch
        {
            if (!isPostgres)
            {
            // If ASN tables don't exist, we need to add them (SQLite only)
            Console.WriteLine("Creating ASN tables...");
            await context.Database.ExecuteSqlRawAsync(@"
                CREATE TABLE IF NOT EXISTS AsnShipments (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    AsnNumber TEXT NOT NULL,
                    ShipperGln TEXT NOT NULL,
                    ShipperName TEXT NOT NULL,
                    ShipperAddress TEXT,
                    ShipperCity TEXT,
                    ShipperPostalCode TEXT,
                    ShipperCountryCode TEXT,
                    ReceiverGln TEXT NOT NULL,
                    ReceiverName TEXT NOT NULL,
                    ShipDate TEXT NOT NULL,
                    DeliveryDate TEXT,
                    PoReference TEXT,
                    CarrierName TEXT,
                    TransportMode TEXT,
                    VehicleRegistration TEXT,
                    TotalWeight REAL,
                    TotalPackages INTEGER,
                    SourceFormat TEXT NOT NULL,
                    RawData TEXT,
                    ImportedAt TEXT NOT NULL,
                    Status TEXT NOT NULL,
                    IsSimulated INTEGER NOT NULL DEFAULT 0
                );
                
                CREATE TABLE IF NOT EXISTS AsnPallets (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    AsnShipmentId INTEGER NOT NULL,
                    Sscc TEXT NOT NULL,
                    PackageTypeCode TEXT,
                    GrossWeight REAL,
                    DestinationGln TEXT NOT NULL,
                    DestinationName TEXT NOT NULL,
                    DestinationAddress TEXT,
                    DestinationCity TEXT,
                    DestinationPostalCode TEXT,
                    DestinationCountryCode TEXT,
                    SequenceNumber INTEGER NOT NULL DEFAULT 1,
                    IsSimulated INTEGER NOT NULL DEFAULT 0,
                    FOREIGN KEY (AsnShipmentId) REFERENCES AsnShipments(Id) ON DELETE CASCADE
                );
                
                CREATE TABLE IF NOT EXISTS AsnLineItems (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    AsnPalletId INTEGER NOT NULL,
                    LineNumber INTEGER NOT NULL,
                    Gtin TEXT NOT NULL,
                    Description TEXT NOT NULL,
                    Quantity REAL NOT NULL,
                    UnitOfMeasure TEXT NOT NULL,
                    BatchNumber TEXT,
                    BestBeforeDate TEXT,
                    PoLineReference TEXT,
                    SupplierArticleNumber TEXT,
                    NetWeight REAL,
                    IsSimulated INTEGER NOT NULL DEFAULT 0,
                    FOREIGN KEY (AsnPalletId) REFERENCES AsnPallets(Id) ON DELETE CASCADE
                );
                
                CREATE INDEX IF NOT EXISTS IX_AsnShipments_AsnNumber ON AsnShipments(AsnNumber);
                CREATE INDEX IF NOT EXISTS IX_AsnShipments_ShipDate ON AsnShipments(ShipDate);
                CREATE INDEX IF NOT EXISTS IX_AsnShipments_Status ON AsnShipments(Status);
                CREATE INDEX IF NOT EXISTS IX_AsnPallets_Sscc ON AsnPallets(Sscc);
                CREATE INDEX IF NOT EXISTS IX_AsnPallets_DestinationGln ON AsnPallets(DestinationGln);
                CREATE INDEX IF NOT EXISTS IX_AsnLineItems_Gtin ON AsnLineItems(Gtin);
                CREATE INDEX IF NOT EXISTS IX_AsnLineItems_BatchNumber ON AsnLineItems(BatchNumber);
                CREATE INDEX IF NOT EXISTS IX_AsnShipments_IsSimulated ON AsnShipments(IsSimulated);
                CREATE INDEX IF NOT EXISTS IX_AsnPallets_IsSimulated ON AsnPallets(IsSimulated);
                CREATE INDEX IF NOT EXISTS IX_AsnLineItems_IsSimulated ON AsnLineItems(IsSimulated);
            ");
            
            // Add IsSimulated columns if they don't exist (for existing databases)
            // SQLite doesn't support IF NOT EXISTS for ALTER TABLE, so we check first
            var connection = context.Database.GetDbConnection();
            await connection.OpenAsync();
            try {
                using var command = connection.CreateCommand();
                command.CommandText = "PRAGMA table_info(AsnShipments)";
                using var reader = await command.ExecuteReaderAsync();
                bool hasIsSimulated = false;
                while (await reader.ReadAsync()) {
                    if (reader.GetString(1) == "IsSimulated") {
                        hasIsSimulated = true;
                        break;
                    }
                }
                reader.Close();
                
                if (!hasIsSimulated) {
                    command.CommandText = "ALTER TABLE AsnShipments ADD COLUMN IsSimulated INTEGER NOT NULL DEFAULT 0";
                    await command.ExecuteNonQueryAsync();
                }
            } catch (Exception ex) {
                Console.WriteLine($"Warning: Could not add IsSimulated to AsnShipments: {ex.Message}");
            }
            
            try {
                using var command = connection.CreateCommand();
                command.CommandText = "PRAGMA table_info(AsnPallets)";
                using var reader = await command.ExecuteReaderAsync();
                bool hasIsSimulated = false;
                while (await reader.ReadAsync()) {
                    if (reader.GetString(1) == "IsSimulated") {
                        hasIsSimulated = true;
                        break;
                    }
                }
                reader.Close();
                
                if (!hasIsSimulated) {
                    command.CommandText = "ALTER TABLE AsnPallets ADD COLUMN IsSimulated INTEGER NOT NULL DEFAULT 0";
                    await command.ExecuteNonQueryAsync();
                }
            } catch (Exception ex) {
                Console.WriteLine($"Warning: Could not add IsSimulated to AsnPallets: {ex.Message}");
            }
            
            try {
                using var command = connection.CreateCommand();
                command.CommandText = "PRAGMA table_info(AsnLineItems)";
                using var reader = await command.ExecuteReaderAsync();
                bool hasIsSimulated = false;
                while (await reader.ReadAsync()) {
                    if (reader.GetString(1) == "IsSimulated") {
                        hasIsSimulated = true;
                        break;
                    }
                }
                reader.Close();
                
                if (!hasIsSimulated) {
                    command.CommandText = "ALTER TABLE AsnLineItems ADD COLUMN IsSimulated INTEGER NOT NULL DEFAULT 0";
                    await command.ExecuteNonQueryAsync();
                }
            } catch (Exception ex) {
                Console.WriteLine($"Warning: Could not add IsSimulated to AsnLineItems: {ex.Message}");
            }
            
            await connection.CloseAsync();
            Console.WriteLine("✓ ASN tables created");
            }
            else
            {
                Console.WriteLine("✓ ASN tables (PostgreSQL - created by EnsureCreated)");
            }
        }
        
        // SQLite-specific: ensure columns/tables exist (PostgreSQL uses EnsureCreated)
        if (!isPostgres)
        {
        try
        {
            var conn = context.Database.GetDbConnection();
            if (conn.State != System.Data.ConnectionState.Open)
                await conn.OpenAsync();
            foreach (var table in new[] { "AsnShipments", "AsnPallets", "AsnLineItems" })
            {
                bool hasCol = false;
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = $"PRAGMA table_info({table})";
                    using var r = await cmd.ExecuteReaderAsync();
                    while (await r.ReadAsync())
                    {
                        if (r.GetString(1) == "IsSimulated") { hasCol = true; break; }
                    }
                }
                if (!hasCol)
                {
                    using var cmd = conn.CreateCommand();
                    cmd.CommandText = $"ALTER TABLE {table} ADD COLUMN IsSimulated INTEGER NOT NULL DEFAULT 0";
                    await cmd.ExecuteNonQueryAsync();
                    Console.WriteLine($"✓ Added IsSimulated to {table}");
                }
            }
            if (conn.State == System.Data.ConnectionState.Open)
                await conn.CloseAsync();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"⚠ IsSimulated column check: {ex.Message}");
        }

        // Ensure supply chain tables exist (PackagingLibraryMaterial, PackagingLibrarySupplierProduct, MaterialTaxonomySupplierProduct)
        try
        {
            await context.Database.ExecuteSqlRawAsync(@"
                CREATE TABLE IF NOT EXISTS PackagingLibraryMaterials (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    PackagingLibraryId INTEGER NOT NULL,
                    MaterialTaxonomyId INTEGER NOT NULL,
                    SortOrder INTEGER NOT NULL DEFAULT 0,
                    CreatedAt TEXT NOT NULL,
                    FOREIGN KEY (PackagingLibraryId) REFERENCES PackagingLibraries(Id) ON DELETE CASCADE,
                    FOREIGN KEY (MaterialTaxonomyId) REFERENCES MaterialTaxonomies(Id) ON DELETE CASCADE
                );
                CREATE UNIQUE INDEX IF NOT EXISTS IX_PackagingLibraryMaterials_PackagingLibraryId_MaterialTaxonomyId 
                    ON PackagingLibraryMaterials(PackagingLibraryId, MaterialTaxonomyId);

                CREATE TABLE IF NOT EXISTS PackagingLibrarySupplierProducts (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    PackagingLibraryId INTEGER NOT NULL,
                    PackagingSupplierProductId INTEGER NOT NULL,
                    IsPrimary INTEGER NOT NULL DEFAULT 0,
                    CreatedAt TEXT NOT NULL,
                    FOREIGN KEY (PackagingLibraryId) REFERENCES PackagingLibraries(Id) ON DELETE CASCADE,
                    FOREIGN KEY (PackagingSupplierProductId) REFERENCES PackagingSupplierProducts(Id) ON DELETE CASCADE
                );
                CREATE UNIQUE INDEX IF NOT EXISTS IX_PackagingLibrarySupplierProducts_PackagingLibraryId_PackagingSupplierProductId 
                    ON PackagingLibrarySupplierProducts(PackagingLibraryId, PackagingSupplierProductId);

                CREATE TABLE IF NOT EXISTS MaterialTaxonomySupplierProducts (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    MaterialTaxonomyId INTEGER NOT NULL,
                    PackagingSupplierProductId INTEGER NOT NULL,
                    IsPrimary INTEGER NOT NULL DEFAULT 0,
                    CreatedAt TEXT NOT NULL,
                    FOREIGN KEY (MaterialTaxonomyId) REFERENCES MaterialTaxonomies(Id) ON DELETE CASCADE,
                    FOREIGN KEY (PackagingSupplierProductId) REFERENCES PackagingSupplierProducts(Id) ON DELETE CASCADE
                );
                CREATE UNIQUE INDEX IF NOT EXISTS IX_MaterialTaxonomySupplierProducts_MaterialTaxonomyId_PackagingSupplierProductId 
                    ON MaterialTaxonomySupplierProducts(MaterialTaxonomyId, PackagingSupplierProductId);
            ");
            // Add SuppliedBySupplierId to PackagingSuppliers if missing
            var conn = context.Database.GetDbConnection();
            if (conn.State != System.Data.ConnectionState.Open) await conn.OpenAsync();
            try
            {
                var hasCol = false;
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "PRAGMA table_info(PackagingSuppliers)";
                    using var r = await cmd.ExecuteReaderAsync();
                    while (await r.ReadAsync())
                    {
                        if (r.GetString(1) == "SuppliedBySupplierId") { hasCol = true; break; }
                    }
                }
                if (!hasCol)
                {
                    using var cmd = conn.CreateCommand();
                    cmd.CommandText = "ALTER TABLE PackagingSuppliers ADD COLUMN SuppliedBySupplierId INTEGER REFERENCES PackagingSuppliers(Id)";
                    await cmd.ExecuteNonQueryAsync();
                    Console.WriteLine("✓ Added SuppliedBySupplierId to PackagingSuppliers");
                }
            }
            finally
            {
                if (conn.State == System.Data.ConnectionState.Open) await conn.CloseAsync();
            }
            // Migrate existing PackagingLibrary.MaterialTaxonomyId to PackagingLibraryMaterials
            try
            {
                var libsWithLegacyMaterial = await context.PackagingLibraries
                    .Where(l => l.MaterialTaxonomyId != null)
                    .Select(l => new { l.Id, l.MaterialTaxonomyId })
                    .ToListAsync();
                foreach (var lib in libsWithLegacyMaterial)
                {
                    var exists = await context.PackagingLibraryMaterials
                        .AnyAsync(plm => plm.PackagingLibraryId == lib.Id && plm.MaterialTaxonomyId == lib.MaterialTaxonomyId!.Value);
                    if (!exists)
                    {
                        context.PackagingLibraryMaterials.Add(new EPR.Domain.Entities.PackagingLibraryMaterial
                        {
                            PackagingLibraryId = lib.Id,
                            MaterialTaxonomyId = lib.MaterialTaxonomyId!.Value,
                            SortOrder = 0,
                            CreatedAt = DateTime.UtcNow
                        });
                    }
                }
                await context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"⚠ PackagingLibraryMaterials migration: {ex.Message}");
            }

            Console.WriteLine("✓ Supply chain tables verified");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"⚠ Supply chain tables: {ex.Message}");
        }

        // Ensure Product table has all columns (fixes DBs created before GS1 fields were added)
        try
        {
            var conn = context.Database.GetDbConnection();
            if (conn.State != System.Data.ConnectionState.Open)
                await conn.OpenAsync();
            var existingColumns = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = "PRAGMA table_info(Products)";
                using var r = await cmd.ExecuteReaderAsync();
                while (await r.ReadAsync())
                    existingColumns.Add(r.GetString(1));
            }
            var productColumns = new (string Name, string SqlType)[]
            {
                ("Brand", "TEXT"),
                ("Gtin", "TEXT"),
                ("ProductCategory", "TEXT"),
                ("ProductSubCategory", "TEXT"),
                ("CountryOfOrigin", "TEXT"),
                ("ProductWeight", "REAL"),
                ("ProductWeightUnit", "TEXT"),
                ("ProductVolume", "REAL"),
                ("ProductVolumeUnit", "TEXT"),
                ("ParentUnitGtin", "TEXT"),
                ("UnitsPerPackage", "INTEGER"),
                ("CreatedAt", "TEXT"),
                ("UpdatedAt", "TEXT"),
            };
            foreach (var (name, sqlType) in productColumns)
            {
                if (existingColumns.Contains(name)) continue;
                using var cmd = conn.CreateCommand();
                cmd.CommandText = $"ALTER TABLE Products ADD COLUMN {name} {sqlType}";
                await cmd.ExecuteNonQueryAsync();
                Console.WriteLine($"✓ Added column Products.{name}");
            }
            if (conn.State == System.Data.ConnectionState.Open)
                await conn.CloseAsync();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"⚠ Product table column check: {ex.Message}");
        }

        // Create ProductForms table if missing (existing DBs may not have it)
        try
        {
            var conn = context.Database.GetDbConnection();
            if (conn.State != System.Data.ConnectionState.Open)
                await conn.OpenAsync();
            bool tableExists = false;
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = "SELECT 1 FROM sqlite_master WHERE type='table' AND name='ProductForms'";
                using var r = await cmd.ExecuteReaderAsync();
                tableExists = await r.ReadAsync();
            }
            if (!tableExists)
            {
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                        CREATE TABLE ProductForms (
                            Id INTEGER PRIMARY KEY AUTOINCREMENT,
                            ProductId INTEGER NOT NULL,
                            Gtin TEXT NOT NULL,
                            ProductName TEXT NOT NULL,
                            Brand TEXT NOT NULL,
                            ProductWeight REAL,
                            ProductWeightUnit TEXT,
                            ProductVolume REAL,
                            ProductVolumeUnit TEXT,
                            SkuCode TEXT,
                            ProductCategory TEXT NOT NULL,
                            ProductSubCategory TEXT NOT NULL,
                            ParentUnitGtin TEXT,
                            UnitsPerPackage INTEGER,
                            CountryOfOrigin TEXT NOT NULL,
                            ProductPhotosJson TEXT,
                            PackagingLevel TEXT NOT NULL,
                            PackagingType TEXT NOT NULL,
                            PackagingConfiguration TEXT,
                            TotalPackagingWeight REAL,
                            PackagingComponentsJson TEXT,
                            ClosureTypesJson TEXT,
                            AdhesiveType TEXT,
                            TamperEvidence INTEGER,
                            Resealable INTEGER,
                            LabelTypesJson TEXT,
                            LabelMaterial TEXT,
                            LegalMarksJson TEXT,
                            PrimaryRetailersJson TEXT,
                            RetailFormatJson TEXT,
                            MostCommonPackSize TEXT,
                            IsPrivateLabel INTEGER,
                            GeographicDistributionJson TEXT,
                            ShelfLifeExtension INTEGER,
                            EstimatedShelfLifeDays INTEGER,
                            FoodWasteReductionImpact TEXT,
                            EprSchemeApplicable INTEGER,
                            EprCategory TEXT,
                            ApcoSignatory INTEGER,
                            SustainabilityCertificationsJson TEXT,
                            ProductFamily TEXT,
                            AssociatedSkusJson TEXT,
                            VariantReason TEXT,
                            GeneralNotes TEXT,
                            PackagingRationale TEXT,
                            KnownIssues TEXT,
                            ImprovementPlans TEXT,
                            Status TEXT NOT NULL DEFAULT 'draft',
                            CreatedAt TEXT NOT NULL,
                            UpdatedAt TEXT,
                            CreatedBy TEXT,
                            FOREIGN KEY (ProductId) REFERENCES Products(Id) ON DELETE CASCADE
                        )";
                    await cmd.ExecuteNonQueryAsync();
                }
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "CREATE INDEX IF NOT EXISTS IX_ProductForms_Gtin ON ProductForms(Gtin)";
                    await cmd.ExecuteNonQueryAsync();
                }
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "CREATE INDEX IF NOT EXISTS IX_ProductForms_ProductId ON ProductForms(ProductId)";
                    await cmd.ExecuteNonQueryAsync();
                }
                Console.WriteLine("✓ ProductForms table created");
            }
            if (conn.State == System.Data.ConnectionState.Open)
                await conn.CloseAsync();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"⚠ ProductForms table check: {ex.Message}");
        }

        // Create PackagingSuppliers tables if missing
        try
        {
            var conn = context.Database.GetDbConnection();
            if (conn.State != System.Data.ConnectionState.Open)
                await conn.OpenAsync();
            bool suppliersTableExists = false;
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = "SELECT 1 FROM sqlite_master WHERE type='table' AND name='PackagingSuppliers'";
                using var r = await cmd.ExecuteReaderAsync();
                suppliersTableExists = await r.ReadAsync();
            }
            if (!suppliersTableExists)
            {
                await context.Database.ExecuteSqlRawAsync("CREATE TABLE PackagingSuppliers (Id INTEGER PRIMARY KEY AUTOINCREMENT, Name TEXT NOT NULL, Address TEXT, City TEXT, State TEXT, Country TEXT, Phone TEXT, Email TEXT, Website TEXT, IsActive INTEGER NOT NULL DEFAULT 1, CreatedAt TEXT NOT NULL, UpdatedAt TEXT)");
                await context.Database.ExecuteSqlRawAsync("CREATE TABLE PackagingSupplierContacts (Id INTEGER PRIMARY KEY AUTOINCREMENT, PackagingSupplierId INTEGER NOT NULL, Name TEXT NOT NULL, Title TEXT, Phone TEXT, Email TEXT, FOREIGN KEY (PackagingSupplierId) REFERENCES PackagingSuppliers(Id) ON DELETE CASCADE)");
                await context.Database.ExecuteSqlRawAsync("CREATE TABLE PackagingSupplierProducts (Id INTEGER PRIMARY KEY AUTOINCREMENT, PackagingSupplierId INTEGER NOT NULL, Name TEXT NOT NULL, Description TEXT, ProductCode TEXT, TaxonomyCode TEXT, FOREIGN KEY (PackagingSupplierId) REFERENCES PackagingSuppliers(Id) ON DELETE CASCADE)");
                await context.Database.ExecuteSqlRawAsync("CREATE INDEX IF NOT EXISTS IX_PackagingSupplierContacts_PackagingSupplierId ON PackagingSupplierContacts(PackagingSupplierId)");
                await context.Database.ExecuteSqlRawAsync("CREATE INDEX IF NOT EXISTS IX_PackagingSupplierProducts_PackagingSupplierId ON PackagingSupplierProducts(PackagingSupplierId)");
                await context.Database.ExecuteSqlRawAsync("CREATE TABLE ProductPackagingSupplierProducts (Id INTEGER PRIMARY KEY AUTOINCREMENT, ProductId INTEGER NOT NULL, PackagingSupplierProductId INTEGER NOT NULL, FOREIGN KEY (ProductId) REFERENCES Products(Id) ON DELETE CASCADE, FOREIGN KEY (PackagingSupplierProductId) REFERENCES PackagingSupplierProducts(Id) ON DELETE CASCADE)");
                await context.Database.ExecuteSqlRawAsync("CREATE UNIQUE INDEX IF NOT EXISTS IX_ProductPackagingSupplierProducts_ProductId_PackagingSupplierProductId ON ProductPackagingSupplierProducts(ProductId, PackagingSupplierProductId)");
                Console.WriteLine("✓ PackagingSuppliers tables created");
            }
            else
            {
                // Add City, State, Country columns if they don't exist (for existing DBs)
                var existingCols = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "PRAGMA table_info(PackagingSuppliers)";
                    using var r = await cmd.ExecuteReaderAsync();
                    while (await r.ReadAsync())
                        existingCols.Add(r.GetString(1));
                }
                foreach (var col in new[] { ("City", "TEXT"), ("State", "TEXT"), ("Country", "TEXT") })
                {
                    if (existingCols.Contains(col.Item1)) continue;
                    await context.Database.ExecuteSqlRawAsync($"ALTER TABLE PackagingSuppliers ADD COLUMN {col.Item1} {col.Item2}");
                    Console.WriteLine($"✓ Added PackagingSuppliers.{col.Item1}");
                }
                bool linkTableExists = false;
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "SELECT 1 FROM sqlite_master WHERE type='table' AND name='ProductPackagingSupplierProducts'";
                    using var r2 = await cmd.ExecuteReaderAsync();
                    linkTableExists = await r2.ReadAsync();
                }
                if (!linkTableExists)
                {
                    await context.Database.ExecuteSqlRawAsync("CREATE TABLE ProductPackagingSupplierProducts (Id INTEGER PRIMARY KEY AUTOINCREMENT, ProductId INTEGER NOT NULL, PackagingSupplierProductId INTEGER NOT NULL, FOREIGN KEY (ProductId) REFERENCES Products(Id) ON DELETE CASCADE, FOREIGN KEY (PackagingSupplierProductId) REFERENCES PackagingSupplierProducts(Id) ON DELETE CASCADE)");
                    await context.Database.ExecuteSqlRawAsync("CREATE UNIQUE INDEX IF NOT EXISTS IX_ProductPackagingSupplierProducts_ProductId_PackagingSupplierProductId ON ProductPackagingSupplierProducts(ProductId, PackagingSupplierProductId)");
                    Console.WriteLine("✓ ProductPackagingSupplierProducts table created");
                }
            }
            if (conn.State == System.Data.ConnectionState.Open)
                await conn.CloseAsync();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"⚠ PackagingSuppliers table check: {ex.Message}");
        }
        } // end if (!isPostgres)

            // Always ensure admin user exists with correct password
            var userSeeder = new UserSeeder(context);
            await userSeeder.SeedAsync();
            initDone = true;
        }
        catch (SqliteException ex) when (attempt == 0 && ex.Message.Contains("malformed", StringComparison.OrdinalIgnoreCase))
        {
            try
            {
                var conn = context.Database.GetDbConnection();
                if (conn.State == System.Data.ConnectionState.Open)
                    await conn.CloseAsync();
            }
            catch { }
            var dbPath = GetSqliteDbPath(connectionString);
            if (!string.IsNullOrEmpty(dbPath) && File.Exists(dbPath))
            {
                try
                {
                    File.Delete(dbPath);
                    Console.WriteLine("✓ Removed corrupted database; will recreate on next attempt.");
                }
                catch (Exception deleteEx) { Console.WriteLine($"⚠ Could not delete corrupted DB: {deleteEx.Message}"); }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"⚠ Database initialization warning: {ex.Message}");
            break;
        }
    }
}

// Auto-inject sample data when database has no ASN or no product data (no user interaction required)
using (var seedScope = app.Services.CreateScope())
{
    var seedContext = seedScope.ServiceProvider.GetRequiredService<EPRDbContext>();
    var loggerFactory = seedScope.ServiceProvider.GetRequiredService<ILoggerFactory>();
    var hasAsns = false;
    var hasProducts = false;
    var hasProductForms = false;
    try
    {
        hasAsns = await seedContext.AsnShipments.AnyAsync();
        hasProducts = await seedContext.Products.AnyAsync();
        hasProductForms = await seedContext.ProductForms.AnyAsync();
    }
    catch (Exception ex)
    {
        Console.WriteLine($"⚠ Sample data check skipped: {ex.Message}");
    }
    if (!hasAsns || !hasProducts)
    {
        try
        {
            var scriptLogger = loggerFactory.CreateLogger<CreateDummyAsnData>();
            var seedScript = new CreateDummyAsnData(seedContext, scriptLogger);
            await seedScript.ExecuteAsync();
            Console.WriteLine("✓ Sample data (25 products, 10 ASNs, ProductForms, Distributions) seeded.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"⚠ Sample data seed failed: {ex.Message}");
            Console.WriteLine(ex.StackTrace);
        }
    }
    else if (hasProducts && !hasProductForms)
    {
        try
        {
            var scriptLogger = loggerFactory.CreateLogger<CreateDummyAsnData>();
            var seedScript = new CreateDummyAsnData(seedContext, scriptLogger);
            await seedScript.SeedReportingDataAsync();
            Console.WriteLine("✓ Reporting data (ProductForms, Distributions) seeded.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"⚠ Reporting data seed failed: {ex.Message}");
        }
    }

    // Seed packaging suppliers if empty
    try
    {
        if (!await seedContext.PackagingSuppliers.AnyAsync())
        {
            var supplierSeeder = new PackagingSupplierSeeder(seedContext);
            await supplierSeeder.SeedAsync();
            Console.WriteLine("✓ Packaging suppliers (3 suppliers with contacts and products) seeded.");
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"⚠ Packaging suppliers seed failed: {ex.Message}");
    }

    // Seed dummy raw materials, packaging items, packaging groups and relationships if empty
    try
    {
        var packagingSeeder = new PackagingDummyDataSeeder(seedContext);
        await packagingSeeder.SeedAsync();
        Console.WriteLine("✓ Packaging dummy data (raw materials, packaging items, groups, supply chain) verified.");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"⚠ Packaging dummy data seed: {ex.Message}");
    }

    // Link all products to packaging (ProductPackaging, ProductPackagingSupplierProduct, Distribution)
    try
    {
        var linkSeeder = new LinkProductPackagingSeeder(seedContext);
        await linkSeeder.SeedAsync();
        Console.WriteLine("✓ Product–packaging links (all products linked to packaging and distribution) verified.");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"⚠ Product–packaging link seed: {ex.Message}");
    }

    // Ensure admin user exists (fallback if init block didn't run)
    try
    {
        var userSeeder = new UserSeeder(seedContext);
        await userSeeder.SeedAsync();
    }
    catch (Exception ex)
    {
        Console.WriteLine($"⚠ Admin user seed: {ex.Message}");
    }
}

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    // HTTPS redirection disabled in development (app runs on HTTP only)
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
    // Only use HTTPS redirection in production when HTTPS is configured
    var httpsPort = app.Configuration["ASPNETCORE_HTTPS_PORT"] ?? 
                    app.Configuration["Kestrel:Endpoints:Https:Url"]?.Split(':').LastOrDefault();
    if (!string.IsNullOrEmpty(httpsPort))
    {
        app.UseHttpsRedirection();
    }
}
app.UseStaticFiles();

app.UseRouting();

app.UseSession();

app.UseAuthentication();
app.UseAuthorization();

// Attribute routes (api/packaging-management, api/visual-editor, etc.)
app.MapControllers();

// Distribution API actions (must be before generic distribution route so they are not treated as Index(id))
app.MapControllerRoute(
    name: "distribution-api",
    pattern: "distribution/{action:regex(^(GetAsnShipments|GetAsnShipment|CreateDummyData|CreateAsnShipment|UpdateAsnShipment|DeleteAsnShipment|UploadAsn|GetAsnChain|GetAsnProductPackaging)$)}",
    defaults: new { controller = "Distribution" });

// Specific route for Distribution: /distribution and /distribution/{id}
app.MapControllerRoute(
    name: "distribution",
    pattern: "distribution/{id?}",
    defaults: new { controller = "Distribution", action = "Index" });

// Specific route for Add Product
app.MapControllerRoute(
    name: "addProduct",
    pattern: "ProductForm/AddProduct",
    defaults: new { controller = "ProductForm", action = "AddProduct" });

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
