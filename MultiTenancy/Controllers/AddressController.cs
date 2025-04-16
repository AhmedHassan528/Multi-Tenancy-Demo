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

        public AddressController(IAddressServices addressServices, ITrafficServices trafficServices)
        {
            _addressServices = addressServices;
            _trafficServices = trafficServices;

        }

        [HttpGet]
        public async Task<IActionResult> GetUserAddresses()
        {
            await _trafficServices.AddReqCountAsync();
            // Get the user ID from the claims
            var userID = User.FindFirst("uid")?.Value;
            if (userID == null)
            {
                return BadRequest(new { message = "some thing error when get adresses", StatusCode = 400 });
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
            if (userID == null)
            {
                return NotFound(new { message = "some thing error when create address tray agian later!", StatusCode = 400 });
            }

            var Address = await _addressServices.AddAddress(userID, address);
            if (Address == null)
            {
                return BadRequest("Address not added");
            }
            return Ok(Address);
        }

        [HttpGet("GetAddressByID/{addressID}")]
        public async Task<IActionResult> GetAddressByID(int addressID)
        {
            await _trafficServices.AddReqCountAsync();

            var userID = User.FindFirst("uid")?.Value;
            if (userID == null)
            {
                return BadRequest(new { message = "some thing error when get address try again later!" });

            }
            var Address = await _addressServices.GetAddressByID(userID, addressID);
            if (Address == null)
            {
                return BadRequest("Address not found");
            }
            return Ok(Address);
        }

        [HttpDelete("{addressID}")]
        public async Task<IActionResult> DeleteAddressByID(int addressID)
        {
            await _trafficServices.AddReqCountAsync();

            var userID = User.FindFirst("uid")?.Value;
            if (userID == null || addressID == 0)
            {
                return BadRequest(new { message = "some thing error when delete address try again later!" });

            }
            var Addresses = await _addressServices.DeleteAddressByID(userID, addressID);
            if (Addresses == null)
            {
                return BadRequest("Address not found");
            }
            return Ok(Addresses);
        }

        [HttpDelete]
        public async Task<IActionResult> CrearAllUserAddress()
        {
            await _trafficServices.AddReqCountAsync();

            var userID = User.FindFirst("uid")?.Value;

            if (userID == null)
            {
                return BadRequest(new { message = "some thing error when clear addresses try again later!" });
            }
            var Addresses = await _addressServices.ClearUserAddresses(userID);
            if (Addresses == null)
            {
                return BadRequest(Addresses);
            }
            return Ok("Address are cleared successfully");
        }



    }
}
