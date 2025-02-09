namespace MultiTenancy.Models
{
    public class BrandModel : IMustHaveTenant
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string image { get; set; }
        public string TenantId { get; set; } = null!;

    }
}
