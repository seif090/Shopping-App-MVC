using System.Net;
using System.Net.Mail;
using CartApp.Models;
using Microsoft.Extensions.Options;

namespace CartApp.Services
{
    public class EmailService : IEmailService
    {
        private readonly EmailSettings _emailSettings;

        public EmailService(IOptions<EmailSettings> emailSettings)
        {
            _emailSettings =
                emailSettings.Value ?? throw new ArgumentNullException(nameof(emailSettings));
        }

        public async Task SendEmailAsync(string email, string subject, string message)
        {
            if (string.IsNullOrEmpty(email))
                throw new ArgumentNullException(nameof(email));
            if (string.IsNullOrEmpty(subject))
                throw new ArgumentNullException(nameof(subject));
            if (string.IsNullOrEmpty(message))
                throw new ArgumentNullException(nameof(message));

            try
            {
                using var mailMessage = new MailMessage
                {
                    From = new MailAddress(_emailSettings.FromEmail),
                    Subject = subject,
                    Body = message,
                    IsBodyHtml = true,
                };
                mailMessage.To.Add(email);

                using var client = new SmtpClient(
                    _emailSettings.SmtpServer,
                    _emailSettings.SmtpPort
                )
                {
                    Credentials = new NetworkCredential(
                        _emailSettings.FromEmail,
                        _emailSettings.Password
                    ),
                    EnableSsl = _emailSettings.EnableSsl,
                };

                await client.SendMailAsync(mailMessage).ConfigureAwait(false);
            }
            catch (SmtpException ex)
            {
                throw new InvalidOperationException($"Failed to send email: {ex.Message}", ex);
            }
        }
    }
}
