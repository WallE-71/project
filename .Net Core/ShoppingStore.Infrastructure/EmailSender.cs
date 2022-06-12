using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using ShoppingStore.Application.ViewModels.Settings;
using ShoppingStore.Infrastructure.Contracts;

namespace ShoppingStore.Infrastructure
{
    public class EmailSender : IEmailSender
    {
        private readonly IWritableOptions<SiteSettings> _writableLocations;
        public EmailSender(IWritableOptions<SiteSettings> writableLocations)
        {
            _writableLocations = writableLocations;
        }

        public async Task SendEmailAsync(string email, string subject, string message)
        {
            using (var client = new SmtpClient())
            {
                var credential = new NetworkCredential
                {
                    UserName = _writableLocations.Value.EmailSetting.Username,
                    Password = _writableLocations.Value.EmailSetting.Password,
                };

                client.EnableSsl = true;
                client.Credentials = credential;
                client.UseDefaultCredentials = false;
                client.Host = _writableLocations.Value.EmailSetting.Host;
                client.Port = _writableLocations.Value.EmailSetting.Port;

                using (var emailMessage = new MailMessage())
                {
                    emailMessage.Body = message;
                    emailMessage.Subject = subject;
                    emailMessage.IsBodyHtml = true;
                    emailMessage.To.Add(new MailAddress(email));
                    emailMessage.From = new MailAddress(_writableLocations.Value.EmailSetting.Email);

                    client.Send(emailMessage);
                };
                await Task.CompletedTask;
            }
        }
    }
}
