namespace Multi_Tenancy_Demo.Settings
{
    public class TenantSettings
    {
        public List<Tenant> Tenants { get; set; } = new List<Tenant>();
        public Configuration Configuration { get; set; } = new Configuration();
    }
}
