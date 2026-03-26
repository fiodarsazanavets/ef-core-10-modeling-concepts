namespace OrderManagement.Domain;

public sealed class DigitalProduct : Product
{
    public string DownloadUrl { get; set; } = string.Empty;
    public long FileSizeBytes { get; set; }

    // Same name/type as PhysicalProduct.Manufacturer (TPH “shared column” demo)
    public string? Manufacturer { get; set; }
}
