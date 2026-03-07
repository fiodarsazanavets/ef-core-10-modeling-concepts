namespace OrderManagement.Domain;

public class OrderLine
{
    public int OrderId { get; set; }
    public Order Order { get; set; } = null!;
    public int ProductId { get; set; }
    public Product Product { get; set; } = null!;
    public int Quantity { get; set; }
    public Money UnitPrice { get; set; }
}
