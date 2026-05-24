using IshaYaswanthTravels.Models;
using IshaYaswanthTravels.Services;
using Microsoft.AspNetCore.Mvc;

namespace IshaYaswanthTravels.Controllers
{
    public class HomeController : Controller
    {
        private readonly EmailService _emailService;

        public HomeController(EmailService emailService)
        {
            _emailService = emailService;
        }

        public IActionResult Index()
        {
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> Contact(ContactViewModel model)
        {
            await _emailService.SendContactMail(model);

            return Json(new
            {
                success = true
            });
        }
    }
}