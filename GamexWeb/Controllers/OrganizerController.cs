using System;
using System.Threading.Tasks;
using System.Web.Configuration;
using GamexEntity.Constant;
using GamexService.ViewModel;
using System.Web.Mvc;
using Firebase.Storage;
using GamexWeb.Utilities;

namespace GamexWeb.Controllers
{
    [RoutePrefix("Organizer")]
    public class OrganizerController : Controller
    {
        // GET: Organizer

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

            }
            return Content("ahihi");
            
        }
    }
}