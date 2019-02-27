using GamexEntity.Constant;
using GamexEntity.Enumeration;
using GamexService.Interface;
using GamexService.ViewModel;
using GamexWeb.Identity;
using Microsoft.AspNet.Identity;
using System;
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
            TempData["RedirectCall"] = true;
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
    }
}