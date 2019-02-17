using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace GamexWeb.Controllers
{
    public class AdminController : Controller
    {
        // GET: Admin
        [HttpGet]
        [Route("Admin/Company/Request")]
        public ActionResult ViewCompanyRequest()
        {
            return View();
        }
    }
}