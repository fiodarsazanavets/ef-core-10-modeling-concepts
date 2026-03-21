namespace OrderManagement.Domain;

// Concrete types
public sealed class PhysicalProduct : ShippableProduct
{
    public decimal WeightKg { get; set; }
    public Dimensions Dimensions { get; set; } // derived-only complex property

    // Permutation: same property name/type on siblings (shared column demo in TPH)
    public string? Manufacturer { get; set; }
}
