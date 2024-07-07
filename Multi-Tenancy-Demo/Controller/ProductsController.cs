using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Multi_Tenancy_Demo.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _productService;
        public ProductsController(IProductService productService)
        {
            _productService = productService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllAsync()
        {
            var products = await _productService.GetAllAsync();
            return Ok(products);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetAsync(int id)
        {
            var product = await _productService.GetByIdAsync(id);

            if (product == null)
            {
                return NotFound();
            }

            return Ok(product);
        }

        [HttpPost]
        public async Task<IActionResult> CreateAsync(CreateProductDto dto)
        {
            Product product = new Product
            {
                Name = dto.Name,
                Description = dto.Description,
                Rate = dto.Rate
            };

            var createdProduct = await _productService.CreatedAsync(product);
            return Ok(createdProduct);
        }
    }
}
