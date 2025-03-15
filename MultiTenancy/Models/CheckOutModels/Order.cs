using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace MultiTenancy.Models.CheckOutModels
{
    public class Order : IMustHaveTenant
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string CartOwner { get; set; }

        [Required]
        public int CartId { get; set; }
        [JsonIgnore]
        public virtual CartModel Cart { get; set; }

        [Required]
        public decimal TotalAmount { get; set; }

        public bool status { get; set; } = false;

        public string PaymentIntentId { get; set; }

        public int AddressId { get; set; }
        [JsonIgnore]
        public virtual AddressModel Address { get; set; }

        // Corrected to List<OrderItem>
        public List<OrderItem> Items { get; set; } = new List<OrderItem>();

        public DateTime OrderDate { get; set; } = DateTime.UtcNow;

        [Required]
        public string TenantId { get; set; } = null!;
    }

}
