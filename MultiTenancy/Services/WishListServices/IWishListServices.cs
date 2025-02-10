namespace MultiTenancy.Services.WishListServices
{
    public interface IWishListServices
    {
        Task<IReadOnlyList<WishListModel>> GetWishListAsync(string userId);
        Task<string> AddToWithListAsync(string userId, int productId);
        Task<string> DeleteWishListById (string userId, int productId);
        Task<string> DeleteAllWishList(string userId);

    }
}
