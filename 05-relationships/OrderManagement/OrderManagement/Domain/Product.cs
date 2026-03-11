namespace OrderManagement.Domain;

public class Product
{
    public int Id { get; set; }
    public string Sku { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public Money Price { get; set; }

    // Skip-nav many-to-many
    public List<Tag> Tags { get; set; } = [];

    // Join-entity many-to-many
    public List<ProductSupplier> Suppliers { get; set; } = [];

    // Helpful for demonstrating restrict deletes
    public List<OrderLine> OrderLines { get; set; } = [];
}
