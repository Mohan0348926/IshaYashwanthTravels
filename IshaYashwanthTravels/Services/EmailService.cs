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

        private async Task SendAsync(string toEmail, string subject, string htmlBody)
        {
            var apiKey = _config["Resend:ApiKey"];
            var senderEmail = _config["EmailSettings:SenderEmail"];
            var senderName = _config["EmailSettings:SenderName"];

            var payload = new
            {
                from = $"{senderName} <{senderEmail}>",
                to = new[] { toEmail },
                subject = subject,
                html = htmlBody
            };

            var request = new HttpRequestMessage(HttpMethod.Post, "https://api.resend.com/emails")
            {
                Content = JsonContent.Create(payload)
            };
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);

            var response = await _httpClient.SendAsync(request);
            if (!response.IsSuccessStatusCode)
            {
                var errorBody = await response.Content.ReadAsStringAsync();
                throw new Exception($"Resend API error ({response.StatusCode}): {errorBody}");
            }
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

    var ownerHtml = $@"
<div style=""font-family: Arial, sans-serif; max-width: 600px; margin: auto; border: 1px solid #e0e0e0; border-radius: 8px; overflow: hidden;"">

  <div style=""background: #0b5ed7; padding: 20px; text-align: center;"">
    <h2 style=""color: #ffffff; margin: 0;"">🚖 New Trip Booking</h2>
  </div>

  <div style=""padding: 24px; color: #333;"">

    <table style=""width: 100%; border-collapse: collapse;"">

      <tr>
        <td style=""padding: 8px 0; font-weight: bold; width: 150px;"">Name:</td>
        <td style=""padding: 8px 0;"">{name}</td>
      </tr>

      <tr>
        <td style=""padding: 8px 0; font-weight: bold;"">Email:</td>
        <td style=""padding: 8px 0;"">{email}</td>
      </tr>

      <tr>
        <td style=""padding: 8px 0; font-weight: bold;"">Phone:</td>
        <td style=""padding: 8px 0;"">{phone}</td>
      </tr>

      <tr>
        <td style=""padding: 8px 0; font-weight: bold;"">Travel Date:</td>
        <td style=""padding: 8px 0;"">{travelDate}</td>
      </tr>

      <tr>
        <td style=""padding: 8px 0; font-weight: bold;"">From:</td>
        <td style=""padding: 8px 0;"">{from}</td>
      </tr>

      <tr>
        <td style=""padding: 8px 0; font-weight: bold;"">To:</td>
        <td style=""padding: 8px 0;"">{to}</td>
      </tr>

      <tr>
        <td style=""padding: 8px 0; font-weight: bold;"">Passengers:</td>
        <td style=""padding: 8px 0;"">{passengers}</td>
      </tr>

      <tr>
        <td style=""padding: 8px 0; font-weight: bold;"">Trip Type:</td>
        <td style=""padding: 8px 0;"">{tripType}</td>
      </tr>

      <tr>
        <td style=""padding: 8px 0; font-weight: bold;"">Vehicle:</td>
        <td style=""padding: 8px 0;"">{vehicleType}</td>
      </tr>

      <tr>
        <td style=""padding: 8px 0; font-weight: bold; vertical-align: top;"">
            Additional Requirements:
        </td>
        <td style=""padding: 8px 0;"">{message}</td>
      </tr>

    </table>

  </div>

  <div style=""background: #f5f5f5; padding: 12px; text-align: center; font-size: 12px; color: #888;"">
    Isha Yaswanth Travels — Booking Notification
  </div>

</div>";


    var customerHtml = $@"
<div style=""font-family: Arial, sans-serif; max-width: 600px; margin: auto; border: 1px solid #e0e0e0; border-radius: 8px; overflow: hidden;"">

  <div style=""background: #0b5ed7; padding: 24px; text-align: center;"">
    <h2 style=""color: #ffffff; margin: 0;"">Isha Yaswanth Travels</h2>

    <p style=""color: #dbe8ff; margin: 4px 0 0;"">
        We've received your enquiry 🚐
    </p>
  </div>

  <div style=""padding: 24px; color: #333; line-height: 1.6;"">

    <p>Dear <strong>{name}</strong>,</p>

    <p>
        Thank you for contacting
        <strong>Isha Yaswanth Travels</strong>.
        We've received your enquiry and our team will get back to you shortly.
    </p>

    <div style=""background: #f5f8ff; border-left: 4px solid #0b5ed7; padding: 16px; margin: 16px 0; border-radius: 4px;"">

        <p style=""margin: 0;"">
            <strong>Travel Date:</strong> {travelDate}
        </p>

        <p style=""margin: 4px 0 0;"">
            <strong>From:</strong> {from}
        </p>

        <p style=""margin: 4px 0 0;"">
            <strong>To:</strong> {to}
        </p>

        <p style=""margin: 4px 0 0;"">
            <strong>Passengers:</strong> {passengers}
        </p>

        <p style=""margin: 4px 0 0;"">
            <strong>Trip Type:</strong> {tripType}
        </p>

        <p style=""margin: 4px 0 0;"">
            <strong>Vehicle Type:</strong> {vehicleType}
        </p>

        <p style=""margin: 4px 0 0;"">
            <strong>Additional Requirements:</strong> {message}
        </p>

    </div>

    <p>
        If you have any urgent questions, feel free to call us directly.
    </p>

    <p style=""margin-top: 24px;"">
        Regards,<br/>
        <strong>Ramesh</strong><br/>
        Isha Yaswanth Travels<br/>
        📞 9113908477
    </p>

  </div>

  <div style=""background: #f5f5f5; padding: 12px; text-align: center; font-size: 12px; color: #888;"">
    © Isha Yaswanth Travels
  </div>

</div>";


    // Send owner email first
    await SendAsync(
        ownerEmail,
        "New Trip Booking - " + name,
        ownerHtml
    );

    // Then send customer confirmation
    await SendAsync(
        email,
        "We Received Your Enquiry - Isha Yaswanth Travels",
        customerHtml
    );
}
    }
}
