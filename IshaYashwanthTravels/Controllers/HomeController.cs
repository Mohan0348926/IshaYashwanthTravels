using Microsoft.AspNetCore.Mvc;

namespace IshaYashwanthTravels.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
