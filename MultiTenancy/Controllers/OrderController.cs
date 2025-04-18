using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.DotNet.Scaffolding.Shared.Messaging;
using Microsoft.EntityFrameworkCore;
using MultiTenancy.Dtos.PaymobDtos;
using MultiTenancy.Services.OrderService;
using MultiTenancy.Services.paymobServices;
using MultiTenancy.Services.TrafficServices;
using Stripe;
using Stripe.Checkout;

namespace MultiTenancy.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;
        private readonly ITenantService _tenantService;
        private readonly ApplicationDbContext context;
        private readonly ITrafficServices _trafficServices;
        private readonly IPaymentService _PaymentService;
        private readonly IAuthService _authService;



        public OrderController(IOrderService orderService, ITenantService tenantService, ApplicationDbContext context, ITrafficServices trafficServices, IPaymentService PaymentService, IAuthService authService)
        {
            _orderService = orderService;
            _tenantService = tenantService;
            this.context = context;
            _trafficServices = trafficServices;
            _PaymentService = PaymentService;
            _authService = authService;
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateOrder([FromBody] CreateOrderRequest request)
        {
            await _trafficServices.AddReqCountAsync();
            await _trafficServices.AddOrderCountAsync();


            var userID = User.FindFirst("uid")?.Value;
            var CustomerName = User.FindFirst("CustomerName")?.Value;
            if (CustomerName == null)
            {
                CustomerName = "Customer";
            }

            if (userID == null || !await _authService.isUser(userID))
            {
                return NotFound(new { message = "Error: User not found. \nPlease ensure you have entered the correct username or email, or register for an account.", StatusCode = 401 });
            }

            try
            {

                var (order, sessionId) = await _orderService.CreateOrderAsync(userID, request.CartId, request.addressId, request.HostUrl, CustomerName);
                return Ok(new { orderId = order.Id, sessionId });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }



        [HttpGet("verify-session/{sessionId}")]
        public async Task<IActionResult> VerifySession(string sessionId)
        {
            await _trafficServices.AddReqCountAsync();


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
                    context.Orders.Update(order);
                    await context.SaveChangesAsync();

                    return Ok(new { orderId = order.Id, status = "success" });
                }
            }
            return BadRequest("Payment verification failed or order not found");
        }

        [HttpGet("GetUserOrders")]
        public async Task<IActionResult> GetUserOrders()
        {
            await _trafficServices.AddReqCountAsync();

            var userID = User.FindFirst("uid")?.Value;

            if (userID == null || !await _authService.isUser(userID))
            {
                return NotFound(new { message = "Error: User not found. \nPlease ensure you have entered the correct username or email, or register for an account.", StatusCode = 401 });
            }
            try
            {
                var orders = await _orderService.GetAllOrdersAsync(userID);
                return Ok(orders);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }


        [HttpGet("AdminGetAllOrdersl")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AdminGetAllOrders()
        {
            await _trafficServices.AddReqCountAsync();

            var userID = User.FindFirst("uid")?.Value;
            if (userID == null || !await _authService.isAdmin(userID))
            {
                return NotFound(new { message = "Error: User not found. \nPlease ensure you have entered the correct username or email, or register for an account.", StatusCode = 401 });
            }

            try
            {
                var orders = await _orderService.AdminGetAllOrdersAsync(userID);
                return Ok(orders);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
        // order details
        [HttpGet("{orderId}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetOrder(int orderId)
        {
            await _trafficServices.AddReqCountAsync();
            var userID = User.FindFirst("uid")?.Value;

            if (userID == null || !await _authService.isAdmin(userID))
            {
                return NotFound(new { message = "Error: User not found. \nPlease ensure you have entered the correct username or email, or register for an account.", StatusCode = 401 });
            }

            try
            {
                var order = await _orderService.GetOrderAsync(orderId);
                return Ok(order);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("updateOrderStatus/{orderId}/{statusMass}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> updateOrderStatus(int orderId, string statusMass)
        {
            await _trafficServices.AddReqCountAsync();

            var userId = User.FindFirst("uid")?.Value;
            if (userId == null || !await _authService.isAdmin(userId))
            {
                return NotFound(new { message = "Error: User not found. \nPlease ensure you have entered the correct username or email, or register for an account.", StatusCode = 401 });
            }

            try
            {
                var order = await _orderService.updateOrderStatus(userId, orderId, statusMass);
                if (string.IsNullOrEmpty(order))
                {
                    return Ok(order);
                }
                return BadRequest(new { message = "some thing error when getting order try again later!" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }


        [HttpPost("pay")]
        public async Task<IActionResult> Pay([FromBody] PaymentRequest request)
        {
            var authToken = await _PaymentService.GetAuthTokenAsync();
            var orderId = await _PaymentService.CreateOrderAsync(authToken, request.AmountCents);
            var paymentKey = await _PaymentService.GeneratePaymentKeyAsync(authToken, orderId, request.AmountCents, request.BillingEmail);

            var tenant = _tenantService.GetCurrentTenant();
            var IframeId = tenant?.IframeId;

            var iframeUrl = $"https://accept.paymobsolutions.com/api/acceptance/iframes/{IframeId}?payment_token={paymentKey}";

            return Ok(new { url = iframeUrl });
        }


    }
    public class CreateOrderRequest
    {
        public int CartId { get; set; }
        public int addressId { get; set; }

        public string? HostUrl { get; set; }

    }
}
