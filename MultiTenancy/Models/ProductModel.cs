using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MultiTenancy.Models;

public class ProductModel : IMustHaveTenant
{
    public int Id { get; set; }
    public string NumSold { get; set; } = null!;
    public string images { get; set; } = null!;
    public int ratingsQuantity { get; set; }
    public string title { get; set; }
    public string description { get; set; }
    public int quantity { get; set; }
    public string price { get; set; }
    public string imageCover { get; set; }
    public string TenantId { get; set; } = null!;



    [ForeignKey("CategoryModel")]
    public int? CategoryID { get; set; }
    public virtual CategoryModel? category { get; set; }

    [ForeignKey("BrandModel")]
    public int? BrandID { get; set; }
    public virtual BrandModel? Brand { get; set; }


}