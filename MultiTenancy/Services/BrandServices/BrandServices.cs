
using Microsoft.EntityFrameworkCore;

namespace MultiTenancy.Services.BrandServices
{
    public class BrandServices : IBrandServices
    {
        private readonly ApplicationDbContext _context;
        public BrandServices(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<BrandModel> CreatedAsync(BrandModel brand)
        {
            _context.Brands.Add(brand);
            await _context.SaveChangesAsync();
            return brand;
        }

        public async Task<string> DeleteBrand(int id)
        {
            var brand = await _context.Brands.FindAsync(id);
            if (brand != null)
            {
                _context.Brands.Remove(brand);
                await _context.SaveChangesAsync();
                return "the brand has been deleted";
            }
            return "Cant find this brand";
        }

        public async Task<IReadOnlyList<BrandModel>> GetAllAsync()
        {
            return await _context.Brands.ToListAsync();
        }

        public async Task<BrandModel?> GetByIdAsync(int id)
        {
            var brand = await _context.Brands.FindAsync(id);
            if (brand == null)
            {
                return null;
            }
            return brand;
        }
    }
}
