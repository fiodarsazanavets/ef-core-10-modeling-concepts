namespace OrderManagement.Domain;

public class SalesAgent
{
    public int Id { get; set; }
    public string DisplayName { get; set; } = string.Empty;
    public List<Order> Orders { get; set; } = [];
}
