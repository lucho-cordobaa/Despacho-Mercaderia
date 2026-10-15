using Microsoft.AspNetCore.Mvc;

namespace DespachoApp.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index() => View();
        public IActionResult AcercaDe() => View();
    }
}
