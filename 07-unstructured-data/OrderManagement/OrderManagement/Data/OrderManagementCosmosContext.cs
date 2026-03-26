using Microsoft.EntityFrameworkCore;
using OrderManagement.Domain;

namespace OrderManagement.Data;

public sealed class OrderManagementCosmosContext(DbContextOptions<OrderManagementCosmosContext> options)
    : DbContext(options)
{
    public DbSet<CosmosOrderDocument> Orders => Set<CosmosOrderDocument>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultContainer("OrderManagement");

        modelBuilder.Entity<CosmosOrderDocument>(b =>
        {
            b.ToContainer("Orders");

            // Cosmos requires id; no auto-generation (use string or GUID)
            b.HasKey(o => o.Id);

            // Partition key is crucial for scalability/cost
            b.HasPartitionKey(o => o.CustomerId);  // e.g. partition by customer :contentReference[oaicite:10]{index=10}

            // Optional: if this is the only type in the container
            b.HasNoDiscriminator(); // :contentReference[oaicite:11]{index=11}

            // Owned/embedded graphs are natural in Cosmos
            b.OwnsMany(o => o.Lines);
            b.OwnsOne(o => o.ShipTo);
            b.OwnsOne(o => o.Audit);
        });
    }
}
