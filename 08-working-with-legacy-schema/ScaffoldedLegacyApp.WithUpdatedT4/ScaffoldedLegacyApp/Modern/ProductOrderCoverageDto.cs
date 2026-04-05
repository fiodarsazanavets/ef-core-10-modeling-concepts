namespace ScaffoldedLegacyApp.Modern;

public sealed class ProductOrderCoverageDto
{
    public string SKU { get; init; } = "";
    public string ProductName { get; init; } = "";
    public int? OrderId { get; init; }
    public string? OrderNo { get; init; }
}
