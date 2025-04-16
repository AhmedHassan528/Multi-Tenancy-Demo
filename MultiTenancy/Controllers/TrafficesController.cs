using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MultiTenancy.Services.TrafficServices;

namespace MultiTenancy.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TrafficesController : ControllerBase
    {
        private readonly ITrafficServices _addressServices;
        public TrafficesController(ITrafficServices _addressServices)
        {
            this._addressServices = _addressServices;
        }
        [HttpGet]
        public async Task<IActionResult> GetTraffic()
        {
            var traffic = await _addressServices.GetTrafficAsync();
            if (traffic == null)
            {
                return NotFound();
            }
            return Ok(traffic);
        }
    }
}
