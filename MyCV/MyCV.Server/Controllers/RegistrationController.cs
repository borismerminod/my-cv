using Microsoft.AspNetCore.Mvc;

namespace MyCV.Server.Controllers
{
    public class RegistrationController : Controller
    {
        public IActionResult Register()
        {
            return View();
        }
    }
}
