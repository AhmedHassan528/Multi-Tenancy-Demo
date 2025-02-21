using System.ComponentModel.DataAnnotations;

namespace MultiTenancy.Models
{
    public class CartModel
    {
        [Key]
        public string Id { get; set; } = Guid.NewGuid().ToString();

        [Required]
        public string CartOwner { get; set; }

        public List<CartItemModel> Products { get; set; } = new List<CartItemModel>();

        [Required]
        public decimal TotalCartPrice { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
