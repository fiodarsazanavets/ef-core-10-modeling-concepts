namespace OrderManagement.Domain;

public sealed class PreferenceEntry
{
    public string Key { get; set; } = string.Empty;
    public string? Value { get; set; }
}
