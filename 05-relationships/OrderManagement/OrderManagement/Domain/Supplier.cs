namespace OrderManagement.Domain;

public class Supplier
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;

    public List<ProductSupplier> Products { get; set; } = [];
}
