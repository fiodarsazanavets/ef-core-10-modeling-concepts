namespace OrderManagement.Domain;

public class Customer
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public Address ShippingAddress { get; set; } = new();
    public Address? BillingAddress { get; set; }   // optional complex type (EF Core 10 feature)

    // 1:1
    public CustomerProfile Profile { get; set; } = null!;

    // 1:many
    public List<Order> Orders { get; set; } = [];
}
