using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace MultiTenancy.Models.CheckOutModels
{
    public class OrderItem : IMustHaveTenant
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int OrderId { get; set; }
        [JsonIgnore]
        public virtual Order Order { get; set; }

        [Required]
        public int Count { get; set; }

        [Required]
        public decimal Price { get; set; }

        [Required]
        public int ProductId { get; set; }
        [JsonIgnore]
        public virtual ProductModel Product { get; set; }

        // Optional: Store product name for historical reference
        public string ProductName { get; set; }

        [Required]
        public string TenantId { get; set; } = null!;

    }
}
