using System.Web.Mvc;
using GamexService.Utilities;

namespace GamexWeb.Controllers
{
    public class HomeController : Controller
    {

        public ActionResult Index()
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Account");
            }
            return View();
        }
    }
}