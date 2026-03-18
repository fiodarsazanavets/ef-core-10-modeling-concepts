namespace OrderManagement.Domain;

public class ProductSupplier
{
    public int ProductId { get; set; }
    public Product Product { get; set; } = null!;

    public int SupplierId { get; set; }
    public Supplier Supplier { get; set; } = null!;

    public decimal ContractPrice { get; set; }
    public int LeadTimeDays { get; set; }
    public bool IsPreferred { get; set; }
}
