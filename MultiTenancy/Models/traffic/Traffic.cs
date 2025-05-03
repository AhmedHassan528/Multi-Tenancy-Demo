namespace MultiTenancy.Models.traffic
{
    public class Traffic : IMustHaveTenant
    {
        public int Id { get; set; } = 0;
        public int ProductCount { get; set; } = 0;
        public int CategoryCount { get; set; } = 0;
        public int BrandCount { get; set; } = 0;
        public int OrderCount { get; set; } = 0;
        public List<DateTime> RequestDates { get; set; } = new List<DateTime>(); 
        public DateTime DateNow { get; set; } = DateTime.UtcNow;

        public string TenantId { get; set; }
    }
}
