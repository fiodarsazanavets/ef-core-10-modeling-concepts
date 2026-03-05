using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OrderManagement.Domain;

namespace OrderManagement.Data.Configurations;

public sealed class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        //  Table override
        builder.ToTable("Products");

        // SKU: non-Unicode, length, and uniqueness
        builder.Property(p => p.Sku)
               .HasColumnName("Sku")
               .IsRequired()
               .HasMaxLength(32)
               .IsUnicode(false);

        // Primary key with a named constraint
        builder.HasKey(p => p.Id)
               .HasName("PK_Products");

        // If you want a named UNIQUE CONSTRAINT (SQL Server will emit UNIQUE constraint)
        builder.HasAlternateKey(p => p.Sku)
               .HasName("AK_Products_Sku");

        // Also create a named index for lookup speed (distinct from the unique constraint)
        builder.HasIndex(p => p.Sku)
               .HasDatabaseName("IX_Products_Sku");

        // Name: non-Unicode, length
        builder.Property(p => p.Name)
               .IsRequired()
               .HasMaxLength(200)
               .IsUnicode(false);

        // Price: precision/scale + named check constraint
        builder.Property(p => p.Price)
               .HasPrecision(18, 2);

        builder.ToTable(t => t.HasCheckConstraint(
            "CK_Products_Price_NonNegative",
            "[Price] >= 0"
        ));
    }
}
