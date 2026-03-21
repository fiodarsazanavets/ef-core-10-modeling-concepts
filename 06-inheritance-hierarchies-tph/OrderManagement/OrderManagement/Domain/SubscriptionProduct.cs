namespace OrderManagement.Domain;

public sealed class SubscriptionProduct : Product
{
    public int BillingPeriodMonths { get; set; }
    public bool AutoRenews { get; set; }
    public DateOnly? TrialEndsOn { get; set; }
}
