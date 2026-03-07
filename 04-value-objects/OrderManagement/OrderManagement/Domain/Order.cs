namespace OrderManagement.Domain;

public class Order
{
    public int Id { get; set; }
    public DateTime CreatedUtc { get; set; } = DateTime.UtcNow;
    public int CustomerId { get; set; }
    public Address ShipTo { get; set; } = new();   // snapshot at order time
    public Customer Customer { get; set; } = null!;
    public List<OrderLine> Lines { get; set; } = [];
    public decimal Total => Lines.Sum(l => l.UnitPrice.Amount * l.Quantity);
}
