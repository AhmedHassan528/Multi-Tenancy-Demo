using Microsoft.AspNetCore.Http;
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
        public async Task<IActionResult> GetUserCart([FromHeader]string userId)
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

        [HttpPost("add")]
        public async Task<IActionResult> AddItemToCart([FromHeader] string userId, [FromBody] CartItemRequestDto request)
        {
            try
            {
                var cart = await _cartServices.AddItemToCartAsync(userId, request.ProductId, request.Quantity);
                return Ok(cart);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("remove")]
        public async Task<IActionResult> RemoveItemFromCart([FromHeader] string userId, [FromBody] CartItemRequestDto request)
        {
            try
            {
                var cart = await _cartServices.RemoveItemFromCartAsync(userId, request.ProductId);
                return Ok(cart);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
