using Microsoft.AspNetCore.Mvc;

namespace MultiTenancy.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartController : ControllerBase
    {
        private readonly ICartServices _cartServices;
        public CartController(ICartServices cartServices)
        {
            _cartServices = cartServices;
        }

        [HttpGet]
        public async Task<IActionResult> GetUserCart([FromHeader] string userId)
        {
            try
            {
                var cart = await _cartServices.GetUserCartAsync(userId);
                return Ok(cart);
            }
            catch (Exception ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        [HttpPost("add/{ProductId}")]
        public async Task<IActionResult> AddItemToCart([FromHeader] string userId, int ProductId)
        {
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
        public async Task<IActionResult> RemoveItemFromCart([FromHeader] string userId, int ProductId)
        {
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
        public async Task<IActionResult> IncreaseItemCount([FromHeader] string UserId, int ProductId)
        {
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
        public async Task<IActionResult> DecreaseItemCount([FromHeader] string UserId, int ProductId)
        {
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
        public async Task<IActionResult> ClearCart([FromHeader] string userId)
        {
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
