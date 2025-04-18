using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MultiTenancy.Services.TrafficServices;

namespace MultiTenancy.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CartController : ControllerBase
    {

        private readonly ICartServices _cartServices;
        private readonly ITrafficServices _trafficServices;
        private readonly IAuthService _authService;


        public CartController(ICartServices cartServices, ITrafficServices trafficServices, IAuthService authService)
        {
            _cartServices = cartServices;
            _trafficServices = trafficServices;
            _authService = authService;
        }

        [HttpGet]
        public async Task<IActionResult> GetUserCart()
        {
            await _trafficServices.AddReqCountAsync();

            var userId = User.FindFirst("uid")?.Value;
            if (userId == null || !await _authService.isUser(userId))
            {
                return NotFound(new { message = "Error: User not found. \nPlease ensure you have entered the correct username or email, or register for an account.", StatusCode = 401 });
            }

            try
            {
                var cart = await _cartServices.GetUserCartAsync(userId);
                return Ok(cart);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("add/{ProductId}")]
        public async Task<IActionResult> AddItemToCart( int ProductId)
        {
            await _trafficServices.AddReqCountAsync();

            var userId = User.FindFirst("uid")?.Value;
            if (userId == null || !await _authService.isUser(userId))
            {
                return NotFound(new { message = "Error: User not found. \nPlease ensure you have entered the correct username or email, or register for an account.", StatusCode = 401 });
            }

            try
            {
                var cart = await _cartServices.AddItemToCartAsync(userId, ProductId, 1);
                return Ok(cart);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("Remove/{ProductId}")]
        public async Task<IActionResult> RemoveItemFromCart(int ProductId)
        {
            await _trafficServices.AddReqCountAsync();

            var userId = User.FindFirst("uid")?.Value;
            if (userId == null || !await _authService.isUser(userId))
            {
                return NotFound(new { message = "Error: User not found. \nPlease ensure you have entered the correct username or email, or register for an account.", StatusCode = 401 });
            }
            try
            {
                var cart = await _cartServices.RemoveItemFromCartAsync(userId, ProductId);
                return Ok(cart);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("increase/{ProductId}")]
        public async Task<IActionResult> IncreaseItemCount( int ProductId)
        {
            await _trafficServices.AddReqCountAsync();

            var UserId = User.FindFirst("uid")?.Value;
            if (UserId == null || !await _authService.isUser(UserId))
            {
                return NotFound(new { message = "Error: User not found. \nPlease ensure you have entered the correct username or email, or register for an account.", StatusCode = 401 });
            }

            try
            {
                var cart = await _cartServices.IncreaseItemCountAsync(UserId, ProductId);
                return Ok(cart);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("decrease/{ProductId}")]
        public async Task<IActionResult> DecreaseItemCount(int ProductId)
        {
            await _trafficServices.AddReqCountAsync();

            var UserId = User.FindFirst("uid")?.Value;
            if (UserId == null || !await _authService.isUser(UserId))
            {
                return NotFound(new { message = "Error: User not found. \nPlease ensure you have entered the correct username or email, or register for an account.", StatusCode = 401 });
            }

            try
            {
                var cart = await _cartServices.DecreaseItemCountAsync(UserId, ProductId);
                return Ok(cart);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("clear")]
        public async Task<IActionResult> ClearCart()
        {
            await _trafficServices.AddReqCountAsync();

            var userId = User.FindFirst("uid")?.Value;
            if (userId == null || !await _authService.isUser(userId))
            {
                return NotFound(new { message = "Error: User not found. \nPlease ensure you have entered the correct username or email, or register for an account.", StatusCode = 401 });
            }
            try
            {
                var cart = await _cartServices.ClearCartAsync(userId);
                return Ok(new { message = "Cart cleared successfully", cart });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

    }
}
