using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MultiTenancy.Dtos;
using MultiTenancy.Services.BrandServices;

namespace MultiTenancy.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BrandController : ControllerBase
    {
        private readonly IBrandServices _brandService;
        public BrandController(IBrandServices brandService)
        {
            _brandService = brandService;
        }

        [HttpGet]
        public async Task<IActionResult> GetBrands()
        {
            var brand = await _brandService.GetAllAsync();
            return Ok(brand);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetBrand(int id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var brandModel = await _brandService.GetByIdAsync(id);
            return Ok(brandModel);
        }

        [HttpPost]
        public async Task<IActionResult> CreateCategory(CreateBrandDto dto)
        {
            BrandModel brand = new()
            {
                Name = dto.Name,
                image = dto.image
            };
            var createdBrand = await _brandService.CreatedAsync(brand);
            return Ok(createdBrand);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            if (id != null)
            {
                var message = await _brandService.DeleteBrand(id);
                return Ok(message);
            }
            return NotFound();
        }
    }
}
