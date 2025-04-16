
using Microsoft.EntityFrameworkCore;
using Stripe;
using Stripe.Checkout;

namespace MultiTenancy.Services.OrderService
{
    public class OrderService : IOrderService
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly ITenantService _tenantService;
        private readonly UserManager<AppUser> _userManager;


        public OrderService(ApplicationDbContext dbContext, ITenantService tenantService, UserManager<AppUser> userManager)
        {
            _dbContext = dbContext;
            _tenantService = tenantService;
            _userManager = userManager;
        }


        public async Task<(Order order, string clientSecret)> CreateOrderAsync(string userID, int cartId, int addressID, string HostUrl, string CustomerName)
        {
            var tenant = _tenantService.GetCurrentTenant();

            var cart = await _dbContext.Carts
                .Where(w => w.CartOwner == userID)
                .Include(c => c.Products).ThenInclude(p => p.Product).ThenInclude(p => p.Brand)
                .Include(c => c.Products).ThenInclude(p => p.Product).ThenInclude(p => p.Category)
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
                            Name = ci.Product?.Title ?? "Unknown Product",
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
                AddressName = address.AddressName,
                City = address.City,
                PhoneNumber = address.PhoneNumber,
                Address = address.Address,
                paymentMethodType = "Online Payment",
                TenantId = tenant.TId,
                CustomerName = CustomerName,
                Items = cart.Products.Select(ci => new OrderItem
                {
                    Count = ci.Count,
                    Price = ci.Price,
                    ProductId = ci.ProductId,
                    ProductName = ci.Product?.Title ?? "Unknown Product",
                    ProductDescription = ci.Product?.Description ?? "No Description",
                    ProductImage = ci.Product?.ImageCover ?? "No Image",
                    Category = ci.Product?.Category?.Name ?? "Unknown Category",
                    Brand = ci.Product?.Brand?.Name ?? "Unknown Brand",
                    CategoryId = ci.Product.CategoryID,
                    BrandId = ci.Product.BrandID,
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
                .Include(o => o.Items)
                .FirstOrDefaultAsync(o => o.Id == orderId && o.TenantId == tenant.TId);

            if (order == null)
            {
                throw new Exception("Order not found");
            }

            return order;
        }
        public async Task<Queue<Order>> AdminGetAllOrdersAsync(string userID)
        {
            var user = await _dbContext.Users.FindAsync(userID);
            if (user == null || !(await _userManager.IsInRoleAsync(user, "Admin")))
            {
                throw new UnauthorizedAccessException("User is not authorized to access this resource.");
            }

            var tenant = _tenantService.GetCurrentTenant();
            var orders = await _dbContext.Orders
                .Where(o => o.TenantId == tenant.TId)
                .Include(o => o.Items)
                .ToListAsync();

            return new Queue<Order>(orders);
        }

        public async Task<Queue<Order>> GetAllOrdersAsync(string userID)
        {
            var user = await _dbContext.Users.FindAsync(userID);
            if (user == null)
            {
                throw new UnauthorizedAccessException("User not found.");
            }

            var tenant = _tenantService.GetCurrentTenant();
            var orders = await _dbContext.Orders
                .Where(o => o.CartOwner == userID)
                .Include(o => o.Items)
                .ToListAsync();

            return new Queue<Order>(orders);
        }
        public async Task<string> updateOrderStatus(string userID, int orderID, string statusMass)
        {
            var user = await _dbContext.Users.FindAsync(userID);
            if (user == null || !(await _userManager.IsInRoleAsync(user, "Admin")))
            {
                throw new UnauthorizedAccessException("User is not authorized to access this resource.");
            }

            var tenant = _tenantService.GetCurrentTenant();
            var order = await _dbContext.Orders.FirstOrDefaultAsync(i => i.Id == orderID);


            if (order != null)
            {
                switch (statusMass.ToLower())
                {
                    case "received":
                        order.statusMess = "Received";
                        break;
                    case "delivered":
                        order.statusMess = "Delivered";
                        break;
                    case "canceled":
                        order.statusMess = "Canceled";
                        break;
                    default:
                        throw new ArgumentException("Invalid status message.");
                }
            }

            _dbContext.Orders.Update(order);
            await _dbContext.SaveChangesAsync();

            return "";
        }
        
    }
}
