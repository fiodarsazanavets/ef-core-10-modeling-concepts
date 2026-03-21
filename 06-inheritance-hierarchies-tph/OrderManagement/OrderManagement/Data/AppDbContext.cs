using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OrderManagement.Data.Configurations;
using OrderManagement.Domain;

namespace OrderManagement.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<Product> Products => Set<Product>();
    public DbSet<PhysicalProduct> PhysicalProducts => Set<PhysicalProduct>();
    public DbSet<DigitalProduct> DigitalProducts => Set<DigitalProduct>();
    public DbSet<SubscriptionProduct> SubscriptionProducts => Set<SubscriptionProduct>();
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<OrderLine> OrderLines => Set<OrderLine>();
    public DbSet<Tag> Tags => Set<Tag>();
    public DbSet<Supplier> Suppliers => Set<Supplier>();
    public DbSet<ProductSupplier> ProductSuppliers => Set<ProductSupplier>();
    public DbSet<SalesAgent> SalesAgents => Set<SalesAgent>();
    public DbSet<CustomerProfile> CustomerProfiles => Set<CustomerProfile>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("shop");

        ConfigureProductHierarchy(modelBuilder, InheritanceMappingStrategy.Tph);

        modelBuilder.Entity<Customer>(b =>
        {
            b.ComplexProperty(c => c.BillingAddress, a =>
            {
                a.IsRequired(false);

                a.HasDiscriminator<bool?>("BillPresent")
                 .HasValue(true);

                ConfigureAddress(a, prefix: "Bill", required: false);
            });

            b.ComplexProperty(c => c.ShippingAddress, a =>
                ConfigureAddress(a, prefix: "Ship", required: true));

            b.HasMany(c => c.Orders)
                .WithOne(o => o.Customer)
                .HasForeignKey(o => o.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Order>(b =>
        {
            b.ComplexProperty(o => o.ShipTo, a => ConfigureAddress(a, "ShipTo", required: true));

            b.HasMany(o => o.Lines)
                .WithOne(l => l.Order)
                .HasForeignKey(l => l.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

            b.Property<int?>("SalesAgentId");

            b.HasOne(o => o.SalesAgent)
               .WithMany(a => a.Orders)
               .HasForeignKey("SalesAgentId")
               .OnDelete(DeleteBehavior.SetNull);
        });

        modelBuilder.Entity<OrderLine>(b =>
        {
            b.HasKey(ol => new { ol.OrderId, ol.ProductId });
            b.HasOne(ol => ol.Order)
             .WithMany(o => o.Lines)
             .HasForeignKey(ol => ol.OrderId);

            b.HasOne(ol => ol.Product)
             .WithMany()
             .HasForeignKey(ol => ol.ProductId)
             .OnDelete(DeleteBehavior.Restrict);

            b.ComplexProperty(ol => ol.UnitPrice, m =>
            {
                m.Property(x => x.Amount)
                 .HasColumnName("UnitPrice")
                 .HasPrecision(18, 2);

                m.Property(x => x.Currency)
                 .HasColumnName("UnitPriceCurrency")
                 .HasColumnType("char(3)")
                 .IsUnicode(false)
                 .IsFixedLength()
                 .HasDefaultValue("USD");
            });
        });

        modelBuilder.Entity<Product>(b =>
        {
            b.ComplexProperty(p => p.Price, m => ConfigureMoney(m, amountColumn: "Price", currencyColumn: "PriceCurrency"));
        });

        modelBuilder.Entity<CustomerProfile>(b =>
        {
            b.Property<int>("CustomerId");
            b.HasKey("CustomerId");

            b.HasOne(p => p.Customer)
             .WithOne(c => c.Profile)
             .HasForeignKey<CustomerProfile>("CustomerId")
             .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Product>(b =>
        {
            b.HasMany(p => p.Tags)
             .WithMany(t => t.Products)
             .UsingEntity<Dictionary<string, object>>(
                 "ProductTag",
                 right => right.HasOne<Tag>()
                               .WithMany()
                               .HasForeignKey("TagId")
                               .OnDelete(DeleteBehavior.Cascade),
                 left => left.HasOne<Product>()
                              .WithMany()
                              .HasForeignKey("ProductId")
                              .OnDelete(DeleteBehavior.Cascade),
                 join =>
                 {
                     join.ToTable("ProductTags");
                     join.HasKey("ProductId", "TagId");
                     join.HasIndex("TagId");
                 });
        });

        modelBuilder.Entity<ProductSupplier>(b =>
        {
            b.ToTable("ProductSuppliers");

            b.HasKey(ps => new { ps.ProductId, ps.SupplierId });

            b.Property(ps => ps.ContractPrice).HasPrecision(18, 2);
            b.Property(ps => ps.LeadTimeDays);
            b.Property(ps => ps.IsPreferred);

            b.HasOne(ps => ps.Product)
             .WithMany(p => p.Suppliers)
             .HasForeignKey(ps => ps.ProductId)
             .OnDelete(DeleteBehavior.Cascade);

            b.HasOne(ps => ps.Supplier)
             .WithMany(s => s.Products)
             .HasForeignKey(ps => ps.SupplierId)
             .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.ApplyConfiguration(new CustomerConfiguration());
        modelBuilder.ApplyConfiguration(new ProductConfiguration());
    }

    private static void ConfigureProductHierarchy(ModelBuilder modelBuilder, InheritanceMappingStrategy strategy)
    {
        // Derived-only complex type example
        modelBuilder.Entity<PhysicalProduct>()
            .ComplexProperty(p => p.Dimensions, d =>
            {
                d.Property(x => x.LengthCm).HasColumnName("DimLengthCm").HasPrecision(9, 2);
                d.Property(x => x.WidthCm).HasColumnName("DimWidthCm").HasPrecision(9, 2);
                d.Property(x => x.HeightCm).HasColumnName("DimHeightCm").HasPrecision(9, 2);
            });

        // Strategy-specific configuration
        switch (strategy)
        {
            case InheritanceMappingStrategy.Tph:
                ConfigureTph(modelBuilder);
                break;

            case InheritanceMappingStrategy.Tpt:
                ConfigureTpt(modelBuilder);
                break;

            case InheritanceMappingStrategy.Tpc:
                ConfigureTpc(modelBuilder);
                break;
        }
    }

    private static void ConfigureTph(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Product>(b =>
        {
            // Explicit TPH configuration (TPH is default, but we do it to teach it)
            // IMPORTANT: Use a NON-default discriminator name to avoid a reported EF Core 10 regression
            // with the default name "Discriminator".
            var d = b.HasDiscriminator<ProductKind>("ProductKind");

            d.HasValue<PhysicalProduct>(ProductKind.Physical);
            d.HasValue<DigitalProduct>(ProductKind.Digital);
            d.HasValue<SubscriptionProduct>(ProductKind.Subscription);

            // Permutation #1: discriminator as string (instead of enum/int)
            // b.HasDiscriminator<string>("ProductKind")
            //  .HasValue<PhysicalProduct>("physical")
            //  .HasValue<DigitalProduct>("digital")
            //  .HasValue<SubscriptionProduct>("subscription");

            // Permutation #2: incomplete mapping so unknown discriminator rows don’t throw,
            // and queries always include a discriminator predicate.
            // d.IsComplete(false);
        });

        modelBuilder.Entity<PhysicalProduct>()
            .Property(p => p.Manufacturer)
            .HasColumnName("Manufacturer");

        modelBuilder.Entity<DigitalProduct>()
            .Property(p => p.Manufacturer)
            .HasColumnName("Manufacturer");

        // Extra: enforce “no useless nulls” via CHECK constraints (great for showing TPH drawbacks)
        // Here we’re saying: if ProductKind is Physical then WeightKg must be non-null / > 0, etc.
        modelBuilder.Entity<Product>().ToTable(t => t.HasCheckConstraint(
            "CK_Products_Physical_RequiresWeight",
            "([ProductKind] <> 1) OR ([WeightKg] IS NOT NULL AND [WeightKg] > 0)"));

        modelBuilder.Entity<Product>().ToTable(t => t.HasCheckConstraint(
            "CK_Products_Digital_RequiresUrl",
            "([ProductKind] <> 2) OR ([DownloadUrl] IS NOT NULL AND [DownloadUrl] <> '')"));
    }

    private static void ConfigureTpt(ModelBuilder modelBuilder)
    {
        // Root switches to TPT; tables per type
        modelBuilder.Entity<Product>(b =>
        {
            b.UseTptMappingStrategy();
            b.ToTable("Products");
        });

        modelBuilder.Entity<PhysicalProduct>().ToTable("PhysicalProducts");
        modelBuilder.Entity<DigitalProduct>().ToTable("DigitalProducts");
        modelBuilder.Entity<SubscriptionProduct>().ToTable("SubscriptionProducts");
    }

    private static void ConfigureTpc(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Product>(b =>
        {
            b.UseTpcMappingStrategy();
        });

        // Table per concrete class
        modelBuilder.Entity<PhysicalProduct>().ToTable("PhysicalProducts");
        modelBuilder.Entity<DigitalProduct>().ToTable("DigitalProducts");
        modelBuilder.Entity<SubscriptionProduct>().ToTable("SubscriptionProducts");

        // Permutation: identity-based keys with non-overlapping ranges
        // modelBuilder.Entity<PhysicalProduct>().ToTable("PhysicalProducts",
        //     tb => tb.Property(e => e.Id).UseIdentityColumn(1, 3));
        // modelBuilder.Entity<DigitalProduct>().ToTable("DigitalProducts",
        //     tb => tb.Property(e => e.Id).UseIdentityColumn(2, 3));
        // modelBuilder.Entity<SubscriptionProduct>().ToTable("SubscriptionProducts",
        //     tb => tb.Property(e => e.Id).UseIdentityColumn(3, 3));
    }

    private static void ConfigureAddress(ComplexPropertyBuilder<Address> a, string prefix, bool required)
    {
        ConfigureString(a.Property(x => x.Street), $"{prefix}Street", 200, required);
        ConfigureString(a.Property(x => x.City), $"{prefix}City", 100, required);
        ConfigureString(a.Property(x => x.PostalCode), $"{prefix}PostalCode", 20, required);

        var country = a.Property(x => x.CountryCode)
            .HasColumnName($"{prefix}CountryCode")
            .HasColumnType("char(2)")
            .IsUnicode(false)
            .IsFixedLength();

        country.IsRequired(required);
    }

    private static void ConfigureMoney(ComplexPropertyBuilder<Money> m, string amountColumn, string currencyColumn)
    {
        m.Property(x => x.Amount)
         .HasColumnName(amountColumn)
         .HasPrecision(18, 2);

        m.Property(x => x.Currency)
         .HasColumnName(currencyColumn)
         .HasColumnType("char(3)")
         .IsUnicode(false)
         .IsFixedLength()
         .HasDefaultValue("USD");
    }

    private static void ConfigureString(ComplexTypePropertyBuilder<string> p, string name, int maxLen, bool required)
    {
        p.HasColumnName(name)
         .HasColumnType($"varchar({maxLen})")
         .IsUnicode(false)
         .HasMaxLength(maxLen);

        p.IsRequired(required);
    }
}
