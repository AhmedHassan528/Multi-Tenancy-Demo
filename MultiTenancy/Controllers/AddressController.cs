using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MultiTenancy.Services.TrafficServices;

namespace MultiTenancy.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class AddressController : ControllerBase
    {
        private readonly IAddressServices _addressServices;
        private readonly ITrafficServices _trafficServices;
        private readonly IAuthService _authService;

        public AddressController(IAddressServices addressServices, ITrafficServices trafficServices, IAuthService authService)
        {
            _addressServices = addressServices;
            _trafficServices = trafficServices;
            _authService = authService;
        }

        [HttpGet]
        public async Task<IActionResult> GetUserAddresses()
        {
            await _trafficServices.AddReqCountAsync();

            var userID = User.FindFirst("uid")?.Value;
            if (userID == null || !await _authService.isUser(userID))
            {
                return NotFound(new { message = "Error: User not found. \nPlease ensure you have entered the correct username or email, or register for an account.", StatusCode = 401 });
            }

            var Addresses = await _addressServices.GetUserAddresses(userID);
            if (Addresses == null)
            {
                return Ok("No addresses added");
            }
            return Ok(Addresses);
        }

        [HttpPost]
        public async Task<IActionResult> AddAddress([FromBody] AddresesesDto address)
        {
            await _trafficServices.AddReqCountAsync();

            var userID = User.FindFirst("uid")?.Value;
            if (userID == null || !await _authService.isUser(userID))
            {
                return NotFound(new { message = "Error: User not found. \nPlease ensure you have entered the correct username or email, or register for an account.", StatusCode = 401 });
            }

            var Address = await _addressServices.AddAddress(userID, address);
            if (Address == null)
            {
                return StatusCode(500, Address!.Message);
            }
            return Ok(Address);
        }

        //[HttpGet("GetAddressByID/{addressID}")]
        //public async Task<IActionResult> GetAddressByID(int addressID)
        //{
        //    await _trafficServices.AddReqCountAsync();

        //    var userID = User.FindFirst("uid")?.Value;
        //    if (userID == null || !await _authService.isUser(userID))
        //    {
        //        return NotFound(new { message = "Error: User not found. \nPlease ensure you have entered the correct username or email, or register for an account." });

        //    }

        //    var Address = await _addressServices.GetAddressByID(userID, addressID);

        //    if (!string.IsNullOrEmpty(Address.Message))
        //    {
        //        return StatusCode(500, Address.Message);
        //    }

        //    if (Address == null)
        //    {
        //        return Ok("Address not found");
        //    }

        //    return Ok(Address);
        //}

        [HttpDelete("{addressID}")]
        public async Task<IActionResult> DeleteAddressByID(int addressID)
        {
            await _trafficServices.AddReqCountAsync();

            var userID = User.FindFirst("uid")?.Value;
            if (userID == null || addressID == 0 || !await _authService.isUser(userID))
            {
                return NotFound(new { message = "Error: User not found. \nPlease ensure you have entered the correct username or email, or register for an account." });

            }

            var Addresses = await _addressServices.DeleteAddressByID(userID, addressID);
            if (Addresses == null)
            {
                return NotFound("Address not found");
            }
            return Ok(Addresses);
        }




    }
}
