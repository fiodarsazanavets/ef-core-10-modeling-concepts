namespace OrderManagement.Domain;

public sealed class OrderAuditEvent
{
    public DateTime AtUtc { get; set; } = DateTime.UtcNow;
    public string Type { get; set; } = string.Empty;     // e.g. "Created", "Paid", "Packed"
    public string Actor { get; set; } = "system";         // e.g. username/service
    public string? Note { get; set; }
}
