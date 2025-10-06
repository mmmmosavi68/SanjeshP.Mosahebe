using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace SanjeshP.Mosahebe.Web.Areas.DocumentVerification.Controllers
{
    [AllowAnonymous]
    [Area("DocumentVerification")]
    public class ResearchDocumentByUnivController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
