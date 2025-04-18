namespace MultiTenancy.Services.paymobServices
{
    public interface IPaymentService
    {
        Task<string> GetAuthTokenAsync();
        Task<int> CreateOrderAsync(string authToken, int amountCents);
        Task<string> GeneratePaymentKeyAsync(string authToken, int orderId, int amountCents, string billingEmail);

    }
}
