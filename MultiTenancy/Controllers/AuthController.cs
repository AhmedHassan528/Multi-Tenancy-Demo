using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;

namespace Authentication_With_JWT.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly ISendMail _sendMail;
        private readonly UserManager<AppUser> _userManager;
        public AuthController(IAuthService authService, ISendMail sendMail, UserManager<AppUser> userManager)
        {
            _authService = authService;
            _sendMail = sendMail;
            _userManager = userManager;
        }
        [HttpPost("Register")]
        public async Task<IActionResult> Register([FromBody] RegisterModel model, [FromHeader] string? ReqUrl)
        {
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
        public async Task<string> ForgotPassword([FromBody] string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return "User not found";
            }
            else
            {
                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                var result = await _sendMail.SendEmailAsync(email, "Reset Password", token, "ForgotPasswordConfermation", null);
                return result;
            }
        }

        [HttpPost("ForgotPasswordConfermation")]
        public async Task<string> ForgotPasswordConfermation([FromBody] ForgotPasswordConfermationModel model)
        {
            if (model.newPassword != model.confirmPassword)
            {
                return "Password not match";
            }

            var result = await _authService.ForgotPasswordConfermationModel(model);

            if (!string.IsNullOrEmpty(result))
                return result;

            return "Password changed";
        }

    }
}
