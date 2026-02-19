using EPR.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace EPR.Data;

public class EPRDbContext : DbContext
{
    public EPRDbContext(DbContextOptions<EPRDbContext> options) : base(options)
    {
    }

    // Flow Blueprints
    public DbSet<FlowBlueprint> FlowBlueprints { get; set; }
    public DbSet<FlowBlueprintNode> FlowBlueprintNodes { get; set; }
    public DbSet<FlowBlueprintEdge> FlowBlueprintEdges { get; set; }
    
    // Visual Editor Projects
    public DbSet<VisualEditorProject> VisualEditorProjects { get; set; }
    
    // Legacy entities (keeping for compatibility)
    public DbSet<MaterialClassification> MaterialClassifications { get; set; }
    public DbSet<FeeScheme> FeeSchemes { get; set; }
    public DbSet<ProductGeography> ProductGeographies { get; set; }
    
    // Core EPR entities
    public DbSet<User> Users { get; set; }
    public DbSet<PackagingRawMaterial> PackagingRawMaterials { get; set; }
    public DbSet<MaterialJurisdiction> MaterialJurisdictions { get; set; }
    public DbSet<MaterialJurisdictionHistory> MaterialJurisdictionHistories { get; set; }
    public DbSet<EcoModularityFee> EcoModularityFees { get; set; }
    public DbSet<PackagingType> PackagingTypes { get; set; }
    public DbSet<PackagingTypeMaterial> PackagingTypeMaterials { get; set; }
    public DbSet<PackagingUnit> PackagingUnits { get; set; }
    public DbSet<PackagingUnitItem> PackagingUnitItems { get; set; }
    public DbSet<Product> Products { get; set; }
    public DbSet<ProductPackaging> ProductPackagings { get; set; }
    public DbSet<ProductForm> ProductForms { get; set; }
    public DbSet<Distribution> Distributions { get; set; }
    public DbSet<RecyclingData> RecyclingData { get; set; }
    public DbSet<Geography> Geographies { get; set; }
    public DbSet<Jurisdiction> Jurisdictions { get; set; }
    public DbSet<MaterialTaxonomy> MaterialTaxonomies { get; set; }
    public DbSet<MaterialTaxonomyCountryRequirement> MaterialTaxonomyCountryRequirements { get; set; }
    public DbSet<PackagingLibrary> PackagingLibraries { get; set; }
    public DbSet<PackagingGroup> PackagingGroups { get; set; }
    public DbSet<PackagingGroupItem> PackagingGroupItems { get; set; }
    
    // Packaging Suppliers
    public DbSet<PackagingSupplier> PackagingSuppliers { get; set; }
    public DbSet<PackagingSupplierContact> PackagingSupplierContacts { get; set; }
    public DbSet<PackagingSupplierProduct> PackagingSupplierProducts { get; set; }
    public DbSet<ProductPackagingSupplierProduct> ProductPackagingSupplierProducts { get; set; }

    // Supply chain: packaging item ↔ raw materials, packaging ↔ suppliers, raw materials ↔ suppliers
    public DbSet<PackagingLibraryMaterial> PackagingLibraryMaterials { get; set; }
    public DbSet<PackagingLibrarySupplierProduct> PackagingLibrarySupplierProducts { get; set; }
    public DbSet<MaterialTaxonomySupplierProduct> MaterialTaxonomySupplierProducts { get; set; }

