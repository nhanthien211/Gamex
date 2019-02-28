using GamexEntity.Constant;
using GamexService.ViewModel;
using GamexWeb.Utilities;
using System;
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
        [Route("Exhibition/Upcoming")]
        [Authorize(Roles = AccountRole.Organizer)]
        public ActionResult UpcomingExhibition()
        {
            return View();
        }
    }
}