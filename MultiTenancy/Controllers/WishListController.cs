using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MultiTenancy.Services.TrafficServices;
using MultiTenancy.Services.WishListServices;

namespace MultiTenancy.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]

    public class WishListController : ControllerBase
    {
        private readonly IWishListServices _wishList;
        private readonly ITrafficServices _trafficServices;

        public WishListController(IWishListServices wishList, ITrafficServices trafficServices)
        {
            _wishList = wishList;
            _trafficServices = trafficServices;
        }
        [HttpGet("Product")]
        public async Task<IActionResult> GetWishlistProducts()
        {
            await _trafficServices.AddReqCountAsync();

            var userId = User.FindFirst("uid")?.Value;
            if (userId == null)
            {
                return NotFound();
            }
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
        public async Task<IActionResult> GetWishlist()
        {
            await _trafficServices.AddReqCountAsync();

            var userId = User.FindFirst("uid")?.Value;
            if (userId == null)
            {
                return NotFound();
            }
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
        public async Task<IActionResult> AddToWishlist(int ProductId)
        {
            await _trafficServices.AddReqCountAsync();

            var userId = User.FindFirst("uid")?.Value;
            if (userId == null)
            {
                return NotFound();
            }
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
        public async Task<IActionResult> RemoveFromWishlist(int ProductId)
        {
            await _trafficServices.AddReqCountAsync();

            var userId = User.FindFirst("uid")?.Value;
            if (userId == null)
            {
                return NotFound();
            }
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
        public async Task<IActionResult> ClearWishlist()
        {
            await _trafficServices.AddReqCountAsync();

            var userId = User.FindFirst("uid")?.Value;
            if (userId == null)
            {
                return NotFound();
            }
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
