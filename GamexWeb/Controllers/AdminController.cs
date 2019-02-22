using System;
using System.Linq;
using GamexEntity.Constant;
using System.Web.Mvc;
using GamexService.Interface;
using GamexWeb.Identity;
using Microsoft.AspNet.Identity;

namespace GamexWeb.Controllers
{
    public class AdminController : Controller
    {
        private readonly ICompanyService _companyService;
        private readonly IAccountService _accountService;
        private readonly IIdentityMessageService _emailService;
        private readonly ApplicationUserManager _userManager;

        public AdminController(ICompanyService companyService, IAccountService accountService, IIdentityMessageService emailService, ApplicationUserManager userManager)
        {
            _companyService = companyService;
            _emailService = emailService;
            _accountService = accountService;
            _userManager = userManager;
        }

        // GET: Admin
        [HttpGet]
        [Route("Admin/Company/Request")]
        [Authorize(Roles = UserRole.Admin)]
        public ActionResult CompanyRequest()
        {
            return View();
        }

        [HttpPost]
        [Route("Admin/LoadCompanyRequest")]
        [Authorize(Roles = UserRole.Admin)]
        public ActionResult LoadCompanyRequest()
        {
            var draw = Request.Form.GetValues("draw").FirstOrDefault();
            var start = Request.Form.GetValues("start").FirstOrDefault();
            var length = Request.Form.GetValues("length").FirstOrDefault();           
            var sortColumnDirection = Request.Form.GetValues("order[0][dir]").FirstOrDefault();
            var searchValue = Request.Form.GetValues("search[value]").FirstOrDefault();
            var take = length != null ? Convert.ToInt32(length) : 0;
            var skip = start != null ? Convert.ToInt32(start) : 0;
            

            var data = _companyService.LoadCompanyJoinRequestDataTable(sortColumnDirection, searchValue, skip, take);
            var recordsTotal = data.Count;

            return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data });
        }

        [HttpPost]
        [Authorize(Roles = UserRole.Admin)]
        public ActionResult ApproveOrDeny(int companyId, bool isApproved, string email)
        {
            _companyService.ApproveOrRejectCompanyRequest(companyId, isApproved);
            if (isApproved)
            {
                _emailService.Send(new IdentityMessage
                {
                    Destination = email,
                    Subject = "[INFO] COMPANY STATUS",
                    Body = "Your company join request has been approved. We will send you sign in information later"
                });
            }
            else
            {
                _emailService.Send(new IdentityMessage
                {
                    Destination = email,
                    Subject = "[INFO] COMPANY STATUS",
                    Body = "Your company join request has been rejected due to malicious information"
                });
            }
            return RedirectToAction("CompanyRequest", "Admin");
        }

        [HttpGet]
        [Route("Admin/Company/List")]
        [Authorize(Roles = UserRole.Admin)]
        public ActionResult CompanyList()
        {
            return View();
        }

        [HttpPost]
        [Route("Admin/LoadCompanyList")]
        [Authorize(Roles = UserRole.Admin)]
        public ActionResult LoadCompanyList()
        {
            var draw = Request.Form.GetValues("draw").FirstOrDefault();
            var start = Request.Form.GetValues("start").FirstOrDefault();
            var length = Request.Form.GetValues("length").FirstOrDefault();
            var sortColumnDirection = Request.Form.GetValues("order[0][dir]").FirstOrDefault();
            var searchValue = Request.Form.GetValues("search[value]").FirstOrDefault();
            var take = length != null ? Convert.ToInt32(length) : 0;
            var skip = start != null ? Convert.ToInt32(start) : 0;
            

            var data = _companyService.LoadCompanyDataTable(sortColumnDirection, searchValue, skip, take);
            var recordsTotal = data.Count;

            return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data });
        }

        
    }
}