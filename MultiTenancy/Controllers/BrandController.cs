using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MultiTenancy.Services.TrafficServices;

namespace MultiTenancy.Controllers
{
    //[Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class BrandController : ControllerBase
    {
        private readonly ITrafficServices _trafficServices;
        private readonly IBrandServices _brandService;
        public BrandController(IBrandServices brandService, ITrafficServices trafficServices)
        {
            _brandService = brandService;
            _trafficServices = trafficServices;
        }

        [HttpGet]
        public async Task<IActionResult> GetBrands()
        {

            try
            {
                var brands = await _brandService.GetAllAsync();
                return Ok(brands);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "some thing error get Brands try again later!" });
            }

        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetBrand(int id)
        {
            await _trafficServices.AddReqCountAsync();

            try
            {
                if (id == null)
                {
                    return NotFound();
                }
                var brandModel = await _brandService.GetByIdAsync(id);
                return Ok(brandModel);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "some thing error get Brand try again later!" });
            }
            
        }



        [Authorize]
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> CreateBrand([FromForm]BrandDto dto)
        {
            await _trafficServices.AddReqCountAsync();
            await _trafficServices.AddBrandCountAsync();

            try
            {
                BrandModel brand = new()
                {
                    Name = dto.Name,
                    ImageFiles = dto.ImageFile
                };
                var createdBrand = await _brandService.CreatedAsync(brand);
                return Ok(createdBrand);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "some thing error create Brand try again later!" });
            }

           
        }

        [HttpDelete("{id}")]
        [Authorize]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteBrand(int id)
        {
            await _trafficServices.AddReqCountAsync();
            await _trafficServices.DecreaseBrandCountAsync();

            try
            {
                if (id != null)
                {
                    var message = await _brandService.DeleteBrand(id);
                    return Ok(message);
                }
                return NotFound();
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "some thing error delete Brand try again later!" });
            }

        }

        [HttpPut("{id}")]
        [Authorize]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> EditBrand(int id, [FromForm] BrandDto updateDto)
        {
            await _trafficServices.AddReqCountAsync();

            try
            {
                // Map DTO to BrandModel
                var updatedBrand = new BrandModel
                {
                    Name = updateDto.Name,
                    ImageFiles = updateDto.ImageFile
                };

                // Call the service
                var result = await _brandService.EditBrandAsync(id, updatedBrand);

                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "some thing error when Edit Brand try again later!" });
            }
        }
    }
}
