
using Microsoft.EntityFrameworkCore;

namespace MultiTenancy.Services.WishListServices
{
    public class WishListServices : IWishListServices
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<AppUser> _userManager;
        public WishListServices(ApplicationDbContext context, UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<WishListModel> AddToWishlistAsync(string userId, int productId)
        {
            try
            {
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
                        ProductsIDs = new List<int> { productId }
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

                    var product = await _context.Products.FirstOrDefaultAsync(p => p.Id == productId);
                    product!.LikeCount++;
                    _context.Products.Update(product);

                    await _context.SaveChangesAsync();
                    return wishlist;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error while adding product to wishlist");
            }

        }


        public async Task<WishListModel> GetWishlistAsync(string userId)
        {
            try
            {
                var wishlist = await _context.WishLists.FirstOrDefaultAsync(w => w.UserId == userId);
                return wishlist ?? new WishListModel { UserId = userId, ProductsIDs = new List<int>() };
            }
            catch (Exception)
            {
                throw new Exception("Error while getting wishlist ");
            }

        }

        public async Task<bool> ClearWishlistAsync(string userId)
        {

            try
            {
                var wishlist = await _context.WishLists.FirstOrDefaultAsync(w => w.UserId == userId);
                if (wishlist == null)
                {
                    throw new Exception("Wishlist not found");
                }

                _context.WishLists.Remove(wishlist);
                await _context.SaveChangesAsync();

                return true;
            }
            catch (Exception)
            {
                throw new Exception("Error while clearing wishlist");
            }

        }

        public async Task<WishListModel> RemoveFromWishlistAsync(string userId, int productId)
        {

            try
            {
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
            catch (Exception)
            {
                throw new Exception("Error while removing product from wishlist");
            }

        }

        public async Task<List<ProductModel>> GetAllProductinWishList(string userId)
        {
            try
            {


                var wishList = await GetWishlistAsync(userId);
                if (wishList == null || !wishList.ProductsIDs.Any())
                {
                    return new List<ProductModel>();
                }

                var products = await _context.Products.Include(b => b.Brand).Include(c => c.Category)
                                             .Where(p => wishList.ProductsIDs.Contains(p.Id))
                                             .ToListAsync();

                return products;
            }
            catch (Exception)
            {
                throw new Exception("Error while getting products from wishlist");
            }

        }

    }
}
