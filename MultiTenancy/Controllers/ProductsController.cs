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
    private readonly IAuthService _authService;



    public ProductsController(IProductService productService, ITrafficServices trafficServices, IAuthService authService)
    {
        _productService = productService;
        _trafficServices = trafficServices;
        _authService = authService;
    }


    [HttpGet]
    public async Task<IActionResult> GetAllAsync()
    {
        await _trafficServices.AddReqCountAsync();

        try
        {
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
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }


    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetAsync(int id)
    {
        await _trafficServices.AddReqCountAsync();

        try
        {
            if (id <= 0)
            {
                return NotFound(new { message = "Invalid product ID" });
            }
            var product = await _productService.GetByIdAsync(id);
            if (product == null)
            {
                return NotFound(new { message = "can not find this product" });
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
            return BadRequest(new { message = ex.Message });

        }

    }

    [Authorize]
    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<IActionResult> CreatedAsync([FromForm] CreateProductDto dto)
    {
        await _trafficServices.AddReqCountAsync();
        await _trafficServices.AddProductCountAsync();

        var userID = User.FindFirst("uid")?.Value;
        if (string.IsNullOrEmpty(userID) || !await _authService.isAdmin(userID))
        {
            return NotFound(new { message = "Error: User not found or not authorized. \nPlease ensure you have entered the correct username or email, or register for an account.", StatusCode = 401 });
        }


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
            return BadRequest(new { message = ex.Message });
        }
        
    }

    [HttpDelete("{id}")]
    [Authorize]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteProduct(int id)
    {
        await _trafficServices.AddReqCountAsync();
        await _trafficServices.DecreaseProductCountAsync();

        var userID = User.FindFirst("uid")?.Value;
        if (userID == null || !await _authService.isAdmin(userID))
        {
            return NotFound(new { message = "Error: User not found. \nPlease ensure you have entered the correct username or email, or register for an account.", StatusCode = 401 });
        }


        if (id <= 0)
        {
            return NotFound(new { message = "Invalid product ID" });
        }

        try
        {
            var result = await _productService.DeleteProduct(id);
            return Ok(result);

        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });

        }

    }

    [HttpPut("{id}")]
    [Authorize]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdateProduct(int id, [FromForm] ProductModel productModel)
    {
        await _trafficServices.AddReqCountAsync();

        var userID = User.FindFirst("uid")?.Value;
        if (userID == null || !await _authService.isAdmin(userID))
        {
            return NotFound(new { message = "Error: User not found. \nPlease ensure you have entered the correct username or email, or register for an account.", StatusCode = 401 });
        }

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
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("AdminGetAllAsync")]
    [Authorize]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> AdminGetAllAsync()
    {
        await _trafficServices.AddReqCountAsync();

        var userID = User.FindFirst("uid")?.Value;
        if (userID == null )
        {
            return NotFound(new { message = "Error: User not found. \nPlease ensure you have entered the correct username or email, or register for an account.", StatusCode = 401 });
        }

        try
        {
            var products = await _productService.GetAllAsync();
            return Ok(products);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }

    }
}

