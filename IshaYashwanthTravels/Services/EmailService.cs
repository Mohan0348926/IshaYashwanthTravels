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

        public async Task SendBookingEmailsAsync(
      string name,
      string email,
      string phone,
      string travelDate,
      string from,
      string to,
      string passengers,
      string tripType,
      string vehicleType,
      string message)
        {
            var ownerEmail = _config["EmailSettings:OwnerEmail"];

            var ownerTask = SendAsync(
                ownerEmail,
                "New Trip Booking - " + name,
                $@"NEW TRIP BOOKING

Name: {name}
Email: {email}
Phone: {phone}

TRAVEL DETAILS
-------------------------
Travel Date: {travelDate}
From: {from}
To: {to}
Passengers: {passengers}
Trip Type: {tripType}
Vehicle Type: {vehicleType}

Additional Requirements:
{message}
"
            );

            var customerTask = SendAsync(
                email,
                "We Received Your Enquiry - Isha Yaswanth Travels",
                $@"Dear {name},

Thank you for contacting Isha Yaswanth Travels.

We have received your travel booking enquiry.

Travel Details
-------------------------
Travel Date: {travelDate}
From: {from}
To: {to}
Passengers: {passengers}
Trip Type: {tripType}
Vehicle Type: {vehicleType}

We will contact you soon to confirm the details.

Regards,
Ramesh
Isha Yaswanth Travels
9113908477"
            );

            await Task.WhenAll(ownerTask, customerTask);
        }
    }
}