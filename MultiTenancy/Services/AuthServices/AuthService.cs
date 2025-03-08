
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Authentication_With_JWT.Helper;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Authentication_With_JWT.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly ISendMail _sendMail;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly JWT _jwt;

        public AuthService(UserManager<AppUser> usMan, ISendMail sendMail, RoleManager<IdentityRole> roleManager, IOptions<JWT> jwt)
        {
            _sendMail = sendMail;
            _userManager = usMan;
            _roleManager = roleManager;
            _jwt = jwt.Value;

        }



        public async Task<AuthModel> RegisterAsync(RegisterModel model, string? ReqUrl)
        {
            if (await _userManager.FindByEmailAsync(model.Email) is not null)
                return new AuthModel { Message = "Email is already registered!" };
            if (await _userManager.FindByNameAsync(model.UserName) is not null)
                return new AuthModel { Message = "Username is already taken!" };

            var user = new AppUser
            {
                UserName = model.UserName,
                Email = model.Email.ToLower(),
                FirstName = model.FirstName,
                LastName = model.LastName
            };

            var result = await _userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
            {
                return new AuthModel
                {
                    Message = "Error: " + string.Join(" | ", result.Errors.Select(e => e.Description)),
                    IsAuthenticated = false
                };
            }

            // generate confirm token
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);

            // Send Email
            var EmailSend = await _sendMail.SendEmailAsync(model.Email, "Confirmation Your Account", token, "ConfirmEmail", ReqUrl);
            if (!string.IsNullOrEmpty(EmailSend))
            {
                await _userManager.DeleteAsync(user);
                return new AuthModel { Message = "something error when sending email" };
            }


            if (!result.Succeeded)
            {
                var Errors = string.Empty;
                foreach (var error in result.Errors)
                {
                    Errors += error.Description + Environment.NewLine;
                }
                return new AuthModel { Message = Errors };
            }
            var roleName = "User";
            if (!await _roleManager.RoleExistsAsync(roleName))
            {
                await _roleManager.CreateAsync(new IdentityRole(roleName));
            }

            await _userManager.AddToRoleAsync(user, "User");

            var jwtSecurityToken = await CreateJwtToken(user);

            return new AuthModel
            {
                IsAuthenticated = true,
                Email = user.Email,
                ExpiresOn = jwtSecurityToken.ValidTo,
                Token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken),
                Username = user.UserName,
                Roles = new List<string> { "User" }
            };
        }
        public async Task<AuthModel> LoginAsync(LoginModel model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user is null || !await _userManager.CheckPasswordAsync(user, model.Password))
            {
                return new AuthModel { Message = "Email or password is incorrect" };
            }
            if(!await _userManager.IsEmailConfirmedAsync(user))
            {
                return new AuthModel { Message = "Must Confirm your email address" };
            }
            var jwtSecurityToken = await CreateJwtToken(user);

            var roles = await _userManager.GetRolesAsync(user);
            return new AuthModel
            {
                IsAuthenticated = true,
                Email = user.Email,
                ExpiresOn = jwtSecurityToken.ValidTo,
                Token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken),
                Username = user.UserName,
                Roles = roles.ToList()
            };

        }

        public async Task<string> AddRoleAsync(AddRoleModel model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user is null || user.Id != model.Userid)
                return "User id or email are incorrect";

            if (!await _roleManager.RoleExistsAsync(model.Role))
                return "Role does not exist";

            if (await _userManager.IsInRoleAsync(user, model.Role))
                return "User already has this role";

            var result = await _userManager.AddToRoleAsync(user, model.Role);

            if (!result.Succeeded)
                return "Role did not add Some thing error";

            return string.Empty;
        }


        public async Task<string> ConfirmEmail(string userId, string token)
        {
            var user = await _userManager.FindByIdAsync(userId);
            token = token.Replace(" ", "+");
            var result = await _userManager.ConfirmEmailAsync(user, token);
            if (!result.Succeeded)
            {
                return "Email not confirmed";
            }
            else
            {
                return string.Empty;
            }

        }

        public async Task<string> ForgotPasswordConfermationModel(ForgotPasswordConfermationModel model)
        {
            var user = await _userManager.FindByIdAsync(model.userId);
            if (model.userId == null || model.token == null)
            {
                return "Link expired";
            }
            else if (user == null)
            {
                return "User not Found";
            }
            model.token = model.token.Replace(" ", "+");
            var result = await _userManager.ResetPasswordAsync(user, model.token, model.confirmPassword);
            if (!result.Succeeded)
            {
                return "Password not reset";
            }
            return string.Empty;
        }


        private async Task<JwtSecurityToken> CreateJwtToken(AppUser user)
        {
            var userClaims = await _userManager.GetClaimsAsync(user);
            var roles = await _userManager.GetRolesAsync(user);
            var roleClaims = new List<Claim>();

            foreach (var role in roles)
                roleClaims.Add(new Claim("roles", role));

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim("uid", user.Id)
            }
            .Union(userClaims)
            .Union(roleClaims);

            var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwt.Key));
            var signingCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);
            var jwtSecurityToken = new JwtSecurityToken(
                issuer: _jwt.Issuer,
                audience: _jwt.Audience,
                claims: claims,
                expires: DateTime.Now.AddDays(_jwt.DurationInDay),
                signingCredentials: signingCredentials);

            return jwtSecurityToken;
        }

        public Task<string> DeleteAccount(string error, string email)
        {
            throw new NotImplementedException();
        }
    }
}
