using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MultiTenancy.Models
{
    public class CartItemModel : IMustHaveTenant
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public int Count { get; set; }

        [Required]
        public decimal Price { get; set; }

        // Foreign key to ProductModel
        [Required]
        [ForeignKey("ProductModel")]
        public int ProductId { get; set; }
        public virtual ProductModel? Product { get; set; }

        // Foreign key to Cart
        [ForeignKey("CartModel")]
        public int CartId { get; set; }
        public virtual CartModel? Cart { get; set; }

        public string TenantId { get; set; } = null!;

    }
}
