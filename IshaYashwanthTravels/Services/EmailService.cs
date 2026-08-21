using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace IshaYashwanthTravels.Services
{
    public class EmailService
    {
        private readonly IConfiguration _config;
        private static readonly HttpClient _httpClient = new HttpClient();

        public EmailService(IConfiguration config)
        {
            _config = config;
        }

        private async Task SendAsync(string toEmail, string subject, string body)
        {
            var apiKey = _config["Resend:ApiKey"];
            var senderEmail = _config["EmailSettings:SenderEmail"];
            var senderName = _config["EmailSettings:SenderName"];

            var payload = new
            {
                from = $"{senderName} <{senderEmail}>",
                to = new[] { toEmail },
                subject = subject,
                text = body
            };

            var request = new HttpRequestMessage(HttpMethod.Post, "https://api.resend.com/emails")
            {
                Content = JsonContent.Create(payload)
            };
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);

            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();
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
