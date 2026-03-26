namespace OrderManagement.Domain;

public sealed class ProductCatalog
{
    public string[] Keywords { get; set; } = Array.Empty<string>();
    public CatalogAttribute[] Attributes { get; set; } = Array.Empty<CatalogAttribute>();
}
