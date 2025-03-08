using Microsoft.AspNetCore.Mvc;

namespace MultiTenancy.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoriesServices _categoriesServices;

        public CategoryController(ICategoriesServices categoriesServices)
        {
            _categoriesServices = categoriesServices;
        }

        [HttpGet]
        public async Task<IActionResult> GetCategories()
        {
            var Categories = await _categoriesServices.GetAllAsync();
            return Ok(Categories);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetCategory(int id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var categoryModel = await _categoriesServices.GetByIdAsync(id);
            return Ok(categoryModel);
        }

        [HttpPost]
        public async Task<IActionResult> CreateCategory([FromForm]CreateCategoryDto dto)
        {
            CategoryModel category = new()
            {
                Name = dto.Name,
                ImageFiles = dto.ImageFiles
            };
            var createdProduct = await _categoriesServices.CreatedAsync(category);
            return Ok(createdProduct);

        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            if (id != null)
            {
                var message = await _categoriesServices.DeleteCategory(id);
                return Ok(message);
            }
            return NotFound();
        }
    }
}
