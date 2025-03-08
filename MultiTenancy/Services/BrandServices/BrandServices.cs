
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;

namespace MultiTenancy.Services.BrandServices
{
    public class BrandServices : IBrandServices
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment hosting;

        public BrandServices(ApplicationDbContext context, IWebHostEnvironment hosting)
        {
            _context = context;
            this.hosting = hosting;

        }
        public async Task<BrandModel> CreatedAsync(BrandModel brand)
        {
            string imagePath = "";
            try
            {

                if (brand.ImageFiles != null)
                {
                    string ImageFolder = Path.Combine(hosting.WebRootPath, "BrandImages");

                    string fileExtension = Path.GetExtension(brand.ImageFiles.FileName);
                    string fileName = Guid.NewGuid().ToString() + fileExtension;
                    imagePath = Path.Combine(ImageFolder, fileName);

                    using (var stream = new FileStream(imagePath, FileMode.Create))
                    {
                        brand.ImageFiles.CopyTo(stream);
                    }
                    // Store the full URL instead of just the file name
                    brand.image = $"https://localhost:7060/BrandImages/{fileName}";
                }
                else
                {
                    throw new Exception("Must add cover Image");
                }


                _context.Brands.Add(brand);
                await _context.SaveChangesAsync();
                return brand;
            }
            catch (Exception)
            {
                File.Delete(imagePath!);
                throw new Exception("some thing error");

            }
        }
        public async Task<string> DeleteBrand(int id)
        {
            var brand = await _context.Brands.FindAsync(id);
            if (brand != null)
            {
                _context.Brands.Remove(brand);
                await _context.SaveChangesAsync();
                File.Delete(brand.image!);
                return "the brand has been deleted";
            }
            return "Cant find this brand";
        }

        public async Task<IReadOnlyList<BrandModel>> GetAllAsync()
        {
            return await _context.Brands.AsNoTracking().ToListAsync();
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
