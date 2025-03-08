namespace MultiTenancy.Dtos
{
    public class CreateBrandDto
    {
        public string Name { get; set; }
        public IFormFile imageFile { get; set; }
    }
}
