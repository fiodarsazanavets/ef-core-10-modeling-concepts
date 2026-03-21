namespace OrderManagement.Domain;

public class Customer
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public Address ShippingAddress { get; set; } = new();
    public Address? BillingAddress { get; set; }
    public CustomerProfile Profile { get; set; } = null!;
    public List<Order> Orders { get; set; } = [];
}
