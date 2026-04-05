namespace ScaffoldedLegacyApp.Legacy;

public sealed class LegacyOrderProjection
{
    public int OrderId { get; init; }
    public string OrderNo { get; init; } = "";
    public string CustomerName { get; init; } = "";
    public string StatusName { get; init; } = "";
    public string? SalesRepName { get; init; }
    public decimal NetAmount { get; init; }
    public int TotalQuantity { get; init; }
}
