using Microsoft.AspNetCore.Mvc;

namespace NineERP.Web.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index(string slug)
        {
            TempData["slug"] = slug;
            return View();
        }
    }
}
