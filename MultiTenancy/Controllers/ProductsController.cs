using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MultiTenancy.Dtos;
using MultiTenancy.Services.TrafficServices;

namespace MultiTenancy.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ProductsController : ControllerBase
{
    private readonly IProductService _productService;
    private readonly ITrafficServices _trafficServices;


    public ProductsController(IProductService productService, ITrafficServices trafficServices)
    {
        _productService = productService;
        _trafficServices = trafficServices;
    }


    [HttpGet]
    public async Task<IActionResult> GetAllAsync()
    {
        await _trafficServices.AddReqCountAsync();

        var products = await _productService.GetAllAsync();
        var productDtos = products.Select(p => new ProductsDtos
        {
            Id = p.Id,
            Title = p.Title,
            Price = p.Price,
            RatingsQuantity = p.RatingsQuantity,
            ImageCover = p.ImageCover,
            CategoryName = p.Category?.Name,
            BrandName = p.Brand?.Name
        }).ToList();

        return Ok(productDtos);

    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetAsync(int id)
    {
        await _trafficServices.AddReqCountAsync();

        try
        {
            var product = await _productService.GetByIdAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            var productDto = new ProductsDtos
            {
                Id = product.Id,
                Title = product.Title,
                Description = product.Description,
                Price = product.Price,
                RatingsQuantity = product.RatingsQuantity,
                Images = product.Images,
                CategoryID = product.CategoryID,
                CategoryName = product.Category?.Name,
                BrandName = product.Brand?.Name,
                BrandID = product.BrandID
            };

            return Ok(productDto);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = "some thing error when getting products try again later!", StatusCode = 400 });

        }

    }


    [HttpPost]
    public async Task<IActionResult> CreatedAsync([FromForm] CreateProductDto dto)
    {
        await _trafficServices.AddReqCountAsync();
        await _trafficServices.AddProductCountAsync();

        try
        {
            ProductModel product = new()
            {
                NumSold = dto.NumSold,
                ImageFiles = dto.ImageFiles,
                RatingsQuantity = dto.ratingsQuantity,
                Title = dto.title,
                Description = dto.description,
                Price = dto.price,
                ImageCoverFile = dto.ImageCoverFile,
                CategoryID = dto.CategoryID,
                BrandID = dto.BrandID
            };

            var createdProduct = await _productService.CreatedAsync(product);

            return Ok(new { message = "Product created successfully!", data = createdProduct, StatusCode = 200 });

        }
        catch (Exception ex) {
            return BadRequest(new { message = "some thing error when creating products try again later!", StatusCode = 400 });
        }
        
    }

    [HttpDelete("{id}")]
    [Authorize]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteProduct(int id)
    {
        await _trafficServices.AddReqCountAsync();
        await _trafficServices.DecreaseProductCountAsync();


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
    [Authorize]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdateProduct(int id, [FromForm] ProductModel productModel)
    {
        await _trafficServices.AddReqCountAsync();

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
            return Ok(new { message = "Product Updated successfully!", data = updatedProduct, StatusCode = 200 });

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

    [HttpGet("AdminGetAllAsync")]
    [Authorize]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> AdminGetAllAsync()
    {
        await _trafficServices.AddReqCountAsync();

        try
        {
            var products = await _productService.GetAllAsync();
            return Ok(products);
        }
        catch
        {
            return BadRequest(new { message = "some thing error when getting products try again later!", StatusCode = 400 });
        }

    }
}

