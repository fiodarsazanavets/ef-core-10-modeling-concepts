namespace OrderManagement.Domain;

public class Customer
{
    public int Id { get; set; }                 // convention: PK
    public string Name { get; set; } = string.Empty;      // convention: required? depends on nullable context
    public string Email { get; set; } = string.Empty;

    // convention: one-to-many
    public List<Order> Orders { get; set; } = [];
}
