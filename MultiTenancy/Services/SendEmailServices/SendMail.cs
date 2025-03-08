
using Authentication_With_JWT.Setting;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;

namespace Authentication_With_JWT.Services
{
    public class SendMail : ISendMail
    {
        private readonly MailSettings _mailSetting;
        private readonly UserManager<AppUser> _userManager;


        public SendMail(IOptions<MailSettings> mailSetting, UserManager<AppUser> userManager)
        {
            _mailSetting = mailSetting.Value;
            _userManager = userManager;

        }

        public async Task<string> SendEmailAsync(string emailTo, string subject, string? token, string controllerName)
        {

            try
            {
                var email = new MimeMessage
                {
                    Sender = MailboxAddress.Parse(_mailSetting.Email),
                    Subject = subject
                };

                email.To.Add(MailboxAddress.Parse(emailTo));

                var builder = new BodyBuilder();

                // body are
                var user = await _userManager.FindByEmailAsync(emailTo);
                if (user is null)
                    return "Email is incorrect";


                if (string.IsNullOrEmpty(token))
                {
                    token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                }
                var confirmationLink = $"https://localhost:7241/api/Auth/{controllerName}?UserId={user.Id}&Token={token}";
                builder.HtmlBody = confirmationLink;
                email.Body = builder.ToMessageBody();

                email.From.Add(new MailboxAddress(_mailSetting.DisplayName, _mailSetting.Email));


                using var smtp = new SmtpClient();
                smtp.Connect(_mailSetting.Host, _mailSetting.Port, SecureSocketOptions.SslOnConnect);
                smtp.Authenticate(_mailSetting.Email, _mailSetting.Password);
                await smtp.SendAsync(email);
                smtp.Disconnect(true);
                return string.Empty;
            }
            catch (Exception ex)
            {
                return ex.Message.ToString();
            }

        }
    }
}
