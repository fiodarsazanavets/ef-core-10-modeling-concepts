namespace OrderManagement.Domain;

public class Order
{
    public int Id { get; set; }

    public DateTime CreatedUtc { get; set; } = DateTime.UtcNow;

    // convention: FK + relationship
    public int CustomerId { get; set; }
    public Customer Customer { get; set; } = null!;

    public List<OrderLine> Lines { get; set; } = [];

    // convenience total (not mapped unless configured later)
    public decimal Total => Lines.Sum(l => l.UnitPrice * l.Quantity);
}
