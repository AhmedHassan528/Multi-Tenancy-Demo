﻿
using Microsoft.EntityFrameworkCore;

namespace MultiTenancy.Services.AddressServices
{
    public class AddressServices : IAddressServices
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<AppUser> _userManager;
        private readonly IProductService _productService;
        public AddressServices(ApplicationDbContext context, UserManager<AppUser> userManager, IProductService productService)
        {
            _context = context;
            _userManager = userManager;
            _productService = productService;
        }

        public async Task<AddressModel> AddAddress(string userID, AddresesesDto address)
        {
            if (!await UserExistsAsync(userID))
            {
                return null;
            }
            AddressModel model = new()
            {
                userID = userID,
                AddressName = address.AddressName,
                City = address.City,
                Address = address.Address,
                phoneNumber = address.phoneNumber
            };
            _context.Addresses.Add(model);
            await _context.SaveChangesAsync();
            return model;
        }
        public async Task<List<AddressModel>> GetUserAddresses(string userID)
        {
            if (!await UserExistsAsync(userID))
            {
                return null;
            }
            return await _context.Addresses.Where(x => x.userID == userID).ToListAsync();
        }

        public async Task<AddressModel> GetAddressByID(string userID, int addressID)
        {
            if (!await UserExistsAsync(userID))
            {
                return null;
            }
            if (addressID == null)
            {
                return null;
            }
            return await _context.Addresses.AsNoTracking().FirstOrDefaultAsync(x => x.userID == userID && x.Id == addressID);
        }

        public async Task<List<AddressModel>> DeleteAddressByID(string userID, int addressID)
        {
            if (!await UserExistsAsync(userID))
            {
                return null;
            }
            if (addressID == null)
            {
                return null;
            }
            var address = await _context.Addresses.FirstOrDefaultAsync(x => x.userID == userID && x.Id == addressID);
            if (address == null)
            {
                return null;
            }
            _context.Addresses.Remove(address);
            await _context.SaveChangesAsync();
            return await _context.Addresses.AsNoTracking().Where(x => x.userID == userID).ToListAsync();

        }


        public async Task<string> ClearUserAddresses(string userID)
        {
            if (!await UserExistsAsync(userID))
            {
                return "user is not found";
            }
            var addresses = await _context.Addresses.Where(x => x.userID == userID).ToListAsync();
            if (addresses == null)
            {
                return "no addresses to remove it";
            }
            _context.Addresses.RemoveRange(addresses);
            await _context.SaveChangesAsync();
            return "";
        }



        private async Task<bool> UserExistsAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return false;
            }
            return true;
        }
    }
}
