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
                var coverFilePath = await SaveFileAsync(product.ImageCoverFile, "ProductCoverImages");
                product.imageCover = coverFilePath;
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
                    var filePath = await SaveFileAsync(file, "ProductImages");
                    imageUrls.Add(filePath);
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

        product.viewCount++;
        await _context.SaveChangesAsync();

        return product;
    }

    public async Task<ProductModel> UpdateProductAsync(int id, ProductModel productModel, IFormFile imageCoverFile, List<IFormFile> imageFiles)
    {
        if (productModel == null) throw new ArgumentNullException(nameof(productModel));

        var existingProduct = await _context.Products
            .Include(p => p.category)
            .Include(p => p.Brand)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (existingProduct == null)
        {
            throw new ArgumentException("Product not found", nameof(id));
        }

        // Update scalar properties
        existingProduct.NumSold = productModel.NumSold;
        existingProduct.ratingsQuantity = productModel.ratingsQuantity;
        existingProduct.title = productModel.title;
        existingProduct.description = productModel.description;
        existingProduct.price = productModel.price;
        existingProduct.viewCount = productModel.viewCount;
        existingProduct.CategoryID = productModel.CategoryID;
        existingProduct.BrandID = productModel.BrandID;

        // Handle ImageCoverFile
        if (imageCoverFile != null && imageCoverFile.Length > 0)
        {
            var coverFilePath = await SaveFileAsync(imageCoverFile, "ProductCoverImages");
            existingProduct.imageCover = coverFilePath;
        }

        // Handle ImageFiles
        if (imageFiles != null && imageFiles.Any())
        {
            var imagePaths = new List<string>();
            foreach (var file in imageFiles)
            {
                if (file.Length > 0)
                {
                    var filePath = await SaveFileAsync(file, "ProductImages");
                    imagePaths.Add(filePath);
                }
            }
            existingProduct.Images = imagePaths; 
        }

        _context.Entry(existingProduct).State = EntityState.Modified;
        await _context.SaveChangesAsync();

        return existingProduct;
    }

    private async Task<string> SaveFileAsync(IFormFile file, string folder)
    {
        if (file == null || file.Length == 0) throw new ArgumentException("File is empty", nameof(file));

        var uploadsFolder = Path.Combine(hosting.WebRootPath, folder);
        if (!Directory.Exists(uploadsFolder))
        {
            Directory.CreateDirectory(uploadsFolder);
        }

        var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
        var filePath = Path.Combine(uploadsFolder, fileName);

        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }
        var FinalPath = Path.Combine(folder, fileName).Replace("\\", "/");
        // Return relative path for storage
        return $"https://localhost:7060/{FinalPath}";
    }
}


