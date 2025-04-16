using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using MultiTenancy.Services.TrafficServices;

namespace Authentication_With_JWT.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly ISendMail _sendMail;
        private readonly UserManager<AppUser> _userManager;
        private readonly ITrafficServices _trafficServices;

        public AuthController(IAuthService authService, ISendMail sendMail, UserManager<AppUser> userManager, ITrafficServices trafficServices)
        {
            _authService = authService;
            _sendMail = sendMail;
            _userManager = userManager;
            _trafficServices = trafficServices;
        }
        [HttpPost("Register")]
        public async Task<IActionResult> Register([FromBody] RegisterModel model, [FromHeader] string? ReqUrl)
        {
            await _trafficServices.AddReqCountAsync();

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _authService.RegisterAsync(model, ReqUrl);

            if (!result.IsAuthenticated)
                return BadRequest(result.Message);

            return Ok(new { message = "You’ve got mail! Please check your inbox to confirm your email address." });

        }
        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            await _trafficServices.AddReqCountAsync();

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _authService.LoginAsync(model);

            if (!result.IsAuthenticated)
                return BadRequest(new { message = result.Message});

            return Ok(result);
        }
        [HttpPost("AddRole")]
        public async Task<IActionResult> AddRole([FromBody] AddRoleModel model)
        {
            await _trafficServices.AddReqCountAsync();

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _authService.AddRoleAsync(model);

            if (!string.IsNullOrEmpty(result))
                return BadRequest(result);

            return Ok(result);
        }

        [HttpPost("ConfirmEmail")]
        public async Task<IActionResult> ConfirmEmail(string userId, string token)
        {
            await _trafficServices.AddReqCountAsync();

            var user = await _userManager.FindByIdAsync(userId);
            if (userId == null || token == null)
            {
                return BadRequest("Link expired");

            }
            else if (user == null)
            {
                return BadRequest("User not Found");

            }
            var result = await _authService.ConfirmEmail(userId, token);

            if (!string.IsNullOrEmpty(result))
                return BadRequest(result);
            return Ok("Your Email confirmed");

        }
        [HttpPost("ForgotPassword")]
        public async Task<IActionResult> ForgotPassword([FromBody] string email, [FromHeader] string ReqUrl)
        {
            await _trafficServices.AddReqCountAsync();

            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return BadRequest("User not found");
            }
            else
            {
                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                var result = await _sendMail.SendEmailAsync(email, "Reset Password", token, "ForgotPasswordConfermation", ReqUrl);
                return Ok(result);
            }
        }
        [HttpPost("ForgotPasswordConfermation")]
        public async Task<IActionResult> ForgotPasswordConfermation([FromBody] ForgotPasswordConfermationModel model)
        {
            await _trafficServices.AddReqCountAsync();

            if (model.newPassword != model.confirmPassword)
            {
                return BadRequest("Password not match");
            }

            var result = await _authService.ForgotPasswordConfermationModel(model);

            if (!string.IsNullOrEmpty(result))
                return BadRequest(result);

            return Ok("Password changed");

        }




        [HttpPost("AddRoleToUser")]
        [Authorize]
        [Authorize(Roles = "Admin")]

        public async Task<IActionResult> AddRoleToUser([FromHeader] string userEmail)
        {
            await _trafficServices.AddReqCountAsync();

            var AdminID = User.FindFirst("uid")?.Value;
            if (AdminID == null || userEmail == null)
            {
                return BadRequest("User not found");
            }

            var result = await _authService.setAdminRole(AdminID, userEmail);
            if (!string.IsNullOrEmpty(result))
                return BadRequest(result);
            return Ok("user is now Admin");
        }

        [HttpGet("GetAllUsersAsync")]
        [Authorize]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllUsersAsync()
        {
            await _trafficServices.AddReqCountAsync();

            var AdminID = User.FindFirst("uid")?.Value;
            if (AdminID == null)
            {
                return NotFound();
            }

            try
            {
                var result = await _authService.GetAllUsersAsync(AdminID);
                return Ok(result);
            }
            catch (Exception e)
            {
                throw;
            }
        }


    }
}
