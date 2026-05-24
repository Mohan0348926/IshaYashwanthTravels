using IshaYaswanthTravels.Models;
using MailKit.Net.Smtp;
using MimeKit;

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
<div style='margin:0;padding:20px;background:#f4f6f9;font-family:Arial,Segoe UI,sans-serif;'>

    <div style='max-width:650px;margin:0 auto;background:#ffffff;border-radius:16px;overflow:hidden;'>

        <div style='background:#0d6efd;color:#ffffff;text-align:center;padding:28px 20px;'>
            <h2 style='margin:0;font-size:26px;'>✈ Isha Yaswanth Travels</h2>
            <p style='margin:10px 0 0;font-size:15px;'>New Customer Enquiry</p>
        </div>

        <div style='padding:25px;'>

            <div style='margin-bottom:16px;padding:16px;background:#f8f9fa;border-radius:12px;'>
                <strong>Name:</strong>
                <div style='margin-top:6px;'>{model.Name}</div>
            </div>

            <div style='margin-bottom:16px;padding:16px;background:#f8f9fa;border-radius:12px;'>
                <strong>Company:</strong>
                <div style='margin-top:6px;'>{model.Company}</div>
            </div>

            <div style='margin-bottom:16px;padding:16px;background:#f8f9fa;border-radius:12px;'>
                <strong>Phone:</strong>
                <div style='margin-top:6px;'>{model.Phone}</div>
            </div>

            <div style='margin-bottom:16px;padding:16px;background:#f8f9fa;border-radius:12px;'>
                <strong>Email:</strong>
                <div style='margin-top:6px;'>{model.Mail}</div>
            </div>

            <div style='margin-top:22px;padding:20px;background:#eef4ff;border-left:5px solid #0d6efd;border-radius:12px;'>
                <strong style='font-size:17px;'>Message:</strong>
                <div style='margin-top:12px;line-height:1.8;word-break:break-word;'>
                    {model.Message}
                </div>
            </div>

        </div>

        <div style='background:#111827;color:white;text-align:center;padding:20px;font-size:14px;line-height:1.7;'>
            © 2026 Isha Yaswanth Travels <br/>
            Safe Journey • Trusted Service • Best Promotions
        </div>

    </div>

</div>"
            };
            using var smtp = new SmtpClient();
            await smtp.ConnectAsync(
                _config["EmailSettings:SmtpServer"],
                Convert.ToInt32(_config["EmailSettings:Port"]),
                MailKit.Security.SecureSocketOptions.StartTls);

            await smtp.AuthenticateAsync(
                _config["EmailSettings:SenderEmail"],
                _config["EmailSettings:SenderPassword"]);

            await smtp.SendAsync(email);
            await smtp.DisconnectAsync(true);
        }
    }
}