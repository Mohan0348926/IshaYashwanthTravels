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
            var email = new MimeMessage();

            email.From.Add(MailboxAddress.Parse(_config["EmailSettings:SenderEmail"]));
            email.To.Add(MailboxAddress.Parse(_config["EmailSettings:OwnerEmail"]));

            email.Subject = "New Enquiry - Isha Yaswanth Travels";

            email.Body = new TextPart("html")
            {
                Text = $@"
                <h2>New Enquiry</h2>
                <p><b>Name:</b> {model.Name}</p>
                <p><b>Company:</b> {model.Company}</p>
                <p><b>Phone:</b> {model.Phone}</p>
                <p><b>Email:</b> {model.Mail}</p>
                <p><b>Message:</b> {model.Message}</p>"
            };

            using var smtp = new SmtpClient();

            smtp.Timeout = 120000; // 2 minutes

            await smtp.ConnectAsync(
                _config["EmailSettings:SmtpServer"],
                int.Parse(_config["EmailSettings:Port"]),
                SecureSocketOptions.StartTls);

            await smtp.AuthenticateAsync(
                _config["EmailSettings:SenderEmail"],
                _config["EmailSettings:SenderPassword"]);

            await smtp.SendAsync(email);

            await smtp.DisconnectAsync(true);
        }
    }
}