namespace MultiTenancy.Settings;

public class Tenant
{
    public string Name { get; set; } = null!;
    public string TId { get; set; } = null!;

    public string? ConnectionString { get; set; }

    public string paymentGateway { get; set; }

    // for stripe
    public string? StripeSecretKey { get; set; }
    public string? StripePublishableKey { get; set; }

    // for paymob
    public string? ApiKey { get; set; }
    public string? IntegrationId { get; set; }
    public string? IframeId { get; set; }



}