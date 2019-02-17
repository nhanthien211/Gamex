using System;
using System.Collections.Generic;
using System.Web.Mvc;
using GamexService.Interface;
using GamexService.Utilities;

namespace GamexWeb.Controllers
{
    public class HomeController : Controller
    {
        [HttpGet]
        [AllowAnonymous]
        public ActionResult Index()
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("AccountInfo", "Account");
            }
            return View();
        }

        [HttpGet]
        [AllowAnonymous]
        [Route("Register")]
        public ActionResult Register()
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("AccountInfo", "Account");
            }
            return View();
        }
    }
}