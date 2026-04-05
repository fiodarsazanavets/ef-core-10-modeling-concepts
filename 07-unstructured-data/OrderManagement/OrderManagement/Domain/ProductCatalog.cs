namespace OrderManagement.Domain;

public sealed class ProductCatalog
{
    public string[] Keywords { get; set; } = Array.Empty<string>();
    public ICollection<CatalogAttribute> Attributes { get; set; } = [];
}
