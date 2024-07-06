namespace Multi_Tenancy_Demo.Settings
{
    public class Tenant
    {
        public string Name { get; set; } = null!;
        public string TId { get; set; } = null!;
        public string? ConnectionString { get; set; }
    }
}
