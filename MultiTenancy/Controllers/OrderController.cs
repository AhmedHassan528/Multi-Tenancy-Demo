using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MultiTenancy.Services.OrderService;
using Stripe;
using Stripe.Checkout;

namespace MultiTenancy.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;
        private readonly ITenantService _tenantService;
        private readonly ApplicationDbContext context;

        public OrderController(IOrderService orderService, ITenantService tenantService, ApplicationDbContext context)
        {
            _orderService = orderService;
            _tenantService = tenantService;
            this.context = context;
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateOrder([FromHeader] string userID, [FromBody] CreateOrderRequest request)
        {
            try
            {

                var (order, sessionId) = await _orderService.CreateOrderAsync(userID, request.CartId, request.addressId, request.HostUrl);
                return Ok(new { orderId = order.Id, sessionId });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("{orderId}")]
        public async Task<IActionResult> GetOrder(int orderId)
        {
            try
            {
                var order = await _orderService.GetOrderAsync(orderId);
                return Ok(order);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("verify-session/{sessionId}")]
        public async Task<IActionResult> VerifySession(string sessionId)
        {
            var tenant = _tenantService.GetCurrentTenant();
            StripeConfiguration.ApiKey = tenant.StripeSecretKey;

            var service = new SessionService();
            var session = await service.GetAsync(sessionId);

            if (session.Status == "complete" && session.PaymentStatus == "paid")
            {
                var order = await context.Orders
                    .FirstOrDefaultAsync(o => o.PaymentIntentId == session.Id );
                if (order != null)
                {
                    order.status = true;
                    return Ok(new { orderId = order.Id, status = "success" });
                }
            }
            return BadRequest("Payment verification failed or order not found");
        }


    }
    public class CreateOrderRequest
    {
        public int CartId { get; set; }
        public int addressId { get; set; }

        public string? HostUrl { get; set; }

    }
}
