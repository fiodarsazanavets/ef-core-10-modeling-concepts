namespace OrderManagement.Domain;

public sealed class CatalogAttribute
{
    public string Name { get; set; } = string.Empty;   // e.g. "color", "material"
    public string Value { get; set; } = string.Empty;  // e.g. "black", "steel"
    public string? Unit { get; set; }                  // e.g. "kg"
}