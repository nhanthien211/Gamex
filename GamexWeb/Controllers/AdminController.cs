using GamexEntity.Constant;
using GamexService.Interface;
using GamexWeb.Identity;
using GamexWeb.Utilities;
using Microsoft.AspNet.Identity;
using System;
using System.Linq;
using System.Web.Mvc;
using GamexEntity.Enumeration;
using GamexService.Utilities;
using GamexService.ViewModel;

namespace GamexWeb.Controllers
{
    [RoutePrefix("Admin")]
    public class AdminController : Controller
    {
        private readonly IIdentityMessageService _emailService;
        private readonly IAdminService _adminService;
        private readonly IAccountService _accountService;
        private readonly ApplicationUserManager _userManager;
        

        public AdminController(IIdentityMessageService emailService, IAdminService adminService, IAccountService accountService, ApplicationUserManager userManager)
        {
            _emailService = emailService;
            _adminService = adminService;
            _accountService = accountService;
            _userManager = userManager;
        }

        // GET: Admin
        [HttpGet]
        [Route("Company/Request")]
        [Authorize(Roles = AccountRole.Admin)]
        public ActionResult CompanyRequest()
        {
            return View();
        }

        [HttpPost]
        [Route("LoadCompanyRequest")]
        [Authorize(Roles = AccountRole.Admin)]
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
        [Authorize(Roles = AccountRole.Admin)]
        public ActionResult ApproveOrDeny(string companyId, bool isApproved)
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
        [Route("Company/List")]
        [Authorize(Roles = AccountRole.Admin)]
        public ActionResult CompanyList()
        {
            return View();
        }

        [HttpPost]
        [Route("LoadCompanyList")]
        [Authorize(Roles = AccountRole.Admin)]
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

        [HttpGet]
        [Route("Organizer/List")]
        [Authorize(Roles = AccountRole.Admin)]
        public ActionResult OrganizerList()
        {
            return View();
        }

        [HttpPost]
        [Route("LoadOrganizerList")]
        [Authorize(Roles = AccountRole.Admin)]
        public ActionResult LoadOrganizerList()
        {
            var draw = Request.Form.GetValues("draw").FirstOrDefault();
            var start = Request.Form.GetValues("start").FirstOrDefault();
            var length = Request.Form.GetValues("length").FirstOrDefault();
            var sortColumnDirection = Request.Form.GetValues("order[0][dir]").FirstOrDefault();
            var searchValue = Request.Form.GetValues("search[value]").FirstOrDefault();
            var take = length != null ? Convert.ToInt32(length) : 0;
            var skip = start != null ? Convert.ToInt32(start) : 0;
            var data = _adminService.LoadOrganizerDataTable(sortColumnDirection, searchValue, skip, take);
            var recordsTotal = data.Count;
            return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data });
        }

        [HttpGet]
        [Route("Organizer/Create")]
        [Authorize(Roles = AccountRole.Admin)]
        public ActionResult CreateOrganizer()
        {
            return View();
        }

        [HttpPost]
        [Route("Organizer/Create")]
        [Authorize(Roles = AccountRole.Admin)]
        [ValidateAntiForgeryToken]
        public ActionResult CreateOrganizer(CreateOrganizerViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            //check email
            var isDuplicate = _accountService.IsUsernameDuplicate(model.Email);
            if (isDuplicate)
            {
                ModelState.AddModelError("Email", "Email is already taken");
                return View(model);
            }
            var user = new ApplicationUser
            {
                Email = model.Email,
                UserName = model.Email,
                FirstName = model.FirstName,
                LastName = model.LastName,
                Point = 0,
                TotalPointEarned = 0,
                StatusId = (int) AccountStatusEnum.Active
            };
            var randomPassword = MyUtilities.GenerateRandomPassword();
            var createResult = _userManager.Create(user, randomPassword);

            if (createResult.Succeeded)
            {
                //success
                //add to role
                var roleResult = _userManager.AddToRole(user.Id, AccountRole.Organizer);
                if (roleResult.Succeeded)
                {
                    //send mail with password
                    _emailService.Send(new IdentityMessage
                    {
                        Destination = user.Email,
                        Subject = "[INFO] ORGANIZER LOGIN ACCOUNT",
                        Body = "Please login with the following information: " +
                               "Username/Email: " + user.Email + " " +
                               "Password: " + randomPassword
                    });
                    return RedirectToAction("OrganizerList", "Admin");
                }
                _userManager.Delete(user);
                ModelState.AddModelError("ErrorMessage", "Cannot create account right now. Please try again later");
                return View(model);
            }
            var message = "";
            foreach (var error in createResult.Errors)
            {
                message += error;
            }
            ModelState.AddModelError("ErrorMessage", message);
            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = AccountRole.Admin)]
        public ActionResult ActivateOrDeactivateAccount(string userId, bool isActivate)
        {
            var user = _userManager.FindById(userId);
            if (user != null)
            {
                switch (isActivate)
                {
                    case true:
                        user.StatusId = (int) AccountStatusEnum.Active;
                        _userManager.Update(user);
                        break;;
                    case false:
                        user.StatusId = (int)AccountStatusEnum.Deactive;
                        _userManager.Update(user);
                        //sign out that user
                        //also check OnValidateIdentity in Startup.Auth and set Timespan to 1 secs for 
                        //immediately sign out
                        _userManager.UpdateSecurityStamp(user.Id);
                        break;
                }
            }
            return RedirectToAction("OrganizerList", "Admin");
        }
    }
}