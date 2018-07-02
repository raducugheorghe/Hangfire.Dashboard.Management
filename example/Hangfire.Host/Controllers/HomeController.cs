using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Hangfire.Host.Controllers
{

    public class HomeController : Controller
    {
        public IActionResult Index(string message = null)
        {
            return Redirect("/dashboard");
        }

        [AllowAnonymous]
        public IActionResult Error()
        {
            return View();
        }

        [AllowAnonymous]
        public IActionResult Forbidden()
        {
            return View();
        }

    }
}
