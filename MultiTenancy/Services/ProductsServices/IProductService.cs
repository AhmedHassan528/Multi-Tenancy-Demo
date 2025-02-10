namespace MultiTenancy.Services.ProductsServices;

public interface IProductService
{
    Task<ProductModel> CreatedAsync(ProductModel product);
    Task<ProductModel?> GetByIdAsync(int id);
    Task<string> DeleteProduct(int id);
    Task<IReadOnlyList<ProductModel>> GetAllAsync();
}
