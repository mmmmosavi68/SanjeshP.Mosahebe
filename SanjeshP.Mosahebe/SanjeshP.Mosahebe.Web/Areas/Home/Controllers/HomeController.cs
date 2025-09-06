using Microsoft.AspNetCore.Mvc;

namespace SanjeshP.Mosahebe.Web.Areas.Home.Controllers
{
    [Area("Home")]
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
