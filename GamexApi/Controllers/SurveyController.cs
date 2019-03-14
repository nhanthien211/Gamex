using GamexApiService.Interface;
using GamexApiService.Models;
using GamexEntity.Constant;
using Microsoft.AspNet.Identity;
using System.Collections.Generic;
using System.Web.Http;

namespace GamexApi.Controllers
{
    [Authorize(Roles = AccountRole.User)]
    [System.Web.Mvc.RequireHttps]
    [RoutePrefix("api")]
    public class SurveyController : ApiController {
        private ISurveyService _surveyService;
        private IActivityHistoryService _activityService;
        private IAccountService _accountService;
        private ISurveyParticipationService _surveyParticipationService;

        public SurveyController(
            ISurveyService surveyService,
            IActivityHistoryService activityService,
            IAccountService accountService,
            ISurveyParticipationService surveyParticipationService) {
            _surveyService = surveyService;
            _accountService = accountService;
            _activityService = activityService;
            _surveyParticipationService = surveyParticipationService;
        }

        [HttpGet]
        [Route("surveys")]
        public List<SurveyShortViewModel> GetSurveys(string exhibitionId, string companyId) {
            var accountId = User.Identity.GetUserId();
            return _surveyService.GetSurveys(accountId, exhibitionId, companyId);
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
                var survey = _surveyService.GetSurvey(surveyAnswerModel.SurveyId);
                _activityService.AddActivity(accountId, "Completed survey " + survey.Title);
                _accountService.EarnPoint(accountId, survey.Point);
                _surveyParticipationService.CompleteSurvey(accountId, survey.SurveyId);
                return Ok(new { point = survey.Point});
            }

            return BadRequest("Submit survey answer failed!");
        }
    }
}
