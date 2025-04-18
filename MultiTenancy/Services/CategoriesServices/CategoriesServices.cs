
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace MultiTenancy.Services.CategoriesServices
{
    public class CategoriesServices : ICategoriesServices
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment hosting;
        private readonly string _imageStoragePath;

        public CategoriesServices(ApplicationDbContext context, IWebHostEnvironment hosting)
        {
            _context = context;
            this.hosting = hosting;

            _imageStoragePath = Path.Combine(hosting.WebRootPath, "CategoryImages");
            Directory.CreateDirectory(_imageStoragePath);

        }

        public async Task<CategoryModel> CreatedAsync(CategoryModel category)
        {
            string imagePath = "";
            try
            {

                if (category.ImageFiles != null)
                {
                    string ImageFolder = Path.Combine(hosting.WebRootPath, "CategoryImages");

                    string fileExtension = Path.GetExtension(category.ImageFiles.FileName);
                    string fileName = Guid.NewGuid().ToString() + fileExtension;
                    imagePath = Path.Combine(ImageFolder, fileName);

                    using (var stream = new FileStream(imagePath, FileMode.Create))
                    {
                        category.ImageFiles.CopyTo(stream);
                    }
                    // Store the full URL instead of just the file name
                    category.Image = $"https://localhost:7060/CategoryImages/{fileName}";
                }
                else
                {
                    throw new Exception("Must add cover Image");
                }


                _context.Categories.Add(category);
                await _context.SaveChangesAsync();
                return category;
            }
            catch (Exception)
            {
                File.Delete(imagePath!);
                throw new Exception("Some thing error in category");

            }
        }

        public async Task<string> DeleteCategory(int id)
        {
            try
            {
                var category = await _context.Categories.FindAsync(id);
                if (category != null)
                {
                    var products = await _context.Products.Where(p => p.CategoryID == id).ToListAsync();

                    _context.Products.RemoveRange(products);

                    _context.Categories.Remove(category);

                    await _context.SaveChangesAsync();
                    return "The category and its related products have been deleted";
                }
                throw new Exception( "Can't find this category");
            }
            catch (Exception)
            {
                throw new Exception("some thing error when deleting Category try again later!");
            }

        }

        public async Task<IReadOnlyList<CategoryModel>> GetAllAsync()
        {
            try
            {
                return await _context.Categories.ToListAsync();

            }
            catch (Exception)
            {
                throw new Exception("Some thing error in category");
            }

        }

        public async Task<CategoryModel?> GetByIdAsync(int id)
        {
            try
            {
                var category = await _context.Categories.FindAsync(id);
                if (category == null)
                {
                    throw new Exception("can not find category!!");
                }
                return category;
            }
            catch (Exception)
            {
                throw new Exception("Some thing error in category");
            }

        }


        public async Task<CategoryModel> EditCategoryAsync(int id, CategoryModel updatedCategory)
        {

            try
            {
                // Find the existing brand
                var category = await _context.Categories
                    .FirstOrDefaultAsync(b => b.Id == id);

                if (category == null)
                {
                    throw new Exception("Brand not found or you do not have access to it.");
                }

                // Update name if provided
                if (!string.IsNullOrWhiteSpace(updatedCategory.Name))
                {
                    category.Name = updatedCategory.Name;
                }

                // Handle image upload if provided
                if (updatedCategory.ImageFiles != null && updatedCategory.ImageFiles.Length > 0)
                {
                    // Validate file type
                    var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
                    var extension = Path.GetExtension(updatedCategory.ImageFiles.FileName).ToLowerInvariant();
                    if (!allowedExtensions.Contains(extension))
                    {
                        throw new Exception("Invalid image format. Only JPG, JPEG, PNG, and GIF are allowed.");
                    }

                    // Generate unique file name
                    var fileName = $"{Guid.NewGuid()}{extension}";
                    var filePath = Path.Combine(_imageStoragePath, fileName);

                    // Save the file
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await updatedCategory.ImageFiles.CopyToAsync(stream);
                    }

                    // Delete old image if it exists
                    if (!string.IsNullOrEmpty(category.Image))
                    {
                        var oldImagePath = Path.Combine(_imageStoragePath, Path.GetFileName(category.Image));
                        if (File.Exists(oldImagePath))
                        {
                            File.Delete(oldImagePath);
                        }
                    }

                    // Update image path
                    category.Image = $"/BrandImages/{fileName}"; // Updated path
                }

                // Save changes
                _context.Categories.Update(category);
                await _context.SaveChangesAsync();

                return category;
            }
            catch (Exception)
            {
                throw new Exception("Some thing error in category");
            }

        }
    }
}
