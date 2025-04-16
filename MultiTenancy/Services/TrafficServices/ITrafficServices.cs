using MultiTenancy.Models.traffic;

namespace MultiTenancy.Services.TrafficServices
{
    public interface ITrafficServices
    {
        Task<Traffic> GetTrafficAsync();
        Task AddCategoryCountAsync();
        Task AddBrandCountAsync();
        Task AddProductCountAsync();
        Task AddOrderCountAsync();
        Task AddReqCountAsync();

        Task DecreaseCategoryCountAsync();
        Task DecreaseBrandCountAsync();
        Task DecreaseProductCountAsync();


    }
}
