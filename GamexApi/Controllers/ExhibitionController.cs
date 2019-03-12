using GamexApiService.Interface;
using GamexApiService.ViewModel;
using System.Collections.Generic;
using System.Web.Http;
using System.Web.Http.Results;
using GamexEntity.Constant;
using Microsoft.AspNet.Identity;

namespace GamexApi.Controllers {
    [Authorize(Roles = AccountRole.User)]
    [System.Web.Mvc.RequireHttps]
    [RoutePrefix("api")]
    public class ExhibitionController : ApiController {

        private readonly IExhibitionService _exhibitionService;

        public ExhibitionController(IExhibitionService exhibitionService) {
            _exhibitionService = exhibitionService;
        }

        // GET /exhibitions
        [HttpGet]
        [Route("exhibitions")]
        public List<ExhibitionShortViewModel> GetExhibitions(string type = ExhibitionTypes.Ongoing,
                int take = 5, int skip = 0, string lat = "0", string lng = "0") {

            var accountId = User.Identity.GetUserId();
            var exhibitionList = _exhibitionService.GetExhibitions(type, take, skip, lat, lng, accountId);
            return exhibitionList;
        }

        [HttpGet]
        [Route("exhibition")]
        public ExhibitionViewModel GetExhibition(string id) {
            return _exhibitionService.GetExhibition(id);
        }

        [HttpPost]
        [Route("exhibition/check-in")]
        public IHttpActionResult CheckInExhibition(ExhibitionCheckInBindingModel model) {
            var accountId = User.Identity.GetUserId();
            var result = _exhibitionService.CheckInExhibition(accountId, model.Id);

            if (result) {
                // TODO: add account activity for checking in event
                return Ok();
            }
            return BadRequest("Check-in failed! Please ensure the parameters have been passed correctly!");
        }
    }
}
