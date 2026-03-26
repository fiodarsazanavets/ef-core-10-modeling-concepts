namespace OrderManagement.Domain;

public sealed class NotificationPreferences
{
    public bool EmailEnabled { get; set; } = true;
    public bool SmsEnabled { get; set; } = false;
    public bool MarketingOptIn { get; set; } = false;
}
