namespace OrderManagement.Domain;

public sealed class UiPreferences
{
    public string Theme { get; set; } = "light";  // “light”/“dark”
    public string Language { get; set; } = "en-GB";
}
