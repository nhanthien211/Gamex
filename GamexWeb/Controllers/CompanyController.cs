using GamexService.Interface;
using GamexService.ViewModel;
using System.Web.Mvc;
using GamexEntity.Enumeration;

namespace GamexWeb.Controllers
{
    public class CompanyController : Controller
    {
        private readonly ICompanyService _companyService;

        public CompanyController(ICompanyService companyService)
        {
            _companyService = companyService;
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
                    model.ErrorMessage =
                        "Your company is registered but not approved yet. " +
                        "We will inform via your email when you're read";
                    return View("~/Views/Home/Register.cshtml", model);
                }
                //return to employee signup page
            }
            //return to company registration form
            return RedirectToAction("CompanyRegister", "Company");
        }

        [HttpGet]
        [Route("Register/Company")]
        public ActionResult CompanyRegister()
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("AccountInfo", "Account");
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
                return View("~/Views/Company/CompanyRegister.cshtml", model);
            }

            var isRegistered = _companyService.IsCompanyRegistered(model.TaxNumber);
            if (isRegistered)
            {
                model.ErrorMessage = "Company is already in our system";
                return View("~/Views/Company/CompanyRegister.cshtml", model);
            }

            var result = _companyService.RegisterNewCompany(model);
            if (!result)
            {
                model.ErrorMessage = "Cannot submit your registration. Please try again later";
                return View("~/Views/Company/CompanyRegister.cshtml", model);
            }

            //return success information;
            ModelState.Clear();
             return View("~/Views/Company/CompanyRegister.cshtml", model);
        }
    }
}