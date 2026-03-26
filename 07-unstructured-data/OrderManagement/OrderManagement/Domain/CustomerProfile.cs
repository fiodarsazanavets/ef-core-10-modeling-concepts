namespace OrderManagement.Domain;

public class CustomerProfile
{
    public Customer Customer { get; set; } = null!;
    public string LoyaltyTier { get; set; } = "Standard";
    public DateTime? DateOfBirthUtc { get; set; }
}
