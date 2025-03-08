
using Microsoft.EntityFrameworkCore;

namespace MultiTenancy.Services.CartServices
{
    public class CartServices : ICartServices
    {
        private readonly ApplicationDbContext _context;
        public CartServices(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<CartModel> AddItemToCartAsync(string userId, int productId, int quantity)
        {
            var product = await _context.Products.FindAsync(productId);
            if (product == null)
                throw new Exception("Product not found");

            var cart = await _context.Carts
                .Include(c => c.Products)
                .FirstOrDefaultAsync(c => c.CartOwner == userId);

            if (cart == null)
            {
                cart = new CartModel
                {
                    CartOwner = userId,
                    Products = new List<CartItemModel>(),
                    TotalCartPrice = 0
                };
                _context.Carts.Add(cart);
            }

            var cartItem = cart.Products.FirstOrDefault(ci => ci.ProductId == productId);

            if (cartItem != null)
            {
                cartItem.Count += quantity;
            }
            else
            {
                cart.Products.Add(new CartItemModel
                {
                    ProductId = productId,
                    Count = quantity,
                    Price = product.price
                });
            }

            cart.TotalCartPrice = cart.Products.Sum(ci => ci.Count * ci.Price);
            cart.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return cart;
        }

        public async Task<CartModel> RemoveItemFromCartAsync(string userId, int productId)
        {
            var cart = await _context.Carts
                .Include(c => c.Products)
                .FirstOrDefaultAsync(c => c.CartOwner == userId);

            if (cart == null)
                throw new Exception("Cart not found");

            var cartItem = cart.Products.FirstOrDefault(ci => ci.ProductId == productId);
            if (cartItem == null)
                throw new Exception("Product not found in cart");

            cart.Products.Remove(cartItem);
            cart.TotalCartPrice = cart.Products.Sum(ci => ci.Count * ci.Price);
            cart.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return cart;
        }

        public async Task<CartModel> GetUserCartAsync(string userId)
        {
            var cart = await _context.Carts
                .AsNoTracking()
                .Include(c => c.Products)
                    .ThenInclude(ci => ci.Product)
                        .ThenInclude(p => p.category)
                .Include(c => c.Products)
                    .ThenInclude(ci => ci.Product)
                        .ThenInclude(p => p.Brand)
                .FirstOrDefaultAsync(c => c.CartOwner == userId);

            if (cart == null)
                throw new Exception("Cart not found");

            return cart;


        }
        public async Task<CartModel> IncreaseItemCountAsync(string userId, int productId)
        {
            var cart = await _context.Carts
                .Include(c => c.Products)
                .FirstOrDefaultAsync(c => c.CartOwner == userId);

            if (cart == null)
                throw new Exception("Cart not found");

            var cartItem = cart.Products.FirstOrDefault(ci => ci.ProductId == productId);
            if (cartItem == null)
                throw new Exception("Product not found in cart");

            cartItem.Count++; // Increase count
            cart.TotalCartPrice = cart.Products.Sum(ci => ci.Count * ci.Price);
            cart.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return cart;
        }

        public async Task<CartModel> DecreaseItemCountAsync(string userId, int productId)
        {
            var cart = await _context.Carts
                .Include(c => c.Products)
                .FirstOrDefaultAsync(c => c.CartOwner == userId);

            if (cart == null)
                throw new Exception("Cart not found");

            var cartItem = cart.Products.FirstOrDefault(ci => ci.ProductId == productId);
            if (cartItem == null)
                throw new Exception("Product not found in cart");

            if (cartItem.Count > 1)
            {
                cartItem.Count--;
            }
            else
            {
                cart.Products.Remove(cartItem);
            }

            cart.TotalCartPrice = cart.Products.Sum(ci => ci.Count * ci.Price);
            cart.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return cart;
        }

        public async Task<CartModel> ClearCartAsync(string userId)
        {
            var cart = await _context.Carts
                .Include(c => c.Products)
                .FirstOrDefaultAsync(c => c.CartOwner == userId);

            if (cart == null)
                throw new Exception("Cart not found");

            cart.Products.Clear(); // Remove all items from cart
            cart.TotalCartPrice = 0;
            cart.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return cart;
        }

    }
}
