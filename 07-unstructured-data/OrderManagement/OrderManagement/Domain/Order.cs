namespace OrderManagement.Domain;

public class Order
{
    public int Id { get; set; }
    public DateTime CreatedUtc { get; set; } = DateTime.UtcNow;
    public int CustomerId { get; set; }
    public Address ShipTo { get; set; } = new();
    public OrderAudit Audit { get; set; } = new();
    public Customer Customer { get; set; } = null!;
    public List<OrderLine> Lines { get; set; } = [];
    public SalesAgent? SalesAgent { get; set; }

    public decimal Total => Lines.Sum(l => l.UnitPrice.Amount * l.Quantity);
}
