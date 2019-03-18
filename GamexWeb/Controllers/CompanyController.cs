using GamexEntity.Constant;
using GamexEntity.Enumeration;
using GamexService.Interface;
using GamexService.ViewModel;
using GamexWeb.Identity;
using GamexWeb.Utilities;
using Microsoft.AspNet.Identity;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using GamexEntity;

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
            var data = _companyService.LoadExhibitionDataTable(ExhibitionTypes.New, sortColumnDirection, searchValue, skip, take, User.Identity.GetCompanyId());
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
            var detail = _companyService.GetExhibitionDetail(id, ExhibitionTypes.New);
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
            var data = _companyService.LoadExhibitionDataTable(ExhibitionTypes.Upcoming, sortColumnDirection, searchValue, skip, take, User.Identity.GetCompanyId());
            var recordsTotal = data.Count;
            return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data });
        }

        [HttpGet]
        [Authorize(Roles = AccountRole.Company)]
        [FilterConfig.NoDirectAccess]
        [Route("Company/Exhibition/Upcoming/{id}")]
        public ActionResult UpcomingExhibitionDetail(string id)
        {
            //check if company join exhibition or not (validation)
            var companyId = User.Identity.GetCompanyId();
            var join = _companyService.IsCompanyHasJoinExhibition(id, companyId);

            if (!join)
            {
                return RedirectToAction("UpcomingExhibition", "Company");
            }
            var detail = _companyService.GetExhibitionDetail(id, ExhibitionTypes.Upcoming);
            if (detail == null)
            {
                return RedirectToAction("UpcomingExhibition", "Company");
            }

            detail.Booth = _companyService.GetCompanyBoothInExhibition(id, companyId);
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
        [Route("Company/Exhibition/Upcoming/{id}/Survey/Create")]
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
        [Route("Company/Exhibition/Upcoming/{id}/Survey/Create")]
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
        [Route("Company/Exhibition/Upcoming/{id}/Survey/Manage")]
        [FilterConfig.NoDirectAccess]
        [Authorize(Roles = AccountRole.Company)]
        public ActionResult UpcomingSurvey(string id)
        {
            return View((object)id);
        }

        [HttpPost]
        [Route("Company/Exhibition/Upcoming/{id}/Survey/Manage")]
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

        [HttpGet]
        [Route("Company/Exhibition/Upcoming/{exhibitionId}/Survey/{id}")]
        [FilterConfig.NoDirectAccess]
        [Authorize(Roles = AccountRole.Company)]
        public ActionResult UpcomingSurveyDetail(string id, string exhibitionId)
        {
            var survey = _companyService.GetUpcomingSurveyDetail(id);
            if (survey == null)
            {
                return RedirectToAction("UpcomingSurvey", "Company", exhibitionId);
            }
            survey.ExhibitionId = exhibitionId;
            return View(survey);
        }

        [HttpPost]
        [Route("Company/Exhibition/Upcoming/{exhibitionId}/Survey/{id}")]
        [Authorize(Roles = AccountRole.Company)]
        public ActionResult UpdateSurveyInformation(UpcomingSurveyDetailViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("~/Views/Company/UpcomingSurveyDetail.cshtml", model);
            }
            var updateResult =
                _companyService.UpdateSurveyInfo(model);
            if (updateResult)
            {
                //update successfully
                model.IsSuccessful = true;
                return View("~/Views/Company/UpcomingSurveyDetail.cshtml", model);
            }
            model.IsSuccessful = false;
            return View("~/Views/Company/UpcomingSurveyDetail.cshtml", model);
        }

        [HttpPost]
        [Authorize(Roles = AccountRole.Company)]
        public ActionResult DeleteSurvey(string surveyId, string exhibitionId)
        {
            _companyService.RemoveSurvey(surveyId);
            return RedirectToAction("UpcomingSurvey", "Company", new { id = exhibitionId});
        }

        [HttpGet]
        [Route("Company/Exhibition/Upcoming/{exhibitionId}/Survey/{id}/Question/Create")]
        [FilterConfig.NoDirectAccess]
        [Authorize(Roles = AccountRole.Company)]
        public ActionResult CreateQuestion(string id, string exhibitionId)
        {
            ViewBag.SurveyId = id;
            ViewBag.ExhibitionId = exhibitionId;
            return View();
        }

        [HttpPost]
        [Route("Company/Exhibition/Upcoming/{exhibitionId}/Survey/{id}/Question/Create")]
        [FilterConfig.NoDirectAccess]
        [Authorize(Roles = AccountRole.Company)]
        public ActionResult CreateQuestion(string questionTitle, string[] answers, string id, string questionType, string exhibitionId)
        {
            ViewBag.SurveyId = id;
            ViewBag.ExhibitionId = exhibitionId;
            var isValidParams = _companyService.ValidateQuestionCreateField(questionType, id, questionTitle, answers);
            if (!isValidParams)
            {
                ViewBag.IsSuccessful = false;
                return View();
            }
            //TODO: add question
            ViewBag.IsSuccessful = _companyService.AddQuestionAndAnswer(questionTitle, answers, id, questionType);
            return View();
        }

        [HttpPost]
        [Route("Company/TextQuestion")]
        [Authorize(Roles = AccountRole.Company)]
        public ActionResult TextQuestionPartialView()
        {
            return PartialView("~/Views/Partial/_TextQuestion.cshtml");
        }

        [HttpPost]
        [Route("Company/MultipleChoice")]
        [Authorize(Roles = AccountRole.Company)]
        public ActionResult MultipleChoicePartialView()
        {
            return PartialView("~/Views/Partial/_MultipleChoice.cshtml");
        }

        [HttpGet]
        [Route("Company/Exhibition/Upcoming/{exhibitionId}/Survey/{id}/Question/Manage")]
        [FilterConfig.NoDirectAccess]
        [Authorize(Roles = AccountRole.Company)]
        public ActionResult UpcomingSurveyQuestion(string id, string exhibitionId)
        {
            ViewBag.SurveyId = id;
            ViewBag.ExhibitionId = exhibitionId;
            return View();
        }

        [HttpPost]
        [Route("Company/Survey/{surveyId}/Question/Upcoming")]
        [Authorize(Roles = AccountRole.Company)]
        public ActionResult LoadUpcomingSurveyQuestion(string surveyId)
        {
            var draw = Request.Form.GetValues("draw").FirstOrDefault();
            var start = Request.Form.GetValues("start").FirstOrDefault();
            var length = Request.Form.GetValues("length").FirstOrDefault();
            var sortColumnDirection = Request.Form.GetValues("order[0][dir]").FirstOrDefault();
            var searchValue = Request.Form.GetValues("search[value]").FirstOrDefault();
            var take = length != null ? Convert.ToInt32(length) : 0;
            var skip = start != null ? Convert.ToInt32(start) : 0;
            var data = _companyService.LoadUpcomingSurveyQuestionDataTable(sortColumnDirection, searchValue, skip, take, surveyId);
            var recordsTotal = data.Count;
            return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data });
        }

        [HttpGet]
        [Authorize(Roles = AccountRole.Company)]
        [FilterConfig.NoDirectAccess]
        [Route("Company/Exhibition/Upcoming/{exhibitionId}/Survey/{surveyId}/Question/{questionId}/Type/{questionType}")]
        public ActionResult UpcomingSurveyQuestionDetail(string exhibitionId, string surveyId, string questionId, string questionType)
        {
            var question = _companyService.GetSurveyQuestionDetail(questionId, questionType);
            if (question == null)
            {
                return RedirectToAction("UpcomingSurveyQuestion", "Company",
                    new {exhibitionId = exhibitionId, id = surveyId});
            }
            question.ExhibitionId = exhibitionId;
            question.SurveyId = surveyId;
            return View(question);
        }

        [HttpPost]
        [Authorize(Roles = AccountRole.Company)]
        [Route("Company/Exhibition/Upcoming/{exhibitionId}/Survey/{surveyId}/Question/{questionId}/Update")]
        public ActionResult UpdateQuestionDetail(SurveyQuestionDetailViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("~/Views/Company/UpcomingSurveyQuestionDetail.cshtml", model);
            }
            model.IsSuccessful = _companyService.UpdateSurveyQuestionDetail(model);
            return View("~/Views/Company/UpcomingSurveyQuestionDetail.cshtml", model);
        }

        [HttpPost]
        [Authorize(Roles = AccountRole.Company)]
        public ActionResult DeleteQuestion(string questionId, string surveyId, string exhibitionId)
        {
            _companyService.RemoveQuestion(questionId);
            return RedirectToAction("UpcomingSurveyQuestion", "Company", 
                new { exhibitionId = exhibitionId, id = surveyId});
        }

        [HttpGet]
        [Authorize(Roles = AccountRole.Company)]
        [Route("Company/Info")]
        public ActionResult CompanyInfo()
        {
            string companyId = User.Identity.GetCompanyId();
            var profile = _companyService.GetCompanyProfile(companyId);
            if (TempData["RESULT"] != null)
            {
                profile.IsSuccessful = (bool)TempData["RESULT"];
            }
            return View(profile);
        }

        [HttpPost]
        [Authorize(Roles = AccountRole.Company)]
        [Route("Company/Info")]
        public async Task<ActionResult> CompanyInfo(CompanyProfileViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            string companyId = User.Identity.GetCompanyId();
            if (model.Logo != null)
            {
                model.ImageUrl =
                    await FirebaseUploadUtility.UploadImageToFirebase(model.Logo.InputStream, companyId);
            }
            if (!string.IsNullOrEmpty(model.ImageUrl))
            {
                TempData["RESULT"] = _companyService.UpdateCompanyProfile(model, companyId);
            }
            else
            {
                TempData["RESULT"] = false;
            }
            return RedirectToAction("CompanyInfo", "Company");
        }

        [HttpGet]
        [Authorize(Roles = AccountRole.Company)]
        [Route("Company/Employee/Join")]
        public ActionResult EmployeeJoinList()
        {
            return View();
        }

        [HttpGet]
        [Authorize(Roles = AccountRole.Company)]
        [Route("Company/Exhibition/Ongoing")]
        public ActionResult OngoingExhibition()
        {
            return View();
        }

        [HttpPost]
        [Route("Company/LoadOngoingExhibitionList")]
        [Authorize(Roles = AccountRole.Company)]
        public ActionResult LoadOngoingExhibitionList()
        {
            var draw = Request.Form.GetValues("draw").FirstOrDefault();
            var start = Request.Form.GetValues("start").FirstOrDefault();
            var length = Request.Form.GetValues("length").FirstOrDefault();
            var sortColumnDirection = Request.Form.GetValues("order[0][dir]").FirstOrDefault();
            var searchValue = Request.Form.GetValues("search[value]").FirstOrDefault();
            var take = length != null ? Convert.ToInt32(length) : 0;
            var skip = start != null ? Convert.ToInt32(start) : 0;
            var data = _companyService.LoadExhibitionDataTable(ExhibitionTypes.Ongoing, sortColumnDirection, searchValue, skip, take, User.Identity.GetCompanyId());
            var recordsTotal = data.Count;
            return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data });
        }

        [HttpGet]
        [Authorize(Roles = AccountRole.Company)]
        [FilterConfig.NoDirectAccess]
        [Route("Company/Exhibition/Ongoing/{id}")]
        public ActionResult OngoingExhibitionDetail(string id)
        {
            //check if company join exhibition or not (validation)
            var companyId = User.Identity.GetCompanyId();
            var join = _companyService.IsCompanyHasJoinExhibition(id, companyId);

            if (!join)
            {
                return RedirectToAction("OngoingExhibition", "Company");
            }
            var detail = _companyService.GetExhibitionDetail(id, ExhibitionTypes.Ongoing);
            if (detail == null)
            {
                return RedirectToAction("OngoingExhibition", "Company");
            }

            detail.Booth = _companyService.GetCompanyBoothInExhibition(id, companyId);
            return View(detail);
        }
    }
}