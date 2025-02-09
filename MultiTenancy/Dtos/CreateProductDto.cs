namespace MultiTenancy.Dtos;

public class CreateProductDto
{
    public string title { get; set; }
    public string description { get; set; }
    public string price { get; set; }
    public string images { get; set; } = null!;
    public int quantity { get; set; }
    public string NumSold { get; set; } = null!;
    public int ratingsQuantity { get; set; }
    public string imageCover { get; set; }
    public int? CategoryID { get; set; }
    public int? BrandID { get; set; }


}