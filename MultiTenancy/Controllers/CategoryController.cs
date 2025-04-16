using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MultiTenancy.Services.TrafficServices;

namespace MultiTenancy.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoriesServices _categoriesServices;
        private readonly ITrafficServices _trafficServices;

        public CategoryController(ICategoriesServices categoriesServices, ITrafficServices trafficServices)
        {
            _categoriesServices = categoriesServices;
            _trafficServices = trafficServices;
        }

        [HttpGet]
        public async Task<IActionResult> GetCategories()
        {
            try
            {
                await _trafficServices.AddReqCountAsync();

                var Categories = await _categoriesServices.GetAllAsync();
                return Ok(Categories);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "some thing error when getting Categories try again later!" });
            }
            
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetCategory(int id)
        {
            await _trafficServices.AddReqCountAsync();

            try
            {
                if (id == null)
                {
                    return NotFound();
                }
                var categoryModel = await _categoriesServices.GetByIdAsync(id);
                return Ok(categoryModel);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "some thing error when getting Category try again later!" });
            }

        }


        [HttpPost]
        [Authorize(Roles = "Admin")]
        [Authorize]
        public async Task<IActionResult> CreateCategory([FromForm]CreateCategoryDto dto)
        {
            await _trafficServices.AddReqCountAsync();
            await _trafficServices.AddCategoryCountAsync();

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
                return BadRequest(new { message = "some thing error when creating Category try again later!" });
            }
            

        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        [Authorize]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            await _trafficServices.AddReqCountAsync();
            await _trafficServices.DecreaseCategoryCountAsync();

            try
            {
                if (id != null)
                {
                    var message = await _categoriesServices.DeleteCategory(id);
                    return Ok(message);
                }
                return BadRequest(new { message = "some thing error when deleting Category try again later!" });

            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "some thing error when deleting Category try again later!" });
            }
        }


        [HttpPut("{id}")]
        [Authorize]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> EditCategory(int id, [FromForm] CategoryDto updateDto)
        {
            await _trafficServices.AddReqCountAsync();

            try
            {
                var updatedCategory = new CategoryModel
                {
                    Name = updateDto.Name,
                    ImageFiles = updateDto.ImageFiles
                };

                // Call the service
                var result = await _categoriesServices.EditCategoryAsync(id, updatedCategory);

                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "some thing error when editing Category try again later!" });
            }
        }
    }
}
