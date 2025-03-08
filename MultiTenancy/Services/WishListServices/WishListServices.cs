
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

        public async Task<WishListModel> AddToWishlistAsync(string userId, int productId)
        {
            if (!await UserExistsAsync(userId))
            {
                throw new Exception("User not found");
            }
            var pro = await _context.Products.FirstOrDefaultAsync(w => w.Id == productId);

            if (pro == null)
            {
                throw new Exception("Product not found");
            }

            var wishlist = await _context.WishLists.FirstOrDefaultAsync(w => w.UserId == userId);
            if (wishlist == null)
            {
                WishListModel model = new()
                {
                    UserId = userId,
                    ProductsIDs = new List<int> { productId}
                };
                _context.WishLists.Add(model);
                await _context.SaveChangesAsync();
                return model;
            }
            else
            {
                if (wishlist.ProductsIDs.Contains(productId))
                {
                    throw new Exception("Product already added");
                }
                wishlist.ProductsIDs.Add(productId);
                _context.WishLists.Update(wishlist);
                await _context.SaveChangesAsync();
                return wishlist;
            }
        }


        public async Task<WishListModel> GetWishlistAsync(string userId)
        {
            if(!await UserExistsAsync(userId))
            {
                throw new Exception("User not found");
            }
            var wishlist = await _context.WishLists.FirstOrDefaultAsync(w => w.UserId == userId);
            return wishlist ?? new WishListModel { UserId = userId, ProductsIDs = new List<int>() };
        }

        public async Task<bool> ClearWishlistAsync(string userId)
        {
            if (!await UserExistsAsync(userId))
            {
                throw new Exception("User not found");
            }

            var wishlist = await _context.WishLists.FirstOrDefaultAsync(w => w.UserId == userId);
            if (wishlist == null)
            {
                throw new Exception("Wishlist not found");
            }

            _context.WishLists.Remove(wishlist);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<WishListModel> RemoveFromWishlistAsync(string userId, int productId)
        {
            if (!await UserExistsAsync(userId))
            {
                throw new Exception("User not found");
            }

            var wishpro = await _context.WishLists.FirstOrDefaultAsync(w => w.UserId == userId);
            if (!wishpro.ProductsIDs.Contains(productId))
            {
                throw new Exception("can not find this product in your wishList");
            }
            wishpro.ProductsIDs.Remove(productId);
            _context.WishLists.Update(wishpro);
            await _context.SaveChangesAsync();
            return wishpro;
        }

        public async Task<List<ProductModel>> GetAllProductinWishList(string userId)
        {
            if (!await UserExistsAsync(userId))
            {
                throw new Exception("User not found");
            }

            var wishList = await GetWishlistAsync(userId);
            if (wishList == null || !wishList.ProductsIDs.Any())
            {
                return new List<ProductModel>();
            }

            var products = await _context.Products.Include(b => b.Brand).Include(c => c.category)
                                         .Where(p => wishList.ProductsIDs.Contains(p.Id))
                                         .ToListAsync();

            return products;
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
