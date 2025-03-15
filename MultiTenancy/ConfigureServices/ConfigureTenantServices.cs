using Microsoft.EntityFrameworkCore;

namespace MultiTenancy.ConfigureServices;

public static class ConfigureTenantServices
{
    public static IServiceCollection AddTenancy(this IServiceCollection services, ConfigurationManager configuration)
    {
        services.AddScoped<ITenantService, TenantService>();
        services.AddHttpContextAccessor();

        services.Configure<TenantSettings>(configuration.GetSection(nameof(TenantSettings)));

        TenantSettings options = new();
        configuration.GetSection(nameof(TenantSettings)).Bind(options);

        var defaultDbProvider = options.Defaults.DBProvider;

        if (defaultDbProvider.ToLower() == "mssql")
        {
            services.AddDbContext<ApplicationDbContext>((serviceProvider, dbOptions) =>
            {
                var tenantService = serviceProvider.GetRequiredService<ITenantService>();
                var connectionString = tenantService.GetConnectionString();
                dbOptions.UseSqlServer(connectionString);
            });
        }

        // Apply migrations (run once at startup, not per request)
        using var scope = services.BuildServiceProvider().CreateScope();
        foreach (var tenant in options.Tenants)
        {
            var connectionString = tenant.ConnectionString ?? options.Defaults.ConnectionString;
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            dbContext.Database.SetConnectionString(connectionString);

            if (dbContext.Database.GetPendingMigrations().Any())
            {
                dbContext.Database.Migrate();
            }
        }

        return services;
    }
}
