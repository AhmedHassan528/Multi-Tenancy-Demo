using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace MultiTenancy.Models.CheckOutModels
{
    public class OrderItem : IMustHaveTenant
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [ForeignKey("Order")]
        public int OrderId { get; set; }

        public virtual Order Order { get; set; }

        [Required]
        [ForeignKey("ProductModel")]
        public int ProductId { get; set; }

        public virtual ProductModel Product { get; set; }

        [Required]
        public int Quantity { get; set; }

        [Required]
        public decimal Price { get; set; }


        public string TenantId { get; set; } = null!;

    }
}
