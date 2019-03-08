using System;
using GamexApiService.Interface;
using GamexApiService.ViewModel;
using System.Collections.Generic;
using System.Web.Http;

namespace GamexApi.Controllers
{
//    [Authorize]
    [System.Web.Mvc.RequireHttps]
    [RoutePrefix("api")]
    public class ExhibitionController : ApiController
    {

        private readonly IExhibitionService _exhibitionService;

        public ExhibitionController(IExhibitionService exhibitionService)
        {
            _exhibitionService = exhibitionService;
        }

        // GET /exhibition
        [Route("exhibition")]
        public List<ExhibitionViewModel> GetExhibitions()
        {
            var exhibitionList = _exhibitionService.GetExhibitions();
            return exhibitionList;
        }

    }
}
