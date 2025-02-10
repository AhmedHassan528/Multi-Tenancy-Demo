using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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

        [HttpPost]
        public async Task<IActionResult> AddproductAsync(WishListDto wishList)
        {    
            var result = await _wishList.AddToWithListAsync(wishList.userID, wishList.productID);
            if(string.IsNullOrEmpty(result))
            {
                return Ok(await _wishList.GetWishListAsync(wishList.userID));
            }
            return Ok(result);
        }
        [HttpGet]
        public async Task<IActionResult> GetAllWithList([FromHeader] string userID)
        {
            var result = await _wishList.GetWishListAsync(userID);
            return Ok(result);
        }

        [HttpDelete("{productID}")]
        public async Task<IActionResult> DeleteWishList(int productID, [FromHeader] string userID)
        {
            if (userID == null)
            {
                return NotFound();
            }
            var result = await _wishList.DeleteWishListById(userID, productID);
            if (string.IsNullOrEmpty(result))
            {
                return Ok(await _wishList.GetWishListAsync(userID));
            }
            return Ok(result);

        }
        [HttpDelete]
        public async Task<IActionResult> ClearWishList([FromHeader] string userID)
        {
            if (userID == null)
            {
                return NotFound();
            }
            var result = await _wishList.DeleteAllWishList(userID);
            if (string.IsNullOrEmpty(result))
            {
                return Ok("all products all deleted");
            }
            return Ok(result);

        }
    }
}
