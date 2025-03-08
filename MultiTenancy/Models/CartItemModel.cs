using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MultiTenancy.Models
{
    public class CartItemModel
    {
        [Key]
        public string Id { get; set; } = Guid.NewGuid().ToString();

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
        public string CartId { get; set; }
        public virtual CartModel? Cart { get; set; }
    }
}
