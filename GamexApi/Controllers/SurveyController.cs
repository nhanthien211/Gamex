using System.Collections.Generic;
using System.Web.Http;
using GamexApiService.Interface;
using GamexApiService.Models;
using GamexEntity.Constant;
using Microsoft.AspNet.Identity;

namespace GamexApi.Controllers {
    [Authorize(Roles = AccountRole.User)]
    [System.Web.Mvc.RequireHttps]
    [RoutePrefix("api")]
    public class SurveyController : ApiController {
        private ISurveyService _surveyService;

        public SurveyController(ISurveyService surveyService) {
            _surveyService = surveyService;
        }

        [HttpGet]
        [Route("surveys")]
        public List<SurveyShortViewModel> GetSurveys(string exhibitionId, string companyId) {
            return _surveyService.GetSurveys(exhibitionId, companyId);
        }

        [HttpGet]
        [Route("survey")]
        public SurveyDetailViewModel GetSurvey(int id) {
            return _surveyService.GetSurvey(id);
        }

        [HttpPost]
        [Route("survey")]
        public IHttpActionResult SubmitSurvey(SurveyAnswerBindingModel surveyAnswerModel) {
            var accountId = User.Identity.GetUserId();
            var result = _surveyService.SubmitSurvey(accountId, surveyAnswerModel);
            if (result) {
                return Ok();
            }

            return BadRequest("Submit survey answer failed!");
        }
    }
}
