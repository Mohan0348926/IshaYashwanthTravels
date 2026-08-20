using Microsoft.AspNetCore.Mvc;
using IshaYashwanthTravels.Services;
using System;
using System.Threading.Tasks;

namespace IshaYashwanthTravels.Controllers
{
    public class HomeController : Controller
    {
        private readonly EmailService _emailService;
        private readonly ILogger<HomeController> _logger;

        public HomeController(EmailService emailService, ILogger<HomeController> logger)
        {
            _emailService = emailService;
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult SubmitBooking(string Name, string Email, string Phone, string VehicleType, string Message)
        {
            if (string.IsNullOrWhiteSpace(Name) || string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Phone))
            {
                return Json(new { success = false, message = "Please fill all required fields." });
            }

            // Fire-and-forget: don't make the customer wait for SMTP round trip
            _ = Task.Run(async () =>
            {
                try
                {
                    await _emailService.SendBookingEmailsAsync(Name, Email, Phone, VehicleType, Message);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to send booking emails for {Name}", Name);
                }
            });

            return Json(new { success = true, message = "Thank you! We will contact you soon." });
        }
    }
}