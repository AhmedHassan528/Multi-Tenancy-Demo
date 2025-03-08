using Microsoft.EntityFrameworkCore;

namespace MultiTenancy.Services.ProductsServices;

public class ProductService : IProductService
{
    private readonly ApplicationDbContext _context;
    private readonly IWebHostEnvironment hosting;

    public ProductService(ApplicationDbContext context, IWebHostEnvironment hosting)
    {
        _context = context;
        this.hosting = hosting;
    }

    public async Task<ProductModel> CreatedAsync(ProductModel product)
    {
        List<string> savedFilePaths = new List<string>();

        try
        {
            if (product.ImageCoverFile != null)
            {
                string ImageFolder = Path.Combine(hosting.WebRootPath, "ProductCoverImages");

                string fileExtension = Path.GetExtension(product.ImageCoverFile.FileName);
                string fileName = Guid.NewGuid().ToString() + fileExtension;
                string imagePath = Path.Combine(ImageFolder, fileName);

                using (var stream = new FileStream(imagePath, FileMode.Create))
                {
                    product.ImageCoverFile.CopyTo(stream);
                }

                // Store the full URL instead of just the file name
                product.imageCover = $"https://localhost:7060/ProductCoverImages/{fileName}";
            }
            else
            {
                throw new Exception("Must add cover Image");
            }

            string imageFolder = Path.Combine(hosting.WebRootPath, "ProductImages");

            List<string> imageUrls = new List<string>();

            if (product.ImageFiles != null && product.ImageFiles.Count > 0)
            {
                foreach (var file in product.ImageFiles)
                {
                    string fileExtension = Path.GetExtension(file.FileName);
                    string fileName = $"{Guid.NewGuid()}{fileExtension}";
                    string imagePath = Path.Combine(imageFolder, fileName);

                    using (var stream = new FileStream(imagePath, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }

                    savedFilePaths.Add(imagePath);
                    imageUrls.Add($"https://localhost:7060/ProductImages/{fileName}");
                }
            }
            product.Images = imageUrls;


            var category = await _context.Categories.FindAsync(product.CategoryID);
            var brand = await _context.Brands.FindAsync(product.BrandID);

            if (category == null || brand == null)
            {
                throw new Exception("Invalid Category or Brand");
            }

            product.category = category;
            product.Brand = brand;

            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            return product;
        }
        catch (Exception ex)
        {
            foreach (var filePath in savedFilePaths)
            {
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }
            }
            throw;
        }
    }

    public async Task<string> DeleteProduct(int id)
    {
        var product = await _context.Products.FindAsync(id);
        if (product is null)
        {
            throw new Exception("Cannot find this product");
        }

        // Delete associated images from the server
        if (product.Images != null && product.Images.Count > 0)
        {
            foreach (var imageUrl in product.Images)
            {
                string fileName = Path.GetFileName(imageUrl);
                string imagePath = Path.Combine(hosting.WebRootPath, "ProductImages", fileName);

                if (File.Exists(imagePath))
                {
                    File.Delete(imagePath);
                }
            }
        }

        if (product.imageCover != null)
        {
            string fileName = Path.GetFileName(product.imageCover);
            string imagePath = Path.Combine(hosting.WebRootPath, "ProductCoverImages", fileName);
            if (File.Exists(imagePath))
            {
                File.Delete(imagePath);
            }
        }

        _context.Products.Remove(product);
        await _context.SaveChangesAsync();

        return "The product and its images have been deleted successfully";
    }

    public async Task<IReadOnlyList<ProductModel>> GetAllAsync()
    {
        var pro = await _context.Products.AsNoTracking().Include(p => p.category).Include(p => p.Brand).ToListAsync();
        return pro;
    }

    public async Task<ProductModel?> GetByIdAsync(int id)
    {
        var product = await _context.Products
                .Include(p => p.category)
                .Include(p => p.Brand)
                .FirstOrDefaultAsync(p => p.Id == id);

        if (product == null)
            return null;

        return product;
    }
}
