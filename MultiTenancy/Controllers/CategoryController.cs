using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.DotNet.Scaffolding.Shared.Messaging;
using MultiTenancy.Services.TrafficServices;

namespace MultiTenancy.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoriesServices _categoriesServices;
        private readonly ITrafficServices _trafficServices;
        private readonly IAuthService _authService;


        public CategoryController(ICategoriesServices categoriesServices, ITrafficServices trafficServices, IAuthService authService)
        {
            _categoriesServices = categoriesServices;
            _trafficServices = trafficServices;
            _authService = authService;
        }

        [HttpGet]
        public async Task<IActionResult> GetCategories()
        {
            await _trafficServices.AddReqCountAsync();
            try
            {
                var Categories = await _categoriesServices.GetAllAsync();
                return Ok(Categories);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetCategory(int id)
        {
            await _trafficServices.AddReqCountAsync();

            try
            {
                if (id == 0)
                {
                    return NotFound(new {Message = "can not find category!!" });
                }
                var categoryModel = await _categoriesServices.GetByIdAsync(id);
                return Ok(categoryModel);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }

        }


        [HttpPost]
        [Authorize(Roles = "Admin")]
        [Authorize]
        public async Task<IActionResult> CreateCategory([FromForm]CreateCategoryDto dto)
        {
            await _trafficServices.AddReqCountAsync();
            await _trafficServices.AddCategoryCountAsync();

            var userID = User.FindFirst("uid")?.Value;
            if (userID == null || !await _authService.isAdmin(userID))
            {
                return NotFound(new { message = "Error: User not found. \nPlease ensure you have entered the correct username or email, or register for an account.", StatusCode = 401 });
            }

            try
            {
                CategoryModel category = new()
                {
                    Name = dto.Name,
                    ImageFiles = dto.ImageFiles
                };
                var createdProduct = await _categoriesServices.CreatedAsync(category);
                return Ok(createdProduct);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            

        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        [Authorize]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            await _trafficServices.AddReqCountAsync();
            await _trafficServices.DecreaseCategoryCountAsync();

            var userID = User.FindFirst("uid")?.Value;
            if (userID == null || !await _authService.isAdmin(userID))
            {
                return NotFound(new { message = "Error: User not found. \nPlease ensure you have entered the correct username or email, or register for an account.", StatusCode = 401 });
            }

            try
            {
                if (id == 0)
                {
                    var message = await _categoriesServices.DeleteCategory(id);
                    return Ok(message);
                }
                return BadRequest(new { message = "some thing error when deleting Category try again later!" });

            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }


        [HttpPut("{id}")]
        [Authorize]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> EditCategory(int id, [FromForm] CategoryDto updateDto)
        {
            await _trafficServices.AddReqCountAsync();

            var userID = User.FindFirst("uid")?.Value;
            if (userID == null || !await _authService.isAdmin(userID))
            {
                return NotFound(new { message = "Error: User not found. \nPlease ensure you have entered the correct username or email, or register for an account.", StatusCode = 401 });
            }

            try
            {
                var updatedCategory = new CategoryModel
                {
                    Name = updateDto.Name,
                    ImageFiles = updateDto.ImageFiles
                };

                var result = await _categoriesServices.EditCategoryAsync(id, updatedCategory);

                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
