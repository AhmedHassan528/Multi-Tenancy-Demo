using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace MultiTenancy.Models
{
    public class CategoryModel : IMustHaveTenant
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string? image { get; set; }
        [NotMapped]
        [JsonIgnore]
        public IFormFile ImageFiles { get; set; }

        [JsonIgnore]
        public string TenantId { get; set; } = null!;

    }
}
