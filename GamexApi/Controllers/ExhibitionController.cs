using System;
using GamexApiService.Interface;
using GamexApiService.ViewModel;
using System.Collections.Generic;
using System.Web.Http;
using System.Web.Http.Results;
using GamexEntity.Constant;
using Microsoft.AspNet.Identity;

namespace GamexApi.Controllers {
    //    [Authorize]
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
            int take = 5, int skip = 0) {

            var accountId = User.Identity.GetUserId();
            var exhibitionList = _exhibitionService.GetExhibitions(type, take, skip, accountId);
            return exhibitionList;
        }
        
        [HttpGet]
        [Route("exhibition")]
        public ExhibitionViewModel GetExhibition(string id) {
            return _exhibitionService.GetExhibition(id);
        }
    }
}
