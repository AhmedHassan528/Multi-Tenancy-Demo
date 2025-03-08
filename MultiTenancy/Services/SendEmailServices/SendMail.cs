
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

        public async Task<string> SendEmailAsync(string emailTo, string subject, string? token, string controllerName, string? ReqUrl)
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

                var confirmationLink = "";
                if (!string.IsNullOrEmpty(ReqUrl))
                {
                    confirmationLink = $"{ReqUrl}/ConfirmEmail?UserId={user.Id}&Token={token}";
                }
                else
                {
                    confirmationLink = $"{ReqUrl}/ConfirmEmail?UserId={user.Id}&Token={token}";
                }
                builder.HtmlBody = 
                    $@"
                    <html>
                    <body style='font-family: Arial, sans-serif;'>
                        <h2>{subject}</h2>
                        <p>Dear {user.FirstName} {user.LastName},</p>
                        <p>Thank you for signing up! Please confirm your email address by clicking the button below:</p>
                        <p>
                            <a href='{confirmationLink}' 
                               style='background-color: #007bff; color: white; padding: 10px 20px; text-decoration: none; border-radius: 5px;'>
                                Confirm Email
                            </a>
                        </p>
                        <p>If the button doesn't work, copy and paste the following link into your browser:</p>
                        <p><a href='{confirmationLink}'>{confirmationLink}</a></p>
                        <br>
                        <p>If you didn’t request this, please ignore this email.</p>
                        <p>Best regards,<br><strong>Ahmed Hassan</strong></p>
                    </body>
                    </html>";
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
