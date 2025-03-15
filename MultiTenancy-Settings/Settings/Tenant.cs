namespace MultiTenancy.Settings;

public class Tenant
{
    public string Name { get; set; } = null!;
    public string TId { get; set; } = null!;
    public string? ConnectionString { get; set; }
    public string StripeSecretKey { get; set; }
    public string StripePublishableKey { get; set; }
}