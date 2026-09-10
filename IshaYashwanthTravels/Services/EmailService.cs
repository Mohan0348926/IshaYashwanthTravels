
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace IshaYashwanthTravels.Services
{
    public class EmailService
    {
        private readonly IConfiguration _config;
        private readonly ILogger<EmailService> _logger;

        private static readonly HttpClient _httpClient = new HttpClient();

        public EmailService(
            IConfiguration config,
            ILogger<EmailService> logger)
        {
            _config = config;
            _logger = logger;
        }

        // ============================================================
        // SEND EMAIL THROUGH RESEND
        // ============================================================

        private async Task SendAsync(
            string toEmail,
            string subject,
            string htmlBody)
        {
            if (string.IsNullOrWhiteSpace(toEmail))
            {
                throw new Exception("Recipient email address is empty.");
            }

            var apiKey = _config["Resend:ApiKey"];
            var senderEmail = _config["EmailSettings:SenderEmail"];
            var senderName = _config["EmailSettings:SenderName"];

            if (string.IsNullOrWhiteSpace(apiKey))
            {
                throw new Exception("Resend API key is missing.");
            }

            if (string.IsNullOrWhiteSpace(senderEmail))
            {
                throw new Exception("Sender email is missing.");
            }

            var payload = new
            {
                from = $"{senderName} <{senderEmail}>",
                to = new[] { toEmail },
                subject = subject,
                html = htmlBody
            };

            using var request = new HttpRequestMessage(
                HttpMethod.Post,
                "https://api.resend.com/emails");

            request.Content = JsonContent.Create(payload);

            request.Headers.Authorization =
                new AuthenticationHeaderValue(
                    "Bearer",
                    apiKey);

            var response = await _httpClient.SendAsync(request);

            var responseBody =
                await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception(
                    $"Resend API error ({(int)response.StatusCode} {response.StatusCode}): {responseBody}");
            }

            _logger.LogInformation(
                "Email sent successfully to {Email}. Resend response: {Response}",
                toEmail,
                responseBody);
        }


        // ============================================================
        // SEND BOOKING EMAILS
        // ============================================================

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
            var ownerEmail =
                _config["EmailSettings:OwnerEmail"];


            // ========================================================
            // OWNER EMAIL HTML
            // ========================================================

            var ownerHtml = $@"
<!DOCTYPE html>
<html>
<head>
    <meta charset=""UTF-8"">
    <title>New Trip Booking</title>
</head>

<body style=""margin:0;padding:20px;background:#f5f5f5;font-family:Arial,sans-serif;"">

<div style=""max-width:600px;margin:auto;background:#ffffff;border:1px solid #e0e0e0;border-radius:10px;overflow:hidden;"">

    <div style=""background:#0b5ed7;padding:22px;text-align:center;"">

        <h2 style=""color:#ffffff;margin:0;"">
            🚖 New Trip Booking
        </h2>

        <p style=""color:#dbe8ff;margin:6px 0 0;"">
            Isha Yaswanth Travels
        </p>

    </div>


    <div style=""padding:25px;color:#333333;"">

        <table style=""width:100%;border-collapse:collapse;"">

            <tr>
                <td style=""padding:9px 0;font-weight:bold;width:160px;"">
                    Name
                </td>

                <td style=""padding:9px 0;"">
                    {name}
                </td>
            </tr>


            <tr>
                <td style=""padding:9px 0;font-weight:bold;"">
                    Email
                </td>

                <td style=""padding:9px 0;"">
                    {email}
                </td>
            </tr>


            <tr>
                <td style=""padding:9px 0;font-weight:bold;"">
                    Phone
                </td>

                <td style=""padding:9px 0;"">
                    {phone}
                </td>
            </tr>


            <tr>
                <td style=""padding:9px 0;font-weight:bold;"">
                    Travel Date
                </td>

                <td style=""padding:9px 0;"">
                    {travelDate}
                </td>
            </tr>


            <tr>
                <td style=""padding:9px 0;font-weight:bold;"">
                    From
                </td>

                <td style=""padding:9px 0;"">
                    {from}
                </td>
            </tr>


            <tr>
                <td style=""padding:9px 0;font-weight:bold;"">
                    To
                </td>

                <td style=""padding:9px 0;"">
                    {to}
                </td>
            </tr>


            <tr>
                <td style=""padding:9px 0;font-weight:bold;"">
                    Passengers
                </td>

                <td style=""padding:9px 0;"">
                    {passengers}
                </td>
            </tr>


            <tr>
                <td style=""padding:9px 0;font-weight:bold;"">
                    Trip Type
                </td>

                <td style=""padding:9px 0;"">
                    {tripType}
                </td>
            </tr>


            <tr>
                <td style=""padding:9px 0;font-weight:bold;"">
                    Vehicle
                </td>

                <td style=""padding:9px 0;"">
                    {vehicleType}
                </td>
            </tr>


            <tr>
                <td style=""padding:9px 0;font-weight:bold;vertical-align:top;"">
                    Additional Requirements
                </td>

                <td style=""padding:9px 0;"">
                    {(string.IsNullOrWhiteSpace(message) ? "None" : message)}
                </td>
            </tr>

        </table>

    </div>


    <div style=""background:#f5f5f5;padding:14px;text-align:center;font-size:12px;color:#888888;"">

        Isha Yaswanth Travels — Booking Notification

    </div>

</div>

</body>
</html>";


            // ========================================================
            // CUSTOMER EMAIL HTML
            // ========================================================

            var customerHtml = $@"
<!DOCTYPE html>
<html>
<head>
    <meta charset=""UTF-8"">
    <title>Isha Yaswanth Travels</title>
</head>

<body style=""margin:0;padding:20px;background:#f5f5f5;font-family:Arial,sans-serif;"">

<div style=""max-width:600px;margin:auto;background:#ffffff;border:1px solid #e0e0e0;border-radius:10px;overflow:hidden;"">

    <div style=""background:#0b5ed7;padding:25px;text-align:center;"">

        <h2 style=""color:#ffffff;margin:0;"">
            Isha Yaswanth Travels
        </h2>

        <p style=""color:#dbe8ff;margin:6px 0 0;"">
            We've received your enquiry 🚐
        </p>

    </div>


    <div style=""padding:25px;color:#333333;line-height:1.6;"">

        <p>
            Dear <strong>{name}</strong>,
        </p>


        <p>
            Thank you for contacting
            <strong>Isha Yaswanth Travels</strong>.
            We've received your booking enquiry and our team
            will contact you shortly.
        </p>


        <div style=""background:#f5f8ff;border-left:4px solid #0b5ed7;padding:16px;margin:20px 0;border-radius:4px;"">

            <p style=""margin:0 0 6px;"">
                <strong>Travel Date:</strong>
                {travelDate}
            </p>

            <p style=""margin:6px 0;"">
                <strong>From:</strong>
                {from}
            </p>

            <p style=""margin:6px 0;"">
                <strong>To:</strong>
                {to}
            </p>

            <p style=""margin:6px 0;"">
                <strong>Passengers:</strong>
                {passengers}
            </p>

            <p style=""margin:6px 0;"">
                <strong>Trip Type:</strong>
                {tripType}
            </p>

            <p style=""margin:6px 0;"">
                <strong>Vehicle Type:</strong>
                {vehicleType}
            </p>

            <p style=""margin:6px 0 0;"">
                <strong>Additional Requirements:</strong>
                {(string.IsNullOrWhiteSpace(message) ? "None" : message)}
            </p>

        </div>


        <p>
            If you have any urgent questions,
            please feel free to contact us directly.
        </p>


        <p style=""margin-top:25px;"">

            Regards,<br/>

            <strong>Ramesh</strong><br/>

            Isha Yaswanth Travels<br/>

            📞 9113908477

        </p>

    </div>


    <div style=""background:#f5f5f5;padding:14px;text-align:center;font-size:12px;color:#888888;"">

        © 2026 Isha Yaswanth Travels

    </div>

</div>

</body>
</html>";


            // ========================================================
            // 1. SEND OWNER EMAIL
            // ========================================================

            await SendAsync(
                ownerEmail,
                "New Trip Booking - " + name,
                ownerHtml
            );


            // ========================================================
            // 2. SEND CUSTOMER EMAIL
            // ========================================================

            try
            {
                await SendAsync(
                    email,
                    "We Received Your Enquiry - Isha Yaswanth Travels",
                    customerHtml
                );
            }
            catch (Exception ex)
            {
                // Customer email failure should NOT make
                // the booking itself fail because the owner
                // has already received the booking.

                _logger.LogError(
                    ex,
                    "Customer confirmation email failed. Customer: {CustomerEmail}",
                    email
                );
            }
        }
    }
}