    // ASN (Advanced Shipping Notice) entities
    public DbSet<AsnShipment> AsnShipments { get; set; }
    public DbSet<AsnPallet> AsnPallets { get; set; }
    public DbSet<AsnLineItem> AsnLineItems { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // FlowBlueprint configuration
        modelBuilder.Entity<FlowBlueprint>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Key).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Purpose).HasMaxLength(500);
            entity.Property(e => e.DefaultMetric).HasMaxLength(100);
            entity.HasIndex(e => e.Key).IsUnique();
        });

        // FlowBlueprintNode configuration
        modelBuilder.Entity<FlowBlueprintNode>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.NodeKey).IsRequired().HasMaxLength(100);
            entity.Property(e => e.EntityType).IsRequired().HasMaxLength(100);
            entity.HasOne(e => e.FlowBlueprint)
                .WithMany(b => b.Nodes)
                .HasForeignKey(e => e.FlowBlueprintId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // FlowBlueprintEdge configuration
        modelBuilder.Entity<FlowBlueprintEdge>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.FromNodeKey).IsRequired().HasMaxLength(100);
            entity.Property(e => e.ToNodeKey).IsRequired().HasMaxLength(100);
            entity.HasOne(e => e.FlowBlueprint)
                .WithMany(b => b.Edges)
                .HasForeignKey(e => e.FlowBlueprintId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // MaterialClassification configuration
        modelBuilder.Entity<MaterialClassification>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Code).IsRequired().HasMaxLength(50);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
            entity.HasIndex(e => e.Code).IsUnique();
        });

        // Geography configuration
        modelBuilder.Entity<Geography>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Code).IsRequired().HasMaxLength(50);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
            entity.HasOne(e => e.ParentGeography)
                .WithMany(g => g.ChildGeographies)
                .HasForeignKey(e => e.ParentGeographyId)
                .OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(e => e.Jurisdiction)
                .WithMany(j => j.Geographies)
                .HasForeignKey(e => e.JurisdictionId)
                .OnDelete(DeleteBehavior.Restrict);
            entity.HasIndex(e => e.Code).IsUnique();
        });

        // Jurisdiction configuration
        modelBuilder.Entity<Jurisdiction>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Code).IsRequired().HasMaxLength(50);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
            entity.Property(e => e.CountryCode).HasMaxLength(10);
            entity.HasIndex(e => e.Code).IsUnique();
        });

        // FeeScheme configuration
        modelBuilder.Entity<FeeScheme>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Code).IsRequired().HasMaxLength(50);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
            entity.Property(e => e.BaseRate).HasPrecision(18, 4);
            entity.HasOne(e => e.Jurisdiction)
                .WithMany(j => j.FeeSchemes)
                .HasForeignKey(e => e.JurisdictionId)
                .OnDelete(DeleteBehavior.Restrict);
            entity.HasIndex(e => e.Code).IsUnique();
        });

        // ProductGeography configuration
        modelBuilder.Entity<ProductGeography>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Quantity).HasPrecision(18, 4);
            entity.HasOne(e => e.Product)
                .WithMany(p => p.ProductGeographies)
                .HasForeignKey(e => e.ProductId)
                .OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(e => e.Geography)
                .WithMany(g => g.ProductGeographies)
                .HasForeignKey(e => e.GeographyId)
                .OnDelete(DeleteBehavior.Restrict);
            entity.HasIndex(e => new { e.ProductId, e.GeographyId }).IsUnique();
        });

        // User configuration
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Username).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Email).IsRequired().HasMaxLength(200);
            entity.Property(e => e.PasswordHash).IsRequired().HasMaxLength(255);
            entity.HasIndex(e => e.Username).IsUnique();
            entity.HasIndex(e => e.Email).IsUnique();
        });

        // MaterialTaxonomy configuration
        modelBuilder.Entity<MaterialTaxonomy>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Code).IsRequired().HasMaxLength(100);
            entity.Property(e => e.DisplayName).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Name).HasMaxLength(500);
            entity.Property(e => e.IconClass).HasMaxLength(100);
            entity.HasOne(e => e.ParentTaxonomy)
                .WithMany(t => t.ChildTaxonomies)
                .HasForeignKey(e => e.ParentTaxonomyId)
                .OnDelete(DeleteBehavior.Restrict);
            entity.HasIndex(e => new { e.Level, e.Code }).IsUnique();
            entity.HasIndex(e => e.ParentTaxonomyId);
        });
        
        // MaterialTaxonomyCountryRequirement configuration
        modelBuilder.Entity<MaterialTaxonomyCountryRequirement>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.CountryCode).IsRequired().HasMaxLength(10);
            entity.Property(e => e.CountryName).IsRequired().HasMaxLength(200);
            entity.HasOne(e => e.MaterialTaxonomy)
                .WithMany(t => t.CountryRequirements)
                .HasForeignKey(e => e.MaterialTaxonomyId)
                .OnDelete(DeleteBehavior.Cascade);
            entity.HasIndex(e => new { e.MaterialTaxonomyId, e.CountryCode }).IsUnique();
        });
        
        // PackagingRawMaterial configuration
        modelBuilder.Entity<PackagingRawMaterial>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
            entity.HasOne(e => e.ParentMaterial)
                .WithMany(m => m.SubMaterials)
                .HasForeignKey(e => e.ParentMaterialId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // MaterialJurisdiction configuration
        modelBuilder.Entity<MaterialJurisdiction>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.FeePerTonne).HasPrecision(18, 4);
            entity.Property(e => e.Currency).HasMaxLength(10);
            entity.HasOne(e => e.Material)
                .WithMany(m => m.Jurisdictions)
                .HasForeignKey(e => e.MaterialId)
                .OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(e => e.Jurisdiction)
                .WithMany()
                .HasForeignKey(e => e.JurisdictionId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // MaterialJurisdictionHistory configuration
        modelBuilder.Entity<MaterialJurisdictionHistory>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.FeePerTonne).HasPrecision(18, 4);
            entity.HasOne(e => e.MaterialJurisdiction)
                .WithMany(mj => mj.History)
                .HasForeignKey(e => e.MaterialJurisdictionId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // EcoModularityFee configuration
        modelBuilder.Entity<EcoModularityFee>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.FeePerTonne).HasPrecision(18, 4);
            entity.Property(e => e.Currency).HasMaxLength(10);
            entity.Property(e => e.DisposalMethod).HasMaxLength(100);
            entity.HasOne(e => e.Material)
                .WithMany()
                .HasForeignKey(e => e.MaterialId)
                .OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(e => e.Jurisdiction)
                .WithMany()
                .HasForeignKey(e => e.JurisdictionId)
                .OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(e => e.Geography)
                .WithMany()
                .HasForeignKey(e => e.GeographyId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // PackagingType configuration
        modelBuilder.Entity<PackagingType>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Height).HasPrecision(18, 4);
            entity.Property(e => e.Weight).HasPrecision(18, 4);
            entity.Property(e => e.Depth).HasPrecision(18, 4);
            entity.Property(e => e.Volume).HasPrecision(18, 4);
            entity.Property(e => e.LibrarySource).HasMaxLength(100);
        });

        // PackagingTypeMaterial configuration
        modelBuilder.Entity<PackagingTypeMaterial>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasOne(e => e.PackagingType)
                .WithMany(pt => pt.Materials)
                .HasForeignKey(e => e.PackagingTypeId)
                .OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(e => e.Material)
                .WithMany(m => m.PackagingTypes)
                .HasForeignKey(e => e.MaterialId)
                .OnDelete(DeleteBehavior.Restrict);
            entity.HasIndex(e => new { e.PackagingTypeId, e.MaterialId }).IsUnique();
        });

        // PackagingUnit configuration
        modelBuilder.Entity<PackagingUnit>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
            entity.Property(e => e.UnitLevel).HasMaxLength(50);
        });

        // PackagingUnitItem configuration
        modelBuilder.Entity<PackagingUnitItem>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.CollectionName).HasMaxLength(200);
            entity.HasOne(e => e.PackagingUnit)
                .WithMany(pu => pu.Items)
                .HasForeignKey(e => e.PackagingUnitId)
                .OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(e => e.PackagingType)
                .WithMany(pt => pt.PackagingUnitItems)
                .HasForeignKey(e => e.PackagingTypeId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // Product configuration
        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Sku).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Size).HasPrecision(18, 4);
            entity.Property(e => e.Weight).HasPrecision(18, 4);
            entity.Property(e => e.Height).HasPrecision(18, 4);
            entity.Property(e => e.Quantity).HasPrecision(18, 4);
            entity.Property(e => e.Gtin).HasMaxLength(14);
            entity.Property(e => e.Brand).HasMaxLength(200);
            entity.Property(e => e.ProductCategory).HasMaxLength(100);
            entity.Property(e => e.ProductSubCategory).HasMaxLength(100);
            entity.Property(e => e.CountryOfOrigin).HasMaxLength(2);
            entity.Property(e => e.ProductWeightUnit).HasMaxLength(10);
            entity.Property(e => e.ProductVolumeUnit).HasMaxLength(10);
            entity.Property(e => e.ParentUnitGtin).HasMaxLength(14);
            entity.HasIndex(e => e.Sku).IsUnique();
            entity.HasIndex(e => e.Gtin);
        });

        // ProductForm configuration
        modelBuilder.Entity<ProductForm>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Gtin).IsRequired().HasMaxLength(14);
            entity.Property(e => e.ProductName).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Brand).IsRequired().HasMaxLength(200);
            entity.Property(e => e.ProductCategory).IsRequired().HasMaxLength(100);
            entity.Property(e => e.ProductSubCategory).IsRequired().HasMaxLength(100);
            entity.Property(e => e.CountryOfOrigin).IsRequired().HasMaxLength(2);
            entity.Property(e => e.PackagingLevel).IsRequired().HasMaxLength(50);
            entity.Property(e => e.PackagingType).IsRequired().HasMaxLength(100);
            entity.Property(e => e.PackagingConfiguration).HasMaxLength(50);
            entity.Property(e => e.Status).IsRequired().HasMaxLength(20).HasDefaultValue("draft");
            entity.HasOne(e => e.Product)
                .WithMany()
                .HasForeignKey(e => e.ProductId)
                .OnDelete(DeleteBehavior.Cascade);
            entity.HasIndex(e => e.Gtin);
            entity.HasIndex(e => e.ProductId);
        });

        // ProductPackaging configuration
        modelBuilder.Entity<ProductPackaging>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasOne(e => e.Product)
                .WithMany(p => p.ProductPackagings)
                .HasForeignKey(e => e.ProductId)
                .OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(e => e.PackagingUnit)
                .WithMany(pu => pu.ProductPackagings)
                .HasForeignKey(e => e.PackagingUnitId)
                .OnDelete(DeleteBehavior.Restrict);
            entity.HasIndex(e => new { e.ProductId, e.PackagingUnitId }).IsUnique();
        });

        // Distribution configuration
        modelBuilder.Entity<Distribution>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Quantity).HasPrecision(18, 4);
            entity.Property(e => e.City).IsRequired().HasMaxLength(100);
            entity.Property(e => e.StateProvince).IsRequired().HasMaxLength(100);
            entity.Property(e => e.County).HasMaxLength(100);
            entity.Property(e => e.PostcodeZipcode).HasMaxLength(20);
            entity.Property(e => e.Country).IsRequired().HasMaxLength(100);
            entity.HasOne(e => e.Product)
                .WithMany(p => p.Distributions)
                .HasForeignKey(e => e.ProductId)
                .OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(e => e.PackagingUnit)
                .WithMany()
                .HasForeignKey(e => e.PackagingUnitId)
                .OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(e => e.Geography)
                .WithMany()
                .HasForeignKey(e => e.GeographyId)
                .OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(e => e.Jurisdiction)
                .WithMany()
                .HasForeignKey(e => e.JurisdictionId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // RecyclingData configuration
        modelBuilder.Entity<RecyclingData>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.RecyclablePercentage).HasPrecision(5, 2);
            entity.Property(e => e.NonRecyclablePercentage).HasPrecision(5, 2);
            entity.HasOne(e => e.Material)
                .WithMany()
                .HasForeignKey(e => e.MaterialId)
                .OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(e => e.Geography)
                .WithMany()
                .HasForeignKey(e => e.GeographyId)
                .OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(e => e.Jurisdiction)
                .WithMany()
                .HasForeignKey(e => e.JurisdictionId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // PackagingLibrary configuration
        modelBuilder.Entity<PackagingLibrary>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.TaxonomyCode).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Weight).HasPrecision(18, 4);
            entity.HasOne(e => e.MaterialTaxonomy)
                .WithMany()
                .HasForeignKey(e => e.MaterialTaxonomyId)
                .OnDelete(DeleteBehavior.SetNull);
            entity.HasIndex(e => e.TaxonomyCode);
        });

        // PackagingGroup configuration
        modelBuilder.Entity<PackagingGroup>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.PackId).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
            entity.Property(e => e.PackagingLayer).HasMaxLength(200);
            entity.Property(e => e.Style).HasMaxLength(200);
            entity.Property(e => e.Shape).HasMaxLength(200);
            entity.Property(e => e.Size).HasMaxLength(200);
            entity.Property(e => e.VolumeDimensions).HasMaxLength(200);
            entity.Property(e => e.ColoursAvailable).HasMaxLength(500);
            entity.Property(e => e.RecycledContent).HasMaxLength(200);
            entity.Property(e => e.TotalPackWeight).HasPrecision(18, 4);
            entity.Property(e => e.WeightBasis).HasMaxLength(200);
            entity.Property(e => e.Notes).HasMaxLength(2000);
            entity.Property(e => e.ExampleReference).HasMaxLength(500);
            entity.Property(e => e.Source).HasMaxLength(500);
            entity.Property(e => e.Url).HasMaxLength(1000);
            entity.HasIndex(e => e.PackId).IsUnique();
        });

        // PackagingGroupItem configuration
        modelBuilder.Entity<PackagingGroupItem>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasOne(e => e.PackagingGroup)
                .WithMany(g => g.Items)
                .HasForeignKey(e => e.PackagingGroupId)
                .OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(e => e.PackagingLibrary)
                .WithMany(l => l.PackagingGroupItems)
                .HasForeignKey(e => e.PackagingLibraryId)
                .OnDelete(DeleteBehavior.Restrict);
            entity.HasIndex(e => new { e.PackagingGroupId, e.PackagingLibraryId }).IsUnique();
        });

        // VisualEditorProject configuration
        modelBuilder.Entity<VisualEditorProject>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Key).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
            entity.Property(e => e.ProjectDataJson).IsRequired();
            entity.HasIndex(e => e.Key).IsUnique();
        });

        // AsnShipment configuration
        modelBuilder.Entity<AsnShipment>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.AsnNumber).IsRequired().HasMaxLength(50);
            entity.Property(e => e.ShipperGln).IsRequired().HasMaxLength(13);
            entity.Property(e => e.ShipperName).IsRequired().HasMaxLength(200);
            entity.Property(e => e.ShipperAddress).HasMaxLength(500);
            entity.Property(e => e.ShipperCity).HasMaxLength(100);
            entity.Property(e => e.ShipperPostalCode).HasMaxLength(20);
            entity.Property(e => e.ShipperCountryCode).HasMaxLength(2);
            entity.Property(e => e.ReceiverGln).IsRequired().HasMaxLength(13);
            entity.Property(e => e.ReceiverName).IsRequired().HasMaxLength(200);
            entity.Property(e => e.PoReference).HasMaxLength(50);
            entity.Property(e => e.CarrierName).HasMaxLength(200);
            entity.Property(e => e.TransportMode).HasMaxLength(50);
            entity.Property(e => e.VehicleRegistration).HasMaxLength(50);
            entity.Property(e => e.TotalWeight).HasPrecision(18, 2);
            entity.Property(e => e.SourceFormat).IsRequired().HasMaxLength(20);
            entity.Property(e => e.Status).IsRequired().HasMaxLength(20);
            entity.Property(e => e.IsSimulated).IsRequired().HasDefaultValue(false);
            
            entity.HasIndex(e => e.AsnNumber);
            entity.HasIndex(e => e.ShipDate);
            entity.HasIndex(e => e.Status);
            entity.HasIndex(e => e.IsSimulated);
        });

        // AsnPallet configuration
        modelBuilder.Entity<AsnPallet>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Sscc).IsRequired().HasMaxLength(18);
            entity.Property(e => e.PackageTypeCode).HasMaxLength(10);
            entity.Property(e => e.GrossWeight).HasPrecision(18, 2);
            entity.Property(e => e.DestinationGln).IsRequired().HasMaxLength(13);
            entity.Property(e => e.DestinationName).IsRequired().HasMaxLength(200);
            entity.Property(e => e.DestinationAddress).HasMaxLength(500);
            entity.Property(e => e.DestinationCity).HasMaxLength(100);
            entity.Property(e => e.DestinationPostalCode).HasMaxLength(20);
            entity.Property(e => e.DestinationCountryCode).HasMaxLength(2);
            entity.HasOne(e => e.AsnShipment)
                .WithMany(s => s.Pallets)
                .HasForeignKey(e => e.AsnShipmentId)
                .OnDelete(DeleteBehavior.Cascade);
            entity.HasIndex(e => e.Sscc);
            entity.HasIndex(e => e.DestinationGln);
        });

        // PackagingSupplier configuration
        modelBuilder.Entity<PackagingSupplier>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Address).HasMaxLength(500);
            entity.Property(e => e.City).HasMaxLength(100);
            entity.Property(e => e.State).HasMaxLength(100);
            entity.Property(e => e.Country).HasMaxLength(100);
            entity.Property(e => e.Phone).HasMaxLength(50);
            entity.Property(e => e.Email).HasMaxLength(200);
            entity.Property(e => e.Website).HasMaxLength(500);
            entity.HasOne(e => e.SuppliedBySupplier)
                .WithMany(s => s.SuppliedSuppliers)
                .HasForeignKey(e => e.SuppliedBySupplierId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // PackagingSupplierContact configuration
        modelBuilder.Entity<PackagingSupplierContact>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Title).HasMaxLength(100);
            entity.Property(e => e.Phone).HasMaxLength(50);
            entity.Property(e => e.Email).HasMaxLength(200);
            entity.HasOne(e => e.PackagingSupplier)
                .WithMany(s => s.Contacts)
                .HasForeignKey(e => e.PackagingSupplierId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // PackagingSupplierProduct configuration
        modelBuilder.Entity<PackagingSupplierProduct>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Description).HasMaxLength(1000);
            entity.Property(e => e.ProductCode).HasMaxLength(100);
            entity.Property(e => e.TaxonomyCode).HasMaxLength(100);
            entity.HasOne(e => e.PackagingSupplier)
                .WithMany(s => s.Products)
                .HasForeignKey(e => e.PackagingSupplierId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // ProductPackagingSupplierProduct configuration
        modelBuilder.Entity<ProductPackagingSupplierProduct>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasOne(e => e.Product)
                .WithMany(p => p.ProductPackagingSupplierProducts)
                .HasForeignKey(e => e.ProductId)
                .OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(e => e.PackagingSupplierProduct)
                .WithMany(p => p.ProductPackagingSupplierProducts)
                .HasForeignKey(e => e.PackagingSupplierProductId)
                .OnDelete(DeleteBehavior.Cascade);
            entity.HasIndex(e => new { e.ProductId, e.PackagingSupplierProductId }).IsUnique();
        });

        // PackagingLibraryMaterial configuration
        modelBuilder.Entity<PackagingLibraryMaterial>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasOne(e => e.PackagingLibrary)
                .WithMany(l => l.PackagingLibraryMaterials)
                .HasForeignKey(e => e.PackagingLibraryId)
                .OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(e => e.MaterialTaxonomy)
                .WithMany(m => m.PackagingLibraryMaterials)
                .HasForeignKey(e => e.MaterialTaxonomyId)
                .OnDelete(DeleteBehavior.Cascade);
            entity.HasIndex(e => new { e.PackagingLibraryId, e.MaterialTaxonomyId }).IsUnique();
        });

        // PackagingLibrarySupplierProduct configuration
        modelBuilder.Entity<PackagingLibrarySupplierProduct>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasOne(e => e.PackagingLibrary)
                .WithMany(l => l.PackagingLibrarySupplierProducts)
                .HasForeignKey(e => e.PackagingLibraryId)
                .OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(e => e.PackagingSupplierProduct)
                .WithMany(p => p.PackagingLibrarySupplierProducts)
                .HasForeignKey(e => e.PackagingSupplierProductId)
                .OnDelete(DeleteBehavior.Cascade);
            entity.HasIndex(e => new { e.PackagingLibraryId, e.PackagingSupplierProductId }).IsUnique();
        });

        // MaterialTaxonomySupplierProduct configuration
        modelBuilder.Entity<MaterialTaxonomySupplierProduct>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasOne(e => e.MaterialTaxonomy)
                .WithMany(m => m.MaterialTaxonomySupplierProducts)
                .HasForeignKey(e => e.MaterialTaxonomyId)
                .OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(e => e.PackagingSupplierProduct)
                .WithMany(p => p.MaterialTaxonomySupplierProducts)
                .HasForeignKey(e => e.PackagingSupplierProductId)
                .OnDelete(DeleteBehavior.Cascade);
            entity.HasIndex(e => new { e.MaterialTaxonomyId, e.PackagingSupplierProductId }).IsUnique();
        });

        // AsnLineItem configuration
        modelBuilder.Entity<AsnLineItem>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Gtin).IsRequired().HasMaxLength(14);
            entity.Property(e => e.Description).IsRequired().HasMaxLength(500);
            entity.Property(e => e.Quantity).HasPrecision(18, 2);
            entity.Property(e => e.UnitOfMeasure).IsRequired().HasMaxLength(10);
            entity.Property(e => e.BatchNumber).HasMaxLength(50);
            entity.Property(e => e.PoLineReference).HasMaxLength(50);
            entity.Property(e => e.SupplierArticleNumber).HasMaxLength(50);
            entity.Property(e => e.NetWeight).HasPrecision(18, 2);
            entity.Property(e => e.IsSimulated).IsRequired().HasDefaultValue(false);
            entity.HasOne(e => e.AsnPallet)
                .WithMany(p => p.LineItems)
                .HasForeignKey(e => e.AsnPalletId)
                .OnDelete(DeleteBehavior.Cascade);
            entity.HasIndex(e => e.Gtin);
            entity.HasIndex(e => e.BatchNumber);
        });
    }
}
