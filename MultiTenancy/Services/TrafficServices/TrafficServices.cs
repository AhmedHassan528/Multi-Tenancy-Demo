using System.Data;
using Microsoft.EntityFrameworkCore;
using MultiTenancy.Models.traffic;
using MultiTenancy.Settings;
using Stripe;

namespace MultiTenancy.Services.TrafficServices
{
    public class TrafficServices : ITrafficServices
    {
        private readonly ApplicationDbContext _context;
        private readonly ITenantService _tenantService;


        public TrafficServices(ApplicationDbContext context, ITenantService tenantService)
        {
            _context = context;
            _tenantService = tenantService;
        }
        public async Task<Traffic> GetTrafficAsync()
        {
            var tenant = _tenantService.GetCurrentTenant();
            var tenantId = tenant.TId;

            // Validate tenantId
            if (string.IsNullOrEmpty(tenantId))
            {
                throw new ArgumentException("TenantId cannot be null or empty.", nameof(tenantId));
            }

            // Query traffic data
            var traffic = await _context.traffics
                .FirstOrDefaultAsync(t => t.TenantId == tenantId);

            if (traffic == null)
            {
                await CreateTrafficAsync();
            }

            return traffic;

        }

        public async Task AddCategoryCountAsync()
        {
            var tenant = _tenantService.GetCurrentTenant();
            var tenantId = tenant.TId;

            var traffic = await _context.traffics.FirstOrDefaultAsync(t => t.TenantId == tenantId);
            if (traffic == null)
            {
                await CreateTrafficAsync();
            }
            else
            {
                traffic.CategoryCount++;
                _context.traffics.Update(traffic);
                await _context.SaveChangesAsync();
            }

        }



        public async Task AddBrandCountAsync()
        {
            var tenant = _tenantService.GetCurrentTenant();
            var tenantId = tenant.TId;

            var traffic = await _context.traffics.FirstOrDefaultAsync(t => t.TenantId == tenantId);
            if (traffic == null)
            {
                await CreateTrafficAsync();
            }
            else
            {
                traffic.BrandCount++;
                _context.traffics.Update(traffic);
                await _context.SaveChangesAsync();
            }

        }

        public async Task AddProductCountAsync()
        {
            var tenant = _tenantService.GetCurrentTenant();
            var tenantId = tenant.TId;

            var traffic = await _context.traffics.FirstOrDefaultAsync(t => t.TenantId == tenantId);
            if (traffic == null)
            {
                await CreateTrafficAsync();
            }
            else
            {
                traffic.ProductCount++;
                _context.traffics.Update(traffic);
                await _context.SaveChangesAsync();
            }

        }

        public async Task AddOrderCountAsync()
        {
            var tenant = _tenantService.GetCurrentTenant();
            var tenantId = tenant.TId;

            var traffic = await _context.traffics.FirstOrDefaultAsync(t => t.TenantId == tenantId);
            if (traffic == null)
            {
                await CreateTrafficAsync();
            }
            else
            {
                traffic.OrderCount++;
                _context.traffics.Update(traffic);
                await _context.SaveChangesAsync();
            }

        }

        public async Task AddReqCountAsync()
        {
            var tenant = _tenantService.GetCurrentTenant();
            var tenantId = tenant.TId;

            var traffic = await _context.traffics.FirstOrDefaultAsync(t => t.TenantId == tenantId);
            if (traffic == null)
            {
                await CreateTrafficAsync();
            }
            else
            {
                traffic.RequestDates.RemoveAll(d => (DateTime.UtcNow - d).TotalDays >= 30);

                traffic.RequestDates.Add(DateTime.UtcNow);

                traffic.DateNow = DateTime.UtcNow;

                _context.traffics.Update(traffic);
            }

            await _context.SaveChangesAsync();
        }

        public async Task CreateTrafficAsync()
        {
            var tenant = _tenantService.GetCurrentTenant();
            var tenantId = tenant.TId;

            var traffic = new Traffic
            {
                TenantId = tenantId,
                ProductCount = 0,
                CategoryCount = 0,
                BrandCount = 0,
                OrderCount = 0,
                RequestDates = new List<DateTime> { DateTime.UtcNow },
                DateNow = DateTime.UtcNow
            };
            _context.traffics.Add(traffic);
            await _context.SaveChangesAsync();
        }

        public async Task DecreaseCategoryCountAsync()
        {
            var tenant = _tenantService.GetCurrentTenant();
            var tenantId = tenant.TId;

            var traffic = await _context.traffics.FirstOrDefaultAsync(t => t.TenantId == tenantId);
            if (traffic == null)
            {
                CreateTrafficAsync();
            }
            else
            {
                if (traffic.CategoryCount > 0)
                {
                    traffic.CategoryCount--;
                }
                else
                {
                    traffic.CategoryCount = 0;
                }
                _context.traffics.Update(traffic);
                await _context.SaveChangesAsync();
            }

        }

        public async Task DecreaseBrandCountAsync()
        {
            var tenant = _tenantService.GetCurrentTenant();
            var tenantId = tenant.TId;

            var traffic = await _context.traffics.FirstOrDefaultAsync(t => t.TenantId == tenantId);
            if (traffic == null)
            {
                CreateTrafficAsync();
            }
            else
            {
                if (traffic.BrandCount > 0)
                {
                    traffic.BrandCount--;
                }
                else
                {
                    traffic.BrandCount = 0;
                }
                _context.traffics.Update(traffic);
                await _context.SaveChangesAsync();
            }

        }

        public async Task DecreaseProductCountAsync()
        {
            var tenant = _tenantService.GetCurrentTenant();
            var tenantId = tenant.TId;

            var traffic = await _context.traffics.FirstOrDefaultAsync(t => t.TenantId == tenantId);
            if (traffic == null)
            {
                CreateTrafficAsync();
            }
            else
            {
                if (traffic.ProductCount > 0)
                {
                    traffic.ProductCount--;
                }
                else
                {
                    traffic.ProductCount = 0;
                }
                _context.traffics.Update(traffic);
                await _context.SaveChangesAsync();
            }

        }



        public async Task ResetTraffic()
        {
            var tenant = _tenantService.GetCurrentTenant();
            var tenantId = tenant.TId;
            var traffic = await _context.traffics.FirstOrDefaultAsync(t => t.TenantId == tenantId);
            if (traffic != null)
            {
                traffic.ProductCount = 0;
                traffic.CategoryCount = 0;
                traffic.BrandCount = 0;
                traffic.OrderCount = 0;
                traffic.RequestDates.Clear();
                traffic.DateNow = DateTime.UtcNow;
                _context.traffics.Update(traffic);
                await _context.SaveChangesAsync();
            }

        }

    }
    public class TrafficDto
    {
        public int Id { get; set; }
        public string TenantId { get; set; }
        public int ProductCount { get; set; }
        public int CategoryCount { get; set; }
        public int BrandCount { get; set; }
        public int OrderCount { get; set; }
        public List<DateTime> RequestDates { get; set; }
        public int TotalRequests { get; set; }
        public DateTime DateNow { get; set; }
    }

}


