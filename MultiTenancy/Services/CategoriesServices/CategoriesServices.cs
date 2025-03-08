
using Microsoft.EntityFrameworkCore;

namespace MultiTenancy.Services.CategoriesServices
{
    public class CategoriesServices : ICategoriesServices
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment hosting;

        public CategoriesServices(ApplicationDbContext context, IWebHostEnvironment hosting)
        {
            _context = context;
            this.hosting = hosting;

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
                    category.image = $"https://localhost:7060/CategoryImages/{fileName}";
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
                throw new Exception("some thing error");

            }
        }

        public async Task<string> DeleteCategory(int id)
        {
            var Category = await _context.Categories.FindAsync(id);
            if (Category != null)
            {
                _context.Categories.Remove(Category);
                await _context.SaveChangesAsync();
                return "the category has been deleted";
            }
            return "Cant find this category";
        }

        public async Task<IReadOnlyList<CategoryModel>> GetAllAsync()
        {
            return await _context.Categories.ToListAsync();
        }

        public async Task<CategoryModel?> GetByIdAsync(int id)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category == null)
            {
                return null;
            }
            return category;
        }
    }
}
