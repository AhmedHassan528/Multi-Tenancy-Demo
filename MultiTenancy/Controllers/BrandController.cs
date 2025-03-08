using Microsoft.AspNetCore.Mvc;

namespace MultiTenancy.Controllers
{
    //[Authorize]
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
        public async Task<IActionResult> CreateBrand([FromForm]CreateBrandDto dto)
        {
            BrandModel brand = new()
            {
                Name = dto.Name,
                ImageFiles = dto.imageFile
            };
            var createdBrand = await _brandService.CreatedAsync(brand);
            return Ok(createdBrand);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBrand(int id)
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
