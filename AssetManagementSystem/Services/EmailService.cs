using AssetManagementSystem.DTO;
using System.Net.Mail;
using System.Net;
using Microsoft.Extensions.Options;
using System.Net.Security;

namespace AssetManagementSystem.Services
{
    public class EmailService: IEmailService
    {
        private readonly EmailSettings _emailSettings;

        public EmailService(IOptions<EmailSettings> emailSettings)
        {
            _emailSettings = emailSettings.Value;
        }
        public async Task SendEmailAsync(string to, string subject, string body)
        {
            //ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            using var client = new SmtpClient(_emailSettings.SmtpServer, _emailSettings.SmtpPort)
            {
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false, // ensure we don't fallback to Windows creds
                Credentials = new NetworkCredential(
                    _emailSettings.Username,
                    _emailSettings.Password
                ),
                EnableSsl = true, // ensures STARTTLS is attempted
                Timeout = 20000,
                Host = _emailSettings.SmtpServer
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress(_emailSettings.SenderEmail, _emailSettings.SenderName),
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            };

            mailMessage.To.Add(to);

            try
            {
                await client.SendMailAsync(mailMessage);
                Console.WriteLine($"[SUCCESS] Email sent to {to}");
            }
            catch (SmtpException ex)
            {
                Console.WriteLine($"[SMTP ERROR] {ex.StatusCode}: {ex.Message}");
                throw;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] {ex.Message}");
                throw;
            }
        }



    }

}
