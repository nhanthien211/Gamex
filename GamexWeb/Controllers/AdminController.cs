using GamexEntity.Constant;
using GamexService.Interface;
using GamexWeb.Identity;
using GamexWeb.Utilities;
using Microsoft.AspNet.Identity;
using System;
using System.Linq;
using System.Web.Mvc;
using GamexEntity.Enumeration;

namespace GamexWeb.Controllers
{
    public class AdminController : Controller
    {
        private readonly IIdentityMessageService _emailService;
        private readonly IAdminService _adminService;
        private readonly ApplicationUserManager _userManager;
        

        public AdminController(IIdentityMessageService emailService, IAdminService adminService, ApplicationUserManager userManager)
        {
            _emailService = emailService;
            _adminService = adminService;
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
            

            var data = _adminService.LoadCompanyJoinRequestDataTable(sortColumnDirection, searchValue, skip, take);
            var recordsTotal = data.Count;

            return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data });
        }

        [HttpPost]
        [Authorize(Roles = UserRole.Admin)]
        public ActionResult ApproveOrDeny(int companyId, bool isApproved)
        {
            var userid = "";
            var result = _adminService.ApproveOrRejectCompanyRequest(companyId, isApproved, ref userid);

            if (result)
            {
                var user = _userManager.FindById(userid);
                if (isApproved)
                {
                    //add password
                    var randomPassword = MyUtilities.GenerateRandomPassword();
                    user.StatusId = (int) AccountStatusEnum.Active;
                    _userManager.AddPassword(userid, randomPassword);
                    _userManager.Update(user);
                    

                    _emailService.Send(new IdentityMessage
                    {
                        Destination = user.Email,
                        Subject = "[INFO] COMPANY STATUS",
                        Body = "Your company join request has been approved. " +
                               "Please login with the following information: " +
                               "Username/Email: " + user.Email + " " +
                               "Password: " + randomPassword
                    });
                }
                else
                {
                    _emailService.Send(new IdentityMessage
                    {
                        Destination = user.Email,
                        Subject = "[INFO] COMPANY STATUS",
                        Body = "Your company join request has been rejected due to malicious information"
                    });
                }
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
            

            var data = _adminService.LoadCompanyDataTable(sortColumnDirection, searchValue, skip, take);
            var recordsTotal = data.Count;

            return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data });
        }

        
    }
}