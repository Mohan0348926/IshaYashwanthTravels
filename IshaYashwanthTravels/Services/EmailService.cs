using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace IshaYashwanthTravels.Services
{
    public class EmailService
    {
        private readonly IConfiguration _config;

        public EmailService(IConfiguration config)
        {
            _config = config;
        }

        private async Task SendAsync(string toEmail, string subject, string body)
        {
            var smtpServer = _config["EmailSettings:SmtpServer"];
            var smtpPort = int.Parse(_config["EmailSettings:SmtpPort"]);
            var senderEmail = _config["EmailSettings:SenderEmail"];
            var senderPassword = _config["EmailSettings:SenderPassword"];
            var senderName = _config["EmailSettings:SenderName"];

            using var client = new SmtpClient(smtpServer, smtpPort)
            {
                Credentials = new NetworkCredential(senderEmail, senderPassword),
                EnableSsl = true
            };

            var mail = new MailMessage
            {
                From = new MailAddress(senderEmail, senderName),
                Subject = subject,
                Body = body
            };
            mail.To.Add(toEmail);

            await client.SendMailAsync(mail);
        }

        public async Task SendBookingEmailsAsync(string name, string email, string phone, string vehicleType, string message)
        {
            var ownerEmail = _config["EmailSettings:OwnerEmail"];

            var ownerTask = SendAsync(
                ownerEmail,
                "New Trip Booking - " + name,
                $"Name: {name}\nEmail: {email}\nPhone: {phone}\nVehicle Type: {vehicleType}\nMessage: {message}"
            );

            var customerTask = SendAsync(
                email,
                "We Received Your Enquiry - Isha Yaswanth Travels",
                $"Dear {name},\n\nThank you for contacting Isha Yaswanth Travels. We will contact you soon.\n\nRegards,\nRamesh\nIsha Yaswanth Travels\n9113908477"
            );

            await Task.WhenAll(ownerTask, customerTask);
        }
    }
}