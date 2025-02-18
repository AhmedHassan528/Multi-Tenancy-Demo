using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MultiTenancy.Models
{
    public class AddressModel : IMustHaveTenant
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required]
        public string userID { get; set; }
        [Required]
        [MaxLength(20)]
        public string AddressName { get; set; }
        [Required]
        [MaxLength(20)]
        public string City { get; set; }
        [Required]
        [MaxLength(100)]
        public string Address { get; set; }
        [Required]
        [RegularExpression(@"\d{11}$", ErrorMessage = "Phone number must start with +20 and be 11 digits long.")]
        public string phoneNumber { get; set; }
        public string TenantId { get; set; } = null!;
    }
}
