using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.DotNet.Scaffolding.Shared.Messaging;
using Microsoft.EntityFrameworkCore;
using MultiTenancy.Services.OrderService;
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


        public OrderController(IOrderService orderService, ITenantService tenantService, ApplicationDbContext context, ITrafficServices trafficServices)
        {
            _orderService = orderService;
            _tenantService = tenantService;
            this.context = context;
            _trafficServices = trafficServices;
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateOrder([FromBody] CreateOrderRequest request)
        {
            await _trafficServices.AddReqCountAsync();
            await _trafficServices.AddOrderCountAsync();


            var userID = User.FindFirst("uid")?.Value;
            var CustomerName = User.FindFirst("CustomerName")?.Value;

            if (userID == null || CustomerName == null)
            {
                return BadRequest(new { message = "some thing error when creating order try again later!" });
            }
            try
            {

                var (order, sessionId) = await _orderService.CreateOrderAsync(userID, request.CartId, request.addressId, request.HostUrl, CustomerName);
                return Ok(new { orderId = order.Id, sessionId });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "some thing error when creating order try again later!" });
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
            if (userID == null)
            {
                return NotFound();
            }
            try
            {
                var orders = await _orderService.GetAllOrdersAsync(userID);
                return Ok(orders);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "some thing error when getting orders try again later!" });
            }
        }


        [HttpGet("AdminGetAllOrdersl")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AdminGetAllOrders()
        {
            await _trafficServices.AddReqCountAsync();

            var userID = User.FindFirst("uid")?.Value;
            if (userID == null)
            {
                return NotFound();
            }
            try
            {
                var orders = await _orderService.AdminGetAllOrdersAsync(userID);
                return Ok(orders);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "some thing error when getting order try again later!" });
            }
        }
        // order details
        [HttpGet("{orderId}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetOrder(int orderId)
        {
            await _trafficServices.AddReqCountAsync();

            try
            {
                var order = await _orderService.GetOrderAsync(orderId);
                return Ok(order);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "some thing error when getting order try again later!" });
            }
        }

        [HttpPut("updateOrderStatus/{orderId}/{statusMass}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> updateOrderStatus(int orderId, string statusMass)
        {
            await _trafficServices.AddReqCountAsync();

            var userId = User.FindFirst("uid")?.Value;
            if (userId == null)
            {
                return NotFound();
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
                return BadRequest(new { message = "some thing error when getting order try again later!" });
            }
        }


    }
    public class CreateOrderRequest
    {
        public int CartId { get; set; }
        public int addressId { get; set; }

        public string? HostUrl { get; set; }

    }
}
