using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace SanjeshP.Mosahebe.Web.Areas.DocumentVerification.Controllers
{
    [Area("DocumentVerification")]
    public class ResearchDocumentByMinistryController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
