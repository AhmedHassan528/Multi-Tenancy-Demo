using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace MultiTenancy.Models.CheckOutModels
{
    public class Order : IMustHaveTenant
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; } // Buyer

        public DateTime OrderDate { get; set; } = DateTime.UtcNow;

        [Required]
        public decimal TotalPrice { get; set; }

        public string Status { get; set; } = "Pending"; // Pending, Shipped, Delivered

        public virtual List<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

        [Required]
        [ForeignKey("AddressModel")]
        public int AddressId { get; set; }

        public virtual AddressModel ShippingAddress { get; set; }

        public string TenantId { get; set; } = null!;
    }

}
