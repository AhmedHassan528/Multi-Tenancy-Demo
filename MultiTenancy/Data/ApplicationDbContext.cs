﻿using System.Text.Json;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MultiTenancy.Models.CheckOutModels;
using MultiTenancy.Models.traffic;

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
    public DbSet<AddressModel> Addresses { get; set; }
    public DbSet<CartModel> Carts { get; set; }
    public DbSet<CartItemModel> CartItemes { get; set; }

    public DbSet<Order> Orders { get; set; }
    public DbSet<OrderItem> OrderItems { get; set; }
    public DbSet<Items> Items { get; set; }
    
    public DbSet<Traffic> traffics { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ProductModel>().HasQueryFilter(e => e.TenantId == TenantId);
        modelBuilder.Entity<BrandModel>().HasQueryFilter(e => e.TenantId == TenantId);
        modelBuilder.Entity<CategoryModel>().HasQueryFilter(e => e.TenantId == TenantId);
        modelBuilder.Entity<WishListModel>().HasQueryFilter(e => e.TenantId == TenantId);
        modelBuilder.Entity<AddressModel>().HasQueryFilter(e => e.TenantId == TenantId);
        modelBuilder.Entity<CartModel>().HasQueryFilter(e => e.TenantId == TenantId);
        modelBuilder.Entity<CartItemModel>().HasQueryFilter(e => e.TenantId == TenantId);
        modelBuilder.Entity<Order>().HasQueryFilter(e => e.TenantId == TenantId);
        modelBuilder.Entity<OrderItem>().HasQueryFilter(e => e.TenantId == TenantId);
        modelBuilder.Entity<Items>().HasQueryFilter(e => e.TenantId == TenantId);
        modelBuilder.Entity<Traffic>().HasQueryFilter(e => e.TenantId == TenantId);

        modelBuilder.Entity<Order>(entity =>
        {
            entity.Property(e => e.CustomerName).HasColumnName("CustomerName");
            entity.Property(e => e.paymentMethodType).HasColumnName("PaymentMethodType");
            entity.Property(e => e.statusMess).HasColumnName("StatusMessage");
        });




        modelBuilder.Entity<ProductModel>().Property(p => p.Images)
        .HasConversion(
            v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
            v => string.IsNullOrEmpty(v) ? new List<string>() : JsonSerializer.Deserialize<List<string>>(v, (JsonSerializerOptions?)null) ?? new List<string>()
        );

        modelBuilder.Entity<CartModel>()
            .HasMany(c => c.Products)
            .WithOne(ci => ci.Cart)
            .HasForeignKey(ci => ci.CartId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<CartItemModel>()
            .HasOne(ci => ci.Product)
            .WithMany()
            .HasForeignKey(ci => ci.ProductId)
            .OnDelete(DeleteBehavior.Restrict);

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