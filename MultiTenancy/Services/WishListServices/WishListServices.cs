
using Microsoft.EntityFrameworkCore;

namespace MultiTenancy.Services.WishListServices
{
    public class WishListServices : IWishListServices
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<AppUser> _userManager;
        private readonly IProductService _productService;
        public WishListServices(ApplicationDbContext context, UserManager<AppUser> userManager, IProductService productService)
        {
            _context = context;
            _userManager = userManager;
            _productService = productService;
        }

        public async Task<string> AddToWithListAsync(string userId, int productId)
        {
            if (!await UserExistsAsync(userId))
            {
                return "user is not found";
            }
            var product = await _productService.GetByIdAsync(productId);
            if (product == null)
            {
                return "the product is not found";
            }
            var isinWishList = await _context.WishLists.AnyAsync(i => i.productID == productId);
            if (isinWishList == true)
            {
                return "the product is already existed in wishlist";
            }
            WishListModel model = new()
            {
                productID = product.Id,
                userID = userId,
                NumSold = product.NumSold,
                images = product.images,
                ratingsQuantity = product.ratingsQuantity,
                title = product.title,
                description = product.description,
                quantity = product.quantity,
                price = product.price,
                imageCover = product.imageCover,
                CategoryID = product.CategoryID,
                BrandID = product.BrandID
            };
            _context.WishLists.Add(model);
            await _context.SaveChangesAsync();

            return "";
        }

        public async Task<string> DeleteAllWishList(string userId)
        {
            if (!await UserExistsAsync(userId))
            {
                return "user is not found";
            }
            var allWishList = await _context.WishLists.Where(i => i.userID == userId).ToListAsync();
            if (allWishList == null || !allWishList.Any())
            {
                return "no items found in wishlist";
            }
            _context.WishLists.RemoveRange(allWishList);
            await _context.SaveChangesAsync();
            return "";
        }

        public async Task<string> DeleteWishListById(string userId, int productId)
        {
            if (!await UserExistsAsync(userId))
            {
                return "user is not found";
            }
            var product = await _context.WishLists.FindAsync(productId);
            if (product == null)
            {
                return "the product is not found in wishList";
            }
            _context.WishLists.Remove(product);
            await _context.SaveChangesAsync();
            return "";

        }

        public async Task<IReadOnlyList<WishListModel>> GetWishListAsync(string userId)
        {
            if (!await UserExistsAsync(userId))
            {
                return null;
            }
            return await _context.WishLists.Include(p => p.category).Include(p => p.Brand).Where(i => i.userID == userId).ToListAsync();
        }
        private async Task<bool> UserExistsAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return false;
            }
            return true;
        }
    }
}
