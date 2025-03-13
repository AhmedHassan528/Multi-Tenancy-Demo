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

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteProduct(int id)
    {
        if (id == 0)
        {
            return NotFound();
        }
        try
        {
            var result = await _productService.DeleteProduct(id);
            return Ok(result);

        }
        catch (Exception ex)
        {
            return BadRequest($"error: {ex.Message}");

        }

    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateProduct(int id, [FromForm] ProductModel productModel)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            var updatedProduct = await _productService.UpdateProductAsync(
                id,
                productModel,
                productModel.ImageCoverFile,
                productModel.ImageFiles ?? new List<IFormFile>()
            );
            //return Ok(updatedProduct);
            return Ok(new { message = "Product Updated successfully!", data = updatedProduct });

        }
        catch (ArgumentException ex)
        {
            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            return BadRequest($"Internal server error: {ex.Message}");
        }
    }
}