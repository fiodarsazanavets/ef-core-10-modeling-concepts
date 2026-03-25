namespace OrderManagement.Domain;

// Intermediate abstract type (multi-level inheritance)
// Great for: showing abstract types have no table in TPC
public abstract class ShippableProduct : Product
{
    public bool RequiresSignature { get; set; }
}
