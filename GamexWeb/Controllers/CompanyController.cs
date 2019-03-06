using GamexEntity.Constant;
using GamexEntity.Enumeration;
using GamexService.Interface;
using GamexService.ViewModel;
using GamexWeb.Identity;
using GamexWeb.Utilities;
using Microsoft.AspNet.Identity;
using System;
using System.Linq;
using System.Web.Mvc;

namespace GamexWeb.Controllers
{
    public class CompanyController : Controller
    {
        private readonly ApplicationUserManager _userManager;
        private readonly IAccountService _accountService;
        private readonly ICompanyService _companyService;

        public CompanyController(ICompanyService companyService, ApplicationUserManager userManager, IAccountService accountService)
        {
            _companyService = companyService;
            _userManager = userManager;
            _accountService = accountService;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CheckBeforeRegistration(SelectCompanyViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("~/Views/Home/Register.cshtml", model);
            }
            var company = _companyService.SelectCompanyRegisterStatus(model);
            if (company != null) 
            {
                //Company already registered
                //First check is approved
                if (company.Status == (int) CompanyStatusEnum.Pending)
                {
                    //display popup tell user to wait until being approved
                    model.ErrorMessage = "Your company is registered but not approved yet";
                    return View("~/Views/Home/Register.cshtml", model);
                }
                if (company.Status == (int) CompanyStatusEnum.Active)
                {
                    //return to employee sign up page
                    return RedirectToAction("CompanyEmployeeRegister", "Company",
                        new {companyName = company.CompanyName, companyId = company.CompanyId});
                }
            }
            //return to company registration form
            return RedirectToAction("CompanyRegister", "Company");
        }

        [HttpGet]
        [AllowAnonymous]
        [FilterConfig.NoDirectAccess]
        [Route("Register/Employee")]
        public ActionResult CompanyEmployeeRegister(string companyName, string companyId)
        {
            //Display register form wth hidden company name and id
            var viewModel = new CompanyEmployeeRegisterViewModel
            {
                CompanyName = companyName,
                CompanyId = companyId
            };
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        [Route("Register/Employee")]
        public ActionResult CompanyEmployeeRegister(CompanyEmployeeRegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            //check if username is duplicate
            var isDuplicateUsername = _accountService.IsUsernameDuplicate(model.Username);
            if (isDuplicateUsername)
            {
                ModelState.AddModelError("Username", "Name " + model.Username + " is already taken");
                return View(model);
            }
            
            var user = new ApplicationUser
            {
                UserName = model.Username,
                Email = model.Email,
                FirstName = model.FirstName,
                LastName = model.LastName,
                Point = 0,
                TotalPointEarned = 0,
                CompanyId = model.CompanyId,
                StatusId = (int) AccountStatusEnum.Pending
            };
            var result =  _userManager.Create(user, model.Password);
            
            if (result.Succeeded)
            {
                //tao thanh cong
                var roleResult = _userManager.AddToRole(user.Id, AccountRole.Company);
                if (roleResult.Succeeded)
                {
                    var newModel = new CompanyEmployeeRegisterViewModel
                    {
                        IsSuccessful = true, 
                        CompanyId =  model.CompanyId,
                        CompanyName = model.CompanyName
                    };
                    ModelState.Clear();
                    return View(newModel);
                }
                model.IsSuccessful = false;
                _userManager.Delete(user);
                ModelState.AddModelError("", "Cannot submit your request. Please try again later.");
                return View(model);
            }
            foreach (var resultError in result.Errors)
            {
                ModelState.AddModelError("", resultError);
            }
            model.IsSuccessful = false;
            return View(model);
        }

        [HttpGet]
        [FilterConfig.NoDirectAccess]
        [Route("Register/Company")]
        public ActionResult CompanyRegister()
        {
            return View();
        }

        [HttpPost]
        [Route("Register/Company")]
        [ValidateAntiForgeryToken]
        public ActionResult CompanyRegister(CompanyRegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var isRegistered = _companyService.IsCompanyRegistered(model.TaxNumber);
            if (isRegistered)
            {
                model.IsSuccessful = false;
                model.ErrorMessage = "Company is already in our system";
                return View(model);
            }
            //check if email exist
            var isDuplicateUsername = _accountService.IsUsernameDuplicate(model.EmployeeEmail);
            if (isDuplicateUsername)
            {
                ModelState.AddModelError("EmployeeEmail", "Name " + model.EmployeeEmail + " is already taken");
                return View(model);
            }

            var companyId = Guid.NewGuid().ToString();                        
            var companyResult = _companyService.RegisterNewCompany(model, companyId);
            if (!companyResult)
            {
                model.IsSuccessful = false;
                model.ErrorMessage = "Cannot submit your registration. Please try again later";
                return View(model);
            }
            var user = new ApplicationUser
            {
                UserName = model.EmployeeEmail,
                Email = model.EmployeeEmail,
                FirstName = model.FirstName,
                LastName = model.LastName,
                Point = 0,
                TotalPointEarned = 0,
                CompanyId = companyId,
                StatusId = (int)AccountStatusEnum.Pending
            };
            var userResult = _userManager.Create(user);
            if (userResult.Succeeded)
            {
                var roleResult = _userManager.AddToRole(user.Id, AccountRole.Company);
                if (roleResult.Succeeded)
                {
                    model = new CompanyRegisterViewModel();
                    model.IsSuccessful = true;
                    ModelState.Clear();
                    //return success information;
                    return View(model);
                }
                _userManager.Delete(user);
                model.IsSuccessful = false;
                model.ErrorMessage = "Cannot submit your registration. Please try again later";
                return View(model);
            }
            model.IsSuccessful = false;
            _companyService.RemoveCompany(companyId);
            model.ErrorMessage = "";
            foreach (var error in userResult.Errors)
            {
                model.ErrorMessage += error;
            }
            return View(model);          
        }

        [HttpGet]
        [Authorize(Roles = AccountRole.Company)]
        [Route("Company/Exhibition/New")]
        public ActionResult NewExhibition()
        {
            return View();
        }

        [HttpPost]
        [Route("Company/LoadNewExhibitionList")]
        [Authorize(Roles = AccountRole.Company)]
        public ActionResult LoadNewExhibitionList()
        {
            var draw = Request.Form.GetValues("draw").FirstOrDefault();
            var start = Request.Form.GetValues("start").FirstOrDefault();
            var length = Request.Form.GetValues("length").FirstOrDefault();
            var sortColumnDirection = Request.Form.GetValues("order[0][dir]").FirstOrDefault();
            var searchValue = Request.Form.GetValues("search[value]").FirstOrDefault();
            var take = length != null ? Convert.ToInt32(length) : 0;
            var skip = start != null ? Convert.ToInt32(start) : 0;
            var data = _companyService.LoadNewExhibitionDataTable(sortColumnDirection, searchValue, skip, take, User.Identity.GetCompanyId());
            var recordsTotal = data.Count;
            return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data });
        }

