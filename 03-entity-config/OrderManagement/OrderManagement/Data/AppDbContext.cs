using Microsoft.EntityFrameworkCore;
using OrderManagement.Data.Configurations;
using OrderManagement.Domain;

namespace OrderManagement.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<Product> Products => Set<Product>();
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<OrderLine> OrderLines => Set<OrderLine>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("shop");

        modelBuilder.Entity<OrderLine>(b =>
        {
            b.HasKey(ol => new { ol.OrderId, ol.ProductId });

            b.HasOne(ol => ol.Order)
             .WithMany(o => o.Lines)
             .HasForeignKey(ol => ol.OrderId);

            b.HasOne(ol => ol.Product)
             .WithMany()
             .HasForeignKey(ol => ol.ProductId);
        });

        modelBuilder.ApplyConfiguration(new CustomerConfiguration());
        modelBuilder.ApplyConfiguration(new ProductConfiguration());
    }
}
