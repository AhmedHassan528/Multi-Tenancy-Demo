
using Microsoft.EntityFrameworkCore;
using Stripe;
using Stripe.Checkout;

namespace MultiTenancy.Services.OrderService
{
    public class OrderService : IOrderService
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly ITenantService _tenantService;

        public OrderService(ApplicationDbContext dbContext, ITenantService tenantService)
        {
            _dbContext = dbContext;
            _tenantService = tenantService;
        }

        public async Task<(Order order, string clientSecret)> CreateOrderAsync(string userID, int cartId, int addressID, string HostUrl)
        {
            var tenant = _tenantService.GetCurrentTenant();

            var cart = await _dbContext.Carts
                .Where(w => w.CartOwner == userID)
                .Include(c => c.Products).ThenInclude(p => p.Product)
                .FirstOrDefaultAsync(c => c.Id == cartId);

            var address = await _dbContext.Addresses.FirstOrDefaultAsync(a => a.Id == addressID);

            if (cart == null)
            {
                throw new Exception("Cart not found");
            }
            if (addressID == 0)
            {
                throw new Exception("Address not set for this cart");
            }
            if (!cart.Products.Any())
            {
                throw new Exception("Cart is empty");
            }

            StripeConfiguration.ApiKey = tenant.StripeSecretKey;

            // Create PaymentIntent
            var options = new SessionCreateOptions
            {
                LineItems = cart.Products.Select(ci => new SessionLineItemOptions
                {
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                        Currency = "usd",
                        UnitAmount = (long)(ci.Price * 100), // Convert to cents
                        ProductData = new SessionLineItemPriceDataProductDataOptions
                        {
                            Name = ci.Product?.title ?? "Unknown Product",
                        },
                    },
                    Quantity = ci.Count,
                }).ToList(),
                Mode = "payment",
                SuccessUrl = HostUrl + "/orderHistory?session_id={CHECKOUT_SESSION_ID}",
                CancelUrl = HostUrl + "/cart",
            };

            var service = new SessionService();
            var session = await service.CreateAsync(options);

            // Create Order with Items
            var order = new Order
            {
                CartOwner = cart.CartOwner,
                CartId = cart.Id,
                TotalAmount = cart.TotalCartPrice,
                PaymentIntentId = session.Id,
                AddressId = addressID,
                TenantId = tenant.TId,
                Items = cart.Products.Select(ci => new OrderItem
                {
                    Count = ci.Count,
                    Price = ci.Price,
                    ProductId = ci.ProductId,
                    ProductName = ci.Product?.title, // Matches ProductModel
                    TenantId = tenant.TId
                }).ToList()
            };

            _dbContext.Orders.Add(order);
            cart.Products.Clear();
            cart.TotalCartPrice = 0;
            cart.UpdatedAt = DateTime.UtcNow;

            await _dbContext.SaveChangesAsync();

            return (order, session.Id);
        }
        public async Task<Order> GetOrderAsync(int orderId)
        {
            var tenant = _tenantService.GetCurrentTenant();
            var order = await _dbContext.Orders
                .Include(o => o.Items).ThenInclude(i => i.Product)
                .Include(o => o.Address)
                .FirstOrDefaultAsync(o => o.Id == orderId && o.TenantId == tenant.TId);

            if (order == null)
            {
                throw new Exception("Order not found");
            }

            return order;
        }
    }
}
