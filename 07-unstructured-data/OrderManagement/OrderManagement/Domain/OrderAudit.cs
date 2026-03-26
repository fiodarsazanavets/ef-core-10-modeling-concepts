namespace OrderManagement.Domain;

public sealed class OrderAudit
{
    // Nested collection inside a JSON column
    public OrderAuditEvent[] Events { get; set; } = Array.Empty<OrderAuditEvent>();

    // A “query hot” summary field you can also expose relationally later
    public string? LastEventType { get; set; }
}
