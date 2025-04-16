﻿
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;

namespace MultiTenancy.Services.BrandServices
{
    public class BrandServices : IBrandServices
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment hosting;
        private readonly string _imageStoragePath;
        private readonly IProductService _productService;


        public BrandServices(ApplicationDbContext context, IWebHostEnvironment hosting, IProductService productService)
        {
            _context = context;
            this.hosting = hosting;
            _productService = productService;

            _imageStoragePath = Path.Combine(hosting.WebRootPath, "BrandImages");
            Directory.CreateDirectory(_imageStoragePath);

        }
        public async Task<BrandModel> CreatedAsync(BrandModel brand)
        {
            string imagePath = "";
            try
            {

                if (brand.ImageFiles != null)
                {
                    if (string.IsNullOrEmpty(hosting.WebRootPath))
                    {
                        hosting.WebRootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
                    }

                    // Ensure wwwroot folder exists
                    if (!Directory.Exists(hosting.WebRootPath))
                    {
                        Directory.CreateDirectory(hosting.WebRootPath);
                    }

                    string ImageFolder = Path.Combine(hosting.WebRootPath, "BrandImages");

                    string fileExtension = Path.GetExtension(brand.ImageFiles.FileName);
                    string fileName = Guid.NewGuid().ToString() + fileExtension;
                    imagePath = Path.Combine(ImageFolder, fileName);

                    using (var stream = new FileStream(imagePath, FileMode.Create))
                    {
                        brand.ImageFiles.CopyTo(stream);
                    }
                    // Store the full URL instead of just the file name
                    brand.Image = $"https://localhost:7060/BrandImages/{fileName}";
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
                var products = await _context.Products.Where(p => p.BrandID == id).ToListAsync();

                foreach(var pro in products)
                {
                    await _productService.DeleteProduct(pro.Id);

                }

                _context.Brands.Remove(brand);
                await _context.SaveChangesAsync();

                File.Delete(Path.Combine(_imageStoragePath, Path.GetFileName(brand.Image!)));

                return "The brand and its related products have been deleted";
            }
            return "Can't find this brand";
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

        public async Task<BrandModel> EditBrandAsync(int id, BrandModel updatedBrand)
        {


            // Find the existing brand
            var brand = await _context.Brands
                .FirstOrDefaultAsync(b => b.Id == id);

            if (brand == null)
            {
                throw new Exception("Brand not found or you do not have access to it.");
            }

            // Update name if provided
            if (!string.IsNullOrWhiteSpace(updatedBrand.Name))
            {
                brand.Name = updatedBrand.Name;
            }

            // Handle image upload if provided
            if (updatedBrand.ImageFiles != null && updatedBrand.ImageFiles.Length > 0)
            {
                // Validate file type
                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
                var extension = Path.GetExtension(updatedBrand.ImageFiles.FileName).ToLowerInvariant();
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
                    await updatedBrand.ImageFiles.CopyToAsync(stream);
                }

                // Delete old image if it exists
                if (!string.IsNullOrEmpty(brand.Image))
                {
                    var oldImagePath = Path.Combine(_imageStoragePath, Path.GetFileName(brand.Image));
                    if (File.Exists(oldImagePath))
                    {
                        File.Delete(oldImagePath);
                    }
                }

                // Update image path
                brand.Image = $"/BrandImages/{fileName}"; // Updated path
            }

            // Save changes
            _context.Brands.Update(brand);
            await _context.SaveChangesAsync();

            return brand;
        }
    }
}
