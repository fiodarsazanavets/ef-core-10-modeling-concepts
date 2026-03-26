namespace OrderManagement.Domain;

public sealed class CosmosOrderDocument
{
    // Cosmos id (string is typical)
    public string Id { get; set; } = Guid.NewGuid().ToString("n");

    // Partition key (must be present)
    public string CustomerId { get; set; } = string.Empty;

    public DateTime CreatedUtc { get; set; } = DateTime.UtcNow;

    public Address ShipTo { get; set; } = new();

    public List<CosmosOrderLineDocument> Lines { get; set; } = new();

    // Same audit concept as relational JSON column
    public OrderAudit Audit { get; set; } = new();
}

public sealed class CosmosOrderLineDocument
{
    public int ProductId { get; set; }
    public int Quantity { get; set; }
    public Money UnitPrice { get; set; } = new(0m, "USD");
}
