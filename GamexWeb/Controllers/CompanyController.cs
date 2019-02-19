using System.Linq;
using GamexService.Interface;
using GamexService.ViewModel;
using System.Web.Mvc;
using GamexEntity.Constant;
using GamexEntity.Enumeration;
using GamexWeb.Identity;
using GamexWeb.Models;
using Microsoft.Ajax.Utilities;
using Microsoft.AspNet.Identity;

namespace GamexWeb.Controllers
{
    public class CompanyController : Controller
    {
        private readonly ApplicationUserManager _userManager;
        private readonly ICompanyService _companyService;

        public CompanyController(ICompanyService companyService, ApplicationUserManager userManager)
        {
            _companyService = companyService;
            _userManager = userManager;
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
                    model.ErrorMessage =
                        "Your company is registered but not approved yet. " +
                        "We will inform via your email when you're read";
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
        [Route("Register/Employee")]
        public ActionResult CompanyEmployeeRegister(string companyName, int? companyId)
        {
            if (TempData["RedirectCall"] == null)
            {
                return RedirectToAction("Register", "Home");
            }
            //Display register form wth hidden company name and id
            var viewModel = new CompanyEmployeeRegisterViewModel
            {
                CompanyName = companyName,
                CompanyId = companyId.GetValueOrDefault()
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
                var roleResult = _userManager.AddToRole(user.Id, UserRole.Company);
                if (roleResult.Succeeded)
                {
                    model = new CompanyEmployeeRegisterViewModel();
                    model.IsSuccessful = true;
                    ModelState.Clear();
                    return View(model);
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
        [Route("Register/Company")]
        public ActionResult CompanyRegister()
        {
            if (TempData["RedirectCall"] == null)
            {
                return RedirectToAction("Register", "Home");
            }
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
                model.ErrorMessage = "Company is already in our system";
                return View(model);
            }

            var result = _companyService.RegisterNewCompany(model);
            if (!result)
            {
                model.ErrorMessage = "Cannot submit your registration. Please try again later";
                return View(model);
            }
            model = new CompanyRegisterViewModel();
            ModelState.Clear();
            //return success information;
            return View(model);
        }

    }
}