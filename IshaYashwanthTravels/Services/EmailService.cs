using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using IshaYaswanthTravels.Models;

namespace IshaYaswanthTravels.Services
{
    public class EmailService
    {
        private readonly IConfiguration _config;

        public EmailService(IConfiguration config)
        {
            _config = config;
        }

        public async Task SendContactMail(ContactViewModel model)
        {
            try
            {
                Console.WriteLine("Creating email...");

                var email = new MimeMessage();

                email.From.Add(MailboxAddress.Parse(_config["EmailSettings:SenderEmail"]));
                email.To.Add(MailboxAddress.Parse(_config["EmailSettings:OwnerEmail"]));

                email.Subject = "New Enquiry - Isha Yaswanth Travels";

                email.Body = new TextPart("html")
                {
                    Text = $"Name: {model.Name}<br/>Phone: {model.Phone}"
                };

                using var smtp = new SmtpClient();

                smtp.Timeout = 30000;

                Console.WriteLine("Connecting...");

                await smtp.ConnectAsync(
                    _config["EmailSettings:SmtpServer"],
                    int.Parse(_config["EmailSettings:Port"]),
                    SecureSocketOptions.StartTls);

                Console.WriteLine("Connected.");

                await smtp.AuthenticateAsync(
                    _config["EmailSettings:SenderEmail"],
                    _config["EmailSettings:SenderPassword"]);

                Console.WriteLine("Authenticated.");

                await smtp.SendAsync(email);

                Console.WriteLine("Mail sent.");

                await smtp.DisconnectAsync(true);
            }
            catch (Exception ex)
            {
                Console.WriteLine("EMAIL ERROR:");
                Console.WriteLine(ex.ToString());

                throw;
            }
        }
    }
}