        [HttpGet]
        [FilterConfig.NoDirectAccess]
        [Route("Company/Exhibition/View/{id}")]
        [Authorize(Roles = AccountRole.Company)]
        public ActionResult NewExhibitionDetail(string id)
        {
            //check if company join exhibition or not (validation)
            var join = _companyService.IsCompanyHasJoinExhibition(id, User.Identity.GetCompanyId());

            if (join)
            {
                return RedirectToAction("NewExhibition", "Company");
            }
            var detail = _companyService.GetExhibitionDetail(id);
            if (detail == null)
            {
                return RedirectToAction("NewExhibition", "Company");
            }

            if (TempData["RESULT"] != null)
            {
                detail.IsSuccessful = false;
            }
            return View(detail);
        }

        [HttpPost]
        [Route("Company/Exhibition/Join/")]
        [Authorize(Roles = AccountRole.Company)]
        public ActionResult JoinExhibition(string exhibitionId)
        {
            //TODO: check if exhibitionId modify and exhibition is Expired
            var join = _companyService.IsCompanyHasJoinExhibition(exhibitionId, User.Identity.GetCompanyId());

            if (join)
            {
                return RedirectToAction("NewExhibition", "Company");
            }
            //hasn't joined yet

            var joinResult = _companyService.JoinExhibition(exhibitionId, User.Identity.GetCompanyId());
            if (joinResult)
            {
                //successfully joined, redirect to manage my exhibition page
                return RedirectToAction("UpcomingExhibition", "Company");
            }
            TempData["RESULT"] = false;
            return RedirectToAction("NewExhibitionDetail", "Company", exhibitionId);
        }

        [HttpGet]
        [Authorize(Roles = AccountRole.Company)]
        [Route("Company/Exhibition/Upcoming")]
        public ActionResult UpcomingExhibition()
        {
            return View();
        }

