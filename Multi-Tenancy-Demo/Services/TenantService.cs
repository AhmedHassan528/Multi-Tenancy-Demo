
using Microsoft.Extensions.Options;
using Multi_Tenancy_Demo.Settings;

namespace Multi_Tenancy_Demo.Services
{
    public class TenantService : ITenantService
    {
        private readonly TenantSettings _tenantSettings;
        private Tenant _CurrentTenant;
        private HttpContext _httpContext;

        public TenantService(IHttpContextAccessor contextAccessor, IOptions<TenantSettings> tenantSettings)
        {
            _httpContext = contextAccessor.HttpContext;
            _tenantSettings = tenantSettings.Value;

            if (_httpContext.Request.Headers.TryGetValue("tenant", out var SetTenant))
            {
                SetCurrentTenant(SetTenant!);
            }
        }




        public string? GetDatabaseProvider()
        {
            var currentConnectionString = _CurrentTenant is null
                ? _tenantSettings.Configuration.ConnectionString
                : _CurrentTenant.ConnectionString;

            return currentConnectionString;
        }
        public string? GetConnectionString()
        {
            return _CurrentTenant.ConnectionString;
        }
        public Tenant? GetCurrentTenant()
        {
            return _CurrentTenant;
        }

        private void SetCurrentTenant(string tenantId)
        {
            _CurrentTenant = _tenantSettings.Tenants.FirstOrDefault(t => t.TId == tenantId);

            if (_CurrentTenant == null)
            {
                throw new Exception("Tenant not found");
            }
            if (string.IsNullOrEmpty(_CurrentTenant.ConnectionString))
            {
                _CurrentTenant.ConnectionString = _tenantSettings.Configuration.ConnectionString;
            }
        }
    }
}
