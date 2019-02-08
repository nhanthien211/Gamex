using System.Web.Mvc;
using GamexService.ViewModel;

namespace GamexWeb.Controllers
{
    public class HomeController : Controller
    {

        public ActionResult Index()
        {
//            if (model == null)
//            {
//                model = new LoginViewModel();
//            }
            return View();
        }
    }
}