        [HttpPost]
        [Route("Company/LoadUpcomingExhibitionList")]
        [Authorize(Roles = AccountRole.Company)]
        public ActionResult LoadUpcomingExhibitionList()
        {
            var draw = Request.Form.GetValues("draw").FirstOrDefault();
            var start = Request.Form.GetValues("start").FirstOrDefault();
            var length = Request.Form.GetValues("length").FirstOrDefault();
            var sortColumnDirection = Request.Form.GetValues("order[0][dir]").FirstOrDefault();
            var searchValue = Request.Form.GetValues("search[value]").FirstOrDefault();
            var take = length != null ? Convert.ToInt32(length) : 0;
            var skip = start != null ? Convert.ToInt32(start) : 0;
            var data = _companyService.LoadUpcomingExhibitionDataTable(sortColumnDirection, searchValue, skip, take, User.Identity.GetCompanyId());
            var recordsTotal = data.Count;
            return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data });
        }

        [HttpGet]
        [Authorize(Roles = AccountRole.Company)]
        [FilterConfig.NoDirectAccess]
        [Route("Company/Exhibition/{id}")]
        public ActionResult UpcomingExhibitionDetail(string id)
        {
            //check if company join exhibition or not (validation)
            var join = _companyService.IsCompanyHasJoinExhibition(id, User.Identity.GetCompanyId());

            if (!join)
            {
                return RedirectToAction("UpcomingExhibition", "Company");
            }
            var detail = _companyService.GetExhibitionDetail(id);
            if (detail == null)
            {
                return RedirectToAction("UpcomingExhibition", "Company");
            }

            if (TempData["RESULT"] != null)
            {
                detail.IsSuccessful = false;
            }
            return View(detail);
            
        }

        [HttpPost]
        [Route("Company/Exhibition/Quit/")]
        [Authorize(Roles = AccountRole.Company)]
        public ActionResult QuitExhibition(string exhibitionId)
        {
            //TODO: check if exhibitionId modify and exhibition is ongoing
            var join = _companyService.IsCompanyHasJoinExhibition(exhibitionId, User.Identity.GetCompanyId());

            if (!join)
            {
                return RedirectToAction("UpcomingExhibition", "Company");
            }
            var quitResult = _companyService.QuitExhibition(exhibitionId, User.Identity.GetCompanyId());
            if (quitResult)
            {
                //successfully quit, redirect to manage my exhibition page
                return RedirectToAction("UpcomingExhibition", "Company");
            }
            TempData["RESULT"] = false;
            return RedirectToAction("UpcomingExhibitionDetail", "Company", exhibitionId);
        }

        [HttpGet]
        [Route("Company/Exhibition/{id}/Survey/Create")]
        [FilterConfig.NoDirectAccess]
        [Authorize(Roles = AccountRole.Company)]
        public ActionResult CreateSurvey(string id)
        {
            var model = new CreateSurveyViewModel();
            model.ExhibitionId = id;
            ModelState.Clear();
            
            return View(model);
        }

        [HttpPost]
        [Route("Company/Exhibition/{id}/Survey/Create")]
        [Authorize(Roles = AccountRole.Company)]
        public ActionResult CreateSurvey(CreateSurveyViewModel model)
        {
            //TODO: check if exhibitionId modify and exhibition is ongoing or expired
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var addResult =
                _companyService.CreateSurvey(model, User.Identity.GetCompanyId(), User.Identity.GetUserId());
            if (addResult)
            {
                //add successfully
                model.IsSuccessful = true;
                model.Title = model.Description = "";
                ModelState.Clear();
                return View(model);
            }
            model.IsSuccessful = false;
            return View(model);
        }

        [HttpGet]
        [Route("Company/Exhibition/{id}/Survey/Manage")]
        [FilterConfig.NoDirectAccess]
        [Authorize(Roles = AccountRole.Company)]
        public ActionResult UpcomingSurvey(string id)
        {
            return View((object)id);
        }

        [HttpPost]
        [Route("Company/Exhibition/{id}/Survey/Manage")]
        [Authorize(Roles = AccountRole.Company)]
        public ActionResult LoadUpcomingSurveyList(string id)
        {
            var draw = Request.Form.GetValues("draw").FirstOrDefault();
            var start = Request.Form.GetValues("start").FirstOrDefault();
            var length = Request.Form.GetValues("length").FirstOrDefault();
            var sortColumnDirection = Request.Form.GetValues("order[0][dir]").FirstOrDefault();
            var searchValue = Request.Form.GetValues("search[value]").FirstOrDefault();
            var take = length != null ? Convert.ToInt32(length) : 0;
            var skip = start != null ? Convert.ToInt32(start) : 0;
            var data = _companyService.LoadUpcomingSurveyDataTable(sortColumnDirection, searchValue, skip, take, User.Identity.GetCompanyId(), id);
            var recordsTotal = data.Count;
            return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data });
        }
    }
}