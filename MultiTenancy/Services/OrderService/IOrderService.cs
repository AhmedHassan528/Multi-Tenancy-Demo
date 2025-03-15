

namespace MultiTenancy.Services.OrderService
{
    public interface IOrderService
    {
        Task<(Order order, string clientSecret)> CreateOrderAsync(string userID, int cartId, int addressID, string HostUrl);
        Task<Order> GetOrderAsync(int orderId);
    }
}
