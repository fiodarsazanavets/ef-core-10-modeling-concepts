namespace OrderManagement.Domain;

public sealed class CustomerPreferences
{
    public UiPreferences Ui { get; set; } = new();
    public NotificationPreferences Notifications { get; set; } = new();

    // Primitive collection: good JSON candidate
    public string[] FavoriteCategories { get; set; } = Array.Empty<string>();

    // “Soft schema” style attributes without a dictionary (easier to query/index)
    public PreferenceEntry[] Extras { get; set; } = Array.Empty<PreferenceEntry>();
}
