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
<div style='font-family:Segoe UI;
            padding:30px;
            background:#f4f6f9;'>

    <div style='max-width:750px;
                margin:auto;
                background:white;
                border-radius:20px;
                overflow:hidden;
                box-shadow:0 10px 30px rgba(0,0,0,0.12);'>

        <div style='background:linear-gradient(90deg,#0d6efd,#6610f2);
                    color:white;
                    padding:35px 25px;
                    text-align:center;'>

            <h1 style='margin:0;
                       font-size:32px;
                       font-weight:700;'>

                ✈ Isha Yaswanth Travels
            </h1>

            <p style='margin-top:15px;
                      font-size:17px;
                      opacity:.95;'>

                New Customer Enquiry
            </p>
        </div>

        <div style='padding:40px 35px;'>

            <table style='width:100%;
                          border-collapse:separate;
                          border-spacing:0 12px;
                          font-size:16px;'>

                <tr>
                    <td style='padding:14px 18px;
                               font-weight:600;
                               width:220px;
                               background:#f8f9fa;
                               border-radius:10px 0 0 10px;'>

                        Customer Name
                    </td>

                    <td style='padding:14px 18px;
                               background:#fcfcfc;
                               border-radius:0 10px 10px 0;'>

                        {model.Name}
                    </td>
                </tr>

                <tr>
                    <td style='padding:14px 18px;
                               font-weight:600;
                               background:#f8f9fa;
                               border-radius:10px 0 0 10px;'>

                        Company
                    </td>

                    <td style='padding:14px 18px;
                               background:#fcfcfc;
                               border-radius:0 10px 10px 0;'>

                        {model.Company}
                    </td>
                </tr>

                <tr>
                    <td style='padding:14px 18px;
                               font-weight:600;
                               background:#f8f9fa;
                               border-radius:10px 0 0 10px;'>

                        Phone Number
                    </td>

                    <td style='padding:14px 18px;
                               background:#fcfcfc;
                               border-radius:0 10px 10px 0;'>

                        {model.Phone}
                    </td>
                </tr>

                <tr>
                    <td style='padding:14px 18px;
                               font-weight:600;
                               background:#f8f9fa;
                               border-radius:10px 0 0 10px;'>

                        Email
                    </td>

                    <td style='padding:14px 18px;
                               background:#fcfcfc;
                               border-radius:0 10px 10px 0;'>

                        {model.Mail}
                    </td>
                </tr>

            </table>

            <div style='margin-top:35px;'>

                <h3 style='margin-bottom:15px;
                           color:#111827;
                           font-size:20px;'>

                    Customer Message
                </h3>

                <div style='padding:25px;
                            background:#f8fbff;
                            border:1px solid #dbeafe;
                            border-left:6px solid #0d6efd;
                            border-radius:15px;
                            line-height:1.9;
                            font-size:16px;
                            color:#374151;
                            word-break:break-word;'>

                    {model.Message}
                </div>

            </div>

            <div style='margin-top:35px;
                        padding:22px;
                        background:#eef4ff;
                        border-left:6px solid #0d6efd;
                        border-radius:15px;'>

                <p style='margin:0;
                          font-size:15px;
                          color:#374151;
                          line-height:1.8;'>

                    A customer has shown interest in your travel services.
                    Please contact them as soon as possible for better engagement.
                </p>
            </div>

        </div>

        <div style='background:#111827;
                    color:white;
                    text-align:center;
                    padding:25px 20px;
                    font-size:14px;
                    line-height:1.8;'>

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