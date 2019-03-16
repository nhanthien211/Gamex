using GamexEntity.Constant;
using GamexService.ViewModel;
using GamexWeb.Utilities;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using GamexService.Interface;
using Microsoft.AspNet.Identity;

namespace GamexWeb.Controllers
{
    [RoutePrefix("Organizer")]
    public class OrganizerController : Controller
    {
        private readonly IOrganizerService _organizerService;
        // GET: Organizer

        public OrganizerController(IOrganizerService organizerService)
        {
            _organizerService = organizerService;
        }

        [HttpGet]
        [Authorize(Roles = AccountRole.Organizer)]
        [Route("Exhibition/Create")]
        public ActionResult CreateExhibition()
        {
            return View();
        }

        [HttpPost]
        [Route("Exhibition/Create")]
        [Authorize(Roles = AccountRole.Organizer)]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateExhibition(CreateExhibitionViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var exhibitionCode = Guid.NewGuid().ToString();
            var uploadUrl = await FirebaseUploadUtility.UploadImageToFirebase(model.Logo.InputStream, exhibitionCode);
            if (!string.IsNullOrEmpty(uploadUrl))
            {
                //successful upload
                //create exhibition
                var result = _organizerService.CreateExhibition(model, exhibitionCode, uploadUrl, User.Identity.GetUserId());
                if (result)
                {
                    //create successfully
                    model = new CreateExhibitionViewModel();
                    ModelState.Clear();
                    model.IsSuccessful = true;
                }
                else
                {
                    model.IsSuccessful = false;
                }
            }
            return View(model); ;
        }

        [HttpGet]
        [Authorize(Roles = AccountRole.Organizer)]
        [Route("Exhibition/Upcoming")]
        public ActionResult UpcomingExhibition()
        {
            return View();
        }

        [HttpPost]
        [Route("LoadUpcomingExhibitionList")]
        [Authorize(Roles = AccountRole.Organizer)]
        public ActionResult LoadUpcomingExhibitionList()
        {
            var draw = Request.Form.GetValues("draw").FirstOrDefault();
            var start = Request.Form.GetValues("start").FirstOrDefault();
            var length = Request.Form.GetValues("length").FirstOrDefault();
            var sortColumnDirection = Request.Form.GetValues("order[0][dir]").FirstOrDefault();
            var searchValue = Request.Form.GetValues("search[value]").FirstOrDefault();
            var take = length != null ? Convert.ToInt32(length) : 0;
            var skip = start != null ? Convert.ToInt32(start) : 0;
            var data = _organizerService.LoadUpcomingExhibitionDataTable(sortColumnDirection, searchValue, skip, take, User.Identity.GetUserId());
            var recordsTotal = data.Count;
            return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data });
        }

        [HttpGet]
        [Authorize(Roles = AccountRole.Organizer)]
        [FilterConfig.NoDirectAccess]
        [Route("Exhibition/Upcoming/{id}")]
        public ActionResult UpcomingExhibitionDetail(string id)
        {
            var detail = _organizerService.GetExhibitionDetail(id);
            if (detail == null)
            {
                return RedirectToAction("UpcomingExhibition", "Organizer");
            }
            return View(detail);
        }

        [HttpPost]
        [Authorize(Roles = AccountRole.Organizer)]
        [Route("Exhibition/Upcoming/{id}/Upcoming")]
        public async Task<ActionResult> UpdateUpcomingExhibitionDetail(ExhibitionDetailViewModel model) 
        {
            if (!ModelState.IsValid)
            {
                return View("~/Views/Organizer/UpcomingExhibitionDetail.cshtml", model);
            }

            if (model.Logo != null)
            {
                model.ImageUrl =
                    await FirebaseUploadUtility.UploadImageToFirebase(model.Logo.InputStream, model.ExhibitionId);
            }

            if (!string.IsNullOrEmpty(model.ImageUrl))
            {
                model.IsSuccessful = _organizerService.UpdateExhibitionDetail(model);
            }
            else
            {
                model.IsSuccessful = false;
            }
            return View("~/Views/Organizer/UpcomingExhibitionDetail.cshtml", model);
        }

        [HttpGet]
        [Authorize(Roles = AccountRole.Organizer)]
        [FilterConfig.NoDirectAccess]
        [Route("Exhibition/Upcoming/{id}/Company")]
        public ActionResult UpcomingExhibitionCompanyList(string id)
        {
            ViewBag.ExhibitionId = id;
            return View();
        }

        [HttpPost]
        [Authorize(Roles = AccountRole.Organizer)]
        [Route("LoadAttendedCompanyList/{id}")]
        public ActionResult LoadAttendedCompany(string id)
        {
            var draw = Request.Form.GetValues("draw").FirstOrDefault();
            var start = Request.Form.GetValues("start").FirstOrDefault();
            var length = Request.Form.GetValues("length").FirstOrDefault();
            var sortColumnDirection = Request.Form.GetValues("order[0][dir]").FirstOrDefault();
            var searchValue = Request.Form.GetValues("search[value]").FirstOrDefault();
            var take = length != null ? Convert.ToInt32(length) : 0;
            var skip = start != null ? Convert.ToInt32(start) : 0;
            var data = _organizerService.LoadAttendedCompanyList(sortColumnDirection, searchValue, skip, take, id);
            var recordsTotal = data.Count;
            return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data });
        }

        [HttpGet]
        [Authorize(Roles = AccountRole.Organizer)]
        [FilterConfig.NoDirectAccess]
        [Route("Exhibition/Upcoming/{exhibitionId}/Company/{companyId}")]
        public ActionResult AttendedCompanyDetail(string exhibitionId, string companyId)
        {
            var company = _organizerService.GetAttendedCompanyDetail(exhibitionId, companyId);
            if (TempData["RESULT"] != null)
            {
                company.IsSuccessful = (bool) TempData["RESULT"];
            }
            return View(company);
        }

        [HttpPost]
        [Authorize(Roles = AccountRole.Organizer)]
        [Route("Exhibition/Upcoming/{exhibitionId}/Company/{companyId}")]
        public ActionResult AssignBooth(AttendedCompanyDetailViewModel model, string exhibitionId, string companyId)
        {
            if (!ModelState.IsValid)
            {
                TempData["RESULT"] = false;
                return RedirectToAction("AttendedCompanyDetail", "Organizer", new { exhibitionId = exhibitionId, companyId = companyId });
            }
            TempData["RESULT"] = _organizerService.AssignBoothToCompany(model, exhibitionId, companyId);
            return RedirectToAction("AttendedCompanyDetail", "Organizer", new {exhibitionId = exhibitionId, companyId = companyId});
        }
    }
}