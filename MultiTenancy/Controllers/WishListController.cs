using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MultiTenancy.Services.WishListServices;

namespace MultiTenancy.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]

    public class WishListController : ControllerBase
    {
        private readonly IWishListServices _wishList;

        public WishListController(IWishListServices wishList)
        {
            _wishList = wishList;
        }
        [HttpGet("Product")]
        public async Task<IActionResult> GetWishlistProducts([FromHeader] string userId)
        {
            try
            {
                var Products = await _wishList.GetAllProductinWishList(userId);
                return Ok(Products);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetWishlist([FromHeader] string userId)
        {
            try
            {
                var wishlist = await _wishList.GetWishlistAsync(userId);
                return Ok(wishlist);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("add/{ProductId}")]
        public async Task<IActionResult> AddToWishlist([FromHeader] string userId, int ProductId)
        {
            try
            {
                var wishlist = await _wishList.AddToWishlistAsync(userId, ProductId);
                return Ok(new { message = "the product are added successfully", wishlist });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("remove/{ProductId}")]
        public async Task<IActionResult> RemoveFromWishlist([FromHeader] string userId, int ProductId)
        {
            try
            {
                var wishlist = await _wishList.RemoveFromWishlistAsync(userId, ProductId);
                return Ok(new { message = "the product are deleted successfully", wishlist });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // Clear Wishlist
        [HttpDelete("clear")]
        public async Task<IActionResult> ClearWishlist([FromHeader] string userId)
        {
            try
            {
                var success = await _wishList.ClearWishlistAsync(userId);
                return success ? Ok(new { message = "Wishlist cleared successfully" }) : NotFound();
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
