using System.ComponentModel.DataAnnotations;

namespace MultiTenancy.Models
{
    public class CartModel : IMustHaveTenant 
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string CartOwner { get; set; }

        public List<CartItemModel> Products { get; set; } = new List<CartItemModel>();



        [Required]
        public decimal TotalCartPrice { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        public string TenantId { get; set; } = null!;

    }
}
