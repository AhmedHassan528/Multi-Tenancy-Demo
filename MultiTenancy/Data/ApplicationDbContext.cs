using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MultiTenancy.Models.AuthModels;

namespace MultiTenancy.Data;

public class ApplicationDbContext : IdentityDbContext<AppUser>
{
    public string TenantId { get; set; }
    private readonly ITenantService _tenantService;

    public ApplicationDbContext(DbContextOptions options, ITenantService tenantService) : base(options)
    {
        _tenantService = tenantService;
        TenantId = _tenantService.GetCurrentTenant()?.TId;
    }

    public DbSet<ProductModel> Products { get; set; }
    public DbSet<BrandModel> Brands { get; set; }
    public DbSet<CategoryModel> Categories { get; set; }
    public DbSet<WishListModel> WishLists { get; set; }




    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ProductModel>().HasQueryFilter(e => e.TenantId == TenantId);
        base.OnModelCreating(modelBuilder);
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        var tenantConnectionString = _tenantService.GetConnectionString();

        if(!string.IsNullOrWhiteSpace(tenantConnectionString))
        {
            var dbProvider = _tenantService.GetDatabaseProvider();

            if(dbProvider?.ToLower() == "mssql")
            {
                optionsBuilder.UseSqlServer(tenantConnectionString);
            }
        }
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        foreach (var entry in ChangeTracker.Entries<IMustHaveTenant>().Where(e => e.State == EntityState.Added))
        {
            entry.Entity.TenantId = TenantId;
        }

        return base.SaveChangesAsync(cancellationToken);
    }
}