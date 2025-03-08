using Microsoft.AspNetCore.Mvc;

namespace MultiTenancy.Controllers;

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

        return product is null ? NotFound() : Ok(product);
    }

    [HttpPost]
    public async Task<IActionResult> CreatedAsync([FromForm] CreateProductDto dto)
    {
        ProductModel product = new()
        {
            NumSold = dto.NumSold,
            ImageFiles = dto.ImageFiles,
            ratingsQuantity = dto.ratingsQuantity,
            title = dto.title,
            description = dto.description,
            price = dto.price,
            ImageCoverFile = dto.ImageCoverFile,
            CategoryID = dto.CategoryID,
            BrandID = dto.BrandID
        };

        var createdProduct = await _productService.CreatedAsync(product);

        return Ok(new { message = "Product created successfully!", data = createdProduct });
    }

    [HttpDelete]
    public async Task<IActionResult> DeleteProduct(int id)
    {
        if (id == null)
        {
            return NotFound();
        }
        var result = await _productService.DeleteProduct(id);

        return Ok(result);
    }
}