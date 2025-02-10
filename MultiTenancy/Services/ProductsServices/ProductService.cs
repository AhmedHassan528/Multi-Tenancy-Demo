using Microsoft.EntityFrameworkCore;

namespace MultiTenancy.Services.ProductsServices;

public class ProductService : IProductService
{
    private readonly ApplicationDbContext _context;

    public ProductService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<ProductModel> CreatedAsync(ProductModel product)
    {
        _context.Products.Add(product);

        await _context.SaveChangesAsync();

        return product;
    }

    public async Task<string> DeleteProduct(int id)
    {
        var product = await _context.Products.FindAsync(id);
        if (product is not null)
        {
            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
            return "the Product has been deleted";
        }
        else
        {
            return "Cant find this product";
        }

    }

    public async Task<IReadOnlyList<ProductModel>> GetAllAsync()
    {
        return await _context.Products.Include(p => p.category).Include(p => p.Brand).ToListAsync();
    }

    public async Task<ProductModel?> GetByIdAsync(int id)
    {
        return await _context.Products.FindAsync(id);
    }
}