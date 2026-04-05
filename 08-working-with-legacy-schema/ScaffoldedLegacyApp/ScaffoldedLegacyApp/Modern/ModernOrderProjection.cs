namespace ScaffoldedLegacyApp.Modern;

public sealed class ModernOrderProjection
{
    public int OrderId { get; init; }
    public string OrderNo { get; init; } = "";
    public string CustomerName { get; init; } = "";
    public string StatusName { get; init; } = "";
    public string SalesRepName { get; init; } = "[Unassigned]";
    public decimal NetAmount { get; init; }
    public int TotalQuantity { get; init; }
}
