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
        private readonly IAuthService _authService;

        public BrandController(IBrandServices brandService, ITrafficServices trafficServices, IAuthService authService)
        {
            _brandService = brandService;
            _trafficServices = trafficServices;
            _authService = authService;
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
                return BadRequest(new { message = ex.Message });
            }

        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetBrand(int id)
        {
            await _trafficServices.AddReqCountAsync();

            try
            {
                if (id == 0 )
                {
                    return NotFound();
                }
                var brandModel = await _brandService.GetByIdAsync(id);
                return Ok(brandModel);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
            
        }



        [Authorize]
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> CreateBrand([FromForm]BrandDto dto)
        {
            await _trafficServices.AddReqCountAsync();
            await _trafficServices.AddBrandCountAsync();


            var userID = User.FindFirst("uid")?.Value;
            if (userID == null || !await _authService.isAdmin(userID))
            {
                return NotFound(new { message = "Error: User not found. \nPlease ensure you have entered the correct username or email, or register for an account.", StatusCode = 401 });
            }

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
                return StatusCode(500, new { message = ex.Message });
            }

           
        }

        [HttpDelete("{id}")]
        [Authorize]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteBrand(int id)
        {
            await _trafficServices.AddReqCountAsync();
            await _trafficServices.DecreaseBrandCountAsync();

            var userID = User.FindFirst("uid")?.Value;
            if (userID == null || !await _authService.isAdmin(userID))
            {
                return NotFound(new { message = "Error: User not found. \nPlease ensure you have entered the correct username or email, or register for an account.", StatusCode = 401 });
            }


            try
            {
                if (id != 0)
                {
                    var message = await _brandService.DeleteBrand(id);
                    return Ok(message);
                }
                return NotFound();
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }

        }

        [HttpPut("{id}")]
        [Authorize]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> EditBrand(int id, [FromForm] BrandDto updateDto)
        {
            await _trafficServices.AddReqCountAsync();

            var userID = User.FindFirst("uid")?.Value;
            if (userID == null || !await _authService.isAdmin(userID))
            {
                return BadRequest(new { message = "some thing error when get adresses", StatusCode = 400 });
            }

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
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
