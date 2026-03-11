namespace OrderManagement.Domain;

public class CustomerProfile
{
    // no CustomerId property on purpose (we’ll use a shadow PK/FK)
    public Customer Customer { get; set; } = null!;
    public string LoyaltyTier { get; set; } = "Standard";
    public DateTime? DateOfBirthUtc { get; set; }
}
