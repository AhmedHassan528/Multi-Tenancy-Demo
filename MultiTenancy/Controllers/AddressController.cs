using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MultiTenancy.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AddressController : ControllerBase
    {
        private readonly IAddressServices _addressServices;
        public AddressController(IAddressServices addressServices)
        {
            _addressServices = addressServices;
        }

        [HttpGet]
        public async Task<IActionResult> GetUserAddresses([FromHeader] string userID)
        {
            var Addresses = await _addressServices.GetUserAddresses(userID);
            if (Addresses == null)
            {
                return Ok("No addresses added");
            }
            return Ok(Addresses);
        }

        [HttpPost]
        public async Task<IActionResult> AddAddress([FromHeader] string userID, [FromBody] AddresesesDto address)
        {
            var Address = await _addressServices.AddAddress(userID,address);
            if (Address == null)
            {
                return BadRequest("Address not added");
            }
            return Ok(Address);
        }

        [HttpGet("GetAddressByID/{addressID}")]
        public async Task<IActionResult> GetAddressByID(int addressID, [FromHeader] string userID )
        {
            var Address = await _addressServices.GetAddressByID(userID, addressID);
            if (Address == null)
            {
                return BadRequest("Address not found");
            }
            return Ok(Address);
        }

        [HttpDelete("{addressID}")]
        public async Task<IActionResult> DeleteAddressByID(int addressID, [FromHeader] string userID)
        {
            if (userID == null || addressID == 0)
            {
                return NotFound();
            }
            var Addresses = await _addressServices.DeleteAddressByID(userID, addressID);
            if (Addresses == null)
            {
                return BadRequest("Address not found");
            }
            return Ok(Addresses);
        }

        [HttpDelete]
        public async Task<IActionResult> CrearAllUserAddress([FromHeader] string userID)
        {
            if (userID == null)
            {
                return NotFound();
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
