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

        public CartController(ICartServices cartServices, ITrafficServices trafficServices)
        {
            _cartServices = cartServices;
            _trafficServices = trafficServices;
        }

        [HttpGet]
        public async Task<IActionResult> GetUserCart()
        {
            await _trafficServices.AddReqCountAsync();

            var userId = User.FindFirst("uid")?.Value;
            if (userId == null)
            {
                return BadRequest();
            }
            try
            {
                var cart = await _cartServices.GetUserCartAsync(userId);
                return Ok(cart);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "some thing error when get cart" });
            }
        }

        [HttpPost("add/{ProductId}")]
        public async Task<IActionResult> AddItemToCart( int ProductId)
        {
            await _trafficServices.AddReqCountAsync();

            var userId = User.FindFirst("uid")?.Value;
            if (userId == null)
            {
                return NotFound();
            }
            try
            {
                var cart = await _cartServices.AddItemToCartAsync(userId, ProductId, 1);
                return Ok(cart);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "some thing error when adding to cart \n try again later!" });
            }
        }

        [HttpDelete("Remove/{ProductId}")]
        public async Task<IActionResult> RemoveItemFromCart(int ProductId)
        {
            await _trafficServices.AddReqCountAsync();

            var userId = User.FindFirst("uid")?.Value;
            if (userId == null)
            {
                return BadRequest();
            }
            try
            {
                var cart = await _cartServices.RemoveItemFromCartAsync(userId, ProductId);
                return Ok(cart);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "some thing error when Removing from cart \n try again later!" });
            }
        }

        [HttpPut("increase/{ProductId}")]
        public async Task<IActionResult> IncreaseItemCount( int ProductId)
        {
            await _trafficServices.AddReqCountAsync();

            var UserId = User.FindFirst("uid")?.Value;
            if (UserId == null)
            {
                return NotFound();
            }
            try
            {
                var cart = await _cartServices.IncreaseItemCountAsync(UserId, ProductId);
                return Ok(cart);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "some thing error on cart \n try again later!" });
            }
        }

        [HttpPut("decrease/{ProductId}")]
        public async Task<IActionResult> DecreaseItemCount(int ProductId)
        {
            await _trafficServices.AddReqCountAsync();

            var UserId = User.FindFirst("uid")?.Value;
            if (UserId == null)
            {
                return NotFound();
            }
            try
            {
                var cart = await _cartServices.DecreaseItemCountAsync(UserId, ProductId);
                return Ok(cart);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "some thing error on cart \n try again later!" });
            }
        }

        [HttpDelete("clear")]
        public async Task<IActionResult> ClearCart()
        {
            await _trafficServices.AddReqCountAsync();

            var userId = User.FindFirst("uid")?.Value;
            if (userId == null)
            {
                return NotFound();
            }
            try
            {
                var cart = await _cartServices.ClearCartAsync(userId);
                return Ok(new { message = "Cart cleared successfully", cart });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "some thing error when clear cart \n try again later!" });
            }
        }

    }
}
