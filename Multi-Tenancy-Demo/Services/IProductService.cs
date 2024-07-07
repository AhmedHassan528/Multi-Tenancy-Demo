using Multi_Tenancy_Demo.Models;

namespace Multi_Tenancy_Demo.Services
{
    public interface IProductService
    {
        Task<Product> CreatedAsync(Product product);
        Task<Product?> GetByIdAsync(int id);

        Task<IReadOnlyList<Product>> GetAllAsync();

    }
}
