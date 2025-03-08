using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace MultiTenancy.Models;

public class ProductModel : IMustHaveTenant
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required]
    public int NumSold { get; set; }

    [Required]
    public double ratingsQuantity { get; set; }
    [Required]
    [StringLength(50, ErrorMessage = "Title cannot exceed 50 characters.")]
    public string title { get; set; }

    [Required]
    [StringLength(500, ErrorMessage = "Title cannot exceed 50 characters.")]
    public string description { get; set; }

    [Required]
    public decimal price { get; set; }

    public string? imageCover { get; set; }
    [NotMapped]
    [JsonIgnore]
    public IFormFile ImageCoverFile { get; set; }
    public List<string>? Images { get; set; } = new List<string>();
    [NotMapped]
    [JsonIgnore]
    public List<IFormFile> ImageFiles { get; set; }

    [JsonIgnore]
    public string TenantId { get; set; } = null!;



    [ForeignKey("CategoryModel")]
    [JsonIgnore]
    public int? CategoryID { get; set; }
    public virtual CategoryModel? category { get; set; }

    [ForeignKey("BrandModel")]
    [JsonIgnore]
    public int? BrandID { get; set; }
    public virtual BrandModel? Brand { get; set; }


}