
using Microsoft.EntityFrameworkCore;

namespace MultiTenancy.Services.CategoriesServices
{
    public class CategoriesServices : ICategoriesServices
    {
        private readonly ApplicationDbContext _context;
        public CategoriesServices(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<CategoryModel> CreatedAsync(CategoryModel category)
        {
            _context.Categories.Add(category);
            await _context.SaveChangesAsync();
            return category;
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
