namespace OrderManagement.Domain;

public sealed class OrderAudit
{
    // Nested collection inside a JSON column
    public ICollection<OrderAuditEvent> Events { get; set; } = [];

    // A “query hot” summary field you can also expose relationally later
    public string? LastEventType { get; set; }
}
