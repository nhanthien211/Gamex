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
                return RedirectToAction("AccountInfo", "Account");
            }
            return View();
        }
    }
}