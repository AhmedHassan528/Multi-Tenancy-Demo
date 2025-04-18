
using Microsoft.EntityFrameworkCore;
using Stripe;

namespace MultiTenancy.Services.AddressServices
{
    public class AddressServices : IAddressServices
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<AppUser> _userManager;

        public AddressServices(ApplicationDbContext context, UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        public async Task<List<AddressModel>> GetUserAddresses(string userID)
        {
            return await _context.Addresses.Where(x => x.UserID == userID).ToListAsync();
        }

        public async Task<AddressModel> AddAddress(string userID, AddresesesDto address)
        {
            AddressModel model = new()
            {
                UserID = userID,
                AddressName = address.AddressName,
                City = address.City,
                Address = address.Address,
                PhoneNumber = address.phoneNumber
            };
            try
            {
                _context.Addresses.Add(model);
                await _context.SaveChangesAsync();
                return model;
            }
            catch (Exception ex)
            {
                return new AddressModel { Message = ex.Message };
            }
        }


        public async Task<AddressModel> GetAddressByID(string userID, int addressID)
        {

            if (addressID == null)
            {
                return new AddressModel { Message = "address not found" };
            }
            try
            {
                return await _context.Addresses.AsNoTracking().FirstOrDefaultAsync(x => x.UserID == userID && x.Id == addressID);
            }
            catch (Exception ex)
            {
                return new AddressModel { Message = ex.Message };
            }
        }

        public async Task<List<AddressModel>> DeleteAddressByID(string userID, int addressID)
        {

            var address = await _context.Addresses.FirstOrDefaultAsync(x => x.UserID == userID && x.Id == addressID);
            if (address == null)
            {
                return null;
            }

            _context.Addresses.Remove(address);
            await _context.SaveChangesAsync();
            return await _context.Addresses.AsNoTracking().Where(x => x.UserID == userID).ToListAsync();
        }

    }
}
