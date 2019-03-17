using GamexApiService.Interface;
using GamexApiService.Models;
using GamexEntity.Constant;
using Microsoft.AspNet.Identity;
using System.Collections.Generic;
using System.Web.Http;

namespace GamexApi.Controllers {
    [Authorize(Roles = AccountRole.User)]
    [System.Web.Mvc.RequireHttps]
    [RoutePrefix("api")]
    public class ExhibitionController : ApiController {

        private readonly IExhibitionService _exhibitionService;
        private readonly IActivityHistoryService _activityHistoryService;
        private readonly IAccountService _accountService;

        private const int CheckInExhibitionPoint = 100;

        public ExhibitionController(
            IExhibitionService exhibitionService,
            IActivityHistoryService activityHistoryService,
            IAccountService accountService
            ) {
            _exhibitionService = exhibitionService;
            _activityHistoryService = activityHistoryService;
            _accountService = accountService;
        }

        // GET /exhibitions
        [HttpGet]
        [Route("exhibitions")]
        public List<ExhibitionShortViewModel> GetExhibitions(string list = null, string type = ExhibitionTypes.Ongoing,
                int take = 5, int skip = 0, string lat = "0", string lng = "0") {

            var accountId = User.Identity.GetUserId();
            var exhibitionList = _exhibitionService.GetExhibitions(list, type, take, skip, lat, lng, accountId);
            return exhibitionList;
        }

        [HttpGet]
        [Route("exhibition")]
        public ExhibitionDetailViewModel GetExhibition(string id) {
            var accountId = User.Identity.GetUserId();
            return _exhibitionService.GetExhibition(accountId, id);
        }

        [HttpPost]
        [Route("user/exhibition")]
        public IHttpActionResult CheckInExhibition(ExhibitionCheckInBindingModel model) {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }

            var accountId = User.Identity.GetUserId();
            var result = _exhibitionService.CheckInExhibition(accountId, model.Id);
            if (result.Ok) {
                var exhibition = _exhibitionService.GetExhibition(accountId, model.Id);
                RecordActivity(accountId, "Checked in exhibition " + exhibition.Name);
                EarnPoint(accountId, CheckInExhibitionPoint);
                return Ok(new { point = CheckInExhibitionPoint, message = result.Message });
            }
            return BadRequest(result.Message);
        }

        private bool RecordActivity(string accountId, string activity) {
            return _activityHistoryService.AddActivity(accountId, activity);
        }

        private bool EarnPoint(string accountId, int point) {
            return _accountService.EarnPoint(accountId, point);
        }
    }
}
