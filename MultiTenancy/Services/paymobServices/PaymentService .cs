using Microsoft.AspNetCore.Authentication.OAuth;
using MultiTenancy.Dtos.PaymobDtos;

namespace MultiTenancy.Services.paymobServices
{
    public class PaymentService : IPaymentService
    {
        private readonly HttpClient _httpClient;
        private readonly ITenantService _tenantService;


        public PaymentService(HttpClient httpClient, ITenantService tenantService)
        {
            _httpClient = httpClient;
            _tenantService = tenantService;
        }

        public async Task<string> GetAuthTokenAsync()
        {
            var tenant = _tenantService.GetCurrentTenant();
            var ApiKey = tenant?.ApiKey;

            var payload = new { api_key = ApiKey };
            var response = await _httpClient.PostAsJsonAsync("https://accept.paymobsolutions.com/api/auth/tokens", payload);
            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadFromJsonAsync<AuthTokenResponse>();
            return result?.token;
        }

        public async Task<int> CreateOrderAsync(string authToken, int amountCents)
        {
            var payload = new
            {
                auth_token = authToken,
                delivery_needed = false,
                amount_cents = amountCents,
                currency = "EGP",
                items = new object[] { }
            };

            var response = await _httpClient.PostAsJsonAsync("https://accept.paymobsolutions.com/api/ecommerce/orders", payload);
            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadFromJsonAsync<OrderResponse>();
            return result.id;
        }

        public async Task<string> GeneratePaymentKeyAsync(string authToken, int orderId, int amountCents, string billingEmail)
        {
            var tenant = _tenantService.GetCurrentTenant();
            var IntegrationId = tenant?.IntegrationId;

            var payload = new
            {
                auth_token = authToken,
                amount_cents = amountCents,
                expiration = 3600,
                order_id = orderId,
                billing_data = new
                {
                    apartment = "NA",
                    email = billingEmail,
                    floor = "NA",
                    first_name = "John",
                    last_name = "Doe",
                    phone_number = "+201000000000",
                    city = "Cairo",
                    country = "EG",
                    state = "Cairo",
                    street = "NA",
                    building = "NA",
                    shipping_method = "NA",
                    postal_code = "NA"
                },
                currency = "EGP",
                integration_id = int.Parse(IntegrationId)
            };

            var response = await _httpClient.PostAsJsonAsync("https://accept.paymobsolutions.com/api/acceptance/payment_keys", payload);

            var responseContent = await response.Content.ReadAsStringAsync();
            Console.WriteLine(responseContent);

            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadFromJsonAsync<PaymentKeyResponse>();
            return result?.token;
        }
    }

}
