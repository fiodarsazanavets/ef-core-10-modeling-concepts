using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OrderManagement.Data.Configurations;
using OrderManagement.Domain;

namespace OrderManagement.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<Product> Products => Set<Product>();
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

        modelBuilder.Entity<Customer>(b =>
        {
            b.ComplexProperty(c => c.BillingAddress, a =>
            {
                a.IsRequired(false); // BillingAddress can be null :contentReference[oaicite:1]{index=1}

                // Shadow discriminator column (nullable) marks presence
                a.HasDiscriminator<bool?>("BillPresent")
                 .HasValue(true); // value when the complex object is present :contentReference[oaicite:2]{index=2}

                ConfigureAddress(a, prefix: "Bill", required: false); // your existing column naming
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

            // Shadow FK to keep Order clean: SalesAgentId column exists, but no property in Order
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
             .OnDelete(DeleteBehavior.Restrict); // keep order history stable

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
            // Shadow key/FK column "CustomerId" (no property in class)
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

    private static void ConfigureAddress(ComplexPropertyBuilder<Address> a, string prefix, bool required)
    {
        // Column naming + types + lengths (Clip 3)
        ConfigureString(a.Property(x => x.Street), $"{prefix}Street", 200, required);
        ConfigureString(a.Property(x => x.City), $"{prefix}City", 100, required);
        ConfigureString(a.Property(x => x.PostalCode), $"{prefix}PostalCode", 20, required);

        // ISO2, fixed-length, non-unicode
        var country = a.Property(x => x.CountryCode)
            .HasColumnName($"{prefix}CountryCode")
            .HasColumnType("char(2)")
            .IsUnicode(false)
            .IsFixedLength();

        country.IsRequired(required);
    }

    private static void ConfigureMoney(ComplexPropertyBuilder<Money> m, string amountColumn, string currencyColumn)
    {
        // Fixes your decimal warning and makes precision explicit
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
