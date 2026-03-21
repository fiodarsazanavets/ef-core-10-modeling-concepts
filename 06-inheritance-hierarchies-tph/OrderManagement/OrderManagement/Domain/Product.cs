namespace OrderManagement.Domain;

// Root: shared identity + shared columns
public abstract class Product
{
    public int Id { get; set; }
    public string Sku { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public Money Price { get; set; }

    // Useful for filters & strategy choice
    public bool IsDiscontinued { get; set; }

    public List<Tag> Tags { get; set; } = [];
    public List<ProductSupplier> Suppliers { get; set; } = [];
    public List<OrderLine> OrderLines { get; set; } = [];
}
