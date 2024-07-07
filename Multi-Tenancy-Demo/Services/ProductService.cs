


namespace Multi_Tenancy_Demo.Services
{
    public class ProductService : IProductService
    {
        private readonly ApplicationDbContext _db;
        public ProductService(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<Product> CreatedAsync(Product product)
        {
            _db.Products.Add(product);
            await _db.SaveChangesAsync();

            return product;
        }

        public async Task<IReadOnlyList<Product>> GetAllAsync()
        {
            return await _db.Products.ToListAsync();
        }

        public async Task<Product?> GetByIdAsync(int id)
        {
           var product = await _db.Products.FindAsync(id);
           return product;
        }
    }
}
